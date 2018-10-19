using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CPlayerMentorInfoController : Singleton<CPlayerMentorInfoController>
	{
		public ResFamousMentor m_famousMentorData;

		public int m_addViewtype = 3;

		private string mentorStateStr;

		public string mentorBtnStr
		{
			get;
			private set;
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Mentor_WatchHisMentor, new CUIEventManager.OnUIEventHandler(this.OnWatchHisMentor));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Mentor_OpenMentorPage, new CUIEventManager.OnUIEventHandler(this.OnOpenMentorPage));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Mentor_ApplyRequest, new CUIEventManager.OnUIEventHandler(this.OnApplyRequest));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Mentor_OpenMentorIntro, new CUIEventManager.OnUIEventHandler(this.OnOpenMentorIntro));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Mentor_WatchHisMentor, new CUIEventManager.OnUIEventHandler(this.OnWatchHisMentor));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Mentor_OpenMentorPage, new CUIEventManager.OnUIEventHandler(this.OnOpenMentorPage));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Mentor_ApplyRequest, new CUIEventManager.OnUIEventHandler(this.OnApplyRequest));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Mentor_OpenMentorIntro, new CUIEventManager.OnUIEventHandler(this.OnOpenMentorIntro));
		}

		public void UpdateUI()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile == null)
			{
				return;
			}
			this.m_famousMentorData = null;
			GameObject widget = form.GetWidget(21);
			GameObject widget2 = form.GetWidget(18);
			GameObject widget3 = form.GetWidget(19);
			GameObject widget4 = form.GetWidget(20);
			GameObject widget5 = form.GetWidget(22);
			GameObject widget6 = form.GetWidget(23);
			GameObject widget7 = form.GetWidget(24);
			GameObject widget8 = form.GetWidget(25);
			GameObject widget9 = form.GetWidget(26);
			GameObject widget10 = form.GetWidget(27);
			GameObject widget11 = form.GetWidget(28);
			GameObject widget12 = form.GetWidget(29);
			GameObject widget13 = form.GetWidget(30);
			widget12.CustomSetActive(true);
			widget13.CustomSetActive(false);
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_NoData");
			widget7.CustomSetActive(profile._mentorInfo.ullMasterUid == 0uL);
			widget5.CustomSetActive(profile._mentorInfo.ullMasterUid != 0uL);
			if (profile._mentorInfo.dwStudentNum != 0u)
			{
				widget3.GetComponent<Text>().text = profile._mentorInfo.dwStudentNum.ToString();
			}
			else
			{
				widget3.GetComponent<Text>().text = text;
			}
			if (profile._mentorInfo.dwFinishStudentNum != 0u)
			{
				widget4.GetComponent<Text>().text = profile._mentorInfo.dwFinishStudentNum.ToString();
			}
			else
			{
				widget4.GetComponent<Text>().text = text;
			}
			string text2 = Utility.UTF8Convert(profile._mentorInfo.szRoleName);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = text;
			}
			enMentorState mentorState = CFriendContoller.GetMentorState((int)profile.GetHistoryHighestRankGrade(), null);
			if (mentorState == enMentorState.IWantMentor || mentorState == enMentorState.IHasMentor || profile._mentorInfo.dwMasterPoint == 0u)
			{
				this.SetTempNoData(form, text2);
			}
			else
			{
				widget10.CustomSetActive(true);
				widget8.CustomSetActive(true);
				widget9.CustomSetActive(true);
				GameDataMgr.famousMentorDatabin.Accept(new Action<ResFamousMentor>(this.FamousMentorInVisitor));
				if (this.m_famousMentorData == null)
				{
					this.SetTempNoData(form, text2);
					return;
				}
				widget11.CustomSetActive(true);
				widget8.GetComponent<Text>().text = this.m_famousMentorData.szTitle;
				string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, this.m_famousMentorData.iLevel.ToString());
				CUIUtility.SetImageSprite(widget11.GetComponent<Image>(), prefabPath, null, true, false, false, false);
				widget2.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mentor_PlayerInfo", new string[]
				{
					this.m_famousMentorData.iLevel.ToString(),
					this.m_famousMentorData.szTitle,
					profile._mentorInfo.dwMasterPoint.ToString(),
					text2
				});
				ResFamousMentor dataByKey = GameDataMgr.famousMentorDatabin.GetDataByKey(this.m_famousMentorData.dwID + 1u);
				if (dataByKey == null)
				{
					widget9.GetComponent<Text>().text = profile._mentorInfo.dwMasterPoint + string.Empty;
				}
				else
				{
					widget9.GetComponent<Text>().text = profile._mentorInfo.dwMasterPoint + "/" + dataByKey.dwPoint;
				}
				uint num = 0u;
				if (this.m_famousMentorData.dwID != 1u)
				{
					ResFamousMentor dataByKey2 = GameDataMgr.famousMentorDatabin.GetDataByKey(this.m_famousMentorData.dwID - 1u);
					if (dataByKey2 != null)
					{
						num = dataByKey2.dwPoint;
					}
				}
				if (this.m_famousMentorData.dwPoint == num)
				{
					widget10.GetComponent<Image>().fillAmount = 0f;
				}
				else
				{
					widget10.GetComponent<Image>().fillAmount = (profile._mentorInfo.dwMasterPoint - num) / (this.m_famousMentorData.dwPoint - num);
				}
			}
			this.MentorBtnUIUpdate();
			widget6.CustomSetActive(this.mentorBtnStr != null);
			if (this.mentorBtnStr != null)
			{
				widget6.transform.Find("Text").GetComponent<Text>().text = this.mentorBtnStr;
			}
		}

		public void MentorBtnUIUpdate()
		{
			this.mentorStateStr = null;
			this.mentorBtnStr = null;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (!profile.isMasterData)
			{
				int bLogicGrade = (int)CLadderSystem.GetGradeDataByShowGrade((int)masterRoleInfo.m_rankHistoryHighestGrade).bLogicGrade;
				int bLogicGrade2 = (int)CLadderSystem.GetGradeDataByShowGrade((int)profile.GetHistoryHighestRankGrade()).bLogicGrade;
				enMentorState mentorState = CFriendContoller.GetMentorState((int)profile.GetHistoryHighestRankGrade(), null);
				enMentorState mentorState2 = CFriendContoller.GetMentorState(bLogicGrade, null);
				if ((mentorState == enMentorState.IHasApprentice || mentorState == enMentorState.IWantApprentice) && bLogicGrade2 > bLogicGrade)
				{
					this.mentorStateStr = Singleton<CTextManager>.GetInstance().GetText("Mentor_GetMentor");
					this.mentorBtnStr = Singleton<CTextManager>.GetInstance().GetText("Mentor_ProposeMentor");
					this.m_addViewtype = 2;
					if (masterRoleInfo.m_mentorInfo != null && masterRoleInfo.m_mentorInfo.ullMasterUid != 0uL)
					{
						this.mentorStateStr = null;
					}
				}
				else if ((mentorState2 == enMentorState.IHasApprentice || mentorState2 == enMentorState.IWantApprentice) && bLogicGrade2 < bLogicGrade)
				{
					this.mentorBtnStr = Singleton<CTextManager>.GetInstance().GetText("Mentor_ProposeApprentice");
					this.mentorStateStr = Singleton<CTextManager>.GetInstance().GetText("Mentor_GetApprentice");
					this.m_addViewtype = 3;
				}
			}
		}

		private void SetTempNoData(CUIFormScript form, string mentorName)
		{
			GameObject widget = form.GetWidget(21);
			GameObject widget2 = form.GetWidget(18);
			GameObject widget3 = form.GetWidget(22);
			GameObject widget4 = form.GetWidget(23);
			GameObject widget5 = form.GetWidget(24);
			GameObject widget6 = form.GetWidget(28);
			GameObject widget7 = form.GetWidget(27);
			GameObject widget8 = form.GetWidget(25);
			GameObject widget9 = form.GetWidget(26);
			GameObject widget10 = form.GetWidget(29);
			GameObject widget11 = form.GetWidget(30);
			widget10.CustomSetActive(false);
			widget11.CustomSetActive(true);
			widget7.CustomSetActive(false);
			widget8.CustomSetActive(false);
			widget9.CustomSetActive(false);
			Text component = widget2.GetComponent<Text>();
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_NoData");
			component.text = Singleton<CTextManager>.GetInstance().GetText("Mentor_PlayerInfo", new string[]
			{
				text,
				text,
				text,
				mentorName
			});
			widget6.CustomSetActive(false);
		}

		private void FamousMentorInVisitor(ResFamousMentor resFmsMentor)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile == null)
			{
				return;
			}
			if (this.m_famousMentorData == null)
			{
				this.m_famousMentorData = resFmsMentor;
				return;
			}
			if (this.m_famousMentorData.dwPoint < profile._mentorInfo.dwMasterPoint && resFmsMentor.dwPoint <= profile._mentorInfo.dwMasterPoint)
			{
				this.m_famousMentorData = resFmsMentor;
			}
		}

		private void OnWatchHisMentor(CUIEvent uievt)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile == null)
			{
				return;
			}
			Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(profile._mentorInfo.ullMasterUid, (int)profile._mentorInfo.dwMasterLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
		}

		private void OnOpenMentorPage(CUIEvent uievt)
		{
			CFriendView.OpenFriendTab(CFriendView.Tab.Mentor);
			uievt.m_srcFormScript.Close();
			Singleton<CUIManager>.GetInstance().CloseForm(RankingSystem.s_rankingForm);
		}

		private void OnApplyRequest(CUIEvent uievt)
		{
			if (Singleton<CPlayerInfoSystem>.instance.GetProfile() == null)
			{
				return;
			}
			string stringPlacer = null;
			string title = null;
			if (this.mentorStateStr != null)
			{
				title = Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqTitle", new string[]
				{
					this.mentorStateStr
				});
				stringPlacer = Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqReplacer", new string[]
				{
					this.mentorStateStr
				});
			}
			Singleton<CUIManager>.GetInstance().OpenStringSenderBox(title, Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqDesc"), stringPlacer, new CUIManager.StringSendboxOnSend(this.OnMentorApplyVerifyBoxRetrun), CFriendView.Verfication.GetRandomMentorReuqestStr(this.m_addViewtype));
		}

		private void OnOpenMentorIntro(CUIEvent uievt)
		{
			Singleton<CUIManager>.GetInstance().OpenInfoForm(21);
		}

		public void OnMentorApplyVerifyBoxRetrun(string str)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile == null)
			{
				return;
			}
			FriendSysNetCore.Send_Request_BeMentor(profile.m_uuid, (uint)profile.m_iLogicWorldId, this.m_addViewtype, str);
		}
	}
}
