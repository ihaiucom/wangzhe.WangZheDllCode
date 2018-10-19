using PigeonCoopToolkit.Utillities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	public abstract class TrailRenderer_Base : MonoBehaviour
	{
		public PCTrailRendererData TrailData;

		public bool Emit;

		protected bool _emit;

		protected bool _noDecay;

		private PCTrail _activeTrail;

		private ListView<PCTrail> _fadingTrails;

		protected Transform _t;

		private static DictionaryView<Material, ListView<PCTrail>> _matToTrailList;

		private static ListView<Mesh> _toClean;

		private static bool _hasRenderer;

		private static int GlobalTrailRendererCount;

		protected virtual void Awake()
		{
			TrailRenderer_Base.GlobalTrailRendererCount++;
			if (TrailRenderer_Base.GlobalTrailRendererCount == 1)
			{
				TrailRenderer_Base._matToTrailList = new DictionaryView<Material, ListView<PCTrail>>();
				TrailRenderer_Base._toClean = new ListView<Mesh>();
			}
			this._fadingTrails = new ListView<PCTrail>();
			this._t = base.transform;
			this._emit = this.Emit;
			if (this._emit)
			{
				this._activeTrail = new PCTrail(this.GetMaxNumberOfPoints());
				this._activeTrail.IsActiveTrail = true;
				this.OnStartEmit();
			}
		}

		protected virtual void Start()
		{
		}

		protected virtual void LateUpdate()
		{
			if (TrailRenderer_Base._hasRenderer)
			{
				return;
			}
			TrailRenderer_Base._hasRenderer = true;
			foreach (KeyValuePair<Material, ListView<PCTrail>> current in TrailRenderer_Base._matToTrailList)
			{
				CombineInstance[] array = new CombineInstance[current.Value.Count];
				for (int i = 0; i < current.Value.Count; i++)
				{
					array[i] = new CombineInstance
					{
						mesh = current.Value[i].Mesh,
						subMeshIndex = 0,
						transform = Matrix4x4.identity
					};
				}
				Mesh mesh = new Mesh();
				mesh.CombineMeshes(array, true, false);
				TrailRenderer_Base._toClean.Add(mesh);
				this.DrawMesh(mesh, current.Key);
				current.Value.Clear();
			}
		}

		protected virtual void Update()
		{
			if (TrailRenderer_Base._hasRenderer)
			{
				TrailRenderer_Base._hasRenderer = false;
				if (TrailRenderer_Base._toClean.Count > 0)
				{
					for (int i = 0; i < TrailRenderer_Base._toClean.Count; i++)
					{
						Mesh mesh = TrailRenderer_Base._toClean[i];
						if (mesh != null)
						{
							if (Application.isEditor)
							{
								UnityEngine.Object.DestroyImmediate(mesh, true);
							}
							else
							{
								UnityEngine.Object.Destroy(mesh);
							}
						}
					}
				}
				TrailRenderer_Base._toClean.Clear();
			}
			if (!TrailRenderer_Base._matToTrailList.ContainsKey(this.TrailData.TrailMaterial))
			{
				TrailRenderer_Base._matToTrailList.Add(this.TrailData.TrailMaterial, new ListView<PCTrail>());
			}
			if (this._activeTrail != null)
			{
				this.UpdatePoints(this._activeTrail, Time.deltaTime);
				this.UpdateTrail(this._activeTrail, Time.deltaTime);
				this.GenerateMesh(this._activeTrail);
				TrailRenderer_Base._matToTrailList[this.TrailData.TrailMaterial].Add(this._activeTrail);
			}
			for (int j = this._fadingTrails.Count - 1; j >= 0; j--)
			{
				if (this._fadingTrails[j] == null || !this.AnyElement(this._fadingTrails[j].Points))
				{
					if (this._fadingTrails[j] != null)
					{
						this._fadingTrails[j].Dispose();
					}
					this._fadingTrails.RemoveAt(j);
				}
				else
				{
					this.UpdatePoints(this._fadingTrails[j], Time.deltaTime);
					this.UpdateTrail(this._fadingTrails[j], Time.deltaTime);
					this.GenerateMesh(this._fadingTrails[j]);
					TrailRenderer_Base._matToTrailList[this.TrailData.TrailMaterial].Add(this._fadingTrails[j]);
				}
			}
			this.CheckEmitChange();
		}

		protected bool AnyElement(CircularBuffer<PCTrailPoint> InPoints)
		{
			for (int i = 0; i < InPoints.Count; i++)
			{
				PCTrailPoint pCTrailPoint = InPoints[i];
				if (pCTrailPoint.TimeActive() < this.TrailData.Lifetime)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void OnDestroy()
		{
			TrailRenderer_Base.GlobalTrailRendererCount--;
			if (TrailRenderer_Base.GlobalTrailRendererCount == 0)
			{
				if (TrailRenderer_Base._toClean != null && TrailRenderer_Base._toClean.Count > 0)
				{
					for (int i = 0; i < TrailRenderer_Base._toClean.Count; i++)
					{
						Mesh mesh = TrailRenderer_Base._toClean[i];
						if (mesh != null)
						{
							if (Application.isEditor)
							{
								UnityEngine.Object.DestroyImmediate(mesh, true);
							}
							else
							{
								UnityEngine.Object.Destroy(mesh);
							}
						}
					}
				}
				TrailRenderer_Base._toClean = null;
				TrailRenderer_Base._matToTrailList.Clear();
				TrailRenderer_Base._matToTrailList = null;
			}
			if (this._activeTrail != null)
			{
				this._activeTrail.Dispose();
				this._activeTrail = null;
			}
			if (this._fadingTrails != null)
			{
				for (int j = 0; j < this._fadingTrails.Count; j++)
				{
					PCTrail pCTrail = this._fadingTrails[j];
					if (pCTrail != null)
					{
						pCTrail.Dispose();
					}
				}
				this._fadingTrails.Clear();
			}
		}

		protected virtual void OnStopEmit()
		{
		}

		protected virtual void OnStartEmit()
		{
		}

		protected virtual void OnTranslate(Vector3 t)
		{
		}

		protected abstract int GetMaxNumberOfPoints();

		protected virtual void Reset()
		{
			if (this.TrailData == null)
			{
				this.TrailData = new PCTrailRendererData();
			}
			this.TrailData.Lifetime = 1f;
			this.TrailData.UsingSimpleColor = false;
			this.TrailData.UsingSimpleSize = false;
			this.TrailData.ColorOverLife = new Gradient();
			this.TrailData.SimpleColorOverLifeStart = Color.white;
			this.TrailData.SimpleColorOverLifeEnd = new Color(1f, 1f, 1f, 0f);
			this.TrailData.SizeOverLife = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 1f),
				new Keyframe(1f, 0f)
			});
			this.TrailData.SimpleSizeOverLifeStart = 1f;
			this.TrailData.SimpleSizeOverLifeEnd = 0f;
		}

		protected virtual void InitialiseNewPoint(PCTrailPoint newPoint)
		{
		}

		protected virtual void UpdateTrail(PCTrail trail, float deltaTime)
		{
		}

		protected void AddPoint(PCTrailPoint newPoint, Vector3 pos)
		{
			if (this._activeTrail == null)
			{
				return;
			}
			newPoint.Position = pos;
			newPoint.PointNumber = ((this._activeTrail.Points.Count != 0) ? (this._activeTrail.Points[this._activeTrail.Points.Count - 1].PointNumber + 1) : 0);
			this.InitialiseNewPoint(newPoint);
			newPoint.SetDistanceFromStart((this._activeTrail.Points.Count != 0) ? (this._activeTrail.Points[this._activeTrail.Points.Count - 1].GetDistanceFromStart() + Vector3.Distance(this._activeTrail.Points[this._activeTrail.Points.Count - 1].Position, pos)) : 0f);
			if (this.TrailData.UseForwardOverride)
			{
				newPoint.Forward = ((!this.TrailData.ForwardOverrideRelative) ? this.TrailData.ForwardOverride.normalized : this._t.TransformDirection(this.TrailData.ForwardOverride.normalized));
			}
			this._activeTrail.Points.Add(newPoint);
		}

		private void GenerateMesh(PCTrail trail)
		{
			trail.Mesh.Clear(false);
			Vector3 vector = (!(Camera.main != null)) ? Vector3.forward : Camera.main.transform.forward;
			if (this.TrailData.UseForwardOverride)
			{
				vector = this.TrailData.ForwardOverride.normalized;
			}
			trail.activePointCount = this.NumberOfActivePoints(trail);
			if (trail.activePointCount < 2)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < trail.Points.Count; i++)
			{
				PCTrailPoint pCTrailPoint = trail.Points[i];
				float num2 = pCTrailPoint.TimeActive() / this.TrailData.Lifetime;
				if (pCTrailPoint.TimeActive() <= this.TrailData.Lifetime)
				{
					if (this.TrailData.UseForwardOverride && this.TrailData.ForwardOverrideRelative)
					{
						vector = pCTrailPoint.Forward;
					}
					Vector3 a = Vector3.zero;
					if (i < trail.Points.Count - 1)
					{
						a = Vector3.Cross((trail.Points[i + 1].Position - pCTrailPoint.Position).normalized, vector).normalized;
					}
					else
					{
						a = Vector3.Cross((pCTrailPoint.Position - trail.Points[i - 1].Position).normalized, vector).normalized;
					}
					Color color = (!this.TrailData.StretchColorToFit) ? ((!this.TrailData.UsingSimpleColor) ? this.TrailData.ColorOverLife.Evaluate(num2) : Color.Lerp(this.TrailData.SimpleColorOverLifeStart, this.TrailData.SimpleColorOverLifeEnd, num2)) : ((!this.TrailData.UsingSimpleColor) ? this.TrailData.ColorOverLife.Evaluate(1f - (float)num / (float)trail.activePointCount / 2f) : Color.Lerp(this.TrailData.SimpleColorOverLifeStart, this.TrailData.SimpleColorOverLifeEnd, 1f - (float)num / (float)trail.activePointCount / 2f));
					float d = (!this.TrailData.StretchSizeToFit) ? ((!this.TrailData.UsingSimpleSize) ? this.TrailData.SizeOverLife.Evaluate(num2) : Mathf.Lerp(this.TrailData.SimpleSizeOverLifeStart, this.TrailData.SimpleSizeOverLifeEnd, num2)) : ((!this.TrailData.UsingSimpleSize) ? this.TrailData.SizeOverLife.Evaluate(1f - (float)num / (float)trail.activePointCount / 2f) : Mathf.Lerp(this.TrailData.SimpleSizeOverLifeStart, this.TrailData.SimpleSizeOverLifeEnd, 1f - (float)num / (float)trail.activePointCount / 2f));
					trail.verticies[num] = pCTrailPoint.Position + a * d;
					if (this.TrailData.MaterialTileLength <= 0f)
					{
						trail.uvs[num] = new Vector2((float)num / (float)trail.activePointCount / 2f, 0f);
					}
					else
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / this.TrailData.MaterialTileLength, 0f);
					}
					trail.normals[num] = vector;
					trail.colors[num] = color;
					num++;
					trail.verticies[num] = pCTrailPoint.Position - a * d;
					if (this.TrailData.MaterialTileLength <= 0f)
					{
						trail.uvs[num] = new Vector2((float)num / (float)trail.activePointCount / 2f, 1f);
					}
					else
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / this.TrailData.MaterialTileLength, 1f);
					}
					trail.normals[num] = vector;
					trail.colors[num] = color;
					num++;
				}
			}
			Vector2 v = trail.verticies[num - 1];
			for (int j = num; j < trail.verticies.Length; j++)
			{
				trail.verticies[j] = v;
			}
			int num3 = 0;
			for (int k = 0; k < 2 * (trail.activePointCount - 1); k++)
			{
				if (k % 2 == 0)
				{
					trail.indicies[num3] = k;
					num3++;
					trail.indicies[num3] = k + 1;
					num3++;
					trail.indicies[num3] = k + 2;
				}
				else
				{
					trail.indicies[num3] = k + 2;
					num3++;
					trail.indicies[num3] = k + 1;
					num3++;
					trail.indicies[num3] = k;
				}
				num3++;
			}
			int num4 = trail.indicies[num3 - 1];
			for (int l = num3; l < trail.indicies.Length; l++)
			{
				trail.indicies[l] = num4;
			}
			trail.Mesh.vertices = trail.verticies;
			trail.Mesh.SetIndices(trail.indicies, MeshTopology.Triangles, 0);
			trail.Mesh.uv = trail.uvs;
			trail.Mesh.normals = trail.normals;
			trail.Mesh.colors = trail.colors;
		}

		private void DrawMesh(Mesh trailMesh, Material trailMaterial)
		{
			Graphics.DrawMesh(trailMesh, Matrix4x4.identity, trailMaterial, base.gameObject.layer);
		}

		private void UpdatePoints(PCTrail line, float deltaTime)
		{
			for (int i = 0; i < line.Points.Count; i++)
			{
				line.Points[i].Update((!this._noDecay) ? deltaTime : 0f);
			}
		}

		[Obsolete("UpdatePoint is deprecated, you should instead override UpdateTrail and loop through the individual points yourself (See Smoke or Smoke Plume scripts for how to do this).", true)]
		protected virtual void UpdatePoint(PCTrailPoint pCTrailPoint, float deltaTime)
		{
		}

		private void CheckEmitChange()
		{
			if (this._emit != this.Emit)
			{
				this._emit = this.Emit;
				if (this._emit)
				{
					if (this._activeTrail == null || this._activeTrail.NumPoints != this.GetMaxNumberOfPoints())
					{
						if (this._activeTrail != null)
						{
							this._activeTrail.Dispose();
						}
						this._activeTrail = new PCTrail(this.GetMaxNumberOfPoints());
					}
					else
					{
						this._activeTrail.Points.Clear();
					}
					this._activeTrail.IsActiveTrail = true;
					this.OnStartEmit();
				}
				else
				{
					this.OnStopEmit();
					this._activeTrail.IsActiveTrail = false;
					this._fadingTrails.Add(this._activeTrail);
				}
			}
		}

		private int NumberOfActivePoints(PCTrail line)
		{
			int num = 0;
			for (int i = 0; i < line.Points.Count; i++)
			{
				if (line.Points[i].TimeActive() < this.TrailData.Lifetime)
				{
					num++;
				}
			}
			return num;
		}

		[ContextMenu("Toggle inspector size input method")]
		protected void ToggleSizeInputStyle()
		{
			this.TrailData.UsingSimpleSize = !this.TrailData.UsingSimpleSize;
		}

		[ContextMenu("Toggle inspector color input method")]
		protected void ToggleColorInputStyle()
		{
			this.TrailData.UsingSimpleColor = !this.TrailData.UsingSimpleColor;
		}

		public void LifeDecayEnabled(bool enabled)
		{
			this._noDecay = !enabled;
		}

		public void Translate(Vector3 t)
		{
			if (this._activeTrail != null)
			{
				for (int i = 0; i < this._activeTrail.Points.Count; i++)
				{
					this._activeTrail.Points[i].Position += t;
				}
			}
			if (this._fadingTrails != null)
			{
				for (int j = 0; j < this._fadingTrails.Count; j++)
				{
					PCTrail pCTrail = this._fadingTrails[j];
					if (pCTrail != null)
					{
						for (int k = 0; k < pCTrail.Points.Count; k++)
						{
							pCTrail.Points[k].Position += t;
						}
					}
				}
			}
			this.OnTranslate(t);
		}

		public void CreateTrail(Vector3 from, Vector3 to, float distanceBetweenPoints)
		{
			float num = Vector3.Distance(from, to);
			Vector3 normalized = (to - from).normalized;
			float num2 = 0f;
			CircularBuffer<PCTrailPoint> circularBuffer = new CircularBuffer<PCTrailPoint>(this.GetMaxNumberOfPoints());
			int num3 = 0;
			while (num2 < num)
			{
				PCTrailPoint pCTrailPoint = new PCTrailPoint();
				pCTrailPoint.PointNumber = num3;
				pCTrailPoint.Position = from + normalized * num2;
				circularBuffer.Add(pCTrailPoint);
				this.InitialiseNewPoint(pCTrailPoint);
				num3++;
				if (distanceBetweenPoints <= 0f)
				{
					break;
				}
				num2 += distanceBetweenPoints;
			}
			PCTrailPoint pCTrailPoint2 = new PCTrailPoint();
			pCTrailPoint2.PointNumber = num3;
			pCTrailPoint2.Position = to;
			circularBuffer.Add(pCTrailPoint2);
			this.InitialiseNewPoint(pCTrailPoint2);
			PCTrail pCTrail = new PCTrail(this.GetMaxNumberOfPoints());
			pCTrail.Points = circularBuffer;
			this._fadingTrails.Add(pCTrail);
		}

		public void ClearSystem(bool emitState)
		{
			if (this._fadingTrails != null)
			{
				for (int i = 0; i < this._fadingTrails.Count; i++)
				{
					PCTrail pCTrail = this._fadingTrails[i];
					if (pCTrail != null && pCTrail != this._activeTrail)
					{
						pCTrail.Dispose();
					}
				}
				this._fadingTrails.Clear();
			}
			this.Emit = emitState;
			this._emit = !emitState;
			this.CheckEmitChange();
		}

		public int NumSegments()
		{
			int num = 0;
			if (this._activeTrail != null && this.NumberOfActivePoints(this._activeTrail) != 0)
			{
				num++;
			}
			return num + this._fadingTrails.Count;
		}
	}
}
