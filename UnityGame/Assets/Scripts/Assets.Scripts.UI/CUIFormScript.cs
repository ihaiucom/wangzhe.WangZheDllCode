using Assets.Scripts.Framework;
using Assets.Scripts.Sound;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	[ExecuteInEditMode]
	public class CUIFormScript : MonoBehaviour, IComparable
	{
		private struct stInitWidgetPosition
		{
			public int m_renderFrameStamp;

			public GameObject m_widget;

			public Vector3 m_worldPosition;
		}

		private class CASyncLoadedImage
		{
			public Image m_image;

			public string m_prefabPath;

			public bool m_needCached;

			public bool m_unloadBelongedAssetBundleAfterLoaded;

			public bool m_isShowSpecMatrial;

			public CASyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
			{
				this.m_image = image;
				this.m_prefabPath = prefabPath;
				this.m_needCached = needCached;
				this.m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
				this.m_isShowSpecMatrial = isShowSpecMatrial;
			}
		}

		private const int c_openOrderMask = 10;

		private const int c_priorityOrderMask = 1000;

		private const int c_overlayOrderMask = 10000;

		public Vector2 m_referenceResolution = new Vector2(960f, 640f);

		public bool m_isSingleton;

		public bool m_isModal;

		public enFormPriority m_priority;

		private enFormPriority m_defaultPriority;

		public int m_group;

		public bool m_fullScreenBG;

		public bool m_disableInput;

		[HideInInspector]
		public enUIEventID[] m_eventIDs = new enUIEventID[Enum.GetValues(typeof(enFormEventType)).get_Length()];

		public stUIEventParams[] m_eventParams = new stUIEventParams[Enum.GetValues(typeof(enFormEventType)).get_Length()];

		public string[] m_openedWwiseEvents = new string[]
		{
			"UI_Default_Open_Window"
		};

		public string[] m_closedWwiseEvents = new string[]
		{
			"UI_Default_Close_Window"
		};

		[HideInInspector]
		public enUIEventID m_revertToVisibleEvent;

		[HideInInspector]
		public enUIEventID m_revertToHideEvent;

		public bool m_hideUnderForms;

		public bool m_alwaysKeepVisible;

		public bool m_enableMultiClickedEvent = true;

		public GameObject[] m_formWidgets = new GameObject[0];

		public enFormFadeAnimationType m_formFadeInAnimationType;

		public string m_formFadeInAnimationName = string.Empty;

		public bool m_isPlayFadeInWithAppear;

		private CUIComponent m_formFadeInAnimationScript;

		[HideInInspector]
		public enFormFadeAnimationType m_formFadeOutAnimationType;

		[HideInInspector]
		public string m_formFadeOutAnimationName = string.Empty;

		private CUIComponent m_formFadeOutAnimationScript;

		[HideInInspector]
		public int m_clickedEventDispatchedCounter;

		[HideInInspector]
		public string m_formPath;

		[HideInInspector]
		public bool m_useFormPool;

		private bool m_isNeedClose;

		private bool m_isClosed;

		private bool m_isInFadeIn;

		private bool m_isInFadeOut;

		private Canvas m_canvas;

		[HideInInspector]
		public CanvasScaler m_canvasScaler;

		private GraphicRaycaster m_graphicRaycaster;

		[HideInInspector]
		[NonSerialized]
		public SGameGraphicRaycaster m_sgameGraphicRaycaster;

		private int m_openOrder;

		private int m_sortingOrder;

		private int m_sequence;

		private bool m_isHided;

		private int m_hideFlags;

		private int m_renderFrameStamp;

		private List<CUIFormScript.stInitWidgetPosition> m_initWidgetPositions;

		private bool m_isInitialized;

		private ListView<CUIComponent> m_uiComponents;

		[HideInInspector]
		private ListView<GameObject> m_relatedScenes;

		[HideInInspector]
		private ListView<ListView<Camera>> m_relatedSceneCamera;

		private ListView<CUIFormScript.CASyncLoadedImage> m_asyncLoadedImages;

		private Dictionary<string, GameObject> m_loadedSpriteDictionary;

		private void Awake()
		{
			this.m_uiComponents = new ListView<CUIComponent>();
			this.m_relatedScenes = new ListView<GameObject>();
			this.m_relatedSceneCamera = new ListView<ListView<Camera>>();
			this.InitializeCanvas();
		}

		private void OnDestroy()
		{
			if (this.m_asyncLoadedImages != null)
			{
				this.m_asyncLoadedImages.Clear();
				this.m_asyncLoadedImages = null;
			}
			if (this.m_loadedSpriteDictionary != null)
			{
				this.m_loadedSpriteDictionary.Clear();
				this.m_loadedSpriteDictionary = null;
			}
		}

		public void CustomUpdate()
		{
			this.UpdateFadeIn();
			this.UpdateFadeOut();
		}

		public void CustomLateUpdate()
		{
			if (this.m_initWidgetPositions != null)
			{
				int i = 0;
				while (i < this.m_initWidgetPositions.get_Count())
				{
					CUIFormScript.stInitWidgetPosition stInitWidgetPosition = this.m_initWidgetPositions.get_Item(i);
					if (this.m_renderFrameStamp - stInitWidgetPosition.m_renderFrameStamp <= 1)
					{
						if (stInitWidgetPosition.m_widget != null)
						{
							stInitWidgetPosition.m_widget.transform.position = stInitWidgetPosition.m_worldPosition;
						}
						i++;
					}
					else
					{
						this.m_initWidgetPositions.RemoveAt(i);
					}
				}
			}
			this.UpdateASyncLoadedImage();
			this.m_clickedEventDispatchedCounter = 0;
			this.m_renderFrameStamp++;
		}

		public int GetSequence()
		{
			return this.m_sequence;
		}

		public void SetDisplayOrder(int openOrder)
		{
			DebugHelper.Assert(openOrder > 0, "openOrder = {0}, 该值必须大于0", new object[]
			{
				openOrder
			});
			this.m_openOrder = openOrder;
			if (this.m_canvas != null)
			{
				this.m_sortingOrder = this.CalculateSortingOrder(this.m_priority, this.m_openOrder);
				this.m_canvas.sortingOrder = this.m_sortingOrder;
				try
				{
					if (this.m_canvas.enabled)
					{
						this.m_canvas.enabled = false;
						this.m_canvas.enabled = true;
					}
				}
				catch (Exception ex)
				{
					DebugHelper.Assert(false, "Error form {0}: message: {1}, callstack: {2}", new object[]
					{
						base.name,
						ex.get_Message(),
						ex.get_StackTrace()
					});
				}
			}
			this.SetComponentSortingOrder(this.m_sortingOrder);
		}

		public void Open(string formPath, Camera camera, int sequence, bool exist, int openOrder)
		{
			this.m_formPath = formPath;
			if (this.m_canvas != null)
			{
				this.m_canvas.worldCamera = camera;
				if (camera == null)
				{
					if (this.m_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
					{
						this.m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					}
				}
				else if (this.m_canvas.renderMode != RenderMode.ScreenSpaceCamera)
				{
					this.m_canvas.renderMode = RenderMode.ScreenSpaceCamera;
				}
				this.m_canvas.pixelPerfect = true;
			}
			this.RefreshCanvasScaler();
			this.Open(sequence, exist, openOrder);
		}

		public void Open(int sequence, bool exist, int openOrder)
		{
			this.m_isNeedClose = false;
			this.m_isClosed = false;
			this.m_isInFadeIn = false;
			this.m_isInFadeOut = false;
			this.m_clickedEventDispatchedCounter = 0;
			this.m_sequence = sequence;
			this.SetDisplayOrder(openOrder);
			this.m_renderFrameStamp = 0;
			if (!exist)
			{
				this.Initialize();
				if (this.m_graphicRaycaster)
				{
					this.m_graphicRaycaster.enabled = !this.m_disableInput;
				}
				this.DispatchFormEvent(enFormEventType.Open);
				for (int i = 0; i < this.m_openedWwiseEvents.Length; i++)
				{
					if (!string.IsNullOrEmpty(this.m_openedWwiseEvents[i]))
					{
						Singleton<CSoundManager>.GetInstance().PostEvent(this.m_openedWwiseEvents[i], null);
					}
				}
				if (this.IsNeedFadeIn())
				{
					this.StartFadeIn();
				}
			}
		}

		public void Close()
		{
			if (this.m_isNeedClose)
			{
				return;
			}
			this.m_isNeedClose = true;
			this.DispatchFormEvent(enFormEventType.Close);
			for (int i = 0; i < this.m_closedWwiseEvents.Length; i++)
			{
				if (!string.IsNullOrEmpty(this.m_closedWwiseEvents[i]))
				{
					Singleton<CSoundManager>.GetInstance().PostEvent(this.m_closedWwiseEvents[i], null);
				}
			}
			this.CloseComponent();
		}

		public bool IsNeedClose()
		{
			return this.m_isNeedClose;
		}

		public bool TurnToClosed(bool ignoreFadeOut)
		{
			this.m_isNeedClose = false;
			this.m_isClosed = true;
			Singleton<EventRouter>.GetInstance().BroadCastEvent<string>(EventID.UI_FORM_CLOSED, this.m_formPath);
			if (ignoreFadeOut || !this.IsNeedFadeOut())
			{
				return true;
			}
			this.StartFadeOut();
			return false;
		}

		public bool IsClosed()
		{
			return this.m_isClosed;
		}

		public bool IsCanvasEnabled()
		{
			return !(this.m_canvas == null) && this.m_canvas.enabled;
		}

		public float ChangeScreenValueToForm(float value)
		{
			if (this.m_canvasScaler.get_matchWidthOrHeight() == 0f)
			{
				return value * this.m_canvasScaler.get_referenceResolution().x / (float)Screen.width;
			}
			if (this.m_canvasScaler.get_matchWidthOrHeight() == 1f)
			{
				return value * this.m_canvasScaler.get_referenceResolution().y / (float)Screen.height;
			}
			return value;
		}

		public float ChangeFormValueToScreen(float value)
		{
			if (this.m_canvasScaler.get_matchWidthOrHeight() == 0f)
			{
				return value * (float)Screen.width / this.m_canvasScaler.get_referenceResolution().x;
			}
			if (this.m_canvasScaler.get_matchWidthOrHeight() == 1f)
			{
				return value * (float)Screen.height / this.m_canvasScaler.get_referenceResolution().y;
			}
			return value;
		}

		public void InitializeWidgetPosition(int widgetIndex, Vector3 worldPosition)
		{
			this.InitializeWidgetPosition(this.GetWidget(widgetIndex), worldPosition);
		}

		public void InitializeWidgetPosition(GameObject widget, Vector3 worldPosition)
		{
			if (this.m_initWidgetPositions == null)
			{
				this.m_initWidgetPositions = new List<CUIFormScript.stInitWidgetPosition>();
			}
			CUIFormScript.stInitWidgetPosition stInitWidgetPosition = default(CUIFormScript.stInitWidgetPosition);
			stInitWidgetPosition.m_renderFrameStamp = this.m_renderFrameStamp;
			stInitWidgetPosition.m_widget = widget;
			stInitWidgetPosition.m_worldPosition = worldPosition;
			this.m_initWidgetPositions.Add(stInitWidgetPosition);
		}

		public void Initialize()
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.m_defaultPriority = this.m_priority;
			this.InitializeComponent(base.gameObject);
			this.m_isInitialized = true;
		}

		public void SetPriority(enFormPriority priority)
		{
			if (this.m_priority == priority)
			{
				return;
			}
			this.m_priority = priority;
			this.SetDisplayOrder(this.m_openOrder);
			this.DispatchChangeFormPriorityEvent();
		}

		public void RestorePriority()
		{
			this.SetPriority(this.m_defaultPriority);
		}

		public void SetActive(bool active)
		{
			base.gameObject.CustomSetActive(active);
			if (active)
			{
				this.Appear(enFormHideFlag.HideByCustom, true);
			}
			else
			{
				this.Hide(enFormHideFlag.HideByCustom, true);
			}
		}

		public void Hide(enFormHideFlag hideFlag = enFormHideFlag.HideByCustom, bool dispatchVisibleChangedEvent = true)
		{
			if (this.m_alwaysKeepVisible)
			{
				return;
			}
			this.m_hideFlags |= (int)hideFlag;
			if (this.m_hideFlags == 0 || this.m_isHided)
			{
				return;
			}
			this.m_isHided = true;
			if (this.m_canvas != null)
			{
				this.m_canvas.enabled = false;
			}
			this.TryEnableInput(false);
			for (int i = 0; i < this.m_relatedScenes.Count; i++)
			{
				CUIUtility.SetGameObjectLayer(this.m_relatedScenes[i], 31);
				this.SetSceneCameraEnable(i, false);
			}
			this.HideComponent();
			if (this.m_revertToHideEvent != enUIEventID.None)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(this.m_revertToHideEvent);
			}
			if (dispatchVisibleChangedEvent)
			{
				this.DispatchVisibleChangedEvent();
			}
		}

		public void SetSceneCameraEnable(int index, bool bEnable)
		{
			if (index < 0 || index >= this.m_relatedSceneCamera.Count || this.m_relatedSceneCamera[index] == null)
			{
				return;
			}
			for (int i = 0; i < this.m_relatedSceneCamera[index].Count; i++)
			{
				if (this.m_relatedSceneCamera[index][i] != null)
				{
					this.m_relatedSceneCamera[index][i].enabled = bEnable;
				}
			}
		}

		public bool IsHided()
		{
			return this.m_isHided;
		}

		public void Appear(enFormHideFlag hideFlag = enFormHideFlag.HideByCustom, bool dispatchVisibleChangedEvent = true)
		{
			if (this.m_alwaysKeepVisible)
			{
				return;
			}
			this.m_hideFlags &= (int)(~(int)hideFlag);
			if (this.m_hideFlags != 0 || !this.m_isHided)
			{
				return;
			}
			this.m_isHided = false;
			if (this.m_canvas != null)
			{
				this.m_canvas.enabled = true;
				this.m_canvas.sortingOrder = this.m_sortingOrder;
			}
			this.TryEnableInput(true);
			for (int i = 0; i < this.m_relatedScenes.Count; i++)
			{
				CUIUtility.SetGameObjectLayer(this.m_relatedScenes[i], 18);
				this.SetSceneCameraEnable(i, true);
			}
			this.AppearComponent();
			this.DispatchRevertVisibleFormEvent();
			if (dispatchVisibleChangedEvent)
			{
				this.DispatchVisibleChangedEvent();
			}
		}

		public void TryEnableInput(bool isEnable)
		{
			if (this.m_graphicRaycaster == null)
			{
				return;
			}
			if (!isEnable)
			{
				this.m_graphicRaycaster.enabled = false;
			}
			else if (isEnable && !this.m_disableInput)
			{
				this.m_graphicRaycaster.enabled = true;
			}
		}

		public int CompareTo(object obj)
		{
			CUIFormScript cUIFormScript = obj as CUIFormScript;
			if (this.m_sortingOrder > cUIFormScript.m_sortingOrder)
			{
				return 1;
			}
			if (this.m_sortingOrder == cUIFormScript.m_sortingOrder)
			{
				return 0;
			}
			return -1;
		}

		public void InitializeCanvas()
		{
			this.m_canvas = base.gameObject.GetComponent<Canvas>();
			this.m_canvasScaler = base.gameObject.GetComponent<CanvasScaler>();
			this.m_graphicRaycaster = base.GetComponent<GraphicRaycaster>();
			this.m_sgameGraphicRaycaster = (this.m_graphicRaycaster as SGameGraphicRaycaster);
			this.MatchScreen();
		}

		public void MatchScreen()
		{
			if (this.m_canvasScaler == null)
			{
				return;
			}
			this.m_canvasScaler.set_referenceResolution(this.m_referenceResolution);
			if ((float)Screen.width / this.m_canvasScaler.get_referenceResolution().x > (float)Screen.height / this.m_canvasScaler.get_referenceResolution().y)
			{
				if (this.m_fullScreenBG)
				{
					this.m_canvasScaler.set_matchWidthOrHeight(0f);
				}
				else
				{
					this.m_canvasScaler.set_matchWidthOrHeight(1f);
				}
			}
			else if (this.m_fullScreenBG)
			{
				this.m_canvasScaler.set_matchWidthOrHeight(1f);
			}
			else
			{
				this.m_canvasScaler.set_matchWidthOrHeight(0f);
			}
		}

		public GameObject GetWidget(int index)
		{
			if (index < 0 || index >= this.m_formWidgets.Length)
			{
				return null;
			}
			return this.m_formWidgets[index];
		}

		public GraphicRaycaster GetGraphicRaycaster()
		{
			return this.m_graphicRaycaster;
		}

		public Camera GetCamera()
		{
			if (this.m_canvas == null || this.m_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				return null;
			}
			return this.m_canvas.worldCamera;
		}

		public Vector2 GetReferenceResolution()
		{
			return (this.m_canvasScaler == null) ? Vector2.zero : this.m_canvasScaler.get_referenceResolution();
		}

		public int GetSortingOrder()
		{
			return this.m_sortingOrder;
		}

		public void AddUIComponent(CUIComponent uiComponent)
		{
			if (uiComponent != null && !this.m_uiComponents.Contains(uiComponent))
			{
				this.m_uiComponents.Add(uiComponent);
			}
		}

		public void RemoveUIComponent(CUIComponent uiComponent)
		{
			if (this.m_uiComponents.Contains(uiComponent))
			{
				this.m_uiComponents.Remove(uiComponent);
			}
		}

		public bool IsRelatedSceneExist(string sceneName)
		{
			for (int i = 0; i < this.m_relatedScenes.Count; i++)
			{
				if (string.Equals(sceneName, this.m_relatedScenes[i].name))
				{
					return true;
				}
			}
			return false;
		}

		public void AddRelatedScene(GameObject scene, string sceneName)
		{
			scene.name = sceneName;
			scene.transform.SetParent(base.gameObject.transform);
			scene.transform.localPosition = Vector3.zero;
			scene.transform.localRotation = Quaternion.identity;
			this.m_relatedScenes.Add(scene);
			this.m_relatedSceneCamera.Add(new ListView<Camera>());
			this.AddRelatedSceneCamera(this.m_relatedSceneCamera.Count - 1, scene);
		}

		public void AddRelatedSceneCamera(int index, GameObject go)
		{
			if (index < 0 || index >= this.m_relatedSceneCamera.Count || go == null)
			{
				return;
			}
			Camera component = go.GetComponent<Camera>();
			if (component != null)
			{
				this.m_relatedSceneCamera[index].Add(component);
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				this.AddRelatedSceneCamera(index, go.transform.GetChild(i).gameObject);
			}
		}

		public void AddASyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
		{
			if (this.m_asyncLoadedImages == null)
			{
				this.m_asyncLoadedImages = new ListView<CUIFormScript.CASyncLoadedImage>();
			}
			if (this.m_loadedSpriteDictionary == null)
			{
				this.m_loadedSpriteDictionary = new Dictionary<string, GameObject>();
			}
			for (int i = 0; i < this.m_asyncLoadedImages.Count; i++)
			{
				if (this.m_asyncLoadedImages[i].m_image == image)
				{
					this.m_asyncLoadedImages[i].m_prefabPath = prefabPath;
					return;
				}
			}
			CUIFormScript.CASyncLoadedImage item = new CUIFormScript.CASyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded, isShowSpecMatrial);
			this.m_asyncLoadedImages.Add(item);
		}

		private void UpdateASyncLoadedImage()
		{
			if (this.m_asyncLoadedImages == null)
			{
				return;
			}
			bool flag = false;
			int i = 0;
			while (i < this.m_asyncLoadedImages.Count)
			{
				CUIFormScript.CASyncLoadedImage cASyncLoadedImage = this.m_asyncLoadedImages[i];
				Image image = cASyncLoadedImage.m_image;
				if (image != null)
				{
					GameObject gameObject = null;
					string prefabPath = cASyncLoadedImage.m_prefabPath;
					if (!this.m_loadedSpriteDictionary.TryGetValue(prefabPath, ref gameObject) && !flag)
					{
						gameObject = CUIUtility.GetSpritePrefeb(prefabPath, cASyncLoadedImage.m_needCached, cASyncLoadedImage.m_unloadBelongedAssetBundleAfterLoaded);
						this.m_loadedSpriteDictionary.Add(prefabPath, gameObject);
						flag = true;
					}
					if (gameObject != null)
					{
						image.set_color(new Color(image.get_color().r, image.get_color().g, image.get_color().b, 1f));
						image.SetSprite(gameObject, cASyncLoadedImage.m_isShowSpecMatrial);
						this.m_asyncLoadedImages.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
				else
				{
					this.m_asyncLoadedImages.RemoveAt(i);
				}
			}
		}

		public bool IsNeedFadeIn()
		{
			return GameSettings.RenderQuality != SGameRenderQuality.Low && this.m_formFadeInAnimationType != enFormFadeAnimationType.None && !string.IsNullOrEmpty(this.m_formFadeInAnimationName);
		}

		public bool IsNeedFadeOut()
		{
			return false;
		}

		public void RePlayFadIn()
		{
			this.StartFadeIn();
		}

		private void RefreshCanvasScaler()
		{
			try
			{
				if (this.m_canvasScaler != null)
				{
					this.m_canvasScaler.enabled = false;
					this.m_canvasScaler.enabled = true;
				}
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Error form {0}: message: {1}, callstack: {2}", new object[]
				{
					base.name,
					ex.get_Message(),
					ex.get_StackTrace()
				});
			}
		}

		private void DispatchFormEvent(enFormEventType formEventType)
		{
			if (this.m_eventIDs[(int)formEventType] == enUIEventID.None)
			{
				return;
			}
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = this.m_eventIDs[(int)formEventType];
			uIEvent.m_eventParams = this.m_eventParams[(int)formEventType];
			uIEvent.m_srcFormScript = this;
			uIEvent.m_srcWidget = null;
			uIEvent.m_srcWidgetScript = null;
			uIEvent.m_srcWidgetBelongedListScript = null;
			uIEvent.m_srcWidgetIndexInBelongedList = 0;
			uIEvent.m_pointerEventData = null;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
		}

		private void DispatchChangeFormPriorityEvent()
		{
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.UI_OnFormPriorityChanged;
			uIEvent.m_srcFormScript = this;
			uIEvent.m_srcWidget = null;
			uIEvent.m_srcWidgetScript = null;
			uIEvent.m_srcWidgetBelongedListScript = null;
			uIEvent.m_srcWidgetIndexInBelongedList = 0;
			uIEvent.m_pointerEventData = null;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
		}

		private void DispatchVisibleChangedEvent()
		{
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.UI_OnFormVisibleChanged;
			uIEvent.m_srcFormScript = this;
			uIEvent.m_srcWidget = null;
			uIEvent.m_srcWidgetScript = null;
			uIEvent.m_srcWidgetBelongedListScript = null;
			uIEvent.m_srcWidgetIndexInBelongedList = 0;
			uIEvent.m_pointerEventData = null;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
		}

		private void DispatchRevertVisibleFormEvent()
		{
			if (this.m_revertToVisibleEvent == enUIEventID.None)
			{
				return;
			}
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = this.m_revertToVisibleEvent;
			uIEvent.m_srcFormScript = this;
			uIEvent.m_srcWidget = null;
			uIEvent.m_srcWidgetScript = null;
			uIEvent.m_srcWidgetBelongedListScript = null;
			uIEvent.m_srcWidgetIndexInBelongedList = 0;
			uIEvent.m_pointerEventData = null;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
		}

		private bool IsOverlay()
		{
			return !(this.m_canvas == null) && (this.m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || this.m_canvas.worldCamera == null);
		}

		private int CalculateSortingOrder(enFormPriority priority, int openOrder)
		{
			if (openOrder * 10 >= 1000)
			{
				openOrder %= 100;
			}
			return (int)((this.IsOverlay() ? 10000 : 0) + priority * (enFormPriority)1000 + openOrder * 10);
		}

		public void InitializeComponent(GameObject root)
		{
			CUIComponent[] components = root.GetComponents<CUIComponent>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					components[i].Initialize(this);
				}
			}
			for (int j = 0; j < root.transform.childCount; j++)
			{
				this.InitializeComponent(root.transform.GetChild(j).gameObject);
			}
		}

		private void CloseComponent()
		{
			for (int i = 0; i < this.m_uiComponents.Count; i++)
			{
				this.m_uiComponents[i].Close();
			}
		}

		private void HideComponent()
		{
			for (int i = 0; i < this.m_uiComponents.Count; i++)
			{
				this.m_uiComponents[i].Hide();
			}
		}

		private void AppearComponent()
		{
			for (int i = 0; i < this.m_uiComponents.Count; i++)
			{
				this.m_uiComponents[i].Appear();
			}
		}

		private void SetComponentSortingOrder(int sortingOrder)
		{
			for (int i = 0; i < this.m_uiComponents.Count; i++)
			{
				this.m_uiComponents[i].SetSortingOrder(sortingOrder);
			}
		}

		public void ClearFadeAnimationEndEvent()
		{
			CUIAnimationScript component = base.gameObject.GetComponent<CUIAnimationScript>();
			if (component != null)
			{
				component.SetAnimationEvent(enAnimationEventType.AnimationEnd, enUIEventID.None, default(stUIEventParams));
			}
		}

		private void StartFadeIn()
		{
			if (this.m_formFadeInAnimationType == enFormFadeAnimationType.None || string.IsNullOrEmpty(this.m_formFadeInAnimationName))
			{
				return;
			}
			enFormFadeAnimationType formFadeInAnimationType = this.m_formFadeInAnimationType;
			if (formFadeInAnimationType != enFormFadeAnimationType.Animation)
			{
				if (formFadeInAnimationType == enFormFadeAnimationType.Animator)
				{
					this.m_formFadeInAnimationScript = base.gameObject.GetComponent<CUIAnimatorScript>();
					if (this.m_formFadeInAnimationScript != null)
					{
						((CUIAnimatorScript)this.m_formFadeInAnimationScript).PlayAnimator(this.m_formFadeInAnimationName);
						this.m_isInFadeIn = true;
					}
				}
			}
			else
			{
				this.m_formFadeInAnimationScript = base.gameObject.GetComponent<CUIAnimationScript>();
				if (this.m_formFadeInAnimationScript != null)
				{
					this.ClearFadeAnimationEndEvent();
					((CUIAnimationScript)this.m_formFadeInAnimationScript).PlayAnimation(this.m_formFadeInAnimationName, true);
					this.m_isInFadeIn = true;
				}
			}
		}

		private void StartFadeOut()
		{
			if (this.m_formFadeOutAnimationType == enFormFadeAnimationType.None || string.IsNullOrEmpty(this.m_formFadeOutAnimationName))
			{
				return;
			}
			enFormFadeAnimationType formFadeOutAnimationType = this.m_formFadeOutAnimationType;
			if (formFadeOutAnimationType != enFormFadeAnimationType.Animation)
			{
				if (formFadeOutAnimationType == enFormFadeAnimationType.Animator)
				{
					this.m_formFadeOutAnimationScript = base.gameObject.GetComponent<CUIAnimatorScript>();
					if (this.m_formFadeOutAnimationScript != null)
					{
						((CUIAnimatorScript)this.m_formFadeOutAnimationScript).PlayAnimator(this.m_formFadeOutAnimationName);
						this.m_isInFadeOut = true;
					}
				}
			}
			else
			{
				this.m_formFadeOutAnimationScript = base.gameObject.GetComponent<CUIAnimationScript>();
				if (this.m_formFadeOutAnimationScript != null)
				{
					((CUIAnimationScript)this.m_formFadeOutAnimationScript).PlayAnimation(this.m_formFadeOutAnimationName, true);
					this.m_isInFadeOut = true;
				}
			}
		}

		private void UpdateFadeIn()
		{
			if (this.m_isInFadeIn)
			{
				enFormFadeAnimationType formFadeInAnimationType = this.m_formFadeInAnimationType;
				if (formFadeInAnimationType != enFormFadeAnimationType.Animation)
				{
					if (formFadeInAnimationType == enFormFadeAnimationType.Animator && (this.m_formFadeInAnimationScript == null || ((CUIAnimatorScript)this.m_formFadeInAnimationScript).IsAnimationStopped(this.m_formFadeInAnimationName)))
					{
						this.m_isInFadeIn = false;
					}
				}
				else if (this.m_formFadeInAnimationScript == null || ((CUIAnimationScript)this.m_formFadeInAnimationScript).IsAnimationStopped(this.m_formFadeInAnimationName))
				{
					this.m_isInFadeIn = false;
				}
			}
		}

		private void UpdateFadeOut()
		{
			if (this.m_isInFadeOut)
			{
				enFormFadeAnimationType formFadeOutAnimationType = this.m_formFadeOutAnimationType;
				if (formFadeOutAnimationType != enFormFadeAnimationType.Animation)
				{
					if (formFadeOutAnimationType == enFormFadeAnimationType.Animator && (this.m_formFadeOutAnimationScript == null || ((CUIAnimatorScript)this.m_formFadeOutAnimationScript).IsAnimationStopped(this.m_formFadeOutAnimationName)))
					{
						this.m_isInFadeOut = false;
					}
				}
				else if (this.m_formFadeOutAnimationScript == null || ((CUIAnimationScript)this.m_formFadeOutAnimationScript).IsAnimationStopped(this.m_formFadeOutAnimationName))
				{
					this.m_isInFadeOut = false;
				}
			}
		}

		public bool IsInFadeIn()
		{
			return this.m_isInFadeIn;
		}

		public bool IsInFadeOut()
		{
			return this.m_isInFadeOut;
		}

		public float GetScreenScaleValue()
		{
			float result = 1f;
			RectTransform component = base.GetComponent<RectTransform>();
			if (component && this.m_canvasScaler.get_matchWidthOrHeight() == 0f)
			{
				result = component.rect.width / component.rect.height / (this.m_canvasScaler.get_referenceResolution().x / this.m_canvasScaler.get_referenceResolution().y);
			}
			return result;
		}

		public void SetHideUnderForm(bool isHideUnderForm)
		{
			this.m_hideUnderForms = isHideUnderForm;
			Singleton<CUIManager>.instance.ResetAllFormHideOrShowState();
		}

		public void SetFormEventParams(enFormEventType formEventType, stUIEventParams formEventParams)
		{
			this.m_eventParams[(int)formEventType] = formEventParams;
		}

		public stUIEventParams GetFormEventParams(enFormEventType formEventType)
		{
			return this.m_eventParams[(int)formEventType];
		}
	}
}
