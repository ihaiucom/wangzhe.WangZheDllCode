using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CEquipInfo : IComparable
	{
		public ushort m_equipID;

		public ResEquipInBattle m_resEquipInBattle;

		public ResEquipInBattle m_resRemoteAtkEquipInBattle;

		public string m_equipName;

		public string m_equipDesc;

		public string m_equipIconPath;

		public string[] m_equipBuffDescs = new string[3];

		public ushort[] m_requiredEquipIDs;

		public string m_equipPropertyDesc;

		public List<ushort> m_backEquipIDs;

		public CEquipInfo(ushort equipID)
		{
			this.m_equipID = equipID;
			this.m_resEquipInBattle = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipID);
			if (this.m_resEquipInBattle != null)
			{
				if (this.m_resEquipInBattle.wEquipIDRemoteAtk > 0)
				{
					this.m_resRemoteAtkEquipInBattle = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)this.m_resEquipInBattle.wEquipIDRemoteAtk);
				}
				this.m_equipName = StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.szName);
				this.m_equipDesc = StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.szDesc);
				this.m_equipIconPath = CUIUtility.s_Sprite_System_BattleEquip_Dir + StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.szIcon);
				string text = StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.szActiveSkillDes);
				for (int i = 0; i < 3; i++)
				{
					this.m_equipBuffDescs[i] = StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.astEffectCombine[i].szDesc);
				}
				this.m_requiredEquipIDs = this.GetRequiredEquipIDs(this.m_resEquipInBattle);
				this.m_equipPropertyDesc = string.Empty;
				string equipPropertyValueDesc = this.GetEquipPropertyValueDesc(this.m_resEquipInBattle);
				if (!string.IsNullOrEmpty(equipPropertyValueDesc))
				{
					this.m_equipPropertyDesc = this.m_equipPropertyDesc + equipPropertyValueDesc + "\n";
				}
				string equipPassiveSkillDesc = this.GetEquipPassiveSkillDesc(this.m_resEquipInBattle);
				string equipPassiveEftDesc = this.GetEquipPassiveEftDesc(this.m_resEquipInBattle);
				string text2 = equipPassiveSkillDesc + equipPassiveEftDesc;
				if (!string.IsNullOrEmpty(text2))
				{
					this.m_equipPropertyDesc += text2;
				}
				if (!string.IsNullOrEmpty(text))
				{
					this.m_equipPropertyDesc += text;
				}
				if (!string.IsNullOrEmpty(this.m_resEquipInBattle.szExtraDesc))
				{
					this.m_equipPropertyDesc += this.m_resEquipInBattle.szExtraDesc;
				}
			}
		}

		public int CompareTo(object obj)
		{
			CEquipInfo cEquipInfo = obj as CEquipInfo;
			if (this.m_resEquipInBattle.dwBuyPrice > cEquipInfo.m_resEquipInBattle.dwBuyPrice)
			{
				return -1;
			}
			if (this.m_resEquipInBattle.dwBuyPrice != cEquipInfo.m_resEquipInBattle.dwBuyPrice)
			{
				return 1;
			}
			if (this.m_equipID > cEquipInfo.m_equipID)
			{
				return -1;
			}
			if (this.m_equipID == cEquipInfo.m_equipID)
			{
				return 0;
			}
			return 1;
		}

		public void AddBackEquipID(ushort backEquipID)
		{
			if (this.m_backEquipIDs == null)
			{
				this.m_backEquipIDs = new List<ushort>();
			}
			if (!this.m_backEquipIDs.Contains(backEquipID))
			{
				this.m_backEquipIDs.Add(backEquipID);
			}
		}

		private ushort[] GetRequiredEquipIDs(ResEquipInBattle resEquipInBattle)
		{
			string text = StringHelper.UTF8BytesToString(ref resEquipInBattle.szRequireEquip);
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					','
				});
				ushort[] array2 = new ushort[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						array2[i] = ushort.Parse(array[i].Trim());
					}
					catch (Exception)
					{
						array2[i] = 0;
					}
				}
				return array2;
			}
			return null;
		}

		private string GetEquipPropertyValueDesc(ResEquipInBattle resEquipInBattle)
		{
			if (resEquipInBattle == null)
			{
				return string.Empty;
			}
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			string text = string.Empty;
			if (resEquipInBattle.dwPhyAttack > 0u)
			{
				text = string.Format("{0}+{1} {2}\n", text, resEquipInBattle.dwPhyAttack, instance.GetText("Hero_Prop_PhyAtkPt"));
			}
			if (resEquipInBattle.dwAttackSpeed > 0u)
			{
				text = string.Format("{0}+{1}% {2}\n", text, resEquipInBattle.dwAttackSpeed / 100u, instance.GetText("Hero_Prop_AtkSpd"));
			}
			if (resEquipInBattle.dwCriticalHit > 0u)
			{
				text = string.Format("{0}+{1}% {2}\n", text, resEquipInBattle.dwCriticalHit / 100u, instance.GetText("Hero_Prop_CritRate"));
			}
			if (resEquipInBattle.dwHealthSteal > 0u)
			{
				text = string.Format("{0}+{1}% {2}\n", text, resEquipInBattle.dwHealthSteal / 100u, instance.GetText("Hero_Prop_PhyVamp"));
			}
			if (resEquipInBattle.dwMagicAttack > 0u)
			{
				text = string.Format("{0}+{1} {2}\n", text, resEquipInBattle.dwMagicAttack, instance.GetText("Hero_Prop_MgcAtkPt"));
			}
			if (resEquipInBattle.dwCDReduce > 0u)
			{
				text = string.Format("{0}+{1}% {2}\n", text, resEquipInBattle.dwCDReduce / 100u, instance.GetText("Hero_Prop_CdReduce"));
			}
			if (resEquipInBattle.dwMagicPoint > 0u)
			{
				text = string.Format("{0}+{1} {2}\n", text, resEquipInBattle.dwMagicPoint, instance.GetText("Hero_Prop_MaxEp"));
			}
			if (resEquipInBattle.dwMagicRecover > 0u)
			{
				text = string.Format("{0}+{1} {2}\n", text, resEquipInBattle.dwMagicRecover, instance.GetText("Hero_Prop_EpRecover"));
			}
			if (resEquipInBattle.dwPhyDefence > 0u)
			{
				text = string.Format("{0}+{1} {2}\n", text, resEquipInBattle.dwPhyDefence, instance.GetText("Hero_Prop_PhyDefPt"));
			}
			if (resEquipInBattle.dwMagicDefence > 0u)
			{
				text = string.Format("{0}+{1} {2}\n", text, resEquipInBattle.dwMagicDefence, instance.GetText("Hero_Prop_MgcDefPt"));
			}
			if (resEquipInBattle.dwHealthPoint > 0u)
			{
				text = string.Format("{0}+{1} {2}\n", text, resEquipInBattle.dwHealthPoint, instance.GetText("Hero_Prop_MaxHp"));
			}
			if (resEquipInBattle.dwHealthRecover > 0u)
			{
				text = string.Format("{0}+{1} {2}\n", text, resEquipInBattle.dwHealthRecover, instance.GetText("Hero_Prop_HpRecover"));
			}
			if (resEquipInBattle.dwMoveSpeed > 0u)
			{
				text = string.Format("{0}+{1}% {2}\n", text, resEquipInBattle.dwMoveSpeed / 100u, instance.GetText("Hero_Prop_MoveSpd"));
			}
			return text;
		}

		private string GetEquipPassiveSkillDesc(ResEquipInBattle resEquipInBattle)
		{
			if (resEquipInBattle == null)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = 0; i < resEquipInBattle.astPassiveSkill.Length; i++)
			{
				if (!string.IsNullOrEmpty(resEquipInBattle.astPassiveSkill[i].szDesc))
				{
					text = text + resEquipInBattle.astPassiveSkill[i].szDesc + "\n";
				}
			}
			return text;
		}

		private string GetEquipPassiveEftDesc(ResEquipInBattle resEquipInBattle)
		{
			if (resEquipInBattle == null)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = 0; i < resEquipInBattle.astEffectCombine.Length; i++)
			{
				if (!string.IsNullOrEmpty(resEquipInBattle.astEffectCombine[i].szDesc))
				{
					text = text + resEquipInBattle.astEffectCombine[i].szDesc + "\n";
				}
			}
			return text;
		}
	}
}
