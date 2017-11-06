using System;

namespace Assets.Scripts.GameSystem
{
	public struct stRcmdEquipItem
	{
		public bool bSelfDefine;

		public string Name;

		public ushort[] EquipId;

		public stRcmdEquipItem(int index)
		{
			this.bSelfDefine = false;
			this.Name = string.Format(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_RcmdEquipPlan_Name"), index + 1);
			this.EquipId = new ushort[6];
		}
	}
}
