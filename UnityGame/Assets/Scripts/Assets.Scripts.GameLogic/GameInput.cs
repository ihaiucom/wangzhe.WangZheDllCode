using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class GameInput : Singleton<GameInput>
	{
		private StateMachine inputMode = new StateMachine();

		public static float DoubleTouchDeltaTime = 0.25f;

		public static float UseDirectionSkillDistance = 80f;

		public static float minGuestureCircleRadian = 20f;

		public static float minCurveTrackDistance = 60f;

		public static int enemyExploreOptRadius = 2000;

		private int ConfirmDirSndFrame = -1;

		private int PreMoveDirection = 2147483647;

		private int FixtimeDirSndFrame = -1;

		private byte nDirMoveSeq;

		public bool isSlowUpMoveCmd;

		private bool bSmartUse;

		private DateTime lastClickEscape = DateTime.get_Now();

		private bool bMessageBoxIsOpen;

		public override void Init()
		{
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, new RefAction<DefaultGameEventParam>(this.OnHostActorClearMove));
			this.inputMode.RegisterState<LobbyInputMode>(new LobbyInputMode(this), "LobbyInputMode");
			this.inputMode.RegisterState<JoystickMode>(new JoystickMode(this), "JoystickMode");
			this.inputMode.ChangeState("LobbyInputMode");
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnQuitGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Quit_GameCancel, new CUIEventManager.OnUIEventHandler(this.OnQuitCameCancel));
			this.bSmartUse = false;
		}

		public bool IsSmartUse()
		{
			return this.bSmartUse;
		}

		public void SetSmartUse(bool _bUse)
		{
			this.bSmartUse = _bUse;
		}

		public void UpdateFrame()
		{
			if (this.inputMode.tarState != null)
			{
				((GameInputMode)this.inputMode.tarState).Update();
			}
			if (this.ConfirmDirSndFrame > 0 && this.PreMoveDirection != -2147483648)
			{
				int num = Time.frameCount - this.ConfirmDirSndFrame;
				if (num > 0 && num < 15 && num % 6 == 0)
				{
					int confirmDirSndFrame = this.ConfirmDirSndFrame;
					if (this.PreMoveDirection != 2147483647)
					{
						this.SendMoveDirection(this.PreMoveDirection);
					}
					else
					{
						this.SendStopMove(true);
					}
					this.ConfirmDirSndFrame = confirmDirSndFrame;
				}
			}
			this.UpdateEscape();
		}

		public void UpdateEscape()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if ((DateTime.get_Now() - this.lastClickEscape).get_TotalMilliseconds() < 1500.0)
				{
					if (!this.bMessageBoxIsOpen)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Is_QuitGame"), enUIEventID.Quit_Game, enUIEventID.Quit_GameCancel, false);
						this.bMessageBoxIsOpen = true;
					}
				}
				else
				{
					this.lastClickEscape = DateTime.get_Now();
				}
			}
		}

		private void OnQuitGame(CUIEvent uiEvent)
		{
			SGameApplication.Quit();
		}

		private void OnQuitCameCancel(CUIEvent uiEvent)
		{
			this.bMessageBoxIsOpen = false;
		}

		public void ChangeLobbyMode()
		{
			this.inputMode.ChangeState("LobbyInputMode");
		}

		public void ChangeBattleMode(bool bBriefness)
		{
			this.inputMode.ChangeState("JoystickMode");
		}

		private void OnHostActorClearMove(ref DefaultGameEventParam prm)
		{
			if (ActorHelper.IsHostCtrlActor(ref prm.src))
			{
				this.PreMoveDirection = -2147483648;
			}
		}

		public void OnHostActorRecvMove(int nDegree)
		{
			if (nDegree == this.PreMoveDirection)
			{
				this.ConfirmDirSndFrame = -1;
			}
		}

		public void StopInput()
		{
			((GameInputMode)this.inputMode.tarState).StopInput();
		}

		private VInt3 CalcDirectionByTouchPosition(Vector2 InFirst, Vector2 InSecond)
		{
			if (Camera.main != null)
			{
				Vector3 b = Camera.main.ScreenToWorldPoint(new Vector3(InFirst.x, InFirst.y, Camera.main.nearClipPlane));
				Vector3 a = Camera.main.ScreenToWorldPoint(new Vector3(InSecond.x, InSecond.y, Camera.main.nearClipPlane));
				Vector3 normalized = Vector3.ProjectOnPlane((a - b).normalized, new Vector3(0f, 1f, 0f)).normalized;
				return new VInt3(normalized);
			}
			DebugHelper.Assert(false, "CalcDirectionByTouchPosition, Main camera is null");
			return VInt3.forward;
		}

		public void SendMoveDirection(Vector2 start, Vector2 end)
		{
			this.FixtimeDirSndFrame++;
			VInt3 lhs = this.CalcDirectionByTouchPosition(start, end);
			if (lhs != VInt3.zero)
			{
				int num = (int)((double)(IntMath.atan2(-lhs.z, lhs.x).single * 180f) / 3.1416);
				DebugHelper.Assert(num < 32767 && num > -32768, "向量转换成2pi空间超过范围了");
				int num2 = num - this.PreMoveDirection;
				if ((num2 > 1 || num2 < -1 || this.FixtimeDirSndFrame > 30) && (!this.isSlowUpMoveCmd || this.ConfirmDirSndFrame < Time.frameCount - 1))
				{
					this.SendMoveDirection(num);
				}
			}
		}

		private void SendMoveDirection(int moveDegree)
		{
			this.PreMoveDirection = moveDegree;
			this.ConfirmDirSndFrame = Time.frameCount;
			this.FixtimeDirSndFrame = 0;
			FrameCommand<MoveDirectionCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<MoveDirectionCommand>();
			frameCommand.cmdData.Degree = (short)moveDegree;
			if ((int)this.nDirMoveSeq < 256 && this.nDirMoveSeq >= 0)
			{
				Singleton<FrameSynchr>.GetInstance().m_MoveCMDSendTime[(int)this.nDirMoveSeq] = (uint)(Time.realtimeSinceStartup * 1000f);
			}
			byte b = this.nDirMoveSeq;
			this.nDirMoveSeq = b + 1;
			frameCommand.cmdData.nSeq = b;
			frameCommand.Send();
		}

		public void SendStopMove(bool force = false)
		{
			if (this.PreMoveDirection != 2147483647 || force)
			{
				this.PreMoveDirection = 2147483647;
				this.ConfirmDirSndFrame = Time.frameCount;
				this.FixtimeDirSndFrame = 0;
				FrameCommandFactory.CreateFrameCommand<StopMoveCommand>().Send();
			}
		}
	}
}
