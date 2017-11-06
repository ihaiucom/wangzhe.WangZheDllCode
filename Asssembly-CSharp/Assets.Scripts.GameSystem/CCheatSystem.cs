using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CCheatSystem : Singleton<CCheatSystem>
	{
		public delegate void OnDisable();

		public bool m_enabled;

		private bool m_cheatFormHasBeenOpend;

		private List<GameObject> m_triggers = new List<GameObject>();

		private static string s_cheatTriggerFormPath = "UGUI/Form/System/Cheat/Form_CheatTrigger.prefab";

		private static string s_cheatFormPath = "UGUI/Form/System/Cheat/Form_Cheat.prefab";

		private static string s_cheatErrorLogFormPath = "UGUI/Form/System/Cheat/Form_CheatErrorLog.prefab";

		private static string[] s_tversionServerTypeName = new string[]
		{
			"正式服",
			"中转服",
			"测试服",
			"体验版正式服",
			"体验版中转服",
			"测试专用服",
			"比赛正式服",
			"比赛测试服",
			"跳过更新"
		};

		private static string[] s_TDirServerTypeName = new string[]
		{
			"使用默认",
			"测试服",
			"中转服",
			"正式服",
			"体验版正式服",
			"体验版中转服",
			"测试专用服",
			"比赛测试服",
			"比赛正式服"
		};

		private static string[] s_errorLogFlagName = new string[]
		{
			"显示ErrorLog",
			"输出更新日志"
		};

		private static string[] s_ignoreMaintainName = new string[]
		{
			"不选可以忽略维护状态"
		};

		private static string[] s_joystickConfigNames = new string[]
		{
			"强制跟随移动",
			"增强重连显示"
		};

		private static bool[] s_errorLogFlags = new bool[CCheatSystem.s_errorLogFlagName.Length];

		private static bool[] s_joystickConfigs = new bool[CCheatSystem.s_joystickConfigNames.Length];

		private CCheatSystem.OnDisable m_onDisable;

		private string m_errorLog = string.Empty;

		private static string s_rmsJoystickForceMoveable = "RMS_JoystickForceMoveable";

		public override void Init()
		{
			CCheatSystem.s_joystickConfigs[0] = (PlayerPrefs.GetInt(CCheatSystem.s_rmsJoystickForceMoveable, 0) == 1);
			CCheatSystem.s_joystickConfigs[1] = false;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_TriggerDown, new CUIEventManager.OnUIEventHandler(this.OnCheatTriggerDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_TriggerUp, new CUIEventManager.OnUIEventHandler(this.OnCheatTriggerUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_OnIIPSServerSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnCheatOnIIPSServerSelectChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_CheatFormClosed, new CUIEventManager.OnUIEventHandler(this.OnCheatFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_ClearCache, new CUIEventManager.OnUIEventHandler(this.OnClearCache));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_TDirChanged, new CUIEventManager.OnUIEventHandler(this.OnCheatOnTDirSelectChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_OnErrorLogSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnCheatErrorLogSelectChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_HideErrorLogPanel, new CUIEventManager.OnUIEventHandler(this.OnCheatHideErrorLogPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_AppearErrorLogPanel, new CUIEventManager.OnUIEventHandler(this.OnCheatAppearErrorLogPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_ClearErrorLog, new CUIEventManager.OnUIEventHandler(this.OnCheatClearErrorLog));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_MaintainBlock, new CUIEventManager.OnUIEventHandler(this.OnCheatMaintainBlock));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_JoystickConfigChanged, new CUIEventManager.OnUIEventHandler(this.OnCheatJoystickConfigChanged));
		}

		public override void UnInit()
		{
		}

		public bool IsDisplayErrorLog()
		{
			return CCheatSystem.s_errorLogFlags[0];
		}

		public void RecordErrorLog(string errorLog)
		{
			if (!this.IsDisplayErrorLog())
			{
				return;
			}
			this.m_errorLog += errorLog;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CCheatSystem.s_cheatErrorLogFormPath, false, false);
			if (cUIFormScript != null)
			{
				Text component = cUIFormScript.GetWidget(1).GetComponent<Text>();
				if (component != null)
				{
					component.set_text(this.m_errorLog);
				}
			}
		}

		public static bool IsJoystickForceMoveable()
		{
			return CCheatSystem.s_joystickConfigs[0];
		}

		public static bool IsInhanceReconnect()
		{
			return CCheatSystem.s_joystickConfigs[1];
		}

		private void OnCheatTriggerDown(CUIEvent uiEvent)
		{
			if (!this.m_triggers.Contains(uiEvent.m_srcWidget))
			{
				this.m_triggers.Add(uiEvent.m_srcWidget);
			}
			if (this.m_triggers.get_Count() >= 5)
			{
				if (!this.m_cheatFormHasBeenOpend)
				{
					this.OpenCheatForm();
					this.m_cheatFormHasBeenOpend = true;
				}
				this.m_triggers.Clear();
			}
		}

		private void OnCheatTriggerUp(CUIEvent uiEvent)
		{
			if (this.m_triggers.Contains(uiEvent.m_srcWidget))
			{
				this.m_triggers.Remove(uiEvent.m_srcWidget);
			}
		}

		private void OnCheatOnIIPSServerSelectChanged(CUIEvent uiEvent)
		{
			enIIPSServerType enIIPSServerType = (enIIPSServerType)((CUIToggleListScript)uiEvent.m_srcWidgetScript).GetSelected();
			if (enIIPSServerType > enIIPSServerType.None)
			{
				enIIPSServerType = enIIPSServerType.Test;
				((CUIToggleListScript)uiEvent.m_srcWidgetScript).SetSelected((int)enIIPSServerType);
			}
			CVersionUpdateSystem.SetIIPSServerType(enIIPSServerType);
		}

		private void OnCheatOnTDirSelectChanged(CUIEvent uiEvent)
		{
			TdirConfig.cheatServerType = (TdirServerType)((CUIToggleListScript)uiEvent.m_srcWidgetScript).GetSelected();
		}

		private void OnCheatErrorLogSelectChanged(CUIEvent uiEvent)
		{
			bool[] multiSelected = ((CUIToggleListScript)uiEvent.m_srcWidgetScript).GetMultiSelected();
			for (int i = 0; i < CCheatSystem.s_errorLogFlags.Length; i++)
			{
				CCheatSystem.s_errorLogFlags[i] = multiSelected[i];
				if (i == 1)
				{
					CVersionUpdateSystem.EnableLogDebug(CCheatSystem.s_errorLogFlags[i]);
				}
			}
		}

		private void OnCheatMaintainBlock(CUIEvent uiEvent)
		{
			bool[] multiSelected = ((CUIToggleListScript)uiEvent.m_srcWidgetScript).GetMultiSelected();
			if (multiSelected.Length > 0)
			{
				TdirMgr.s_maintainBlock = multiSelected[0];
			}
		}

		private void OnCheatJoystickConfigChanged(CUIEvent uiEvent)
		{
			bool[] multiSelected = ((CUIToggleListScript)uiEvent.m_srcWidgetScript).GetMultiSelected();
			for (int i = 0; i < multiSelected.Length; i++)
			{
				CCheatSystem.s_joystickConfigs[i] = multiSelected[i];
				if (i == 0)
				{
					PlayerPrefs.SetInt(CCheatSystem.s_rmsJoystickForceMoveable, CCheatSystem.s_joystickConfigs[i] ? 1 : 0);
				}
			}
			PlayerPrefs.Save();
		}

		private void OnCheatHideErrorLogPanel(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			srcFormScript.GetWidget(0).CustomSetActive(false);
			srcFormScript.GetWidget(2).CustomSetActive(false);
			srcFormScript.GetWidget(3).CustomSetActive(true);
		}

		private void OnCheatAppearErrorLogPanel(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			srcFormScript.GetWidget(0).CustomSetActive(true);
			srcFormScript.GetWidget(2).CustomSetActive(true);
			srcFormScript.GetWidget(3).CustomSetActive(false);
		}

		private void OnCheatClearErrorLog(CUIEvent uiEvent)
		{
			this.m_errorLog = string.Empty;
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.GetWidget(1).GetComponent<Text>();
			if (component != null)
			{
				component.set_text(string.Empty);
			}
		}

		private void OnCheatFormClosed(CUIEvent uiEvent)
		{
			this.m_enabled = false;
			if (this.m_onDisable != null)
			{
				this.m_onDisable();
			}
		}

		private void OnClearCache(CUIEvent uiEvent)
		{
			if (MonoSingleton<CVersionUpdateSystem>.GetInstance().ClearCachePath())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("缓存清理成功！", false, 1.5f, null, new object[0]);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("缓存清理失败！", false, 1.5f, null, new object[0]);
			}
		}

		public void OpenCheatTriggerForm(CCheatSystem.OnDisable onDisable)
		{
			Singleton<CUIManager>.GetInstance().OpenForm(CCheatSystem.s_cheatTriggerFormPath, false, false);
			this.m_onDisable = onDisable;
		}

		public void CloseCheatTriggerForm()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CCheatSystem.s_cheatTriggerFormPath);
		}

		private void OpenCheatForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CCheatSystem.s_cheatFormPath, false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			this.m_enabled = true;
			GameObject widget = cUIFormScript.GetWidget(0);
			if (widget != null)
			{
				CUIToggleListScript component = widget.GetComponent<CUIToggleListScript>();
				if (component != null)
				{
					component.SetElementAmount(CCheatSystem.s_tversionServerTypeName.Length);
					for (int i = 0; i < CCheatSystem.s_tversionServerTypeName.Length; i++)
					{
						CUIListElementScript elemenet = component.GetElemenet(i);
						Transform transform = elemenet.gameObject.transform.Find("Label");
						if (transform != null)
						{
							Text component2 = transform.gameObject.GetComponent<Text>();
							if (component2 != null)
							{
								component2.set_text(CCheatSystem.s_tversionServerTypeName[i]);
								if (i == 8)
								{
									component2.set_color(new Color(1f, 0f, 0f, 1f));
								}
							}
						}
					}
					component.SetSelected((int)CVersionUpdateSystem.GetIIPSServerType());
				}
			}
			GameObject widget2 = cUIFormScript.GetWidget(1);
			if (widget2 != null)
			{
				CUIToggleListScript component3 = widget2.GetComponent<CUIToggleListScript>();
				if (component3 != null)
				{
					component3.SetElementAmount(CCheatSystem.s_TDirServerTypeName.Length);
					for (int j = 0; j < CCheatSystem.s_TDirServerTypeName.Length; j++)
					{
						CUIListElementScript elemenet2 = component3.GetElemenet(j);
						Transform transform2 = elemenet2.gameObject.transform.Find("Label");
						if (transform2 != null)
						{
							Text component4 = transform2.gameObject.GetComponent<Text>();
							if (component4 != null)
							{
								component4.set_text(CCheatSystem.s_TDirServerTypeName[j]);
								if (j == 0)
								{
									component4.set_color(new Color(1f, 0f, 0f, 1f));
								}
							}
						}
					}
					component3.SetSelected((int)TdirConfig.cheatServerType);
				}
			}
			GameObject widget3 = cUIFormScript.GetWidget(2);
			if (widget3 != null)
			{
				CUIToggleListScript component5 = widget3.GetComponent<CUIToggleListScript>();
				if (component5 != null)
				{
					component5.SetElementAmount(CCheatSystem.s_errorLogFlagName.Length);
					for (int k = 0; k < CCheatSystem.s_errorLogFlagName.Length; k++)
					{
						CUIListElementScript elemenet3 = component5.GetElemenet(k);
						Transform transform3 = elemenet3.gameObject.transform.Find("Label");
						if (transform3 != null)
						{
							Text component6 = transform3.gameObject.GetComponent<Text>();
							if (component6 != null)
							{
								component6.set_text(CCheatSystem.s_errorLogFlagName[k]);
							}
						}
					}
					CCheatSystem.s_errorLogFlags[1] = CVersionUpdateSystem.IsEnableLogDebug();
					for (int l = 0; l < CCheatSystem.s_errorLogFlags.Length; l++)
					{
						component5.SetMultiSelected(l, CCheatSystem.s_errorLogFlags[l]);
					}
				}
			}
			GameObject widget4 = cUIFormScript.GetWidget(3);
			if (widget4 != null)
			{
				CUIToggleListScript component7 = widget4.GetComponent<CUIToggleListScript>();
				if (component7 != null)
				{
					component7.SetElementAmount(CCheatSystem.s_ignoreMaintainName.Length);
					for (int m = 0; m < CCheatSystem.s_ignoreMaintainName.Length; m++)
					{
						CUIListElementScript elemenet4 = component7.GetElemenet(m);
						Transform transform4 = elemenet4.gameObject.transform.Find("Label");
						if (transform4 != null)
						{
							Text component8 = transform4.gameObject.GetComponent<Text>();
							if (component8 != null)
							{
								component8.set_text(CCheatSystem.s_ignoreMaintainName[m]);
							}
						}
					}
					component7.SetMultiSelected(0, TdirMgr.s_maintainBlock);
				}
			}
			GameObject widget5 = cUIFormScript.GetWidget(4);
			if (widget5 != null)
			{
				CUIToggleListScript component9 = widget5.GetComponent<CUIToggleListScript>();
				if (component9 != null)
				{
					component9.SetElementAmount(CCheatSystem.s_joystickConfigNames.Length);
					for (int n = 0; n < CCheatSystem.s_joystickConfigNames.Length; n++)
					{
						CUIListElementScript elemenet5 = component9.GetElemenet(n);
						Transform transform5 = elemenet5.gameObject.transform.Find("Label");
						if (transform5 != null)
						{
							Text component10 = transform5.gameObject.GetComponent<Text>();
							if (component10 != null)
							{
								component10.set_text(CCheatSystem.s_joystickConfigNames[n]);
							}
						}
						component9.SetMultiSelected(n, CCheatSystem.s_joystickConfigs[n]);
					}
				}
			}
		}
	}
}
