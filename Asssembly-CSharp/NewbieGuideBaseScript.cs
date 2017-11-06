using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class NewbieGuideBaseScript : MonoBehaviour
{
	public enum EOpacityLadder
	{
		Standard,
		Transparent,
		Darker,
		Count
	}

	public enum eFlipType
	{
		FlipNone,
		FlipX,
		FlipY,
		FlipXY
	}

	private enum ECompleteType
	{
		ClickButton,
		ClickAnyWhere,
		WaitOneWhile
	}

	public delegate void NewbieGuideBaseScriptDelegate();

	public const string HighlighterPath = "UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab";

	public const string HighlightAreaMask = "UGUI/Form/System/Dialog/HighlightAreaMask.prefab";

	private static Vector3 s_FlipNone = Vector3.one;

	private static Vector3 s_FlipX = new Vector3(-1f, 1f, 1f);

	private static Vector3 s_FlipY = new Vector3(1f, -1f, 1f);

	private static Vector3 s_FlipXY = new Vector3(-1f, -1f, 1f);

	private static CUIFormScript ms_originalForm = null;

	protected static List<GameObject> ms_highlitGo = new List<GameObject>();

	protected static List<GameObject> ms_highlighter = new List<GameObject>();

	protected static List<GameObject> ms_originalGo = new List<GameObject>();

	protected static List<GameObject> ms_guideTextList = new List<GameObject>();

	private NewbieGuideBaseScript.ECompleteType m_completeType;

	public event NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate CompleteEvent
	{
		[MethodImpl(32)]
		add
		{
			this.CompleteEvent = (NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate)Delegate.Combine(this.CompleteEvent, value);
		}
		[MethodImpl(32)]
		remove
		{
			this.CompleteEvent = (NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate)Delegate.Remove(this.CompleteEvent, value);
		}
	}

	public event NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate onCompleteAll
	{
		[MethodImpl(32)]
		add
		{
			this.onCompleteAll = (NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate)Delegate.Combine(this.onCompleteAll, value);
		}
		[MethodImpl(32)]
		remove
		{
			this.onCompleteAll = (NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate)Delegate.Remove(this.onCompleteAll, value);
		}
	}

	public NewbieGuideScriptConf currentConf
	{
		get;
		protected set;
	}

	public bool isInitialize
	{
		get;
		protected set;
	}

	public bool isGuiding
	{
		get;
		protected set;
	}

	protected string logTitle
	{
		get
		{
			return "[<color=cyan>新手引导</color>][<color=green>" + this.currentConf.dwID + "</color>]";
		}
	}

	public void SetData(NewbieGuideScriptConf conf)
	{
		this.currentConf = conf;
	}

	private void Awake()
	{
	}

	private void Start()
	{
		this.Initialize();
	}

	protected virtual void Initialize()
	{
		if (!this.isInitialize)
		{
			this.TryToPauseGame();
			this.TryToPlayerSound();
			this.AddDelegate();
			this.isInitialize = true;
			this.isGuiding = true;
		}
	}

	protected virtual void Clear()
	{
		this.isGuiding = false;
		this.TryToResumeGame();
		this.ClearDelegate();
		this.ClearHighLightGameObject();
	}

	private void TryToPlayerSound()
	{
		if (!string.IsNullOrEmpty(this.currentConf.szSoundFileName))
		{
			Singleton<CSoundManager>.GetInstance().PostEvent(this.currentConf.szSoundFileName, null);
		}
	}

	private void TryToPauseGame()
	{
		if (this.currentConf.bStopGame == 1)
		{
			Singleton<CBattleGuideManager>.GetInstance().PauseGame(this, false);
		}
	}

	private void TryToAddClickEvent()
	{
		List<GameObject>.Enumerator enumerator = NewbieGuideBaseScript.ms_highlitGo.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameObject current = enumerator.get_Current();
			if (!(current == null))
			{
				CUIEventScript component = current.GetComponent<CUIEventScript>();
				if (component != null)
				{
					CUIEventScript cUIEventScript = component;
					cUIEventScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Combine(cUIEventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
				}
				CUIMiniEventScript cUIMiniEventScript = current.GetComponent<CUIMiniEventScript>();
				if (cUIMiniEventScript != null)
				{
					CUIMiniEventScript cUIMiniEventScript2 = cUIMiniEventScript;
					cUIMiniEventScript2.onClick = (CUIMiniEventScript.OnUIEventHandler)Delegate.Combine(cUIMiniEventScript2.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
				}
				if (component == null && cUIMiniEventScript == null)
				{
					cUIMiniEventScript = current.AddComponent<CUIMiniEventScript>();
					CUIMiniEventScript cUIMiniEventScript3 = cUIMiniEventScript;
					cUIMiniEventScript3.onClick = (CUIMiniEventScript.OnUIEventHandler)Delegate.Combine(cUIMiniEventScript3.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
				}
			}
		}
		if (this.m_completeType == NewbieGuideBaseScript.ECompleteType.ClickAnyWhere)
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Dialogue_AnyWhereClick, new CUIEventManager.OnUIEventHandler(this.AnyWhereClick));
		}
	}

	private void AddDelegate()
	{
		if (this.IsDelegateClickEvent())
		{
			this.TryToAddClickEvent();
		}
		if (this.IsDelegateSpecialTip())
		{
			this.AddSpecialTip();
		}
		if (!this.IsShowGuideMask())
		{
			this.ShowGuideMask(false);
		}
	}

	private void TryToResumeGame()
	{
		if (this.isInitialize && this.currentConf.bStopGame == 1)
		{
			Singleton<CBattleGuideManager>.GetInstance().ResumeGame(this);
		}
	}

	private void TryToRemoveClickEvent()
	{
		bool flag = false;
		int num = 0;
		List<GameObject>.Enumerator enumerator = NewbieGuideBaseScript.ms_highlitGo.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameObject current = enumerator.get_Current();
			if (!(current == null))
			{
				CUIEventScript component = current.GetComponent<CUIEventScript>();
				if (component != null)
				{
					CUIEventScript cUIEventScript = component;
					cUIEventScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(cUIEventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
					flag = true;
				}
				CUIMiniEventScript component2 = current.GetComponent<CUIMiniEventScript>();
				if (component2 != null)
				{
					CUIMiniEventScript cUIMiniEventScript = component2;
					cUIMiniEventScript.onClick = (CUIMiniEventScript.OnUIEventHandler)Delegate.Remove(cUIMiniEventScript.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
					flag = true;
				}
				Toggle component3 = current.GetComponent<Toggle>();
				if (component3 != null)
				{
					component3.set_isOn(false);
					Toggle component4 = NewbieGuideBaseScript.ms_originalGo.get_Item(num).GetComponent<Toggle>();
					if (component4 != null)
					{
						component4.set_isOn(true);
					}
				}
				if (!flag)
				{
				}
				num++;
			}
		}
		if (this.m_completeType == NewbieGuideBaseScript.ECompleteType.ClickAnyWhere)
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Dialogue_AnyWhereClick, new CUIEventManager.OnUIEventHandler(this.AnyWhereClick));
		}
	}

	private void ClearDelegate()
	{
		if (this.IsDelegateClickEvent())
		{
			this.TryToRemoveClickEvent();
		}
		if (this.IsDelegateSpecialTip())
		{
			this.RemoveSpecialTip();
		}
		if (!this.IsShowGuideMask())
		{
			this.ShowGuideMask(true);
		}
	}

	protected virtual bool IsDelegateClickEvent()
	{
		return false;
	}

	protected virtual bool IsDelegateSpecialTip()
	{
		return true;
	}

	protected virtual bool IsShowGuideMask()
	{
		return true;
	}

	protected virtual bool IsDelegateAutoDispatchCompleteEvent()
	{
		return true;
	}

	public virtual bool IsTimeOutSkip()
	{
		return true;
	}

	protected virtual void CompleteHandler()
	{
		this.Clear();
		if (this.IsDelegateAutoDispatchCompleteEvent())
		{
			this.DispatchCompleteEvent();
		}
	}

	protected virtual void CompleteAllHandler()
	{
		this.Clear();
		if (this.IsDelegateAutoDispatchCompleteEvent())
		{
			this.DispatchCompleteAllEvent();
		}
	}

	protected void DispatchCompleteEvent()
	{
		if (this.CompleteEvent != null)
		{
			this.CompleteEvent();
		}
	}

	protected void DispatchCompleteAllEvent()
	{
		if (this.onCompleteAll != null)
		{
			this.onCompleteAll();
		}
	}

	private void AnyWhereClick(CUIEvent inUiEvent)
	{
		this.ClickHandler(inUiEvent);
	}

	protected virtual void ClickHandler(CUIEvent uiEvent)
	{
		this.CompleteHandler();
	}

	private void AddSpecialTip()
	{
		if (NewbieGuideScriptControl.FormGuideMask != null)
		{
			GameObject gameObject = NewbieGuideScriptControl.FormGuideMask.transform.FindChild("GuideTextStatic").transform.gameObject;
			gameObject.CustomSetActive(false);
		}
		if (this.currentConf.wSpecialTip != 0)
		{
			if (NewbieGuideScriptControl.FormGuideMask == null)
			{
				NewbieGuideScriptControl.OpenGuideForm();
			}
			GameObject gameObject2 = NewbieGuideScriptControl.FormGuideMask.transform.FindChild("GuideTextStatic").transform.gameObject;
			gameObject2.CustomSetActive(false);
			NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig((uint)this.currentConf.wSpecialTip);
			if (specialTipConfig != null && specialTipConfig.bGuideTextPos == 0)
			{
				gameObject2.CustomSetActive(true);
				GameObject gameObject3 = gameObject2.transform.FindChild("RightSpecial/Text").transform.gameObject;
				Text component = gameObject3.GetComponent<Text>();
				component.set_text(StringHelper.UTF8BytesToString(ref specialTipConfig.szTipText));
			}
		}
	}

	private void RemoveSpecialTip()
	{
		if (this.currentConf.wSpecialTip != 0)
		{
		}
	}

	private void ShowGuideMask(bool bShow)
	{
		if (bShow)
		{
			NewbieGuideScriptControl.OpenGuideForm();
		}
		else
		{
			NewbieGuideScriptControl.CloseGuideForm();
		}
	}

	protected void SetBackgroundTransparency(NewbieGuideBaseScript.EOpacityLadder inLadder)
	{
		if (NewbieGuideScriptControl.FormGuideMask != null && NewbieGuideScriptControl.FormGuideMask.transform != null)
		{
			float a = 0f;
			switch (inLadder)
			{
			case NewbieGuideBaseScript.EOpacityLadder.Standard:
				a = 0.4f;
				break;
			case NewbieGuideBaseScript.EOpacityLadder.Transparent:
				a = 0f;
				break;
			case NewbieGuideBaseScript.EOpacityLadder.Darker:
				a = 0.7f;
				break;
			}
			Transform transform = NewbieGuideScriptControl.FormGuideMask.transform.FindChild("Bg");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				Image component = gameObject.GetComponent<Image>();
				if (component != null)
				{
					component.set_color(new Color(0f, 0f, 0f, a));
				}
			}
		}
	}

	private void OpenGuideForm()
	{
		this.SetBackgroundTransparency((NewbieGuideBaseScript.EOpacityLadder)this.currentConf.bShowTransparency);
	}

	private bool DoesShowArrow()
	{
		return this.currentConf.bNotShowArrow == 0;
	}

	private void PreHighlight()
	{
		this.m_completeType = NewbieGuideBaseScript.ECompleteType.ClickButton;
		NewbieGuideBaseScript.ms_originalGo.Clear();
		NewbieGuideBaseScript.ms_originalForm = null;
		this.ClearHighlitObjs();
		if (NewbieGuideScriptControl.FormGuideMask != null && NewbieGuideScriptControl.FormGuideMask.gameObject != null)
		{
			Transform transform = NewbieGuideScriptControl.FormGuideMask.gameObject.transform.Find("GuideText");
			if (transform != null && transform.gameObject != null)
			{
				GameObject gameObject = transform.gameObject;
				gameObject.CustomSetActive(false);
			}
		}
	}

	private void AddHighlightInternal(GameObject baseGo, CUIFormScript inOriginalForm, bool cloneEvent, bool bShowFinger)
	{
		this.PreHighlight();
		if (baseGo != null)
		{
			NewbieGuideBaseScript.ms_originalGo.Add(baseGo);
		}
		NewbieGuideBaseScript.ms_originalForm = inOriginalForm;
		this.OpenGuideForm();
		if (NewbieGuideScriptControl.FormGuideMask == null)
		{
			NewbieGuideScriptControl.OpenGuideForm();
		}
		List<GameObject>.Enumerator enumerator = NewbieGuideBaseScript.ms_originalGo.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			GameObject current = enumerator.get_Current();
			if (!(current == null))
			{
				GameObject gameObject = Object.Instantiate(current) as GameObject;
				if (!(gameObject == null))
				{
					RectTransform rectTransform = gameObject.transform as RectTransform;
					rectTransform.SetParent(NewbieGuideScriptControl.FormGuideMask.transform);
					rectTransform.SetSiblingIndex(1);
					rectTransform.localScale = current.transform.localScale;
					RectTransform rectTransform2 = current.transform as RectTransform;
					rectTransform.pivot = rectTransform2.pivot;
					rectTransform.sizeDelta = rectTransform2.sizeDelta;
					LayoutElement component = current.GetComponent<LayoutElement>();
					if (component != null && rectTransform.sizeDelta == Vector2.zero)
					{
						rectTransform.sizeDelta = new Vector2(component.get_preferredWidth(), component.get_preferredHeight());
					}
					rectTransform.position = current.transform.position;
					Vector2 screenPoint = CUIUtility.WorldToScreenPoint(inOriginalForm.GetCamera(), current.transform.position);
					Vector3 worldPosition = CUIUtility.ScreenToWorldPoint(NewbieGuideScriptControl.FormGuideMask.GetCamera(), screenPoint, rectTransform.position.z);
					NewbieGuideScriptControl.FormGuideMask.InitializeWidgetPosition(gameObject, worldPosition);
					gameObject.CustomSetActive(false);
					if (cloneEvent)
					{
						CUIEventScript component2 = current.GetComponent<CUIEventScript>();
						CUIEventScript component3 = gameObject.GetComponent<CUIEventScript>();
						if (component2 && component3)
						{
							component3.m_onDownEventParams = component2.m_onDownEventParams;
							component3.m_onUpEventParams = component2.m_onUpEventParams;
							component3.m_onClickEventParams = component2.m_onClickEventParams;
							component3.m_onHoldStartEventParams = component2.m_onHoldStartEventParams;
							component3.m_onHoldEventParams = component2.m_onHoldEventParams;
							component3.m_onHoldEndEventParams = component2.m_onHoldEndEventParams;
							component3.m_onDragStartEventParams = component2.m_onDragStartEventParams;
							component3.m_onDragEventParams = component2.m_onDragEventParams;
							component3.m_onDragEndEventParams = component2.m_onDragEndEventParams;
							component3.m_onDropEventParams = component2.m_onDropEventParams;
							component3.m_closeFormWhenClicked = component2.m_closeFormWhenClicked;
							component3.m_belongedFormScript = component2.m_belongedFormScript;
							component3.m_belongedListScript = component2.m_belongedListScript;
							component3.m_indexInlist = component2.m_indexInlist;
						}
						CUIMiniEventScript component4 = current.GetComponent<CUIMiniEventScript>();
						CUIMiniEventScript component5 = gameObject.GetComponent<CUIMiniEventScript>();
						if (component4 && component5)
						{
							component5.m_onDownEventParams = component4.m_onDownEventParams;
							component5.m_onUpEventParams = component4.m_onUpEventParams;
							component5.m_onClickEventParams = component4.m_onClickEventParams;
							component5.m_closeFormWhenClicked = component4.m_closeFormWhenClicked;
							component5.m_belongedFormScript = component4.m_belongedFormScript;
							component5.m_belongedListScript = component4.m_belongedListScript;
							component5.m_indexInlist = component4.m_indexInlist;
						}
					}
					else
					{
						CUIEventScript component6 = gameObject.GetComponent<CUIEventScript>();
						if (component6)
						{
							component6.enabled = false;
						}
						CUIMiniEventScript component7 = gameObject.GetComponent<CUIMiniEventScript>();
						if (component7)
						{
							component7.enabled = false;
						}
					}
					gameObject.CustomSetActive(true);
					CUIAnimatorScript component8 = current.GetComponent<CUIAnimatorScript>();
					if (component8 != null)
					{
						CUICommonSystem.PlayAnimator(gameObject, component8.m_currentAnimatorStateName);
					}
					NewbieGuideBaseScript.ms_highlitGo.Add(gameObject);
					if (bShowFinger)
					{
						GameObject gameObject2 = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab", typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
						if (gameObject2 != null)
						{
							GameObject gameObject3 = Object.Instantiate(gameObject2) as GameObject;
							if (gameObject3 != null)
							{
								gameObject3.transform.SetParent(gameObject.transform);
								Transform transform = gameObject3.transform;
								switch (this.currentConf.wFlipType)
								{
								case 0:
									transform.localScale = NewbieGuideBaseScript.s_FlipNone;
									break;
								case 1:
									transform.localScale = NewbieGuideBaseScript.s_FlipX;
									break;
								case 2:
									transform.localScale = NewbieGuideBaseScript.s_FlipY;
									break;
								case 3:
									transform.localScale = NewbieGuideBaseScript.s_FlipXY;
									break;
								}
								gameObject3.transform.position = gameObject.transform.position;
								(gameObject3.transform as RectTransform).anchoredPosition = new Vector2((float)this.currentConf.iOffsetHighLightX, (float)this.currentConf.iOffsetHighLightY);
								if (!this.DoesShowArrow())
								{
									gameObject3.transform.FindChild("Panel/ImageFinger").gameObject.CustomSetActive(false);
								}
								NewbieGuideBaseScript.ms_highlighter.Add(gameObject3);
							}
						}
					}
					if (num == 0 && this.currentConf.wSpecialTip != 0)
					{
						NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig((uint)this.currentConf.wSpecialTip);
						if (specialTipConfig != null && specialTipConfig.bGuideTextPos > 0)
						{
							GameObject gameObject4 = NewbieGuideBaseScript.InstantiateGuideText(specialTipConfig, gameObject, NewbieGuideScriptControl.FormGuideMask, inOriginalForm);
							if (gameObject4 != null)
							{
								NewbieGuideBaseScript.ms_guideTextList.Add(gameObject4);
								gameObject4.CustomSetActive(false);
							}
						}
					}
					num++;
				}
			}
		}
	}

	protected void AddHighLightGameObject(GameObject baseGo, bool isUI, CUIFormScript inOriginalForm, bool cloneEvent = true)
	{
		this.AddHighlightInternal(baseGo, inOriginalForm, cloneEvent, true);
	}

	protected void AddHighLightAreaClickAnyWhere(GameObject baseGo, CUIFormScript inOriginalForm)
	{
		this.AddHighlightInternal(baseGo, inOriginalForm, false, false);
		List<GameObject>.Enumerator enumerator = NewbieGuideBaseScript.ms_highlitGo.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameObject current = enumerator.get_Current();
			if (!(current == null))
			{
				RectTransform rectTransform = current.transform as RectTransform;
				GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Form/System/Dialog/HighlightAreaMask.prefab", typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
				if (gameObject != null)
				{
					GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
					if (gameObject2 != null)
					{
						RectTransform rectTransform2 = gameObject2.transform as RectTransform;
						rectTransform2.SetParent(rectTransform);
						rectTransform2.SetAsLastSibling();
						rectTransform2.localScale = NewbieGuideBaseScript.s_FlipNone;
						Transform transform = baseGo.transform;
						if (transform.parent.name == "ScrollRect")
						{
							Rect rect = (transform.parent.transform as RectTransform).rect;
							Vector2 sizeDelta = new Vector2(rect.width, rect.height);
							rectTransform2.sizeDelta = sizeDelta;
						}
						else
						{
							rectTransform2.sizeDelta = rectTransform.sizeDelta;
						}
						rectTransform2.position = rectTransform.position;
						rectTransform2.anchoredPosition = Vector2.zero;
					}
				}
			}
		}
		this.m_completeType = NewbieGuideBaseScript.ECompleteType.ClickAnyWhere;
	}

	protected void AddHighLightAnyWhere()
	{
		this.PreHighlight();
		this.m_completeType = NewbieGuideBaseScript.ECompleteType.ClickAnyWhere;
		this.OpenGuideForm();
	}

	protected void AddHighlightWaiting()
	{
		this.PreHighlight();
		this.m_completeType = NewbieGuideBaseScript.ECompleteType.WaitOneWhile;
		this.OpenGuideForm();
	}

	private void ClearHighLightGameObject()
	{
		this.ClearHighlightInternal();
	}

	private void ClearHighlightInternal()
	{
		this.SetBackgroundTransparency(NewbieGuideBaseScript.EOpacityLadder.Transparent);
		this.ClearHighlitObjs();
		NewbieGuideBaseScript.ms_originalGo.Clear();
		NewbieGuideBaseScript.ms_originalForm = null;
		this.m_completeType = NewbieGuideBaseScript.ECompleteType.ClickButton;
	}

	private void ClearHighlitObjs()
	{
		List<GameObject> list = new List<GameObject>();
		list.AddRange(NewbieGuideBaseScript.ms_highlitGo);
		list.AddRange(NewbieGuideBaseScript.ms_highlighter);
		List<GameObject>.Enumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameObject current = enumerator.get_Current();
			if (!(current == null))
			{
				CUICommonSystem.DestoryObj(current, 0.1f);
			}
		}
		NewbieGuideBaseScript.ms_highlitGo.Clear();
		NewbieGuideBaseScript.ms_highlighter.Clear();
		int count = NewbieGuideBaseScript.ms_guideTextList.get_Count();
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = NewbieGuideBaseScript.ms_guideTextList.get_Item(i);
			if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
		}
		NewbieGuideBaseScript.ms_guideTextList.Clear();
	}

	protected void SetHighlighterAndTextShow(bool bShow)
	{
		if (bShow)
		{
			this.SetBackgroundTransparency((NewbieGuideBaseScript.EOpacityLadder)this.currentConf.bShowTransparency);
		}
		else
		{
			this.SetBackgroundTransparency(NewbieGuideBaseScript.EOpacityLadder.Transparent);
		}
		List<GameObject> list = new List<GameObject>();
		list.AddRange(NewbieGuideBaseScript.ms_highlighter);
		List<GameObject>.Enumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameObject current = enumerator.get_Current();
			if (!(current == null))
			{
				current.CustomSetActive(bShow);
			}
		}
		int count = NewbieGuideBaseScript.ms_guideTextList.get_Count();
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = NewbieGuideBaseScript.ms_guideTextList.get_Item(i);
			if (gameObject != null)
			{
				gameObject.CustomSetActive(bShow);
			}
		}
	}

	protected virtual void Update()
	{
		CUIFormScript formGuideMask = NewbieGuideScriptControl.FormGuideMask;
		if (NewbieGuideBaseScript.ms_originalForm != null && formGuideMask != null)
		{
			int count = NewbieGuideBaseScript.ms_highlitGo.get_Count();
			DebugHelper.Assert(count <= NewbieGuideBaseScript.ms_originalGo.get_Count());
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = NewbieGuideBaseScript.ms_highlitGo.get_Item(i);
				GameObject gameObject2 = NewbieGuideBaseScript.ms_originalGo.get_Item(i);
				GameObject gameObject3 = null;
				if (NewbieGuideBaseScript.ms_highlighter.get_Count() > i)
				{
					gameObject3 = NewbieGuideBaseScript.ms_highlighter.get_Item(i);
				}
				if (!(gameObject == null) && !(gameObject2 == null))
				{
					gameObject.CustomSetActive(gameObject2.activeSelf);
					Image component = gameObject2.GetComponent<Image>();
					Image component2 = gameObject.GetComponent<Image>();
					if (component != null && component2 != null)
					{
						component2.set_color(component.get_color());
					}
					RectTransform rectTransform = gameObject.transform as RectTransform;
					RectTransform rectTransform2 = gameObject2.transform as RectTransform;
					rectTransform.localScale = rectTransform2.localScale;
					rectTransform.pivot = rectTransform2.pivot;
					rectTransform.sizeDelta = rectTransform2.sizeDelta;
					LayoutElement component3 = rectTransform2.GetComponent<LayoutElement>();
					if (component3 != null && rectTransform.sizeDelta == Vector2.zero)
					{
						rectTransform.sizeDelta = new Vector2(component3.get_preferredWidth(), component3.get_preferredHeight());
					}
					rectTransform.position = rectTransform2.position;
					Vector2 screenPoint = CUIUtility.WorldToScreenPoint(NewbieGuideBaseScript.ms_originalForm.GetCamera(), rectTransform2.position);
					Vector3 vector = CUIUtility.ScreenToWorldPoint(NewbieGuideScriptControl.FormGuideMask.GetCamera(), screenPoint, rectTransform.position.z);
					if (i < NewbieGuideBaseScript.ms_guideTextList.get_Count())
					{
						NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig((uint)this.currentConf.wSpecialTip);
						if (specialTipConfig != null && specialTipConfig.bGuideTextPos > 0)
						{
							if (gameObject3 != null)
							{
								NewbieGuideBaseScript.ms_guideTextList.get_Item(i).CustomSetActive(gameObject3.activeSelf);
							}
							NewbieGuideBaseScript.UpdateGuideTextPos(specialTipConfig, gameObject2, formGuideMask, NewbieGuideBaseScript.ms_originalForm, NewbieGuideBaseScript.ms_guideTextList.get_Item(i));
						}
					}
				}
			}
		}
	}

	public static GameObject InstantiateGuideText(NewbieGuideSpecialTipConf tipConf, GameObject inParentObj, CUIFormScript inGuideForm, CUIFormScript inOriginalForm)
	{
		if (tipConf == null || inParentObj == null || inGuideForm == null || inOriginalForm == null)
		{
			return null;
		}
		GameObject gameObject = inGuideForm.gameObject.transform.Find("GuideText").gameObject;
		if (gameObject != null)
		{
			Transform transform = gameObject.transform.FindChild("RightSpecial/Text").transform;
			if (transform != null)
			{
				GameObject gameObject2 = transform.gameObject;
				Text component = gameObject2.GetComponent<Text>();
				if (component != null)
				{
					component.set_text(StringHelper.UTF8BytesToString(ref tipConf.szTipText));
				}
			}
			gameObject.CustomSetActive(true);
		}
		return gameObject;
	}

	public static void UpdateGuideTextPos(NewbieGuideSpecialTipConf tipConf, GameObject inParentObj, CUIFormScript inGuideForm, CUIFormScript inOriginalForm, GameObject rootPanel)
	{
		if (tipConf == null || inParentObj == null || inGuideForm == null || inOriginalForm == null)
		{
			return;
		}
		RectTransform rectTransform = (RectTransform)rootPanel.transform.FindChild("RightSpecial").transform;
		RectTransform rectTransform2 = (RectTransform)rootPanel.transform;
		Vector3 position = inParentObj.transform.position;
		Vector2 vector = CUIUtility.WorldToScreenPoint(inOriginalForm.GetCamera(), position);
		float num = vector.x;
		float num2 = vector.y;
		float num3 = 142f;
		float num4 = 85f;
		Vector2 vector2 = new Vector2(0f, 0f);
		switch (tipConf.bSpecialTipPos)
		{
		case 0:
			vector2 = new Vector2(-num3, num4);
			break;
		case 1:
			vector2 = new Vector2(-num3, -num4);
			break;
		case 2:
			vector2 = new Vector2(num3, num4);
			break;
		case 3:
			vector2 = new Vector2(num3, -num4);
			break;
		}
		if (tipConf.iOffsetX != 0)
		{
			vector2.x += (float)tipConf.iOffsetX;
		}
		if (tipConf.iOffsetY != 0)
		{
			vector2.y += (float)tipConf.iOffsetY;
		}
		vector2.x = inGuideForm.ChangeFormValueToScreen(vector2.x);
		vector2.y = inGuideForm.ChangeFormValueToScreen(vector2.y);
		rectTransform2.sizeDelta = rectTransform.sizeDelta;
		float num5 = rectTransform2.rect.width / 2f;
		num5 = inGuideForm.ChangeFormValueToScreen(num5);
		num5 += 3f;
		float num6 = rectTransform2.rect.height / 2f;
		num6 = inGuideForm.ChangeFormValueToScreen(num6);
		num6 += 3f;
		num += vector2.x;
		num2 += vector2.y;
		if (num < num5)
		{
			num = num5;
		}
		else if (num + num5 > (float)Screen.width)
		{
			num = (float)Screen.width - num5;
		}
		if (num2 < num6)
		{
			num2 = num6;
		}
		else if (num2 + num6 > (float)Screen.height)
		{
			num2 = (float)Screen.height - num6;
		}
		num = inGuideForm.ChangeScreenValueToForm(num);
		num2 = inGuideForm.ChangeScreenValueToForm(num2);
		rectTransform2.anchoredPosition = new Vector2(num, num2);
	}

	public void Stop()
	{
		this.Clear();
	}
}
