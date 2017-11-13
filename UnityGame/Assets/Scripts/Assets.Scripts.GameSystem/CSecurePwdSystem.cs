using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CSecurePwdSystem : Singleton<CSecurePwdSystem>
	{
		public static string[] s_OpRiskNameKeys = new string[]
		{
			"SecurePwd_OpRisk_Use_Battle_Record_Card",
			"SecurePwd_OpRisk_Buy_Hero_For_Friend",
			"SecurePwd_OpRisk_Buy_Skin_For_Friend",
			"SecurePwd_OpRisk_Break_Symbol",
			"SecurePwd_OpRisk_Guild_Transfer_Chairman",
			"SecurePwd_OpRisk_Sale_Symbol"
		};

		public PwdStatus EnableStatus;

		public PwdCloseStatus CloseStatus;

		public int CloseTimerSeq;

		private uint m_CloseTime;

		public static string sSetPwdFormPath = "UGUI/Form/System/SecurePwd/Form_SetPwd.prefab";

		public static string sModifyPwdFormPath = "UGUI/Form/System/SecurePwd/Form_ModifyPwd.prefab";

		public static string sClosePwdFormPath = "UGUI/Form/System/SecurePwd/Form_ClosePwd.prefab";

		public static string sApplyClosePwdFormPath = "UGUI/Form/System/SecurePwd/Form_ApplyClose.prefab";

		public static string sValidatePwdFormPath = "UGUI/Form/System/SecurePwd/Form_Validate.prefab";

		public uint CloseTime
		{
			get
			{
				return this.m_CloseTime;
			}
			set
			{
				this.m_CloseTime = value;
				if (this.m_CloseTime > 0u)
				{
					if (this.CloseTimerSeq > 0)
					{
						Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.CloseTimerSeq);
					}
					DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
					TimeSpan timeSpan = Utility.ToUtcTime2Local((long)((ulong)this.m_CloseTime)) - dateTime;
					if (timeSpan.get_TotalSeconds() > 0.0)
					{
						long num = (long)(timeSpan.get_TotalSeconds() * 1000.0);
						if (num > 0L && num < 2147483647L)
						{
							this.CloseTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer((int)num, 1, new CTimer.OnTimeUpHandler(this.RefreshStatus));
						}
					}
				}
				else if (this.CloseTimerSeq > 0)
				{
					Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.CloseTimerSeq);
				}
			}
		}

		public override void Init()
		{
			this.EnableStatus = PwdStatus.Disable;
			this.CloseStatus = PwdCloseStatus.Close;
			this.m_CloseTime = 0u;
			this.CloseTimerSeq = -1;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_ApplyForceCloseConfirm, new CUIEventManager.OnUIEventHandler(this.ApplyForceCloseConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_ApplyCancelForceCloseConfirm, new CUIEventManager.OnUIEventHandler(this.ApplyCancelForceCloseConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OpenSetPwdForm, new CUIEventManager.OnUIEventHandler(this.OpenSetForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OpenModifyPwdForm, new CUIEventManager.OnUIEventHandler(this.OpenModifyForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OpenClosePwdForm, new CUIEventManager.OnUIEventHandler(this.OpenCloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OpenApplyClosePwdForm, new CUIEventManager.OnUIEventHandler(this.OpenApplyClosePwdForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnSetPwd, new CUIEventManager.OnUIEventHandler(this.OnSetPwd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnModifyPwd, new CUIEventManager.OnUIEventHandler(this.OnModifyPwd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnClosePwd, new CUIEventManager.OnUIEventHandler(this.OnClosePwd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnApplyClose, new CUIEventManager.OnUIEventHandler(this.OnApplyClosePwd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnCancelApplyClose, new CUIEventManager.OnUIEventHandler(this.OnCancelApplyClosePwd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnOpCancel, new CUIEventManager.OnUIEventHandler(this.OnOpCancel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnValidateConfirm, new CUIEventManager.OnUIEventHandler(this.OnValidateConfirm));
			Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_SET_RESULT, new Action<OpResult>(this.OnPwdSet));
			Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, new Action<OpResult>(this.OnPwdModify));
			Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, new Action<OpResult>(this.OnPwdClose));
			Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_FORCE_CLOSE_RESULT, new Action<OpResult>(this.OnPwdForceClose));
			Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_CANCEL_FORCE_CLOSE_RESULT, new Action<OpResult>(this.OnPwdCancelForceClose));
			Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_VALIDATE_RESULT, new Action<OpResult>(this.OnPwdValidate));
		}

		public override void UnInit()
		{
			base.UnInit();
			this.CloseStatus = PwdCloseStatus.Close;
			this.m_CloseTime = 0u;
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.CloseTimerSeq);
			this.CloseTimerSeq = -1;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_ApplyForceCloseConfirm, new CUIEventManager.OnUIEventHandler(this.ApplyForceCloseConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_ApplyCancelForceCloseConfirm, new CUIEventManager.OnUIEventHandler(this.ApplyCancelForceCloseConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OpenSetPwdForm, new CUIEventManager.OnUIEventHandler(this.OpenSetForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OpenModifyPwdForm, new CUIEventManager.OnUIEventHandler(this.OpenModifyForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OpenClosePwdForm, new CUIEventManager.OnUIEventHandler(this.OpenCloseForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OpenApplyClosePwdForm, new CUIEventManager.OnUIEventHandler(this.OpenApplyClosePwdForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnSetPwd, new CUIEventManager.OnUIEventHandler(this.OnSetPwd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnModifyPwd, new CUIEventManager.OnUIEventHandler(this.OnModifyPwd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnClosePwd, new CUIEventManager.OnUIEventHandler(this.OnClosePwd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnApplyClose, new CUIEventManager.OnUIEventHandler(this.OnApplyClosePwd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnCancelApplyClose, new CUIEventManager.OnUIEventHandler(this.OnCancelApplyClosePwd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnOpCancel, new CUIEventManager.OnUIEventHandler(this.OnOpCancel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnValidateConfirm, new CUIEventManager.OnUIEventHandler(this.OnValidateConfirm));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_SET_RESULT, new Action<OpResult>(this.OnPwdSet));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, new Action<OpResult>(this.OnPwdModify));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, new Action<OpResult>(this.OnPwdClose));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_FORCE_CLOSE_RESULT, new Action<OpResult>(this.OnPwdForceClose));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_CANCEL_FORCE_CLOSE_RESULT, new Action<OpResult>(this.OnPwdCancelForceClose));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_VALIDATE_RESULT, new Action<OpResult>(this.OnPwdValidate));
		}

		private void OpenSetForm(CUIEvent uiEvent)
		{
			if (this.EnableStatus == PwdStatus.Enable)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSecurePwdSystem.sSetPwdFormPath, false, true);
		}

		private void OpenModifyForm(CUIEvent uiEvent)
		{
			if (this.EnableStatus == PwdStatus.Disable || this.CloseStatus == PwdCloseStatus.Open)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSecurePwdSystem.sModifyPwdFormPath, false, true);
		}

		private void OpenCloseForm(CUIEvent uiEvent)
		{
			if (this.EnableStatus == PwdStatus.Disable)
			{
				return;
			}
			if (this.CloseStatus == PwdCloseStatus.Open)
			{
				DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				TimeSpan timeSpan = Utility.ToUtcTime2Local((long)((ulong)this.m_CloseTime)) - dateTime;
				if (timeSpan.get_TotalSeconds() < 0.0 || timeSpan.get_TotalSeconds() < 2.0)
				{
					return;
				}
			}
			PwdCloseStatus closeStatus = this.CloseStatus;
			if (closeStatus != PwdCloseStatus.Open)
			{
				if (closeStatus == PwdCloseStatus.Close)
				{
					CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSecurePwdSystem.sClosePwdFormPath, false, true);
				}
			}
			else
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SecurePwd_OpenApplyClosePwdForm);
			}
		}

		private void OpenApplyClosePwdForm(CUIEvent uiEvent)
		{
			if (this.EnableStatus == PwdStatus.Disable)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSecurePwdSystem.sApplyClosePwdFormPath, false, true);
			if (cUIFormScript == null)
			{
				DebugHelper.Assert(false, "Apply Close Pwd Form Is Null");
				return;
			}
			Transform transform = cUIFormScript.transform.Find("pnlBg/Panel_Main/LeftTime");
			Transform transform2 = cUIFormScript.transform.Find("pnlBg/Panel_Main/Button");
			Transform transform3 = cUIFormScript.transform.Find("pnlBg/Panel_Main/Desc");
			if (transform == null)
			{
				DebugHelper.Assert(false, "SecurePwdSys left time trans is null");
				return;
			}
			if (transform2 == null)
			{
				DebugHelper.Assert(false, "SecurePwdSys op btn trans is null");
				return;
			}
			if (transform3 == null)
			{
				DebugHelper.Assert(false, "SecurePwdSys desc trans is null");
				return;
			}
			Text component = transform3.GetComponent<Text>();
			if (component != null && GameDataMgr.svr2CltCfgDict.ContainsKey(13u))
			{
				ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(13u, out resGlobalInfo))
				{
					float num = resGlobalInfo.dwConfValue;
					float num2 = num / 86000f;
					component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Force_Close_Desc"), num2.ToString("F0")));
				}
			}
			PwdCloseStatus closeStatus = this.CloseStatus;
			if (closeStatus != PwdCloseStatus.Open)
			{
				if (closeStatus == PwdCloseStatus.Close)
				{
					transform.gameObject.CustomSetActive(false);
					CUIEventScript component2 = transform2.GetComponent<CUIEventScript>();
					if (component2 != null)
					{
						component2.SetUIEvent(enUIEventType.Up, enUIEventID.SecurePwd_OnApplyClose);
					}
					Text componetInChild = Utility.GetComponetInChild<Text>(transform2.gameObject, "Text");
					if (componetInChild != null)
					{
						componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Force_Close_Btn"));
					}
				}
			}
			else
			{
				transform.gameObject.CustomSetActive(true);
				CUITimerScript component3 = transform.Find("Timer").GetComponent<CUITimerScript>();
				if (component3 == null)
				{
					DebugHelper.Assert(false, "SecurePwdSys left timer is null");
					return;
				}
				DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				TimeSpan timeSpan = Utility.ToUtcTime2Local((long)((ulong)this.m_CloseTime)) - dateTime;
				if (timeSpan.get_TotalSeconds() > 0.0)
				{
					component3.SetTotalTime((float)timeSpan.get_TotalSeconds());
					component3.ReStartTimer();
					CUIEventScript component4 = transform2.GetComponent<CUIEventScript>();
					if (component4 != null)
					{
						component4.SetUIEvent(enUIEventType.Up, enUIEventID.SecurePwd_OnCancelApplyClose);
					}
					Text componetInChild2 = Utility.GetComponetInChild<Text>(transform2.gameObject, "Text");
					if (componetInChild2 != null)
					{
						componetInChild2.set_text(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Cancel_Force_Close_Btn"));
					}
				}
				else
				{
					this.EnableStatus = PwdStatus.Disable;
					this.CloseStatus = PwdCloseStatus.Close;
					this.CloseTime = 0u;
					Singleton<CUIManager>.GetInstance().CloseForm(CSecurePwdSystem.sApplyClosePwdFormPath);
					Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
				}
			}
		}

		private void OnSetPwd(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSecurePwdSystem.sSetPwdFormPath);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("pnlBg/Panel_Main/PwdContainer/InputField");
			Transform transform2 = form.transform.Find("pnlBg/Panel_Main/PwdConfirmContainer/InputField");
			if (transform == null)
			{
				DebugHelper.Assert(false, "Password InputField is null!");
				return;
			}
			if (transform2 == null)
			{
				DebugHelper.Assert(false, "ConfirmPassword InputField is null!");
				return;
			}
			InputField component = transform.GetComponent<InputField>();
			InputField component2 = transform2.GetComponent<InputField>();
			if (component == null)
			{
				DebugHelper.Assert(false, "Password InputField Component is null!");
				return;
			}
			if (component2 == null)
			{
				DebugHelper.Assert(false, "ConfirmPassword InputField Component is null!");
				return;
			}
			string text = component.get_text();
			string text2 = component2.get_text();
			this.SetPwd(text, text2);
		}

		private void OnModifyPwd(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSecurePwdSystem.sModifyPwdFormPath);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("pnlBg/Panel_Main/OldPwdContainer/InputField");
			Transform transform2 = form.transform.Find("pnlBg/Panel_Main/PwdContainer/InputField");
			Transform transform3 = form.transform.Find("pnlBg/Panel_Main/PwdConfirmContainer/InputField");
			if (transform == null)
			{
				DebugHelper.Assert(false, "Old Password InputField is null!");
				return;
			}
			if (transform2 == null)
			{
				DebugHelper.Assert(false, "Password InputField is null!");
				return;
			}
			if (transform3 == null)
			{
				DebugHelper.Assert(false, "ConfirmPassword InputField is null!");
				return;
			}
			InputField component = transform.GetComponent<InputField>();
			InputField component2 = transform2.GetComponent<InputField>();
			InputField component3 = transform3.GetComponent<InputField>();
			if (component == null)
			{
				DebugHelper.Assert(false, "Old Password InputField Component is null!");
				return;
			}
			if (component2 == null)
			{
				DebugHelper.Assert(false, "Password InputField Component is null!");
				return;
			}
			if (component3 == null)
			{
				DebugHelper.Assert(false, "ConfirmPassword InputField Component is null!");
				return;
			}
			string text = component.get_text();
			string text2 = component2.get_text();
			string text3 = component3.get_text();
			this.ModifyPwd(text2, text3, text);
		}

		private void OnApplyClosePwd(CUIEvent uiEvent)
		{
			if (this.EnableStatus == PwdStatus.Disable || this.CloseStatus == PwdCloseStatus.Open)
			{
				return;
			}
			this.ReqApplyClose();
		}

		private void OnCancelApplyClosePwd(CUIEvent uiEvent)
		{
			if (this.EnableStatus == PwdStatus.Disable || this.CloseStatus == PwdCloseStatus.Close)
			{
				return;
			}
			this.ReqCancelApplyClose();
		}

		private void OnClosePwd(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSecurePwdSystem.sClosePwdFormPath);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("pnlBg/Panel_Main/PwdContainer/InputField");
			if (transform == null)
			{
				DebugHelper.Assert(false, "Password InputField is null!");
				return;
			}
			InputField component = transform.GetComponent<InputField>();
			if (component == null)
			{
				DebugHelper.Assert(false, "Password InputField Component is null!");
				return;
			}
			string text = component.get_text();
			this.ApplyClose(text);
		}

		private void OnOpCancel(CUIEvent uiEvent)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_OP_CANCEL);
		}

		private void OnValidateConfirm(CUIEvent uiEvent)
		{
			stUIEventParams eventParams = uiEvent.m_eventParams;
			if (this.EnableStatus == PwdStatus.Enable)
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSecurePwdSystem.sValidatePwdFormPath);
				if (form == null)
				{
					return;
				}
				Transform transform = form.transform.Find("pnlBg/Panel_Main/PwdContainer/InputField");
				if (transform == null)
				{
					DebugHelper.Assert(false, "Password InputField is null!");
					return;
				}
				InputField component = transform.GetComponent<InputField>();
				if (component == null)
				{
					DebugHelper.Assert(false, "Password InputField Component is null!");
					return;
				}
				string text = component.get_text();
				if (!CSecurePwdSystem.Check(text))
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_VALIDATE_RESULT, OpResult.Invalid);
					return;
				}
				Singleton<CUIManager>.GetInstance().CloseForm(CSecurePwdSystem.sValidatePwdFormPath);
				eventParams.pwd = text;
			}
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(eventParams.srcUIEventID, eventParams);
		}

		private void OnPwdSet(OpResult rs)
		{
			if (rs == OpResult.Success)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CSecurePwdSystem.sSetPwdFormPath);
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Set_Pwd_Success"), false, 1.5f, null, new object[0]);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
				return;
			}
			this.HandleOpError(rs);
		}

		private void OnPwdModify(OpResult rs)
		{
			if (rs == OpResult.Success)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CSecurePwdSystem.sModifyPwdFormPath);
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Modify_Pwd_Success"), false, 1.5f, null, new object[0]);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
				return;
			}
			this.HandleOpError(rs);
		}

		private void OnPwdClose(OpResult rs)
		{
			if (rs == OpResult.Success)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CSecurePwdSystem.sClosePwdFormPath);
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Close_Pwd_Success"), false, 1.5f, null, new object[0]);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
				return;
			}
			this.HandleOpError(rs);
		}

		private void OnPwdForceClose(OpResult rs)
		{
			if (rs == OpResult.Success)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CSecurePwdSystem.sApplyClosePwdFormPath);
				Singleton<CUIManager>.GetInstance().CloseForm(CSecurePwdSystem.sModifyPwdFormPath);
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Apply_Force_Close_Success"), false, 1.5f, null, new object[0]);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
				return;
			}
			this.HandleOpError(rs);
		}

		private void OnPwdCancelForceClose(OpResult rs)
		{
			if (rs == OpResult.Success)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CSecurePwdSystem.sApplyClosePwdFormPath);
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Cancel_Force_Close_Success"), false, 1.5f, null, new object[0]);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
				return;
			}
			this.HandleOpError(rs);
		}

		private void OnPwdValidate(OpResult rs)
		{
			this.HandleOpError(rs);
		}

		public static bool Check(string pwd)
		{
			if (pwd == null)
			{
				return false;
			}
			pwd = pwd.TrimEnd(new char[]
			{
				'\r',
				'\n'
			});
			if (string.IsNullOrEmpty(pwd))
			{
				return false;
			}
			string text = "^[a-zA-Z\\d-`=\\\\\\[\\];',./~!@#$%^&*()_+|{}:\"<>?]{6,12}$";
			if (!Regex.IsMatch(pwd, text))
			{
				return false;
			}
			byte[] bytes = Encoding.get_UTF8().GetBytes(pwd);
			return bytes.Length <= 64;
		}

		public void SetPwd(string newPwd, string newPwdConfirm)
		{
			if (this.EnableStatus != PwdStatus.Disable)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_SET_RESULT, OpResult.Illegal);
				return;
			}
			if (newPwd != newPwdConfirm)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_SET_RESULT, OpResult.NotEuqal);
				return;
			}
			if (!CSecurePwdSystem.Check(newPwd) || !CSecurePwdSystem.Check(newPwdConfirm))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_SET_RESULT, OpResult.Invalid);
				return;
			}
			this.ReqSet(newPwd);
		}

		public void ApplyClose(string pwd)
		{
			if (this.EnableStatus != PwdStatus.Enable)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, OpResult.Illegal);
				return;
			}
			if (!CSecurePwdSystem.Check(pwd))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, OpResult.Invalid);
				return;
			}
			this.ReqClose(pwd);
		}

		public void ApplyForceClose()
		{
			if (this.EnableStatus != PwdStatus.Enable)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_FORCE_CLOSE_RESULT, OpResult.Illegal);
				return;
			}
			float num = GameDataMgr.globalInfoDatabin.GetDataByKey(13u).dwConfValue;
			float num2 = num / 86000f;
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Force_Close_Desc"), num2.ToString("F0")), enUIEventID.SecurePwd_ApplyForceCloseConfirm, enUIEventID.None, false);
		}

		public void ModifyPwd(string newPwd, string newPwdConfirm, string oldPwd)
		{
			if (this.EnableStatus != PwdStatus.Enable)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, OpResult.Illegal);
				return;
			}
			if (newPwd != newPwdConfirm)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, OpResult.NotEuqal);
				return;
			}
			if (!CSecurePwdSystem.Check(oldPwd))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("旧密码错误，请重新输入", false, 1.5f, null, new object[0]);
				return;
			}
			if (!CSecurePwdSystem.Check(newPwd) || !CSecurePwdSystem.Check(newPwdConfirm))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("新密码不符合规则，请重新输入", false, 1.5f, null, new object[0]);
				return;
			}
			this.ReqModify(newPwd, oldPwd);
		}

		public static void TryToValidate(enOpPurpose purpose, enUIEventID confirmEventID, stUIEventParams confirmEventParams = default(stUIEventParams))
		{
			CSecurePwdSystem instance = Singleton<CSecurePwdSystem>.GetInstance();
			if (instance.EnableStatus == PwdStatus.Enable)
			{
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSecurePwdSystem.sValidatePwdFormPath, false, true);
				if (cUIFormScript == null)
				{
					return;
				}
				Transform transform = cUIFormScript.transform.Find("pnlBg/Panel_Main/BtnConfirm");
				Transform transform2 = cUIFormScript.transform.Find("pnlBg/Panel_Main/BtnCancel");
				if (transform == null || transform2 == null)
				{
					return;
				}
				CUIEventScript component = transform.GetComponent<CUIEventScript>();
				CUIEventScript component2 = transform2.GetComponent<CUIEventScript>();
				if (component == null || component2 == null)
				{
					return;
				}
				confirmEventParams.srcUIEventID = confirmEventID;
				component.SetUIEvent(enUIEventType.Up, enUIEventID.SecurePwd_OnValidateConfirm, confirmEventParams);
			}
			else
			{
				confirmEventParams.srcUIEventID = confirmEventID;
				string text = Singleton<CTextManager>.GetInstance().GetText(CSecurePwdSystem.s_OpRiskNameKeys[(int)purpose]);
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.SecurePwd_OnValidateConfirm, enUIEventID.SecurePwd_OpenSetPwdForm, confirmEventParams, "暂时不用", "前往设置", true, string.Empty);
			}
		}

		private void HandleOpError(OpResult err)
		{
			switch (err)
			{
			case OpResult.Fail:
				Singleton<CUIManager>.GetInstance().OpenTips("设置密码失败,请稍后重试", false, 1.5f, null, new object[0]);
				break;
			case OpResult.NotEuqal:
				Singleton<CUIManager>.GetInstance().OpenTips("两次密码不相同，请重新输入", false, 1.5f, null, new object[0]);
				break;
			case OpResult.Invalid:
				Singleton<CUIManager>.GetInstance().OpenTips("密码不符合规则，请重新输入", false, 1.5f, null, new object[0]);
				break;
			case OpResult.Illegal:
				Singleton<CUIManager>.GetInstance().OpenTips("当前操作不允许", false, 1.5f, null, new object[0]);
				break;
			}
		}

		private void RefreshStatus(int seq)
		{
			this.m_CloseTime = 0u;
			this.CloseTimerSeq = -1;
			this.CloseStatus = PwdCloseStatus.Close;
			this.EnableStatus = PwdStatus.Disable;
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
		}

		private void ApplyForceCloseConfirm(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1907u);
			CSPKG_ACNT_PSWDINFO_FORCE cSPKG_ACNT_PSWDINFO_FORCE = new CSPKG_ACNT_PSWDINFO_FORCE();
			cSPKG_ACNT_PSWDINFO_FORCE.bOpt = 1;
			cSPkg.stPkgData.stAcntPswdForceReq = cSPKG_ACNT_PSWDINFO_FORCE;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ApplyCancelForceCloseConfirm(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1909u);
			CSPKG_ACNT_PSWDINFO_FORCECAL cSPKG_ACNT_PSWDINFO_FORCECAL = new CSPKG_ACNT_PSWDINFO_FORCECAL();
			cSPKG_ACNT_PSWDINFO_FORCECAL.bOpt = 1;
			cSPkg.stPkgData.stAcntPswdFroceCalReq = cSPKG_ACNT_PSWDINFO_FORCECAL;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ReqSet(string pwd)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1901u);
			CSPKG_ACNT_PSWDINFO_OPEN cSPKG_ACNT_PSWDINFO_OPEN = new CSPKG_ACNT_PSWDINFO_OPEN();
			StringHelper.StringToUTF8Bytes(pwd, ref cSPKG_ACNT_PSWDINFO_OPEN.szPswdStr);
			cSPkg.stPkgData.stAcntPswdOpenReq = cSPKG_ACNT_PSWDINFO_OPEN;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ReqModify(string pwd, string oldPwd = null)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1905u);
			CSPKG_ACNT_PSWDINFO_CHG cSPKG_ACNT_PSWDINFO_CHG = new CSPKG_ACNT_PSWDINFO_CHG();
			StringHelper.StringToUTF8Bytes(pwd, ref cSPKG_ACNT_PSWDINFO_CHG.szNewPswdStr);
			StringHelper.StringToUTF8Bytes(oldPwd, ref cSPKG_ACNT_PSWDINFO_CHG.szOldPswdStr);
			cSPkg.stPkgData.stAcntPswdChgReq = cSPKG_ACNT_PSWDINFO_CHG;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ReqClose(string pwd)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1903u);
			CSPKG_ACNT_PSWDINFO_CLOSE cSPKG_ACNT_PSWDINFO_CLOSE = new CSPKG_ACNT_PSWDINFO_CLOSE();
			StringHelper.StringToUTF8Bytes(pwd, ref cSPKG_ACNT_PSWDINFO_CLOSE.szPswdStr);
			cSPkg.stPkgData.stAcntPswdCloseReq = cSPKG_ACNT_PSWDINFO_CLOSE;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ReqApplyClose()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1907u);
			CSPKG_ACNT_PSWDINFO_FORCE cSPKG_ACNT_PSWDINFO_FORCE = new CSPKG_ACNT_PSWDINFO_FORCE();
			cSPKG_ACNT_PSWDINFO_FORCE.bOpt = 1;
			cSPkg.stPkgData.stAcntPswdForceReq = cSPKG_ACNT_PSWDINFO_FORCE;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ReqCancelApplyClose()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1909u);
			CSPKG_ACNT_PSWDINFO_FORCECAL cSPKG_ACNT_PSWDINFO_FORCECAL = new CSPKG_ACNT_PSWDINFO_FORCECAL();
			cSPKG_ACNT_PSWDINFO_FORCECAL.bOpt = 0;
			cSPkg.stPkgData.stAcntPswdFroceCalReq = cSPKG_ACNT_PSWDINFO_FORCECAL;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1902)]
		public static void ReceiveSetRs(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAcntPswdOpenRsp.iResult == 0)
			{
				Singleton<CSecurePwdSystem>.GetInstance().EnableStatus = PwdStatus.Enable;
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_SET_RESULT, OpResult.Success);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1902, msg.stPkgData.stAcntPswdOpenRsp.iResult), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1906)]
		public static void ReceiveModifyRs(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAcntPswdChgRsp.iResult == 0)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, OpResult.Success);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1906, msg.stPkgData.stAcntPswdChgRsp.iResult), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1904)]
		public static void ReceiveCloseRs(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAcntPswdCloseRsp.iResult == 0)
			{
				Singleton<CSecurePwdSystem>.GetInstance().EnableStatus = PwdStatus.Disable;
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, OpResult.Success);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1904, msg.stPkgData.stAcntPswdCloseRsp.iResult), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1908)]
		public static void ReceiveForceCloseRs(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAcntPswdForceRsp.iResult == 0)
			{
				Singleton<CSecurePwdSystem>.GetInstance().CloseStatus = PwdCloseStatus.Open;
				Singleton<CSecurePwdSystem>.GetInstance().CloseTime = msg.stPkgData.stAcntPswdForceRsp.dwForceTime;
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_FORCE_CLOSE_RESULT, OpResult.Success);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1908, msg.stPkgData.stAcntPswdForceRsp.iResult), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1910)]
		public static void ReceiveCancelForceCloseRs(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAcntPswdFroceCalRsp.iResult == 0)
			{
				Singleton<CSecurePwdSystem>.GetInstance().CloseStatus = PwdCloseStatus.Close;
				Singleton<CSecurePwdSystem>.GetInstance().CloseTime = 0u;
				Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_CANCEL_FORCE_CLOSE_RESULT, OpResult.Success);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1910, msg.stPkgData.stAcntPswdFroceCalRsp.iResult), false, 1.5f, null, new object[0]);
			}
		}
	}
}
