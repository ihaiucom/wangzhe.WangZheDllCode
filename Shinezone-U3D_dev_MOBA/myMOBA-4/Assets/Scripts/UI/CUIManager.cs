using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIManager : Singleton<CUIManager>
	{
		private enum enInfoFormWidgets
		{
			Title_Text,
			Info_Text
		}

		public enum enEditFormWidgets
		{
			Title_Text,
			Input_Text,
			Confirm_Button
		}

		public delegate void OnFormSorted(ListView<CUIFormScript> inForms);

		public delegate void StringSendboxOnSend(string msg);

		private const int c_formCameraMaskLayer = 5;

		private const int c_formCameraDepth = 10;

		private const string K_SEARCH_FORM_PATH = "UGUI/Form/Common/Search/Form_Search.prefab";

		private const string K_STR_SENDER_FORM_PATH = "UGUI/Form/Common/Search/Form_StringSender.prefab";

		private ListView<CUIFormScript> m_forms;

		private ListView<CUIFormScript> m_pooledForms;

		private int m_formSequence;

		private List<int> m_existFormSequences;

		private GameObject m_uiRoot;

		public CUIManager.OnFormSorted onFormSorted;

		public static int s_uiSystemRenderFrameCounter;

		private EventSystem m_uiInputEventSystem;

		private Camera m_formCamera;

		private static string s_formCameraName = "Camera_Form";

		private static string s_uiSceneName = "UI_Scene";

		private bool m_needSortForms;

		private bool m_needUpdateRaycasterAndHide;

		private int _curMaxExchangeCount = 20;

		private enUIEventID _confirmId;

		private ushort _curValidIndex;

		private uint _price;

		private uint _totalPirce;

		private uint m_searchHandlerCMD;

		private uint m_recommendHandlerCMD;

		private GameObject m_searchResultGo;

		private GameObject m_searchRecommendGo;

		private string m_searchEvtCallBack;

		private string m_recommendEvtCallBack;

		private string m_recommendEvtCallBackSingleEnable;

		private Vector2 m_searchBoxOrgSizeDetla;

		private float m_deltaSearchResultHeight;

		private CUIManager.StringSendboxOnSend m_strSendboxCb;

		public Camera FormCamera
		{
			get
			{
				return this.m_formCamera;
			}
		}

		public override void Init()
		{
			this.m_forms = new ListView<CUIFormScript>();
			this.m_pooledForms = new ListView<CUIFormScript>();
			this.m_formSequence = 0;
			this.m_existFormSequences = new List<int>();
			CUIManager.s_uiSystemRenderFrameCounter = 0;
			this.CreateUIRoot();
			this.CreateEventSystem();
			this.CreateCamera();
			this.CreateUISecene();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UI_OnFormPriorityChanged, new CUIEventManager.OnUIEventHandler(this.OnFormPriorityChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UI_OnFormVisibleChanged, new CUIEventManager.OnUIEventHandler(this.OnFormVisibleChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_OnAddExchangeCount, new CUIEventManager.OnUIEventHandler(this.OnAddExchangeCount));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_OnDecreseExchangeCount, new CUIEventManager.OnUIEventHandler(this.OnDecreaseExchangeCount));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_OnMaxExchangeCount, new CUIEventManager.OnUIEventHandler(this.OnMaxExchangeCount));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SearchBox_CloseForm, new CUIEventManager.OnUIEventHandler(this.SearchBox_OnClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.StrSenderBox_OnSend, new CUIEventManager.OnUIEventHandler(this.OnStringSenderBoxSend));
		}

		public void LoadSoundBank()
		{
			Singleton<CSoundManager>.GetInstance().LoadBank("UI", CSoundManager.BankType.Global);
		}

		public void Update()
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				this.m_forms[i].CustomUpdate();
				if (this.m_forms[i].IsNeedClose())
				{
					if (this.m_forms[i].TurnToClosed(false))
					{
						this.RecycleForm(i);
						this.m_needSortForms = true;
						continue;
					}
				}
				else if (this.m_forms[i].IsClosed() && !this.m_forms[i].IsInFadeOut())
				{
					this.RecycleForm(i);
					this.m_needSortForms = true;
					continue;
				}
			}
			if (this.m_needSortForms)
			{
				this.ProcessFormList(true, true);
			}
			else if (this.m_needUpdateRaycasterAndHide)
			{
				this.ProcessFormList(false, true);
			}
			this.m_needSortForms = false;
			this.m_needUpdateRaycasterAndHide = false;
		}

		public void LateUpdate()
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				this.m_forms[i].CustomLateUpdate();
			}
			CUIManager.s_uiSystemRenderFrameCounter++;
		}

		public CUIFormScript OpenForm(string formPath, bool useFormPool, bool useCameraRenderMode = true)
		{
			CUIFormScript cUIFormScript = this.GetUnClosedForm(formPath);
			if (cUIFormScript != null && cUIFormScript.m_isSingleton)
			{
				this.RemoveFromExistFormSequenceList(cUIFormScript.GetSequence());
				this.AddToExistFormSequenceList(this.m_formSequence);
				int formOpenOrder = this.GetFormOpenOrder(this.m_formSequence);
				cUIFormScript.Open(this.m_formSequence, true, formOpenOrder);
				this.m_formSequence++;
				this.m_needSortForms = true;
				return cUIFormScript;
			}
			GameObject gameObject = this.CreateForm(formPath, useFormPool);
			if (gameObject == null)
			{
				return null;
			}
			if (!gameObject.activeSelf)
			{
				gameObject.CustomSetActive(true);
			}
			string formName = this.GetFormName(formPath);
			gameObject.name = formName;
			if (gameObject.transform.parent != this.m_uiRoot.transform)
			{
				gameObject.transform.SetParent(this.m_uiRoot.transform);
			}
			cUIFormScript = gameObject.GetComponent<CUIFormScript>();
			if (cUIFormScript != null)
			{
				this.AddToExistFormSequenceList(this.m_formSequence);
				int formOpenOrder2 = this.GetFormOpenOrder(this.m_formSequence);
				cUIFormScript.Open(formPath, (!useCameraRenderMode) ? null : this.m_formCamera, this.m_formSequence, false, formOpenOrder2);
				if (cUIFormScript.m_group > 0)
				{
					this.CloseGroupForm(cUIFormScript.m_group);
				}
				this.m_forms.Add(cUIFormScript);
			}
			this.m_formSequence++;
			this.m_needSortForms = true;
			return cUIFormScript;
		}

		public void CloseForm(string formPath)
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				if (string.Equals(this.m_forms[i].m_formPath, formPath))
				{
					this.m_forms[i].Close();
				}
			}
		}

		public void CloseForm(CUIFormScript formScript)
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				if (this.m_forms[i] == formScript)
				{
					this.m_forms[i].Close();
				}
			}
		}

		public void CloseForm(int formSequence)
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				if (this.m_forms[i].GetSequence() == formSequence)
				{
					this.m_forms[i].Close();
				}
			}
		}

		public void CloseAllFormExceptLobby(bool closeImmediately = true)
		{
			string[] exceptFormNames = new string[]
			{
				CLobbySystem.LOBBY_FORM_PATH,
				CLobbySystem.SYSENTRY_FORM_PATH,
				CChatController.ChatFormPath,
				CLobbySystem.RANKING_BTN_FORM_PATH
			};
			Singleton<CUIManager>.GetInstance().CloseAllForm(exceptFormNames, closeImmediately, true);
			Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
		}

		public void CloseAllForm(string[] exceptFormNames = null, bool closeImmediately = true, bool clearFormPool = true)
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				bool flag = true;
				if (exceptFormNames != null)
				{
					for (int j = 0; j < exceptFormNames.Length; j++)
					{
						if (string.Equals(this.m_forms[i].m_formPath, exceptFormNames[j]))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					this.m_forms[i].Close();
				}
			}
			if (closeImmediately)
			{
				int k = 0;
				while (k < this.m_forms.Count)
				{
					if (this.m_forms[k].IsNeedClose() || this.m_forms[k].IsClosed())
					{
						if (this.m_forms[k].IsNeedClose())
						{
							this.m_forms[k].TurnToClosed(true);
						}
						this.RecycleForm(k);
					}
					else
					{
						k++;
					}
				}
				if (exceptFormNames != null)
				{
					this.ProcessFormList(true, true);
				}
			}
			if (clearFormPool)
			{
				this.ClearFormPool();
			}
		}

		private void RecycleForm(int formIndex)
		{
			this.RemoveFromExistFormSequenceList(this.m_forms[formIndex].GetSequence());
			this.RecycleForm(this.m_forms[formIndex]);
			this.m_forms.RemoveAt(formIndex);
		}

		public void AddToExistFormSequenceList(int formSequence)
		{
			if (this.m_existFormSequences != null)
			{
				this.m_existFormSequences.Add(formSequence);
			}
		}

		public void RemoveFromExistFormSequenceList(int formSequence)
		{
			if (this.m_existFormSequences != null)
			{
				this.m_existFormSequences.Remove(formSequence);
			}
		}

		public int GetFormOpenOrder(int formSequence)
		{
			int num = this.m_existFormSequences.IndexOf(formSequence);
			return (num < 0) ? 0 : (num + 1);
		}

		public bool HasForm()
		{
			return this.m_forms.Count > 0;
		}

		public CUIFormScript GetForm(string formPath)
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				if (this.m_forms[i].m_formPath.Equals(formPath) && !this.m_forms[i].IsNeedClose() && !this.m_forms[i].IsClosed())
				{
					return this.m_forms[i];
				}
			}
			return null;
		}

		public CUIFormScript GetForm(int formSequence)
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				if (this.m_forms[i].GetSequence() == formSequence && !this.m_forms[i].IsNeedClose() && !this.m_forms[i].IsClosed())
				{
					return this.m_forms[i];
				}
			}
			return null;
		}

		public void CloseGroupForm(int group)
		{
			if (group == 0)
			{
				return;
			}
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				if (this.m_forms[i].m_group == group)
				{
					this.m_forms[i].Close();
				}
			}
		}

		public void DisableInput()
		{
			if (this.m_uiInputEventSystem != null)
			{
				this.m_uiInputEventSystem.gameObject.CustomSetActive(false);
			}
		}

		public void EnableInput()
		{
			if (this.m_uiInputEventSystem != null)
			{
				this.m_uiInputEventSystem.gameObject.CustomSetActive(true);
			}
		}

		public void ClearEventGraphicsData()
		{
			System.Reflection.MemberInfo[] member = typeof(GraphicRaycaster).GetMember("s_SortedGraphics", BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.NonPublic);
			if (member != null && member.Length == 1)
			{
				System.Reflection.MemberInfo memberInfo = member[0];
				if (memberInfo != null && memberInfo.MemberType == MemberTypes.Field)
				{
					FieldInfo fieldInfo = memberInfo as FieldInfo;
					if (fieldInfo != null)
					{
						List<Graphic> list = fieldInfo.GetValue(null) as List<Graphic>;
						if (list != null)
						{
							list.Clear();
						}
					}
				}
			}
		}

		public void ClearFormPool()
		{
			for (int i = 0; i < this.m_pooledForms.Count; i++)
			{
				CUICommonSystem.DestoryObj(this.m_pooledForms[i].gameObject, 0.1f);
			}
			this.m_pooledForms.Clear();
		}

		public CUIFormScript GetTopForm()
		{
			CUIFormScript cUIFormScript = null;
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				if (!(this.m_forms[i] == null))
				{
					if (cUIFormScript == null)
					{
						cUIFormScript = this.m_forms[i];
					}
					else if (this.m_forms[i].GetSortingOrder() > cUIFormScript.GetSortingOrder())
					{
						cUIFormScript = this.m_forms[i];
					}
				}
			}
			return cUIFormScript;
		}

		public ListView<CUIFormScript> GetForms()
		{
			return this.m_forms;
		}

		public EventSystem GetEventSystem()
		{
			return this.m_uiInputEventSystem;
		}

		public void ResetAllFormHideOrShowState()
		{
			this.m_needUpdateRaycasterAndHide = true;
		}

		private void ProcessFormList(bool sort, bool handleInputAndHide)
		{
			if (sort)
			{
				this.m_forms.Sort();
				for (int i = 0; i < this.m_forms.Count; i++)
				{
					int formOpenOrder = this.GetFormOpenOrder(this.m_forms[i].GetSequence());
					this.m_forms[i].SetDisplayOrder(formOpenOrder);
				}
			}
			if (handleInputAndHide)
			{
				this.UpdateFormHided();
				this.UpdateFormRaycaster();
			}
			if (this.onFormSorted != null)
			{
				this.onFormSorted(this.m_forms);
			}
		}

		private string GetFormName(string formPath)
		{
			return CFileManager.EraseExtension(CFileManager.GetFullName(formPath));
		}

		private GameObject CreateForm(string formPrefabPath, bool useFormPool)
		{
			GameObject gameObject = null;
			if (useFormPool)
			{
				for (int i = 0; i < this.m_pooledForms.Count; i++)
				{
					if (string.Equals(formPrefabPath, this.m_pooledForms[i].m_formPath, StringComparison.OrdinalIgnoreCase))
					{
						this.m_pooledForms[i].Appear(enFormHideFlag.HideByCustom, true);
						gameObject = this.m_pooledForms[i].gameObject;
						this.m_pooledForms.RemoveAt(i);
						break;
					}
				}
			}
			if (gameObject == null)
			{
				GameObject gameObject2 = (GameObject)Singleton<CResourceManager>.GetInstance().GetResource(formPrefabPath, typeof(GameObject), enResourceType.UIForm, false, false).m_content;
				if (gameObject2 == null)
				{
					return null;
				}
				gameObject = (GameObject)UnityEngine.Object.Instantiate(gameObject2);
			}
			if (gameObject != null)
			{
				CUIFormScript component = gameObject.GetComponent<CUIFormScript>();
				if (component != null)
				{
					component.m_useFormPool = useFormPool;
				}
			}
			return gameObject;
		}

		private void RecycleForm(CUIFormScript formScript)
		{
			if (formScript == null)
			{
				return;
			}
			if (formScript.m_useFormPool)
			{
				formScript.Hide(enFormHideFlag.HideByCustom, true);
				this.m_pooledForms.Add(formScript);
			}
			else
			{
				try
				{
					if (formScript.m_canvasScaler != null)
					{
						formScript.m_canvasScaler.enabled = false;
					}
					CUICommonSystem.DestoryObj(formScript.gameObject, 0f);
				}
				catch (Exception ex)
				{
					DebugHelper.Assert(false, "Error destroy {0} formScript gameObject: message: {1}, callstack: {2}", new object[]
					{
						formScript.name,
						ex.Message,
						ex.StackTrace
					});
				}
			}
		}

		private CUIFormScript GetUnClosedForm(string formPath)
		{
			for (int i = 0; i < this.m_forms.Count; i++)
			{
				if (this.m_forms[i].m_formPath.Equals(formPath) && !this.m_forms[i].IsClosed())
				{
					return this.m_forms[i];
				}
			}
			return null;
		}

		private void CreateUIRoot()
		{
			this.m_uiRoot = new GameObject("CUIManager");
			GameObject gameObject = GameObject.Find("BootObj");
			if (gameObject != null)
			{
				this.m_uiRoot.transform.parent = gameObject.transform;
			}
		}

		private void CreateEventSystem()
		{
			this.m_uiInputEventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
			if (this.m_uiInputEventSystem == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				this.m_uiInputEventSystem = gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<TouchInputModule>();
			}
			this.m_uiInputEventSystem.gameObject.transform.parent = this.m_uiRoot.transform;
		}

		private void CreateCamera()
		{
			GameObject gameObject = new GameObject(CUIManager.s_formCameraName);
			gameObject.transform.SetParent(this.m_uiRoot.transform, true);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			Camera camera = gameObject.AddComponent<Camera>();
			camera.orthographic = true;
			camera.orthographicSize = 50f;
			camera.clearFlags = CameraClearFlags.Depth;
			camera.cullingMask = 32;
			camera.depth = 10f;
			this.m_formCamera = camera;
		}

		private void CreateUISecene()
		{
			GameObject gameObject = new GameObject(CUIManager.s_uiSceneName);
			gameObject.transform.parent = this.m_uiRoot.transform;
		}

		private void UpdateFormRaycaster()
		{
			bool flag = true;
			for (int i = this.m_forms.Count - 1; i >= 0; i--)
			{
				if (!this.m_forms[i].m_disableInput && !this.m_forms[i].IsHided())
				{
					GraphicRaycaster graphicRaycaster = this.m_forms[i].GetGraphicRaycaster();
					if (graphicRaycaster != null)
					{
						graphicRaycaster.enabled = flag;
					}
					if (this.m_forms[i].m_isModal && flag)
					{
						flag = false;
					}
				}
			}
		}

		private void UpdateFormHided()
		{
			bool flag = false;
			for (int i = this.m_forms.Count - 1; i >= 0; i--)
			{
				if (flag)
				{
					this.m_forms[i].Hide(enFormHideFlag.HideByOtherForm, false);
				}
				else
				{
					this.m_forms[i].Appear(enFormHideFlag.HideByOtherForm, false);
				}
				if (!flag && !this.m_forms[i].IsHided() && this.m_forms[i].m_hideUnderForms)
				{
					flag = true;
				}
			}
		}

		private void OnFormPriorityChanged(CUIEvent uiEvent)
		{
			this.m_needSortForms = true;
		}

		private void OnFormVisibleChanged(CUIEvent uiEvent)
		{
			this.m_needUpdateRaycasterAndHide = true;
		}

		public void OpenTips(string strContent, bool bReadDatabin = false, float timeDuration = 1.5f, GameObject referenceGameObject = null, params object[] replaceArr)
		{
			string text = strContent;
			if (bReadDatabin)
			{
				text = Singleton<CTextManager>.GetInstance().GetText(strContent);
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (replaceArr != null)
			{
				try
				{
					text = string.Format(text, replaceArr);
				}
				catch (FormatException ex)
				{
					DebugHelper.Assert(false, "Format Exception for string \"{0}\", Exception:{1}", new object[]
					{
						text,
						ex.Message
					});
				}
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_Tips.prefab", false, false);
			if (cUIFormScript != null)
			{
				Text component = cUIFormScript.gameObject.transform.Find("Panel/Text").GetComponent<Text>();
				component.text = text;
			}
			if (cUIFormScript != null && referenceGameObject != null)
			{
				RectTransform component2 = referenceGameObject.GetComponent<RectTransform>();
				RectTransform rectTransform = cUIFormScript.gameObject.transform.Find("Panel") as RectTransform;
				if (component2 != null && rectTransform != null)
				{
					Vector3[] array = new Vector3[4];
					component2.GetWorldCorners(array);
					float num = Math.Abs(CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array[2]).x - CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array[0]).x);
					float num2 = Math.Abs(CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array[2]).y - CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array[0]).y);
					Vector2 screenPoint = new Vector2(CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array[0]).x + num / 2f, CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array[0]).y + num2 / 2f);
					rectTransform.position = CUIUtility.ScreenToWorldPoint(null, screenPoint, rectTransform.position.z);
				}
			}
			if (cUIFormScript != null)
			{
				CUITimerScript component3 = cUIFormScript.gameObject.transform.Find("Timer").GetComponent<CUITimerScript>();
				component3.EndTimer();
				component3.m_totalTime = (double)timeDuration;
				component3.StartTimer();
			}
			Singleton<CSoundManager>.instance.PostEvent("UI_Click", null);
		}

		public bool IsTipsFormExist()
		{
			return this.GetForm("UGUI/Form/Common/Form_Tips.prefab") != null;
		}

		public void CloseTips()
		{
			this.CloseForm("UGUI/Form/Common/Form_Tips.prefab");
		}

		public void OpenSendMsgAlert(int autoCloseTime = 5, enUIEventID timeUpEventId = enUIEventID.None)
		{
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Common_SendMsgAlertOpen;
			cUIEvent.m_eventParams = new stUIEventParams
			{
				tag = autoCloseTime,
				tag2 = (int)timeUpEventId
			};
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		public void OpenSendMsgAlert(string txtContent, int autoCloseTime = 10, enUIEventID timeUpEventId = enUIEventID.None)
		{
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Common_SendMsgAlertOpen;
			cUIEvent.m_eventParams = new stUIEventParams
			{
				tagStr = txtContent,
				tag = autoCloseTime,
				tag2 = (int)timeUpEventId
			};
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		public void CloseSendMsgAlert()
		{
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Common_SendMsgAlertClose;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		public void OpenMessageBox(string strContent, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, false, enUIEventID.None, enUIEventID.None, default(stUIEventParams), isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
		}

		public void OpenMessageBox(string strContent, enUIEventID confirmID, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, default(stUIEventParams), isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
		}

		public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
		}

		public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, string.Empty, string.Empty, 0, enUIEventID.None);
		}

		public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, string titleStr, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, string.Empty, titleStr, 0, enUIEventID.None);
		}

		public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, string cancleStr, string titleStr, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, cancleStr, titleStr, 0, enUIEventID.None);
		}

		public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, default(stUIEventParams), isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
		}

		public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
		}

		public void OpenMessageBoxWithCancelAndAutoClose(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, int autoCloseTime = 0, enUIEventID timeUpID = enUIEventID.None, string confirmStr = "", string cancelStr = "")
		{
			this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, confirmStr, cancelStr, string.Empty, autoCloseTime, timeUpID);
		}

		public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, string confirmStr, string cancelStr, bool isContentLeftAlign = false)
		{
			this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, default(stUIEventParams), isContentLeftAlign, confirmStr, cancelStr, string.Empty, 0, enUIEventID.None);
		}

		public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams param, string confirmStr, string cancelStr, bool isContentLeftAlign = false, string titleStr = "")
		{
			this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, param, isContentLeftAlign, confirmStr, cancelStr, titleStr, 0, enUIEventID.None);
		}

		private void OpenMessageBoxBase(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, string confirmStr = "", string cancelStr = "", string titleStr = "", int autoCloseTime = 0, enUIEventID timeUpID = enUIEventID.None)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_MessageBox.prefab", false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject gameObject = cUIFormScript.gameObject;
			if (gameObject == null)
			{
				return;
			}
			if (confirmStr == string.Empty)
			{
				confirmStr = Singleton<CTextManager>.GetInstance().GetText("Common_Confirm");
			}
			if (cancelStr == string.Empty)
			{
				cancelStr = Singleton<CTextManager>.GetInstance().GetText("Common_Cancel");
			}
			if (titleStr == string.Empty)
			{
				titleStr = Singleton<CTextManager>.GetInstance().GetText("Common_MsgBox_Title");
			}
			GameObject gameObject2 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
			gameObject2.GetComponentInChildren<Text>().text = confirmStr;
			GameObject gameObject3 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
			gameObject3.GetComponentInChildren<Text>().text = cancelStr;
			GameObject gameObject4 = gameObject.transform.Find("Panel/Panel/title/Text").gameObject;
			gameObject4.GetComponentInChildren<Text>().text = titleStr;
			Text component = gameObject.transform.Find("Panel/Panel/Text").GetComponent<Text>();
			component.text = strContent;
			if (!isHaveCancelBtn)
			{
				gameObject3.CustomSetActive(false);
			}
			else
			{
				gameObject3.CustomSetActive(true);
			}
			CUIEventScript component2 = gameObject2.GetComponent<CUIEventScript>();
			CUIEventScript component3 = gameObject3.GetComponent<CUIEventScript>();
			component2.SetUIEvent(enUIEventType.Click, confirmID, par);
			component3.SetUIEvent(enUIEventType.Click, cancelID, par);
			if (isContentLeftAlign)
			{
				component.alignment = TextAnchor.MiddleLeft;
			}
			if (autoCloseTime != 0)
			{
				Transform transform = cUIFormScript.transform.Find("closeTimer");
				if (transform != null)
				{
					CUITimerScript component4 = transform.GetComponent<CUITimerScript>();
					if (component4 != null)
					{
						component4.SetTotalTime((float)autoCloseTime);
						component4.StartTimer();
						component4.m_eventIDs[1] = timeUpID;
						component4.m_eventParams[1] = par;
					}
				}
			}
			this.CloseSendMsgAlert();
		}

		public void CloseMessageBox()
		{
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_MessageBox.prefab");
		}

		public void OpenSmallMessageBox(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, int autoCloseTime = 0, enUIEventID closeTimeID = enUIEventID.None, string confirmStr = "", string cancelStr = "", bool isContentLeftAlign = false)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_SmallMessageBox.prefab", false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject gameObject = cUIFormScript.gameObject;
			if (gameObject == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(confirmStr))
			{
				confirmStr = Singleton<CTextManager>.GetInstance().GetText("Common_Confirm");
			}
			if (string.IsNullOrEmpty(cancelStr))
			{
				cancelStr = Singleton<CTextManager>.GetInstance().GetText("Common_Cancel");
			}
			GameObject gameObject2 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
			gameObject2.GetComponentInChildren<Text>().text = confirmStr;
			GameObject gameObject3 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
			gameObject3.GetComponentInChildren<Text>().text = cancelStr;
			Text component = gameObject.transform.Find("Panel/Panel/Text").GetComponent<Text>();
			component.text = strContent;
			if (!isHaveCancelBtn)
			{
				gameObject3.CustomSetActive(false);
			}
			else
			{
				gameObject3.CustomSetActive(true);
			}
			CUIEventScript component2 = gameObject2.GetComponent<CUIEventScript>();
			CUIEventScript component3 = gameObject3.GetComponent<CUIEventScript>();
			component2.SetUIEvent(enUIEventType.Click, confirmID, par);
			component3.SetUIEvent(enUIEventType.Click, cancelID, par);
			if (isContentLeftAlign)
			{
				component.alignment = TextAnchor.MiddleLeft;
			}
			if (autoCloseTime != 0)
			{
				Transform transform = cUIFormScript.transform.Find("closeTimer");
				if (transform != null)
				{
					CUITimerScript component4 = transform.GetComponent<CUITimerScript>();
					if (component4 != null)
					{
						if (closeTimeID > enUIEventID.None)
						{
							component4.m_eventIDs[1] = closeTimeID;
						}
						component4.SetTotalTime((float)autoCloseTime);
						component4.StartTimer();
					}
				}
			}
			this.CloseSendMsgAlert();
		}

		public void CloseSmallMessageBox()
		{
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_SmallMessageBox.prefab");
		}

		public void OpenMessageBoxBig(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, string confirmStr = "", string cancelStr = "", string titleStr = "", int autoCloseTime = 0, enUIEventID timeUpID = enUIEventID.None)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_MessageBoxBig.prefab", false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject gameObject = cUIFormScript.gameObject;
			if (gameObject == null)
			{
				return;
			}
			if (confirmStr == string.Empty)
			{
				confirmStr = Singleton<CTextManager>.GetInstance().GetText("Common_Confirm");
			}
			if (cancelStr == string.Empty)
			{
				cancelStr = Singleton<CTextManager>.GetInstance().GetText("Common_Cancel");
			}
			if (titleStr == string.Empty)
			{
				titleStr = Singleton<CTextManager>.GetInstance().GetText("Common_MsgBox_Title");
			}
			GameObject gameObject2 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
			gameObject2.GetComponentInChildren<Text>().text = confirmStr;
			GameObject gameObject3 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
			gameObject3.GetComponentInChildren<Text>().text = cancelStr;
			GameObject gameObject4 = gameObject.transform.Find("Panel/Panel/title/Text").gameObject;
			gameObject4.GetComponentInChildren<Text>().text = titleStr;
			Text component = gameObject.transform.Find("Panel/Panel/ScrollRect/Text").GetComponent<Text>();
			component.text = strContent;
			if (!isHaveCancelBtn)
			{
				gameObject3.CustomSetActive(false);
			}
			else
			{
				gameObject3.CustomSetActive(true);
			}
			CUIEventScript component2 = gameObject2.GetComponent<CUIEventScript>();
			CUIEventScript component3 = gameObject3.GetComponent<CUIEventScript>();
			component2.SetUIEvent(enUIEventType.Click, confirmID, par);
			component3.SetUIEvent(enUIEventType.Click, cancelID, par);
			if (isContentLeftAlign)
			{
				component.alignment = TextAnchor.MiddleLeft;
			}
			if (autoCloseTime != 0)
			{
				Transform transform = cUIFormScript.transform.Find("closeTimer");
				if (transform != null)
				{
					CUITimerScript component4 = transform.GetComponent<CUITimerScript>();
					if (component4 != null)
					{
						component4.SetTotalTime((float)autoCloseTime);
						component4.StartTimer();
						component4.m_eventIDs[1] = timeUpID;
						component4.m_eventParams[1] = par;
					}
				}
			}
			this.CloseSendMsgAlert();
		}

		public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, stUIEventParams par)
		{
			this.OpenInputBoxBase(title, inputTip, confirmID, enUIEventID.None, par, "确定", "取消");
		}

		public void OpenInputBox(string title, string inputTip, enUIEventID confirmID)
		{
			this.OpenInputBoxBase(title, inputTip, confirmID, enUIEventID.None, default(stUIEventParams), "确定", "取消");
		}

		public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par)
		{
			this.OpenInputBoxBase(title, inputTip, confirmID, cancelID, par, "确定", "取消");
		}

		public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, string confirmStr, string cancelStr)
		{
			this.OpenInputBoxBase(title, inputTip, confirmID, cancelID, par, confirmStr, cancelStr);
		}

		private void OpenInputBoxBase(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, string confirmStr = "确定", string cancelStr = "取消")
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_InputBox.prefab", false, false);
			GameObject gameObject = null;
			if (cUIFormScript != null)
			{
				gameObject = cUIFormScript.gameObject;
			}
			if (gameObject != null)
			{
				GameObject gameObject2 = gameObject.transform.Find("Panel/btnGroup/Button_Confirm").gameObject;
				gameObject2.GetComponentInChildren<Text>().text = confirmStr;
				GameObject gameObject3 = gameObject.transform.Find("Panel/btnGroup/Button_Cancel").gameObject;
				gameObject3.GetComponentInChildren<Text>().text = cancelStr;
				Text component = gameObject.transform.Find("Panel/title/Text").GetComponent<Text>();
				component.text = title;
				Text component2 = gameObject.transform.Find("Panel/inputText/Placeholder").GetComponent<Text>();
				component2.text = inputTip;
				CUIEventScript component3 = gameObject2.GetComponent<CUIEventScript>();
				CUIEventScript component4 = gameObject3.GetComponent<CUIEventScript>();
				component3.SetUIEvent(enUIEventType.Click, confirmID, par);
				component4.SetUIEvent(enUIEventType.Click, cancelID, par);
			}
		}

		public void OpenAwardTip(CUseable[] items, string title = null, bool playSound = false, enUIEventID eventID = enUIEventID.None, bool displayAll = false, bool forceNotGoToBag = false, string formPath = "Form_Award")
		{
			if (items == null)
			{
				return;
			}
			int b = 10;
			int num = Mathf.Min(items.Length, b);
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/" + formPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			cUIFormScript.transform.FindChild("btnGroup/Button_Back").GetComponent<CUIEventScript>().m_onClickEventID = eventID;
			if (title != null)
			{
				Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "bg/Title").text = title;
			}
			CUIListScript component = cUIFormScript.transform.FindChild("IconContainer").gameObject.GetComponent<CUIListScript>();
			component.SetElementAmount(num);
			for (int i = 0; i < num; i++)
			{
				if (!(component.GetElemenet(i) == null) && items[i] != null)
				{
					GameObject gameObject = component.GetElemenet(i).gameObject;
					CUICommonSystem.SetItemCell(cUIFormScript, gameObject, items[i], true, displayAll, false, false);
					gameObject.CustomSetActive(true);
					gameObject.transform.FindChild("ItemName").GetComponent<Text>().text = items[i].m_name;
					if (playSound)
					{
						COM_REWARDS_TYPE mapRewardType = items[i].MapRewardType;
						if (mapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN)
						{
							if (mapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP)
							{
								if (mapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND)
								{
									Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_diamond", null);
								}
							}
							else
							{
								Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_physical_power", null);
							}
						}
						else
						{
							Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_coin", null);
						}
					}
				}
			}
			CUIEventScript component2 = cUIFormScript.transform.Find("btnGroup/Button_Use").GetComponent<CUIEventScript>();
			component2.gameObject.CustomSetActive(false);
			if (!forceNotGoToBag && num == 1 && items[0].m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				CItem cItem = items[0] as CItem;
				if (cItem.m_itemData.bType == 4 || cItem.m_itemData.bType == 1 || cItem.m_itemData.bType == 11)
				{
					CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
					CUseable useableByBaseID = useableContainer.GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, cItem.m_baseID);
					if (useableByBaseID != null)
					{
						component2.gameObject.CustomSetActive(true);
						component2.m_onClickEventParams.iconUseable = useableByBaseID;
						component2.m_onClickEventParams.tag = Mathf.Min(cItem.m_stackCount, useableByBaseID.m_stackCount);
					}
				}
			}
		}

		public void OpenInfoForm(int txtKey)
		{
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long)txtKey);
			if (dataByKey != null)
			{
				string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
				string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
				Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
			}
		}

		public string GetRuleTextContent(int txtKey)
		{
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long)txtKey);
			if (dataByKey != null)
			{
				return StringHelper.UTF8BytesToString(ref dataByKey.szContent);
			}
			return null;
		}

		public string GetRuleTextTitle(int txtKey)
		{
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long)txtKey);
			if (dataByKey != null)
			{
				return StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
			}
			return null;
		}

		public void OpenInfoForm(string title = null, string info = null)
		{
			CUIFormScript cUIFormScript = this.OpenForm("UGUI/Form/Common/Form_Info.prefab", false, true);
			DebugHelper.Assert(cUIFormScript != null, "CUIManager.OpenInfoForm(): form == null!!!");
			if (cUIFormScript == null)
			{
				return;
			}
			if (title != null)
			{
				Text component = cUIFormScript.GetWidget(0).GetComponent<Text>();
				component.text = title;
			}
			if (info != null)
			{
				Text component2 = cUIFormScript.GetWidget(1).GetComponent<Text>();
				component2.text = info;
			}
		}

		public void OpenEditForm(string title, string editContent, enUIEventID confirmEventId = enUIEventID.None)
		{
			CUIFormScript cUIFormScript = this.OpenForm("UGUI/Form/Common/Form_Edit.prefab", false, true);
			DebugHelper.Assert(cUIFormScript != null, "CUIManager.OpenEditForm(): form == null!!!");
			if (cUIFormScript == null)
			{
				return;
			}
			if (title != null)
			{
				Text component = cUIFormScript.GetWidget(0).GetComponent<Text>();
				component.text = title;
			}
			if (editContent != null)
			{
				Text component2 = cUIFormScript.GetWidget(1).GetComponent<Text>();
				component2.text = editContent;
			}
			CUIEventScript component3 = cUIFormScript.GetWidget(2).GetComponent<CUIEventScript>();
			component3.SetUIEvent(enUIEventType.Click, confirmEventId);
		}

		public void LoadUIScenePrefab(string sceneName, CUIFormScript formScript)
		{
			if (formScript == null)
			{
				return;
			}
			if (formScript.IsRelatedSceneExist(sceneName))
			{
				return;
			}
			formScript.AddRelatedScene(CUICommonSystem.GetAnimation3DOjb(sceneName), sceneName);
		}

		public void OpenExchangeCountSelectForm(CUseable item, int count, enUIEventID confirmId, stUIEventParams par, uint price = 0u, uint totalPrice = 0u)
		{
			if (item == null)
			{
				return;
			}
			this._curMaxExchangeCount = Math.Min(count, 300);
			this._confirmId = confirmId;
			this._curValidIndex = par.commonUInt16Param1;
			this._price = price;
			this._totalPirce = totalPrice;
			CUIFormScript cUIFormScript = this.OpenForm("UGUI/Form/Common/Form_ExchangeCountSelect.prefab", false, true);
			if (cUIFormScript != null)
			{
				Text component = cUIFormScript.gameObject.transform.FindChild("Panel/Count").gameObject.GetComponent<Text>();
				component.text = 1.ToString();
				Text component2 = cUIFormScript.gameObject.transform.FindChild("Panel/Jifen").gameObject.GetComponent<Text>();
				if (this._price > 0u && this._totalPirce >= this._price)
				{
					component2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeNumSelectPoint"), this._price, this._totalPirce);
					component2.gameObject.CustomSetActive(true);
				}
				else
				{
					component2.gameObject.CustomSetActive(false);
				}
				Button component3 = cUIFormScript.gameObject.transform.FindChild("Panel/Button_Count_Down").gameObject.GetComponent<Button>();
				Button component4 = cUIFormScript.gameObject.transform.FindChild("Panel/Button_Count_Up").gameObject.GetComponent<Button>();
				CUICommonSystem.SetButtonEnable(component3, false, false, true);
				CUICommonSystem.SetButtonEnable(component4, true, true, true);
				Image component5 = cUIFormScript.gameObject.transform.FindChild("Panel/Slot/Icon").gameObject.GetComponent<Image>();
				CUIUtility.SetImageSprite(component5, item.GetIconPath(), cUIFormScript, false, false, false, false);
				Text component6 = cUIFormScript.gameObject.transform.FindChild("Panel/Name").gameObject.GetComponent<Text>();
				component6.text = item.m_name;
				Text component7 = cUIFormScript.gameObject.transform.FindChild("Panel/Slot/lblIconCount").gameObject.GetComponent<Text>();
				component7.text = item.m_stackCount.ToString();
				component7.gameObject.CustomSetActive(true);
				CUIEventScript component8 = cUIFormScript.gameObject.transform.FindChild("Panel/Button_Exchange").gameObject.GetComponent<CUIEventScript>();
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.commonUInt32Param1 = 1u;
				eventParams.commonUInt16Param1 = this._curValidIndex;
				component8.SetUIEvent(enUIEventType.Click, this._confirmId, eventParams);
			}
		}

		private void OnAddExchangeCount(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.gameObject.transform.FindChild("Panel/Count").gameObject.GetComponent<Text>();
			if (component != null)
			{
				int num = int.Parse(component.text);
				num++;
				num = Math.Min(Math.Min(num, this._curMaxExchangeCount), 300);
				component.text = num.ToString();
				Text component2 = srcFormScript.gameObject.transform.FindChild("Panel/Jifen").gameObject.GetComponent<Text>();
				if (this._price > 0u && this._totalPirce >= this._price)
				{
					component2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeNumSelectPoint"), (long)((ulong)this._price * (ulong)((long)num)), this._totalPirce);
					component2.gameObject.CustomSetActive(true);
				}
				else
				{
					component2.gameObject.CustomSetActive(false);
				}
				Button component3 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Count_Down").gameObject.GetComponent<Button>();
				Button component4 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Count_Up").gameObject.GetComponent<Button>();
				if (num == 1)
				{
					CUICommonSystem.SetButtonEnable(component3, false, false, true);
					CUICommonSystem.SetButtonEnable(component4, true, true, true);
				}
				else if (num == this._curMaxExchangeCount)
				{
					CUICommonSystem.SetButtonEnable(component3, true, true, true);
					CUICommonSystem.SetButtonEnable(component4, false, false, true);
				}
				else
				{
					CUICommonSystem.SetButtonEnable(component3, true, true, true);
					CUICommonSystem.SetButtonEnable(component4, true, true, true);
				}
				CUIEventScript component5 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Exchange").gameObject.GetComponent<CUIEventScript>();
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.commonUInt32Param1 = (uint)num;
				eventParams.commonUInt16Param1 = this._curValidIndex;
				component5.SetUIEvent(enUIEventType.Click, this._confirmId, eventParams);
			}
		}

		private void OnDecreaseExchangeCount(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.gameObject.transform.FindChild("Panel/Count").gameObject.GetComponent<Text>();
			if (component != null)
			{
				int num = int.Parse(component.text);
				num--;
				num = Math.Max(num, 1);
				component.text = num.ToString();
				Text component2 = srcFormScript.gameObject.transform.FindChild("Panel/Jifen").gameObject.GetComponent<Text>();
				if (this._price > 0u && this._totalPirce >= this._price)
				{
					component2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeNumSelectPoint"), (long)((ulong)this._price * (ulong)((long)num)), this._totalPirce);
					component2.gameObject.CustomSetActive(true);
				}
				else
				{
					component2.gameObject.CustomSetActive(false);
				}
				Button component3 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Count_Down").gameObject.GetComponent<Button>();
				Button component4 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Count_Up").gameObject.GetComponent<Button>();
				if (num == 1)
				{
					CUICommonSystem.SetButtonEnable(component3, false, false, true);
					CUICommonSystem.SetButtonEnable(component4, true, true, true);
				}
				else if (num == this._curMaxExchangeCount)
				{
					CUICommonSystem.SetButtonEnable(component3, true, true, true);
					CUICommonSystem.SetButtonEnable(component4, false, false, true);
				}
				else
				{
					CUICommonSystem.SetButtonEnable(component3, true, true, true);
					CUICommonSystem.SetButtonEnable(component4, true, true, true);
				}
				CUIEventScript component5 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Exchange").gameObject.GetComponent<CUIEventScript>();
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.commonUInt32Param1 = (uint)num;
				eventParams.commonUInt16Param1 = this._curValidIndex;
				component5.SetUIEvent(enUIEventType.Click, this._confirmId, eventParams);
			}
		}

		private void OnMaxExchangeCount(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.gameObject.transform.FindChild("Panel/Count").gameObject.GetComponent<Text>();
			if (component != null)
			{
				component.text = this._curMaxExchangeCount.ToString();
				Text component2 = srcFormScript.gameObject.transform.FindChild("Panel/Jifen").gameObject.GetComponent<Text>();
				if (this._price > 0u && this._totalPirce >= this._price)
				{
					component2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeNumSelectPoint"), (long)((ulong)this._price * (ulong)((long)this._curMaxExchangeCount)), this._totalPirce);
					component2.gameObject.CustomSetActive(true);
				}
				else
				{
					component2.gameObject.CustomSetActive(false);
				}
				Button component3 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Count_Down").gameObject.GetComponent<Button>();
				Button component4 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Count_Up").gameObject.GetComponent<Button>();
				CUICommonSystem.SetButtonEnable(component3, true, true, true);
				CUICommonSystem.SetButtonEnable(component4, false, false, true);
				CUIEventScript component5 = srcFormScript.gameObject.transform.FindChild("Panel/Button_Exchange").gameObject.GetComponent<CUIEventScript>();
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.commonUInt32Param1 = (uint)this._curMaxExchangeCount;
				eventParams.commonUInt16Param1 = this._curValidIndex;
				component5.SetUIEvent(enUIEventType.Click, this._confirmId, eventParams);
			}
		}

		public bool OpenSearchBox(string title, string recommendItemPath, string resultItemPath, uint recommendHandleCMD, uint searchHandleCMD, string evtCallbackSearch, string evtCallbackRecommend, string evtCallbackRecommendSingleEnable = null)
		{
			CUIFormScript cUIFormScript = this.GetForm("UGUI/Form/Common/Search/Form_Search.prefab");
			if (cUIFormScript != null)
			{
				return false;
			}
			cUIFormScript = this.OpenForm("UGUI/Form/Common/Search/Form_Search.prefab", false, true);
			GameObject widget = cUIFormScript.GetWidget(2);
			if (title != null)
			{
				widget.GetComponent<Text>().text = title;
			}
			this.m_searchResultGo = cUIFormScript.GetWidget(0);
			this.m_searchRecommendGo = cUIFormScript.GetWidget(1);
			GameObject gameObject = (GameObject)Singleton<CResourceManager>.GetInstance().GetResource(recommendItemPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content;
			gameObject.transform.parent = this.m_searchRecommendGo.transform;
			Transform transform = this.m_searchResultGo.transform.Find("Result");
			this.m_deltaSearchResultHeight = (gameObject.transform as RectTransform).rect.height - (transform as RectTransform).rect.height;
			this.m_searchResultGo.CustomSetActive(false);
			this.m_searchRecommendGo.CustomSetActive(false);
			NetMsgDelegate handler = new NetMsgDelegate(this.SearchBox_ResultHandler);
			this.m_searchHandlerCMD = searchHandleCMD;
			Singleton<NetworkModule>.GetInstance().RegisterMsgHandler(searchHandleCMD, handler);
			handler = new NetMsgDelegate(this.SearchBox_RecommendHandler);
			this.m_recommendHandlerCMD = recommendHandleCMD;
			Singleton<NetworkModule>.GetInstance().RegisterMsgHandler(recommendHandleCMD, handler);
			this.m_searchEvtCallBack = evtCallbackSearch;
			this.m_recommendEvtCallBack = evtCallbackRecommend;
			this.m_recommendEvtCallBackSingleEnable = evtCallbackRecommendSingleEnable;
			return true;
		}

		public void SearchBox_ResultHandler(CSPkg msg)
		{
			if (this.m_searchEvtCallBack != null)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg, GameObject>(this.m_searchEvtCallBack, msg, this.m_searchResultGo);
			}
			if (this.m_searchResultGo.activeInHierarchy)
			{
				Transform transform = this.m_searchResultGo.transform;
				RectTransform rectTransform = transform as RectTransform;
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + this.m_deltaSearchResultHeight);
				Transform transform2 = this.m_searchRecommendGo.transform;
				RectTransform rectTransform2 = transform2 as RectTransform;
				rectTransform2.anchoredPosition = new Vector2(rectTransform2.anchoredPosition.x, rectTransform2.anchoredPosition.y + this.m_deltaSearchResultHeight);
				rectTransform2.sizeDelta = new Vector2(rectTransform2.sizeDelta.x, rectTransform2.sizeDelta.y - this.m_deltaSearchResultHeight);
			}
		}

		public void SearchBox_RecommendHandler(CSPkg msg)
		{
			if (this.m_recommendEvtCallBack != null)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg, GameObject>(this.m_recommendEvtCallBack, msg, this.m_searchRecommendGo);
			}
		}

		public void SearchBox_OnRecommendListEnable(CUIEvent evt)
		{
			if (this.m_recommendEvtCallBackSingleEnable != null)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<GameObject, CUIEvent>(this.m_recommendEvtCallBackSingleEnable, this.m_searchRecommendGo, evt);
			}
		}

		public void SearchBox_OnClose(CUIEvent evt)
		{
			Singleton<NetworkModule>.GetInstance().RemoveMsgHandler(this.m_searchHandlerCMD);
			Singleton<NetworkModule>.GetInstance().RemoveMsgHandler(this.m_recommendHandlerCMD);
			this.m_searchEvtCallBack = null;
			this.m_recommendEvtCallBack = null;
			this.m_recommendEvtCallBackSingleEnable = null;
		}

		public void OpenStringSenderBox(string title, string desc, string stringPlacer, CUIManager.StringSendboxOnSend onSendCallback, string defaultString = "")
		{
			CUIFormScript cUIFormScript = this.GetForm("UGUI/Form/Common/Search/Form_StringSender.prefab");
			if (cUIFormScript != null)
			{
				return;
			}
			cUIFormScript = this.OpenForm("UGUI/Form/Common/Search/Form_StringSender.prefab", true, true);
			if (title != null)
			{
				GameObject widget = cUIFormScript.GetWidget(2);
				widget.GetComponent<Text>().text = title;
			}
			if (desc != null)
			{
				GameObject widget2 = cUIFormScript.GetWidget(1);
				widget2.GetComponent<Text>().text = desc;
			}
			if (stringPlacer != null)
			{
				GameObject widget3 = cUIFormScript.GetWidget(3);
				widget3.GetComponent<Text>().text = stringPlacer;
			}
			GameObject widget4 = cUIFormScript.GetWidget(0);
			widget4.GetComponent<InputField>().text = defaultString;
			this.m_strSendboxCb = onSendCallback;
		}

		public void OnStringSenderBoxSend(CUIEvent evt)
		{
			if (this.m_strSendboxCb != null)
			{
				Text component = evt.m_srcFormScript.GetWidget(0).transform.Find("Text").GetComponent<Text>();
				this.m_strSendboxCb(component.text);
			}
			this.CloseForm(evt.m_srcFormScript);
		}
	}
}
