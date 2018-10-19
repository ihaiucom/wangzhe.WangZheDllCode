using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class IDIPSys : MonoSingleton<IDIPSys>
	{
		public struct IDIPItem
		{
			public GameObject root;

			public Text name;

			public Image icon;

			public Text TypeText;

			public Image flag;

			public Image glow;

			public GameObject RedSpot;

			public IDIPItem(GameObject node)
			{
				this.root = node;
				this.name = Utility.GetComponetInChild<Text>(node, "Name");
				this.icon = Utility.GetComponetInChild<Image>(node, "Icon");
				this.TypeText = Utility.GetComponetInChild<Text>(node, "Flag/Text");
				this.glow = Utility.GetComponetInChild<Image>(node, "Glow");
				this.flag = Utility.GetComponetInChild<Image>(node, "Flag");
				this.RedSpot = node.transform.Find("Hotspot").gameObject;
			}
		}

		public enum BTN_DOSOMTHING
		{
			BTN_DOSOMTHING_NONE,
			BTN_DOSOMTHING_URL,
			BTN_DOSOMTHING_GAME
		}

		public class IDIPData
		{
			public uint dwNoticeID;

			public ulong startTime;

			public ulong endTime;

			public string title;

			public string content;

			public byte bNoticeLabelType;

			public byte bNoticeType;

			public byte bPriority;

			public ulong ullNoticeTime;

			public uint dwLogicWorldID;

			public bool bLoad;

			public IDIPSys.BTN_DOSOMTHING btnDoSth;

			public string btnUrl;

			public bool bVisited;

			public string btnTitle;
		}

		public delegate void LoadRCallBack(Texture2D image);

		public delegate void LoadRCallBack2(Texture2D image, int imageIdx);

		public static string s_formPath = CUIUtility.s_Form_Activity_Dir + "Form_Activity.prefab";

		private CUIFormScript m_IDIPForm;

		private CUIListScript m_uiListMenu;

		private Text m_TextContent;

		private Image m_ImageContent;

		private Text m_Title;

		private GameObject m_BtnDoSth;

		private IDIPSys.IDIPData m_CurSelectData;

		private RectTransform m_ScrollRect;

		private bool m_bShow;

		private Sprite m_BackImageSprite;

		private List<IDIPSys.IDIPItem> m_IDIPItemList;

		public ListView<IDIPSys.IDIPData> m_IDIPDataList = new ListView<IDIPSys.IDIPData>();

		private uint m_DataVersion;

		private int m_nRedPoint;

		private bool m_bHaveUpdateList;

		private string m_MatchBegin = "<button=";

		private string m_MatchEnd = "$>";

		private string m_MatchChildEnd = "=$";

		private string m_matchTitle = "title=";

		private bool m_bFirst = true;

		public int m_ChannelID;

		private string m_MyOpenID = string.Empty;

		private uint[] m_BanTimeInfo = new uint[100];

		public bool HaveUpdateList
		{
			get
			{
				if (!this.m_bHaveUpdateList)
				{
					return this.m_nRedPoint > 0;
				}
				return this.m_bHaveUpdateList;
			}
			set
			{
				this.m_bHaveUpdateList = value;
			}
		}

		public bool RedPotState
		{
			get
			{
				return this.m_IDIPDataList.Count > 0;
			}
		}

		private void CheckIsBtnUrl(IDIPSys.IDIPData data)
		{
			string content = data.content;
			int num = content.IndexOf(this.m_MatchBegin);
			if (num > 0)
			{
				string content2 = content.Substring(0, num);
				int num2 = content.IndexOf(this.m_MatchEnd);
				if (num2 < 0)
				{
					data.content = content2;
					return;
				}
				int num3 = content.IndexOf(this.m_MatchChildEnd);
				if (num3 < 0)
				{
					data.content = content2;
					return;
				}
				string a = content.Substring(num + this.m_MatchBegin.Length, num3 - num - this.m_MatchBegin.Length);
				if (a == "0")
				{
					data.btnDoSth = IDIPSys.BTN_DOSOMTHING.BTN_DOSOMTHING_URL;
				}
				else
				{
					data.btnDoSth = IDIPSys.BTN_DOSOMTHING.BTN_DOSOMTHING_GAME;
				}
				data.btnTitle = "去完成";
				string text = content.Substring(num3 + this.m_MatchChildEnd.Length, num2 - num3 - this.m_MatchChildEnd.Length);
				if (text.Contains(this.m_MatchChildEnd))
				{
					string text2 = text;
					int num4 = text2.IndexOf(this.m_MatchChildEnd);
					string btnUrl = text2.Substring(0, num4);
					data.btnUrl = btnUrl;
					string text3 = text2.Substring(num4 + this.m_MatchChildEnd.Length, text2.Length - num4 - this.m_MatchChildEnd.Length);
					int num5 = text3.IndexOf(this.m_matchTitle);
					if (num5 >= 0 && text3.Length > 0)
					{
						string btnTitle = text3.Substring(num5 + this.m_matchTitle.Length, text3.Length - num5 - this.m_matchTitle.Length);
						data.btnTitle = btnTitle;
					}
				}
				else
				{
					data.btnUrl = text;
				}
				data.content = content2;
			}
			else
			{
				data.btnUrl = string.Empty;
				data.btnDoSth = IDIPSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
			}
		}

		protected override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IDIP_GOTO_COMPLETE, new CUIEventManager.OnUIEventHandler(this.OnBtnComplete));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.XinYue_Open, new CUIEventManager.OnUIEventHandler(this.OnClickOpenXinYue));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OPEN_HELPME, new CUIEventManager.OnUIEventHandler(this.OnClickOpenHelpMe));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OPEN_HELPMEMONEY, new CUIEventManager.OnUIEventHandler(this.OnClickOpenHelpMeMoney));
			Singleton<CTimerManager>.GetInstance().AddTimer(300000, -1, new CTimer.OnTimeUpHandler(this.OnRequestNoticeNum));
			Singleton<PopupMenuListSys>.CreateInstance();
		}

		public void OnOpenIDIPForm(CUIFormScript IDIPForm)
		{
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				return;
			}
			this.m_IDIPForm = IDIPForm;
			this.m_bShow = true;
			this.m_bFirst = true;
			this.HaveUpdateList = false;
			this.RequestNoticeList();
			this.ShowForm();
		}

		private void OnRequestNoticeNum(int timer)
		{
			if (Singleton<CBattleSystem>.instance.FormScript == null)
			{
				this.RequestNoticeNum();
			}
		}

		private void ShowForm()
		{
			if (!this.m_bShow)
			{
				return;
			}
			Transform transform = this.m_IDIPForm.transform.Find("Panel/Panle_Activity");
			if (transform)
			{
				transform.gameObject.CustomSetActive(false);
			}
			Transform transform2 = this.m_IDIPForm.transform.Find("Panel/Panle_IDIP");
			if (transform2)
			{
				transform2.gameObject.CustomSetActive(true);
			}
			this.m_uiListMenu = Utility.GetComponetInChild<CUIListScript>(this.m_IDIPForm.gameObject, "Panel/Panle_IDIP/Menu/List");
			this.m_TextContent = Utility.GetComponetInChild<Text>(this.m_IDIPForm.gameObject, "Panel/Panle_IDIP/ScrollRect/Content/Text");
			this.m_ImageContent = Utility.GetComponetInChild<Image>(this.m_IDIPForm.gameObject, "Panel/Panle_IDIP/Image");
			this.m_Title = Utility.GetComponetInChild<Text>(this.m_IDIPForm.gameObject, "Panel/Panle_IDIP/GameObject/ContentTitle");
			if (this.m_ImageContent != null && this.m_bFirst)
			{
				this.m_bFirst = false;
				this.m_BackImageSprite = this.m_ImageContent.sprite;
			}
			this.m_ScrollRect = this.m_IDIPForm.gameObject.transform.FindChild("Panel/Panle_IDIP/ScrollRect/Content").GetComponent<RectTransform>();
			this.m_BtnDoSth = this.m_IDIPForm.gameObject.transform.FindChild("Panel/Panle_IDIP/Button_DoComplete").gameObject;
			this.m_BtnDoSth.CustomSetActive(false);
			this.CheckValidNotice();
			this.SortbyPriority();
			this.BuildMenuList();
			this.SelectMenuItem(0);
			this.m_uiListMenu.SelectElement(0, true);
			this.UpdateRedPoint();
			Singleton<EventRouter>.instance.BroadCastEvent("IDIPNOTICE_UNREAD_NUM_UPDATE");
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IDIP_SelectItem, new CUIEventManager.OnUIEventHandler(this.OnSelectItem));
		}

		public void ShareActivityTask(string cdnUrl)
		{
			base.StartCoroutine(this.DownloadImage(cdnUrl, delegate(Texture2D text2)
			{
				string fileName = this.ToMD5(cdnUrl);
				string cachePath = CFileManager.GetCachePath(fileName);
				MonoSingleton<ShareSys>.GetInstance().GShare("TimeLine/Qzone", cachePath);
			}, 0));
		}

		public void UpdateGlobalPoint()
		{
			int num = 0;
			int count = this.m_IDIPDataList.Count;
			for (int i = 0; i < count; i++)
			{
				IDIPSys.IDIPData iDIPData = this.m_IDIPDataList[i];
				if (iDIPData != null && !iDIPData.bVisited)
				{
					num++;
				}
			}
			this.m_nRedPoint = num;
			Singleton<EventRouter>.instance.BroadCastEvent("IDIPNOTICE_UNREAD_NUM_UPDATE");
		}

		private void UpdateRedPoint()
		{
			int num = 0;
			int count = this.m_IDIPDataList.Count;
			int count2 = this.m_IDIPItemList.Count;
			for (int i = 0; i < count; i++)
			{
				IDIPSys.IDIPData iDIPData = this.m_IDIPDataList[i];
				if (iDIPData != null && i < count2)
				{
					if (!iDIPData.bVisited)
					{
						num++;
						if (this.m_IDIPItemList[i].RedSpot)
						{
							this.m_IDIPItemList[i].RedSpot.CustomSetActive(true);
						}
					}
					else if (this.m_IDIPItemList[i].RedSpot)
					{
						this.m_IDIPItemList[i].RedSpot.CustomSetActive(false);
					}
				}
			}
			this.m_nRedPoint = num;
			Singleton<EventRouter>.instance.BroadCastEvent("IDIPNOTICE_UNREAD_NUM_UPDATE");
		}

		public void OnCloseIDIPForm(CUIEvent uiEvent)
		{
			this.m_IDIPForm = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IDIP_SelectItem, new CUIEventManager.OnUIEventHandler(this.OnSelectItem));
			this.m_bShow = false;
			this.m_bFirst = false;
			if (this.m_ImageContent != null && this.m_ImageContent.sprite != null && this.m_BackImageSprite != this.m_ImageContent.sprite)
			{
				UnityEngine.Object.Destroy(this.m_ImageContent.sprite);
			}
			this.m_CurSelectData = null;
		}

		public void ClearIDIPData()
		{
			this.m_IDIPDataList.Clear();
			this.m_DataVersion = 0u;
		}

		private void SortbyPriority()
		{
			int count = this.m_IDIPDataList.Count;
			if (count > 0)
			{
				this.m_IDIPDataList.Sort(delegate(IDIPSys.IDIPData a, IDIPSys.IDIPData b)
				{
					if (a.bPriority < b.bPriority)
					{
						return -1;
					}
					if (a.bPriority > b.bPriority)
					{
						return 1;
					}
					if (a.ullNoticeTime > b.ullNoticeTime)
					{
						return -1;
					}
					if (a.ullNoticeTime < b.ullNoticeTime)
					{
						return 1;
					}
					return 0;
				});
			}
		}

		private void CheckValidNotice()
		{
			for (int i = this.m_IDIPDataList.Count - 1; i >= 0; i--)
			{
				IDIPSys.IDIPData iDIPData = this.m_IDIPDataList[i];
				if (iDIPData != null && ((long)CRoleInfo.GetCurrentUTCTime() < (long)iDIPData.startTime || (long)CRoleInfo.GetCurrentUTCTime() >= (long)iDIPData.endTime))
				{
					this.m_IDIPDataList.Remove(iDIPData);
				}
			}
		}

		private void BuildMenuList()
		{
			this.m_IDIPItemList = new List<IDIPSys.IDIPItem>();
			int count = this.m_IDIPDataList.Count;
			this.m_uiListMenu.SetElementAmount(count);
			for (int i = 0; i < count; i++)
			{
				IDIPSys.IDIPData iDIPData = this.m_IDIPDataList[i];
				CUIListElementScript elemenet = this.m_uiListMenu.GetElemenet(i);
				IDIPSys.IDIPItem item = new IDIPSys.IDIPItem(elemenet.gameObject);
				item.name.text = iDIPData.title;
				if (item.glow)
				{
					item.glow.gameObject.CustomSetActive(false);
				}
				if (iDIPData.bNoticeLabelType == 1)
				{
					item.flag.SetSprite(CUIUtility.GetSpritePrefeb("UGUI/Sprite/Dynamic/Activity/RES_WEAL_COLORBAR_TYPE_NOTICE", false, false), false);
					item.TypeText.text = "公告";
				}
				else if (iDIPData.bNoticeLabelType == 2)
				{
					item.flag.SetSprite(CUIUtility.GetSpritePrefeb("UGUI/Sprite/Dynamic/Activity/RES_WEAL_COLORBAR_TYPE_LIMIT", false, false), false);
					item.TypeText.text = "活动";
				}
				else if (iDIPData.bNoticeLabelType == 3)
				{
					item.flag.SetSprite(CUIUtility.GetSpritePrefeb("UGUI/Sprite/Dynamic/Activity/RES_WEAL_COLORBAR_TYPE_HOT", false, false), false);
					item.TypeText.text = "赛事";
				}
				this.m_IDIPItemList.Add(item);
			}
		}

		private void SelectMenuItem(int index)
		{
			this.ClearContent();
			for (int i = 0; i < this.m_IDIPItemList.Count; i++)
			{
				if (i == index)
				{
					if (this.m_IDIPItemList[i].glow != null)
					{
						this.m_IDIPItemList[i].glow.gameObject.CustomSetActive(true);
					}
				}
				else if (this.m_IDIPItemList[i].glow != null)
				{
					this.m_IDIPItemList[i].glow.gameObject.CustomSetActive(false);
				}
			}
			if (index >= 0 && index < this.m_IDIPDataList.Count)
			{
				IDIPSys.IDIPData iDIPData = this.m_IDIPDataList[index];
				if (iDIPData != null)
				{
					iDIPData.bVisited = true;
					this.SetVisited(iDIPData.ullNoticeTime);
					if (this.m_IDIPItemList[index].RedSpot)
					{
						this.m_IDIPItemList[index].RedSpot.SetActive(false);
					}
					if (iDIPData != null)
					{
						if (iDIPData.bLoad)
						{
							this.UpdateContent(iDIPData);
						}
						else
						{
							this.UpdateContent(iDIPData);
							this.RequestNoticeContentInfo(index);
						}
					}
				}
			}
			this.UpdateRedPoint();
			if (this.m_ScrollRect != null)
			{
				this.m_ScrollRect.anchoredPosition = new Vector2(this.m_ScrollRect.anchoredPosition.x, 0f);
			}
		}

		private void OnSelectItem(CUIEvent uiEvent)
		{
			this.SelectMenuItem(uiEvent.m_srcWidgetIndexInBelongedList);
		}

        public IEnumerator GetChannelID()
        {
            var localCachePath = CFileManager.GetStreamingAssetsPathWithHeader("channel.ini");
            var www = new WWW(localCachePath);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log("GetChannelID3 = " + www.error);
                yield break;
            }
            var text = www.text;
            text = text.Replace("CHANNEL=", string.Empty);
            try
            {
                m_ChannelID = Convert.ToInt32(text);
            }
            catch (FormatException exception)
            {
                m_ChannelID = 0;
                Debug.Log("getchanelid " + exception.ToString());
            }
            www.Dispose();
        }
			

		public string ToMD5(string str)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(str));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}

        public IEnumerator DownloadImage(string preUrl, IDIPSys.LoadRCallBack callBack, int checkDays = 0)
        {
            var localCachePath = CFileManager.GetCachePath(ToMD5(preUrl));
            var localCacheExpires = false;
            var bExist = File.Exists(localCachePath);
            if (bExist && (checkDays > 0))
            {
                try
                {
                    var _deltaTime___4 = (TimeSpan)(DateTime.Now - File.GetLastWriteTime(localCachePath));
                    if ((Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) && (_deltaTime___4.TotalDays >= checkDays))
                    {
                        localCacheExpires = true;
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log("downloadImage error " + exception);
                    localCacheExpires = true;
                }
            }
            if (bExist && !localCacheExpires)
            {
                try
                {
                    var _data___6 = File.ReadAllBytes(localCachePath);
                    var _tex___7 = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                    if (_tex___7.LoadImage(_data___6) && (callBack != null))
                    {
                        callBack(_tex___7);
                    }
                }
                catch (Exception exception2)
                {
                    object[] inParameters = new object[] { exception2.Message, exception2.StackTrace };
                    DebugHelper.Assert(false, "Exception in IDIPSys.DownloadImage, {0}, {1}", inParameters);
                }
                yield break;
            }
            
            var www = new WWW(preUrl);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log("loadimageerror " + www.error);
                www.Dispose();
                yield break;
            }
            string _imageType___10 = null;
            www.responseHeaders.TryGetValue("CONTENT-TYPE", out _imageType___10);
            if (_imageType___10 != null)
            {
                _imageType___10 = _imageType___10.ToLower();
            }
            if (string.IsNullOrEmpty(_imageType___10) || !_imageType___10.Contains("image/"))
            {
                yield break;
            }
            var _tex___11 = www.texture;
            if ((_tex___11 != null) && (localCachePath != null))
            {
                CFileManager.WriteFile(localCachePath, www.bytes);
            }
            if (callBack != null)
            {
                callBack(_tex___11);
            }
            www.Dispose();
        }

        public IEnumerator DownloadImageByTag(string preUrl, int ImageIDx, IDIPSys.LoadRCallBack2 callBack, string tagPath, int checkDays = 0)
        {
            var _key___0 = ToMD5(preUrl);
            var _localCachePath___1 = CFileManager.CombinePath(tagPath, _key___0);
            var _bExist___2 = false;
            var _localCacheExpires___3 = false;
            _bExist___2 = File.Exists(_localCachePath___1);
            if (_bExist___2 && (checkDays > 0))
            {
                try
                {
                    var _deltaTime___4 = (TimeSpan)(DateTime.Now - File.GetLastWriteTime(_localCachePath___1));
                    if ((Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) && (_deltaTime___4.TotalDays >= checkDays))
                    {
                        _localCacheExpires___3 = true;
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log("downloadImage error " + exception);
                    _localCacheExpires___3 = true;
                }
            }
            if (_bExist___2 && !_localCacheExpires___3)
            {
                var _data___6 = File.ReadAllBytes(_localCachePath___1);
                var _tex___7 = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                if (_tex___7.LoadImage(_data___6) && (callBack != null))
                {
                    callBack(_tex___7, ImageIDx);
                }
                yield break;
            }
            
            var _www___8 = new WWW(preUrl);
            yield return _www___8;

            if (!string.IsNullOrEmpty(_www___8.error))
            {
                Debug.Log("DownloadImageByTag " + _www___8.error + " " + preUrl);
                _www___8.Dispose();
                yield break;
            }
            string _imageType___9 = null;
            _www___8.responseHeaders.TryGetValue("CONTENT-TYPE", out _imageType___9);
            if (_imageType___9 != null)
            {
                _imageType___9 = _imageType___9.ToLower();
            }
            if (string.IsNullOrEmpty(_imageType___9) || !_imageType___9.Contains("image/"))
            {
                yield break;
            }
            var _tex___10 = _www___8.texture;
            if ((_tex___10 != null) && (_localCachePath___1 != null))
            {
                CFileManager.WriteFile(_localCachePath___1, _www___8.bytes);
            }
            if (callBack != null)
            {
                callBack(_tex___10, ImageIDx);
            }
            _www___8.Dispose();
        }

		private void ProcessGetNoticeList(CSPkg msg)
		{
			this.m_MyOpenID = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false).OpenId;
			this.m_DataVersion = msg.stPkgData.stNoticeListRsp.dwDataVersion;
			ListView<IDIPSys.IDIPData> listView = new ListView<IDIPSys.IDIPData>();
			for (int i = 0; i < (int)msg.stPkgData.stNoticeListRsp.wNoticeCnt; i++)
			{
				CSDT_NOTICE_INFO cSDT_NOTICE_INFO = msg.stPkgData.stNoticeListRsp.astNoticeList[i];
				IDIPSys.IDIPData iDIPData = null;
				for (int j = 0; j < this.m_IDIPDataList.Count; j++)
				{
					IDIPSys.IDIPData iDIPData2 = this.m_IDIPDataList[j];
					if (iDIPData2.dwLogicWorldID == cSDT_NOTICE_INFO.dwLogicWorldID && iDIPData2.ullNoticeTime == cSDT_NOTICE_INFO.ullNoticeTime)
					{
						iDIPData = iDIPData2;
						break;
					}
				}
				if (iDIPData == null)
				{
					iDIPData = new IDIPSys.IDIPData();
					iDIPData.dwNoticeID = cSDT_NOTICE_INFO.dwNoticeID;
					iDIPData.startTime = (ulong)cSDT_NOTICE_INFO.dwStartTime;
					iDIPData.endTime = (ulong)cSDT_NOTICE_INFO.dwEndTime;
					iDIPData.bNoticeType = cSDT_NOTICE_INFO.bNoticeType;
					iDIPData.bNoticeLabelType = cSDT_NOTICE_INFO.bNoticeLabelType;
					iDIPData.bPriority = cSDT_NOTICE_INFO.bPriority;
					iDIPData.dwLogicWorldID = cSDT_NOTICE_INFO.dwLogicWorldID;
					iDIPData.ullNoticeTime = cSDT_NOTICE_INFO.ullNoticeTime;
					iDIPData.bLoad = false;
					if (!this.isVisited(iDIPData.ullNoticeTime))
					{
						iDIPData.bVisited = false;
					}
					else
					{
						iDIPData.bVisited = true;
					}
					iDIPData.title = Utility.UTF8Convert(cSDT_NOTICE_INFO.szSubject, (int)cSDT_NOTICE_INFO.bSubjectLen);
					iDIPData.content = Utility.UTF8Convert(cSDT_NOTICE_INFO.szContent, (int)cSDT_NOTICE_INFO.wContentLen);
					this.CheckIsBtnUrl(iDIPData);
					listView.Add(iDIPData);
				}
				else
				{
					listView.Add(iDIPData);
				}
			}
			this.m_IDIPDataList.Clear();
			this.m_IDIPDataList = listView;
			if (this.m_IDIPDataList.Count > 0 && Singleton<CLobbySystem>.GetInstance().IsInLobbyForm())
			{
				this.ShowForm();
			}
		}

		private bool isVisited(ulong noticeTime)
		{
			string key = string.Format("Notice|{0}|{1}", this.m_MyOpenID, noticeTime.ToString());
			return PlayerPrefs.GetInt(key) > 0;
		}

		private void SetVisited(ulong noticeTime)
		{
			string key = string.Format("Notice|{0}|{1}", this.m_MyOpenID, noticeTime);
			PlayerPrefs.SetInt(key, 1);
		}

		private void PrcessOnNoticeInfo(CSPkg msg)
		{
			for (int i = 0; i < this.m_IDIPDataList.Count; i++)
			{
				IDIPSys.IDIPData iDIPData = this.m_IDIPDataList[i];
				if (iDIPData != null && iDIPData.dwNoticeID == msg.stPkgData.stNoticeInfoRsp.dwNoticeID)
				{
					iDIPData.content = Utility.UTF8Convert(msg.stPkgData.stNoticeInfoRsp.szContent, (int)msg.stPkgData.stNoticeInfoRsp.wContentLen);
					this.CheckIsBtnUrl(iDIPData);
					iDIPData.ullNoticeTime = msg.stPkgData.stNoticeInfoRsp.ullNoticeTime;
					iDIPData.bLoad = true;
					MonoSingleton<IDIPSys>.GetInstance().UpdateContent(iDIPData);
					break;
				}
			}
		}

		public void RequestNoticeNum()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1420u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(1421)]
		public static void On_GetNiticeNum(CSPkg msg)
		{
			if (msg.stPkgData.stNoticeNewRsp.bHaveNew > 0)
			{
				MonoSingleton<IDIPSys>.GetInstance().HaveUpdateList = true;
			}
			else
			{
				MonoSingleton<IDIPSys>.GetInstance().HaveUpdateList = false;
			}
			Singleton<EventRouter>.instance.BroadCastEvent("IDIPNOTICE_UNREAD_NUM_UPDATE");
		}

		private void RequestNoticeList()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1422u);
			cSPkg.stPkgData.stNoticeListReq.dwDataVersion = this.m_DataVersion;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(1423)]
		public static void On_GetNoticeList(CSPkg msg)
		{
			MonoSingleton<IDIPSys>.GetInstance().ProcessGetNoticeList(msg);
		}

		private void RequestNoticeContentInfo(int idx)
		{
			if (idx < this.m_IDIPDataList.Count)
			{
				IDIPSys.IDIPData iDIPData = this.m_IDIPDataList[idx];
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1424u);
				cSPkg.stPkgData.stNoticeInfoReq.dwLogicWorldID = iDIPData.dwLogicWorldID;
				cSPkg.stPkgData.stNoticeInfoReq.dwNoticeID = iDIPData.dwNoticeID;
				cSPkg.stPkgData.stNoticeInfoReq.ullNoticeTime = iDIPData.ullNoticeTime;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}

		[MessageHandler(1425)]
		public static void On_GetNoticeContentInfo(CSPkg msg)
		{
			MonoSingleton<IDIPSys>.GetInstance().PrcessOnNoticeInfo(msg);
		}

		[MessageHandler(1405)]
		public static void On_IDIPSelfMsgInfo(CSPkg msg)
		{
			for (int i = 0; i < (int)msg.stPkgData.stAcntSelfMsgInfo.bMsgCnt; i++)
			{
				CSDT_SELFMSG_INFO cSDT_SELFMSG_INFO = msg.stPkgData.stAcntSelfMsgInfo.astMsgList[i];
				string content = Utility.UTF8Convert(cSDT_SELFMSG_INFO.szContent, (int)cSDT_SELFMSG_INFO.wContentLen);
				PopupMenuListSys.PopupMenuListItem item = default(PopupMenuListSys.PopupMenuListItem);
				item.m_show = new PopupMenuListSys.PopupMenuListItem.Show(MonoSingleton<IDIPSys>.GetInstance().ShowSelfMsgInfo);
				item.content = content;
				Singleton<PopupMenuListSys>.GetInstance().AddItem(item);
			}
			Singleton<PopupMenuListSys>.GetInstance().PopupMenuListStart();
		}

		private void ShowSelfMsgInfo(string content)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBox(content, enUIEventID.MENU_PopupMenuFinish, false);
		}

		private void ClearContent()
		{
			if (this.m_bShow)
			{
				if (this.m_TextContent)
				{
					this.m_TextContent.gameObject.CustomSetActive(false);
				}
				if (this.m_Title)
				{
					this.m_Title.gameObject.CustomSetActive(false);
				}
				if (this.m_ImageContent)
				{
					this.m_ImageContent.gameObject.CustomSetActive(false);
				}
			}
		}

		private void OnBtnComplete(CUIEvent ciEvent)
		{
			if (this.m_CurSelectData != null && this.m_bShow)
			{
				IDIPSys.IDIPData curSelectData = this.m_CurSelectData;
				if (curSelectData.btnDoSth == IDIPSys.BTN_DOSOMTHING.BTN_DOSOMTHING_URL)
				{
					CUICommonSystem.OpenUrl(curSelectData.btnUrl, true, 0);
				}
				else if (curSelectData.btnDoSth == IDIPSys.BTN_DOSOMTHING.BTN_DOSOMTHING_GAME)
				{
					int num = 0;
					int.TryParse(curSelectData.btnUrl, out num);
					if (num > 0)
					{
						CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)num, 0, 0, null);
					}
				}
			}
		}

		private void ShowBtnDoSth(IDIPSys.IDIPData curData)
		{
			this.m_CurSelectData = null;
			if (curData.btnDoSth == IDIPSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE)
			{
				this.m_BtnDoSth.CustomSetActive(false);
			}
			else
			{
				this.m_CurSelectData = curData;
				this.m_BtnDoSth.CustomSetActive(true);
				if (this.m_BtnDoSth != null && this.m_CurSelectData != null)
				{
					Transform transform = this.m_BtnDoSth.transform.Find("Text");
					if (transform)
					{
						transform.GetComponent<Text>().text = this.m_CurSelectData.btnTitle;
					}
				}
			}
		}

		private void UpdateContent(IDIPSys.IDIPData curData)
		{
			if (this.m_bShow)
			{
				if (curData.bNoticeType == 1)
				{
					if (this.m_ImageContent != null)
					{
						this.m_ImageContent.gameObject.CustomSetActive(true);
					}
					DebugHelper.Assert(this.m_TextContent != null && this.m_Title != null);
					if (this.m_TextContent != null)
					{
						this.m_TextContent.gameObject.CustomSetActive(false);
					}
					if (this.m_Title != null)
					{
						this.m_Title.gameObject.CustomSetActive(false);
					}
					base.StartCoroutine(this.DownloadImage(curData.content, delegate(Texture2D text2)
					{
						if (this.m_bShow && this.m_ImageContent != null)
						{
							this.m_ImageContent.SetSprite(Sprite.Create(text2, new Rect(0f, 0f, (float)text2.width, (float)text2.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
						}
					}, 0));
				}
				else if (curData.bNoticeType == 0)
				{
					DebugHelper.Assert(this.m_ImageContent != null && this.m_TextContent != null && this.m_Title != null);
					if (this.m_ImageContent != null)
					{
						this.m_ImageContent.gameObject.CustomSetActive(false);
					}
					if (this.m_TextContent != null)
					{
						this.m_TextContent.gameObject.CustomSetActive(true);
						this.m_TextContent.text = curData.content;
						RectTransform component = this.m_TextContent.transform.parent.gameObject.GetComponent<RectTransform>();
						if (component)
						{
							component.sizeDelta = new Vector2(component.sizeDelta.x, this.m_TextContent.preferredHeight + 50f);
						}
					}
					if (this.m_Title != null)
					{
						this.m_Title.text = curData.title;
						this.m_Title.gameObject.CustomSetActive(true);
					}
				}
				this.ShowBtnDoSth(curData);
			}
		}

		public void SetBanTimeInfo(COM_ACNT_BANTIME_TYPE kType, uint kBanTime)
		{
			if (kType < COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_MAX)
			{
				this.m_BanTimeInfo[(int)kType] = kBanTime;
			}
		}

		[MessageHandler(1043)]
		public static void ModifyBantimeInfo(CSPkg msg)
		{
			for (int i = 0; i < (int)msg.stPkgData.stBanTimeChg.bBanTypeNum; i++)
			{
				ushort kType = msg.stPkgData.stBanTimeChg.BanType[i];
				MonoSingleton<IDIPSys>.GetInstance().SetBanTimeInfo((COM_ACNT_BANTIME_TYPE)kType, msg.stPkgData.stBanTimeChg.BanTime[i]);
			}
		}

		public DateTime GetBanTime(COM_ACNT_BANTIME_TYPE kType)
		{
			if (kType < COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_MAX)
			{
				return Utility.ToUtcTime2Local((long)((ulong)this.m_BanTimeInfo[(int)kType]));
			}
			return default(DateTime);
		}

		public void BanTimeTips(COM_ACNT_BANTIME_TYPE type)
		{
			DateTime banTime = MonoSingleton<IDIPSys>.GetInstance().GetBanTime(type);
			string strContent = string.Format("您被禁止竞技！截止时间为{0}年{1}月{2}日{3}时{4}分", new object[]
			{
				banTime.Year,
				banTime.Month,
				banTime.Day,
				banTime.Hour,
				banTime.Minute
			});
			Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
		}

		public bool IsUseCoinforbid()
		{
			DateTime banTime = this.GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_COINFROZEN);
			DateTime t = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			if (banTime > t)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("UseGoldCoinForbid");
				Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
				return true;
			}
			return false;
		}

		public bool IsUseDianQuanForbid()
		{
			DateTime banTime = this.GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_COUPONSFROZEN);
			DateTime t = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			if (banTime > t)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("UseDianQuanForbid");
				Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
				return true;
			}
			return false;
		}

		public bool IsUseDiamondforbid()
		{
			DateTime banTime = this.GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_DIAMONDFROZEN);
			DateTime t = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			if (banTime > t)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("UseDiamondForbid");
				Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
				return true;
			}
			return false;
		}

		public void RequestQQBox()
		{
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1320u);
			if (accountInfo != null)
			{
				string str = accountInfo.Pf;
				if (accountInfo.Pf == string.Empty)
				{
					str = "desktop_m_qq-73213123-android-73213123-qq-1104466820-BC569F700D770A26CD422F24FD675F10";
				}
				Utility.StringToByteArray(str, ref cSPkg.stPkgData.stGainChestReq.szPf);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}

		[MessageHandler(1321)]
		public static void On_GetQQBoxInfo(CSPkg msg)
		{
			if (msg.stPkgData.stGainChestRsp.iResult == 0)
			{
				int iActId = msg.stPkgData.stGainChestRsp.iActId;
				string text = Utility.UTF8Convert(msg.stPkgData.stGainChestRsp.szGainChestId);
				string title = Utility.UTF8Convert(msg.stPkgData.stGainChestRsp.szChestTitle, (int)msg.stPkgData.stGainChestRsp.wTitleLen);
				string desc = Utility.UTF8Convert(msg.stPkgData.stGainChestRsp.szChestContent, (int)msg.stPkgData.stGainChestRsp.wContentLen);
				Debug.Log("QQBox  receive srv msg boxid " + text);
				Singleton<ApolloHelper>.GetInstance().ShareQQBox(iActId.ToString(), text, title, desc);
			}
			else
			{
				Debug.Log("QQBox getboxinfo error " + msg.stPkgData.stGainChestRsp.iResult);
			}
		}

		public void OnClickOpenXinYue(CUIEvent uiEvent)
		{
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().ShowGameRedDot = false;
			CUICommonSystem.OpenUrl(this.MakeXinYueHttp(), true, 0);
		}

		public void OnClickOpenHelpMe(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenUrl("https://kf.qq.com/touch/scene_product.html?scene_id=kf1062", true, 0);
		}

		public void OnClickOpenHelpMeMoney(CUIEvent uiEvent)
		{
			string strUrl = "https://kf.qq.com/touch/scene_faq.html?scene_id=kf1064";
			CUICommonSystem.OpenUrl(strUrl, true, 0);
		}

		private string MakeXinYueHttp()
		{
			string result = string.Empty;
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			if (accountInfo != null && (accountInfo.Platform == ApolloPlatform.QQ || accountInfo.Platform == ApolloPlatform.Wechat))
			{
				int num = 52;
				string s = string.Empty;
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = string.Empty;
				int logicWorldID = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
				int num2 = 0;
				int num3;
				if (accountInfo.Platform == ApolloPlatform.QQ)
				{
					text2 = ApolloConfig.QQAppID;
					num3 = 1;
				}
				else
				{
					text2 = ApolloConfig.WXAppID;
					num3 = 2;
				}
				for (int i = 0; i < accountInfo.TokenList.Count; i++)
				{
					if (accountInfo.TokenList[i].Type == ApolloTokenType.Access)
					{
						text3 = accountInfo.TokenList[i].Value;
						break;
					}
				}
				s = string.Format("{0},{1},{2},{3}", new object[]
				{
					accountInfo.OpenId,
					text3,
					text2,
					num3
				});
				text = Convert.ToBase64String(Encoding.Default.GetBytes(s));
				result = string.Format("http://apps.game.qq.com/php/tgclub/v2/mobile_open/redirect?game_id={0}&opencode={1}&partition_id={2}&role_id={3}", new object[]
				{
					num,
					text,
					logicWorldID,
					num2
				});
			}
			return result;
		}
	}
}
