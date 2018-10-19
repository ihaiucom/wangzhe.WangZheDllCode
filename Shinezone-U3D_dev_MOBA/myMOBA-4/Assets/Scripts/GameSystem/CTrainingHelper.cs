using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CTrainingHelper : Singleton<CTrainingHelper>
	{
		private enum ECheatAct : byte
		{
			LevelUp,
			SetLevel,
			FullHp,
			FullEp,
			ToggleInvincible,
			ToggleAi,
			ToggleSoldier,
			ResetSoldier,
			AddGold,
			ToggleZeroCd,
			Count
		}

		private enum EActTarget : byte
		{
			Friendly,
			Hostile,
			Count
		}

		private struct ActorSpawnInfo
		{
			public int ConfigId;

			public ActorTypeDef ActorType;

			public COM_PLAYERCAMP CampType;

			public VInt3 BornPos;

			public VInt3 BornDir;

			public ActorSpawnInfo(int inCfgId, ActorTypeDef inActorType, COM_PLAYERCAMP inCampType, VInt3 inPos, VInt3 inDir)
			{
				this.ConfigId = inCfgId;
				this.ActorType = inActorType;
				this.CampType = inCampType;
				this.BornPos = inPos;
				this.BornDir = inDir;
			}
		}

		private CUIFormScript m_form;

		private GameObject m_openBtn;

		private GameObject m_panelObj;

		private GameObject m_cdBtnToggle;

		private GameObject m_invincibleBtnToggle;

		private GameObject m_aiBtnToggle;

		private GameObject m_soldierBtnToggle;

		private bool m_cdToggleFlag;

		private bool m_invincibleToggleFlag;

		private bool m_aiToggleFlag;

		private bool m_soldierToggleFlag;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperOpen, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperClose, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperInit, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperInit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperUninit, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperUninit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperCheat, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperCheat));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperCheat, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperCheat));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperUninit, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperUninit));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperInit, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperInit));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperOpen, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperOpen));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperClose, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperClose));
		}

		private void OnTrainingHelperInit(CUIEvent uiEvent)
		{
			if (this.m_form == null)
			{
				this.m_form = Singleton<CBattleSystem>.instance.FightFormScript;
				DebugHelper.Assert(this.m_form != null);
				this.m_openBtn = this.m_form.transform.FindChild("Panel_Prop/ButtonOpen").gameObject;
				this.m_panelObj = this.m_form.transform.FindChild("Panel_Prop/Panel_BaseProp").gameObject;
				this.InitHostileFuncList();
				this.InitSelfFuncList();
				this.InitGeneralFuncList();
				this.InitBtnToggle();
				this.m_form.transform.FindChild("Panel_Prop").gameObject.CustomSetActive(true);
				Transform transform = this.m_form.transform.FindChild("MapPanel");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(false);
				}
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
				if (theMinimapSys != null)
				{
					theMinimapSys.mmRoot.CustomSetActive(false);
				}
			}
		}

		private void InitHostileFuncList()
		{
			DebugHelper.Assert(this.m_panelObj != null);
			if (this.m_panelObj == null)
			{
				return;
			}
			Transform transform = this.m_panelObj.transform.FindChild("HostileFuncList");
			CUIMiniEventScript component = transform.GetChild(0).GetComponent<CUIMiniEventScript>();
			component.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.LevelUp,
				CTrainingHelper.ECheatAct.FullHp,
				CTrainingHelper.ECheatAct.FullEp
			});
			component.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[]
			{
				CTrainingHelper.EActTarget.Hostile
			});
			CUIMiniEventScript component2 = transform.GetChild(1).GetComponent<CUIMiniEventScript>();
			component2.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.SetLevel,
				CTrainingHelper.ECheatAct.FullHp,
				CTrainingHelper.ECheatAct.FullEp
			});
			component2.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[]
			{
				CTrainingHelper.EActTarget.Hostile
			});
			component2.m_onClickEventParams.tag3 = 1;
			CUIMiniEventScript component3 = transform.GetChild(2).GetComponent<CUIMiniEventScript>();
			component3.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.ToggleAi
			});
			component3.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[]
			{
				CTrainingHelper.EActTarget.Hostile
			});
			this.m_aiBtnToggle = component3.gameObject;
		}

		private void InitSelfFuncList()
		{
			DebugHelper.Assert(this.m_panelObj != null);
			if (this.m_panelObj == null)
			{
				return;
			}
			Transform transform = this.m_panelObj.transform.FindChild("SelfFuncList");
			CUIMiniEventScript component = transform.GetChild(0).GetComponent<CUIMiniEventScript>();
			component.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.LevelUp,
				CTrainingHelper.ECheatAct.FullHp,
				CTrainingHelper.ECheatAct.FullEp
			});
			component.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[1]);
			CUIMiniEventScript component2 = transform.GetChild(1).GetComponent<CUIMiniEventScript>();
			component2.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.SetLevel,
				CTrainingHelper.ECheatAct.FullHp,
				CTrainingHelper.ECheatAct.FullEp
			});
			component2.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[1]);
			component2.m_onClickEventParams.tag3 = 1;
			CUIMiniEventScript component3 = transform.GetChild(2).GetComponent<CUIMiniEventScript>();
			component3.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.ToggleZeroCd
			});
			component3.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[1]);
			this.m_cdBtnToggle = component3.gameObject;
			CUIMiniEventScript component4 = transform.GetChild(3).GetComponent<CUIMiniEventScript>();
			component4.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.ToggleInvincible
			});
			component4.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[1]);
			this.m_invincibleBtnToggle = component4.gameObject;
		}

		private void InitGeneralFuncList()
		{
			DebugHelper.Assert(this.m_panelObj != null);
			if (this.m_panelObj == null)
			{
				return;
			}
			Transform transform = this.m_panelObj.transform.FindChild("GeneralFuncList");
			CUIMiniEventScript component = transform.GetChild(0).GetComponent<CUIMiniEventScript>();
			component.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.ToggleSoldier,
				CTrainingHelper.ECheatAct.ResetSoldier
			});
			component.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[1]);
			this.m_soldierBtnToggle = component.gameObject;
			CUIMiniEventScript component2 = transform.GetChild(1).GetComponent<CUIMiniEventScript>();
			component2.m_onClickEventParams.tag = this.CheatActListToMask(new CTrainingHelper.ECheatAct[]
			{
				CTrainingHelper.ECheatAct.AddGold
			});
			component2.m_onClickEventParams.tag2 = this.ActTarListToMask(new CTrainingHelper.EActTarget[1]);
			component2.m_onClickEventParams.tag3 = 1000;
		}

		private void InitBtnToggle()
		{
			this.m_aiToggleFlag = false;
			this.m_soldierToggleFlag = false;
			this.m_invincibleToggleFlag = false;
			this.m_cdToggleFlag = false;
			this.RefreshBtnToggleAi();
			this.RefreshBtnToggleCd();
			this.RefreshBtnToggleInvincible();
			this.RefreshBtnToggleSoldier();
		}

		private void RefreshBtnToggleAi()
		{
			this.RefreshBtnToggle(this.m_aiToggleFlag, this.m_aiBtnToggle);
		}

		private void RefreshBtnToggleCd()
		{
			this.RefreshBtnToggle(this.m_cdToggleFlag, this.m_cdBtnToggle);
		}

		private void RefreshBtnToggleInvincible()
		{
			this.RefreshBtnToggle(this.m_invincibleToggleFlag, this.m_invincibleBtnToggle);
		}

		private void RefreshBtnToggleSoldier()
		{
			this.RefreshBtnToggle(this.m_soldierToggleFlag, this.m_soldierBtnToggle);
		}

		private void RefreshBtnToggle(bool bFlag, GameObject inBtn)
		{
			if (inBtn != null)
			{
				GameObject gameObject = inBtn.transform.GetChild(0).gameObject;
				GameObject gameObject2 = inBtn.transform.GetChild(1).gameObject;
				if (bFlag)
				{
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(true);
				}
				else
				{
					gameObject.CustomSetActive(true);
					gameObject2.CustomSetActive(false);
				}
			}
		}

		private void OnTrainingHelperUninit(CUIEvent uiEvent)
		{
			if (this.m_form != null)
			{
				this.m_form.transform.FindChild("Panel_Prop").gameObject.CustomSetActive(false);
				this.m_form = null;
				this.m_openBtn = null;
				this.m_panelObj = null;
			}
		}

		public void SetButtonActive(bool bAct)
		{
			if (this.m_openBtn != null)
			{
				this.m_openBtn.CustomSetActive(bAct);
			}
		}

		public bool IsOpenBtnActive()
		{
			return !(this.m_openBtn == null) && this.m_openBtn.activeSelf;
		}

		public bool IsPanelActive()
		{
			return !(this.m_panelObj == null) && this.m_panelObj.activeSelf;
		}

		private void OnTrainingHelperOpen(CUIEvent uiEvent)
		{
			this.m_openBtn.CustomSetActive(false);
			this.m_panelObj.CustomSetActive(true);
			if (this.m_form != null)
			{
				Transform transform = this.m_form.transform.FindChild("MapPanel");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(false);
				}
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
				if (theMinimapSys != null)
				{
					theMinimapSys.mmRoot.CustomSetActive(false);
				}
			}
		}

		private void OnTrainingHelperClose(CUIEvent uiEvent)
		{
			this.m_openBtn.CustomSetActive(true);
			this.m_panelObj.CustomSetActive(false);
			if (this.m_form != null)
			{
				Transform transform = this.m_form.transform.FindChild("MapPanel");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(true);
				}
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
				if (theMinimapSys != null)
				{
					theMinimapSys.mmRoot.CustomSetActive(true);
				}
			}
		}

		private List<CTrainingHelper.ECheatAct> CheatActMaskToList(int cheatActMask)
		{
			List<CTrainingHelper.ECheatAct> list = new List<CTrainingHelper.ECheatAct>();
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				if ((cheatActMask & 1 << i) > 0)
				{
					list.Add((CTrainingHelper.ECheatAct)i);
				}
			}
			return list;
		}

		private int CheatActListToMask(List<CTrainingHelper.ECheatAct> inList)
		{
			int num = 0;
			List<CTrainingHelper.ECheatAct>.Enumerator enumerator = inList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CTrainingHelper.ECheatAct current = enumerator.Current;
				num |= 1 << (int)current;
			}
			return num;
		}

		private int CheatActListToMask(params CTrainingHelper.ECheatAct[] inList)
		{
			return this.CheatActListToMask(new List<CTrainingHelper.ECheatAct>(inList));
		}

		private List<CTrainingHelper.EActTarget> ActTarMaskToList(int actTarMask)
		{
			List<CTrainingHelper.EActTarget> list = new List<CTrainingHelper.EActTarget>();
			int num = 2;
			for (int i = 0; i < num; i++)
			{
				if ((actTarMask & 1 << i) > 0)
				{
					list.Add((CTrainingHelper.EActTarget)i);
				}
			}
			return list;
		}

		private int ActTarListToMask(List<CTrainingHelper.EActTarget> inList)
		{
			int num = 0;
			List<CTrainingHelper.EActTarget>.Enumerator enumerator = inList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CTrainingHelper.EActTarget current = enumerator.Current;
				num |= 1 << (int)current;
			}
			return num;
		}

		private int ActTarListToMask(params CTrainingHelper.EActTarget[] inList)
		{
			return this.ActTarListToMask(new List<CTrainingHelper.EActTarget>(inList));
		}

		private void OnTrainingHelperCheat(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			int tag2 = uiEvent.m_eventParams.tag2;
			int tag3 = uiEvent.m_eventParams.tag3;
			List<CTrainingHelper.ECheatAct> list = this.CheatActMaskToList(tag);
			List<CTrainingHelper.EActTarget> list2 = this.ActTarMaskToList(tag2);
			List<CTrainingHelper.ECheatAct>.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CTrainingHelper.ECheatAct current = enumerator.Current;
				List<CTrainingHelper.EActTarget>.Enumerator enumerator2 = list2.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					CTrainingHelper.EActTarget current2 = enumerator2.Current;
					this.DoCheatAction(current, current2, tag3);
				}
			}
		}

		private static void HeroVisiter(COM_PLAYERCAMP inCamp, int inParam, Action<ActorRoot, int> inFunc)
		{
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GameObjMgr>.instance.HeroActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current)
				{
					PoolObjHandle<ActorRoot> current = enumerator.Current;
					ActorRoot handle = current.handle;
					if (handle.TheActorMeta.ActorCamp == inCamp)
					{
						inFunc(handle, inParam);
					}
				}
			}
		}

		private static void OrganVisiter(COM_PLAYERCAMP inCamp, int inParam, Action<ActorRoot, int> inFunc)
		{
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GameObjMgr>.instance.OrganActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current)
				{
					PoolObjHandle<ActorRoot> current = enumerator.Current;
					ActorRoot handle = current.handle;
					if (handle.TheActorMeta.ActorCamp == inCamp)
					{
						inFunc(handle, inParam);
					}
				}
			}
		}

		private static void FullHp(ActorRoot inActor, int inParam)
		{
			if (inActor != null && inActor.ValueComponent != null)
			{
				inActor.ValueComponent.RecoverHp();
			}
		}

		private static void FullEp(ActorRoot inActor, int inParam)
		{
			if (inActor != null && inActor.ValueComponent != null)
			{
				inActor.ValueComponent.RecoverEp();
			}
		}

		private static void LevelUp(ActorRoot inActor, int inParam)
		{
			if (inActor != null && inActor.ValueComponent != null)
			{
				inActor.ValueComponent.ForceSoulLevelUp();
			}
		}

		private static void SetLevel(ActorRoot inActor, int inParam)
		{
			if (inActor != null && inActor.ValueComponent != null)
			{
				inActor.ValueComponent.ForceSetSoulLevel(inParam);
			}
		}

		private static void ResetSkillLevel(ActorRoot inActor, int inParam)
		{
			if (inActor != null && inActor.SkillControl != null)
			{
				inActor.SkillControl.ResetSkillLevel();
			}
		}

		private static void ToggleAi(ActorRoot inActor, int inParam)
		{
			if (inActor != null && inActor.ActorAgent != null && inActor.ActorControl != null)
			{
				if (!inActor.ActorAgent.bPaused && !inActor.ActorControl.IsDeadState)
				{
					inActor.ActorControl.CmdStopMove();
					if (inActor.AnimControl != null)
					{
						PlayAnimParam param = default(PlayAnimParam);
						param.animName = "Idle";
						param.blendTime = 0f;
						param.loop = true;
						param.layer = 0;
						param.speed = 1f;
						inActor.AnimControl.Play(param);
					}
				}
				inActor.ActorAgent.SetPaused(!inActor.ActorAgent.bPaused);
			}
		}

		private static void ToggleZeroCd(ActorRoot inActor, int inParam)
		{
			if (inActor != null && inActor.SkillControl != null)
			{
				inActor.SkillControl.ToggleZeroCd();
			}
		}

		private static void ReviveTower(ActorRoot inActor, int inParam)
		{
			if (inActor != null && inActor.ActorControl != null && inActor.ActorControl.IsDeadState)
			{
				inActor.ActorControl.Revive(false);
				inActor.RecoverOriginalActorMesh();
				if (inActor.ActorMesh != null)
				{
					inActor.ActorMesh.SetLayer("Actor", "Particles", true);
				}
				inActor.ActorControl.PlayAnimation("Born", 0.01f, 0, false);
			}
		}

		private static void SpawnDynamicActor(ref CTrainingHelper.ActorSpawnInfo inSpawnInfo)
		{
			ActorMeta actorMeta = default(ActorMeta);
			actorMeta.ActorType = inSpawnInfo.ActorType;
			actorMeta.ConfigId = inSpawnInfo.ConfigId;
			actorMeta.ActorCamp = inSpawnInfo.CampType;
			VInt3 bornPos = inSpawnInfo.BornPos;
			VInt3 bornDir = inSpawnInfo.BornDir;
			PoolObjHandle<ActorRoot> poolObjHandle = Singleton<GameObjMgr>.instance.SpawnActorEx(null, ref actorMeta, bornPos, bornDir, false, true);
			if (poolObjHandle)
			{
				poolObjHandle.handle.InitActor();
				poolObjHandle.handle.PrepareFight();
				Singleton<GameObjMgr>.instance.AddActor(poolObjHandle);
				poolObjHandle.handle.StartFight();
			}
		}

		private void DoCheatAction(CTrainingHelper.ECheatAct inAct, CTrainingHelper.EActTarget inTar, int inParam)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			COM_PLAYERCAMP playerCamp = hostPlayer.PlayerCamp;
			COM_PLAYERCAMP cOM_PLAYERCAMP = (playerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_1 : COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			COM_PLAYERCAMP inCamp = (inTar != CTrainingHelper.EActTarget.Hostile) ? playerCamp : cOM_PLAYERCAMP;
			switch (inAct)
			{
			case CTrainingHelper.ECheatAct.LevelUp:
				CTrainingHelper.HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.LevelUp));
				break;
			case CTrainingHelper.ECheatAct.SetLevel:
				CTrainingHelper.HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.ResetSkillLevel));
				CTrainingHelper.HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.SetLevel));
				break;
			case CTrainingHelper.ECheatAct.FullHp:
				CTrainingHelper.HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.FullHp));
				break;
			case CTrainingHelper.ECheatAct.FullEp:
				CTrainingHelper.HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.FullEp));
				break;
			case CTrainingHelper.ECheatAct.ToggleInvincible:
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				bool arg_FA_0 = curLvelContext != null && curLvelContext.IsMobaMode();
				Player hostPlayer2 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
				if (hostPlayer2 != null && hostPlayer2.Captain && hostPlayer2.Captain.handle.ActorControl is HeroWrapper)
				{
					HeroWrapper heroWrapper = (HeroWrapper)hostPlayer2.Captain.handle.ActorControl;
					heroWrapper.bGodMode = !heroWrapper.bGodMode;
				}
				this.m_invincibleToggleFlag = !this.m_invincibleToggleFlag;
				this.RefreshBtnToggleInvincible();
				break;
			}
			case CTrainingHelper.ECheatAct.ToggleAi:
				CTrainingHelper.HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.ToggleAi));
				this.m_aiToggleFlag = !this.m_aiToggleFlag;
				this.RefreshBtnToggleAi();
				break;
			case CTrainingHelper.ECheatAct.ToggleSoldier:
				Singleton<BattleLogic>.GetInstance().mapLogic.EnableSoldierRegion(this.m_soldierToggleFlag);
				this.m_soldierToggleFlag = !this.m_soldierToggleFlag;
				this.RefreshBtnToggleSoldier();
				break;
			case CTrainingHelper.ECheatAct.ResetSoldier:
			{
				Singleton<BattleLogic>.instance.mapLogic.ResetSoldierRegion();
				Singleton<BattleLogic>.instance.dynamicProperty.ResetTimer();
				Singleton<GameObjMgr>.GetInstance().KillSoldiers();
				CTrainingHelper.OrganVisiter(COM_PLAYERCAMP.COM_PLAYERCAMP_1, inParam, new Action<ActorRoot, int>(CTrainingHelper.ReviveTower));
				CTrainingHelper.OrganVisiter(COM_PLAYERCAMP.COM_PLAYERCAMP_2, inParam, new Action<ActorRoot, int>(CTrainingHelper.ReviveTower));
				AttackOrder attackOrder = Singleton<BattleLogic>.instance.attackOrder;
				if (attackOrder != null)
				{
					attackOrder.FightOver();
					attackOrder.FightStart();
				}
				break;
			}
			case CTrainingHelper.ECheatAct.AddGold:
			{
				SLevelContext curLvelContext2 = Singleton<BattleLogic>.instance.GetCurLvelContext();
				bool arg_26E_0 = curLvelContext2 != null && curLvelContext2.IsMobaMode();
				Player hostPlayer3 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
				if (hostPlayer3 != null && hostPlayer3.Captain)
				{
					if (hostPlayer3.Captain.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
					{
						CallActorWrapper callActorWrapper = hostPlayer3.Captain.handle.ActorControl as CallActorWrapper;
						if (callActorWrapper != null && callActorWrapper.hostActor)
						{
							callActorWrapper.hostActor.handle.ValueComponent.ChangeGoldCoinInBattle(1000, true, true, default(Vector3), false, default(PoolObjHandle<ActorRoot>));
						}
					}
					else if (hostPlayer3.Captain.handle.ValueComponent != null)
					{
						hostPlayer3.Captain.handle.ValueComponent.ChangeGoldCoinInBattle(1000, true, true, default(Vector3), false, default(PoolObjHandle<ActorRoot>));
					}
				}
				break;
			}
			case CTrainingHelper.ECheatAct.ToggleZeroCd:
				CTrainingHelper.HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.ToggleZeroCd));
				this.m_cdToggleFlag = !this.m_cdToggleFlag;
				this.RefreshBtnToggleCd();
				break;
			}
		}
	}
}
