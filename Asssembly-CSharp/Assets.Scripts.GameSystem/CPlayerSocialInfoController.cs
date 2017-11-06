using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using MiniJSON;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CPlayerSocialInfoController : Singleton<CPlayerSocialInfoController>
	{
		public const int k_maxTagSelectCount = 4;

		public const string k_defaultSelectionStr = "-";

		private const char k_splitChar = '~';

		private bool m_isInEditmode;

		private Transform[] m_searchTypeNodes;

		private Transform[] m_displyTypeNodes;

		private string[][] m_searchStrings;

		private int[][] m_searchVals;

		private List<object> m_addressBook;

		public static string s_socialTagEditFormPath = "UGUI/Form/System/Player/Form_SocialTag.prefab";

		private ArrayList m_tags;

		private Color m_colorTagCool = new Color(0.360784322f, 0.8039216f, 0.996078432f);

		private Color m_colorTagKawaii = new Color(1f, 0.5058824f, 0.482352942f);

		private Color m_colorNormal = new Color(0.8156863f, 0.843137264f, 0.8862745f);

		private Color m_colorHighlight = new Color(0.992156863f, 0.7254902f, 0f);

		private string m_configsStr = string.Empty;

		public override void Init()
		{
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_EditCard, new CUIEventManager.OnUIEventHandler(this.OnBeginEditCard));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_CancelEdit, new CUIEventManager.OnUIEventHandler(this.OnCancelEdit));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_SaveEdit, new CUIEventManager.OnUIEventHandler(this.OnSaveEdit));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_EditTag, new CUIEventManager.OnUIEventHandler(this.OnEditTag));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_TagSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnTagSelectChanged));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_TagEditEnd, new CUIEventManager.OnUIEventHandler(this.OnTagEditEnd));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_HideCard, new CUIEventManager.OnUIEventHandler(this.OnHideShowCard));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_AddFriend, new CUIEventManager.OnUIEventHandler(this.On_AddFriend));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_InviteGuild, new CUIEventManager.OnUIEventHandler(this.On_InviteGuild));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_HideCardConfirm, new CUIEventManager.OnUIEventHandler(this.OnHideShowCardConfirm));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_HideCardCancel, new CUIEventManager.OnUIEventHandler(this.OnHideShowCardCancel));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_CancelEditNotsave, new CUIEventManager.OnUIEventHandler(this.OnCancelEditNotSave));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Social_CancelEditSave, new CUIEventManager.OnUIEventHandler(this.OnCancelEditSave));
			this.m_tags = new ArrayList();
			this.m_isInEditmode = false;
			this.m_searchTypeNodes = new Transform[7];
			this.m_displyTypeNodes = new Transform[7];
			this.m_searchStrings = new string[7][];
			this.m_searchVals = new int[7][];
			for (int i = 0; i < 4; i++)
			{
				ResSocialEnum dataByKey = GameDataMgr.socialEnumDatabin.GetDataByKey((long)i);
				string[] array = dataByKey.szEnum.Split(new char[]
				{
					'~'
				});
				if (i != 0)
				{
					this.m_configsStr += '`';
				}
				this.m_configsStr += dataByKey.szEnum;
				if (array != null)
				{
					this.m_searchStrings[i] = new string[array.Length];
					this.m_searchVals[i] = new int[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						string[] array2 = array[j].Split(new char[]
						{
							'#'
						});
						if (array2.Length == 2)
						{
							this.m_searchStrings[i][j] = array2[0];
							try
							{
								this.m_searchVals[i][j] = Convert.ToInt32(array2[1]);
							}
							catch (Exception var_5_273)
							{
								Debug.LogError("Res Social Enum databin is filled with the wrong format due to the value after # is not a number!");
							}
						}
					}
				}
			}
			this.m_searchStrings[4] = this.m_searchStrings[3];
			this.m_searchVals[4] = this.m_searchVals[3];
			this.CheckLoadAddressBook();
		}

		public void ClearAddressBook()
		{
			if (this.m_addressBook != null)
			{
				this.m_addressBook.Clear();
				this.m_addressBook = null;
			}
		}

		public void CheckLoadAddressBook()
		{
			if (this.m_addressBook == null)
			{
				string text = "Config/Address.json";
				CBinaryObject cBinaryObject = Singleton<CResourceManager>.GetInstance().GetResource(text, typeof(TextAsset), enResourceType.Numeric, false, false).m_content as CBinaryObject;
				if (cBinaryObject == null)
				{
					Debug.LogError(string.Format("Can't find file: {0}", text));
					Singleton<CResourceManager>.GetInstance().RemoveCachedResource(text);
				}
				else
				{
					string @string = Encoding.get_UTF8().GetString(cBinaryObject.m_data);
					this.m_addressBook = (Json.Deserialize(@string) as List<object>);
					if (this.m_addressBook != null && this.m_searchStrings[5] == null)
					{
						int num = 0;
						this.m_searchStrings[5] = new string[this.m_addressBook.get_Count()];
						using (List<object>.Enumerator enumerator = this.m_addressBook.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Dictionary<string, object> dictionary = (Dictionary<string, object>)enumerator.get_Current();
								this.m_searchStrings[5][num] = (dictionary.get_Item("name") as string);
								num++;
							}
						}
					}
					Singleton<CResourceManager>.GetInstance().RemoveCachedResource(text);
				}
			}
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_EditCard, new CUIEventManager.OnUIEventHandler(this.OnBeginEditCard));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_CancelEdit, new CUIEventManager.OnUIEventHandler(this.OnCancelEdit));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_SaveEdit, new CUIEventManager.OnUIEventHandler(this.OnSaveEdit));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_EditTag, new CUIEventManager.OnUIEventHandler(this.OnEditTag));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_TagSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnTagSelectChanged));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_TagEditEnd, new CUIEventManager.OnUIEventHandler(this.OnTagEditEnd));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_HideCard, new CUIEventManager.OnUIEventHandler(this.OnHideShowCard));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_AddFriend, new CUIEventManager.OnUIEventHandler(this.On_AddFriend));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_InviteGuild, new CUIEventManager.OnUIEventHandler(this.On_InviteGuild));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_HideCardConfirm, new CUIEventManager.OnUIEventHandler(this.OnHideShowCardConfirm));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_HideCardCancel, new CUIEventManager.OnUIEventHandler(this.OnHideShowCardCancel));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_CancelEditNotsave, new CUIEventManager.OnUIEventHandler(this.OnCancelEditNotSave));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Social_CancelEditSave, new CUIEventManager.OnUIEventHandler(this.OnCancelEditSave));
		}

		public string[] GetLocations(string srvAddr)
		{
			string[] array = new string[]
			{
				"-",
				"-"
			};
			string[] array2 = srvAddr.Split(new char[]
			{
				'~'
			});
			if (this.m_searchStrings == null || this.m_searchStrings.Length < 5 || this.m_searchStrings[5] == null)
			{
				return array;
			}
			for (int i = 0; i < this.m_searchStrings[5].Length; i++)
			{
				if (this.m_searchStrings[5][i].Equals(array2[0]))
				{
					array[0] = array2[0];
					Dictionary<string, object> dictionary = this.m_addressBook.get_Item(i) as Dictionary<string, object>;
					object obj = null;
					if (dictionary.TryGetValue("sub", ref obj))
					{
						List<object> list = obj as List<object>;
						if (list != null)
						{
							using (List<object>.Enumerator enumerator = list.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									Dictionary<string, object> dictionary2 = (Dictionary<string, object>)enumerator.get_Current();
									object obj2 = null;
									if (dictionary2.TryGetValue("name", ref obj2))
									{
										string text = obj2 as string;
										if (text.Equals(array2[1]))
										{
											array[1] = array2[1];
											break;
										}
									}
								}
							}
						}
					}
					break;
				}
			}
			return array;
		}

		public string GetSocialConfigStr()
		{
			return this.m_configsStr;
		}

		public void UpdateUI()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(32);
			if (widget == null)
			{
				return;
			}
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile == null)
			{
				return;
			}
			this.CheckLoadAddressBook();
			this.SetFriendCardByWidgets(ref profile._socialCardInfo, true);
			bool flag = CPlayerInfoSystem.isSelf(profile.m_uuid);
			Transform transform = widget.transform.Find("pnlContainer/Social_BtnGroupTop");
			Transform transform2 = widget.transform.Find("pnlContainer/Social_BtnGroupTopWatch");
			Transform transform3 = widget.transform.Find("pnlContainer/Social_BtnGroupBottom");
			Transform transform4 = widget.transform.Find("pnlContainer/Social_Sign");
			if (!flag)
			{
				this.m_isInEditmode = false;
				Singleton<CPlayerMentorInfoController>.GetInstance().MentorBtnUIUpdate();
			}
			for (int i = 0; i < this.m_searchTypeNodes.Length; i++)
			{
				if (this.m_searchTypeNodes[i] != null)
				{
					this.m_searchTypeNodes[i].gameObject.CustomSetActive(this.m_isInEditmode);
				}
			}
			string[] array = null;
			GameObject gameObject = this.m_displyTypeNodes[0].gameObject;
			if (!this.m_isInEditmode)
			{
				string srvAddr = StringHelper.UTF8BytesToString(ref profile._socialCardInfo.szPlace);
				array = this.GetLocations(srvAddr);
				gameObject.GetComponent<Text>().set_color(this.m_colorHighlight);
				gameObject.transform.parent.Find("Title").GetComponent<Text>().set_color(this.m_colorHighlight);
			}
			else
			{
				gameObject.GetComponent<Text>().set_color(this.m_colorNormal);
				gameObject.transform.parent.Find("Title").GetComponent<Text>().set_color(this.m_colorNormal);
			}
			try
			{
				GameObject gameObject2 = this.m_displyTypeNodes[3].transform.parent.Find("img1").gameObject;
				GameObject gameObject3 = this.m_displyTypeNodes[3].transform.parent.Find("img2").gameObject;
				gameObject2.CustomSetActive(!this.m_isInEditmode);
				gameObject3.CustomSetActive(!this.m_isInEditmode);
				for (int j = 0; j < this.m_searchTypeNodes.Length; j++)
				{
					if (this.m_displyTypeNodes[j] != null)
					{
						this.m_displyTypeNodes[j].gameObject.CustomSetActive(!this.m_isInEditmode);
						Text component = this.m_displyTypeNodes[j].GetComponent<Text>();
						if (!this.m_isInEditmode)
						{
							switch (j)
							{
							case 0:
								component.set_text(this.GetSearchStr(j, (int)profile._socialCardInfo.dwSearchType));
								break;
							case 1:
								component.set_text(this.GetSearchStr(j, (int)profile._socialCardInfo.dwDay));
								break;
							case 2:
								component.set_text(this.GetSearchStr(j, (int)profile._socialCardInfo.dwHour));
								break;
							case 3:
								try
								{
									string searchStr = this.GetSearchStr(j, (int)profile._socialCardInfo.wHeroTypeOne);
									int num = Convert.ToInt32(searchStr);
									gameObject2.CustomSetActive(num != 0);
									if (num == 0)
									{
										component.set_text("-");
									}
									else
									{
										component.set_text(CHeroInfo.GetHeroJobStr((RES_HERO_JOB)num));
										CUICommonSystem.SetHeroJob(form, gameObject2, (enHeroJobType)num);
									}
									this.m_searchTypeNodes[j].transform.FindChild("List").GetComponent<CUIListScript>().SelectElement(this.GetIndexByString(j, searchStr), true);
								}
								catch (Exception var_18_38C)
								{
									Debug.LogError("social 配置表JOB错误，找jason");
								}
								break;
							case 4:
								try
								{
									string searchStr2 = this.GetSearchStr(j, (int)profile._socialCardInfo.wHeroTypeTwo);
									int num2 = Convert.ToInt32(searchStr2);
									gameObject3.CustomSetActive(num2 != 0);
									if (num2 == 0)
									{
										component.set_text("-");
									}
									else
									{
										component.set_text(CHeroInfo.GetHeroJobStr((RES_HERO_JOB)num2));
										CUICommonSystem.SetHeroJob(form, gameObject3, (enHeroJobType)num2);
									}
									this.m_searchTypeNodes[j].transform.FindChild("List").GetComponent<CUIListScript>().SelectElement(this.GetIndexByString(j, searchStr2), true);
								}
								catch (Exception var_21_431)
								{
									Debug.LogError("social 配置表JOB错误，找jason");
								}
								break;
							case 5:
								if (array != null && array.Length > 0)
								{
									component.set_text(array[0]);
								}
								else
								{
									component.set_text("-");
								}
								break;
							case 6:
								if (array != null && array.Length > 1)
								{
									component.set_text(array[1]);
								}
								else
								{
									component.set_text("-");
								}
								break;
							}
							if (flag && j != 3 && j != 4 && this.m_searchStrings[j] != null)
							{
								this.m_searchTypeNodes[j].transform.FindChild("List").GetComponent<CUIListScript>().SelectElement(this.GetIndexByString(j, component.get_text()), true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("social card setting error:" + ex);
			}
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(flag && !this.m_isInEditmode);
				if (flag)
				{
					this.SetHideCardButton();
				}
			}
			if (transform3 != null)
			{
				transform3.gameObject.CustomSetActive(flag && this.m_isInEditmode);
			}
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(!flag);
				if (!flag)
				{
					GameObject gameObject4 = transform2.transform.FindChild("AddFriendBtn").gameObject;
					bool flag2 = Singleton<CFriendContoller>.GetInstance().model.IsContain(CFriendModel.FriendType.GameFriend, profile.m_uuid, (uint)profile.m_iLogicWorldId);
					gameObject4.CustomSetActive(!flag2);
					GameObject gameObject5 = transform2.transform.FindChild("AddGuildBtn").gameObject;
					bool bActive = Singleton<CGuildSystem>.GetInstance().IsInNormalGuild() && !CGuildHelper.IsInSameGuild((ulong)((long)profile.m_iLogicWorldId));
					gameObject5.CustomSetActive(bActive);
					GameObject gameObject6 = transform2.transform.FindChild("AddMentorBtn").gameObject;
					Singleton<CPlayerMentorInfoController>.GetInstance().MentorBtnUIUpdate();
					gameObject6.CustomSetActive(Singleton<CPlayerMentorInfoController>.GetInstance().mentorBtnStr != null);
					if (Singleton<CPlayerMentorInfoController>.GetInstance().mentorBtnStr != null)
					{
						gameObject6.transform.Find("Text").GetComponent<Text>().set_text(Singleton<CPlayerMentorInfoController>.GetInstance().mentorBtnStr);
					}
				}
			}
			if (transform4 != null)
			{
				GameObject gameObject7 = transform4.FindChild("InputCtrl").gameObject;
				InputField signInputText = this.GetSignInputText();
				gameObject7.CustomSetActive(true);
				gameObject7.GetComponent<InputField>().set_interactable(this.m_isInEditmode);
			}
			if (!this.m_isInEditmode)
			{
				string text = StringHelper.UTF8BytesToString(ref profile._socialCardInfo.szDatingDeclaration);
				InputField signInputText2 = this.GetSignInputText();
				if (signInputText2)
				{
					signInputText2.set_text(text);
				}
			}
			this.UpdateTags();
		}

		private void UpdateTags()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(32);
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("pnlContainer/Social_Tag");
			if (transform != null)
			{
				if (!this.m_isInEditmode)
				{
					CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
					this.m_tags.Clear();
					for (int i = 0; i < (int)profile._socialCardInfo.bTagNum; i++)
					{
						this.m_tags.Add(profile._socialCardInfo.TagList[i]);
					}
				}
				transform.FindChild("btnEdit").gameObject.CustomSetActive(this.m_isInEditmode);
				for (int j = 0; j < this.m_tags.get_Count(); j++)
				{
					GameObject gameObject = transform.FindChild("TagGroup/Tag" + (j + 1)).gameObject;
					gameObject.CustomSetActive(true);
					Text component = gameObject.transform.Find("Text").GetComponent<Text>();
					int id = Convert.ToInt32(this.m_tags.get_Item(j));
					ResSocialTags dataByIndex = GameDataMgr.socialTagsDataBin.GetDataByIndex(id);
					component.set_text(dataByIndex.szText);
					byte bType = dataByIndex.bType;
					if (bType != 1)
					{
						if (bType == 2)
						{
							component.set_color(this.m_colorTagKawaii);
						}
					}
					else
					{
						component.set_color(this.m_colorTagCool);
					}
				}
				for (int k = this.m_tags.get_Count(); k < 4; k++)
				{
					GameObject gameObject2 = transform.FindChild("TagGroup/Tag" + (k + 1)).gameObject;
					gameObject2.CustomSetActive(false);
				}
			}
		}

		private InputField GetSignInputText()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return null;
			}
			GameObject widget = form.GetWidget(32);
			if (widget == null)
			{
				return null;
			}
			Transform transform = widget.transform.Find("pnlContainer/Social_Sign");
			GameObject gameObject = transform.FindChild("InputCtrl").gameObject;
			return gameObject.GetComponent<InputField>();
		}

		public string GetSearchStr(int type, int val)
		{
			return this.m_searchStrings[type][this.GetIndexByVal(type, val)];
		}

		public int GetSearchVal(int type, string str)
		{
			return this.m_searchVals[type][this.GetIndexByString(type, str)];
		}

		public int GetIndexByVal(int type, int val)
		{
			for (int i = 0; i < this.m_searchStrings[type].Length; i++)
			{
				if (this.m_searchVals[type][i] == val)
				{
					return i;
				}
			}
			return 0;
		}

		public int GetIndexByString(int type, string str)
		{
			for (int i = 0; i < this.m_searchStrings[type].Length; i++)
			{
				if (this.m_searchStrings[type][i].Equals(str))
				{
					return i;
				}
			}
			return 0;
		}

		public void Load(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(32);
			GameObject widget2 = form.GetWidget(33);
			widget2.CustomSetActive(false);
			if (widget == null)
			{
				for (int i = 0; i < this.m_searchTypeNodes.Length; i++)
				{
					this.m_searchTypeNodes[i] = null;
				}
				for (int j = 0; j < this.m_searchTypeNodes.Length; j++)
				{
					this.m_displyTypeNodes[j] = null;
				}
				return;
			}
			this.m_searchTypeNodes[0] = widget.transform.Find("pnlContainer/Social_IWant/DropList");
			this.m_searchTypeNodes[5] = widget.transform.Find("pnlContainer/Social_Position/DropList1");
			this.m_searchTypeNodes[6] = widget.transform.Find("pnlContainer/Social_Position/DropList2");
			this.m_searchTypeNodes[1] = widget.transform.Find("pnlContainer/Social_Time/WeekDropList");
			this.m_searchTypeNodes[2] = widget.transform.Find("pnlContainer/Social_Time/DayDropList");
			this.m_searchTypeNodes[3] = widget.transform.Find("pnlContainer/Social_Speciality/DropList1");
			this.m_searchTypeNodes[4] = widget.transform.Find("pnlContainer/Social_Speciality/DropList2");
			this.m_displyTypeNodes[0] = widget.transform.Find("pnlContainer/Social_IWant/Content");
			this.m_displyTypeNodes[5] = widget.transform.Find("pnlContainer/Social_Position/Content1");
			this.m_displyTypeNodes[6] = widget.transform.Find("pnlContainer/Social_Position/Content2");
			this.m_displyTypeNodes[1] = widget.transform.Find("pnlContainer/Social_Time/WeekContent");
			this.m_displyTypeNodes[2] = widget.transform.Find("pnlContainer/Social_Time/DayContent");
			this.m_displyTypeNodes[3] = widget.transform.Find("pnlContainer/Social_Speciality/Content1");
			this.m_displyTypeNodes[4] = widget.transform.Find("pnlContainer/Social_Speciality/Content2");
			string[] array = this.m_searchStrings[3];
			string[] array2 = new string[array.Length];
			try
			{
				for (int k = 0; k < array.Length; k++)
				{
					array2[k] = CHeroInfo.GetHeroJobStr((RES_HERO_JOB)Convert.ToInt32(array[k]));
				}
			}
			catch (Exception var_7_20E)
			{
				Debug.LogError("social 配置表JOB错误，找jason");
			}
			CUIUtility.ClearTextComboboxContent();
			for (int l = 0; l < 6; l++)
			{
				if (!(this.m_searchTypeNodes[l] == null) && this.m_searchStrings[l] != null)
				{
					CUIListScript component = this.m_searchTypeNodes[l].Find("List").GetComponent<CUIListScript>();
					GameObject gameObject = this.m_searchTypeNodes[l].Find("Button_Down").gameObject;
					switch (l)
					{
					case 3:
					case 4:
						CUIUtility.RegisterTextComboboxContent(gameObject, component, array2, null);
						break;
					case 5:
						CUIUtility.RegisterTextComboboxContent(gameObject, component, this.m_searchStrings[l], new CUIUtility.TextComboClickCallback(this.OnLocation1Choose));
						break;
					default:
						CUIUtility.RegisterTextComboboxContent(gameObject, component, this.m_searchStrings[l], null);
						break;
					}
				}
			}
			this.m_isInEditmode = false;
			this.UpdateUI();
		}

		public void OnLocation1Choose(CUIEvent evt)
		{
			if (evt == null || evt.m_srcWidget == null)
			{
				return;
			}
			CUIListScript component = evt.m_srcWidget.GetComponent<CUIListScript>();
			if (component == null)
			{
				return;
			}
			Transform transform = this.m_searchTypeNodes[6].transform;
			if (transform == null)
			{
				return;
			}
			GameObject gameObject = transform.Find("Button_Down").gameObject;
			CUIListScript component2 = transform.Find("List").GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			if (selectedIndex < this.m_addressBook.get_Count())
			{
				Dictionary<string, object> dictionary = this.m_addressBook.get_Item(selectedIndex) as Dictionary<string, object>;
				object obj = null;
				if (dictionary.TryGetValue("sub", ref obj))
				{
					List<object> list = obj as List<object>;
					if (list != null)
					{
						int num = 0;
						this.m_searchStrings[6] = new string[list.get_Count()];
						using (List<object>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Dictionary<string, object> dictionary2 = (Dictionary<string, object>)enumerator.get_Current();
								object obj2 = null;
								if (dictionary2.TryGetValue("name", ref obj2))
								{
									this.m_searchStrings[6][num] = (obj2 as string);
									num++;
								}
							}
						}
						CUIUtility.RegisterTextComboboxContent(gameObject, component2, this.m_searchStrings[6], null);
					}
				}
			}
		}

		public void OnBeginEditCard(CUIEvent evt)
		{
			this.m_isInEditmode = true;
			this.UpdateUI();
		}

		private void OnSaveEdit(CUIEvent evt)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5609u);
			this.SetFriendCardByWidgets(ref cSPkg.stPkgData.stChgFriendCardReq.stFriendCardInfo, false);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private int GetDropListSelectIdx(int searchType)
		{
			CUIListScript component = this.m_searchTypeNodes[searchType].transform.FindChild("List").GetComponent<CUIListScript>();
			if (component != null)
			{
				int num = component.GetSelectedIndex();
				if (num < 0)
				{
					num = 0;
					component.SelectElement(0, true);
				}
				return num;
			}
			return 0;
		}

		private bool ChcekFriendCardDiffWidgets(COMDT_FRIEND_CARD card)
		{
			int num = 1;
			if (card.dwDay != (uint)this.m_searchVals[num][this.GetDropListSelectIdx(num)])
			{
				return true;
			}
			num = 2;
			if (card.dwHour != (uint)this.m_searchVals[num][this.GetDropListSelectIdx(num)])
			{
				return true;
			}
			num = 0;
			if (card.dwSearchType != (uint)this.m_searchVals[num][this.GetDropListSelectIdx(num)])
			{
				return true;
			}
			num = 3;
			if (card.wHeroTypeOne != (ushort)this.m_searchVals[num][this.GetDropListSelectIdx(num)])
			{
				return true;
			}
			num = 4;
			if (card.wHeroTypeTwo != (ushort)this.m_searchVals[num][this.GetDropListSelectIdx(num)])
			{
				return true;
			}
			num = 5;
			string text = this.m_searchStrings[num][this.GetDropListSelectIdx(num)];
			num = 6;
			string text2 = "-";
			if (this.m_searchStrings[num] != null)
			{
				text2 = this.m_searchStrings[num][this.GetDropListSelectIdx(num)];
			}
			byte[] array = new byte[card.szPlace.Length];
			StringHelper.StringToUTF8Bytes(text + '~' + text2, ref array);
			if (array == null || array.Length != card.szPlace.Length)
			{
				return true;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != card.szPlace[i])
				{
					return true;
				}
			}
			InputField signInputText = this.GetSignInputText();
			if (signInputText)
			{
				array = new byte[card.szDatingDeclaration.Length];
				StringHelper.StringToUTF8Bytes(signInputText.get_text(), ref array);
				if (array == null || array.Length != card.szDatingDeclaration.Length)
				{
					return true;
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] != card.szDatingDeclaration[j])
					{
						return true;
					}
				}
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return false;
			}
			GameObject widget = form.GetWidget(32);
			if (widget == null)
			{
				return false;
			}
			Transform x = widget.transform.Find("pnlContainer/Social_Tag");
			if (x != null)
			{
				for (int k = 0; k < this.m_tags.get_Count(); k++)
				{
					if (card.TagList[k] != (ushort)Convert.ToByte(this.m_tags.get_Item(k)))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void SetFriendCardByWidgets(ref COMDT_FRIEND_CARD card, bool checkNeed = false)
		{
			int num = 1;
			if (card.dwDay == 0u || !checkNeed)
			{
				card.dwDay = (uint)this.m_searchVals[num][this.GetDropListSelectIdx(num)];
			}
			num = 2;
			if (card.dwHour == 0u || !checkNeed)
			{
				card.dwHour = (uint)this.m_searchVals[num][this.GetDropListSelectIdx(num)];
			}
			num = 0;
			if (card.dwSearchType == 0u || !checkNeed)
			{
				card.dwSearchType = (uint)this.m_searchVals[num][this.GetDropListSelectIdx(num)];
			}
			num = 3;
			if (card.wHeroTypeOne == 0 || !checkNeed)
			{
				card.wHeroTypeOne = (ushort)this.m_searchVals[num][this.GetDropListSelectIdx(num)];
			}
			num = 4;
			if (card.wHeroTypeTwo == 0 || !checkNeed)
			{
				card.wHeroTypeTwo = (ushort)this.m_searchVals[num][this.GetDropListSelectIdx(num)];
			}
			if (!checkNeed)
			{
				num = 5;
				string text = this.m_searchStrings[num][this.GetDropListSelectIdx(num)];
				num = 6;
				string text2 = "-";
				if (this.m_searchStrings[num] != null)
				{
					text2 = this.m_searchStrings[num][this.GetDropListSelectIdx(num)];
				}
				StringHelper.StringToUTF8Bytes(text + '~' + text2, ref card.szPlace);
			}
			InputField signInputText = this.GetSignInputText();
			if (signInputText && (card.szDatingDeclaration.Length == 0 || !checkNeed))
			{
				StringHelper.StringToUTF8Bytes(signInputText.get_text(), ref card.szDatingDeclaration);
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(32);
			if (widget == null)
			{
				return;
			}
			Transform x = widget.transform.Find("pnlContainer/Social_Tag");
			if (x != null && !checkNeed)
			{
				card.bTagNum = (byte)this.m_tags.get_Count();
				card.TagList = new ushort[(int)card.bTagNum];
				for (int i = 0; i < this.m_tags.get_Count(); i++)
				{
					card.TagList[i] = (ushort)Convert.ToByte(this.m_tags.get_Item(i));
				}
			}
		}

		[MessageHandler(5610)]
		public static void OnSaveCardSuc(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			Singleton<CPlayerSocialInfoController>.instance.SetFriendCardByWidgets(ref profile._socialCardInfo, false);
			Singleton<CPlayerSocialInfoController>.instance.SetFriendCardByWidgets(ref masterRoleInfo.m_socialFriendCard, false);
			Singleton<CPlayerSocialInfoController>.instance.m_isInEditmode = false;
			Singleton<CPlayerSocialInfoController>.instance.UpdateUI();
		}

		public void OnCancelEdit(CUIEvent evt)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (this.ChcekFriendCardDiffWidgets(profile._socialCardInfo))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Social_ChangeCardConfirm"), enUIEventID.Social_CancelEditSave, enUIEventID.Social_CancelEditNotsave, Singleton<CTextManager>.GetInstance().GetText("Save"), Singleton<CTextManager>.GetInstance().GetText("DontSave"), false);
			}
			else
			{
				this.OnCancelEditNotSave(evt);
			}
		}

		public void OnCancelEditSave(CUIEvent evt)
		{
			this.OnSaveEdit(evt);
		}

		public void OnCancelEditNotSave(CUIEvent evt)
		{
			this.m_isInEditmode = false;
			this.UpdateUI();
		}

		public void OnEditTag(CUIEvent evt)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CPlayerSocialInfoController.s_socialTagEditFormPath, false, true);
			if (!cUIFormScript)
			{
				return;
			}
			GameObject widget = cUIFormScript.GetWidget(0);
			if (widget != null)
			{
				CUIToggleListScript component = widget.transform.GetComponent<CUIToggleListScript>();
				int num = GameDataMgr.socialTagsDataBin.Count();
				component.SetElementAmount(num);
				for (int i = 0; i < num; i++)
				{
					CUIToggleListElementScript cUIToggleListElementScript = (CUIToggleListElementScript)component.GetElemenet(i);
					ResSocialTags dataByIndex = GameDataMgr.socialTagsDataBin.GetDataByIndex(i);
					if (cUIToggleListElementScript != null)
					{
						Transform transform = cUIToggleListElementScript.gameObject.transform;
						Transform transform2 = transform.Find("Text");
						CUICommonSystem.SetTextContent(transform2.gameObject, dataByIndex.szText);
						Text component2 = transform2.transform.GetComponent<Text>();
						byte bType = dataByIndex.bType;
						if (bType != 1)
						{
							if (bType == 2)
							{
								component2.set_color(this.m_colorTagKawaii);
							}
						}
						else
						{
							component2.set_color(this.m_colorTagCool);
						}
					}
				}
				component.SetSelectLimitCount(4);
				for (int j = 0; j < this.m_tags.get_Count(); j++)
				{
					component.SetMultiSelected(Convert.ToInt32(this.m_tags.get_Item(j)), true);
				}
			}
			this.OnTagSelectChanged(null);
		}

		private void OnTagSelectChanged(CUIEvent evt)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPlayerSocialInfoController.s_socialTagEditFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			if (widget == null)
			{
				return;
			}
			CUIToggleListScript component = widget.GetComponent<CUIToggleListScript>();
			string text = Singleton<CTextManager>.GetInstance().GetText("Social_TagTips", new string[]
			{
				component.GetMultiSelectCount().ToString(),
				4.ToString()
			});
			GameObject widget2 = form.GetWidget(1);
			widget2.GetComponent<Text>().set_text(text);
		}

		private void OnTagEditEnd(CUIEvent evt)
		{
			CUIFormScript srcFormScript = evt.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			this.m_tags.Clear();
			GameObject widget = srcFormScript.GetWidget(0);
			CUIToggleListScript component = widget.GetComponent<CUIToggleListScript>();
			bool[] multiSelected = component.GetMultiSelected();
			for (int i = 0; i < component.GetElementAmount(); i++)
			{
				if (multiSelected[i])
				{
					this.m_tags.Add(i);
				}
			}
			this.UpdateTags();
			srcFormScript.Close();
		}

		private void OnHideShowCard(CUIEvent evt)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile.IsPrivacyOpen(COM_USER_PRIVACY_MASK.COM_USER_PRIVACY_MASK_FRIEND_CARD))
			{
				this.OnHideShowCardConfirm(null);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Social_HideCardConfirm"), enUIEventID.Social_HideCardConfirm, enUIEventID.Social_HideCardCancel, false);
			}
		}

		private void OnHideShowCardConfirm(CUIEvent evt)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile == null)
			{
				return;
			}
			if (!CPlayerInfoSystem.isSelf(profile.m_uuid))
			{
				return;
			}
			bool bOpen = !profile.IsPrivacyOpen(COM_USER_PRIVACY_MASK.COM_USER_PRIVACY_MASK_FRIEND_CARD);
			profile.SetPrivacyBit(bOpen, COM_USER_PRIVACY_MASK.COM_USER_PRIVACY_MASK_FRIEND_CARD);
			Singleton<CSettingsSys>.GetInstance().reqOperateUserPrivacyBit(bOpen, COM_USER_PRIVACY_MASK.COM_USER_PRIVACY_MASK_FRIEND_CARD);
			this.SetHideCardButton();
		}

		private void OnHideShowCardCancel(CUIEvent evt)
		{
		}

		private void SetHideCardButton()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(32);
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("pnlContainer/Social_BtnGroupTop");
			GameObject gameObject = transform.FindChild("HideBtn").gameObject;
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			Text component = gameObject.transform.Find("Text").GetComponent<Text>();
			if (profile.IsPrivacyOpen(COM_USER_PRIVACY_MASK.COM_USER_PRIVACY_MASK_FRIEND_CARD))
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Social_ShowCard"));
			}
			else
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Social_HideCard"));
			}
		}

		private void On_InviteGuild(CUIEvent evt)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile == null)
			{
				return;
			}
			bool flag = CPlayerInfoSystem.isSelf(profile.m_uuid);
			if (flag)
			{
				return;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, int>("Guild_Invite", profile.m_uuid, profile.m_iLogicWorldId);
		}

		private void On_AddFriend(CUIEvent evt)
		{
			if (Singleton<CPlayerInfoSystem>.instance.GetProfile() == null)
			{
				return;
			}
			string randomReuqestStr = CFriendView.Verfication.GetRandomReuqestStr("FriendVerify_Text_", 1, 4);
			Singleton<CUIManager>.GetInstance().OpenStringSenderBox(Singleton<CTextManager>.GetInstance().GetText("Friend_AddTitle"), Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqDesc"), randomReuqestStr, new CUIManager.StringSendboxOnSend(this.OnFriendApplyVerifyBoxRetrun), randomReuqestStr);
		}

		public void OnFriendApplyVerifyBoxRetrun(string str)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile == null)
			{
				return;
			}
			FriendSysNetCore.Send_Request_BeFriend(profile.m_uuid, (uint)profile.m_iLogicWorldId, str, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1);
		}
	}
}
