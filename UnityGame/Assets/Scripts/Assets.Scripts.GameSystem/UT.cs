using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class UT
	{
		public enum FriendResultType
		{
			RequestBeFriend
		}

		public static CFriendRelationship FRData()
		{
			return Singleton<CFriendContoller>.instance.model.FRData;
		}

		public static void Add2List<T>(T data, ListView<T> list)
		{
			if (data == null || list == null)
			{
				return;
			}
			list.Add(data);
		}

		public static bool BEqual_ACNT_UNIQ(COMDT_ACNT_UNIQ a, COMDT_ACNT_UNIQ b, bool ingore_worldid = false)
		{
			if (!ingore_worldid)
			{
				return a.ullUid == b.ullUid && a.dwLogicWorldId == b.dwLogicWorldId;
			}
			return a.ullUid == b.ullUid;
		}

		public static string Bytes2String(byte[] bytes)
		{
			return Encoding.get_UTF8().GetString(bytes).TrimEnd(new char[1]);
		}

		public static string Bytes2String(string str)
		{
			return str;
		}

		public static byte[] String2Bytes(string name)
		{
			return Encoding.get_UTF8().GetBytes(name);
		}

		public static string GetTimeString(uint time)
		{
			string result = string.Empty;
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)time));
			DateTime dateTime2 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			if (DateTime.Compare(dateTime2, dateTime) > 0)
			{
				TimeSpan timeSpan = dateTime2 - dateTime;
				if (timeSpan.get_Days() == 0)
				{
					if (timeSpan.get_Hours() == 0)
					{
						result = string.Format(UT.GetText("Friend_Tips_lastTime_min"), timeSpan.get_Minutes());
					}
					else
					{
						result = string.Format(UT.GetText("Friend_Tips_lastTime_hour_min"), timeSpan.get_Hours(), timeSpan.get_Minutes());
					}
				}
				else
				{
					int num = timeSpan.get_Days();
					if (num > 7)
					{
						num = 7;
					}
					result = string.Format(UT.GetText("Friend_Tips_lastTime_day"), num);
				}
			}
			return result;
		}

		public static int CalcDeltaHorus(uint fromT, uint toT)
		{
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)fromT));
			DateTime dateTime2 = Utility.ToUtcTime2Local((long)((ulong)toT));
			if (DateTime.Compare(dateTime2, dateTime) > 0)
			{
				return Math.Max((int)(dateTime2 - dateTime).get_TotalHours(), 1);
			}
			return 0;
		}

		public static void ShowFriendNetResult(uint dwResult, UT.FriendResultType type)
		{
			string strContent = string.Empty;
			if (dwResult == 0u)
			{
				strContent = UT.GetFriendResultTypeString(type);
			}
			else
			{
				strContent = UT.ErrorCode_String(dwResult);
			}
			Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
		}

		public static string GetFriendResultTypeString(UT.FriendResultType type)
		{
			if (type != UT.FriendResultType.RequestBeFriend)
			{
				return string.Empty;
			}
			return UT.GetText("Friend_Tips_Send_BeFriend_Ok");
		}

		public static string ErrorCode_String(uint dwResult)
		{
			switch (dwResult)
			{
			case 171u:
				return UT.GetText("CS_ERR_FRIEND_IN_BLACK");
			case 172u:
			case 173u:
			case 176u:
			case 177u:
			case 178u:
			case 179u:
			case 182u:
			case 183u:
			case 184u:
			case 185u:
				IL_76:
				switch (dwResult)
				{
				case 144u:
					return UT.GetText("Friend_CS_ERR_FRIEND_RECALL_REPEATED");
				case 145u:
					return UT.GetText("Friend_CS_ERR_FRIEND_EXCEED");
				case 146u:
					return UT.GetText("Friend_CS_ERR_FRIEND_RECALL_TIME_LIMIT");
				case 147u:
				case 148u:
				case 149u:
				case 150u:
				case 153u:
				case 155u:
				case 156u:
				case 157u:
				case 158u:
				case 159u:
					IL_D2:
					switch (dwResult)
					{
					case 101u:
						return UT.GetText("Friend_CS_ERR_FRIEND_TCAPLUS_ERR");
					case 102u:
						return UT.GetText("Friend_CS_ERR_FRIEND_RECORD_NOT_EXSIST");
					case 103u:
						return UT.GetText("Friend_CS_ERR_FRIEND_NUM_EXCEED");
					case 104u:
						return UT.GetText("Friend_CS_ERR_PEER_FRIEND_NUM_EXCEED");
					case 105u:
						return UT.GetText("Friend_CS_ERR_FRIEND_DONATE_AP_EXCEED");
					case 106u:
						return UT.GetText("Friend_CS_ERR_FRIEND_RECV_AP_EXCEED");
					case 107u:
						return UT.GetText("Friend_CS_ERR_FRIEND_ADD_FRIEND_DENY");
					case 108u:
						return UT.GetText("Friend_CS_ERR_FRIEND_ADD_FRIEND_SELF");
					case 109u:
						return UT.GetText("Friend_CS_ERR_FRIEND_ADD_FRIEND_EXSIST");
					case 110u:
						return UT.GetText("Friend_CS_ERR_FRIEND_REQ_REPEATED");
					case 111u:
						return UT.GetText("Friend_CS_ERR_FRIEND_NOT_EXSIST");
					case 112u:
						return UT.GetText("Friend_CS_ERR_FRIEND_SEND_MAIL");
					case 113u:
						return UT.GetText("Friend_CS_ERR_FRIEND_DONATE_REPEATED");
					case 114u:
						return UT.GetText("Friend_CS_ERR_FRIEND_AP_FULL");
					case 115u:
						IL_11F:
						switch (dwResult)
						{
						case 37u:
							return UT.GetText("CS_ERR_RECRUITER_LEVELLIMIT");
						case 38u:
						case 39u:
						case 40u:
						case 41u:
							IL_15C:
							switch (dwResult)
							{
							case 2u:
								return UT.GetText("Friend_CS_ERR_STARTSINGLEGAME_FAIL");
							case 4u:
								return UT.GetText("Friend_CS_ERR_FINSINGLEGAME_FAIL");
							case 5u:
								return UT.GetText("Friend_CS_ERR_QUITMULTGAME_FAIL");
							case 6u:
								return UT.GetText("Friend_CS_ERR_REGISTER_NAME_DUP_FAIL");
							case 7u:
								return UT.GetText("Friend_CS_ERR_SHOULD_REFRESH_TASK");
							case 8u:
								return UT.GetText("Friend_CS_ERR_COMMIT_ERR");
							}
							return string.Format(UT.GetText("Friend_CS_ERR_FRIEND_DEFAULT"), dwResult);
						case 42u:
							return UT.GetText("CS_ERR_RECRUIT_INVITECODE");
						case 43u:
							return UT.GetText("CS_ERR_RECRUIT_LEVELLIMIT");
						case 44u:
							return UT.GetText("CS_ERR_RECRUIT_NOTONEPLAT");
						case 45u:
							return UT.GetText("CS_ERR_PASSIVE_RECRUIT_NUMLIMIT");
						case 46u:
							return UT.GetText("CS_ERR_ACTIVE_RECRUIT_NUMLIMIT");
						case 47u:
							return UT.GetText("CS_ERR_RECRUIT_OTHER");
						case 48u:
							return UT.GetText("CS_ERR_RECRUIT_SELF");
						case 49u:
							return UT.GetText("CS_ERR_RECRUIT_CLOSING");
						}
						goto IL_15C;
					case 116u:
						return UT.GetText("Friend_CS_ERR_FRIEND_ADD_FRIEND_ZONE");
					case 117u:
						return UT.GetText("Friend_CS_ERR_FRIEND_OTHER");
					}
					goto IL_11F;
				case 151u:
					return UT.GetText("CS_ERR_REFUSE_RECALL_REPEATED");
				case 152u:
					return UT.GetText("CS_ERR_REFUSE_ADDFRIEND");
				case 154u:
					return UT.GetText("CS_ERR_VERIFICATION_ILLEGAL");
				case 160u:
					return UT.GetText("CS_ERR_DEFRIEND_REPEATED");
				case 161u:
					return UT.GetText("CS_ERR_BLACKLIST_NOT_EXSIST");
				case 162u:
					return UT.GetText("CS_ERR_BLACKLIST_EXCEED");
				case 163u:
					return UT.GetText("CS_ERR_FRIEND_INVALID_PLAT");
				}
				goto IL_D2;
			case 174u:
				return UT.GetText("CS_ERR_LBS_LIMIT");
			case 175u:
				return UT.GetText("CS_ERR_LBSSERECH_TIMELIMIT");
			case 180u:
				return UT.GetText("CS_ERR_FRIEND_ADD_LOCK");
			case 181u:
				return UT.GetText("CS_ERR_FRIEND_ADD_TOO_OFTEN");
			case 186u:
				return UT.GetText("CS_ERR_REQUEST_LIST_NUM_EXCEED");
			case 187u:
				return UT.GetText("CS_ERR_INTIMACY_REQUEST_SELF");
			case 188u:
				return UT.GetText("CS_ERR_INTIMACY_REQUEST_TIME_LIMIT");
			case 189u:
				return UT.GetText("CS_ERR_INTIMACY_VALUE_NOTENOUGH");
			case 190u:
				return UT.GetText("CS_ERR_INTIMACY_REQUEST_REPEATED");
			case 191u:
				return UT.GetText("CS_ERR_INTIMACY_RELATION_NUM_EXCEED");
			case 192u:
				return UT.GetText("CS_ERR_PEER_INTIMACY_RELATION_NUM_EXCEED");
			case 193u:
				return UT.GetText("CS_ERR_INTIMACY_RELATION_EXSIST");
			case 194u:
				return UT.GetText("CS_ERR_INTIMACY_RELATION_NOTEXIST");
			case 195u:
				return UT.GetText("CS_ERR_INTIMACY_RELATION_OTHER");
			case 196u:
				return UT.GetText("CS_ERR_INTIMACY_RELATION_OFTEN");
			}
			goto IL_76;
		}

		public static void SetListIndex(CUIListScript com, int index)
		{
			com.m_alwaysDispatchSelectedChangeEvent = true;
			com.SelectElement(index, true);
			com.m_alwaysDispatchSelectedChangeEvent = false;
		}

		public static void SetChatFace(CUIFormScript formScript, Image img, int index)
		{
			img.SetSprite(string.Format("UGUI/Sprite/Dynamic/ChatFace/{0}", index), formScript, true, false, false, false);
		}

		public static void If_Null_Error<T>(T v) where T : class
		{
			if (v == null)
			{
			}
		}

		public static void Check_AddHeartCD(COMDT_ACNT_UNIQ uniq)
		{
			if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() == null)
			{
				return;
			}
			Singleton<CFriendContoller>.GetInstance().model.HeartData.Add(uniq);
		}

		public static void Check_AddReCallCD(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
		{
			Singleton<CFriendContoller>.GetInstance().model.SnsReCallData.Add(uniq, friendType);
		}

		public static void SetImage(Image img, bool bGray)
		{
			if (img != null)
			{
				img.set_color(bGray ? new Color(0f, 1f, 1f) : new Color(1f, 1f, 1f, 1f));
			}
		}

		public static bool DebugBattleMiniMapConfigInfo(SLevelContext data)
		{
			if (data == null)
			{
				return false;
			}
			if (!data.IsMobaMode())
			{
				return false;
			}
			bool flag = true;
			string text = string.Format("---地图，id:{0},name:{1},", data.m_mapID, data.m_levelName);
			if (string.IsNullOrEmpty(data.m_miniMapPath))
			{
				text += string.Format("缩略图路径配置为空,", new object[0]);
				flag = false;
			}
			if (string.IsNullOrEmpty(data.m_bigMapPath))
			{
				text += string.Format("地图路径配置为空,", new object[0]);
				flag = false;
			}
			if (data.m_mapWidth == 0)
			{
				text += string.Format("地图宽度配置为空,", new object[0]);
				flag = false;
			}
			if (data.m_mapHeight == 0)
			{
				text += string.Format("地图高度配置为空,", new object[0]);
				flag = false;
			}
			return flag && flag;
		}

		public static void ResetTimer(int timer, bool bPause)
		{
			Singleton<CTimerManager>.GetInstance().PauseTimer(timer);
			Singleton<CTimerManager>.GetInstance().ResetTimer(timer);
			if (!bPause)
			{
				Singleton<CTimerManager>.GetInstance().ResumeTimer(timer);
			}
		}

		public static string GetText(string key)
		{
			return Singleton<CTextManager>.instance.GetText(key);
		}

		public static void SetHttpImage(CUIHttpImageScript HttpImage, byte[] szHeadUrl)
		{
			if (szHeadUrl == null)
			{
				UT.SetHttpImage(HttpImage, string.Empty);
			}
			else
			{
				UT.SetHttpImage(HttpImage, UT.Bytes2String(szHeadUrl));
			}
		}

		public static void SetHttpImage(CUIHttpImageScript HttpImage, string url)
		{
			if (HttpImage == null)
			{
				return;
			}
			if (CSysDynamicBlock.bFriendBlocked)
			{
				return;
			}
			if (url == null)
			{
				url = string.Empty;
			}
			if (HttpImage.gameObject.activeSelf)
			{
				HttpImage.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
			}
		}

		public static bool NeedShowGenderGradeByMentor(FriendShower.ItemType type, CFriendModel.FriendType friendType)
		{
			return friendType == CFriendModel.FriendType.MentorRecommend || type == FriendShower.ItemType.AddMentor || type == FriendShower.ItemType.AddApprentice || friendType == CFriendModel.FriendType.MentorRequestList;
		}

		public static void SetAddNodeActive(GameObject addnode, CFriendModel.FriendType friendType, bool ifGrey = false)
		{
			if (addnode != null)
			{
				addnode.CustomSetActive(true);
				Transform transform = addnode.transform.Find("add_btn");
				if (transform != null)
				{
					GameObject gameObject = transform.gameObject;
					if (gameObject != null)
					{
						gameObject.CustomSetActive(!ifGrey);
						stUIEventParams eventParams = default(stUIEventParams);
						eventParams.tag = (int)friendType;
						CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
						if (component != null)
						{
							component.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_RequestBeFriend, eventParams);
						}
					}
				}
			}
		}

		public static void SetMentorLv(GameObject mentorLvGo, int mentorLv)
		{
			if (mentorLvGo == null)
			{
				return;
			}
			mentorLvGo.CustomSetActive(mentorLv > 0);
			Transform transform = mentorLvGo.transform.Find("Text");
			if (transform != null)
			{
				transform.GetComponent<Text>().set_text(mentorLv.ToString());
			}
		}

		public static void ShowFriendData(COMDT_FRIEND_INFO info, FriendShower com, FriendShower.ItemType type, bool bShowNickName, CFriendModel.FriendType friendType, CUIFormScript form, bool useMask = true)
		{
			if (info == null || com == null)
			{
				return;
			}
			com.ullUid = info.stUin.ullUid;
			com.dwLogicWorldID = info.stUin.dwLogicWorldId;
			UT.SetHttpImage(com.HttpImage, info.szHeadUrl);
			if (com.nobeIcon)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(com.nobeIcon.GetComponent<Image>(), (int)info.stGameVip.dwCurLevel, false, false, info.ullUserPrivacyBits);
			}
			if (com.HeadIconBack)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(com.HeadIconBack.GetComponent<Image>(), (int)info.stGameVip.dwHeadIconId);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(com.HeadIconBack.GetComponent<Image>(), (int)info.stGameVip.dwHeadIconId, form, 1f, useMask);
			}
			if (com.QQVipImage)
			{
				MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(com.QQVipImage.GetComponent<Image>(), (int)info.dwQQVIPMask);
			}
			UT.SetMentorLv(com.mentorLv_node, (int)info.dwMasterLvl);
			if ((type == FriendShower.ItemType.Normal && friendType == CFriendModel.FriendType.GameFriend) || type == FriendShower.ItemType.Mentor || type == FriendShower.ItemType.Apprentice)
			{
				CFriendModel model = Singleton<CFriendContoller>.instance.model;
				ushort num;
				CFriendModel.EIntimacyType type2;
				bool bFreeze;
				if (model.GetFriendIntimacy(info.stUin.ullUid, info.stUin.dwLogicWorldId, out num, out type2, out bFreeze))
				{
					if (num == 0)
					{
						com.intimacyNode.CustomSetActive(false);
					}
					else
					{
						com.intimacyNode.CustomSetActive(true);
						CFR cfr = model.FRData.GetCfr(info.stUin.ullUid, info.stUin.dwLogicWorldId);
						if (cfr != null)
						{
							com.ShowIntimacyNum((int)num, type2, bFreeze, cfr.state);
						}
						else
						{
							com.ShowIntimacyNum((int)num, type2, bFreeze, COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL);
						}
					}
				}
				else
				{
					com.intimacyNode.CustomSetActive(false);
				}
			}
			else
			{
				com.intimacyNode.CustomSetActive(false);
			}
			com.SetFriendItemType(type, !bShowNickName);
			bool flag = type == FriendShower.ItemType.Normal || type == FriendShower.ItemType.Mentor || type == FriendShower.ItemType.Apprentice || type == FriendShower.ItemType.AddMentor || type == FriendShower.ItemType.AddApprentice || type == FriendShower.ItemType.MentorRequest;
			com.SetBGray(flag && info.bIsOnline != 1);
			com.ShowLevel(info.dwPvpLvl);
			com.ShowVipLevel(info.dwVipLvl);
			com.ShowLastTime(info.bIsOnline != 1, UT.GetTimeString(info.dwLastLoginTime));
			switch (type)
			{
			case FriendShower.ItemType.Request:
			{
				string friendVerifyContent = Singleton<CFriendContoller>.instance.model.GetFriendVerifyContent(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Friend);
				com.ShowVerify(friendVerifyContent);
				break;
			}
			case FriendShower.ItemType.MentorRequest:
			{
				string friendVerifyContent2 = Singleton<CFriendContoller>.instance.model.GetFriendVerifyContent(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Mentor);
				com.ShowVerify(friendVerifyContent2);
				break;
			}
			}
			CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(info.stUin.ullUid, info.stUin.dwLogicWorldId);
			string text;
			if (friendInGaming == null)
			{
				text = UT.Bytes2String(info.szUserName);
				com.ShowGameState(friendInGaming, info.bIsOnline == 1);
			}
			else
			{
				if (!string.IsNullOrEmpty(friendInGaming.NickName) && bShowNickName)
				{
					text = string.Format("{0}({1})", UT.Bytes2String(info.szUserName), friendInGaming.NickName);
				}
				else
				{
					text = UT.Bytes2String(info.szUserName);
				}
				com.ShowGameState(friendInGaming, info.bIsOnline == 1);
			}
			switch (type)
			{
			case FriendShower.ItemType.Request:
			{
				string friendSourceDesc = UT.GetFriendSourceDesc(info.stUin.ullUid, Singleton<CFriendContoller>.instance.model.GetFriendVerifySource(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Friend));
				if (!string.IsNullOrEmpty(friendSourceDesc))
				{
					text = text + " " + friendSourceDesc;
				}
				break;
			}
			case FriendShower.ItemType.MentorRequest:
			{
				COMDT_FRIEND_SOURCE friendVerifySource = Singleton<CFriendContoller>.instance.model.GetFriendVerifySource(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Mentor);
				if (friendVerifySource != null)
				{
					string friendSourceDesc2 = UT.GetFriendSourceDesc(info.stUin.ullUid, friendVerifySource);
					if (!string.IsNullOrEmpty(friendSourceDesc2))
					{
						text = text + " " + friendSourceDesc2;
					}
				}
				break;
			}
			}
			com.ShowName(text);
			if (Singleton<CGuildSystem>.GetInstance().CanInvite(info))
			{
				if (Singleton<CGuildSystem>.GetInstance().HasInvited(info.stUin.ullUid))
				{
					com.ShowinviteGuild(true, false);
				}
				else
				{
					com.ShowinviteGuild(true, true);
				}
			}
			else if (Singleton<CGuildSystem>.GetInstance().CanRecommend(info))
			{
				if (Singleton<CGuildSystem>.GetInstance().HasRecommended(info.stUin.ullUid))
				{
					com.ShowRecommendGuild(true, false);
				}
				else
				{
					com.ShowRecommendGuild(true, true);
				}
			}
			else
			{
				com.ShowinviteGuild(false, false);
			}
			bool bEnable = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(info.stUin);
			com.ShowSendButton(bEnable);
			if (CSysDynamicBlock.bSocialBlocked)
			{
				com.HideSendButton();
			}
			if (com.m_mentorTitleObj != null && com.m_mentorTitleObj.transform.parent != null)
			{
				com.m_mentorTitleObj.transform.parent.gameObject.CustomSetActive(false);
			}
			if (UT.NeedShowGenderGradeByMentor(type, friendType))
			{
				com.ShowMentorSearchInfo(info, friendType, type);
				com.HideSendButton();
			}
			else if (friendType == CFriendModel.FriendType.Mentor || friendType == CFriendModel.FriendType.Apprentice)
			{
				com.ShowInviteButton(false, false);
			}
			else if (CSysDynamicBlock.bSocialBlocked)
			{
				com.ShowInviteButton(false, false);
			}
			else if (friendType == CFriendModel.FriendType.GameFriend)
			{
				com.ShowInviteButton(false, false);
			}
			else if (friendType == CFriendModel.FriendType.SNS)
			{
				bool isShow = CFriendReCallData.BLose(info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
				bool flag2 = Singleton<CFriendContoller>.instance.model.SnsReCallData.BInCd(info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
				bool flag3 = CFriendModel.IsOnSnsSwitch(info.dwRefuseFriendBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC);
				com.ShowInviteButton(isShow, !flag2 && !flag3);
			}
			if (Singleton<COBSystem>.instance.IsInOBFriendList(info.stUin.ullUid))
			{
				com.ShowOBButton(true);
			}
			else
			{
				com.ShowOBButton(false);
			}
			com.ShowGenderType((COM_SNSGENDER)info.bGender);
			com.ShowPlatChannelIcon(info);
			com.ShowSendGiftBtn(true);
			if (friendType == CFriendModel.FriendType.Mentor || friendType == CFriendModel.FriendType.Apprentice)
			{
				enMentorRelationType enMentorRelationType = (enMentorRelationType)(info.bStudentType >> 4);
				int num2 = (int)(info.bStudentType & 15);
				switch (enMentorRelationType)
				{
				case enMentorRelationType.mentor:
					if (com.mentor_relationship != null)
					{
						com.mentor_relationship.CustomSetActive(friendType == CFriendModel.FriendType.Mentor);
						com.mentor_relationship.transform.Find("MentorRelationText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_mentor"));
					}
					break;
				case enMentorRelationType.schoolmate:
					if (com.mentor_relationship != null)
					{
						com.mentor_relationship.CustomSetActive(friendType == CFriendModel.FriendType.Mentor);
						com.mentor_relationship.transform.Find("MentorRelationText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_schoolmate"));
					}
					if (com.normal_node != null)
					{
						com.normal_node.CustomSetActive(true);
					}
					for (int i = 0; i < com.normal_node.transform.childCount; i++)
					{
						Transform child = com.normal_node.transform.GetChild(i);
						if (child != null)
						{
							child.gameObject.CustomSetActive(false);
						}
					}
					break;
				}
				int num3 = num2;
				if (num3 != 1)
				{
					if (num3 == 2 && com.mentor_graduation != null)
					{
						com.mentor_graduation.CustomSetActive(friendType == CFriendModel.FriendType.Apprentice || enMentorRelationType == enMentorRelationType.schoolmate);
						com.mentor_graduation.transform.Find("MentorStatusText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_Graduated"));
						if (com.normal_node != null)
						{
							com.normal_node.CustomSetActive(true);
						}
						for (int j = 0; j < com.normal_node.transform.childCount; j++)
						{
							Transform child2 = com.normal_node.transform.GetChild(j);
							if (child2 != null)
							{
								child2.gameObject.CustomSetActive(false);
							}
						}
						if (com.del_node != null)
						{
							com.del_node.CustomSetActive(enMentorRelationType == enMentorRelationType.apprentice);
						}
					}
				}
				else if (com.mentor_graduation != null)
				{
					com.mentor_graduation.CustomSetActive(friendType == CFriendModel.FriendType.Apprentice || enMentorRelationType == enMentorRelationType.schoolmate);
					com.mentor_graduation.transform.Find("MentorStatusText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_InStudy"));
				}
				CFriendModel model2 = Singleton<CFriendContoller>.instance.model;
				UT.SetAddNodeActive(com.add_node, friendType, model2.IsSnsFriend(com.ullUid, com.dwLogicWorldID) || model2.IsGameFriend(com.ullUid, com.dwLogicWorldID));
			}
		}

		public static void ShowSNSFriendData(COMDT_SNS_FRIEND_INFO info, FriendShower com)
		{
			com.ullUid = info.ullUid;
			com.dwLogicWorldID = info.dwLogicWorldId;
			string url = UT.Bytes2String(info.szHeadUrl);
			com.HttpImage.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
			com.SetFriendItemType(FriendShower.ItemType.Add, true);
			com.SetBGray(false);
			com.ShowName(string.Format("{0}({1})", UT.Bytes2String(info.szRoleName), UT.Bytes2String(info.szNickName)));
			com.ShowLevel(info.dwPvpLvl);
			com.ShowVipLevel(info.dwPvpLvl);
			com.ShowLastTime(true, UT.GetTimeString(info.dwLastLoginTime));
		}

		public static void ShowBlackListData(ref CFriendModel.stBlackName info, FriendShower com)
		{
			if (com == null)
			{
				return;
			}
			com.ullUid = info.ullUid;
			com.dwLogicWorldID = info.dwLogicWorldId;
			com.HttpImage.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(info.szHeadUrl));
			if (com.nobeIcon)
			{
				com.nobeIcon.CustomSetActive(false);
			}
			if (com.HeadIconBack)
			{
				com.HeadIconBack.CustomSetActive(false);
			}
			if (com.QQVipImage)
			{
				com.QQVipImage.CustomSetActive(false);
			}
			com.SetFriendItemType(FriendShower.ItemType.BlackList, true);
			com.SetBGray(false);
			com.ShowName(info.name);
			com.ShowLevel(info.dwPvpLvl);
			com.ShowLastTime(true, UT.GetTimeString(info.dwLastLoginTime));
			com.intimacyNode.CustomSetActive(false);
			com.ShowGenderType((COM_SNSGENDER)info.bGender);
			UT.SetMentorLv(com.mentorLv_node, (int)info.dwMentorLv);
		}

		public static void ShowLBSUserData(CSDT_LBS_USER_INFO info, FriendShower com)
		{
			if (com == null)
			{
				return;
			}
			com.ullUid = info.stLbsUserInfo.stUin.ullUid;
			com.dwLogicWorldID = info.stLbsUserInfo.stUin.dwLogicWorldId;
			if (info.stLbsUserInfo.szHeadUrl != null)
			{
				string url = UT.Bytes2String(info.stLbsUserInfo.szHeadUrl);
				com.HttpImage.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
			}
			if (com.nobeIcon)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(com.nobeIcon.GetComponent<Image>(), (int)info.stLbsUserInfo.stGameVip.dwCurLevel, false, false, info.stLbsUserInfo.ullUserPrivacyBits);
			}
			if (com.HeadIconBack)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(com.HeadIconBack.GetComponent<Image>(), (int)info.stLbsUserInfo.stGameVip.dwHeadIconId);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(com.HeadIconBack.GetComponent<Image>(), (int)info.stLbsUserInfo.stGameVip.dwHeadIconId, com.HttpImage.m_belongedFormScript, 1f, true);
			}
			if (com.QQVipImage)
			{
				MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(com.QQVipImage.GetComponent<Image>(), (int)info.stLbsUserInfo.dwQQVIPMask);
			}
			if (com.mentorLv_node)
			{
				com.mentorLv_node.CustomSetActive(false);
			}
			com.intimacyNode.CustomSetActive(false);
			com.SetFriendItemType(FriendShower.ItemType.LBS, true);
			com.ShowName(UT.Bytes2String(info.stLbsUserInfo.szUserName));
			com.ShowLevel(info.stLbsUserInfo.dwPvpLvl);
			com.ShowLastTime(true, UT.GetTimeString(info.stLbsUserInfo.dwLastLoginTime));
			com.ShowGenderType((COM_SNSGENDER)info.stLbsUserInfo.bGender);
			com.ShowDistance(UT.GetDistance(info.dwDistance));
			com.SetBGray(info.stLbsUserInfo.bIsOnline != 1);
			if (com.platChannelIcon != null)
			{
				com.platChannelIcon.CustomSetActive(false);
			}
			if (com.lbsAddFriendBtn != null)
			{
				CFriendModel model = Singleton<CFriendContoller>.instance.model;
				if (model.IsSnsFriend(com.ullUid, com.dwLogicWorldID) || model.IsGameFriend(com.ullUid, com.dwLogicWorldID))
				{
					CUICommonSystem.SetButtonEnableWithShader(com.lbsAddFriendBtn, false, true);
				}
				else
				{
					CUICommonSystem.SetButtonEnableWithShader(com.lbsAddFriendBtn, true, true);
				}
			}
			GameObject gameObject = com.gameObject.transform.Find("body/LBS/Rank").gameObject;
			GameObject gameObject2 = com.gameObject.transform.Find("body/LBS/HisRank").gameObject;
			UT.ShowRank(com.formScript, gameObject, info.bGradeOfRank, info.stLbsUserInfo.dwRankClass);
			UT.ShowRank(com.formScript, gameObject2, info.bMaxGradeOfRank, info.stLbsUserInfo.dwRankClass);
		}

		public static string GetDistance(uint distance)
		{
			if (distance >= 1000u)
			{
				return string.Format("{0}公里", distance / 1000u);
			}
			return string.Format("{0}米", distance);
		}

		public static void SetShow(CanvasGroup cg, bool bShow)
		{
			if (cg == null)
			{
				return;
			}
			if (bShow)
			{
				cg.alpha = 1f;
				cg.blocksRaycasts = true;
			}
			else
			{
				cg.alpha = 0f;
				cg.blocksRaycasts = false;
			}
		}

		public static bool IsFreeze(uint lastTime)
		{
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)lastTime));
			DateTime dateTime2 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			return DateTime.Compare(dateTime2, dateTime) > 0 && (dateTime2 - dateTime).get_Days() >= Singleton<CFriendContoller>.instance.model.freezeDayCount;
		}

		public static void SetTabList(List<string> titles, int start_index, CUIListScript tablistScript)
		{
			if (tablistScript == null)
			{
				return;
			}
			DebugHelper.Assert(start_index < titles.get_Count(), "SetTabList, should start_index < titles.Count");
			tablistScript.SetElementAmount(titles.get_Count());
			for (int i = 0; i < tablistScript.m_elementAmount; i++)
			{
				CUIListElementScript elemenet = tablistScript.GetElemenet(i);
				Text component = elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>();
				component.set_text(titles.get_Item(i));
			}
			tablistScript.m_alwaysDispatchSelectedChangeEvent = true;
			tablistScript.SelectElement(start_index, true);
			tablistScript.m_alwaysDispatchSelectedChangeEvent = false;
		}

		public static void CheckGPS()
		{
			bool enableShareLocation = Singleton<CFriendContoller>.instance.model.EnableShareLocation;
			if (enableShareLocation && !MonoSingleton<GPSSys>.instance.bGetGPSData)
			{
				MonoSingleton<GPSSys>.instance.StartGPS();
			}
		}

		public static void ShowRank(CUIFormScript form, GameObject HisRankGo, byte RankGrade, uint RankClass)
		{
			if (form == null || HisRankGo == null)
			{
				return;
			}
			if (RankGrade == 0)
			{
				HisRankGo.CustomSetActive(false);
			}
			else
			{
				HisRankGo.CustomSetActive(true);
				Image image = null;
				Image image2 = null;
				if (HisRankGo != null)
				{
					image = Utility.GetComponetInChild<Image>(HisRankGo, "ImgRank");
					image2 = Utility.GetComponetInChild<Image>(HisRankGo, "ImgRank/ImgSubRank");
				}
				if (image != null)
				{
					string rankSmallIconPath = CLadderView.GetRankSmallIconPath(RankGrade, RankClass);
					image.SetSprite(rankSmallIconPath, form, true, false, false, false);
				}
				if (image2 != null)
				{
					string subRankSmallIconPath = CLadderView.GetSubRankSmallIconPath(RankGrade, RankClass);
					image2.SetSprite(subRankSmallIconPath, form, true, false, false, false);
				}
			}
		}

		public static string GetFriendSourceDesc(ulong friendUid, COMDT_FRIEND_SOURCE friendSource)
		{
			if (friendSource != null)
			{
				COM_ADD_FRIEND_TYPE bAddFriendType = (COM_ADD_FRIEND_TYPE)friendSource.bAddFriendType;
				if (bAddFriendType == COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_PVP)
				{
					string heroName = CHeroInfo.GetHeroName(friendSource.stAddFriendInfo.stPvp.dwHeroID);
					if (!string.IsNullOrEmpty(heroName))
					{
						return Singleton<CTextManager>.GetInstance().GetText("Friend_Apply_Play_With_You_Tip", new string[]
						{
							heroName
						});
					}
					DebugHelper.Assert(false, "好友来源是PVP，但却获取不了英雄名，heroId={0}", new object[]
					{
						friendSource.stAddFriendInfo.stPvp.dwHeroID
					});
				}
				else
				{
					if (CGuildHelper.IsInSameGuild(friendUid))
					{
						return Singleton<CTextManager>.GetInstance().GetText("Friend_Apply_Same_Guild_Tip");
					}
					if (bAddFriendType == COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_LBS)
					{
						return Singleton<CTextManager>.GetInstance().GetText("Friend_Apply_Nearby_You_Tip");
					}
				}
			}
			return string.Empty;
		}
	}
}
