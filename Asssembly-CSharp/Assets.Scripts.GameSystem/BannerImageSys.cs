using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class BannerImageSys : MonoSingleton<BannerImageSys>
	{
		public enum BannerType
		{
			BannerType_None,
			BannerType_URL,
			BannerType_InGame,
			BannerType_DeepLink,
			BannerType_Loading,
			BannerType_Outer_Browser,
			BannerType_CheckIn,
			BANNER_TYPE_QQ_BOX,
			BANNER_TYPE_PandoraOpen,
			BANNER_TYPE_BlockWaifaChannel,
			BANNER_TYPE_CDNUrl,
			RES_BANNER_TYPE_14CHECKIN,
			RES_BANNER_TYPE_WeixinZone,
			BANNER_TYPE_GSDK
		}

		public enum BannerPosition
		{
			Lobby,
			Mall
		}

		public class BannerImage
		{
			public int verisoncode;

			public int ImageListCount;

			public BannerImageSys.BannerImageInfo[] m_ImageInfoList;
		}

		public class BannerImageInfo
		{
			public ResBannerImage resImgInfo;

			public bool imgLoadSucc;

			public RES_WEAL_TIME_TYPE TimeType
			{
				get
				{
					return (RES_WEAL_TIME_TYPE)this.resImgInfo.dwTimeType;
				}
			}

			public long StartTime
			{
				get
				{
					if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS)
					{
						CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						if (masterRoleInfo != null)
						{
							if ((ulong)masterRoleInfo.AccountRegisterTime >= this.resImgInfo.ullStartTime)
							{
								return masterRoleInfo.AccountRegisterTime_ZeroDay;
							}
							return (long)((ulong)-195287296);
						}
					}
					return (long)this.resImgInfo.ullStartTime;
				}
			}

			public long CloseTime
			{
				get
				{
					if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS)
					{
						CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						if (masterRoleInfo != null)
						{
							if ((ulong)masterRoleInfo.AccountRegisterTime >= this.resImgInfo.ullStartTime)
							{
								return masterRoleInfo.AccountRegisterTime_ZeroDay + (long)(86399uL * this.resImgInfo.ullEndTime) - 1L;
							}
							return (long)((ulong)-195287296);
						}
					}
					return (long)this.resImgInfo.ullEndTime;
				}
			}

			public BannerImageInfo()
			{
				this.resImgInfo = new ResBannerImage();
				this.imgLoadSucc = false;
			}

			public bool IsShowNow()
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					return false;
				}
				if (this.TimeType != RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_LIMIT && this.TimeType != RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_LIMIT && this.TimeType != RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS)
				{
					return this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_FOREVER;
				}
				if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_LIMIT && ((ulong)masterRoleInfo.AccountRegisterTime < (ulong)this.StartTime || (ulong)masterRoleInfo.AccountRegisterTime > (ulong)this.CloseTime))
				{
					return false;
				}
				long num = (long)CRoleInfo.GetCurrentUTCTime();
				int num2 = (int)(num - this.StartTime);
				if (num2 < 0)
				{
					return false;
				}
				num2 = (int)(num - this.CloseTime);
				return num2 <= 0;
			}
		}

		private struct CDNUrl
		{
			public string url;

			public uint id;
		}

		public struct BtnControlInfo
		{
			public bool bLoadSucc;

			public int linkType;

			public string linkUrl;

			public ulong startTime;

			public ulong endTime;

			public bool isTimeValid(ulong curTime)
			{
				return this.bLoadSucc && this.startTime < curTime && this.endTime >= curTime;
			}
		}

		public struct WaiFaBlockPlatformChannel
		{
			public string m_SrcInfo;

			public int m_nChannelCount;

			public string[] m_ChannelList;

			public WaiFaBlockPlatformChannel(int nCount)
			{
				this.m_nChannelCount = nCount;
				this.m_SrcInfo = string.Empty;
				if (nCount > 0)
				{
					this.m_ChannelList = new string[nCount];
				}
				else
				{
					this.m_ChannelList = null;
				}
			}

			public bool isBlock()
			{
				if (this.m_nChannelCount <= 0)
				{
					return false;
				}
				int channelID = Singleton<ApolloHelper>.GetInstance().GetChannelID();
				for (int i = 0; i < this.m_ChannelList.Length; i++)
				{
					if (this.m_ChannelList[i] == channelID.ToString())
					{
						return true;
					}
				}
				return false;
			}
		}

		private BannerImageSys.BannerImage m_BannerImage;

		private bool m_bLoadAllImage;

		private bool m_bLoadXmlSucc;

		private List<BannerImageSys.CDNUrl> m_CDNUrlMgr = new List<BannerImageSys.CDNUrl>();

		private BannerImageSys.BtnControlInfo m_DeepLinkInfo;

		private BannerImageSys.BtnControlInfo m_QQBoxInfo;

		private BannerImageSys.WaiFaBlockPlatformChannel m_WaifaBlockPlatformChannel;

		private string m_GlobalBannerImagePath = string.Empty;

		private bool m_bTest;

		public static string GlobalLoadPath = "http://image.smoba.qq.com/Banner/img/";

		public bool LoadALLImage
		{
			get
			{
				return this.m_bLoadAllImage;
			}
		}

		public bool LoadXmlSUCC
		{
			get
			{
				return this.m_bLoadXmlSucc;
			}
		}

		public BannerImageSys.BtnControlInfo DeepLinkInfo
		{
			get
			{
				return this.m_DeepLinkInfo;
			}
		}

		public BannerImageSys.BtnControlInfo QQBOXInfo
		{
			get
			{
				return this.m_QQBoxInfo;
			}
		}

		public string GlobalBannerImagePath
		{
			get
			{
				return this.m_GlobalBannerImagePath;
			}
		}

		public BannerImageSys.BannerImage GetCurBannerImage()
		{
			return this.m_BannerImage;
		}

		public void ClearSeverData()
		{
			this.m_BannerImage = null;
			this.m_CDNUrlMgr.Clear();
		}

		public string GetCDNUrl(uint id)
		{
			for (int i = 0; i < this.m_CDNUrlMgr.get_Count(); i++)
			{
				BannerImageSys.CDNUrl cDNUrl = this.m_CDNUrlMgr.get_Item(i);
				if (cDNUrl.id == id)
				{
					return cDNUrl.url;
				}
			}
			return string.Empty;
		}

		public void LoadConfigServer()
		{
			this.ClearSeverData();
			int count = GameDataMgr.svr2BannerImageDict.Count;
			if (count > 0)
			{
				this.m_BannerImage = new BannerImageSys.BannerImage();
				this.m_BannerImage.verisoncode = 0;
				this.m_BannerImage.ImageListCount = count;
				this.m_BannerImage.m_ImageInfoList = new BannerImageSys.BannerImageInfo[count];
				DictionaryView<uint, ResBannerImage>.Enumerator enumerator = GameDataMgr.svr2BannerImageDict.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, ResBannerImage> current = enumerator.Current;
					ResBannerImage value = current.get_Value();
					this.m_BannerImage.m_ImageInfoList[num] = new BannerImageSys.BannerImageInfo();
					this.m_BannerImage.m_ImageInfoList[num].resImgInfo = value;
					this.m_BannerImage.m_ImageInfoList[num].imgLoadSucc = false;
					num++;
				}
			}
			if (this.m_BannerImage != null)
			{
				this.PreloadBannerImage();
			}
		}

		public static int ComparebyShowIdx(BannerImageSys.BannerImageInfo info1, BannerImageSys.BannerImageInfo info2)
		{
			if (info1.resImgInfo.dwShowID > info2.resImgInfo.dwShowID)
			{
				return 1;
			}
			if (info1.resImgInfo.dwShowID == info2.resImgInfo.dwShowID)
			{
				return 0;
			}
			return -1;
		}

		private bool isPreloadImageType(BannerImageSys.BannerType kType)
		{
			return kType == BannerImageSys.BannerType.BannerType_InGame || kType == BannerImageSys.BannerType.BannerType_URL || kType == BannerImageSys.BannerType.BannerType_Loading || kType == BannerImageSys.BannerType.BannerType_CheckIn || kType == BannerImageSys.BannerType.RES_BANNER_TYPE_14CHECKIN;
		}

		private void PreloadBannerImage()
		{
			if (this.m_BannerImage == null)
			{
				return;
			}
			int imageListCount = this.m_BannerImage.ImageListCount;
			for (int i = 0; i < imageListCount; i++)
			{
				BannerImageSys.BannerImageInfo bannerImageInfo = this.m_BannerImage.m_ImageInfoList[i];
				if (bannerImageInfo != null)
				{
					string text = bannerImageInfo.resImgInfo.szImgUrl;
					if (this.isPreloadImageType((BannerImageSys.BannerType)bannerImageInfo.resImgInfo.dwBannerType))
					{
						this.m_BannerImage.m_ImageInfoList[i].imgLoadSucc = false;
						text = string.Format("{0}{1}", BannerImageSys.GlobalLoadPath, text);
						base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImageByTag(text, i, delegate(Texture2D text2, int imageIDX)
						{
							if (this.m_BannerImage != null && this.m_BannerImage.m_ImageInfoList != null && imageIDX < this.m_BannerImage.m_ImageInfoList.Length)
							{
								this.m_BannerImage.m_ImageInfoList[imageIDX].imgLoadSucc = true;
							}
						}, this.GlobalBannerImagePath, 0));
					}
					else if (bannerImageInfo.resImgInfo.dwBannerType == 3u)
					{
						this.m_DeepLinkInfo.linkType = (int)bannerImageInfo.resImgInfo.dwJumpEntrance;
						this.m_DeepLinkInfo.linkUrl = bannerImageInfo.resImgInfo.szHttpUrl;
						this.m_DeepLinkInfo.startTime = bannerImageInfo.resImgInfo.ullStartTime;
						this.m_DeepLinkInfo.endTime = bannerImageInfo.resImgInfo.ullEndTime;
						this.m_DeepLinkInfo.bLoadSucc = true;
					}
					else if (bannerImageInfo.resImgInfo.dwBannerType == 7u)
					{
						this.m_QQBoxInfo.linkUrl = bannerImageInfo.resImgInfo.szHttpUrl;
						this.m_QQBoxInfo.startTime = bannerImageInfo.resImgInfo.ullStartTime;
						this.m_QQBoxInfo.endTime = bannerImageInfo.resImgInfo.ullEndTime;
						this.m_QQBoxInfo.bLoadSucc = true;
					}
					else if (bannerImageInfo.resImgInfo.dwBannerType == 8u)
					{
						if (bannerImageInfo.resImgInfo.iTargetID > 0)
						{
							MonoSingleton<PandroaSys>.GetInstance().InitSys();
						}
					}
					else if (bannerImageInfo.resImgInfo.dwBannerType == 13u)
					{
						this.ProcessNetACC(bannerImageInfo.resImgInfo);
					}
					else if (bannerImageInfo.resImgInfo.dwBannerType == 9u)
					{
						if (!string.IsNullOrEmpty(bannerImageInfo.resImgInfo.szHttpUrl) && Application.platform == RuntimePlatform.Android)
						{
							string szHttpUrl = bannerImageInfo.resImgInfo.szHttpUrl;
							string[] array = szHttpUrl.Split(new char[]
							{
								';'
							});
							if (array.Length > 0)
							{
								this.m_WaifaBlockPlatformChannel = new BannerImageSys.WaiFaBlockPlatformChannel(array.Length);
								this.m_WaifaBlockPlatformChannel.m_SrcInfo = szHttpUrl;
								for (int j = 0; j < array.Length; j++)
								{
									this.m_WaifaBlockPlatformChannel.m_ChannelList[j] = array[j];
								}
							}
						}
					}
					else if (bannerImageInfo.resImgInfo.dwBannerType == 10u)
					{
						BannerImageSys.CDNUrl cDNUrl = default(BannerImageSys.CDNUrl);
						cDNUrl.id = bannerImageInfo.resImgInfo.dwID;
						cDNUrl.url = bannerImageInfo.resImgInfo.szHttpUrl;
						this.m_CDNUrlMgr.Add(cDNUrl);
					}
				}
			}
		}

		private void ProcessNetACC(ResBannerImage resImgInfo)
		{
		}

		public bool IsWaifaBlockChannel()
		{
			return this.m_WaifaBlockPlatformChannel.isBlock();
		}

		protected override void Init()
		{
			this.m_GlobalBannerImagePath = CFileManager.GetCachePath();
			string cachePath = CFileManager.GetCachePath("BannerImage");
			try
			{
				if (!Directory.Exists(cachePath))
				{
					Directory.CreateDirectory(cachePath);
				}
				this.m_GlobalBannerImagePath = cachePath;
			}
			catch (Exception ex)
			{
				Debug.Log("bannerimagesys cannot create dictionary " + ex);
				this.m_GlobalBannerImagePath = CFileManager.GetCachePath();
			}
			this.m_DeepLinkInfo.bLoadSucc = false;
			this.m_QQBoxInfo.bLoadSucc = false;
			this.m_WaifaBlockPlatformChannel = new BannerImageSys.WaiFaBlockPlatformChannel(0);
		}

		public void TrySetCheckInImage(Image img)
		{
			if (this.m_BannerImage == null || img == null)
			{
				return;
			}
			string text = null;
			for (int i = 0; i < this.m_BannerImage.ImageListCount; i++)
			{
				ResBannerImage resImgInfo = this.m_BannerImage.m_ImageInfoList[i].resImgInfo;
				if (resImgInfo.dwBannerType == 6u)
				{
					text = string.Format("{0}{1}", BannerImageSys.GlobalLoadPath, resImgInfo.szImgUrl);
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = string.Format("{0}{1}", BannerImageSys.GlobalLoadPath, "CheckIn/20151028.png");
			}
			if (!string.IsNullOrEmpty(text))
			{
				base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImageByTag(text, 0, delegate(Texture2D text2D, int imageIndex)
				{
					if (img == null || text2D == null)
					{
						return;
					}
					img.SetSprite(Sprite.Create(text2D, new Rect(0f, 0f, (float)text2D.width, (float)text2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
				}, MonoSingleton<BannerImageSys>.GetInstance().GlobalBannerImagePath, 0));
			}
		}

		public void TrySetLoadingImage(uint picIndex, Image img)
		{
			if (this.m_BannerImage == null || img == null)
			{
				return;
			}
			string text = null;
			for (int i = 0; i < this.m_BannerImage.ImageListCount; i++)
			{
				ResBannerImage resImgInfo = this.m_BannerImage.m_ImageInfoList[i].resImgInfo;
				if (resImgInfo.dwBannerType == 4u && resImgInfo.dwShowID == picIndex)
				{
					text = string.Format("{0}{1}", BannerImageSys.GlobalLoadPath, resImgInfo.szImgUrl);
					break;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImageByTag(text, 0, delegate(Texture2D text2D, int imageIndex)
				{
					if (img == null || text2D == null)
					{
						return;
					}
					img.SetSprite(Sprite.Create(text2D, new Rect(0f, 0f, (float)text2D.width, (float)text2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
				}, MonoSingleton<BannerImageSys>.GetInstance().GlobalBannerImagePath, 0));
			}
		}

		public void TrySet14CheckInImage(Image img)
		{
			if (this.m_BannerImage == null || img == null)
			{
				return;
			}
			string text = null;
			for (int i = 0; i < this.m_BannerImage.ImageListCount; i++)
			{
				ResBannerImage resImgInfo = this.m_BannerImage.m_ImageInfoList[i].resImgInfo;
				if (resImgInfo.dwBannerType == 11u)
				{
					text = string.Format("{0}{1}", BannerImageSys.GlobalLoadPath, resImgInfo.szImgUrl);
					break;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImageByTag(text, 0, delegate(Texture2D text2D, int imageIndex)
				{
					if (img == null || text2D == null)
					{
						return;
					}
					img.SetSprite(Sprite.Create(text2D, new Rect(0f, 0f, (float)text2D.width, (float)text2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
				}, MonoSingleton<BannerImageSys>.GetInstance().GlobalBannerImagePath, 0));
			}
		}
	}
}
