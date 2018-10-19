using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CCustomRcmdEquipInfo
	{
		public uint m_customRecommendEquipsLastChangedHeroID;

		public Dictionary<uint, stRcmdEquipListInfo> m_customRecommendEquipDictionary;

		public void InitializeCustomRecommendEquip(COMDT_SELFDEFINE_EQUIP_INFO selfDefineEquipInfo)
		{
			if (this.m_customRecommendEquipDictionary == null)
			{
				this.m_customRecommendEquipDictionary = new Dictionary<uint, stRcmdEquipListInfo>();
			}
			this.m_customRecommendEquipDictionary.Clear();
			this.m_customRecommendEquipsLastChangedHeroID = selfDefineEquipInfo.dwLastChgHeroId;
			int num = 0;
			while ((long)num < (long)((ulong)selfDefineEquipInfo.dwHeroNumNew))
			{
				COMDT_HERO_EQUIPLIST_NEW cOMDT_HERO_EQUIPLIST_NEW = selfDefineEquipInfo.astEquipInfoListNew[num];
				if (this.m_customRecommendEquipDictionary.ContainsKey(cOMDT_HERO_EQUIPLIST_NEW.dwHeroId))
				{
					this.m_customRecommendEquipDictionary.Remove(cOMDT_HERO_EQUIPLIST_NEW.dwHeroId);
				}
				stRcmdEquipListInfo value = CCustomRcmdEquipInfo.ConvertCltRcmdEquipListInfo(cOMDT_HERO_EQUIPLIST_NEW);
				this.m_customRecommendEquipDictionary.Add(cOMDT_HERO_EQUIPLIST_NEW.dwHeroId, value);
				num++;
			}
		}

		public static stRcmdEquipListInfo ConvertCltRcmdEquipListInfo(COMDT_HERO_EQUIPLIST_NEW svrRcmdEquipList)
		{
			stRcmdEquipListInfo result = new stRcmdEquipListInfo(0u);
			if (svrRcmdEquipList != null)
			{
				result.CurUseID = svrRcmdEquipList.dwCurUsed;
				for (int i = 0; i < 3; i++)
				{
					result.ListItem[i].bSelfDefine = (svrRcmdEquipList.astEquipList[i].bSelfDefine > 0);
					result.ListItem[i].Name = CCustomRcmdEquipInfo.GetRcmdEquipPlanName(i, svrRcmdEquipList.astEquipList[i].stEquipName);
					if (result.ListItem[i].EquipId == null)
					{
						result.ListItem[i].EquipId = new ushort[6];
					}
					if (result.ListItem[i].bSelfDefine)
					{
						for (int j = 0; j < 6; j++)
						{
							result.ListItem[i].EquipId[j] = (ushort)svrRcmdEquipList.astEquipList[i].stEquipInfo.EquipID[j];
						}
					}
					else
					{
						CCustomRcmdEquipInfo.SetDefaultRcmdEquip(ref result.ListItem[i].EquipId, svrRcmdEquipList.dwHeroId, (uint)i);
					}
				}
			}
			return result;
		}

		public static string GetRcmdEquipPlanName(int index, COMDT_HERO_EQUIP_NAME svrEquipName)
		{
			if (svrEquipName.dwNameLen == 0u)
			{
				return string.Format(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_RcmdEquipPlan_Name"), index + 1);
			}
			return StringHelper.UTF8BytesToString(ref svrEquipName.szNameStr);
		}

		public bool GetSelfDefineRcmdEquip(uint heroId, uint rcmdId, ref ushort[] equipIds)
		{
			stRcmdEquipListInfo stRcmdEquipListInfo;
			if (this.m_customRecommendEquipDictionary.TryGetValue(heroId, out stRcmdEquipListInfo) && stRcmdEquipListInfo.ListItem[(int)((UIntPtr)rcmdId)].bSelfDefine)
			{
				equipIds = stRcmdEquipListInfo.ListItem[(int)((UIntPtr)rcmdId)].EquipId;
				return true;
			}
			return false;
		}

		public uint GetRcmdEquipCurUseId(uint heroId)
		{
			stRcmdEquipListInfo stRcmdEquipListInfo;
			if (this.m_customRecommendEquipDictionary.TryGetValue(heroId, out stRcmdEquipListInfo))
			{
				return stRcmdEquipListInfo.CurUseID;
			}
			return 0u;
		}

		public void OnSelfDefineRcmdEquipChange(CSDT_HERO_EQUIPLIST svrEquipList)
		{
			uint dwHeroId = svrEquipList.dwHeroId;
			stRcmdEquipListInfo value;
			if (this.m_customRecommendEquipDictionary.TryGetValue(dwHeroId, out value))
			{
				value.ListItem[(int)((UIntPtr)svrEquipList.dwEquipIndex)].bSelfDefine = true;
				for (int i = 0; i < 6; i++)
				{
					value.ListItem[(int)((UIntPtr)svrEquipList.dwEquipIndex)].EquipId[i] = (ushort)svrEquipList.stEquipInfo.EquipID[i];
				}
				this.m_customRecommendEquipDictionary[dwHeroId] = value;
			}
			else
			{
				stRcmdEquipListInfo defaultRcmdEquipListInfo = this.GetDefaultRcmdEquipListInfo(dwHeroId, svrEquipList.dwEquipIndex);
				defaultRcmdEquipListInfo.ListItem[(int)((UIntPtr)svrEquipList.dwEquipIndex)].bSelfDefine = true;
				for (int j = 0; j < 6; j++)
				{
					defaultRcmdEquipListInfo.ListItem[(int)((UIntPtr)svrEquipList.dwEquipIndex)].EquipId[j] = (ushort)svrEquipList.stEquipInfo.EquipID[j];
				}
				this.m_customRecommendEquipDictionary.Add(dwHeroId, defaultRcmdEquipListInfo);
			}
		}

		public void OnRecoverSystemEquipRsp(SCPKG_RECOVER_SYSTEMEQUIP_RSP recoverDefaultRcmdEquip)
		{
			stRcmdEquipListInfo value;
			if (this.m_customRecommendEquipDictionary.TryGetValue(recoverDefaultRcmdEquip.dwHeroId, out value))
			{
				value.ListItem[(int)((UIntPtr)recoverDefaultRcmdEquip.dwEquipIndex)].bSelfDefine = false;
				CCustomRcmdEquipInfo.SetDefaultRcmdEquip(ref value.ListItem[(int)((UIntPtr)recoverDefaultRcmdEquip.dwEquipIndex)].EquipId, recoverDefaultRcmdEquip.dwHeroId, recoverDefaultRcmdEquip.dwEquipIndex);
				this.m_customRecommendEquipDictionary[recoverDefaultRcmdEquip.dwHeroId] = value;
			}
		}

		public void OnChangeUsedHeroRcmdEquipID(uint heroId, uint useRcmdId)
		{
			stRcmdEquipListInfo defaultRcmdEquipListInfo;
			if (this.m_customRecommendEquipDictionary.TryGetValue(heroId, out defaultRcmdEquipListInfo))
			{
				defaultRcmdEquipListInfo.CurUseID = useRcmdId;
				this.m_customRecommendEquipDictionary[heroId] = defaultRcmdEquipListInfo;
			}
			else
			{
				defaultRcmdEquipListInfo = this.GetDefaultRcmdEquipListInfo(heroId, useRcmdId);
				this.m_customRecommendEquipDictionary.Add(heroId, defaultRcmdEquipListInfo);
			}
		}

		public void OnChgHeroRcmdEquipPlanName(uint heroId, uint index, COMDT_HERO_EQUIP_NAME equipName)
		{
			stRcmdEquipListInfo defaultRcmdEquipListInfo;
			if (this.m_customRecommendEquipDictionary.TryGetValue(heroId, out defaultRcmdEquipListInfo))
			{
				if (equipName.dwNameLen > 0u)
				{
					defaultRcmdEquipListInfo.ListItem[(int)((UIntPtr)index)].Name = StringHelper.UTF8BytesToString(ref equipName.szNameStr);
				}
				else
				{
					defaultRcmdEquipListInfo.ListItem[(int)((UIntPtr)index)].Name = string.Format(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_RcmdEquipPlan_Name"), index + 1u);
				}
				this.m_customRecommendEquipDictionary[heroId] = defaultRcmdEquipListInfo;
			}
			else
			{
				defaultRcmdEquipListInfo = this.GetDefaultRcmdEquipListInfo(heroId, 0u);
				defaultRcmdEquipListInfo.ListItem[(int)((UIntPtr)index)].Name = StringHelper.UTF8BytesToString(ref equipName.szNameStr);
				this.m_customRecommendEquipDictionary.Add(heroId, defaultRcmdEquipListInfo);
			}
		}

		private static void SetDefaultRcmdEquip(ref ushort[] equipId, uint heroId, uint rcmdId)
		{
			if (equipId == null)
			{
				equipId = new ushort[6];
			}
			ResRecommendEquipInBattle defaultRecommendEquipInfo = Singleton<CEquipSystem>.GetInstance().GetDefaultRecommendEquipInfo(heroId, rcmdId + 1u);
			if (defaultRecommendEquipInfo != null)
			{
				for (int i = 0; i < 6; i++)
				{
					equipId[i] = defaultRecommendEquipInfo.RecommendEquipID[i];
				}
			}
		}

		public stRcmdEquipListInfo GetRcmdEquipListInfo(uint heroId)
		{
			stRcmdEquipListInfo defaultRcmdEquipListInfo;
			if (!this.m_customRecommendEquipDictionary.TryGetValue(heroId, out defaultRcmdEquipListInfo))
			{
				defaultRcmdEquipListInfo = this.GetDefaultRcmdEquipListInfo(heroId, 0u);
			}
			return defaultRcmdEquipListInfo;
		}

		private stRcmdEquipListInfo GetDefaultRcmdEquipListInfo(uint heroId, uint curUseId)
		{
			stRcmdEquipListInfo result = new stRcmdEquipListInfo(curUseId);
			for (uint num = 0u; num < 3u; num += 1u)
			{
				result.ListItem[(int)((UIntPtr)num)].bSelfDefine = false;
				CCustomRcmdEquipInfo.SetDefaultRcmdEquip(ref result.ListItem[(int)((UIntPtr)num)].EquipId, heroId, num);
			}
			return result;
		}

		public string GetRcmdEquipPlanName(uint heroId, uint index)
		{
			stRcmdEquipListInfo stRcmdEquipListInfo;
			if (this.m_customRecommendEquipDictionary.TryGetValue(heroId, out stRcmdEquipListInfo))
			{
				return stRcmdEquipListInfo.ListItem[(int)((UIntPtr)index)].Name;
			}
			return string.Format(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_RcmdEquipPlan_Name"), index + 1u);
		}
	}
}
