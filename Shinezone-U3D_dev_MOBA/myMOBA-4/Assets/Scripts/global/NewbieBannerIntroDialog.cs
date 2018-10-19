using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NewbieBannerIntroDialog
{
	public enum enIntroDlgWidget
	{
		enTitleTxt,
		enBottomBtn,
		enStepList,
		enPickObject,
		enBtnLeft,
		enBtnRight,
		enTimer,
		enChooseNotShowToggle
	}

	private GameObject m_BottomBtn;

	private CUIStepListScript m_stepList;

	private CUIFormScript m_form;

	private CUITimerScript m_timer;

	private GameObject m_PickObject;

	private GameObject m_BtnLeft;

	private GameObject m_BtnRight;

	private string[] m_imgPath;

	private string m_title;

	private string m_btnName;

	private int[] m_PickIdxList;

	private int m_totalImgNum;

	private int m_curImgIndex;

	private CUIEvent m_uiEventParam;

	private Vector2 m_lastPos = Vector2.zero;

	private bool m_bAutoMove;

	private bool m_bShowChooseGuideNextTime;

	public static string BANNER_INTRO_DIALOG_PATH = "UGUI/Form/System/Newbie/Form_BannerIntroDialog";

	public void OpenForm(int clientBit, string[] imgPath, int imgNum, CUIEvent uiEventParam = null, string title = null, string btnName = null, bool bAutoMove = true, bool bShowChooseGuideNextTime = false)
	{
		this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(NewbieBannerIntroDialog.BANNER_INTRO_DIALOG_PATH, false, true);
		if (this.m_form == null)
		{
			return;
		}
		CUIContainerScript component = this.m_form.GetWidget(2).transform.FindChild("pickObj").GetComponent<CUIContainerScript>();
		this.m_totalImgNum = Math.Min(Math.Min(imgPath.Length, imgNum), component.m_prepareElementAmount);
		this.m_imgPath = imgPath;
		this.m_uiEventParam = uiEventParam;
		this.m_title = title;
		this.m_btnName = btnName;
		this.m_PickIdxList = new int[this.m_totalImgNum];
		this.m_bAutoMove = bAutoMove;
		this.m_bShowChooseGuideNextTime = bShowChooseGuideNextTime;
		this.m_curImgIndex = 0;
		this.m_form.GetWidget(1).GetComponent<CUIEventScript>().m_onClickEventParams.tag = clientBit;
		this.InitForm();
		this.InitPickObjElement(this.m_totalImgNum);
		this.RefreshUI(0);
	}

	public void Confirm()
	{
		if (this.m_uiEventParam != null)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(this.m_uiEventParam);
			this.m_uiEventParam = null;
		}
	}

	public void Clear()
	{
		this.m_imgPath = null;
		this.m_title = null;
		this.m_btnName = null;
		this.m_PickIdxList = null;
		this.m_BottomBtn = null;
		this.m_stepList = null;
		this.m_form = null;
		this.m_timer = null;
		this.m_PickObject = null;
		this.m_BtnLeft = null;
		this.m_BtnRight = null;
	}

	public void DragStart(Vector2 pos)
	{
		this.m_lastPos = pos;
		if (this.m_timer)
		{
			this.m_timer.ResetTime();
		}
	}

	public void DragEnd(Vector2 pos)
	{
		Vector2 vector = pos;
		if (vector.x > this.m_lastPos.x)
		{
			this.MoveToPrePage();
		}
		else if (vector.x < this.m_lastPos.x)
		{
			this.MoveToNextPage();
		}
	}

	public void MoveToPrePage()
	{
		if (this.m_curImgIndex > 0)
		{
			this.m_curImgIndex--;
			if (this.m_stepList)
			{
				this.m_stepList.MoveElementInScrollArea(this.m_curImgIndex, false);
			}
			this.RefreshUI(this.m_curImgIndex);
		}
		if (this.m_timer)
		{
			this.m_timer.ReStartTimer();
		}
	}

	public void MoveToNextPage()
	{
		if (this.m_curImgIndex < this.m_totalImgNum - 1)
		{
			this.m_curImgIndex++;
			if (this.m_stepList)
			{
				this.m_stepList.MoveElementInScrollArea(this.m_curImgIndex, false);
			}
			this.RefreshUI(this.m_curImgIndex);
		}
		if (this.m_timer)
		{
			this.m_timer.ReStartTimer();
		}
	}

	public void AutoMove()
	{
		if (this.m_bAutoMove)
		{
			this.MoveToNextPage();
		}
	}

	public bool CanSetClientBit()
	{
		return !this.m_bShowChooseGuideNextTime || this.m_form.GetWidget(7).GetComponent<Toggle>().isOn;
	}

	private void InitForm()
	{
		if (this.m_form == null)
		{
			return;
		}
		Text component = this.m_form.GetWidget(0).GetComponent<Text>();
		Text component2 = this.m_form.GetWidget(1).transform.FindChild("Text").GetComponent<Text>();
		this.m_stepList = this.m_form.GetWidget(2).GetComponent<CUIStepListScript>();
		this.m_BottomBtn = this.m_form.GetWidget(1).gameObject;
		this.m_PickObject = this.m_form.GetWidget(3).gameObject;
		this.m_BtnLeft = this.m_form.GetWidget(4).gameObject;
		this.m_BtnRight = this.m_form.GetWidget(5).gameObject;
		this.m_timer = this.m_form.GetWidget(6).GetComponent<CUITimerScript>();
		if (this.m_title != null)
		{
			component.text = this.m_title;
		}
		if (this.m_btnName != null)
		{
			component2.text = this.m_btnName;
		}
		this.m_stepList.SetElementAmount(this.m_totalImgNum);
		for (int i = 0; i < this.m_totalImgNum; i++)
		{
			if (this.m_stepList.GetElemenet(i) != null)
			{
				Image component3 = this.m_stepList.GetElemenet(i).transform.FindChild("Image").GetComponent<Image>();
				CBinaryObject cBinaryObject = Singleton<CResourceManager>.GetInstance().GetResource(this.m_imgPath[i] + ".bytes", typeof(TextAsset), enResourceType.UISprite, false, false).m_content as CBinaryObject;
				if (cBinaryObject != null)
				{
					byte[] data = cBinaryObject.m_data;
					Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGB24, false);
					texture2D.LoadImage(data);
					Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
					component3.sprite = sprite;
				}
				else
				{
					component3.SetSprite(this.m_imgPath[i], this.m_form, true, false, false, false);
				}
			}
		}
		this.m_stepList.SetDontUpdate(true);
	}

	private void RefreshUI(int idx)
	{
		if (this.m_form == null)
		{
			return;
		}
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
		bool bActive = idx == this.m_totalImgNum - 1 || this.m_totalImgNum == 0;
		this.m_BottomBtn.CustomSetActive(bActive);
		if (this.m_bShowChooseGuideNextTime)
		{
			this.m_form.GetWidget(7).CustomSetActive(bActive);
		}
		else
		{
			this.m_form.GetWidget(7).CustomSetActive(false);
		}
		this.m_PickObject.CustomSetActive(idx != this.m_totalImgNum - 1);
		this.m_BtnLeft.CustomSetActive(idx != 0 && this.m_totalImgNum > 1);
		this.m_BtnRight.CustomSetActive(idx != this.m_totalImgNum - 1 && this.m_totalImgNum > 1);
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
}
