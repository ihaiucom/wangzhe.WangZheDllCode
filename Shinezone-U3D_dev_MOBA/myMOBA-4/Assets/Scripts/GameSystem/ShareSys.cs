using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Assets.Scripts.GameSystem
{
	public class ShareSys : MonoSingleton<ShareSys>
	{
		private enum HeroShareFormWidgets
		{
			DisplayRect,
			HeroAmount,
			ButtonClose,
			ShareClickText,
			ShareFriendBtn,
			TimeLineBtn,
			SkinAmount
		}

		public enum PVPShareFormWidgets
		{
			DisplayRect,
			ButtonClose,
			ShareImg,
			BtnSave,
			BtnFriend,
			BtnZone
		}

		private enum MysteryDiscountFOrmWigets
		{
			DiscountNum,
			ButtonClose,
			ShareClickText
		}

		public enum QQGameTeamEventType
		{
			join,
			start,
			end,
			leave,
			destroy
		}

		public struct SHARE_INFO
		{
			public uint heroId;

			public uint skinId;

			public COM_REWARDS_TYPE rewardType;

			public float beginDownloadTime;

			public string shareSkinUrl;

			public void clear()
			{
				this.shareSkinUrl = string.Empty;
			}
		}

		public enum ELoadError
		{
			None,
			SUCC,
			NotFound,
			OutOfTime
		}

		public struct CLoadReq
		{
			public string m_Url;

			public ShareSys.ELoadError m_LoadError;

			public string m_CachePath;

			public int m_Type;
		}

		public struct ShareActivityParam
		{
			public byte bShareType;

			public byte bParamCnt;

			public uint[] ShareParam;

			public bool bUsed;

			public ShareActivityParam(bool buse)
			{
				this.bUsed = buse;
				this.bShareType = 0;
				this.bParamCnt = 0;
				this.ShareParam = null;
			}

			public void clear()
			{
				this.bUsed = false;
				this.ShareParam = null;
				this.bParamCnt = 0;
				this.bShareType = 0;
			}

			public void set(byte kShareType, byte kParamCnt, uint[] kShareParam)
			{
				this.clear();
				this.bUsed = true;
				this.bShareType = kShareType;
				this.bParamCnt = kParamCnt;
				this.ShareParam = new uint[(int)kParamCnt];
				for (int i = 0; i < (int)kParamCnt; i++)
				{
					this.ShareParam[i] = kShareParam[i];
				}
			}
		}

		public delegate void LoadRCallBack3(Texture2D image, ShareSys.CLoadReq req);

		private const string HeroShowImgDir = "UGUI/Sprite/Dynamic/HeroShow/";

		public static readonly string SNS_SHARE_COMMON = "SNS_SHARE_SEND_COMMON";

		public static readonly string SNS_SHARE_SEND_HEART = "SNS_SHARE_SEND_HEART";

		public static readonly string SNS_SHARE_RECALL_FRIEND = "SNS_SHARE_RECALL_FRIEND";

		public static readonly string SNS_GUILD_MATCH_INVITE = "SNS_GUILD_MATCH_INVITE";

		public bool m_bHide;

		public static string s_formShareNewHeroPath = "UGUI/Form/System/ShareUI/Form_ShareNewHero.prefab";

		public static string s_formSharePVPPath = "UGUI/Form/System/ShareUI/Form_SharePVPResult.prefab";

		public static string s_formShareNewAchievementPath = "UGUI/Form/System/Achieve/Form_Achievement_Share.prefab";

		public static string s_formShareMysteryDiscountPath = "UGUI/Form/System/ShareUI/Form_ShareMystery_Discount.prefab";

		public static string s_imageSharePVPAchievement = CUIUtility.s_Sprite_Dynamic_PvpAchievementShare_Dir + "Img_PVP_ShareAchievement_";

		public static string s_Form_IntimacyPath = "UGUI/Form/System/Friend/Form_Intimacy.prefab";

		public string m_SharePicCDNCachePath = string.Empty;

		public ShareSys.SHARE_INFO m_ShareInfo;

		private CSPKG_JOINMULTIGAMEREQ m_ShareRoom;

		private COMDT_INVITE_JOIN_INFO m_ShareTeam;

		public COMDT_RECRUITMENT_SOURCE m_RecruitSource;

		private string m_ShareStr = string.Empty;

		private string m_QQGameTeamStr = string.Empty;

		private string m_WakeupOpenId = string.Empty;

		private string m_RoomInfoStr = string.Empty;

		private string m_RoomModeId = string.Empty;

		private string m_MarkStr = string.Empty;

		private bool m_bIsQQGameTeamOwner;

		private string m_downloadUrl = string.Empty;

		private float g_fDownloadOutTime = 10f;

		private List<ShareSys.CLoadReq> m_DownLoadSkinList = new List<ShareSys.CLoadReq>();

		private Transform m_ShareSkinPicImage;

		private Image m_FriendBtnImage;

		private Image m_TimeLineBtnImage;

		private string m_ShareSkinPicNotFound = string.Empty;

		private string m_ShareSkinPicOutofTime = string.Empty;

		private string m_ShareSkinPicLoading = string.Empty;

		private bool m_bShareHero;

		private bool m_bClickShareFriendBtn;

		public bool m_bShowTimeline;

		private bool m_bClickTimeLineBtn;

		private Transform m_TimelineBtn;

		private bool m_bAdreo306;

		private string m_sharePic = CFileManager.GetCachePath("share.jpg");

		private bool isRegisterd;

		public ShareSys.ShareActivityParam m_ShareActivityParam = new ShareSys.ShareActivityParam(false);

		public bool m_bWinPVPResult;

		private bool m_bSharePvpForm;

		private static string[] m_Support3rdAppList = new string[]
		{
			"QQGameTeam",
			"PenguinEsport",
			"GameHelper"
		};

		private ListView<CSDT_SHARE_TLOG_INFO> m_ShareReportInfoList = new ListView<CSDT_SHARE_TLOG_INFO>();

		private Rect GetScreenShotRect(GameObject displayeRect)
		{
			RectTransform rectTransform = (!(displayeRect != null)) ? new RectTransform() : displayeRect.GetComponent<RectTransform>();
			float num = rectTransform.rect.width * 0.5f;
			float num2 = rectTransform.rect.height * 0.5f;
			Vector3 position = displayeRect.transform.TransformPoint(new Vector3(-num, -num2, 0f));
			position = Singleton<CUIManager>.instance.FormCamera.WorldToScreenPoint(position);
			Vector3 position2 = displayeRect.transform.TransformPoint(new Vector3(num, num2, 0f));
			position2 = Singleton<CUIManager>.instance.FormCamera.WorldToScreenPoint(position2);
			return new Rect(position.x, position.y, Math.Abs(position.x - position2.x), Math.Abs(position.y - position2.y));
		}

		private static void OnLoadNewHeroOrSkin3DModel(GameObject rawImage, uint heroId, uint skinId, bool bInitAnima)
		{
			CUI3DImageScript cUI3DImageScript = (!(rawImage != null)) ? null : rawImage.GetComponent<CUI3DImageScript>();
			string objectName = CUICommonSystem.GetHeroPrefabPath(heroId, (int)skinId, true).ObjectName;
			GameObject gameObject = (!(cUI3DImageScript != null)) ? null : cUI3DImageScript.AddGameObject(objectName, false, false);
			CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
			instance.Set3DModel(gameObject);
			if (gameObject == null)
			{
				return;
			}
			if (bInitAnima)
			{
				instance.InitAnimatList();
				instance.InitAnimatSoundList(heroId, skinId);
				instance.OnModePlayAnima("Come");
			}
		}

        public IEnumerator DownloadImageByTag2(string preUrl, ShareSys.CLoadReq req, ShareSys.LoadRCallBack3 callBack, string tagPath)
        {
            var key__0 = MonoSingleton<IDIPSys>.GetInstance().ToMD5(preUrl);
            var localCachePath__1 = CFileManager.CombinePath(tagPath, key__0);
            var www__2 = new WWW(preUrl);
            var beginTime__3 = Time.time;
            var outOfTime__4 = false;
            while (!www__2.isDone && string.IsNullOrEmpty(www__2.error))
            {
                yield return null;
                if ((Time.time - beginTime__3) <= g_fDownloadOutTime)
                {
                    continue;
                }
                outOfTime__4 = true;
                break;
            }

            if (outOfTime__4)
            {
                req.m_LoadError = ShareSys.ELoadError.OutOfTime;
                www__2.Dispose();
                callBack(null, req);
            }
            else if (!string.IsNullOrEmpty(www__2.error))
            {
                req.m_LoadError = ShareSys.ELoadError.NotFound;
                www__2.Dispose();
                callBack(null, req);
            }
            else
            {
                req.m_LoadError = ShareSys.ELoadError.SUCC;
                var tex__5 = www__2.texture;
                if ((tex__5 != null) && (localCachePath__1 != null))
                {
                    CFileManager.WriteFile(localCachePath__1, www__2.bytes);
                }
                if (callBack != null)
                {
                    callBack(tex__5, req);
                }
            }
        }

		public void PreLoadShareSkinImage(ShareSys.CLoadReq loadReq)
		{
			if (!this.SharePicIsExist(loadReq.m_Url, this.m_SharePicCDNCachePath, loadReq.m_Type) && !this.isDownLoading(loadReq))
			{
				this.m_DownLoadSkinList.Add(loadReq);
				string text = string.Empty;
				if (loadReq.m_Type == 1)
				{
					text = string.Format("{0}HeroSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, loadReq.m_Url);
				}
				else if (loadReq.m_Type == 2)
				{
					text = string.Format("{0}SkinSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, loadReq.m_Url);
				}
				if (!string.IsNullOrEmpty(text))
				{
					base.StartCoroutine(this.DownloadImageByTag2(text, loadReq, delegate(Texture2D text2, ShareSys.CLoadReq tempLoadReq)
					{
						int count = this.m_DownLoadSkinList.Count;
						if (count > 0)
						{
							for (int i = this.m_DownLoadSkinList.Count - 1; i >= 0; i--)
							{
								ShareSys.CLoadReq item = this.m_DownLoadSkinList[i];
								if (item.m_Url == tempLoadReq.m_Url)
								{
									this.m_DownLoadSkinList.Remove(item);
									if (tempLoadReq.m_LoadError != ShareSys.ELoadError.SUCC)
									{
										this.m_DownLoadSkinList.Add(tempLoadReq);
									}
								}
							}
						}
						Debug.Log("skic share pic tempLoadReq " + tempLoadReq.m_LoadError);
						if (this.m_bShareHero && this.m_ShareSkinPicImage != null && loadReq.m_Url == this.m_ShareInfo.shareSkinUrl)
						{
							if (tempLoadReq.m_LoadError == ShareSys.ELoadError.SUCC)
							{
								this.m_ShareSkinPicImage.gameObject.CustomSetActive(true);
								Image component = this.m_ShareSkinPicImage.GetComponent<Image>();
								if (component)
								{
									component.SetSprite(Sprite.Create(text2, new Rect(0f, 0f, (float)text2.width, (float)text2.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
								}
								if (this.m_FriendBtnImage)
								{
									this.BtnGray(this.m_FriendBtnImage, true);
								}
								if (this.m_TimeLineBtnImage && !this.m_bShowTimeline)
								{
									this.BtnGray(this.m_TimeLineBtnImage, true);
								}
							}
							else if (tempLoadReq.m_LoadError == ShareSys.ELoadError.OutOfTime)
							{
								Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicOutofTime, false, 1.5f, null, new object[0]);
							}
							else if (tempLoadReq.m_LoadError == ShareSys.ELoadError.NotFound)
							{
								Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicNotFound, false, 1.5f, null, new object[0]);
							}
						}
					}, this.m_SharePicCDNCachePath));
				}
			}
		}

		private void LoadShareSkinImage(ShareSys.CLoadReq loadReq, Image imageObj)
		{
			string url = loadReq.m_Url;
			string cachePath = loadReq.m_CachePath;
			string cDNSharePicUrl = this.GetCDNSharePicUrl(url, loadReq.m_Type);
			string path = MonoSingleton<IDIPSys>.GetInstance().ToMD5(cDNSharePicUrl);
			string path2 = CFileManager.CombinePath(cachePath, path);
			if (File.Exists(path2))
			{
				byte[] data = File.ReadAllBytes(path2);
				Texture2D texture2D = new Texture2D(4, 4, TextureFormat.ARGB32, false);
				if (texture2D.LoadImage(data) && this.m_bShareHero && imageObj)
				{
					imageObj.gameObject.CustomSetActive(true);
					if (this.m_bShareHero && imageObj != null)
					{
						imageObj.SetSprite(Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
					}
					if (this.m_FriendBtnImage)
					{
						this.BtnGray(this.m_FriendBtnImage, true);
					}
					if (this.m_TimeLineBtnImage && !this.m_bShowTimeline)
					{
						this.BtnGray(this.m_TimeLineBtnImage, true);
					}
				}
			}
			else
			{
				ShareSys.ELoadError loadReq2 = this.GetLoadReq(loadReq);
				if (loadReq2 == ShareSys.ELoadError.NotFound)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicNotFound, false, 1.5f, null, new object[0]);
				}
				else if (loadReq2 == ShareSys.ELoadError.OutOfTime)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicOutofTime, false, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicLoading, false, 0.5f, null, new object[0]);
				}
			}
		}

		private bool isDownLoading(ShareSys.CLoadReq url)
		{
			for (int i = 0; i < this.m_DownLoadSkinList.Count; i++)
			{
				if (url.m_Url == this.m_DownLoadSkinList[i].m_Url)
				{
					return true;
				}
			}
			return false;
		}

		private void RemoveDownLoading(string url)
		{
			for (int i = this.m_DownLoadSkinList.Count - 1; i >= 0; i--)
			{
				if (this.m_DownLoadSkinList[i].m_Url == url)
				{
					this.m_DownLoadSkinList.Remove(this.m_DownLoadSkinList[i]);
				}
			}
		}

		private ShareSys.ELoadError GetLoadReq(ShareSys.CLoadReq url)
		{
			for (int i = 0; i < this.m_DownLoadSkinList.Count; i++)
			{
				if (url.m_Url == this.m_DownLoadSkinList[i].m_Url)
				{
					return this.m_DownLoadSkinList[i].m_LoadError;
				}
			}
			return ShareSys.ELoadError.None;
		}

		private string GetCDNSharePicUrl(string url, int type)
		{
			string result = string.Empty;
			if (type == 1)
			{
				result = string.Format("{0}HeroSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, url);
			}
			else if (type == 2)
			{
				result = string.Format("{0}SkinSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, url);
			}
			return result;
		}

		private bool SharePicIsExist(string url, string tagPath, int type)
		{
			string cDNSharePicUrl = this.GetCDNSharePicUrl(url, type);
			string path = MonoSingleton<IDIPSys>.GetInstance().ToMD5(cDNSharePicUrl);
			string path2 = CFileManager.CombinePath(tagPath, path);
			return File.Exists(path2);
		}

		private void BtnGray(Image imgeBtn, bool bShow)
		{
			if (imgeBtn == null)
			{
				return;
			}
			if (bShow)
			{
				imgeBtn.color = new Color(1f, 1f, 1f, 1f);
				CUIEventScript component = imgeBtn.GetComponent<CUIEventScript>();
				component.enabled = true;
			}
			else
			{
				imgeBtn.color = new Color(0f, 1f, 1f, 1f);
				CUIEventScript component2 = imgeBtn.GetComponent<CUIEventScript>();
				component2.enabled = false;
			}
		}

		private void BtnGray(GameObject imageBtnObj, bool bShow)
		{
			if (imageBtnObj == null)
			{
				return;
			}
			Image component = imageBtnObj.GetComponent<Image>();
			if (component == null)
			{
				return;
			}
			if (bShow)
			{
				component.color = new Color(1f, 1f, 1f, 1f);
				CUIEventScript component2 = component.GetComponent<CUIEventScript>();
				component2.enabled = true;
			}
			else
			{
				component.color = new Color(0f, 1f, 1f, 1f);
				CUIEventScript component3 = component.GetComponent<CUIEventScript>();
				component3.enabled = false;
			}
		}

		public void ShowNewSkinShare(uint heroId, uint skinId, bool bInitAnima = true, COM_REWARDS_TYPE rewardType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, bool interactableTransition = false)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(ShareSys.s_formShareNewHeroPath, false, true);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
			enFormPriority priority = enFormPriority.Priority1;
			if (form != null)
			{
				priority = form.m_priority;
			}
			cUIFormScript.SetPriority(priority);
			cUIFormScript.GetWidget(2).CustomSetActive(true);
			Image component = cUIFormScript.GetWidget(0).GetComponent<Image>();
			component.gameObject.CustomSetActive(false);
			this.m_ShareSkinPicImage = component.transform;
			Text component2 = cUIFormScript.GetWidget(1).GetComponent<Text>();
			if (component2)
			{
				component2.gameObject.CustomSetActive(false);
			}
			this.m_FriendBtnImage = cUIFormScript.GetWidget(4).GetComponent<Image>();
			this.m_TimeLineBtnImage = cUIFormScript.GetWidget(5).GetComponent<Image>();
			if (this.m_FriendBtnImage)
			{
				this.BtnGray(this.m_FriendBtnImage, false);
			}
			if (this.m_TimeLineBtnImage)
			{
				this.BtnGray(this.m_TimeLineBtnImage, false);
			}
			if (!string.IsNullOrEmpty(this.m_ShareInfo.shareSkinUrl))
			{
				this.LoadShareSkinImage(new ShareSys.CLoadReq
				{
					m_Url = this.m_ShareInfo.shareSkinUrl,
					m_CachePath = MonoSingleton<ShareSys>.instance.m_SharePicCDNCachePath,
					m_LoadError = ShareSys.ELoadError.None,
					m_Type = 2
				}, component);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("图片还没有上传", false, 1.5f, null, new object[0]);
			}
			ShareSys.SetSharePlatfText(cUIFormScript.GetWidget(3).GetComponent<Text>());
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				Text component3 = cUIFormScript.GetWidget(6).GetComponent<Text>();
				component3.gameObject.CustomSetActive(true);
				component3.text = masterRoleInfo.GetHeroSkinCount(false).ToString();
			}
		}

		public void ShowNewHeroShare(uint heroId, uint skinId, bool bInitAnima = true, COM_REWARDS_TYPE rewardType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, bool interactableTransition = false)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(ShareSys.s_formShareNewHeroPath, false, true);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
			enFormPriority priority = enFormPriority.Priority1;
			if (form != null)
			{
				priority = form.m_priority;
			}
			cUIFormScript.SetPriority(priority);
			cUIFormScript.GetWidget(2).CustomSetActive(true);
			this.m_FriendBtnImage = cUIFormScript.GetWidget(4).GetComponent<Image>();
			this.m_TimeLineBtnImage = cUIFormScript.GetWidget(5).GetComponent<Image>();
			if (this.m_FriendBtnImage)
			{
				this.BtnGray(this.m_FriendBtnImage, false);
			}
			if (this.m_TimeLineBtnImage)
			{
				this.BtnGray(this.m_TimeLineBtnImage, false);
			}
			Image component = cUIFormScript.GetWidget(0).GetComponent<Image>();
			component.gameObject.CustomSetActive(false);
			this.m_ShareSkinPicImage = component.transform;
			if (!string.IsNullOrEmpty(this.m_ShareInfo.shareSkinUrl))
			{
				this.LoadShareSkinImage(new ShareSys.CLoadReq
				{
					m_Url = this.m_ShareInfo.shareSkinUrl,
					m_CachePath = MonoSingleton<ShareSys>.instance.m_SharePicCDNCachePath,
					m_LoadError = ShareSys.ELoadError.None,
					m_Type = 1
				}, component);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("图片还没有上传", false, 1.5f, null, new object[0]);
			}
			ShareSys.SetSharePlatfText(cUIFormScript.GetWidget(3).GetComponent<Text>());
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				Text component2 = cUIFormScript.GetWidget(1).GetComponent<Text>();
				component2.text = masterRoleInfo.GetHaveHeroCount(false).ToString();
				component2.gameObject.CustomSetActive(true);
			}
			Text component3 = cUIFormScript.GetWidget(6).GetComponent<Text>();
			component3.gameObject.CustomSetActive(false);
		}

		protected override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_CloseNewHeroorSkin, new CUIEventManager.OnUIEventHandler(this.CloseNewHeroForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_NewHero, new CUIEventManager.OnUIEventHandler(this.OpenShareNewHeroForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_CloseNewHeroShareForm, new CUIEventManager.OnUIEventHandler(this.CloseShareHeroForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareFriend, new CUIEventManager.OnUIEventHandler(this.ShareFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Pvp_Share, new CUIEventManager.OnUIEventHandler(this.ShareMyPlayInfoToFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareTimeLine, new CUIEventManager.OnUIEventHandler(this.ShareTimeLine));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareSavePic, new CUIEventManager.OnUIEventHandler(this.SavePic));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_SharePVPScore, new CUIEventManager.OnUIEventHandler(this.SettlementShareBtnHandle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_SharePVPSCcoreClose, new CUIEventManager.OnUIEventHandler(this.OnCloseShowPVPSCore));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_MysteryDiscount, new CUIEventManager.OnUIEventHandler(this.ShareMysteryDiscount));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_DownloadPlayConfirm, new CUIEventManager.OnUIEventHandler(this.ShareDownloadPlayConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_DownloadPlayCancel, new CUIEventManager.OnUIEventHandler(this.ShareDownloadPlayCancel));
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(136u).dwConfValue;
			Singleton<CTimerManager>.GetInstance().AddTimer((int)(dwConfValue * 1000u), -1, new CTimer.OnTimeUpHandler(this.OnReportShareInfo));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.SHARE_PVP_SETTLEDATA_CLOSE, new Action(this.On_SHARE_PVP_SETTLEDATA_CLOSE));
			this.m_SharePicCDNCachePath = CFileManager.GetCachePath();
			string cachePath = CFileManager.GetCachePath("SkinSharePic");
			try
			{
				if (!Directory.Exists(cachePath))
				{
					Directory.CreateDirectory(cachePath);
				}
				this.m_SharePicCDNCachePath = cachePath;
			}
			catch (Exception arg)
			{
				Debug.Log("sharesys cannot create dictionary " + arg);
				this.m_SharePicCDNCachePath = CFileManager.GetCachePath();
			}
			this.m_ShareSkinPicNotFound = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_NotFound");
			this.m_ShareSkinPicOutofTime = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_OutofTime");
			this.m_ShareSkinPicLoading = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_Loading");
			this.m_bAdreo306 = this.IsAdreo306();
			this.PreLoadPVPImage();
		}

		private void OnReportShareInfo(int timerSequence)
		{
			if (Singleton<CBattleSystem>.instance.FormScript == null)
			{
				this.ReportShareInfo();
			}
		}

		public void CloseNewHeroForm(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
			if (form)
			{
				DynamicShadow.EnableDynamicShow(form.gameObject, false);
			}
			this.RemoveDownLoading(this.m_ShareInfo.shareSkinUrl);
			this.m_ShareInfo.clear();
			this.m_bShowTimeline = false;
			Singleton<CUIManager>.GetInstance().CloseForm(CUICommonSystem.s_newHeroOrSkinPath);
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Get_Product_OK);
		}

		public void RequestShareHeroSkinForm(uint heroID, uint skinID, COM_REWARDS_TYPE kType)
		{
			this.m_bShowTimeline = false;
			this.m_ShareInfo.heroId = heroID;
			this.m_ShareInfo.skinId = skinID;
			this.m_ShareInfo.rewardType = kType;
			int type = 1;
			if (kType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN)
			{
				type = 2;
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroID, skinID);
				if (heroSkin != null)
				{
					this.m_ShareInfo.shareSkinUrl = heroSkin.szShareSkinUrl;
				}
			}
			else
			{
				this.m_ShareInfo.shareSkinUrl = this.m_ShareInfo.heroId.ToString();
			}
			ShareSys.CLoadReq loadReq = default(ShareSys.CLoadReq);
			loadReq.m_Url = this.m_ShareInfo.shareSkinUrl;
			loadReq.m_CachePath = this.m_SharePicCDNCachePath;
			loadReq.m_LoadError = ShareSys.ELoadError.None;
			loadReq.m_Type = type;
			MonoSingleton<ShareSys>.GetInstance().PreLoadShareSkinImage(loadReq);
			this.OpenShareNewHeroForm(null);
		}

		public void OpenShareNewHeroForm(CUIEvent uiEvent)
		{
			this.m_ShareActivityParam.clear();
			this.AddshareReportInfo(0u, 0u);
			this.m_bShareHero = true;
			this.m_bClickShareFriendBtn = false;
			if (this.m_ShareInfo.rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO)
			{
				this.m_ShareActivityParam.set(3, 1, new uint[]
				{
					this.m_ShareInfo.heroId
				});
				this.ShowNewHeroShare(this.m_ShareInfo.heroId, this.m_ShareInfo.skinId, false, this.m_ShareInfo.rewardType, false);
			}
			else if (this.m_ShareInfo.rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN)
			{
				uint num = this.m_ShareInfo.skinId;
				if (this.m_ShareInfo.heroId != 0u && this.m_ShareInfo.skinId != 0u)
				{
					num = CSkinInfo.GetSkinCfgId(this.m_ShareInfo.heroId, this.m_ShareInfo.skinId);
				}
				this.m_ShareActivityParam.set(3, 1, new uint[]
				{
					num
				});
				this.ShowNewSkinShare(this.m_ShareInfo.heroId, this.m_ShareInfo.skinId, false, this.m_ShareInfo.rewardType, false);
			}
		}

		public void CloseShareHeroForm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(ShareSys.s_formShareNewHeroPath);
			this.m_bShareHero = false;
			this.m_bClickShareFriendBtn = false;
			this.m_ShareSkinPicImage = null;
			this.m_FriendBtnImage = null;
			this.m_TimeLineBtnImage = null;
		}

		public bool IsInstallPlatform()
		{
			if (!Singleton<ApolloHelper>.GetInstance().IsPlatformInstalled(Singleton<ApolloHelper>.GetInstance().CurPlatform))
			{
				if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox("未安装微信，无法使用该功能", false);
				}
				else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox("未安装手机QQ，无法使用该功能", false);
				}
				return false;
			}
			return true;
		}

		public void ShareFriend(CUIEvent uiEvent)
		{
			Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
			this.m_bClickTimeLineBtn = false;
			if (!this.IsInstallPlatform())
			{
				return;
			}
			GameObject btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript);
			if (btnClose == null)
			{
				return;
			}
			Singleton<CUIManager>.instance.CloseTips();
			btnClose.CustomSetActive(false);
			bool bSettltment = false;
			if (uiEvent.m_srcFormScript.m_formPath == SettlementSystem.SettlementFormName)
			{
				bSettltment = true;
				Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
			}
			GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
			if (displayPanel == null)
			{
				return;
			}
			Rect screenShotRect = this.GetScreenShotRect(displayPanel);
			this.m_bClickShareFriendBtn = true;
			GameObject notShowObj = this.GetNotShowBtn(uiEvent.m_srcFormScript);
			if (notShowObj)
			{
				notShowObj.CustomSetActive(true);
			}
			base.StartCoroutine(this.Capture(screenShotRect, delegate(string filename)
			{
				Debug.Log("sns += capture showfriend filename" + filename);
				this.Share("Session", this.m_sharePic);
				btnClose.CustomSetActive(true);
				if (notShowObj)
				{
					notShowObj.CustomSetActive(false);
				}
				if (bSettltment)
				{
					Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
					bSettltment = false;
				}
			}));
		}

		private void ShareMyPlayInfoToFriend(CUIEvent uiEvent)
		{
			if (!this.IsInstallPlatform())
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			GameObject widget2 = form.GetWidget(15);
			GameObject btnGraph = Utility.FindChild(widget, "btnGraph");
			GameObject btnDetail = Utility.FindChild(widget, "btnDetail");
			if (btnGraph == null || btnDetail == null)
			{
				return;
			}
			bool btnGraphActive = btnGraph.activeSelf;
			bool btnDetailActive = btnDetail.activeSelf;
			btnGraph.CustomSetActive(false);
			btnDetail.CustomSetActive(false);
			Rect screenShotRect = this.GetScreenShotRect(widget2);
			base.StartCoroutine(this.Capture(screenShotRect, delegate(string filename)
			{
				this.Share("Session", this.m_sharePic);
				btnGraph.CustomSetActive(btnGraphActive);
				btnDetail.CustomSetActive(btnDetailActive);
			}));
		}

		private void UpdateTimelineBtn()
		{
			if (this.m_TimelineBtn != null)
			{
				GameObject gameObject = this.m_TimelineBtn.gameObject;
				if (this.m_bShowTimeline && gameObject != null)
				{
					CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
					component.enabled = false;
					gameObject.GetComponent<Button>().interactable = false;
					gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, 0.37f);
					Text componentInChildren = gameObject.GetComponentInChildren<Text>();
					componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0.37f);
				}
				this.m_TimelineBtn = null;
			}
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Share_ClosePVPAchievement);
		}

		public void ShareTimeLine(CUIEvent uiEvent)
		{
			Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
			if (!this.IsInstallPlatform())
			{
				return;
			}
			GameObject btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript);
			if (btnClose == null)
			{
				return;
			}
			Debug.Log(" m_bClickTimeLineBtn " + this.m_bClickTimeLineBtn);
			this.m_TimelineBtn = uiEvent.m_srcWidget.transform;
			this.m_bClickTimeLineBtn = true;
			this.m_bClickShareFriendBtn = false;
			btnClose.CustomSetActive(false);
			Singleton<CUIManager>.instance.CloseTips();
			bool bSettltment = false;
			if (uiEvent.m_srcFormScript.m_formPath == SettlementSystem.SettlementFormName)
			{
				bSettltment = true;
				Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
			}
			GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
			if (displayPanel == null)
			{
				return;
			}
			Rect screenShotRect = this.GetScreenShotRect(displayPanel);
			GameObject notShowObj = this.GetNotShowBtn(uiEvent.m_srcFormScript);
			if (notShowObj)
			{
				notShowObj.CustomSetActive(true);
			}
			base.StartCoroutine(this.Capture(screenShotRect, delegate(string filename)
			{
				Debug.Log("sns += capture showfriend filename" + filename);
				this.Share("TimeLine/Qzone", this.m_sharePic);
				btnClose.CustomSetActive(true);
				if (notShowObj)
				{
					notShowObj.CustomSetActive(false);
				}
				if (bSettltment)
				{
					Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
					bSettltment = false;
				}
			}));
		}

		public void SavePic(CUIEvent uiEvent)
		{
			GameObject btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript);
			if (btnClose == null)
			{
				return;
			}
			if (btnClose)
			{
				btnClose.CustomSetActive(false);
			}
			Singleton<CUIManager>.instance.CloseTips();
			bool bSettltment = false;
			if (uiEvent.m_srcFormScript.m_formPath == SettlementSystem.SettlementFormName)
			{
				bSettltment = true;
				Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
			}
			GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
			if (displayPanel == null)
			{
				return;
			}
			Rect screenShotRect = this.GetScreenShotRect(displayPanel);
			GameObject notShowObj = this.GetNotShowBtn(uiEvent.m_srcFormScript);
			if (notShowObj)
			{
				notShowObj.CustomSetActive(true);
			}
			base.StartCoroutine(this.Capture(screenShotRect, delegate(string filename)
			{
				if (btnClose)
				{
					btnClose.CustomSetActive(true);
				}
				if (notShowObj)
				{
					notShowObj.CustomSetActive(false);
				}
				if (bSettltment)
				{
					Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
				}
				if (Application.platform == RuntimePlatform.Android)
				{
					try
					{
						string text = "/mnt/sdcard/DCIM/Sgame";
						if (!Directory.Exists(text))
						{
							Directory.CreateDirectory(text);
						}
						text = string.Format("{0}/share_{1}.png", text, DateTime.Now.ToFileTimeUtc());
						Debug.Log("sns += SavePic " + text);
						using (FileStream fileStream = new FileStream(this.m_sharePic, FileMode.Open, FileAccess.Read))
						{
							byte[] array = new byte[fileStream.Length];
							int count = Convert.ToInt32(fileStream.Length);
							fileStream.Read(array, 0, count);
							fileStream.Close();
							File.WriteAllBytes(text, array);
						}
						this.RefeshPhoto(text);
						Singleton<CUIManager>.instance.OpenTips("成功保存到相册", false, 1.5f, null, new object[0]);
					}
					catch (Exception ex)
					{
						DebugHelper.Assert(false, "Error In Save Pic, {0}", new object[]
						{
							ex.Message
						});
						Singleton<CUIManager>.instance.OpenTips("保存到相册出错", false, 1.5f, null, new object[0]);
					}
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					this.RefeshPhoto(this.m_sharePic);
					Singleton<CUIManager>.instance.OpenTips("成功保存到相册", false, 1.5f, null, new object[0]);
				}
			}));
			uint dwType = 0u;
			if (this.m_bShareHero)
			{
				dwType = 0u;
			}
			else if (this.m_bSharePvpForm)
			{
				dwType = 1u;
			}
			this.AddshareReportInfo(dwType, 1u);
		}

		private void RefeshPhoto(string filename)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if (androidJavaClass != null)
			{
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				if (@static != null)
				{
					Debug.Log("RefeshPhoto in unity");
					@static.Call("RefeshPhoto", new object[]
					{
						filename
					});
				}
			}
		}

		private bool IsAdreo306()
		{
			string text = SystemInfo.graphicsDeviceName.ToLower();
			char[] separator = new char[]
			{
				' ',
				'\t',
				'\r',
				'\n',
				'+',
				'-',
				':'
			};
			string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return false;
			}
			if (array[0] == "adreno")
			{
				int num = 0;
				for (int i = 1; i < array.Length; i++)
				{
					bool flag = int.TryParse(array[i], out num);
					if (num == 306)
					{
						return true;
					}
				}
			}
			return false;
		}

        private IEnumerator Capture(Rect screenShotRect, Action<string> callback)
        {
            yield return null;
            yield return new WaitForEndOfFrame();
            try
            {
                var _filename___0 = m_sharePic;
                Texture2D _result___1 = null;
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (m_bAdreo306)
                    {
                        var _src___2 = new Texture2D((int)screenShotRect.width, (int)screenShotRect.height, TextureFormat.ARGB32, false);
                        _src___2.ReadPixels(screenShotRect, 0, 0);
                        _src___2.Apply();
                        var _srcColors___3 = _src___2.GetPixels();
                        var _noAlphaColors___4 = new Color[_srcColors___3.Length];
                        var _i___5 = 0;
                        while (_i___5 < _srcColors___3.Length)
                        {
                            var _c___6 = _srcColors___3[_i___5];
                            _noAlphaColors___4[_i___5] = new Color(_c___6.r, _c___6.g, _c___6.b);
                            _i___5++;
                        }
                        _result___1 = new Texture2D((int)screenShotRect.width, (int)screenShotRect.height, TextureFormat.RGB24, false);
                        _result___1.SetPixels(_noAlphaColors___4);
                        _result___1.Apply();
                        Object.Destroy(_src___2);
                    }
                    else
                    {
                        _result___1 = new Texture2D((int)screenShotRect.width, (int)screenShotRect.height, TextureFormat.RGB24, false);
                        _result___1.ReadPixels(screenShotRect, 0, 0);
                        _result___1.Apply();
                    }
                }
                else
                {
                    _result___1 = new Texture2D((int)screenShotRect.width, (int)screenShotRect.height, TextureFormat.RGB24, false);
                    _result___1.ReadPixels(screenShotRect, 0, 0);
                    _result___1.Apply();
                }
                byte[] _data___7 = null;
                if (_result___1 != null)
                {
                    _data___7 = _result___1.EncodeToJPG();
                    Object.Destroy(_result___1);
                }
                if (_data___7 != null)
                {
                    CFileManager.WriteFile(_filename___0, _data___7);
                }
                if (callback != null)
                {
                    callback(_filename___0);
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message };
                DebugHelper.Assert(false, "Exception in ShareSys.Capture, {0}", inParameters);
            }
        }


		private string GenHashCodeFromString(string sKey)
		{
			string text = "#s^un2ye31<cn%|aoXpR,+vh";
			ulong num = 13131uL;
			string str = text;
			str += sKey;
			str += text;
			ulong num2 = 0uL;
			for (int i = 0; i < sKey.Length; i++)
			{
				char c = sKey[i];
				num2 = num2 * num + (ulong)c;
			}
			return num2.ToString();
		}

		public void OnShareCallBack()
		{
			IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
			if (apolloSnsService == null)
			{
				return;
			}
			if (!this.isRegisterd)
			{
				apolloSnsService.onShareEvent += delegate(ApolloShareResult shareResponseInfo)
				{
					string str = string.Format("share result:{0} \n share platform:{1} \n share description:{2}\n share extInfo:{3}", new object[]
					{
						shareResponseInfo.result,
						shareResponseInfo.platform,
						shareResponseInfo.drescription,
						shareResponseInfo.extInfo
					});
					Debug.Log("sns += " + str);
					if (shareResponseInfo.result == ApolloResult.Success)
					{
						if (!(shareResponseInfo.extInfo == ShareSys.SNS_SHARE_SEND_HEART))
						{
							if (!(shareResponseInfo.extInfo == ShareSys.SNS_SHARE_RECALL_FRIEND))
							{
								if (shareResponseInfo.extInfo == ShareSys.SNS_GUILD_MATCH_INVITE)
								{
									Singleton<EventRouter>.instance.BroadCastEvent<bool>("Guild_SendGuildMatchInviteToPlatformGroup_Result", true);
								}
								else
								{
									if (this.m_bClickTimeLineBtn)
									{
										this.m_bShowTimeline = true;
										Singleton<EventRouter>.instance.BroadCastEvent<Transform>(EventID.SHARE_TIMELINE_SUCC, this.m_TimelineBtn);
										this.UpdateTimelineBtn();
									}
									uint dwType = 0u;
									if (this.m_bShareHero)
									{
										dwType = 0u;
									}
									else if (this.m_bSharePvpForm)
									{
										dwType = 1u;
									}
									if (this.m_bClickShareFriendBtn)
									{
										if (ApolloConfig.platform == ApolloPlatform.Wechat)
										{
											this.AddshareReportInfo(dwType, 3u);
										}
										else if (ApolloConfig.platform == ApolloPlatform.QQ)
										{
											this.AddshareReportInfo(dwType, 2u);
										}
									}
									else if (this.m_bClickTimeLineBtn)
									{
										if (ApolloConfig.platform == ApolloPlatform.Wechat)
										{
											this.AddshareReportInfo(dwType, 5u);
										}
										else if (ApolloConfig.platform == ApolloPlatform.QQ)
										{
											this.AddshareReportInfo(dwType, 4u);
										}
									}
									CTaskSys.Send_Share_Task();
									if (this.m_bClickTimeLineBtn)
									{
										this.SendShareActivityDoneMsg();
									}
									this.m_bClickTimeLineBtn = false;
									this.m_bClickShareFriendBtn = false;
								}
							}
						}
					}
					else
					{
						this.m_bClickTimeLineBtn = false;
						this.m_bClickShareFriendBtn = false;
					}
				};
				this.isRegisterd = true;
			}
		}

		private void Share(string buttonType, string sharePathPic)
		{
			IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
			if (apolloSnsService == null)
			{
				return;
			}
			using (FileStream fileStream = new FileStream(sharePathPic, FileMode.Open, FileAccess.Read))
			{
				byte[] array = new byte[fileStream.Length];
				int num = Convert.ToInt32(fileStream.Length);
				fileStream.Read(array, 0, num);
				fileStream.Close();
				this.OnShareCallBack();
				if (ApolloConfig.platform == ApolloPlatform.Wechat)
				{
					if (buttonType == "TimeLine/Qzone")
					{
						apolloSnsService.SendToWeixinWithPhoto(ApolloShareScene.TimeLine, "MSG_INVITE", array, num, string.Empty, "WECHAT_SNS_JUMP_APP");
					}
					else if (buttonType == "Session")
					{
						apolloSnsService.SendToWeixinWithPhoto(ApolloShareScene.Session, "apollo test", array, num);
					}
				}
				else if (ApolloConfig.platform == ApolloPlatform.QQ)
				{
					if (buttonType == "TimeLine/Qzone")
					{
						apolloSnsService.SendToQQWithPhoto(ApolloShareScene.TimeLine, sharePathPic);
					}
					else if (buttonType == "Session")
					{
						apolloSnsService.SendToQQWithPhoto(ApolloShareScene.QSession, sharePathPic);
					}
				}
			}
		}

		public void GShare(string buttonType, string sharePathPic)
		{
			this.m_bClickTimeLineBtn = true;
			this.Share(buttonType, sharePathPic);
		}

		public void SendShareActivityDoneMsg()
		{
			if (Singleton<ActivitySys>.GetInstance().IsShareTask && this.m_ShareActivityParam.bUsed)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5234u);
				cSPkg.stPkgData.stWealContentShareDone.bShareType = this.m_ShareActivityParam.bShareType;
				cSPkg.stPkgData.stWealContentShareDone.bParamCnt = this.m_ShareActivityParam.bParamCnt;
				for (int i = 0; i < (int)this.m_ShareActivityParam.bParamCnt; i++)
				{
					cSPkg.stPkgData.stWealContentShareDone.ShareParam[i] = this.m_ShareActivityParam.ShareParam[i];
				}
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				string message = string.Format("SendShareActivityDoneMsg{0}/{1}/{2} ", this.m_ShareActivityParam.bShareType, this.m_ShareActivityParam.bParamCnt, this.m_ShareActivityParam.ShareParam);
				Debug.Log(message);
			}
			this.m_ShareActivityParam.clear();
		}

		public static void SetSharePlatfText(Text platText)
		{
			if (null == platText)
			{
				return;
			}
			if (ApolloConfig.platform == ApolloPlatform.QQ)
			{
				platText.text = "分享空间";
			}
			else
			{
				platText.text = "分享朋友圈";
			}
		}

		private void PreLoadPVPImage()
		{
			string preUrl = string.Format("{0}PVPShare/Img_PVP_ShareDefeat_0.jpg", BannerImageSys.GlobalLoadPath);
			base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, delegate(Texture2D text2)
			{
			}, 10));
			int[] array = new int[]
			{
				0,
				3,
				6,
				7,
				8
			};
			preUrl = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				preUrl = string.Format("{0}PVPShare/Img_PVP_ShareAchievement_{1}.jpg", BannerImageSys.GlobalLoadPath, array[i]);
				base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, delegate(Texture2D text2D)
				{
				}, 10));
			}
		}

		private void SettlementShareBtnHandle(CUIEvent ievent)
		{
			if (MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
			{
				return;
			}
			Singleton<CChatController>.instance.ShowPanel(false, false);
			this.AddshareReportInfo(1u, 0u);
			this.OpenShowSharePVPFrom(RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_COUNT);
		}

		public void OpenShowSharePVPFrom(RES_SHOW_ACHIEVEMENT_TYPE type)
		{
			this.m_ShareActivityParam.clear();
			this.m_bSharePvpForm = true;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(ShareSys.s_formSharePVPPath, false, true);
			this.UpdateSharePVPForm(cUIFormScript, cUIFormScript.GetWidget(0));
			GameObject shareImg = cUIFormScript.GetWidget(2);
			GameObject btnClose = cUIFormScript.GetWidget(3);
			GameObject btnFriend = cUIFormScript.GetWidget(4);
			GameObject btnZone = cUIFormScript.GetWidget(5);
			this.BtnGray(btnClose, false);
			this.BtnGray(btnFriend, false);
			this.BtnGray(btnZone, false);
			string preUrl = string.Format("{0}PVPShare/Img_PVP_ShareAchievement_{1}.jpg", BannerImageSys.GlobalLoadPath, (int)type);
			base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, delegate(Texture2D text2D)
			{
				if (shareImg == null || shareImg.GetComponent<Image>() == null || text2D == null)
				{
					return;
				}
				this.BtnGray(btnClose, true);
				this.BtnGray(btnFriend, true);
				this.BtnGray(btnZone, !this.m_bShowTimeline);
				shareImg.GetComponent<Image>().SetSprite(Sprite.Create(text2D, new Rect(0f, 0f, (float)text2D.width, (float)text2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
			}, 10));
		}

		public void OpenShareFriendRelationFrom(COM_INTIMACY_STATE state, string name)
		{
			this.m_ShareActivityParam.clear();
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(ShareSys.s_Form_IntimacyPath, false, true);
			GameObject shareImg = Utility.FindChild(cUIFormScript.gameObject, "Intimacy");
			Text componetInChild = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "Intimacy/NameGroup/Name");
			if (componetInChild != null)
			{
				componetInChild.text = name;
			}
			GameObject btnShare = Utility.FindChild(cUIFormScript.gameObject, "BtnGroup/Button_Flaunt");
			this.BtnGray(btnShare, false);
			string preUrl = string.Format("{0}FriendShare/FriendShare_{1}.jpg", BannerImageSys.GlobalLoadPath, (int)state);
			base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, delegate(Texture2D text2D)
			{
				if (shareImg == null || shareImg.GetComponent<Image>() == null || text2D == null)
				{
					return;
				}
				this.BtnGray(btnShare, !this.m_bShowTimeline);
				shareImg.GetComponent<Image>().SetSprite(Sprite.Create(text2D, new Rect(0f, 0f, (float)text2D.width, (float)text2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
			}, 10));
		}

		public void SetShareDefeatImage(Transform imageTransform, CUIFormScript form)
		{
			if (imageTransform == null || form == null)
			{
				return;
			}
			GameObject btnBarrige = form.GetWidget(3);
			GameObject btnFriend = form.GetWidget(4);
			GameObject btnZone = form.GetWidget(5);
			this.BtnGray(btnBarrige, false);
			this.BtnGray(btnFriend, false);
			this.BtnGray(btnZone, false);
			string preUrl = string.Format("{0}PVPShare/Img_PVP_ShareDefeat_0.jpg", BannerImageSys.GlobalLoadPath);
			base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, delegate(Texture2D text2D)
			{
				if (imageTransform == null || imageTransform.GetComponent<Image>() == null || text2D == null)
				{
					return;
				}
				this.BtnGray(btnBarrige, true);
				this.BtnGray(btnFriend, true);
				this.BtnGray(btnZone, !this.m_bShowTimeline);
				imageTransform.GetComponent<Image>().SetSprite(Sprite.Create(text2D, new Rect(0f, 0f, (float)text2D.width, (float)text2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
			}, 10));
		}

		public void UpdateSharePVPForm(CUIFormScript form, GameObject shareRootGO)
		{
			if (form == null)
			{
				return;
			}
			ShareSys.SetSharePlatfText(Utility.GetComponetInChild<Text>(form.gameObject, "ShareGroup/Button_TimeLine/ClickText"));
			if (this.m_bShowTimeline)
			{
				Transform transform = null;
				Text[] componentsInChildren = form.transform.GetComponentsInChildren<Text>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Text text = componentsInChildren[i];
					if ((text != null && text.text == "分享空间") || text.text == "分享朋友圈")
					{
						Transform parent = text.transform.parent;
						if (parent.GetComponent<Button>())
						{
							transform = parent;
							break;
						}
					}
				}
				if (transform)
				{
					GameObject gameObject = transform.gameObject;
					if (gameObject || this.m_bShowTimeline)
					{
						CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
						component.enabled = false;
						gameObject.GetComponent<Button>().interactable = false;
						gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, 0.37f);
						Text componentInChildren = gameObject.GetComponentInChildren<Text>();
						componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0.37f);
					}
				}
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(shareRootGO, "PlayerHead");
				componetInChild.SetImageUrl(masterRoleInfo.HeadUrl);
				Text componetInChild2 = Utility.GetComponetInChild<Text>(shareRootGO, "PlayerName");
				componetInChild2.text = masterRoleInfo.Name;
				CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
				DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
				PlayerKDA playerKDA = null;
				int[] array = new int[3];
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
					PlayerKDA value = current.Value;
					if (value.IsHost)
					{
						playerKDA = value;
					}
					array[(int)value.PlayerCamp] += value.numKill;
				}
				Utility.FindChild(componetInChild.gameObject, "Mvp").CustomSetActive(Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, this.m_bWinPVPResult) == playerKDA.PlayerId);
				if (playerKDA != null)
				{
					Text componetInChild3 = Utility.GetComponetInChild<Text>(shareRootGO, "HostKillNum");
					componetInChild3.text = playerKDA.numKill.ToString();
					Text componetInChild4 = Utility.GetComponetInChild<Text>(shareRootGO, "HostDeadNum");
					componetInChild4.text = playerKDA.numDead.ToString();
					Text componetInChild5 = Utility.GetComponetInChild<Text>(shareRootGO, "HostAssistNum");
					componetInChild5.text = playerKDA.numAssist.ToString();
					Text componetInChild6 = Utility.GetComponetInChild<Text>(shareRootGO, "HostKillTotalNum");
					componetInChild6.text = array[(int)playerKDA.PlayerCamp].ToString();
					Text componetInChild7 = Utility.GetComponetInChild<Text>(shareRootGO, "OppoKillTotalNum");
					componetInChild7.text = array[(int)BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp)].ToString();
					ListView<HeroKDA>.Enumerator enumerator2 = playerKDA.GetEnumerator();
					if (enumerator2.MoveNext())
					{
						HeroKDA current2 = enumerator2.Current;
						ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)current2.HeroId);
						Image componetInChild8 = Utility.GetComponetInChild<Image>(shareRootGO, "HeroHead");
						componetInChild8.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false, false);
						int num = 1;
						for (int j = 1; j < 13; j++)
						{
							switch (j)
							{
							case 1:
								if (current2.LegendaryNum > 0)
								{
									CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.Legendary, num++);
								}
								break;
							case 2:
								if (current2.PentaKillNum > 0)
								{
									CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.PentaKill, num++);
								}
								break;
							case 3:
								if (current2.QuataryKillNum > 0)
								{
									CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.QuataryKill, num++);
								}
								break;
							case 4:
								if (current2.TripleKillNum > 0)
								{
									CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.TripleKill, num++);
								}
								break;
							case 5:
								if (current2.DoubleKillNum > 0)
								{
								}
								break;
							case 6:
								if (current2.bKillMost)
								{
									CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.KillMost, num++);
								}
								break;
							case 7:
								if (current2.bHurtMost)
								{
									CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.HurtMost, num++);
								}
								break;
							case 8:
								if (current2.bHurtTakenMost)
								{
									CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.HurtTakenMost, num++);
								}
								break;
							case 9:
								if (current2.bAsssistMost)
								{
									CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.AsssistMost, num++);
								}
								break;
							}
						}
						for (int k = num; k <= 6; k++)
						{
							CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.NULL, k);
						}
					}
				}
			}
		}

		public void OnCloseShowPVPSCore(CUIEvent ievent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(ShareSys.s_formSharePVPPath);
			this.m_bSharePvpForm = false;
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.PvPShareFin, new uint[0]);
			Singleton<CChatController>.instance.ShowPanel(true, false);
		}

		public void UpdateShareGradeForm(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			ShareSys.SetSharePlatfText(Utility.GetComponetInChild<Text>(form.gameObject, "ShareGroup/Button_TimeLine/ClickText"));
			if (this.m_bShowTimeline)
			{
				Transform transform = null;
				Text[] componentsInChildren = form.transform.GetComponentsInChildren<Text>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Text text = componentsInChildren[i];
					if ((text != null && text.text == "分享空间") || text.text == "分享朋友圈")
					{
						Transform parent = text.transform.parent;
						if (parent.GetComponent<Button>())
						{
							transform = parent;
							break;
						}
					}
				}
				if (transform)
				{
					GameObject gameObject = transform.gameObject;
					if (gameObject || this.m_bShowTimeline)
					{
						CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
						component.enabled = false;
						gameObject.GetComponent<Button>().interactable = false;
						gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, 0.37f);
						Text componentInChildren = gameObject.GetComponentInChildren<Text>();
						componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0.37f);
					}
				}
			}
		}

		private void On_SHARE_PVP_SETTLEDATA_CLOSE()
		{
			this.m_bSharePvpForm = false;
		}

		private void ShareMysteryDiscount(CUIEvent uiEvent)
		{
			MySteryShop instance = Singleton<MySteryShop>.GetInstance();
			if (!instance.IsGetDisCount())
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(ShareSys.s_formShareMysteryDiscountPath, false, true);
			DebugHelper.Assert(cUIFormScript != null, "神秘商店分享form打开失败");
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject widget = cUIFormScript.GetWidget(0);
			if (widget != null)
			{
				Image component = widget.GetComponent<Image>();
				if (component != null)
				{
					component.SetSprite(instance.GetDiscountNumIconPath((uint)instance.GetDisCount()), cUIFormScript, true, false, false, false);
				}
			}
			GameObject widget2 = cUIFormScript.GetWidget(2);
			if (widget2)
			{
				ShareSys.SetSharePlatfText(widget2.GetComponent<Text>());
			}
		}

		private GameObject GetCloseBtn(CUIFormScript form)
		{
			if (form == null)
			{
				return null;
			}
			GameObject result;
			if (form.m_formPath == ShareSys.s_formShareNewHeroPath)
			{
				result = form.GetWidget(2);
			}
			else if (form.m_formPath == ShareSys.s_formSharePVPPath)
			{
				result = form.GetWidget(1);
			}
			else if (form.m_formPath == ShareSys.s_formShareNewAchievementPath)
			{
				result = form.GetWidget(3);
			}
			else if (form.m_formPath == ShareSys.s_formShareMysteryDiscountPath)
			{
				result = form.GetWidget(1);
			}
			else if (form.m_formPath == PvpAchievementForm.s_formSharePVPDefeatPath)
			{
				result = form.GetWidget(0);
			}
			else if (form.m_formPath == SettlementSystem.SettlementFormName)
			{
				result = form.gameObject.transform.FindChild("Panel/Btn_Share_PVP_DATA_CLOSE").gameObject;
			}
			else if (form.m_formPath == "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab")
			{
				result = form.gameObject.transform.FindChild("Button_Close").gameObject;
			}
			else
			{
				if (!(form.m_formPath == ShareSys.s_Form_IntimacyPath))
				{
					DebugHelper.Assert(false, "ShareSys.GetCloseBtn(): error form path = {0}", new object[]
					{
						form.m_formPath
					});
					return null;
				}
				result = form.gameObject.transform.FindChild("Button_Close").gameObject;
			}
			return result;
		}

		private GameObject GetNotShowBtn(CUIFormScript form)
		{
			GameObject result = null;
			if (form == null)
			{
				return null;
			}
			if (form.m_formPath == SettlementSystem.SettlementFormName)
			{
				result = form.gameObject.transform.FindChild("Panel/Logo").gameObject;
			}
			return result;
		}

		private GameObject GetDisplayPanel(CUIFormScript form)
		{
			if (form == null)
			{
				return null;
			}
			GameObject result;
			if (form.m_formPath == ShareSys.s_formShareNewHeroPath)
			{
				result = form.GetWidget(0);
			}
			else if (form.m_formPath == ShareSys.s_formSharePVPPath)
			{
				result = form.GetWidget(0);
			}
			else if (form.m_formPath == ShareSys.s_formShareNewAchievementPath)
			{
				result = form.GetWidget(4);
			}
			else if (form.m_formPath == PvpAchievementForm.s_formSharePVPDefeatPath)
			{
				result = form.GetWidget(2);
			}
			else if (form.m_formPath == SettlementSystem.SettlementFormName)
			{
				result = form.gameObject.transform.FindChild("Panel").gameObject;
			}
			else if (form.m_formPath == "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab")
			{
				result = form.gameObject.transform.FindChild("ShareFrame").gameObject;
			}
			else if (form.m_formPath == ShareSys.s_formShareMysteryDiscountPath)
			{
				result = form.gameObject.transform.FindChild("DiscountShow").gameObject;
			}
			else
			{
				if (!(form.m_formPath == ShareSys.s_Form_IntimacyPath))
				{
					DebugHelper.Assert(false, "ShareSys.GetDisplayPanel(): error form path = {0}", new object[]
					{
						form.m_formPath
					});
					return null;
				}
				result = form.gameObject.transform.FindChild("Intimacy").gameObject;
			}
			return result;
		}

		private bool CheckEnterShareTeamLimit(ref string paramDevicePlatStr, ref string paramPlatformStr)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(191u).dwConfValue;
			if (masterRoleInfo != null && masterRoleInfo.PvpLevel < dwConfValue)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Level_Limit", true, 1f, null, new object[]
				{
					dwConfValue
				});
				return false;
			}
			int num = -1;
			if (!int.TryParse(paramPlatformStr, out num) || num != (int)ApolloConfig.platform)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Different_Platform", true, 1.5f, null, new object[0]);
				return false;
			}
			if (Singleton<GameStateCtrl>.GetInstance().isBattleState || Singleton<GameStateCtrl>.GetInstance().isLoadingState)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_InBattle", true, 1.5f, null, new object[0]);
				return false;
			}
			return true;
		}

		public string PackRoomData(int iRoomEntity, uint dwRoomID, uint dwRoomSeq, byte bMapType, uint dwMapId, ulong ullFeature)
		{
			string text = string.Format("ShareRoom_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", new object[]
			{
				iRoomEntity,
				dwRoomID,
				dwRoomSeq,
				bMapType,
				dwMapId,
				ullFeature,
				(int)Application.platform,
				(int)ApolloConfig.platform
			});
			Debug.Log(text);
			return text;
		}

		public bool UnpackDownloadWatch(string data)
		{
			Debug.Log("rcv " + data);
			if (GameDataMgr.globalInfoDatabin.GetDataByKey(328u).dwConfValue == 0u)
			{
				Debug.Log("Download watch is closed by global config, pls contact GD to open it");
				return false;
			}
			if (string.IsNullOrEmpty(data))
			{
				return false;
			}
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array.Length != 6)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Download_ParamWrong"), false, 1.5f, null, new object[0]);
				return false;
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
			if (Singleton<BattleLogic>.GetInstance().isRuning || form != null || form2 != null)
			{
				return false;
			}
			string appVersion = array[2];
			string usedResourceVersion = array[3];
			int num = 0;
			if (!int.TryParse(array[4], out num))
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Download_ParamWrong"), false, 1.5f, null, new object[0]);
				return false;
			}
			this.m_downloadUrl = WWW.UnEscapeURL(array[5]).Replace('|', '_');
			string text = string.Empty;
			if (CVersion.IsSynchronizedVersion(appVersion, usedResourceVersion))
			{
				if (num < 1048576)
				{
					text = (float)num / 1024f + "KB";
				}
				else
				{
					text = (float)num / 1024f / 1024f + "MB";
				}
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Download_Prompt", new string[]
				{
					text
				}), enUIEventID.Share_DownloadPlayConfirm, enUIEventID.Share_DownloadPlayCancel, false);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Download_VersionNotMatch"), false, 1.5f, null, new object[0]);
			}
			return true;
		}

		public bool UnpackGuildMatchInvite(string data)
		{
			if (Singleton<BattleLogic>.GetInstance().isRuning || Singleton<CMatchingSystem>.GetInstance().IsInMatching || Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam || Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX) != null)
			{
				return false;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "Master RoleInfo is null");
				return false;
			}
			if (!Singleton<CGuildSystem>.GetInstance().IsOpenPlatformGroupFunc())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("功能未开启，敬请期待", false, 1.5f, null, new object[0]);
				return false;
			}
			if (string.IsNullOrEmpty(data))
			{
				return false;
			}
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array.Length != 3)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("参数错误", false, 1.5f, null, new object[0]);
				return false;
			}
			ulong num = 0uL;
			int num2 = 0;
			if (int.TryParse(array[1], out num2))
			{
				ApolloPlatform apolloPlatform = (ApolloPlatform)num2;
				if (apolloPlatform != Singleton<ApolloHelper>.GetInstance().CurPlatform)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("参数错误", false, 1.5f, null, new object[0]);
					return false;
				}
			}
			if (ulong.TryParse(array[2], out num) && num != masterRoleInfo.m_baseGuildInfo.uulUid)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("你不属于发出战队赛邀请的战队", false, 1.5f, null, new object[0]);
				return false;
			}
			if (Singleton<CGuildMatchSystem>.GetInstance().IsReady)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Match_OpenMatchForm);
			}
			else
			{
				Singleton<CGuildMatchSystem>.GetInstance().OpenWhenInvitedFromPlatformGroup = true;
			}
			return true;
		}

		public void ShareDownloadPlayConfirm(CUIEvent evt)
		{
			MonoSingleton<HttpDownload>.GetInstance().Download(this.m_downloadUrl, new HttpDownload.DownloadHandler(this.DownloadWatchHandler), Singleton<CTextManager>.GetInstance().GetText("Download_Tip"));
		}

		public void ShareDownloadPlayCancel(CUIEvent evt)
		{
		}

		public void DownloadWatchHandler(string errorString, byte[] data)
		{
			string downloadReplayPathWithoutCreate = DebugHelper.GetDownloadReplayPathWithoutCreate();
			if (string.IsNullOrEmpty(errorString))
			{
				if (downloadReplayPathWithoutCreate != null && Directory.Exists(downloadReplayPathWithoutCreate))
				{
					Utility.ClearDirectory(downloadReplayPathWithoutCreate, false);
				}
				string text = DebugHelper.downloadReplayPath + Path.GetFileName(this.m_downloadUrl);
				bool flag = Utility.WriteFile(text, data);
				if (flag)
				{
					Singleton<WatchController>.GetInstance().StartReplay(text);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("WriteFileFail"), false, 1.5f, null, new object[0]);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Download_Failed"), false, 1.5f, null, new object[0]);
			}
		}

		public bool UnpackLaunchWatch(string data)
		{
			Debug.Log("rcv " + data);
			if (GameDataMgr.globalInfoDatabin.GetDataByKey(329u).dwConfValue == 0u)
			{
				Debug.Log("Launch watch is closed by global config, pls contact GD to open it");
				return false;
			}
			if (string.IsNullOrEmpty(data))
			{
				return false;
			}
			if (Singleton<BattleLogic>.GetInstance().isRuning)
			{
				return false;
			}
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array.Length != 5)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("external watch param wrong!", false, 1.5f, null, new object[0]);
				return false;
			}
			int friendType = -1;
			COMDT_ACNT_UNIQ cOMDT_ACNT_UNIQ = new COMDT_ACNT_UNIQ();
			if (int.TryParse(array[2], out friendType) && uint.TryParse(array[3], out cOMDT_ACNT_UNIQ.dwLogicWorldId) && ulong.TryParse(array[4], out cOMDT_ACNT_UNIQ.ullUid))
			{
				COBSystem.SendOBServeFriend(cOMDT_ACNT_UNIQ, friendType);
				return true;
			}
			Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Watch_ParamsError"), false, 1.5f, null, new object[0]);
			return true;
		}

		public bool UnpackInviteSNSData(string data, string wakeupOpenId)
		{
			Debug.Log("rcv " + data);
			if (string.IsNullOrEmpty(data))
			{
				return false;
			}
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array != null && array[0] == "mqq")
			{
				this.UnpackRecruitFriendInfo(data);
				this.m_ShareStr = string.Empty;
				return true;
			}
			if (array != null && this.IsSupport3rdAPP(array[0]))
			{
				Singleton<ApolloHelper>.GetInstance().IsLastLaunchFrom3rdAPP = true;
			}
			if (MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
			{
				Debug.Log("正在新手引导中");
				return false;
			}
			this.m_WakeupOpenId = wakeupOpenId;
			if (Singleton<LobbyLogic>.instance.isLogin)
			{
				return this.UnPackSNSDataStr(data);
			}
			this.m_ShareStr = data;
			return true;
		}

		private bool IsSupport3rdAPP(string tag)
		{
			for (int i = 0; i < ShareSys.m_Support3rdAppList.Length; i++)
			{
				if (tag.Equals(ShareSys.m_Support3rdAppList[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool UnPackSNSDataStr(string data)
		{
			Debug.Log("UnPackSNSDataStr " + data);
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array == null || array.Length == 0)
			{
				return false;
			}
			string text = array[0];
			string text2 = text;
			switch (text2)
			{
			case "ShareRoom":
				this.UnpackRoomData(data);
				return false;
			case "ShareTeam":
				this.UnpackTeamData(data);
				return false;
			case "OpenForm":
				return Singleton<LobbyLogic>.GetInstance().ExecOpenForm(array);
			case "watch":
				this.UnpackLaunchWatch(data);
				return false;
			case "DownloadWatch":
				this.UnpackDownloadWatch(data);
				return false;
			case "GuildMatchInvite":
				this.UnpackGuildMatchInvite(data);
				return false;
			}
			if (this.IsSupport3rdAPP(text))
			{
				if (!this.UnPackQQGameTeamData(data))
				{
					this.m_QQGameTeamStr = string.Empty;
					this.m_WakeupOpenId = string.Empty;
					return false;
				}
				return true;
			}
			return false;
		}

		public bool UnpackRoomData(string data)
		{
			Debug.Log("UnpackRoomData");
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array == null || array.Length != 9 || array[0] != "ShareRoom")
			{
				return false;
			}
			if (!this.CheckEnterShareTeamLimit(ref array[7], ref array[8]))
			{
				return false;
			}
			this.m_ShareRoom = new CSPKG_JOINMULTIGAMEREQ();
			if (int.TryParse(array[1], out this.m_ShareRoom.iRoomEntity) && uint.TryParse(array[2], out this.m_ShareRoom.dwRoomID) && uint.TryParse(array[3], out this.m_ShareRoom.dwRoomSeq) && byte.TryParse(array[4], out this.m_ShareRoom.bMapType) && uint.TryParse(array[5], out this.m_ShareRoom.dwMapId) && ulong.TryParse(array[6], out this.m_ShareRoom.ullFeature))
			{
				if (Singleton<LobbyLogic>.instance.isLogin)
				{
					this.SendRoomDataMsg(true);
				}
				return true;
			}
			return false;
		}

		public string PackTeamData(ulong uuid, uint dwTeamId, uint dwTeamSeq, int iTeamEntity, ulong ullTeamFeature, byte bInviterGradeOfRank, byte bGameMode, byte bPkAI, byte bMapType, uint dwMapId, byte bAILevel, byte bMaxTeamNum, byte bTeamGradeOfRank)
		{
			string text = string.Format("ShareTeam_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_{9}_{10}_{11}_{12}_{13}_{14}", new object[]
			{
				uuid,
				dwTeamId,
				dwTeamSeq,
				iTeamEntity,
				ullTeamFeature,
				bInviterGradeOfRank,
				bGameMode,
				bPkAI,
				bMapType,
				dwMapId,
				bAILevel,
				(int)Application.platform,
				(int)ApolloConfig.platform,
				bMaxTeamNum,
				bTeamGradeOfRank
			});
			Debug.Log(text);
			return text;
		}

		public bool UnpackTeamData(string data)
		{
			Debug.Log("UnpackTeamData");
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array == null || array.Length != 16 || array[0] != "ShareTeam")
			{
				return false;
			}
			if (!this.CheckEnterShareTeamLimit(ref array[12], ref array[13]))
			{
				return false;
			}
			this.m_ShareTeam = new COMDT_INVITE_JOIN_INFO();
			this.m_ShareTeam.iInviteType = 2;
			this.m_ShareTeam.stInviteDetail.stInviteJoinTeam = new COMDT_INVITE_TEAM_DETAIL();
			try
			{
				if (ulong.TryParse(array[1], out this.m_ShareTeam.stInviterInfo.ullUid) && uint.TryParse(array[2], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamId) && uint.TryParse(array[3], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamSeq) && int.TryParse(array[4], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.iTeamEntity) && ulong.TryParse(array[5], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.ullTeamFeature) && byte.TryParse(array[6], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bShowGradeOfInviter) && byte.TryParse(array[7], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bGameMode) && byte.TryParse(array[8], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bPkAI) && byte.TryParse(array[9], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMapType) && uint.TryParse(array[10], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.dwMapId) && byte.TryParse(array[11], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bAILevel) && byte.TryParse(array[14], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMaxTeamNum) && byte.TryParse(array[15], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bShowGradeOfRank))
				{
					this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bGradeOfInviter = CLadderSystem.GetGradeDataByShowGrade((int)this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bShowGradeOfInviter).bLogicGrade;
					this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bGradeOfRank = CLadderSystem.GetGradeDataByShowGrade((int)this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bShowGradeOfRank).bLogicGrade;
					if (Singleton<LobbyLogic>.instance.isLogin)
					{
						this.SendTeamDataMsg(true);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
			return false;
		}

		private void SendTeamDataMsg(bool clearData = true)
		{
			if (this.m_ShareTeam != null)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5210u);
				cSPkg.stPkgData.stJoinTeamReq.stInviteJoinInfo = this.m_ShareTeam;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
				Debug.Log("share teamdata msg");
			}
			if (clearData)
			{
				this.ClearTeamDataMsg();
			}
		}

		private void ClearTeamDataMsg()
		{
			this.m_ShareTeam = null;
			this.m_ShareStr = string.Empty;
		}

		public string PackRecruitFriendInfo()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return string.Empty;
			}
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			if (accountInfo == null)
			{
				return string.Empty;
			}
			return string.Format("mqq_{0}_{1}_{2}_{3}", new object[]
			{
				masterRoleInfo.playerUllUID,
				MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID,
				accountInfo.OpenId,
				CUICommonSystem.GetPlatformArea()
			});
		}

		public bool UnpackRecruitFriendInfo(string data)
		{
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array == null || array.Length != 5 || array[0] != "mqq")
			{
				return false;
			}
			Debug.Log("UnpackRecruitFriendInfo " + data);
			this.m_RecruitSource = new COMDT_RECRUITMENT_SOURCE();
			uint dwLogicWorldId = 0u;
			uint.TryParse(array[2], out dwLogicWorldId);
			this.m_RecruitSource.stUin.dwLogicWorldId = dwLogicWorldId;
			uint num = 0u;
			uint.TryParse(array[1], out num);
			this.m_RecruitSource.stUin.ullUid = (ulong)num;
			int iPlatId = 0;
			int.TryParse(array[4], out iPlatId);
			this.m_RecruitSource.iPlatId = iPlatId;
			Utility.StringToByteArray(array[3], ref this.m_RecruitSource.szOpenID);
			return true;
		}

		public void ShareRecruitFriend(string title, string desc)
		{
			Singleton<ApolloHelper>.GetInstance().ShareRecruitFriend(title, desc, this.PackRecruitFriendInfo());
		}

		public void SendShareDataMsg()
		{
			if (!string.IsNullOrEmpty(this.m_ShareStr))
			{
				this.UnPackSNSDataStr(this.m_ShareStr);
				this.m_ShareStr = string.Empty;
			}
			else
			{
				if (this.m_ShareRoom != null)
				{
					this.SendRoomDataMsg(string.IsNullOrEmpty(this.m_RoomModeId));
				}
				if (this.m_ShareTeam != null)
				{
					this.SendTeamDataMsg(string.IsNullOrEmpty(this.m_RoomModeId));
				}
				if (!string.IsNullOrEmpty(this.m_RoomModeId))
				{
					this.SendQQGameTeamCreateMsg(this.m_RoomModeId);
				}
				this.m_ShareStr = string.Empty;
			}
		}

		public void ClearShareDataMsg()
		{
			this.ClearTeamDataMsg();
			this.ClearRoomData();
		}

		private void SendRoomDataMsg(bool clearData = true)
		{
			if (this.m_ShareRoom != null)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5208u);
				cSPkg.stPkgData.stJoinMultiGameReq = this.m_ShareRoom;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
				Singleton<WatchController>.GetInstance().Stop();
				Debug.Log("share roomdata msg");
			}
			if (clearData)
			{
				this.ClearRoomData();
			}
		}

		private void ClearRoomData()
		{
			this.m_ShareRoom = null;
			this.m_ShareStr = string.Empty;
		}

		public string PackQQGameTeamData(int iRoomEntity, uint dwRoomID, uint dwRoomSeq, ulong ullFeature)
		{
			if (string.IsNullOrEmpty(this.m_QQGameTeamStr))
			{
				return string.Empty;
			}
			string text = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
			{
				iRoomEntity,
				dwRoomID,
				dwRoomSeq,
				ullFeature,
				(int)Application.platform,
				(int)ApolloConfig.platform
			});
			Debug.Log(text);
			return text;
		}

		public string PackQQGameTeamData(ulong uuid, uint dwTeamId, uint dwTeamSeq, int iTeamEntity, ulong ullTeamFeature, byte bInviterGradeOfRank, byte bGameMode, byte bPkAI, byte bAILevel, int maxTeamerNum, int GradofRank)
		{
			if (string.IsNullOrEmpty(this.m_QQGameTeamStr))
			{
				return string.Empty;
			}
			string text = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_{9}_{10}_{11}", new object[]
			{
				uuid,
				dwTeamId,
				dwTeamSeq,
				iTeamEntity,
				ullTeamFeature,
				bInviterGradeOfRank,
				bGameMode,
				bPkAI,
				bAILevel,
				maxTeamerNum,
				GradofRank,
				(int)ApolloConfig.platform
			});
			Debug.Log(text);
			return text;
		}

		public bool UnPackQQGameTeamData(string data)
		{
			Debug.Log("UnpackQQGameTeamData");
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			if (accountInfo != null && accountInfo.OpenId != this.m_WakeupOpenId)
			{
				Debug.Log(string.Format("UnPackQQGameTeamData: current accountInfo OpenId:: {0}", accountInfo.OpenId));
				Singleton<ApolloHelper>.GetInstance().IsWXGameCenter = false;
				Singleton<ApolloHelper>.GetInstance().IsQQGameCenter = false;
				this.ClearRoomData();
				this.m_QQGameTeamStr = string.Empty;
				this.m_WakeupOpenId = string.Empty;
				this.m_RoomModeId = string.Empty;
				this.m_ShareStr = string.Empty;
				this.m_RoomInfoStr = string.Empty;
				return true;
			}
			this.m_QQGameTeamStr = data;
			this.m_bIsQQGameTeamOwner = false;
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array == null)
			{
				return false;
			}
			int num = array.Length;
			if (num > 0)
			{
				this.m_MarkStr = array[0];
			}
			if (array == null || (num != 4 && num != 10 && num != 16))
			{
				return false;
			}
			if (!this.IsSupport3rdAPP(array[0]))
			{
				return false;
			}
			if (!this.CheckEnterShareTeamLimit(ref array[num - 2], ref array[num - 1]))
			{
				return false;
			}
			this.m_RoomModeId = array[1];
			string[] array2 = this.m_RoomModeId.Split(new char[]
			{
				'-'
			});
			int num2 = array2.Length;
			if (array2 == null || (num2 != 3 && num2 != 8 && num2 != 4))
			{
				this.m_QQGameTeamStr = string.Empty;
				this.m_WakeupOpenId = string.Empty;
				this.m_RoomModeId = string.Empty;
				return false;
			}
			if (num2 == 8)
			{
				string qQGameTeamStr = this.m_QQGameTeamStr;
				string text = array2[7];
				string sKey = qQGameTeamStr.Replace("-" + text, string.Empty);
				string text2 = this.GenHashCodeFromString(sKey);
				if (text2 != text)
				{
					this.m_QQGameTeamStr = string.Empty;
					this.m_WakeupOpenId = string.Empty;
					this.m_RoomModeId = string.Empty;
					Debug.Log("UnpackQQGameTeamData hashcode eror " + text2);
					return false;
				}
			}
			COM_ROOM_TYPE cOM_ROOM_TYPE = (COM_ROOM_TYPE)Convert.ToInt32(array2[0]);
			COM_BATTLE_MAP_TYPE cOM_BATTLE_MAP_TYPE = (COM_BATTLE_MAP_TYPE)Convert.ToInt32(array2[1]);
			uint dwMapId = Convert.ToUInt32(array2[2]);
			if (num == 4)
			{
				this.SendQQGameTeamCreateMsg(this.m_RoomModeId);
				this.m_bIsQQGameTeamOwner = true;
				return true;
			}
			if (cOM_ROOM_TYPE == COM_ROOM_TYPE.COM_ROOM_TYPE_MATCH)
			{
				this.m_ShareTeam = new COMDT_INVITE_JOIN_INFO();
				this.m_ShareTeam.iInviteType = 2;
				this.m_ShareTeam.stInviteDetail.stInviteJoinTeam = new COMDT_INVITE_TEAM_DETAIL();
				if (ulong.TryParse(array[4], out this.m_ShareTeam.stInviterInfo.ullUid) && uint.TryParse(array[5], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamId) && uint.TryParse(array[6], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamSeq) && int.TryParse(array[7], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.iTeamEntity) && ulong.TryParse(array[8], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.ullTeamFeature) && byte.TryParse(array[9], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bShowGradeOfInviter) && byte.TryParse(array[10], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bGameMode) && byte.TryParse(array[11], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bPkAI) && byte.TryParse(array[12], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bAILevel) && byte.TryParse(array[13], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMaxTeamNum) && byte.TryParse(array[14], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bShowGradeOfRank))
				{
					this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bGradeOfInviter = CLadderSystem.GetGradeDataByShowGrade((int)this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bShowGradeOfInviter).bLogicGrade;
					this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bGradeOfRank = CLadderSystem.GetGradeDataByShowGrade((int)this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bShowGradeOfRank).bLogicGrade;
					this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMapType = (byte)cOM_BATTLE_MAP_TYPE;
					this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.dwMapId = dwMapId;
					if (Singleton<LobbyLogic>.instance.isLogin)
					{
						this.SendTeamDataMsg(false);
					}
					return true;
				}
			}
			else if (cOM_ROOM_TYPE == COM_ROOM_TYPE.COM_ROOM_TYPE_NORMAL)
			{
				this.m_ShareRoom = new CSPKG_JOINMULTIGAMEREQ();
				if (int.TryParse(array[4], out this.m_ShareRoom.iRoomEntity) && uint.TryParse(array[5], out this.m_ShareRoom.dwRoomID) && uint.TryParse(array[6], out this.m_ShareRoom.dwRoomSeq) && ulong.TryParse(array[7], out this.m_ShareRoom.ullFeature))
				{
					this.m_ShareRoom.bMapType = (byte)cOM_BATTLE_MAP_TYPE;
					this.m_ShareRoom.dwMapId = dwMapId;
					if (Singleton<LobbyLogic>.instance.isLogin)
					{
						this.SendRoomDataMsg(false);
					}
					return true;
				}
			}
			return false;
		}

		private void SendQQGameTeamCreateMsg(string roomInfoStr)
		{
			Debug.Log("QQGameTeamCreate");
			if (string.IsNullOrEmpty(roomInfoStr))
			{
				return;
			}
			string[] array = roomInfoStr.Split(new char[]
			{
				'-'
			});
			int num = array.Length;
			if (array == null || (num != 3 && num != 8 && num != 4))
			{
				return;
			}
			COM_ROOM_TYPE cOM_ROOM_TYPE = (COM_ROOM_TYPE)Convert.ToInt32(array[0]);
			COM_BATTLE_MAP_TYPE cOM_BATTLE_MAP_TYPE = (COM_BATTLE_MAP_TYPE)Convert.ToInt32(array[1]);
			uint mapId = Convert.ToUInt32(array[2]);
			if (num == 3 || num == 4)
			{
				if (cOM_ROOM_TYPE == COM_ROOM_TYPE.COM_ROOM_TYPE_MATCH)
				{
					if (Singleton<LobbyLogic>.instance.isLogin)
					{
						ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte)cOM_BATTLE_MAP_TYPE, mapId);
						if (num == 4)
						{
							int num2 = 0;
							int.TryParse(array[3], out num2);
							if (num2 > (int)(pvpMapCommonInfo.bMaxAcntNum / 2))
							{
								num2 = (int)(pvpMapCommonInfo.bMaxAcntNum / 2);
							}
							CMatchingSystem.ReqCreateTeam(mapId, false, cOM_BATTLE_MAP_TYPE, num2, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
						}
						else
						{
							CMatchingSystem.ReqCreateTeam(mapId, false, cOM_BATTLE_MAP_TYPE, (int)(pvpMapCommonInfo.bMaxAcntNum / 2), COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
						}
					}
					return;
				}
				if (cOM_ROOM_TYPE == COM_ROOM_TYPE.COM_ROOM_TYPE_NORMAL)
				{
					if (Singleton<LobbyLogic>.instance.isLogin)
					{
						CRoomSystem.ReqCreateRoom(mapId, (byte)cOM_BATTLE_MAP_TYPE, false);
					}
					return;
				}
			}
			else if (num == 8)
			{
				if (Singleton<LobbyLogic>.instance.isLogin)
				{
					COMDT_QQSPROT_EXTRA cOMDT_QQSPROT_EXTRA = new COMDT_QQSPROT_EXTRA();
					if (int.TryParse(array[3], out cOMDT_QQSPROT_EXTRA.iRoomEntity) && uint.TryParse(array[4], out cOMDT_QQSPROT_EXTRA.dwRoomID) && byte.TryParse(array[5], out cOMDT_QQSPROT_EXTRA.bAddType) && uint.TryParse(array[6], out cOMDT_QQSPROT_EXTRA.dwTimeOutSec))
					{
						CRoomSystem.ReqCreateRoomfromAPP(mapId, (byte)cOM_BATTLE_MAP_TYPE, cOMDT_QQSPROT_EXTRA, false);
					}
				}
				return;
			}
		}

		public void SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType EventType, COM_ROOM_TYPE roomType = COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, byte mapType = 0, uint mapID = 0u, string roomStr = "", uint camp1players = 0u, uint camp2players = 0u)
		{
			if (string.IsNullOrEmpty(this.m_QQGameTeamStr))
			{
				return;
			}
			Debug.Log("QQGameTeamStateChg" + EventType);
			if (EventType == ShareSys.QQGameTeamEventType.join)
			{
				if (!this.CheckQQGameTeamInfo(roomType, mapType, mapID))
				{
					this.m_QQGameTeamStr = string.Empty;
					this.m_WakeupOpenId = string.Empty;
					this.m_RoomModeId = string.Empty;
					return;
				}
				this.m_RoomInfoStr = roomStr;
				if (this.m_bIsQQGameTeamOwner)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("QQGameTeam_Tips1", true, 1.5f, null, new object[0]);
					this.m_bIsQQGameTeamOwner = false;
				}
			}
			if (this.m_RoomInfoStr == null)
			{
				return;
			}
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			if (accountInfo == null)
			{
				return;
			}
			string text = string.Empty;
			string text2 = string.Empty;
			ApolloToken token = accountInfo.GetToken(ApolloTokenType.Access);
			if (token != null)
			{
				text2 = token.Value;
			}
			if (this.m_MarkStr == "PenguinEsport")
			{
				string text3 = this.m_QQGameTeamStr + "_" + this.m_RoomInfoStr;
				text = string.Format("http://game.egame.qq.com/cgi-bin/game_notify?event={0}&openid={1}&openkey={2}&gamedata={3}&appid={4}&camp1players={5}&camp2players={6}&sign={7}", new object[]
				{
					EventType.ToString(),
					accountInfo.OpenId,
					text2,
					text3,
					ApolloConfig.GetAppID(),
					camp1players,
					camp2players,
					this.GenHashCodeFromString(text3)
				});
			}
			else if (this.m_MarkStr == "GameHelper")
			{
				string text4 = string.Empty;
				if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
				{
					text4 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID.ToString();
				}
				text = string.Format("http://api.helper.qq.com/play/smobanotify?event={0}&openid={1}&openkey={2}&gamedata={3}&uuid={4}", new object[]
				{
					EventType.ToString(),
					accountInfo.OpenId,
					text2,
					this.m_QQGameTeamStr + "_" + this.m_RoomInfoStr,
					text4
				});
			}
			else
			{
				text = string.Format("http://openmobile.qq.com/gameteam/game_notify?event={0}&openid={1}&openkey={2}&gamedata={3}", new object[]
				{
					EventType.ToString(),
					accountInfo.OpenId,
					text2,
					this.m_QQGameTeamStr + "_" + this.m_RoomInfoStr
				});
			}
			Debug.Log("QQGameTeamStateChg:" + text);
			base.StartCoroutine(this.QQGameTeamStateChgGet(text));
			if (EventType == ShareSys.QQGameTeamEventType.end)
			{
				this.m_QQGameTeamStr = string.Empty;
				this.m_WakeupOpenId = string.Empty;
				this.m_RoomInfoStr = string.Empty;
			}
			this.ClearQQGameCreateInfo();
		}

        private IEnumerator QQGameTeamStateChgGet(string url)
        {
            var www = new WWW(url);
            yield return www;
            if (www.error != null)
            {
                Debug.Log("QQGameTeamStateChgRequestError......" + www.error);
            }
        }

		private bool CheckQQGameTeamInfo(COM_ROOM_TYPE roomType, byte mapType, uint mapID)
		{
			string[] array = this.m_QQGameTeamStr.Split(new char[]
			{
				'_'
			});
			if (array == null)
			{
				return false;
			}
			int num = array.Length;
			if (array == null || (num != 4 && num != 10 && array.Length != 16))
			{
				return false;
			}
			if (!this.IsSupport3rdAPP(array[0]))
			{
				return false;
			}
			string text = array[1];
			string[] array2 = text.Split(new char[]
			{
				'-'
			});
			return array2 != null && (array2.Length == 3 || array2.Length == 8) && Convert.ToUInt32(array2[0]) == (uint)roomType && Convert.ToByte(array2[1]) == mapType && Convert.ToUInt32(array2[2]) == mapID;
		}

		private void ClearQQGameCreateInfo()
		{
			this.m_ShareStr = string.Empty;
			this.m_RoomModeId = string.Empty;
			this.m_ShareRoom = null;
			this.m_ShareTeam = null;
		}

		public bool IsQQGameTeamCreate()
		{
			return !string.IsNullOrEmpty(this.m_QQGameTeamStr);
		}

		public void AddshareReportInfo(uint dwType, uint dwSubType)
		{
			bool flag = false;
			for (int i = 0; i < this.m_ShareReportInfoList.Count; i++)
			{
				CSDT_SHARE_TLOG_INFO cSDT_SHARE_TLOG_INFO = this.m_ShareReportInfoList[i];
				if (cSDT_SHARE_TLOG_INFO.dwType == dwType && cSDT_SHARE_TLOG_INFO.dwSubType == dwSubType)
				{
					cSDT_SHARE_TLOG_INFO.dwCnt += 1u;
					flag = true;
				}
			}
			if (!flag)
			{
				CSDT_SHARE_TLOG_INFO cSDT_SHARE_TLOG_INFO2 = new CSDT_SHARE_TLOG_INFO();
				cSDT_SHARE_TLOG_INFO2.dwCnt = 1u;
				cSDT_SHARE_TLOG_INFO2.dwType = dwType;
				cSDT_SHARE_TLOG_INFO2.dwSubType = dwSubType;
				this.m_ShareReportInfoList.Add(cSDT_SHARE_TLOG_INFO2);
			}
		}

		private void ReportShareInfo()
		{
			CSDT_TRANK_TLOG_INFO[] uiTlog = Singleton<RankingSystem>.instance.GetUiTlog();
			if (uiTlog.Length == 0 && this.m_ShareReportInfoList.Count == 0)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4307u);
			int num = this.m_ShareReportInfoList.Count;
			cSPkg.stPkgData.stShareTLogReq.bNum = (byte)num;
			for (int i = 0; i < num; i++)
			{
				cSPkg.stPkgData.stShareTLogReq.astShareDetail[i].dwType = this.m_ShareReportInfoList[i].dwType;
				cSPkg.stPkgData.stShareTLogReq.astShareDetail[i].dwSubType = this.m_ShareReportInfoList[i].dwSubType;
				cSPkg.stPkgData.stShareTLogReq.astShareDetail[i].dwCnt = this.m_ShareReportInfoList[i].dwCnt;
			}
			num = uiTlog.Length;
			cSPkg.stPkgData.stShareTLogReq.dwTrankNum = (uint)num;
			for (int j = 0; j < num; j++)
			{
				cSPkg.stPkgData.stShareTLogReq.astTrankDetail[j].dwType = uiTlog[j].dwType;
				cSPkg.stPkgData.stShareTLogReq.astTrankDetail[j].dwCnt = uiTlog[j].dwCnt;
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			this.m_ShareReportInfoList.Clear();
			Singleton<RankingSystem>.instance.ClearUiTlog();
		}

		[MessageHandler(4308)]
		public static void OnShareReport(CSPkg msg)
		{
			Debug.Log("share report " + msg.stPkgData.stShareTLogRsp.iErrCode);
		}
	}
}
