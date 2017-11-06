using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class CallMonsterTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int TargetId = -1;

		public int WayPointId = -1;

		public int ConfigID;

		public ECampType CampType;

		public bool Invincible;

		public bool Moveable;

		public bool bSuicideWhenHostDead = true;

		public bool bDrageToHostWhenTooFarAway = true;

		public bool bUseHostValueProperty;

		public bool bInitialVisibility = true;

		public int LifeTime;

		public bool bCopyedHeroInfo;

		private GameObject wayPoint;

		private static readonly int MaxLevel = 6;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			CallMonsterTick callMonsterTick = src as CallMonsterTick;
			this.TargetId = callMonsterTick.TargetId;
			this.WayPointId = callMonsterTick.WayPointId;
			this.ConfigID = callMonsterTick.ConfigID;
			this.LifeTime = callMonsterTick.LifeTime;
			this.CampType = callMonsterTick.CampType;
			this.Invincible = callMonsterTick.Invincible;
			this.Moveable = callMonsterTick.Moveable;
			this.bCopyedHeroInfo = callMonsterTick.bCopyedHeroInfo;
			this.bSuicideWhenHostDead = callMonsterTick.bSuicideWhenHostDead;
			this.bDrageToHostWhenTooFarAway = callMonsterTick.bDrageToHostWhenTooFarAway;
			this.bUseHostValueProperty = callMonsterTick.bUseHostValueProperty;
			this.bInitialVisibility = callMonsterTick.bInitialVisibility;
		}

		public override BaseEvent Clone()
		{
			CallMonsterTick callMonsterTick = ClassObjPool<CallMonsterTick>.Get();
			callMonsterTick.CopyData(this);
			return callMonsterTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.bSuicideWhenHostDead = true;
			this.bDrageToHostWhenTooFarAway = true;
			this.bUseHostValueProperty = false;
			this.bInitialVisibility = true;
		}

		private COM_PLAYERCAMP SelectCamp(ref PoolObjHandle<ActorRoot> InActor)
		{
			if (this.CampType == ECampType.ECampType_Self)
			{
				return InActor.handle.TheActorMeta.ActorCamp;
			}
			if (this.CampType != ECampType.ECampType_Hostility)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
			}
			COM_PLAYERCAMP actorCamp = InActor.handle.TheActorMeta.ActorCamp;
			if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			}
			return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
		}

		private int GetAddValue(ref PoolObjHandle<ActorRoot> OrignalHost, ref PoolObjHandle<ActorRoot> Monster, ref ResCallMonster CallMonsterCfg, RES_FUNCEFT_TYPE type)
		{
			int num = 0;
			byte bAddType = CallMonsterCfg.bAddType;
			if ((CallMonsterCfg.bAddType & 1) != 0)
			{
				num += OrignalHost.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
			}
			if ((CallMonsterCfg.bAddType & 2) != 0)
			{
				num += OrignalHost.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
			}
			if ((CallMonsterCfg.bAddType & 4) != 0)
			{
				num += OrignalHost.handle.ValueComponent.mActorValue[type].totalValue;
			}
			return num;
		}

		private void ApplyMonsterAdditive(ref PoolObjHandle<ActorRoot> OrignalHost, ref PoolObjHandle<ActorRoot> Monster, ref ResCallMonster CallMonsterCfg)
		{
			this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT, (int)CallMonsterCfg.dwAddAttack, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT));
			this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT, (int)CallMonsterCfg.dwAddMagic, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT));
			this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT, (int)CallMonsterCfg.dwAddArmor, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT));
			this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT, (int)CallMonsterCfg.dwAddResistant, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT));
			this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, (int)CallMonsterCfg.dwAddHp, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP));
			if ((CallMonsterCfg.bAddType & 4) != 0)
			{
				Monster.handle.ValueComponent.actorHp = Monster.handle.ValueComponent.actorHpTotal * OrignalHost.handle.ValueComponent.actorHp / OrignalHost.handle.ValueComponent.actorHpTotal;
				Monster.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].addValue = OrignalHost.handle.ValueComponent.actorEpTotal;
				Monster.handle.ValueComponent.actorEp = OrignalHost.handle.ValueComponent.actorEp;
			}
			else
			{
				Monster.handle.ValueComponent.actorHp = Monster.handle.ValueComponent.actorHpTotal;
			}
			this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD, (int)CallMonsterCfg.dwAddSpeed, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD));
		}

		private void ApplyProperty(ref PoolObjHandle<ActorRoot> Monster, RES_FUNCEFT_TYPE InType, int InValue, int InBase)
		{
			int num = (int)((double)InBase * (double)InValue / 10000.0);
			Monster.handle.ValueComponent.mActorValue[InType].addValue = Monster.handle.ValueComponent.mActorValue[InType].addValue + num;
		}

		private int SelectLevel(ref PoolObjHandle<ActorRoot> HostActor, ref ResCallMonster CallMonsterCfg, ref SkillUseContext SkillContext)
		{
			if (CallMonsterCfg.bDependencyType == 1)
			{
				return HostActor.handle.ValueComponent.actorSoulLevel;
			}
			if (CallMonsterCfg.bDependencyType == 2)
			{
				return HostActor.handle.SkillControl.SkillSlotArray[(int)SkillContext.SlotType].GetSkillLevel();
			}
			return 0;
		}

		private void SpawnMonster(Action _action, ref PoolObjHandle<ActorRoot> tarActor)
		{
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			if (refParamObject == null || !refParamObject.Originator || refParamObject.Originator.handle.ActorControl == null)
			{
				DebugHelper.Assert(false, "Failed find orignal actor of this skill. action:{0}", new object[]
				{
					_action.name
				});
				return;
			}
			if (refParamObject.Originator.handle.ActorControl.IsDeadState)
			{
				return;
			}
			DebugHelper.Assert(refParamObject.Originator.handle.ValueComponent != null, "ValueComponent is null");
			ResCallMonster dataByKey = GameDataMgr.callMonsterDatabin.GetDataByKey((long)this.ConfigID);
			DebugHelper.Assert(dataByKey != null, "Failed find call monster config id:{0} action:{1}", new object[]
			{
				this.ConfigID,
				_action.name
			});
			if (dataByKey == null)
			{
				return;
			}
			int num = Math.Min(CallMonsterTick.MaxLevel, this.SelectLevel(ref refParamObject.Originator, ref dataByKey, ref refParamObject));
			ResMonsterCfgInfo dataCfgInfo = MonsterDataHelper.GetDataCfgInfo((int)dataByKey.dwMonsterID, num);
			DebugHelper.Assert(dataCfgInfo != null, "Failed find monster id={0} diff={1} action:{2}", new object[]
			{
				dataByKey.dwMonsterID,
				num,
				_action.name
			});
			if (dataCfgInfo == null)
			{
				return;
			}
			string fullPathInResources = StringHelper.UTF8BytesToString(ref dataCfgInfo.szCharacterInfo) + ".asset";
			CActorInfo exists = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(CActorInfo), enResourceType.BattleScene, false, false).m_content as CActorInfo;
			if (exists)
			{
				ActorMeta actorMeta = default(ActorMeta);
				ActorMeta actorMeta2 = actorMeta;
				actorMeta2.ConfigId = (int)dataByKey.dwMonsterID;
				actorMeta2.ActorType = ActorTypeDef.Actor_Type_Monster;
				actorMeta2.ActorCamp = this.SelectCamp(ref refParamObject.Originator);
				actorMeta2.EnCId = (int)dataByKey.dwMonsterID;
				actorMeta2.Difficuty = (byte)num;
				actorMeta2.SkinID = refParamObject.Originator.handle.TheActorMeta.SkinID;
				actorMeta = actorMeta2;
				VInt3 location = tarActor.handle.location;
				VInt3 forward = tarActor.handle.forward;
				if (!PathfindingUtility.IsValidTarget(refParamObject.Originator.handle, location))
				{
					location = refParamObject.Originator.handle.location;
					forward = refParamObject.Originator.handle.forward;
				}
				PoolObjHandle<ActorRoot> poolObjHandle = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref actorMeta, location, forward, false, true);
				if (poolObjHandle)
				{
					poolObjHandle.handle.InitActor();
					this.ApplyMonsterAdditive(ref refParamObject.Originator, ref poolObjHandle, ref dataByKey);
					MonsterWrapper monsterWrapper = poolObjHandle.handle.ActorControl as MonsterWrapper;
					if (monsterWrapper != null)
					{
						monsterWrapper.SetHostActorInfo(ref refParamObject.Originator, refParamObject.SlotType, this.bCopyedHeroInfo, this.bSuicideWhenHostDead, this.bDrageToHostWhenTooFarAway, this.bUseHostValueProperty);
						if (this.wayPoint != null)
						{
							monsterWrapper.AttackAlongRoute(this.wayPoint.GetComponent<WaypointsHolder>());
						}
						if (this.LifeTime > 0)
						{
							monsterWrapper.LifeTime = this.LifeTime;
						}
					}
					if (this.bUseHostValueProperty)
					{
						refParamObject.Originator.handle.ValueComponent.mActorValue.SetChangeEvent(RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD, new ValueChangeDelegate(poolObjHandle.handle.ActorControl.OnMoveSpdChange));
					}
					refParamObject.Originator.handle.SkillControl.SetSkillIndicatorToCallMonster();
					poolObjHandle.handle.PrepareFight();
					Singleton<GameObjMgr>.instance.AddActor(poolObjHandle);
					poolObjHandle.handle.StartFight();
					poolObjHandle.handle.ObjLinker.Invincible = this.Invincible;
					poolObjHandle.handle.ObjLinker.CanMovable = this.Moveable;
					poolObjHandle.handle.Visible = (!this.bInitialVisibility || refParamObject.Originator.handle.Visible);
					poolObjHandle.handle.ValueComponent.actorSoulLevel = refParamObject.Originator.handle.ValueComponent.actorSoulLevel;
					poolObjHandle.handle.DefaultAttackModeControl = poolObjHandle.handle.CreateLogicComponent<DefaultAttackMode>(poolObjHandle.handle);
					if (FogOfWar.enable && poolObjHandle.handle.HorizonMarker != null)
					{
						poolObjHandle.handle.HorizonMarker.SightRadius = Horizon.QuerySoldierSightRadius();
					}
					refParamObject.Originator.handle.ValueComponent.AddSoulExp(0, false, AddSoulType.Other);
				}
			}
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.TargetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			this.wayPoint = _action.GetGameObject(this.WayPointId);
			this.SpawnMonster(_action, ref actorHandle);
		}
	}
}
