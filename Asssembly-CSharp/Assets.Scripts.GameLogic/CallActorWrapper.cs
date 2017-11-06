using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using ResData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class CallActorWrapper : ObjWrapper
	{
		private uint m_skinCfgId;

		private uint m_skinId;

		public OperateMode CurOpMode;

		private string skillEffectPath = string.Empty;

		protected int m_iLifeTime;

		public PoolObjHandle<ActorRoot> hostActor = new PoolObjHandle<ActorRoot>(null);

		public PoolObjHandle<ActorRoot> imposterActor = new PoolObjHandle<ActorRoot>(null);

		protected SkillSlotType SpawnSkillSlot = SkillSlotType.SLOT_SKILL_VALID;

		private bool m_bIsTrueType;

		public bool IsTrueType
		{
			get
			{
				return this.m_bIsTrueType;
			}
			set
			{
				if (this.m_bIsTrueType != value)
				{
					this.m_bIsTrueType = value;
					if (this.m_bIsTrueType)
					{
						if (this.hostActor)
						{
							this.CopyActorInfo(ref this.hostActor);
						}
						this.LeaveImposterMesh();
					}
					else if (this.imposterActor)
					{
						this.CopyActorInfo(ref this.imposterActor);
					}
					this.actor.HudControl.ChangeCallActorBloodImg(this.m_bIsTrueType);
				}
			}
		}

		public int LifeTime
		{
			get
			{
				return this.m_iLifeTime;
			}
			set
			{
				this.m_iLifeTime = value;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.m_skinCfgId = 0u;
			this.m_skinId = 0u;
			this.skillEffectPath = string.Empty;
			this.CurOpMode = OperateMode.DefaultMode;
			this.m_iLifeTime = 0;
			this.m_bIsTrueType = false;
			this.hostActor.Release();
			this.imposterActor.Release();
		}

		public void InitProperty(ref PoolObjHandle<ActorRoot> hostActor)
		{
			this.SetHostActor(ref hostActor);
			this.CreateImposterActor();
			this.CopyActorSkillLevel(ref hostActor);
		}

		public override PoolObjHandle<ActorRoot> GetOrignalActor()
		{
			return this.hostActor;
		}

		private void CopyActorInfo(ref PoolObjHandle<ActorRoot> targetActor)
		{
			this.CopyActorBuff(ref targetActor);
			if (this.m_bIsTrueType)
			{
				this.CopyActorEquip(ref targetActor);
			}
		}

		public void CopyImposterInfo()
		{
			this.CopyActorInfo(ref this.imposterActor);
			this.actor.HudControl.ChangeCallActorBloodImg(this.m_bIsTrueType);
		}

		private void CopyActorEquip(ref PoolObjHandle<ActorRoot> targetActor)
		{
			if (!targetActor)
			{
				return;
			}
			if (targetActor.handle.EquipComponent == null || this.actor.EquipComponent == null)
			{
				return;
			}
			stEquipInfo[] equips = targetActor.handle.EquipComponent.GetEquips();
			if (equips == null)
			{
				return;
			}
			int num = 6;
			for (int i = 0; i < num; i++)
			{
				this.actor.EquipComponent.CallActorAddEquip(i, equips[i]);
			}
			this.actor.EquipComponent.UpdateEquipEffect();
		}

		private void CopyActorSkillLevel(ref PoolObjHandle<ActorRoot> targetActor)
		{
			if (!targetActor)
			{
				return;
			}
			if (targetActor.handle.SkillControl == null || this.actor.SkillControl == null)
			{
				return;
			}
			for (int i = 1; i <= 3; i++)
			{
				if (this.actor.SkillControl.SkillSlotArray[i] != null && targetActor.handle.SkillControl.SkillSlotArray[i] != null)
				{
					this.actor.SkillControl.SkillSlotArray[i].SetSkillLevel(targetActor.handle.SkillControl.SkillSlotArray[i].GetSkillLevel());
				}
			}
		}

		private void CopyActorBuff(ref PoolObjHandle<ActorRoot> targetActor)
		{
			if (!targetActor)
			{
				return;
			}
			if (targetActor.handle.BuffHolderComp == null || this.actor.BuffHolderComp == null)
			{
				return;
			}
			List<BuffSkill> spawnedBuffList = this.actor.BuffHolderComp.SpawnedBuffList;
			if (spawnedBuffList != null)
			{
				int count = spawnedBuffList.get_Count();
				for (int i = 0; i < count; i++)
				{
					this.actor.BuffHolderComp.RemoveBuff(spawnedBuffList.get_Item(i).SkillID);
				}
			}
			List<BuffSkill> spawnedBuffList2 = targetActor.handle.BuffHolderComp.SpawnedBuffList;
			if (spawnedBuffList2 != null)
			{
				int count2 = spawnedBuffList2.get_Count();
				for (int j = 0; j < count2; j++)
				{
					if (spawnedBuffList2.get_Item(j) != null && spawnedBuffList2.get_Item(j).GetSkillUseContext() != null)
					{
						SkillUseParam skillUseParam = default(SkillUseParam);
						skillUseParam.SetOriginator(spawnedBuffList2.get_Item(j).GetSkillUseContext().Originator);
						skillUseParam.bExposing = spawnedBuffList2.get_Item(j).skillContext.bExposing;
						skillUseParam.uiFromId = spawnedBuffList2.get_Item(j).skillContext.uiFromId;
						skillUseParam.skillUseFrom = spawnedBuffList2.get_Item(j).skillContext.skillUseFrom;
						this.actor.SkillControl.SpawnBuff(this.actorPtr, ref skillUseParam, spawnedBuffList2.get_Item(j).SkillID, true);
					}
				}
			}
		}

		public void CreateImposterActor()
		{
			List<Player> diffCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetDiffCampPlayers(this.actorPtr.handle.TheActorMeta.ActorCamp);
			int count = diffCampPlayers.get_Count();
			if (count > 0)
			{
				int num = (int)FrameRandom.Random((uint)count);
				Player player = diffCampPlayers.get_Item(num);
				if (player.Captain)
				{
					this.imposterActor = player.Captain;
					return;
				}
			}
			this.imposterActor = this.actorPtr;
		}

		public PoolObjHandle<ActorRoot> GetImposterActor()
		{
			return this.imposterActor;
		}

		public void SetHostActor(ref PoolObjHandle<ActorRoot> _actor)
		{
			this.hostActor = _actor;
			if (this.hostActor)
			{
				this.hostActor.handle.ActorControl.eventActorDead += new ActorDeadEventHandler(this.OnActorDead);
			}
		}

		public PoolObjHandle<ActorRoot> GetHostActor()
		{
			return this.hostActor;
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			this.actor.MovementComponent = this.actor.CreateLogicComponent<PlayerMovement>(this.actor);
			this.actor.MatHurtEffect = this.actor.CreateActorComponent<MaterialHurtEffect>(this.actor);
			this.actor.EffectControl = this.actor.CreateLogicComponent<EffectPlayComponent>(this.actor);
			this.actor.EquipComponent = this.actor.CreateLogicComponent<EquipComponent>(this.actor);
			this.actor.ShadowEffect = this.actor.CreateActorComponent<UpdateShadowPlane>(this.actor);
			VCollisionShape.InitActorCollision(this.actor);
			this.actor.DefaultAttackModeControl = this.actor.CreateLogicComponent<DefaultAttackMode>(this.actor);
			this.actor.LockTargetAttackModeControl = this.actor.CreateLogicComponent<LockTargetAttackMode>(this.actor);
		}

		public override void Prepare()
		{
		}

		public override string GetTypeName()
		{
			return "CallActorWrapper";
		}

		public override void Fight()
		{
			base.Fight();
			if (ActorHelper.IsCaptainActor(ref this.actorPtr))
			{
				this.m_isControledByMan = true;
				this.m_isAutoAI = false;
			}
			else
			{
				this.m_isControledByMan = false;
				this.m_isAutoAI = true;
			}
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			ActorServerData actorServerData = default(ActorServerData);
			if (actorDataProvider != null && actorDataProvider.GetActorServerData(ref this.actor.TheActorMeta, ref actorServerData))
			{
				this.m_skinId = actorServerData.SkinId;
				this.m_skinCfgId = CSkinInfo.GetSkinCfgId((uint)this.actor.TheActorMeta.ConfigId, this.m_skinId);
				if (this.m_skinId != 0u)
				{
					ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin((uint)this.actor.TheActorMeta.ConfigId, this.m_skinId);
					if (heroSkin != null && !string.IsNullOrEmpty(heroSkin.szSoundSwitchEvent))
					{
						Singleton<CSoundManager>.instance.PostEvent(heroSkin.szSoundSwitchEvent, this.actor.gameObject);
					}
				}
			}
			this.SetSkillEffectPath();
			base.EnableRVO(false);
			if (this.actor.HorizonMarker != null && FogOfWar.enable)
			{
				this.actor.HorizonMarker.SightRadius = Horizon.QueryGlobalSight();
			}
		}

		public bool GetSkinCfgID(out uint skinCfgID)
		{
			skinCfgID = this.m_skinCfgId;
			return this.m_skinId != 0u;
		}

		private void SetSkillEffectPath()
		{
			int configId = this.actor.TheStaticData.TheActorMeta.ConfigId;
			string heroNamePinYin = this.actor.TheStaticData.TheHeroOnlyInfo.HeroNamePinYin;
			string text = "prefab_skill_effects/hero_skill_effects/";
			StringBuilder stringBuilder = new StringBuilder(text);
			stringBuilder.AppendFormat("{0}_{1}/{2}", configId, heroNamePinYin, this.m_skinCfgId);
			this.skillEffectPath = stringBuilder.ToString();
		}

		public string GetSkinEffectPath()
		{
			return this.skillEffectPath;
		}

		public override bool DoesApplyExposingRule()
		{
			return true;
		}

		public override bool DoesIgnoreAlreadyLit()
		{
			return false;
		}

		public override int QueryExposeDuration()
		{
			return Horizon.QueryExposeDurationHero();
		}

		public override bool DoesApplyShowmarkRule()
		{
			return true;
		}

		protected override void OnDead()
		{
			PoolObjHandle<ActorRoot> attack = this.myLastAtker ? this.myLastAtker.handle.ActorControl.GetOrignalActor() : this.myLastAtker;
			this.deadMode = ObjDeadMode.DeadState_Normal;
			this.TerminateMove();
			base.ClearMoveCommand();
			base.EnableRVO(false);
			if (this.actor.HudControl != null)
			{
				this.actor.HudControl.OnActorDead();
			}
			if (this.actor.BuffHolderComp != null)
			{
				this.actor.BuffHolderComp.OnDead(attack);
			}
			SkillComponent skillControl = this.actor.SkillControl;
			skillControl.OnDead();
			if (!this.m_reviveContext.bEnable)
			{
				skillControl.ResetAllSkillSlot(true);
			}
			if (this.hostActor)
			{
				this.hostActor.handle.ActorControl.eventActorDead -= new ActorDeadEventHandler(this.OnActorDead);
			}
			this.CaptainHostSwitch();
			Singleton<GameObjMgr>.instance.RecycleActor(this.actorPtr, 0);
		}

		protected override void OnRevive()
		{
		}

		public override void Deactive()
		{
		}

		protected override void OnSoulLvlChange()
		{
		}

		private void OnAdvanceSkin()
		{
			if (this.m_skinId > 0u && this.actor.ValueComponent.actorSoulLevel > 1)
			{
				string empty = string.Empty;
				if (this.actor.CharInfo != null && this.actor.CharInfo.GetAdvanceSkinPrefabName(out empty, this.m_skinId, this.actor.ValueComponent.actorSoulLevel, -1))
				{
					bool flag = false;
					GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(empty, false, SceneObjType.ActionRes, Vector3.zero, Quaternion.identity, out flag);
					if (pooledGameObjLOD != null)
					{
						pooledGameObjLOD.transform.SetParent(this.actor.myTransform);
						pooledGameObjLOD.transform.localPosition = Vector3.zero;
						pooledGameObjLOD.transform.localRotation = Quaternion.identity;
						this.actor.SetActorMesh(pooledGameObjLOD);
					}
				}
			}
		}

		public int GetAdvanceSkinIndex()
		{
			if (this.m_skinId > 0u && this.actor.ValueComponent.actorSoulLevel > 1 && this.actor.CharInfo != null)
			{
				return this.actor.CharInfo.GetAdvanceSkinIndexByLevel(this.m_skinId, this.actor.ValueComponent.actorSoulLevel);
			}
			return 0;
		}

		public string GetLevelUpEftPath(int level)
		{
			if (this.m_skinId > 0u && this.actor.ValueComponent.actorSoulLevel > 1 && this.actor.CharInfo != null && this.actor.CharInfo.SkinPrefab != null && this.m_skinId >= 1u && (ulong)this.m_skinId <= (ulong)((long)this.actor.CharInfo.SkinPrefab.Length))
			{
				SkinElement skinElement = this.actor.CharInfo.SkinPrefab[(int)((uint)((UIntPtr)(this.m_skinId - 1u)))];
				if (skinElement != null && skinElement.AdvanceSkin != null)
				{
					for (int i = 0; i < skinElement.AdvanceSkin.Length; i++)
					{
						if (skinElement.AdvanceSkin[i] != null && level == skinElement.AdvanceSkin[i].Level)
						{
							StringBuilder stringBuilder = new StringBuilder(this.GetSkinEffectPath());
							stringBuilder.AppendFormat("/levelUpEftPath{0}", i + 1);
							return stringBuilder.ToString();
						}
					}
				}
			}
			return string.Empty;
		}

		public override int TakeDamage(ref HurtDataInfo hurt)
		{
			if (hurt.hurtType != HurtTypeDef.Therapic)
			{
				this.IsTrueType = true;
			}
			return base.TakeDamage(ref hurt);
		}

		public override void UpdateLogic(int nDelta)
		{
			this.actor.ActorAgent.UpdateLogic(nDelta);
			base.UpdateLogic(nDelta);
			if (this.m_iLifeTime > 0)
			{
				this.m_iLifeTime -= nDelta;
				if (this.m_iLifeTime <= 0)
				{
					base.SetObjBehaviMode(ObjBehaviMode.State_Dead);
				}
			}
		}

		public override void AddDisableSkillFlag(SkillSlotType _type, bool bForce = false)
		{
			base.AddDisableSkillFlag(_type, false);
			if (_type == SkillSlotType.SLOT_SKILL_COUNT)
			{
				for (int i = 0; i < 10; i++)
				{
					if (this.actor.SkillControl.DisableSkill[i] == 1 && (!this.actor.SkillControl.IsIngnoreDisableSkill((SkillSlotType)i) || bForce))
					{
						DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam((SkillSlotType)i, 0, this.actorPtr);
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, base.GetActor(), ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
					}
				}
			}
			else if (this.actor.SkillControl.DisableSkill[(int)_type] == 1 && (!this.actor.SkillControl.IsIngnoreDisableSkill(_type) || bForce))
			{
				DefaultSkillEventParam defaultSkillEventParam2 = new DefaultSkillEventParam(_type, 0, this.actorPtr);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, base.GetActor(), ref defaultSkillEventParam2, GameSkillEventChannel.Channel_HostCtrlActor);
			}
			if (bForce)
			{
				this.actor.SkillControl.SetForceDisableSkillSlot(_type, true);
			}
		}

		public override void RmvDisableSkillFlag(SkillSlotType _type, bool bForce = false)
		{
			base.RmvDisableSkillFlag(_type, false);
			if (_type == SkillSlotType.SLOT_SKILL_COUNT)
			{
				for (int i = 0; i < 10; i++)
				{
					if (this.actor.SkillControl.DisableSkill[i] == 0 && (!this.actor.SkillControl.IsIngnoreDisableSkill((SkillSlotType)i) || bForce))
					{
						DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam((SkillSlotType)i, 0, this.actorPtr);
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, base.GetActor(), ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
					}
				}
			}
			else if (this.actor.SkillControl.DisableSkill[(int)_type] == 0 && (!this.actor.SkillControl.IsIngnoreDisableSkill(_type) || bForce))
			{
				DefaultSkillEventParam defaultSkillEventParam2 = new DefaultSkillEventParam(_type, 0, this.actorPtr);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, base.GetActor(), ref defaultSkillEventParam2, GameSkillEventChannel.Channel_HostCtrlActor);
			}
			if (bForce)
			{
				this.actor.SkillControl.SetForceDisableSkillSlot(_type, false);
			}
		}

		public override void Revive(bool auto)
		{
			base.Revive(auto);
		}

		public override bool IsBossOrHeroAutoAI()
		{
			return this.myBehavior == ObjBehaviMode.State_AutoAI;
		}

		public override BaseAttackMode GetCurrentAttackMode()
		{
			if (this.CurOpMode == OperateMode.DefaultMode)
			{
				return this.actor.DefaultAttackModeControl;
			}
			return this.actor.LockTargetAttackModeControl;
		}

		public void CaptainHostSwitch()
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(this.hostActor.handle.TheActorMeta.PlayerId);
			if (player != null)
			{
				bool isAutoAI = this.actor.ActorControl.m_isAutoAI;
				player.SetCaptain((uint)this.hostActor.handle.TheActorMeta.ConfigId);
				player.Captain.handle.ActorControl.SetAutoAI(isAutoAI);
				if (ActorHelper.IsHostCtrlActor(ref this.hostActor))
				{
					this.hostActor.handle.HudControl.IsFixedBloodPos = false;
					this.UnInitHostActorFixedHud();
					DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.hostActor, this.hostActor);
					Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, ref defaultGameEventParam);
				}
			}
		}

		public void CaptainCallActorSwitch()
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(this.actorPtr.handle.TheActorMeta.PlayerId);
			if (player != null)
			{
				bool flag = ActorHelper.IsHostCtrlActor(ref this.hostActor);
				bool isAutoAI = this.hostActor.handle.ActorControl.m_isAutoAI;
				player.Captain.handle.ActorControl.ClearMoveCommand();
				player.Captain.handle.ActorControl.SetSelected(false);
				player.Captain.handle.ActorControl.m_isControledByMan = true;
				player.Captain.handle.ActorControl.SetAutoAI(false);
				if (flag)
				{
					player.Captain.handle.HudControl.SetSelected(false);
					this.SpawnHostActorFixedHud();
				}
				player.SetCallCaptain((uint)this.actorPtr.handle.TheActorMeta.ConfigId, this.actorPtr.handle.ObjID);
				player.Captain.handle.ActorControl.SetSelected(true);
				player.Captain.handle.ActorControl.SetAutoAI(isAutoAI);
				if (flag)
				{
					player.Captain.handle.HudControl.SetSelected(true);
					DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.actorPtr, this.actorPtr);
					Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, ref defaultGameEventParam);
				}
			}
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			if (prm.src == this.hostActor && this.actorPtr)
			{
				this.actor.Suicide();
			}
		}

		protected override void OnBehaviModeChange(ObjBehaviMode oldState, ObjBehaviMode curState)
		{
			base.OnBehaviModeChange(oldState, curState);
			if (curState > ObjBehaviMode.Direction_Move && curState < ObjBehaviMode.State_AutoAI)
			{
				this.IsTrueType = true;
			}
		}

		private void LeaveImposterMesh()
		{
			this.actor.ActorMesh.CustomSetActive(false);
			this.actor.RecoverOriginalActorMesh();
			this.actor.ActorMesh.transform.localRotation = Quaternion.identity;
		}

		private void SpawnHostActorFixedHud()
		{
			if (!this.hostActor || !ActorHelper.IsHostCtrlActor(ref this.hostActor))
			{
				return;
			}
			if (this.hostActor.handle.FixedHudControl == null)
			{
				this.hostActor.handle.FixedHudControl = this.hostActor.handle.CreateLogicComponent<HudComponent3D>(this.hostActor);
			}
			if (this.hostActor.handle.FixedHudControl != null)
			{
				this.hostActor.handle.FixedHudControl.Init();
				this.hostActor.handle.FixedHudControl.Prepare();
				this.hostActor.handle.FixedHudControl.Fight();
				this.hostActor.handle.FixedHudControl.SetSelected(false);
				this.hostActor.handle.FixedHudControl.FixActorBloodPos();
			}
		}

		private void UnInitHostActorFixedHud()
		{
			if (!this.hostActor || !ActorHelper.IsHostCtrlActor(ref this.hostActor))
			{
				return;
			}
			if (this.hostActor.handle.FixedHudControl != null)
			{
				this.hostActor.handle.FixedHudControl.FightOver();
				this.hostActor.handle.FixedHudControl.Uninit();
			}
		}
	}
}
