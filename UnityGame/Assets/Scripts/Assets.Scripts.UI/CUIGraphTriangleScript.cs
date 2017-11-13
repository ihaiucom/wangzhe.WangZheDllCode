using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIGraphTriangleScript : CUIGraphBaseScript
	{
		public override void Initialize(CUIFormScript formScript)
		{
			base.Initialize(formScript);
		}

		protected override void OnDraw()
		{
			GL.PushMatrix();
			GL.LoadPixelMatrix();
			GL.Begin(4);
			GL.Color(this.m_colors[0]);
			for (int i = 0; i < this.m_vertexs.Length; i++)
			{
				if (i + 2 < this.m_vertexs.Length)
				{
					GL.Vertex3(this.m_vertexs[0][i].x, this.m_vertexs[0][i].y, this.m_vertexs[0][i].z);
					GL.Vertex3(this.m_vertexs[0][i + 1].x, this.m_vertexs[0][i + 1].y, this.m_vertexs[0][i + 1].z);
					GL.Vertex3(this.m_vertexs[0][i + 2].x, this.m_vertexs[0][i + 2].y, this.m_vertexs[0][i + 2].z);
				}
				i += 2;
			}
			GL.End();
			GL.PopMatrix();
		}
	}
}
