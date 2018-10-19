using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public sealed class JoystickMode : GameInputMode
	{
		public enum enJoystickInput
		{
			LeftAxis,
			RightAxis,
			LeftTrigger,
			RightTrigger,
			DPad,
			ButtonX,
			ButtonY,
			ButtonA,
			ButtonB,
			ButtonL1,
			ButtonR1,
			ButtonSelect,
			ButtonStart,
			ButtonL3,
			ButtonR3
		}

		public enum enJoystickButtonState
		{
			None,
			Down,
			Hold,
			Up
		}

		public enum enInputMode
		{
			None,
			UI,
			Joystick
		}

		public static string s_joystickLeftAxisHorizontal = "JoystickLeftAxisHorizontal";

		public static string s_joystickLeftAxisVertical = "JoystickLeftAxisVertical";

		public static string s_joystickRightAxisHorizontal = "JoystickRightAxisHorizontal";

		public static string s_joystickRightAxisVertical = "JoystickRightAxisVertical";

		public static string s_joystickLeftTrigger = "JoystickLeftTrigger";

		public static string s_joystickRightTrigger = "JoystickRightTrigger";

		public static string s_joystickDPadHorizontal = "JoystickDPadHorizontal";

		public static string s_joystickDPadVertical = "JoystickDPadVertical";

		public static string[] s_joystickButtons = new string[]
		{
			"JoystickButtonX",
			"JoystickButtonY",
			"JoystickButtonA",
			"JoystickButtonB",
			"JoystickButtonL",
			"JoystickButtonR",
			"JoystickButtonSelect",
			"JoystickButtonStart",
			"JoystickButtonL3",
			"JoystickButtonR3"
		};

		public Vector2 m_leftAxis;

		public Vector2 m_rightAxis;

		public float m_leftTrigger;

		public float m_rightTrigger;

		public Vector2 m_dpad;

		public int m_leftAxisState;

		public int m_rightAxisState;

		public int m_leftTriggerState;

		public int m_rightTriggerState;

		public int m_dpadState;

		public int[] m_buttonStates = new int[JoystickMode.s_joystickButtons.Length];

		private SkillSlotType m_selectedSkillSlot = SkillSlotType.SLOT_SKILL_VALID;

		private JoystickMode.enInputMode m_leftAxisInputMode;

		private Vector2 m_leftAxisFromUI;

		private Vector2 m_cameraAxisFromUI;

		public JoystickMode(GameInput InSys) : base(InSys)
		{
		}

		public override void OnStateEnter()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAxisChanged, new CUIEventManager.OnUIEventHandler(this.OnLeftAxisChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisChanged, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisReleased, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisReleased));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisPushed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraStartDrag, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraStartDrag));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraDraging, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraDraging));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraEndDrag, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraEndDrag));
		}

		public override void OnStateLeave()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAxisChanged, new CUIEventManager.OnUIEventHandler(this.OnLeftAxisChanged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisChanged, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisChanged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisReleased, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisReleased));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisPushed));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraStartDrag, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraStartDrag));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraDraging, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraDraging));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraEndDrag, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraEndDrag));
		}

		public override void Update()
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm == null)
			{
				return;
			}
			this.HandleUIInput();
		}

		private void OnLeftAxisChanged(CUIEvent uiEvent)
		{
			CUIJoystickScript cUIJoystickScript = uiEvent.m_srcWidgetScript as CUIJoystickScript;
			if (cUIJoystickScript != null)
			{
				this.m_leftAxisFromUI = cUIJoystickScript.GetAxis();
			}
		}

		private void OnCameraAxisChanged(CUIEvent uiEvent)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.ActorControl != null && !hostPlayer.Captain.handle.ActorControl.IsDeadState)
			{
				CUIJoystickScript cUIJoystickScript = uiEvent.m_srcWidgetScript as CUIJoystickScript;
				if (cUIJoystickScript != null)
				{
					this.m_cameraAxisFromUI = cUIJoystickScript.GetAxis();
				}
			}
		}

		private void OnCameraAxisPushed(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.instance.DispatchUIEvent(enUIEventID.Battle_CloseBigMap);
			MonoSingleton<CameraSystem>.instance.enableCameraMovement = true;
		}

		private void OnCameraAxisReleased(CUIEvent uiEvent)
		{
			MonoSingleton<CameraSystem>.instance.enableCameraMovement = false;
		}

		private void OnPanelCameraStartDrag(CUIEvent uiEvent)
		{
			MonoSingleton<CameraSystem>.instance.enableCameraMovement = true;
		}

		private void OnPanelCameraDraging(CUIEvent uiEvent)
		{
			Vector2 vector = new Vector2(uiEvent.m_pointerEventData.delta.x, uiEvent.m_pointerEventData.delta.y);
			MonoSingleton<CameraSystem>.instance.UpdatePanelCameraMovement(ref vector);
			Vector3 position = MonoSingleton<CameraSystem>.instance.MobaCamera.requirements.pivot.position;
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			if (theMinimapSys == null || theMinimapSys.MMiniMapCameraFrame_3Dui == null)
			{
				return;
			}
			if (!theMinimapSys.MMiniMapCameraFrame_3Dui.IsCameraFrameShow)
			{
				theMinimapSys.MMiniMapCameraFrame_3Dui.Show();
				theMinimapSys.MMiniMapCameraFrame_3Dui.ShowNormal();
			}
			theMinimapSys.MMiniMapCameraFrame_3Dui.SetPos(position.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x, position.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y);
		}

		private void OnPanelCameraEndDrag(CUIEvent uiEvent)
		{
			MonoSingleton<CameraSystem>.instance.enableCameraMovement = false;
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			if (theMinimapSys == null || theMinimapSys.MMiniMapCameraFrame_3Dui == null)
			{
				return;
			}
			theMinimapSys.MMiniMapCameraFrame_3Dui.Hide();
		}

		private void HandleUIInput()
		{
			this.HandleMoveInput(this.m_leftAxisFromUI, JoystickMode.enInputMode.UI);
			this.HandleCameraMoveInput(ref this.m_cameraAxisFromUI);
		}

		private void HandleMoveInput(Vector2 axis, JoystickMode.enInputMode inputMode)
		{
			if (axis != Vector2.zero)
			{
				if (inputMode == JoystickMode.enInputMode.UI || inputMode == this.m_leftAxisInputMode || this.m_leftAxisInputMode == JoystickMode.enInputMode.None)
				{
					this.m_leftAxisInputMode = inputMode;
					Singleton<GameInput>.GetInstance().SendMoveDirection(Vector2.zero, axis);
				}
			}
			else if (inputMode == this.m_leftAxisInputMode)
			{
				this.m_leftAxisInputMode = JoystickMode.enInputMode.None;
				Singleton<GameInput>.GetInstance().SendStopMove(false);
			}
		}

		private void HandleCameraMoveInput(ref Vector2 axis)
		{
			MonoSingleton<CameraSystem>.instance.UpdateCameraMovement(ref axis);
		}
	}
}
