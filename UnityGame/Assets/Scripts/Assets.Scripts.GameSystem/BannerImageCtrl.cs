using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class BannerImageCtrl : MonoBehaviour
	{
		private bool m_bUpdateStart;

		private BannerImageSys.BannerImage m_BannerImage;

		private bool m_bStopAutoMove;

		public float m_fSpeed = 2f;

		private Vector2 m_lastPos;

		private int m_CurIdxImagePage;

		private int[] m_PickIdxList = new int[3];

		public CUIStepListScript m_UIListScript;

		public GameObject m_PickObject;

		public BannerImageSys.BannerType[] m_UseTypeList;

		private float m_fBeginTime;

		private int m_fAdd = 1;

		public BannerImageSys.BannerPosition m_DisplayPosition;

		private BannerImageSys.BannerImageInfo[] m_AllLoadImageInfo;

		private void Update()
		{
			if (this.m_bUpdateStart && !this.m_bStopAutoMove && Time.time - this.m_fBeginTime >= this.m_fSpeed)
			{
				this.AutoMoveBannerImage();
				this.m_fBeginTime = Time.time;
			}
		}

		private void StopAutoMove()
		{
			this.m_bStopAutoMove = true;
		}

		private void InitPickObjElement(int nImageCount)
		{
			GameObject pickObject = this.m_PickObject;
			if (pickObject != null)
			{
				CUIContainerScript component = pickObject.GetComponent<CUIContainerScript>();
				if (component != null)
				{
					component.RecycleAllElement();
					for (int i = 0; i < nImageCount; i++)
					{
						this.m_PickIdxList[i] = component.GetElement();
					}
				}
			}
		}

		private void EnablePickObj(int idx)
		{
			GameObject pickObject = this.m_PickObject;
			if (pickObject != null)
			{
				CUIContainerScript component = pickObject.GetComponent<CUIContainerScript>();
				if (component)
				{
					for (int i = 0; i < this.m_PickIdxList.Length; i++)
					{
						if (i == idx)
						{
							GameObject element = component.GetElement(this.m_PickIdxList[i]);
							if (element)
							{
								Transform transform = element.transform.FindChild("Image_Pointer");
								if (transform != null)
								{
									transform.gameObject.CustomSetActive(true);
								}
							}
						}
						else
						{
							GameObject element2 = component.GetElement(this.m_PickIdxList[i]);
							if (element2)
							{
								Transform transform2 = element2.transform.FindChild("Image_Pointer");
								if (transform2 != null)
								{
									transform2.gameObject.CustomSetActive(false);
								}
							}
						}
					}
				}
			}
		}

		private void OnHoldStart_Item(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetBelongedListScript != this.m_UIListScript)
			{
				return;
			}
			this.m_lastPos = uiEvent.m_pointerEventData.get_position();
			this.m_bStopAutoMove = true;
		}

		private void OnHoldEnd_Item(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetBelongedListScript != this.m_UIListScript)
			{
				return;
			}
			if (uiEvent.m_pointerEventData.get_position().x <= this.m_lastPos.x)
			{
				this.m_fAdd = 1;
			}
			else
			{
				this.m_fAdd = -1;
			}
			this.AutoMoveBannerImage();
			this.m_bStopAutoMove = false;
			this.m_fBeginTime = Time.time;
		}

		private void BannerImage_OnClickItem(CUIEvent uiEvent)
		{
			if (this.m_BannerImage == null || uiEvent.m_srcWidgetBelongedListScript != this.m_UIListScript)
			{
				return;
			}
			if (this.m_AllLoadImageInfo == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			int num = this.m_AllLoadImageInfo.Length;
			if (srcWidgetIndexInBelongedList >= num)
			{
				return;
			}
			BannerImageSys.BannerImageInfo bannerImageInfo = this.m_AllLoadImageInfo[srcWidgetIndexInBelongedList];
			if (bannerImageInfo != null)
			{
				if (bannerImageInfo.resImgInfo.dwBannerType == 1u)
				{
					CUICommonSystem.OpenUrl(bannerImageInfo.resImgInfo.szHttpUrl, true, 0);
				}
				else if (bannerImageInfo.resImgInfo.dwBannerType == 2u)
				{
					CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)bannerImageInfo.resImgInfo.dwJumpEntrance, bannerImageInfo.resImgInfo.iTargetID, bannerImageInfo.resImgInfo.iTargetID2, null);
				}
			}
		}

		private void LoadBannerImage()
		{
			if (this.m_AllLoadImageInfo == null)
			{
				return;
			}
			int num = this.m_AllLoadImageInfo.Length;
			for (int i = 0; i < num; i++)
			{
				string text = this.m_AllLoadImageInfo[i].resImgInfo.szImgUrl;
				text = string.Format("{0}{1}", BannerImageSys.GlobalLoadPath, text);
				base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImageByTag(text, i, delegate(Texture2D text2, int imageIDX)
				{
					this.SetElemntTexture(imageIDX, text2);
				}, MonoSingleton<BannerImageSys>.GetInstance().GlobalBannerImagePath, 0));
			}
		}

		private void SetElemntTexture(int idx, Texture2D texture2D)
		{
			if (this.m_UIListScript)
			{
				CUIStepListScript uIListScript = this.m_UIListScript;
				CUIListElementScript elemenet = uIListScript.GetElemenet(idx);
				if (elemenet)
				{
					Image component = elemenet.transform.Find("Img").GetComponent<Image>();
					component.SetSprite(Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
				}
			}
		}

		private void AutoMoveBannerImage()
		{
			if (this.m_UIListScript)
			{
				CUIListScript uIListScript = this.m_UIListScript;
				if (uIListScript)
				{
					int elementAmount = uIListScript.GetElementAmount();
					if (this.m_CurIdxImagePage < 0)
					{
						this.m_CurIdxImagePage = 0;
						this.m_fAdd = 1;
					}
					this.m_CurIdxImagePage += this.m_fAdd;
					if (this.m_CurIdxImagePage >= elementAmount)
					{
						this.m_CurIdxImagePage = elementAmount - 1;
					}
					if (this.m_CurIdxImagePage == elementAmount - 1)
					{
						this.m_fAdd = -1;
					}
					if (this.m_CurIdxImagePage < 0)
					{
						this.m_CurIdxImagePage = 0;
					}
					if (this.m_CurIdxImagePage == 0)
					{
						this.m_fAdd = 1;
					}
				}
				uIListScript.MoveElementInScrollArea(this.m_CurIdxImagePage, false);
				this.EnablePickObj(this.m_CurIdxImagePage);
			}
		}

		private void OnDestroy()
		{
			this.m_BannerImage = null;
			this.m_AllLoadImageInfo = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BannerImage_HoldStart, new CUIEventManager.OnUIEventHandler(this.OnHoldStart_Item));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BannerImage_HoldEnd, new CUIEventManager.OnUIEventHandler(this.OnHoldEnd_Item));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BannerImage_ClickItem, new CUIEventManager.OnUIEventHandler(this.BannerImage_OnClickItem));
		}

		private bool checkImageType(BannerImageSys.BannerType kType)
		{
			int num = this.m_UseTypeList.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.m_UseTypeList[i] == kType)
				{
					return true;
				}
			}
			return false;
		}

		public bool InitSys()
		{
			this.m_BannerImage = MonoSingleton<BannerImageSys>.GetInstance().GetCurBannerImage();
			if (this.m_BannerImage == null)
			{
				return false;
			}
			int num = 0;
			ListView<BannerImageSys.BannerImageInfo> listView = new ListView<BannerImageSys.BannerImageInfo>();
			for (int i = 0; i < this.m_BannerImage.ImageListCount; i++)
			{
				BannerImageSys.BannerImageInfo bannerImageInfo = this.m_BannerImage.m_ImageInfoList[i];
				if (bannerImageInfo != null && bannerImageInfo.imgLoadSucc && this.checkImageType((BannerImageSys.BannerType)bannerImageInfo.resImgInfo.dwBannerType) && bannerImageInfo.resImgInfo.dwLocation == (uint)this.m_DisplayPosition && bannerImageInfo.IsShowNow())
				{
					listView.Add(bannerImageInfo);
					num++;
				}
			}
			if (num > 0)
			{
				if (listView != null)
				{
					this.m_AllLoadImageInfo = new BannerImageSys.BannerImageInfo[listView.Count];
					for (int j = 0; j < listView.Count; j++)
					{
						this.m_AllLoadImageInfo[j] = listView[j];
					}
				}
				Array.Sort<BannerImageSys.BannerImageInfo>(this.m_AllLoadImageInfo, new Comparison<BannerImageSys.BannerImageInfo>(BannerImageSys.ComparebyShowIdx));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BannerImage_HoldStart, new CUIEventManager.OnUIEventHandler(this.OnHoldStart_Item));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BannerImage_HoldEnd, new CUIEventManager.OnUIEventHandler(this.OnHoldEnd_Item));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BannerImage_ClickItem, new CUIEventManager.OnUIEventHandler(this.BannerImage_OnClickItem));
				CUIStepListScript uIListScript = this.m_UIListScript;
				uIListScript.SetDontUpdate(true);
				uIListScript.SetElementAmount(num);
				this.m_PickIdxList = new int[num];
				this.InitPickObjElement(num);
				this.EnablePickObj(0);
				this.LoadBannerImage();
				this.m_bUpdateStart = true;
				this.m_fBeginTime = Time.time;
				return true;
			}
			Debug.Log("not valide bannerImage");
			return false;
		}
	}
}
