using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIGraphIRViewScript : CUIComponent
	{
		public float thickness;

		public float radius = 0.0025f;

		public float intensity = 0.1f;

		public float workRadius = 0.1f;

		public Sprite gradientColorImage;

		public static readonly int s_cullingMask = LayerMask.NameToLayer("UI");

		protected bool vertexChanged;

		private CUIFormScript form;

		private Image m_image;

		private Texture2D m_text2D;

		private Color[] m_gradientColor;

		public Gradient m_gradient;

		public override void Initialize(CUIFormScript formScript)
		{
			this.form = formScript;
			base.Initialize(formScript);
			this.m_image = base.GetComponent<Image>();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public static void SetSettingDataByPrefab(CUIGraphIRViewScript irView, stStateHeatmapStorage st, int srcWidth, int srcHeight)
		{
			st.radius = irView.radius;
			st.workRadius = irView.workRadius;
			st.intensity = irView.intensity;
			st.defaultCtrlRect.x = (irView.transform as RectTransform).rect.width;
			st.defaultCtrlRect.y = (irView.transform as RectTransform).rect.height;
			st.mapOrgRect.x = (float)srcWidth;
			st.mapOrgRect.y = (float)srcHeight;
			if (irView.gradientColorImage != null)
			{
				Texture2D texture = irView.gradientColorImage.texture;
				irView.m_gradientColor = new Color[texture.width];
				irView.m_gradientColor = texture.GetPixels(0, texture.height / 2, texture.width, 1);
			}
		}

		public static void StoreTextureData(List<Vector3> vertexs, stStateHeatmapStorage st, bool isUpdate = false)
		{
			if (vertexs == null)
			{
				return;
			}
			int num = (int)st.mapOrgRect.x;
			int num2 = (int)st.mapOrgRect.y;
			if (st.displayRates == null)
			{
				isUpdate = false;
			}
			if (!isUpdate)
			{
				st.maxRate = 0f;
				int num3 = (int)st.defaultCtrlRect.x + 1;
				int num4 = (int)st.defaultCtrlRect.y + 1;
				st.crossLen = num3 * num3 + num4 * num4;
				st.myVertex.Clear();
				st.displayRates = new float[num3][];
				for (int i = 0; i < num3; i++)
				{
					st.displayRates[i] = new float[num4];
				}
			}
			else if (st.myVertex.get_Count() > vertexs.get_Count())
			{
				Debug.LogError("IRView should not use update way");
			}
			int count = st.myVertex.get_Count();
			for (int j = st.myVertex.get_Count(); j < vertexs.get_Count(); j++)
			{
				float value = (vertexs.get_Item(j).x + (float)(num / 2)) * st.defaultCtrlRect.x / (float)num;
				float value2 = (vertexs.get_Item(j).z + (float)(num2 / 2)) * st.defaultCtrlRect.y / (float)num2;
				float x = Mathf.Clamp(value, 0f, (float)(st.displayRates.Length - 1));
				float y = Mathf.Clamp(value2, 0f, (float)(st.displayRates[0].Length - 1));
				Vector2 vector = new Vector2(x, y);
				st.myVertex.Add(vector);
			}
			int num5 = (int)((float)st.displayRates.Length * st.workRadius);
			Vector2 p = new Vector2(0f, 0f);
			for (int k = count; k < st.myVertex.get_Count(); k++)
			{
				for (int l = -num5; l < num5; l++)
				{
					p.x = st.myVertex.get_Item(k).x + (float)l;
					if (p.x >= 0f && p.x < (float)st.displayRates.Length)
					{
						for (int m = -num5; m < num5; m++)
						{
							p.y = st.myVertex.get_Item(k).y + (float)m;
							if (p.y >= 0f && p.y < (float)st.displayRates[0].Length)
							{
								float num6 = CUIGraphIRViewScript.GetDistance(p, st.myVertex.get_Item(k)) / (float)st.crossLen;
								float num7 = st.displayRates[(int)p.x][(int)p.y] += (1f - Mathf.Clamp(num6 / st.radius, 0f, 1f)) * st.intensity;
								if (num7 > st.maxRate)
								{
									st.maxRate = num7;
								}
							}
						}
					}
				}
			}
		}

		public void UpdateTexture(stStateHeatmapStorage st)
		{
			if (st == null)
			{
				return;
			}
			if (this.m_text2D == null)
			{
				this.m_text2D = new Texture2D((int)st.defaultCtrlRect.x, (int)st.defaultCtrlRect.y, TextureFormat.ARGB32, false);
				this.m_image.set_sprite(Sprite.Create(this.m_text2D, new Rect(0f, 0f, (float)this.m_text2D.width, (float)this.m_text2D.height), new Vector2(0f, 0f)));
			}
			Color color = default(Color);
			if (st.displayRates == null)
			{
				return;
			}
			for (int i = 0; i < st.displayRates.Length - 1; i++)
			{
				if (st.displayRates[i] != null)
				{
					for (int j = 0; j < st.displayRates[i].Length - 1; j++)
					{
						float num = st.displayRates[i][j] / st.maxRate;
						if (num > 0.01f)
						{
							if (this.m_gradientColor != null && this.m_gradientColor.Length != 0)
							{
								int num2 = (int)(num * (float)this.m_gradientColor.Length);
								if (num2 >= this.m_gradientColor.Length)
								{
									num2 = this.m_gradientColor.Length - 1;
								}
								color = this.m_gradientColor[num2];
							}
							else
							{
								color.r = num;
								color.g = num;
								color.b = num;
								color.a = num;
							}
						}
						else
						{
							color.a = 0f;
						}
						this.m_text2D.SetPixel(i, j, color);
					}
				}
			}
			this.m_text2D.Apply();
		}

		private static float GetDistance(Vector2 p1, Vector2 p2)
		{
			return Mathf.Pow(p1.x - p2.x, 2f) + Mathf.Pow(p1.y - p2.y, 2f);
		}

		private void OnPostRender()
		{
		}
	}
}
