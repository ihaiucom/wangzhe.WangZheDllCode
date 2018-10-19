using System;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class SkinElement
	{
		public string[] ArtSkinPrefabLOD = new string[3];

		public string[] ArtSkinLobbyShowLOD = new string[2];

		public AdvanceSkinElement[] AdvanceSkin = new AdvanceSkinElement[0];
	}
}
