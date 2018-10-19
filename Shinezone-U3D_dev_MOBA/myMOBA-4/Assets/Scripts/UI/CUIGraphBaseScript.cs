using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIGraphBaseScript : CUIComponent
	{
		public static readonly int s_depth = 11;

		public static readonly int s_cullingMask = LayerMask.NameToLayer("UI");

		public Color[] m_colors;

		public int cameraDepth = CUIGraphBaseScript.s_depth;

		[SerializeField]
		protected Vector3[][] m_vertexs;

		private static Material s_lineMaterial = null;

		private Camera m_camera;

		protected bool vertexChanged;

		public override void Initialize(CUIFormScript formScript)
		{
			base.Initialize(formScript);
			this.m_camera = base.GetComponent<Camera>();
			if (base.camera == null)
			{
				this.m_camera = base.gameObject.AddComponent<Camera>();
				base.camera.depth = (float)CUIGraphBaseScript.s_depth;
				base.camera.cullingMask = CUIGraphBaseScript.s_cullingMask;
				base.camera.clearFlags = CameraClearFlags.Depth;
			}
			if (CUIGraphBaseScript.s_lineMaterial == null)
			{
				CUIGraphBaseScript.s_lineMaterial = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {     Blend SrcAlpha OneMinusSrcAlpha     ZWrite Off Cull Off Fog { Mode Off }     BindChannels {      Bind \"vertex\", vertex Bind \"color\", color }} } }");
				CUIGraphBaseScript.s_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
				CUIGraphBaseScript.s_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
			}
		}

		protected override void OnDestroy()
		{
			this.m_camera = null;
			base.OnDestroy();
		}

		public Camera GetCamera()
		{
			return this.m_camera;
		}

		public void SetVertexs(Vector3[] vertexs, Color color)
		{
			if (vertexs == null)
			{
				return;
			}
			this.m_vertexs = new Vector3[1][];
			this.m_colors = new Color[1];
			this.m_vertexs[0] = new Vector3[vertexs.Length];
			for (int i = 0; i < vertexs.Length; i++)
			{
				this.m_vertexs[0][i] = new Vector3(vertexs[i].x, vertexs[i].y, 0f);
			}
			this.m_colors[0] = color;
			this.vertexChanged = true;
		}

		public void SetVertexs(Vector3[][] vertexs, Color[] colors)
		{
			if (vertexs == null || colors == null)
			{
				return;
			}
			if (vertexs.Length != colors.Length)
			{
				return;
			}
			this.m_vertexs = new Vector3[vertexs.Length][];
			this.m_colors = new Color[colors.Length];
			for (int i = 0; i < vertexs.Length; i++)
			{
				if (vertexs[i] != null)
				{
					this.m_vertexs[i] = new Vector3[vertexs[i].Length];
					this.m_colors[i] = colors[i];
					for (int j = 0; j < vertexs[i].Length; j++)
					{
						this.m_vertexs[i][j] = new Vector3(vertexs[i][j].x, vertexs[i][j].y, 0f);
					}
				}
			}
			this.vertexChanged = true;
		}

		private void OnPostRender()
		{
			if (this.m_vertexs == null)
			{
				return;
			}
			if (CUIGraphBaseScript.s_lineMaterial == null)
			{
				return;
			}
			CUIGraphBaseScript.s_lineMaterial.SetPass(0);
			this.OnDraw();
		}

		protected virtual void OnDraw()
		{
		}
	}
}
