using Apollo;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CInviteView
	{
		public enum enInviteFormWidget
		{
			Friend_Panel,
			GuildMember_Panel,
			Friend_List,
			FriendEmpty_Panel,
			FriendTotalNum_Text,
			GuildMember_List,
			GuildMemberTotalNum_Text,
			InviteTab_List,
			RefreshGuildMemberGameState_Timer,
			Bottom_Widget,
			LBS_Panel,
			LBS_List,
			LBSTotalNum_Text,
			ReverseTipsGo
		}

		public enum enInviteListTab
		{
			Friend,
			GuildMember,
			LBS,
			Count
		}

		public static string[] TabName = new string[]
		{
			Singleton<CTextManager>.GetInstance().GetText("Invite_Tab_Title_Friend"),
			Singleton<CTextManager>.GetInstance().GetText("Invite_Tab_Title_Guild"),
			Singleton<CTextManager>.GetInstance().GetText("Invite_Tab_Title_LBS")
		};

		public static CInviteView.enInviteListTab GetInviteListTab(int index)
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return (CInviteView.enInviteListTab)index;
			}
			return (index > 0) ? (index + CInviteView.enInviteListTab.GuildMember) : CInviteView.enInviteListTab.Friend;
		}

		public static string GetTabName(int index)
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return CInviteView.TabName[index];
			}
			return (index > 0) ? CInviteView.TabName[index + 1] : CInviteView.TabName[0];
		}

		public static int GetTabCount()
		{
			return Singleton<CGuildSystem>.GetInstance().IsInNormalGuild() ? 3 : 2;
		}

		public static void InitListTab(CUIFormScript form)
		{
			CUIListScript component = form.GetWidget(7).GetComponent<CUIListScript>();
			int tabCount = CInviteView.GetTabCount();
			component.SetElementAmount(tabCount);
			for (int i = 0; i < component.GetElementAmount(); i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				elemenet.transform.Find("txtName").GetComponent<Text>().set_text(CInviteView.GetTabName(i));
			}
			component.SelectElement(0, true);
		}

		public static void SetInviteFriendData(CUIFormScript form, COM_INVITE_JOIN_TYPE joinType)
		{
			ListView<COMDT_FRIEND_INFO> allFriendList = Singleton<CInviteSystem>.instance.GetAllFriendList();
			int count = allFriendList.Count;
			int num = 0;
			CUIListScript component = form.GetWidget(2).GetComponent<CUIListScript>();
			component.SetElementAmount(count);
			form.GetWidget(3).gameObject.CustomSetActive(allFriendList.Count == 0);
			for (int i = 0; i < count; i++)
			{
				if (allFriendList[i].bIsOnline == 1)
				{
					num++;
				}
			}
			Text component2 = form.GetWidget(4).GetComponent<Text>();
			component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Online_Member", new string[]
			{
				num.ToString(),
				count.ToString()
			}));
			GameObject widget = form.GetWidget(13);
			Text component3 = widget.GetComponent<Text>();
			if (component3 != null)
			{
				if (Singleton<CFriendContoller>.instance.model.friendReserve.BServerEnableReverse)
				{
					component3.set_text(Singleton<CTextManager>.instance.GetText("Reserve_Condition_Tip"));
				}
				else
				{
					component3.set_text(Singleton<CTextManager>.instance.GetText("Reserve_ServerDisable_Tip"));
				}
			}
			GameObject widget2 = form.GetWidget(9);
			if (CSysDynamicBlock.bLobbyEntryBlocked || ApolloConfig.IsUseCEPackage() >= 1)
			{
				widget2.CustomSetActive(false);
			}
			else
			{
				Text component4 = Utility.FindChild(widget2, "ShareInviteButton/Text").GetComponent<Text>();
				GameObject gameObject = widget2.transform.FindChild("ShareInviteButton/IconQQ").gameObject;
				GameObject gameObject2 = widget2.transform.FindChild("ShareInviteButton/IconWeixin").gameObject;
				bool flag = Singleton<CRoomSystem>.GetInstance().IsInRoom || Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam;
				if (flag)
				{
					widget2.CustomSetActive(true);
					if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
					{
						component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Share_Room_Info_QQ"));
						gameObject.CustomSetActive(true);
						gameObject2.CustomSetActive(false);
					}
					else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
					{
						component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Share_Room_Info_WX"));
						gameObject.CustomSetActive(false);
						gameObject2.CustomSetActive(true);
					}
				}
				else
				{
					widget2.CustomSetActive(false);
				}
			}
		}

		public static void SetInviteGuildMemberData(CUIFormScript form)
		{
			ListView<GuildMemInfo> allGuildMemberList = Singleton<CInviteSystem>.GetInstance().GetAllGuildMemberList();
			int count = allGuildMemberList.Count;
			int num = 0;
			CInviteView.RefreshInviteGuildMemberList(form, count);
			for (int i = 0; i < count; i++)
			{
				if (CGuildHelper.IsMemberOnline(allGuildMemberList[i]))
				{
					num++;
				}
			}
			Text component = form.GetWidget(6).GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Online_Member", new string[]
			{
				num.ToString(),
				count.ToString()
			}));
		}

		public static void SetLBSData(CUIFormScript form, COM_INVITE_JOIN_TYPE joinType)
		{
			CUIListScript component = form.GetWidget(11).GetComponent<CUIListScript>();
			Text component2 = form.GetWidget(12).GetComponent<Text>();
			int elementAmount = 0;
			if (Singleton<CFriendContoller>.instance.model.EnableShareLocation)
			{
				ListView<CSDT_LBS_USER_INFO> lBSList = Singleton<CFriendContoller>.instance.model.GetLBSList(CFriendModel.LBSGenderType.Both);
				elementAmount = ((lBSList != null) ? lBSList.Count : 0);
				component.SetElementAmount(elementAmount);
				Utility.FindChild(form.GetWidget(10), "Empty/Normal").CustomSetActive(true);
				Utility.FindChild(form.GetWidget(10), "Empty/GotoBtn").CustomSetActive(false);
			}
			else
			{
				component.SetElementAmount(0);
				Utility.FindChild(form.GetWidget(10), "Empty/Normal").CustomSetActive(false);
				Utility.FindChild(form.GetWidget(10), "Empty/GotoBtn").CustomSetActive(true);
			}
			component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Online_Member", new string[]
			{
				elementAmount.ToString(),
				elementAmount.ToString()
			}));
		}

		public static void RefreshInviteGuildMemberList(CUIFormScript form, int allGuildMemberLen)
		{
			CUIListScript component = form.GetWidget(5).GetComponent<CUIListScript>();
			component.SetElementAmount(allGuildMemberLen);
		}

		public static void UpdateFriendListElement(GameObject element, COMDT_FRIEND_INFO friend)
		{
			CInviteView.UpdateFriendListElementBase(element, ref friend);
			CInviteView.SetFriendState(element, ref friend);
		}

		public static void UpdateLBSListElement(GameObject element, CSDT_LBS_USER_INFO LBSInfo)
		{
			CInviteView.UpdateFriendListElementBase(element, ref LBSInfo.stLbsUserInfo);
			CInviteView.SetLBSState(element, ref LBSInfo);
		}

		public static string ConnectPlayerNameAndNickName(byte[] szUserName, string nickName)
		{
			if (szUserName == null)
			{
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(nickName))
			{
				return string.Format("{0}({1})", Utility.UTF8Convert(szUserName), nickName);
			}
			return Utility.UTF8Convert(szUserName);
		}

		public static void UpdateFriendListElementBase(GameObject element, ref COMDT_FRIEND_INFO friend)
		{
			GameObject gameObject = element.transform.FindChild("HeadBg").gameObject;
			Text component = element.transform.FindChild("PlayerName").GetComponent<Text>();
			Image component2 = element.transform.FindChild("NobeIcon").GetComponent<Image>();
			Image component3 = element.transform.FindChild("HeadBg/NobeImag").GetComponent<Image>();
			if (component2)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component2, (int)friend.stGameVip.dwCurLevel, false, false, friend.ullUserPrivacyBits);
			}
			if (component3)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component3, (int)friend.stGameVip.dwHeadIconId);
			}
			CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(friend.stUin.ullUid, friend.stUin.dwLogicWorldId);
			if (friendInGaming == null)
			{
				component.set_text(CInviteView.ConnectPlayerNameAndNickName(friend.szUserName, string.Empty));
			}
			else
			{
				component.set_text(CInviteView.ConnectPlayerNameAndNickName(friend.szUserName, friendInGaming.NickName));
			}
			string url = Utility.UTF8Convert(friend.szHeadUrl);
			if (!CSysDynamicBlock.bFriendBlocked)
			{
				CUIUtility.GetComponentInChildren<CUIHttpImageScript>(gameObject).SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
			}
		}

		private static void SetFriendState(GameObject element, ref COMDT_FRIEND_INFO friend)
		{
			if (element == null || friend == null)
			{
				return;
			}
			CFriendModel model = Singleton<CFriendContoller>.instance.model;
			GameObject gameObject = element.transform.FindChild("HeadBg").gameObject;
			GameObject gameObject2 = element.transform.FindChild("InviteButton").gameObject;
			Text component = element.transform.FindChild("Online").GetComponent<Text>();
			Text component2 = element.transform.FindChild("PlayerName").GetComponent<Text>();
			GameObject obj = Utility.FindChild(element, "HeadBg/AntiDisturbBits");
			obj.CustomSetActive(false);
			Text component3 = element.transform.FindChild("Time").GetComponent<Text>();
			bool flag = false;
			if (component3 != null)
			{
				component3.gameObject.CustomSetActive(false);
			}
			if (component != null)
			{
				component.gameObject.CustomSetActive(true);
			}
			GameObject gameObject3 = Utility.FindChild(element, "ReserveButton");
			gameObject3.CustomSetActive(false);
			GameObject gameObject4 = Utility.FindChild(element, "ReserveText");
			gameObject4.CustomSetActive(false);
			CInviteView.SetListElementLadderInfo(element, friend);
			COM_ACNT_GAME_STATE cOM_ACNT_GAME_STATE = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
			if (friend.bIsOnline == 1)
			{
				CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(friend.stUin.ullUid, friend.stUin.dwLogicWorldId);
				if (friendInGaming == null)
				{
					cOM_ACNT_GAME_STATE = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
				}
				else
				{
					cOM_ACNT_GAME_STATE = friendInGaming.State;
					flag = ((friendInGaming.antiDisturbBits & 1u) == 1u);
				}
				if (cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
				{
					if (flag)
					{
						component.set_text(Singleton<CTextManager>.instance.GetText("Common_NotDisturb"));
						obj.CustomSetActive(true);
					}
					else
					{
						component.set_text(Singleton<CInviteSystem>.instance.GetInviteStateStr(friend.stUin.ullUid, false));
					}
					if (friendInGaming != null && friendInGaming.IsUseTGA())
					{
						component.gameObject.CustomSetActive(true);
						component.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("TGA_Friend_State")));
					}
					CUIEventScript component4 = gameObject2.GetComponent<CUIEventScript>();
					component4.m_onClickEventParams.tag = (int)Singleton<CInviteSystem>.instance.InviteType;
					component4.m_onClickEventParams.tag2 = (int)friend.stUin.dwLogicWorldId;
					component4.m_onClickEventParams.commonUInt64Param1 = friend.stUin.ullUid;
				}
				else if (cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME || cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME || cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_WAITMULTIGAME)
				{
					if (friendInGaming == null)
					{
						component.gameObject.CustomSetActive(true);
						component.set_text("friendInGame is null");
						return;
					}
					if (friendInGaming.startTime > 0u)
					{
						component.gameObject.CustomSetActive(false);
						component3.gameObject.CustomSetActive(true);
						component3.set_text(string.Format(Singleton<CTextManager>.instance.GetText("Common_Gaming_Time"), CInviteView.GetStartMinute(friendInGaming.startTime)));
						Singleton<CInviteSystem>.instance.CheckInviteListGameTimer();
						if (Singleton<CFriendContoller>.instance.model.friendReserve.BServerEnableReverse)
						{
							FriendReserve.Ent ent = model.friendReserve.Find(friend.stUin.ullUid, friend.stUin.dwLogicWorldId, FriendReserve.ReserveDataType.Send);
							ushort num;
							CFriendModel.EIntimacyType eIntimacyType;
							bool flag2;
							model.GetFriendIntimacy(friend.stUin.ullUid, friend.stUin.dwLogicWorldId, out num, out eIntimacyType, out flag2);
							if (ent == null)
							{
								bool flag3 = (friendInGaming.antiDisturbBits & 1u) > 0u;
								bool flag4 = (friend.dwRefuseFriendBits & 16u) == 0u;
								if (flag4 && !flag3 && num >= 100)
								{
									gameObject3.CustomSetActive(true);
									gameObject4.CustomSetActive(false);
									if (gameObject3 != null)
									{
										CUIEventScript component5 = gameObject3.GetComponent<CUIEventScript>();
										component5.m_onClickEventParams.commonUInt64Param1 = friend.stUin.ullUid;
										component5.m_onClickEventParams.tagUInt = friend.stUin.dwLogicWorldId;
										component5.m_onClickEventParams.tagStr = Utility.UTF8Convert(friend.szUserName);
									}
								}
							}
							else
							{
								gameObject3.CustomSetActive(false);
								gameObject4.CustomSetActive(true);
								if (gameObject4 != null)
								{
									Text component6 = gameObject4.GetComponent<Text>();
									if (component6 != null)
									{
										if (ent != null && ent.result == 2)
										{
											component6.set_text(model.friendReserve.Reserve_Success);
										}
										else if (ent != null && ent.result == 1)
										{
											component6.set_text(model.friendReserve.Reserve_Failed);
										}
										else
										{
											component6.set_text(model.friendReserve.Reserve_Wait4Rsp);
										}
									}
								}
							}
						}
					}
					else
					{
						component.gameObject.CustomSetActive(true);
						component.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming_NoTime")));
					}
				}
				else if (cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
				{
					component.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Teaming")));
				}
				component2.set_color(CUIUtility.s_Color_White);
				CUIUtility.GetComponentInChildren<Image>(gameObject).set_color(CUIUtility.s_Color_White);
			}
			else
			{
				component.set_text(string.Format(Singleton<CTextManager>.instance.GetText("Common_Offline"), new object[0]));
				component2.set_color(CUIUtility.s_Color_Grey);
				CUIUtility.GetComponentInChildren<Image>(gameObject).set_color(CUIUtility.s_Color_GrayShader);
			}
			gameObject2.CustomSetActive(friend.bIsOnline == 1 && !flag && cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE);
		}

		private static int GetStartMinute(uint startTime)
		{
			DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			DateTime dateTime2 = Utility.ToUtcTime2Local((long)((ulong)startTime));
			if (dateTime < dateTime2)
			{
				return 1;
			}
			int value = (int)(dateTime - dateTime2).get_TotalMinutes();
			return Mathf.Clamp(value, 1, 99);
		}

		private static void SetLBSState(GameObject element, ref CSDT_LBS_USER_INFO LBSInfo)
		{
			COMDT_FRIEND_INFO stLbsUserInfo = LBSInfo.stLbsUserInfo;
			GameObject gameObject = element.transform.FindChild("HeadBg").gameObject;
			Text component = element.transform.FindChild("Online").GetComponent<Text>();
			GameObject gameObject2 = element.transform.FindChild("InviteButton").gameObject;
			Text component2 = element.transform.FindChild("PlayerName").GetComponent<Text>();
			GameObject obj = Utility.FindChild(element, "HeadBg/AntiDisturbBits");
			obj.CustomSetActive(false);
			bool flag = false;
			CInviteView.SetListElementLadderInfo(element, stLbsUserInfo);
			COM_ACNT_GAME_STATE cOM_ACNT_GAME_STATE = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
			if (stLbsUserInfo.bIsOnline == 1)
			{
				cOM_ACNT_GAME_STATE = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
				CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(stLbsUserInfo.stUin.ullUid, stLbsUserInfo.stUin.dwLogicWorldId);
				if (friendInGaming != null)
				{
					cOM_ACNT_GAME_STATE = friendInGaming.State;
					flag = ((friendInGaming.antiDisturbBits & 1u) == 1u);
				}
				if (cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
				{
					if (flag)
					{
						component.set_text(Singleton<CTextManager>.instance.GetText("Common_NotDisturb"));
						obj.CustomSetActive(true);
					}
					else
					{
						component.set_text(Singleton<CInviteSystem>.instance.GetInviteStateStr(stLbsUserInfo.stUin.ullUid, false));
					}
					CUIEventScript component3 = gameObject2.GetComponent<CUIEventScript>();
					component3.m_onClickEventParams.tag = (int)Singleton<CInviteSystem>.instance.InviteType;
					component3.m_onClickEventParams.tag2 = (int)stLbsUserInfo.stUin.dwLogicWorldId;
					component3.m_onClickEventParams.tag3 = (int)LBSInfo.dwGameSvrEntity;
					component3.m_onClickEventParams.commonUInt64Param1 = stLbsUserInfo.stUin.ullUid;
				}
				else if (cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME || cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME || cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_WAITMULTIGAME)
				{
					component.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming")));
				}
				else if (cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
				{
					component.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Teaming")));
				}
				component2.set_color(CUIUtility.s_Color_White);
				CUIUtility.GetComponentInChildren<Image>(gameObject).set_color(CUIUtility.s_Color_White);
			}
			else
			{
				component.set_text(string.Format(Singleton<CTextManager>.instance.GetText("Common_Offline"), new object[0]));
				component2.set_color(CUIUtility.s_Color_Grey);
				CUIUtility.GetComponentInChildren<Image>(gameObject).set_color(CUIUtility.s_Color_GrayShader);
			}
			gameObject2.CustomSetActive(stLbsUserInfo.bIsOnline == 1 && !flag && cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE);
		}

		public static void UpdateGuildMemberListElement(GameObject element, GuildMemInfo guildMember, bool isGuildMatchInvite)
		{
			Transform transform = element.transform;
			GameObject gameObject = transform.FindChild("HeadBg").gameObject;
			GameObject gameObject2 = transform.FindChild("InviteButton").gameObject;
			Text component = transform.FindChild("PlayerName").GetComponent<Text>();
			Text component2 = transform.FindChild("Online").GetComponent<Text>();
			Image component3 = transform.FindChild("NobeIcon").GetComponent<Image>();
			Image component4 = transform.FindChild("HeadBg/NobeImag").GetComponent<Image>();
			Text component5 = transform.FindChild("Time").GetComponent<Text>();
			if (component5 != null)
			{
				component5.gameObject.CustomSetActive(false);
			}
			if (component2 != null)
			{
				component2.gameObject.CustomSetActive(true);
			}
			GameObject obj = Utility.FindChild(element, "HeadBg/AntiDisturbBits");
			obj.CustomSetActive(false);
			Transform transform2 = transform.FindChild("RemindButton");
			GameObject gameObject3 = null;
			if (transform2 != null)
			{
				gameObject3 = transform2.gameObject;
				gameObject3.CustomSetActive(false);
			}
			CInviteView.SetListElementLadderInfo(element, guildMember);
			if (component3)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component3, (int)guildMember.stBriefInfo.stVip.level, false, true, 0uL);
			}
			if (component4)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component4, (int)guildMember.stBriefInfo.stVip.headIconId);
			}
			component.set_text(Utility.UTF8Convert(guildMember.stBriefInfo.sName));
			if (CGuildHelper.IsMemberOnline(guildMember))
			{
				if (guildMember.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
				{
					bool flag = (guildMember.antiDisturbBits & 1u) == 1u;
					if (flag)
					{
						component2.set_text(Singleton<CTextManager>.instance.GetText("Common_NotDisturb"));
						obj.CustomSetActive(true);
					}
					else
					{
						component2.set_text(Singleton<CInviteSystem>.instance.GetInviteStateStr(guildMember.stBriefInfo.uulUid, isGuildMatchInvite));
					}
					CUIEventScript component6 = gameObject2.GetComponent<CUIEventScript>();
					component6.m_onClickEventParams.tag = (int)Singleton<CInviteSystem>.instance.InviteType;
					component6.m_onClickEventParams.tag2 = guildMember.stBriefInfo.dwLogicWorldId;
					component6.m_onClickEventParams.commonUInt64Param1 = guildMember.stBriefInfo.uulUid;
				}
				else if (guildMember.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME || guildMember.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME || guildMember.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_WAITMULTIGAME)
				{
					if (guildMember.dwGameStartTime > 0u)
					{
						if (component2 != null)
						{
							component2.gameObject.CustomSetActive(false);
						}
						if (component5 != null)
						{
							component5.gameObject.CustomSetActive(true);
						}
						if (component5 != null)
						{
							component5.set_text(string.Format(Singleton<CTextManager>.instance.GetText("Common_Gaming_Time"), CInviteView.GetStartMinute(guildMember.dwGameStartTime)));
						}
						Singleton<CInviteSystem>.instance.CheckInviteListGameTimer();
					}
					else
					{
						if (component2 != null)
						{
							component2.gameObject.CustomSetActive(true);
						}
						if (component2 != null)
						{
							component2.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming_NoTime")));
						}
					}
				}
				else if (guildMember.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
				{
					component2.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Teaming")));
				}
				component.set_color(CUIUtility.s_Color_White);
				CUIUtility.GetComponentInChildren<Image>(gameObject).set_color(CUIUtility.s_Color_White);
			}
			else
			{
				component2.set_text(string.Format(Singleton<CTextManager>.instance.GetText("Common_Offline"), new object[0]));
				component.set_color(CUIUtility.s_Color_Grey);
				CUIUtility.GetComponentInChildren<Image>(gameObject).set_color(CUIUtility.s_Color_GrayShader);
			}
			bool flag2 = CGuildHelper.IsMemberOnline(guildMember) && guildMember.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
			if (isGuildMatchInvite)
			{
				Transform transform3 = transform.Find("TeamState");
				if (transform3 != null)
				{
					Text component7 = transform3.GetComponent<Text>();
					if (Singleton<CGuildMatchSystem>.GetInstance().IsInAnyTeam(guildMember.stBriefInfo.uulUid))
					{
						gameObject2.CustomSetActive(false);
						if (component7 != null)
						{
							component7.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_In_Team"));
						}
					}
					else
					{
						bool flag3 = !CGuildHelper.IsMemberOnline(guildMember) && !guildMember.isGuildMatchOfflineInvitedByHostPlayer;
						gameObject2.CustomSetActive(flag2 || flag3);
						if (component7 != null)
						{
							if (!CGuildHelper.IsMemberOnline(guildMember) && guildMember.isGuildMatchOfflineInvitedByHostPlayer)
							{
								component7.set_text("<color=#e49316>" + Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invite_State_Wait_For_Response") + "</color>");
							}
							else
							{
								component7.set_text(string.Empty);
							}
						}
					}
				}
			}
			else
			{
				gameObject2.CustomSetActive(flag2);
			}
			string szHeadUrl = guildMember.stBriefInfo.szHeadUrl;
			CUIUtility.GetComponentInChildren<CUIHttpImageScript>(gameObject).SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(szHeadUrl));
			if (isGuildMatchInvite && Singleton<CGuildMatchSystem>.GetInstance().IsInGuildMatchTime() && Singleton<CGuildMatchSystem>.GetInstance().IsInTeam(guildMember.GuildMatchInfo.ullTeamLeaderUid, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID) && CGuildHelper.IsMemberOnline(guildMember) && guildMember.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE && !Convert.ToBoolean(guildMember.GuildMatchInfo.bIsReady) && gameObject3 != null)
			{
				gameObject3.CustomSetActive(true);
				CUIEventScript component8 = gameObject3.GetComponent<CUIEventScript>();
				component8.m_onClickEventParams.commonUInt64Param1 = guildMember.stBriefInfo.uulUid;
			}
		}

		private static void SetListElementLadderInfo(GameObject element, COMDT_FRIEND_INFO friendInfo)
		{
			GameObject gameObject = element.transform.Find("RankCon").gameObject;
			if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
			int num;
			int rankStar;
			CInviteView.GetFriendRankGradeAndStar(friendInfo, out num, out rankStar);
			bool flag = Singleton<CLadderSystem>.GetInstance().IsHaveFightRecord(false, num, rankStar);
			if (flag)
			{
				gameObject.CustomSetActive(true);
				CLadderView.ShowRankDetail(gameObject, (byte)num, (uint)((byte)friendInfo.dwRankClass), friendInfo.bIsOnline != 1, true);
			}
		}

		private static void SetListElementLadderInfo(GameObject element, GuildMemInfo guildMemInfo)
		{
			if (guildMemInfo == null)
			{
				return;
			}
			GameObject gameObject = element.transform.Find("RankCon").gameObject;
			if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
			int num;
			int rankStar;
			CInviteView.GetGuildMemberGradeAndStar(guildMemInfo, out num, out rankStar);
			bool flag = Singleton<CLadderSystem>.GetInstance().IsHaveFightRecord(false, num, rankStar);
			if (flag)
			{
				gameObject.CustomSetActive(true);
				CLadderView.ShowRankDetail(gameObject, (byte)num, (uint)((byte)guildMemInfo.stBriefInfo.dwClassOfRank), !CGuildHelper.IsMemberOnline(guildMemInfo), true);
			}
		}

		private static void GetFriendRankGradeAndStar(COMDT_FRIEND_INFO friendInfo, out int rankGrade, out int rankStar)
		{
			if (friendInfo != null && friendInfo.RankVal != null)
			{
				rankGrade = (int)friendInfo.stRankShowGrade.bGrade;
				rankStar = (int)friendInfo.stRankShowGrade.dwScore;
			}
			else
			{
				rankGrade = 0;
				rankStar = 0;
			}
		}

		private static void GetGuildMemberGradeAndStar(GuildMemInfo guildMemInfo, out int rankGrade, out int rankStar)
		{
			if (guildMemInfo != null)
			{
				rankGrade = (int)guildMemInfo.stBriefInfo.rankGrade.bGrade;
				rankStar = (int)guildMemInfo.stBriefInfo.rankGrade.dwScore;
			}
			else
			{
				rankGrade = 0;
				rankStar = 0;
			}
		}
	}
}
