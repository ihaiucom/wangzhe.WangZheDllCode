using System;

namespace Assets.Scripts.GameSystem
{
	public class CTextImageNode
	{
		public string content;

		public CChatParser.InfoType type;

		public float width;

		public float height;

		public float posX;

		public float posY;

		public CTextImageNode(string ct, CChatParser.InfoType type, float width, float height, float x, float y)
		{
			this.content = ct;
			this.type = type;
			this.width = width;
			this.height = height;
			this.posX = x;
			this.posY = y;
		}
	}
}
