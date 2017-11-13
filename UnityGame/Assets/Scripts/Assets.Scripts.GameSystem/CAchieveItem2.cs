using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CAchieveItem2
	{
		public uint ID;

		public uint PrevID;

		public uint DoneTime;

		public CAchieveItem2 Prev;

		public CAchieveItem2 Next;

		public ResAchievement Cfg;

		public COM_ACHIEVEMENT_STATE State;

		public RES_ACHIEVE_DONE_TYPE DoneType;

		public int DoneCnt;

		public CAchieveItem2(ref ResAchievement achievement)
		{
			this.ID = achievement.dwID;
			this.DoneTime = 0u;
			this.State = COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_UNFIN;
			this.DoneType = (RES_ACHIEVE_DONE_TYPE)achievement.dwDoneType;
			this.Cfg = achievement;
			this.PrevID = achievement.dwPreAchievementID;
			this.DoneCnt = 0;
		}

		public bool IsFinish()
		{
			return this.State == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_REWARD || COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN == this.State;
		}

		public bool IsHideForegroundIcon()
		{
			return this.Cfg.bHideForegroundImage > 0;
		}

		public uint GetMostRecentlyModifyTime()
		{
			if (this.Next != null && this.Next.IsFinish())
			{
				return this.Next.GetMostRecentlyModifyTime();
			}
			return this.DoneTime;
		}

		public string GetGotTimeText(bool needCheckMostRecentlyDone = false, bool needJudgeLeveUp = false)
		{
			CAchieveItem2 cAchieveItem = this;
			if (needCheckMostRecentlyDone)
			{
				cAchieveItem = this.TryToGetMostRecentlyDoneItem();
			}
			if (cAchieveItem == null)
			{
				return Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Not_Done");
			}
			if (!cAchieveItem.IsFinish())
			{
				return Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Not_Done");
			}
			if (cAchieveItem == this)
			{
				if (this.DoneTime == 0u)
				{
					return Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done");
				}
				return string.Format("{0:yyyy.M.d} {1}", Utility.ToUtcTime2Local((long)((ulong)this.DoneTime)), Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done"));
			}
			else
			{
				if (cAchieveItem.DoneTime == 0u)
				{
					return Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done");
				}
				if (needJudgeLeveUp)
				{
					return string.Format("{0:yyyy.M.d} {1}", Utility.ToUtcTime2Local((long)((ulong)cAchieveItem.DoneTime)), Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Level_Up"));
				}
				return string.Format("{0:yyyy.M.d} {1}", Utility.ToUtcTime2Local((long)((ulong)cAchieveItem.DoneTime)), Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done"));
			}
		}

		public CAchieveItem2 TryToGetMostRecentlyDoneItem()
		{
			if (this.Next != null && this.Next.IsFinish())
			{
				return this.Next.TryToGetMostRecentlyDoneItem();
			}
			if (this.IsFinish())
			{
				return this;
			}
			return null;
		}

		public string GetAchievementDesc()
		{
			string szDesc = this.Cfg.szDesc;
			if (this.Cfg.dwDoneType == 41u)
			{
				return string.Format(szDesc, CLadderView.GetRankName((byte)this.Cfg.dwDoneCondi, this.Cfg.dwDoneParm));
			}
			if (this.Cfg.dwDoneParm == 0u)
			{
				if (this.Cfg.bShowProcess > 0 && !this.IsFinish())
				{
					return string.Format("{0}\n({1}/{2})", string.Format(szDesc, this.Cfg.dwDoneCondi), this.DoneCnt, this.Cfg.dwDoneCondi);
				}
				return string.Format(szDesc, this.Cfg.dwDoneCondi);
			}
			else
			{
				if (this.Cfg.bShowProcess > 0 && !this.IsFinish())
				{
					return string.Format("{0}\n({1}/{2})", string.Format(szDesc, this.Cfg.dwDoneCondi, this.Cfg.dwDoneParm), this.DoneCnt, this.Cfg.dwDoneCondi);
				}
				return string.Format(szDesc, this.Cfg.dwDoneCondi, this.Cfg.dwDoneParm);
			}
		}

		public string GetAchievementTips()
		{
			string szTips = this.Cfg.szTips;
			return string.Format(szTips, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name);
		}

		public string GetAchieveImagePath()
		{
			return string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Task_Dir, this.Cfg.dwImage);
		}

		public string GetAchievementBgIconPath()
		{
			return string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Task_Dir, this.Cfg.dwBgImage);
		}

		public CAchieveItem2 GetHead()
		{
			if (this.Prev != null)
			{
				return this.Prev.GetHead();
			}
			return this;
		}

		public CAchieveItem2 GetHeadAndSetFinishRecursively(uint doneTime)
		{
			this.State = COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN;
			this.DoneTime = doneTime;
			if (this.Prev != null)
			{
				return this.Prev.GetHeadAndSetFinishRecursively(doneTime);
			}
			return this;
		}

		public int GetAchievementCnt()
		{
			int num = 1;
			if (this.Next != null)
			{
				return num + this.Next.GetAchievementCnt();
			}
			return num;
		}

		public uint GetTotalDonePoints()
		{
			uint num = 0u;
			if (this.IsFinish())
			{
				num += this.Cfg.dwPoint;
			}
			if (this.Next != null && this.Next.IsFinish())
			{
				return num + this.Next.GetTotalDonePoints();
			}
			return num;
		}
	}
}
