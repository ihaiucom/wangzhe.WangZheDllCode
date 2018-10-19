using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CChatEntity
	{
		public ulong ullUid;

		public uint iLogicWorldID;

		public string text;

		public EChaterType type;

		public bool bHasReaded;

		public string name;

		public string openId;

		public string head_url;

		public string level;

		public int time;

		public ListView<CTextImageNode> TextObjList = new ListView<CTextImageNode>();

		public float final_width;

		public float final_height;

		public uint numLine = 1u;

		public COMDT_GAME_VIP_CLIENT stGameVip;

		public void Clear()
		{
			this.ullUid = 0uL;
			this.iLogicWorldID = 0u;
			this.name = (this.head_url = (this.level = (this.text = string.Empty)));
			this.openId = string.Empty;
			this.TextObjList.Clear();
			this.stGameVip = null;
			this.final_width = 0f;
			this.type = EChaterType.None;
			this.bHasReaded = true;
		}
	}
}
