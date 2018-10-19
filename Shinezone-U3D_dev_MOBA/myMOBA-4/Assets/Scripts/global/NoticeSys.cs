using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NoticeSys : MonoSingleton<NoticeSys>
{
	public enum NOTICE_STATE
	{
		LOGIN_Before,
		LOGIN_After
	}

	public enum NoticeType
	{
		Text,
		Image
	}

	public enum NoticePlayMode
	{
		Always,
		Random
	}

	public class NoticeDataEx
	{
		public ApolloNoticeData apolloNoticeData;

		public NoticeSys.NoticeType noticeType;

		public int maxShowCount;

		public NoticeSys.NoticePlayMode playMode;
	}

	public class CNoticeDataExAscendingHelper : IComparer<NoticeSys.NoticeDataEx>
	{
		public int Compare(NoticeSys.NoticeDataEx l, NoticeSys.NoticeDataEx r)
		{
			if (l.playMode > r.playMode)
			{
				return 1;
			}
			if (l.playMode < r.playMode)
			{
				return -1;
			}
			return 0;
		}
	}

	public enum BTN_DOSOMTHING
	{
		BTN_DOSOMTHING_NONE,
		BTN_DOSOMTHING_URL,
		BTN_DOSOMTHING_GAME,
		BTN_DOSOMTHING_NOTSHOW
	}

	public static class UrlX
	{
        private static bool IsSafe(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '!':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                    return true;
            }
            return false;
        }


		private static char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 97);
		}

		public static byte[] UrlEncodeBytesToBytes(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				char c = (char)bytes[offset + i];
				if (c == ' ')
				{
					num++;
				}
				else if (!NoticeSys.UrlX.IsSafe(c))
				{
					num2++;
				}
			}
			if (!alwaysCreateReturnValue && num == 0 && num2 == 0)
			{
				return bytes;
			}
			byte[] array = new byte[count + num2 * 2];
			int num3 = 0;
			for (int j = 0; j < count; j++)
			{
				byte b = bytes[offset + j];
				char c2 = (char)b;
				if (NoticeSys.UrlX.IsSafe(c2))
				{
					array[num3++] = b;
				}
				else if (c2 == ' ')
				{
					array[num3++] = 43;
				}
				else
				{
					array[num3++] = 37;
					array[num3++] = (byte)NoticeSys.UrlX.IntToHex(b >> 4 & 15);
					array[num3++] = (byte)NoticeSys.UrlX.IntToHex((int)(b & 15));
				}
			}
			return array;
		}

		public static byte[] UrlEncodeToBytes(string str)
		{
			if (str == null)
			{
				return null;
			}
			return NoticeSys.UrlX.UrlEncodeToBytes(str, Encoding.UTF8);
		}

		public static byte[] UrlEncodeToBytes(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			byte[] bytes = e.GetBytes(str);
			return NoticeSys.UrlX.UrlEncodeToBytes(bytes);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			return NoticeSys.UrlX.UrlEncodeBytesToBytes(bytes, 0, bytes.Length, false);
		}

		public static string UrlEncode(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			return Encoding.ASCII.GetString(NoticeSys.UrlX.UrlEncodeToBytes(str, e));
		}

		public static string UrlEncode(string str)
		{
			if (str == null)
			{
				return null;
			}
			return NoticeSys.UrlX.UrlEncode(str, Encoding.UTF8);
		}

		public static string UrlEncode(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			return Encoding.ASCII.GetString(NoticeSys.UrlX.UrlEncodeToBytes(bytes));
		}
	}

	private const string Image_Key_Data = "Image_Key_Data_";

	private const int MAX_IMAGE_DATA = 20;

	private const int MAX_SHOW_IMG_COUNT = 3;

	public static string s_formNoticeLoginPath = CUIUtility.s_IDIP_Form_Dir + "Form_NoticeLoginBefore.prefab";

	private CUIFormScript m_Form;

	private Image m_ImageContent;

	private Image m_ImageDefault;

	public static bool m_bShowLoginBefore = false;

	private bool m_bShow;

	private Text m_TextContent;

	private Text m_Title;

	private GameObject m_TitleBoard;

	private GameObject m_ImageTop;

	private Image m_PanelImage;

	private static bool m_bLog = true;

	private ListView<NoticeSys.NoticeDataEx> m_NoticeDataList = new ListView<NoticeSys.NoticeDataEx>();

	private ListView<UrlAction> m_MatchUrlAction = new ListView<UrlAction>();

	private NoticeSys.NOTICE_STATE m_CurState;

	private bool m_bLoadImage;

	private NoticeSys.BTN_DOSOMTHING m_btnDoSth;

	private string m_btnUrl = string.Empty;

	private string m_ImageModleTitle = string.Empty;

	private bool m_bGoto;

	private UrlAction m_urlAction;

	protected override void Init()
	{
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MSDK_NOTICE_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseIDIPForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MSDK_NOTICE_Btn_Complete, new CUIEventManager.OnUIEventHandler(this.OnBtnComplete));
		Singleton<CTimerManager>.GetInstance().AddTimer(3000, 1, new CTimer.OnTimeUpHandler(this.OnTimeEnd));
	}

	private void OnTimeEnd(int timersequence)
	{
		this.PreLoadImage();
	}

	private static void PrintLog(string log)
	{
		if (NoticeSys.m_bLog)
		{
			Debug.Log("[Noticesys] " + log);
		}
	}

	public void PreLoadImage()
	{
		List<string> noticeUrl = Singleton<ApolloHelper>.GetInstance().GetNoticeUrl(0, "1");
		for (int i = 0; i < noticeUrl.Count; i++)
		{
			base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(noticeUrl[i], delegate(Texture2D text2)
			{
			}, 0));
		}
		List<string> noticeUrl2 = Singleton<ApolloHelper>.GetInstance().GetNoticeUrl(0, "2");
		for (int j = 0; j < noticeUrl2.Count; j++)
		{
			base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(noticeUrl2[j], delegate(Texture2D text2)
			{
			}, 0));
		}
	}

	public void OnOpenForm(ApolloNoticeInfo noticeInfo, NoticeSys.NOTICE_STATE noticeState)
	{
		this.m_CurState = noticeState;
		this.m_NoticeDataList = new ListView<NoticeSys.NoticeDataEx>();
		this.m_MatchUrlAction.Clear();
		ListView<NoticeSys.NoticeDataEx> listView = new ListView<NoticeSys.NoticeDataEx>();
		int count = noticeInfo.DataList.Count;
		for (int i = 0; i < count; i++)
		{
			ApolloNoticeData apolloNoticeData = noticeInfo.DataList[i];
			if (apolloNoticeData.MsgType == APOLLO_NOTICETYPE.APO_NOTICETYPE_ALERT)
			{
				ListView<UrlAction> listView2 = UrlAction.ParseFromText(apolloNoticeData.ContentUrl, null);
				if (listView2.Count > 0 && listView2[0].action == UrlAction.Action.openMatchUrl)
				{
					this.m_MatchUrlAction.Add(listView2[0]);
				}
				else
				{
					NoticeSys.NoticeDataEx noticeDataEx = new NoticeSys.NoticeDataEx();
					noticeDataEx.apolloNoticeData = apolloNoticeData;
					if (noticeState == NoticeSys.NOTICE_STATE.LOGIN_After)
					{
						if (apolloNoticeData.ContentType == APOLLO_NOTICE_CONTENTTYPE.APO_CONTENTTYPE_WEB && this.IsImageNotice(ref noticeDataEx))
						{
							listView.Add(noticeDataEx);
						}
						else
						{
							this.m_NoticeDataList.Add(noticeDataEx);
						}
					}
					else
					{
						this.m_NoticeDataList.Add(noticeDataEx);
					}
				}
			}
		}
		this.InitImageShowTimes();
		this.FiterImageNotice(listView);
		if (this.m_NoticeDataList.Count > 0)
		{
			this.ShowNoticeWindow(0);
		}
		else if (this.m_CurState == NoticeSys.NOTICE_STATE.LOGIN_After)
		{
			this.ShowOtherTips();
		}
	}

	private bool IsImageNotice(ref NoticeSys.NoticeDataEx ImageData)
	{
		NoticeSys.NoticeDataEx noticeDataEx = ImageData;
		string text = noticeDataEx.apolloNoticeData.MsgUrl;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		int num = text.IndexOf("&end");
		int length = text.Length;
		if (num > 0 && num + "&end".Length == length)
		{
			return false;
		}
		string[] array = text.Split(new char[]
		{
			'&'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IndexOf("Count=", StringComparison.CurrentCultureIgnoreCase) >= 0)
			{
				string text2 = array[i].Replace("Count=", string.Empty);
				if (!string.IsNullOrEmpty(text2))
				{
					int.TryParse(text2, out noticeDataEx.maxShowCount);
				}
			}
			else if (array[i].IndexOf("Mode=", StringComparison.CurrentCultureIgnoreCase) >= 0)
			{
				string text3 = array[i].Replace("Mode=", string.Empty);
				if (!string.IsNullOrEmpty(text3))
				{
					int playMode = 0;
					int.TryParse(text3, out playMode);
					noticeDataEx.playMode = (NoticeSys.NoticePlayMode)playMode;
				}
			}
		}
		if (num + "&end".Length < length && num >= 0)
		{
			text = text.Substring(0, num + "&end".Length);
			noticeDataEx.apolloNoticeData.MsgUrl = text;
			ImageData = noticeDataEx;
			return true;
		}
		return false;
	}

	private void FiterImageNotice(ListView<NoticeSys.NoticeDataEx> datalist)
	{
		int count = datalist.Count;
		if (count > 0 && count <= 3)
		{
			for (int i = 0; i < count; i++)
			{
				NoticeSys.NoticeDataEx ex = datalist[i];
				this.SelectDatatoShow(ex);
			}
			return;
		}
		datalist.Sort(new NoticeSys.CNoticeDataExAscendingHelper());
		ListView<NoticeSys.NoticeDataEx> listView = new ListView<NoticeSys.NoticeDataEx>();
		ListView<NoticeSys.NoticeDataEx> listView2 = new ListView<NoticeSys.NoticeDataEx>();
		for (int j = 0; j < datalist.Count; j++)
		{
			NoticeSys.NoticeDataEx noticeDataEx = datalist[j];
			if (noticeDataEx.playMode == NoticeSys.NoticePlayMode.Always)
			{
				string empty = string.Empty;
				string text = "0";
				if (this.IsShowTimes(noticeDataEx.apolloNoticeData.MsgID, noticeDataEx.maxShowCount, out empty, out text))
				{
					listView.Add(noticeDataEx);
				}
			}
			else if (noticeDataEx.playMode == NoticeSys.NoticePlayMode.Random)
			{
				string empty2 = string.Empty;
				string text2 = "0";
				if (this.IsShowTimes(noticeDataEx.apolloNoticeData.MsgID, noticeDataEx.maxShowCount, out empty2, out text2))
				{
					listView2.Add(noticeDataEx);
				}
			}
		}
		int count2 = listView.Count;
		int count3 = listView2.Count;
		if (count2 <= 3)
		{
			for (int k = 0; k < count2; k++)
			{
				NoticeSys.NoticeDataEx ex2 = listView[k];
				this.SelectDatatoShow(ex2);
			}
			int num = 3 - count2;
			if (num > 0)
			{
				if (count3 <= num)
				{
					for (int l = 0; l < count3; l++)
					{
						NoticeSys.NoticeDataEx ex3 = listView2[l];
						this.SelectDatatoShow(ex3);
					}
				}
				else
				{
					List<int> list = new List<int>(num);
					System.Random random = new System.Random();
					while (list.Count < num)
					{
						int item = random.Next(0, count3);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
					for (int m = 0; m < list.Count; m++)
					{
						int num2 = list[m];
						if (num2 < count3)
						{
							NoticeSys.NoticeDataEx ex4 = listView2[num2];
							this.SelectDatatoShow(ex4);
						}
					}
				}
			}
		}
		else
		{
			for (int n = 0; n < 3; n++)
			{
				if (n < count2)
				{
					NoticeSys.NoticeDataEx ex5 = listView[n];
					this.SelectDatatoShow(ex5);
				}
			}
		}
	}

	private void SelectDatatoShow(NoticeSys.NoticeDataEx ex)
	{
		string empty = string.Empty;
		string arg = "0";
		if (this.IsShowTimes(ex.apolloNoticeData.MsgID, ex.maxShowCount, out empty, out arg))
		{
			string value = string.Format("{0}_{1}", ex.apolloNoticeData.MsgID, arg);
			this.SaveShowTime(empty, value);
			this.m_NoticeDataList.Add(ex);
		}
	}

	private void InitImageShowTimes()
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null && masterRoleInfo.bFirstLoginToday)
		{
			for (int i = 0; i < 20; i++)
			{
				string key = string.Format("{0}_{1}_{2}", "Image_Key_Data_", masterRoleInfo.playerUllUID, i);
				PlayerPrefs.SetString(key, string.Empty);
			}
		}
	}

	private string FindFirstKey()
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null)
		{
			for (int i = 0; i < 20; i++)
			{
				string text = string.Format("{0}_{1}_{2}", "Image_Key_Data_", masterRoleInfo.playerUllUID, i);
				string @string = PlayerPrefs.GetString(text);
				if (string.IsNullOrEmpty(@string))
				{
					return text;
				}
			}
		}
		return string.Empty;
	}

	private void SaveShowTime(string key, string value)
	{
		string text = key;
		if (string.IsNullOrEmpty(text))
		{
			text = this.FindFirstKey();
		}
		if (!string.IsNullOrEmpty(text))
		{
			PlayerPrefs.SetString(text, value);
		}
	}

	private bool IsShowTimes(string msgID, int maxTime, out string saveKey, out string nRetTimestr)
	{
		string text = string.Empty;
		saveKey = string.Empty;
		nRetTimestr = "1";
		if (string.IsNullOrEmpty(msgID))
		{
			return false;
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null)
		{
			for (int i = 0; i < 20; i++)
			{
				string text2 = string.Format("{0}_{1}_{2}", "Image_Key_Data_", masterRoleInfo.playerUllUID, i);
				text = PlayerPrefs.GetString(text2);
				if (!string.IsNullOrEmpty(text) && text.Contains(msgID))
				{
					saveKey = text2;
					break;
				}
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		string[] array = text.Split(new char[]
		{
			'_'
		});
		if (array == null || array.Length != 2 || !(array[0] == msgID))
		{
			return true;
		}
		string s = array[1];
		int num = 0;
		int.TryParse(s, out num);
		if (num < maxTime)
		{
			nRetTimestr = (num + 1).ToString();
			return true;
		}
		return false;
	}

	public void DelayShowNoticeWindow()
	{
		this.ShowNoticeWindow(0);
	}

	private void ShowNoticeWindow(int idx)
	{
		if (this.m_NoticeDataList != null && idx >= 0 && idx < this.m_NoticeDataList.Count)
		{
			ApolloNoticeData apolloNoticeData = this.m_NoticeDataList[idx].apolloNoticeData;
			this.m_NoticeDataList.Remove(this.m_NoticeDataList[idx]);
			this.ProcessShowNoticeWindown(apolloNoticeData);
		}
	}

	private void ProcessShowNoticeWindown(ApolloNoticeData noticeData)
	{
		this.m_bGoto = false;
		this.m_bLoadImage = false;
		string msgID = noticeData.MsgID;
		string openID = noticeData.OpenID;
		string text = noticeData.MsgUrl;
		ListView<UrlAction> listView = UrlAction.ParseFromText(noticeData.ContentUrl, null);
		if (listView.Count > 0)
		{
			this.m_urlAction = listView[0];
		}
		else
		{
			this.m_urlAction = null;
		}
		if (text == null)
		{
			text = string.Empty;
		}
		APOLLO_NOTICETYPE msgType = noticeData.MsgType;
		string startTime = noticeData.StartTime;
		APOLLO_NOTICE_CONTENTTYPE contentType = noticeData.ContentType;
		string msgTitle = noticeData.MsgTitle;
		string msgContent = noticeData.MsgContent;
		NoticeSys.PrintLog(string.Concat(new object[]
		{
			" msgID ",
			noticeData.MsgID,
			" MsgUrl",
			text,
			"msgtitle = ",
			msgTitle,
			" content ",
			msgContent,
			" openid= ",
			openID,
			" MsgType  = ",
			msgType,
			" contenturl =",
			noticeData.ContentUrl
		}));
		uint num = 0u;
		if (this.m_CurState == NoticeSys.NOTICE_STATE.LOGIN_After)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				num = masterRoleInfo.PvpLevel;
			}
		}
		else
		{
			num = 0u;
		}
		this.m_btnUrl = string.Empty;
		this.m_btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
		if (msgType == APOLLO_NOTICETYPE.APO_NOTICETYPE_ALERT)
		{
			bool flag = false;
			bool flag2 = this.CheckIsBtnUrl(text, ref flag, ref this.m_ImageModleTitle, ref this.m_btnDoSth, ref this.m_btnUrl);
			if (flag2)
			{
				if (flag)
				{
					bool flag3 = false;
					flag2 = this.CheckIsBtnUrl("#" + this.m_btnUrl + "&end", ref flag3, ref this.m_ImageModleTitle, ref this.m_btnDoSth, ref this.m_btnUrl);
				}
				if (this.m_btnDoSth == NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NOTSHOW && this.m_btnUrl != MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString())
				{
					NoticeSys.PrintLog("noticesys not show " + this.m_btnUrl);
					return;
				}
				NoticeSys.PrintLog("find url " + this.m_btnUrl + " ori = " + text);
			}
			else
			{
				NoticeSys.PrintLog("find url   ori = " + text);
			}
			if (this.m_CurState == NoticeSys.NOTICE_STATE.LOGIN_After && num <= 5u)
			{
				this.m_btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
				this.m_btnUrl = string.Empty;
			}
			if (this.m_Form == null)
			{
				this.m_Form = Singleton<CUIManager>.GetInstance().OpenForm(NoticeSys.s_formNoticeLoginPath, false, true);
			}
			Transform transform = this.m_Form.gameObject.transform.Find("Panel/BtnGroup/Button_Complte");
			if (this.m_btnUrl != string.Empty && this.m_btnDoSth != NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE)
			{
				this.m_bGoto = true;
			}
			else
			{
				this.m_bGoto = false;
			}
			this.m_bShow = true;
			this.m_TextContent = Utility.GetComponetInChild<Text>(this.m_Form.gameObject, "Panel/ScrollRect/Content/Text");
			this.m_ImageContent = Utility.GetComponetInChild<Image>(this.m_Form.gameObject, "Panel/Image");
			this.m_ImageDefault = Utility.GetComponetInChild<Image>(this.m_Form.gameObject, "Panel/ImageDefalut");
			this.m_Title = Utility.GetComponetInChild<Text>(this.m_Form.gameObject, "Panel/GameObject/Title/ContentTitle");
			this.m_TitleBoard = this.m_Form.gameObject.transform.Find("Panel/GameObject/Title").gameObject;
			this.m_TextContent.gameObject.CustomSetActive(false);
			this.m_ImageContent.gameObject.CustomSetActive(false);
			if (this.m_ImageDefault)
			{
				this.m_ImageDefault.gameObject.CustomSetActive(false);
			}
			this.m_Title.text = msgTitle;
			if (contentType != APOLLO_NOTICE_CONTENTTYPE.APO_CONTENTTYPE_IMAGE)
			{
				if (contentType == APOLLO_NOTICE_CONTENTTYPE.APO_SCONTENTTYPE_TEXT)
				{
					this.m_TextContent.gameObject.CustomSetActive(true);
					this.m_TextContent.text = msgContent;
					this.m_TitleBoard.CustomSetActive(true);
					RectTransform component = this.m_TextContent.transform.parent.gameObject.GetComponent<RectTransform>();
					if (component)
					{
						component.sizeDelta = new Vector2(0f, this.m_TextContent.preferredHeight + 50f);
					}
				}
				else if (contentType == APOLLO_NOTICE_CONTENTTYPE.APO_CONTENTTYPE_WEB && this.m_urlAction != null)
				{
					this.m_bShow = true;
					this.m_TitleBoard.CustomSetActive(true);
					this.m_Title.text = this.m_ImageModleTitle;
					if (this.m_ImageDefault)
					{
						this.m_ImageDefault.gameObject.CustomSetActive(true);
					}
					this.m_ImageContent.gameObject.CustomSetActive(false);
					base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(this.m_urlAction.target, delegate(Texture2D text2)
					{
						if (this.m_bShow && this.m_ImageContent != null)
						{
							this.m_ImageContent.gameObject.CustomSetActive(true);
							if (this.m_ImageDefault)
							{
								this.m_ImageDefault.gameObject.CustomSetActive(false);
							}
							this.m_bLoadImage = true;
							this.m_ImageContent.SetSprite(Sprite.Create(text2, new Rect(0f, 0f, (float)text2.width, (float)text2.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
							BugLocateLogSys.Log("noticesysy contenturl " + this.m_urlAction.target);
						}
					}, 0));
				}
			}
		}
	}

	private void ProcessButton(uint nLevel, bool isGoto)
	{
		if (this.m_Form == null)
		{
			return;
		}
		Transform transform = this.m_Form.gameObject.transform.Find("Panel/Image/Button_Complte");
		if (nLevel <= 5u)
		{
			transform.gameObject.CustomSetActive(false);
		}
		else if (isGoto)
		{
			transform.gameObject.CustomSetActive(true);
		}
		else
		{
			transform.gameObject.CustomSetActive(false);
		}
	}

	private bool CheckIsBtnUrl(string msgUrl, ref bool bTitle, ref string sTitle, ref NoticeSys.BTN_DOSOMTHING btnDoSth, ref string url)
	{
		if (msgUrl == null)
		{
			url = string.Empty;
			btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
			bTitle = false;
			return false;
		}
		string text = "#";
		string value = "&end";
		string text2 = "&";
		int num = msgUrl.IndexOf(text);
		if (num < 0)
		{
			url = string.Empty;
			btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
			bTitle = false;
			return false;
		}
		int num2 = msgUrl.IndexOf(value);
		if (num2 < 0)
		{
			url = string.Empty;
			btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
			bTitle = false;
			return false;
		}
		int num3 = msgUrl.IndexOf(text2);
		if (num3 < 0)
		{
			url = string.Empty;
			btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
			bTitle = false;
			return false;
		}
		string text3 = msgUrl.Substring(num + text.Length, num3 - num - text.Length);
		if (text3.Contains("title="))
		{
			string stringToUnescape = text3.Substring("title=".Length);
			sTitle = Uri.UnescapeDataString(stringToUnescape);
			bTitle = true;
		}
		else if (text3.Contains("button=0"))
		{
			btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_URL;
		}
		else if (text3.Contains("button=1"))
		{
			btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_GAME;
		}
		else if (text3.Contains("button=2"))
		{
			btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NOTSHOW;
		}
		string text4 = string.Empty;
		if (num2 - num3 - text2.Length > 0)
		{
			text4 = msgUrl.Substring(num3 + text2.Length, num2 - num3 - text2.Length);
		}
		if (bTitle)
		{
			url = text4;
			btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
			return true;
		}
		if (text4.Contains("url="))
		{
			url = text4.Substring("url=".Length);
			return true;
		}
		url = string.Empty;
		btnDoSth = NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
		bTitle = false;
		return false;
	}

	private void OnBtnComplete(CUIEvent ciEvent)
	{
		if (this.m_urlAction != null && this.m_urlAction.Execute())
		{
			this.OnCloseIDIPForm(null);
		}
		else if (this.m_bGoto && this.m_bShow && this.m_btnUrl != string.Empty)
		{
			NoticeSys.BTN_DOSOMTHING btnDoSth = this.m_btnDoSth;
			string text = this.m_btnUrl;
			this.OnCloseIDIPForm(null);
			if (btnDoSth == NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_URL)
			{
				CUICommonSystem.OpenUrl(text, true, 0);
			}
			else if (btnDoSth == NoticeSys.BTN_DOSOMTHING.BTN_DOSOMTHING_GAME)
			{
				int num = 0;
				int.TryParse(text, out num);
				if (num > 0)
				{
					CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)num, 0, 0, null);
				}
			}
			text = string.Empty;
		}
	}

	private void OnCloseIDIPForm(CUIEvent uiEvent)
	{
		if (this.m_Form != null)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(this.m_Form);
			this.m_Form = null;
		}
		this.m_bShow = false;
		if (this.m_ImageContent && this.m_ImageContent.sprite && this.m_bLoadImage)
		{
			UnityEngine.Object.Destroy(this.m_ImageContent.sprite);
		}
		if (this.m_NoticeDataList.Count > 0)
		{
			this.ShowNoticeWindow(0);
		}
		else
		{
			this.ShowOtherTips();
		}
	}

	private void ShowOtherTips()
	{
		int count = this.m_MatchUrlAction.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				UrlAction urlAction = this.m_MatchUrlAction[i];
				if (urlAction != null)
				{
					urlAction.Execute();
				}
			}
		}
		if (Singleton<GameStateCtrl>.instance.GetCurrentState() is LobbyState)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && ActivitySys.NeedShowWhenLogin() && !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
			{
				ActivitySys.UpdateLoginShowCnt();
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_OpenForm);
			}
			else
			{
				MonoSingleton<NobeSys>.GetInstance().ShowDelayNobeLoseTipsInfo();
				MonoSingleton<PandroaSys>.GetInstance().ShowPopNews();
			}
		}
	}
}
