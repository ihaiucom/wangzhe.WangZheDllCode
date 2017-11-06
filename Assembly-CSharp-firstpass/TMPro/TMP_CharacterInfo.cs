using System;
using UnityEngine;

namespace TMPro
{
	public struct TMP_CharacterInfo
	{
		public TMP_CharacterType type;

		public char character;

		public short lineNumber;

		public short pageNumber;

		public short index;

		public int meshIndex;

		public short vertexIndex;

		public TMP_Vertex vertex_TL;

		public TMP_Vertex vertex_BL;

		public TMP_Vertex vertex_TR;

		public TMP_Vertex vertex_BR;

		public Vector3 topLeft;

		public Vector3 bottomLeft;

		public Vector3 topRight;

		public Vector3 bottomRight;

		public float topLine;

		public float baseLine;

		public float bottomLine;

		public float xAdvance;

		public float aspectRatio;

		public float padding;

		public float scale;

		public Color32 color;

		public FontStyles style;

		public bool isVisible;
	}
}
