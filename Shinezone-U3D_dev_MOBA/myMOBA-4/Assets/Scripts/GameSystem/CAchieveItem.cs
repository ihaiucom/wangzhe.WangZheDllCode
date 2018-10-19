using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CAchieveItem : IComparable
	{
		public uint m_cfgId;

		public ResAchievement m_cfgInfo;

		private COM_ACHIEVEMENT_STATE m_state;

		public RES_ACHIEVE_DONE_TYPE m_doneType;

		private int m_doneCnt;

		public int CompareTo(object obj)
		{
			CAchieveItem cAchieveItem = obj as CAchieveItem;
			if (this.m_state == cAchieveItem.m_state)
			{
				if (this.m_state != COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_UNFIN)
				{
					return this.m_cfgId.CompareTo(cAchieveItem.m_cfgId);
				}
				if (this.m_cfgInfo.dwClassification > cAchieveItem.m_cfgInfo.dwClassification)
				{
					return -1;
				}
				if (this.m_cfgInfo.dwClassification < cAchieveItem.m_cfgInfo.dwClassification)
				{
					return 1;
				}
				return this.m_cfgId.CompareTo(cAchieveItem.m_cfgId);
			}
			else
			{
				if (this.m_state == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN)
				{
					return -1;
				}
				if (this.m_state != COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_UNFIN)
				{
					return 1;
				}
				if (cAchieveItem.m_state == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN)
				{
					return 1;
				}
				return -1;
			}
		}

		public void InitStateData(COMDT_ACHIEVEMENT_DATA stateInfo)
		{
			this.m_cfgId = stateInfo.dwID;
			this.m_state = (COM_ACHIEVEMENT_STATE)stateInfo.bState;
			this.m_cfgInfo = GameDataMgr.achieveDatabin.GetDataByKey(this.m_cfgId);
			if (this.m_cfgInfo == null)
			{
				return;
			}
			this.m_doneType = (RES_ACHIEVE_DONE_TYPE)this.m_cfgInfo.dwDoneType;
		}

		public void SetDoneData(ref int[] achieveDoneArr)
		{
			if (achieveDoneArr == null)
			{
				return;
			}
			if (this.m_doneType >= RES_ACHIEVE_DONE_TYPE.RES_ACHIEVE_DONE_GET_GOLD && this.m_doneType < (RES_ACHIEVE_DONE_TYPE)achieveDoneArr.Length)
			{
				this.m_doneCnt = achieveDoneArr[(int)this.m_doneType];
			}
		}

		public string GetAchievementName()
		{
			return StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szName);
		}

		public string GetAchievementDesc()
		{
			string format = StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szDesc);
			if (this.m_cfgInfo.dwDoneParm == 0u)
			{
				return string.Format(format, this.m_cfgInfo.dwDoneCondi);
			}
			return string.Format(format, this.m_cfgInfo.dwDoneCondi, this.m_cfgInfo.dwDoneParm);
		}

		public string GetAchievementTips()
		{
			string format = StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szTips);
			return string.Format(format, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name);
		}

		public string GetAchievementIconPath()
		{
			return CUIUtility.s_Sprite_Dynamic_Task_Dir + this.m_cfgInfo.dwImage;
		}

		public string GetAchievementBgIconPath()
		{
			return CUIUtility.s_Sprite_Dynamic_Task_Dir + this.m_cfgInfo.dwBgImage;
		}

		public uint GetAchievementAwardCnt(RES_REWARDS_TYPE rewardType)
		{
			for (int i = 0; i < this.m_cfgInfo.astReward.Length; i++)
			{
				if (this.m_cfgInfo.astReward[i].bRewardType == (byte)rewardType)
				{
					return this.m_cfgInfo.astReward[i].dwRewardNum;
				}
			}
			return 0u;
		}

		public uint GetAchievementAwardId(RES_REWARDS_TYPE rewardType)
		{
			for (int i = 0; i < this.m_cfgInfo.astReward.Length; i++)
			{
				if (this.m_cfgInfo.astReward[i].bRewardType == (byte)rewardType)
				{
					return this.m_cfgInfo.astReward[i].dwRewardID;
				}
			}
			return 0u;
		}

		public bool IsNeedShow()
		{
			return this.m_cfgInfo.dwPreAchievementID <= 0u || CAchieveInfo.GetAchieveInfo().GetAchieveItemById(this.m_cfgInfo.dwPreAchievementID).IsFinish();
		}

		public bool IsDelayPopUp()
		{
			return this.m_cfgInfo.bIsPopUpDelay > 0;
		}

		public bool IsHideForegroundIcon()
		{
			return this.m_cfgInfo.bHideForegroundImage > 0;
		}

		public bool IsFinish()
		{
			return this.m_state == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_REWARD || COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN == this.m_state;
		}

		public bool IsGotReward()
		{
			return COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_REWARD == this.m_state;
		}

		public bool IsCanGetReward()
		{
			return COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN == this.m_state;
		}

		public COM_ACHIEVEMENT_STATE GetAchieveState()
		{
			return this.m_state;
		}

		public void SetAchieveState(COM_ACHIEVEMENT_STATE stateVal)
		{
			this.m_state = stateVal;
		}

		public int GetAchieveDoneCnt()
		{
			return this.m_doneCnt;
		}

		public void SetAchieveDoneCnt(int cnt)
		{
			this.m_doneCnt = cnt;
		}

		public static string GetAchievementTypeName(int type)
		{
			string key;
			switch (type)
			{
			case 1:
				key = "Achievement_Type_Growing";
				break;
			case 2:
				key = "Achievement_Type_Hero";
				break;
			case 3:
				key = "Achievement_Type_Pvp";
				break;
			case 4:
				key = "Achievement_Type_Pve";
				break;
			case 5:
				key = "Achievement_Type_Socially";
				break;
			case 6:
				key = "Achievement_Type_Rank";
				break;
			default:
				key = "ERROR_ACHIEVE_TYPE";
				break;
			}
			return Singleton<CTextManager>.GetInstance().GetText(key);
		}
	}
}
