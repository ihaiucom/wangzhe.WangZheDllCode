using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class FriendShower : MonoBehaviour
	{
		public enum ItemType
		{
			Add,
			Normal,
			Request,
			BlackList,
			LBS,
			MentorRequest,
			AddMentor,
			AddApprentice,
			Mentor,
			Apprentice
		}

		public ulong ullUid;

		public uint dwLogicWorldID;

		public Button sendHeartButton;

		public Button inviteGuildButton;

		public Button PKButton;

		public Button reCallButton;

		public Button OBButton;

		public Text VerifyText;

		public Text NameText;

		public Text LevelText;

		public Text VipLevel;

		public Text time;

		public Image headIcon;

		public Image sendHeartIcon;

		public Text pvpText;

		public Image pvpIcon;

		public Text distanceTxt;

		public GameObject lbsBodyNode;

		public GameObject intimacyNode;

		public GameObject full;

		public GameObject high;

		public GameObject mid;

		public GameObject low;

		public GameObject freeze;

		public Text intimacyNum;

		public Text reCallText;

		public Text inviteGuildBtnText;

		public CUIHttpImageScript HttpImage;

		public Text SendBtnText;

		public GameObject nobeIcon;

		public GameObject HeadIconBack;

		public GameObject QQVipImage;

		public GameObject ChatButton;

		public CUIEventScript inviteGuildBtn_eventScript;

		public CUIEventScript sendHeartBtn_eventScript;

		public CUIEventScript reCallBtn_eventScript;

		public GameObject add_node;

		public GameObject normal_node;

		public GameObject request_node;

		public GameObject del_node;

		public GameObject black_node;

		public GameObject lbs_node;

		public GameObject addMentor_node;

		public GameObject addApprentice_node;

		public GameObject mentorInfo_node;

		public GameObject mentorLv_node;

		public GameObject mentor_relationship;

		public GameObject mentor_graduation;

		public Button lbsAddFriendBtn;

		public GameObject genderImage;

		public GameObject platChannelIcon;

		public GameObject Giftutton;

		public Text m_mentorTitleObj;

		public GameObject m_RankIconObj;

		public Text m_mentorFamousObj;

		public CUIFormScript formScript;

		public void SetFriendItemType(FriendShower.ItemType type, bool bShowDelete = true)
		{
			this.Showuid(this.ullUid, this.dwLogicWorldID);
			if (this.inviteGuildButton != null)
			{
				this.inviteGuildButton.gameObject.CustomSetActive(false);
			}
			this.add_node.CustomSetActive(false);
			this.normal_node.CustomSetActive(false);
			this.request_node.CustomSetActive(false);
			this.black_node.CustomSetActive(false);
			this.addApprentice_node.CustomSetActive(false);
			this.addMentor_node.CustomSetActive(false);
			this.lbs_node.CustomSetActive(false);
			this.mentorInfo_node.CustomSetActive(false);
			switch (type)
			{
			case FriendShower.ItemType.Add:
				UT.SetAddNodeActive(this.add_node, CFriendModel.FriendType.Recommend, false);
				break;
			case FriendShower.ItemType.Normal:
			case FriendShower.ItemType.Mentor:
			case FriendShower.ItemType.Apprentice:
				this.normal_node.CustomSetActive(true);
				if (this.del_node != null)
				{
					this.del_node.CustomSetActive(bShowDelete || type != FriendShower.ItemType.Normal);
				}
				break;
			case FriendShower.ItemType.Request:
			case FriendShower.ItemType.MentorRequest:
				this.request_node.CustomSetActive(true);
				break;
			case FriendShower.ItemType.BlackList:
				this.black_node.CustomSetActive(true);
				break;
			case FriendShower.ItemType.LBS:
				if (this.lbs_node != null)
				{
					this.lbs_node.CustomSetActive(true);
				}
				break;
			case FriendShower.ItemType.AddMentor:
				this.mentorInfo_node.CustomSetActive(true);
				this.addMentor_node.CustomSetActive(true);
				break;
			case FriendShower.ItemType.AddApprentice:
				this.addApprentice_node.CustomSetActive(true);
				break;
			}
			if (this.del_node != null)
			{
				CUIEventScript component = this.del_node.GetComponent<CUIEventScript>();
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = enUIEventID.Friend_DelFriend;
				uIEvent.m_eventParams.tag = (int)type;
				component.SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
			}
			if (this.Giftutton != null)
			{
				CUIEventScript component2 = this.Giftutton.GetComponent<CUIEventScript>();
				CUIEvent uIEvent2 = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent2.m_eventID = enUIEventID.Friend_Gift;
				uIEvent2.m_eventParams.tag = (int)type;
				component2.SetUIEvent(enUIEventType.Click, uIEvent2.m_eventID, uIEvent2.m_eventParams);
			}
			if (this.ChatButton != null)
			{
				CUIEventScript component3 = this.ChatButton.GetComponent<CUIEventScript>();
				CUIEvent uIEvent3 = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent3.m_eventID = enUIEventID.Friend_Chat_Button;
				uIEvent3.m_eventParams.tag = (int)type;
				component3.SetUIEvent(enUIEventType.Click, uIEvent3.m_eventID, uIEvent3.m_eventParams);
			}
			if (this.VerifyText != null)
			{
				this.VerifyText.transform.parent.gameObject.CustomSetActive(type == FriendShower.ItemType.Request || type == FriendShower.ItemType.MentorRequest);
			}
			if (this.lbsBodyNode != null)
			{
				this.lbsBodyNode.gameObject.CustomSetActive(type == FriendShower.ItemType.LBS);
			}
		}

		public void SetBGray(bool bGray)
		{
			UT.SetImage(this.headIcon, bGray);
		}

		public void ShowPKButton(bool b)
		{
			if (this.PKButton != null)
			{
				if (CSysDynamicBlock.bFriendBlocked && this.PKButton.gameObject.activeSelf)
				{
					this.PKButton.gameObject.SetActive(false);
					return;
				}
				if (!b)
				{
					if (this.PKButton.gameObject.activeSelf)
					{
						this.PKButton.gameObject.SetActive(false);
					}
					return;
				}
				if (!this.PKButton.gameObject.activeSelf)
				{
					this.PKButton.gameObject.SetActive(true);
				}
			}
		}

		public void ShowVerify(string text)
		{
			if (this.VerifyText != null)
			{
				this.VerifyText.text = Singleton<CFriendContoller>.instance.model.friend_static_text + text;
			}
		}

		public void ShowDistance(string txt)
		{
			if (this.distanceTxt != null)
			{
				this.distanceTxt.text = txt;
			}
		}

		private void Showuid(ulong ullUid, uint dwLogicWorldId)
		{
			Transform transform = base.transform.FindChild("body/uid");
			if (Singleton<CFriendContoller>.instance.model.bShowUID)
			{
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(true);
				}
				if (transform != null)
				{
					transform.GetComponent<Text>().text = string.Format("uid:{0},world:{1}", ullUid, this.dwLogicWorldID);
				}
			}
			else if (transform != null)
			{
				transform.gameObject.CustomSetActive(false);
			}
		}

		public void ShowIntimacyNum(int value, CFriendModel.EIntimacyType type, bool bFreeze, COM_INTIMACY_STATE state)
		{
			GameObject gameObject = this.full.transform.parent.FindChild("rela").gameObject;
			bool flag = IntimacyRelationViewUT.IsRelaState(state);
			bool flag2 = IntimacyRelationViewUT.IsRelaStateDeny(state);
			if (flag || flag2)
			{
				this.intimacyNum.gameObject.CustomSetActive(false);
				this.freeze.CustomSetActive(false);
				this.full.CustomSetActive(false);
				this.high.CustomSetActive(false);
				this.mid.CustomSetActive(false);
				this.low.CustomSetActive(false);
				gameObject.CustomSetActive(true);
				COM_INTIMACY_STATE cOM_INTIMACY_STATE = (!flag2) ? state : IntimacyRelationViewUT.GetStateByDenyState(state);
				if (cOM_INTIMACY_STATE != COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL)
				{
					string relaIcon = IntimacyRelationViewUT.GetRelaIcon(value, cOM_INTIMACY_STATE);
					if (!string.IsNullOrEmpty(relaIcon))
					{
						Image componetInChild = Utility.GetComponetInChild<Image>(gameObject, "icon");
						if (componetInChild != null)
						{
							componetInChild.gameObject.CustomSetActive(true);
							componetInChild.SetSprite(relaIcon, this.formScript, true, false, false, false);
						}
					}
					Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject, "txt");
					if (componetInChild2 != null)
					{
						componetInChild2.gameObject.CustomSetActive(true);
						componetInChild2.text = Singleton<CFriendContoller>.instance.model.GetPrefixString(value, state);
					}
				}
			}
			else
			{
				gameObject.CustomSetActive(false);
				if (this.intimacyNum != null)
				{
					this.intimacyNum.gameObject.CustomSetActive(true);
					if ((long)value >= (long)((ulong)Singleton<CFriendContoller>.instance.model.GetIntimacyMaxValue()))
					{
						this.intimacyNum.text = "Max";
					}
					else
					{
						this.intimacyNum.text = value.ToString();
					}
				}
				if (bFreeze)
				{
					this.freeze.CustomSetActive(true);
					this.intimacyNum.color = CUIUtility.Intimacy_Freeze;
				}
				else
				{
					this.freeze.CustomSetActive(false);
					if (type == CFriendModel.EIntimacyType.Low)
					{
						this.full.CustomSetActive(false);
						this.high.CustomSetActive(false);
						this.mid.CustomSetActive(false);
						this.low.CustomSetActive(true);
						this.intimacyNum.color = CUIUtility.Intimacy_Low;
					}
					else if (type == CFriendModel.EIntimacyType.Middle)
					{
						this.full.CustomSetActive(false);
						this.high.CustomSetActive(false);
						this.mid.CustomSetActive(true);
						this.low.CustomSetActive(false);
						this.intimacyNum.color = CUIUtility.Intimacy_Mid;
					}
					else if (type == CFriendModel.EIntimacyType.High)
					{
						this.full.CustomSetActive(false);
						this.high.CustomSetActive(true);
						this.mid.CustomSetActive(false);
						this.low.CustomSetActive(false);
						this.intimacyNum.color = CUIUtility.Intimacy_High;
					}
					else if (type == CFriendModel.EIntimacyType.full)
					{
						this.full.CustomSetActive(true);
						this.high.CustomSetActive(false);
						this.mid.CustomSetActive(false);
						this.low.CustomSetActive(false);
						this.intimacyNum.color = CUIUtility.Intimacy_Full;
					}
				}
			}
		}

		public void ShowName(string name)
		{
			if (this.NameText != null)
			{
				this.NameText.text = name;
			}
			if (this.pvpText != null)
			{
				this.pvpText.gameObject.CustomSetActive(false);
			}
			if (this.pvpIcon != null)
			{
				this.pvpIcon.gameObject.CustomSetActive(false);
			}
			if (this.sendHeartIcon != null)
			{
				this.sendHeartIcon.gameObject.CustomSetActive(false);
			}
		}

		public void ShowLevel(uint level)
		{
			if (this.LevelText != null)
			{
				this.LevelText.text = "LV." + level.ToString();
			}
		}

		public void ShowVipLevel(uint level)
		{
			if (this.VipLevel != null)
			{
				this.VipLevel.text = "VIP." + level.ToString();
			}
		}

		public void ShowLastTime(bool bShow, string text)
		{
			if (this.time != null)
			{
				this.time.gameObject.CustomSetActive(bShow);
				this.time.text = text;
			}
		}

		public void ShowGameState(CFriendModel.FriendInGame st, bool bOnline)
		{
			COM_ACNT_GAME_STATE cOM_ACNT_GAME_STATE = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
			if (st != null)
			{
				cOM_ACNT_GAME_STATE = st.State;
			}
			if (cOM_ACNT_GAME_STATE != COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE && bOnline)
			{
				this.ShowLastTime(true, "游戏中");
			}
			else if (st != null && cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE && st.IsUseTGA())
			{
				this.ShowLastTime(true, Singleton<CTextManager>.instance.GetText("TGA_Friend_State"));
			}
		}

		public void ShowPVP_Level(string text, string icon)
		{
		}

		public void ShowInviteButton(bool isShow, bool isEnable)
		{
			if (this.reCallButton == null)
			{
				return;
			}
			if (CSysDynamicBlock.bFriendBlocked)
			{
				this.reCallButton.gameObject.CustomSetActive(false);
				return;
			}
			if (!isShow)
			{
				this.reCallButton.gameObject.CustomSetActive(false);
				return;
			}
			if (this.reCallText != null)
			{
				if (isEnable)
				{
					this.reCallText.text = Singleton<CTextManager>.instance.GetText("Friend_ReCall_Tips_1");
				}
				else
				{
					this.reCallText.text = Singleton<CTextManager>.instance.GetText("Friend_ReCall_Tips_2");
				}
			}
			this.reCallButton.gameObject.CustomSetActive(true);
			if (this.reCallBtn_eventScript != null)
			{
				this.reCallBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_SNS_ReCall);
			}
			CUICommonSystem.SetButtonEnableWithShader(this.reCallButton, isEnable, true);
		}

		public void ShowOBButton(bool isShow)
		{
			if (this.OBButton == null || this.OBButton.gameObject == null)
			{
				return;
			}
			if (CSysDynamicBlock.bFriendBlocked)
			{
				this.OBButton.gameObject.CustomSetActive(false);
				return;
			}
			this.OBButton.gameObject.CustomSetActive(isShow);
		}

		public void ShowinviteGuild(bool isShow, bool isEnable)
		{
			if (this.inviteGuildButton == null)
			{
				return;
			}
			if (CSysDynamicBlock.bFriendBlocked)
			{
				this.inviteGuildButton.gameObject.SetActive(false);
				return;
			}
			if (!isShow)
			{
				this.inviteGuildButton.gameObject.CustomSetActive(false);
				return;
			}
			this.inviteGuildButton.gameObject.CustomSetActive(true);
			CUICommonSystem.SetButtonEnableWithShader(this.inviteGuildButton, isEnable, true);
			if (this.inviteGuildBtn_eventScript != null)
			{
				this.inviteGuildBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_InviteGuild);
			}
		}

		public void ShowRecommendGuild(bool isShow, bool isEnabled)
		{
			if (this.inviteGuildButton == null)
			{
				return;
			}
			if (CSysDynamicBlock.bFriendBlocked && this.inviteGuildButton.gameObject.activeSelf)
			{
				this.inviteGuildButton.gameObject.SetActive(false);
				return;
			}
			if (!isShow)
			{
				if (this.inviteGuildButton.gameObject.activeSelf)
				{
					this.inviteGuildButton.gameObject.CustomSetActive(false);
				}
				return;
			}
			if (!this.inviteGuildButton.gameObject.activeSelf)
			{
				this.inviteGuildButton.gameObject.CustomSetActive(true);
			}
			if (this.inviteGuildBtn_eventScript != null)
			{
				this.inviteGuildBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_RecommendGuild);
			}
			if (isEnabled)
			{
				CUICommonSystem.SetButtonEnable(this.inviteGuildButton, true, true, true);
				if (this.inviteGuildBtnText != null)
				{
					this.inviteGuildBtnText.text = Singleton<CFriendContoller>.instance.model.Guild_Recommend_txt;
				}
			}
			else
			{
				CUICommonSystem.SetButtonEnable(this.inviteGuildButton, false, false, true);
				if (this.inviteGuildBtnText != null)
				{
					this.inviteGuildBtnText.text = Singleton<CFriendContoller>.instance.model.Guild_Has_Recommended_txt;
				}
			}
		}

		public void ShowSendButton(bool bEnable)
		{
			if (this.sendHeartButton == null || this.sendHeartButton.gameObject == null)
			{
				return;
			}
			CUICommonSystem.SetButtonEnableWithShader(this.sendHeartButton, bEnable, true);
			this.sendHeartButton.gameObject.CustomSetActive(true);
		}

		public void ShowSendGiftBtn(bool bShow)
		{
			if (this.Giftutton)
			{
				if (CSysDynamicBlock.bFriendBlocked)
				{
					this.Giftutton.CustomSetActive(false);
					return;
				}
				this.Giftutton.CustomSetActive(bShow);
			}
		}

		public void HideSendButton()
		{
			if (this.sendHeartButton)
			{
				this.sendHeartButton.gameObject.CustomSetActive(false);
			}
		}

		public void ShowGenderType(COM_SNSGENDER genderType)
		{
			FriendShower.ShowGender(this.genderImage, genderType);
		}

		public static void ShowGender(GameObject genderImage, COM_SNSGENDER genderType)
		{
			if (genderImage == null)
			{
				return;
			}
			genderImage.gameObject.CustomSetActive(genderType != COM_SNSGENDER.COM_SNSGENDER_NONE);
			if (genderType == COM_SNSGENDER.COM_SNSGENDER_MALE)
			{
				CUIUtility.SetImageSprite(genderImage.GetComponent<Image>(), string.Format("{0}icon/Ico_boy", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
			}
			else if (genderType == COM_SNSGENDER.COM_SNSGENDER_FEMALE)
			{
				CUIUtility.SetImageSprite(genderImage.GetComponent<Image>(), string.Format("{0}icon/Ico_girl", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
			}
		}

		public void ShowMentorSearchInfo(COMDT_FRIEND_INFO info, CFriendModel.FriendType friendType, FriendShower.ItemType type)
		{
			enMentorState mentorState = CFriendContoller.GetMentorState(-1, null);
			bool bActive = UT.NeedShowGenderGradeByMentor(type, friendType) && (mentorState == enMentorState.IWantMentor || mentorState == enMentorState.IHasMentor);
			if (this.m_mentorTitleObj != null)
			{
				this.m_mentorTitleObj.transform.parent.gameObject.CustomSetActive(true);
				this.m_mentorTitleObj.gameObject.CustomSetActive(bActive);
				string text = string.Empty;
				try
				{
					text = GameDataMgr.famousMentorDatabin.GetDataByKey(info.dwMasterLvl).szTitle;
				}
				catch (Exception var_3_84)
				{
				}
				this.m_mentorTitleObj.GetComponent<Text>().text = text;
			}
			if (this.m_mentorFamousObj != null)
			{
				this.m_mentorFamousObj.gameObject.CustomSetActive(bActive);
				this.m_mentorFamousObj.text = Singleton<CTextManager>.GetInstance().GetText("Mentor_LvNApprenticeCountInfo", new string[]
				{
					info.dwMasterLvl.ToString(),
					info.dwStudentNum.ToString()
				});
			}
			if (this.m_RankIconObj != null)
			{
				int bGrade = (int)info.stRankShowGrade.bGrade;
				CLadderView.ShowRankDetail(this.m_RankIconObj.transform.parent.gameObject, (byte)bGrade, Singleton<RankingSystem>.GetInstance().GetRankClass(info.stUin.ullUid), 1u, false, true, false, false, true);
			}
			if (UT.NeedShowGenderGradeByMentor(type, friendType))
			{
				this.ShowGenderType((COM_SNSGENDER)info.bGender);
			}
		}

		public void ShowPlatChannelIcon(COMDT_FRIEND_INFO info)
		{
			if (this.platChannelIcon != null)
			{
				this.platChannelIcon.CustomSetActive(!Utility.IsSamePlatformWithSelf(info.stUin.dwLogicWorldId));
				if (CSysDynamicBlock.bLobbyEntryBlocked)
				{
					this.platChannelIcon.CustomSetActive(false);
				}
			}
		}
	}
}
