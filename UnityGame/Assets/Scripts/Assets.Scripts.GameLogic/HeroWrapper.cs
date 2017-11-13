using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using behaviac;
using ResData;
using System;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class HeroWrapper : ObjWrapper
	{
		private int multiKillNum;

		private int contiKillNum;

		private int contiDeadNum;

		private bool autoRevived;

		public bool bGodMode;

		private uint[] m_talentArr = new uint[5];

		private uint m_skinCfgId;

		private uint m_skinId;

		private HeroProficiency m_heroProficiency;

		protected PoolObjHandle<ActorRoot> callMonster = new PoolObjHandle<ActorRoot>(null);

		private bool bDeadLevelUp;

		private string skillEffectPath = string.Empty;

		public OperateMode CurOpMode;

		public PoolObjHandle<ActorRoot> callActor = new PoolObjHandle<ActorRoot>(null);

		public bool hasCalledMonster
		{
			get
			{
				return this.callMonster;
			}
		}

		public PoolObjHandle<ActorRoot> CallMonster
		{
			get
			{
				return this.hasCalledMonster ? this.callMonster : base.GetOrignalActor();
			}
			set
			{
				this.callMonster = value;
			}
		}

		public override int CfgReviveCD
		{
			get
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				return Singleton<BattleLogic>.instance.dynamicProperty.GetDynamicReviveTime(curLvelContext.m_dynamicPropertyConfig, curLvelContext.m_baseReviveTime);
			}
		}

		public int MultiKillNum
		{
			get
			{
				return this.multiKillNum;
			}
			set
			{
				this.multiKillNum = value;
			}
		}

		public int ContiKillNum
		{
			get
			{
				return this.contiKillNum;
			}
			set
			{
				this.contiKillNum = value;
			}
		}

		public int ContiDeadNum
		{
			get
			{
				return this.contiDeadNum;
			}
			set
			{
				this.contiDeadNum = value;
			}
		}

		public uint[] GetTalentArr
		{
			get
			{
				return this.m_talentArr;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			if (this.m_heroProficiency != null)
			{
				this.m_heroProficiency.UnInit();
			}
			this.m_heroProficiency = null;
			this.multiKillNum = 0;
			this.contiKillNum = 0;
			this.contiDeadNum = 0;
			this.bGodMode = false;
			this.autoRevived = false;
			byte b = 0;
			while ((int)b < this.m_talentArr.Length)
			{
				this.m_talentArr[(int)b] = 0u;
				b += 1;
			}
			this.m_skinCfgId = 0u;
			this.m_skinId = 0u;
			this.bDeadLevelUp = false;
			this.skillEffectPath = string.Empty;
			this.CurOpMode = OperateMode.DefaultMode;
			if (this.callActor)
			{
				this.callActor.Release();
			}
			this.callMonster.Release();
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			this.actor.MovementComponent = this.actor.CreateLogicComponent<PlayerMovement>(this.actor);
			this.actor.MatHurtEffect = this.actor.CreateActorComponent<MaterialHurtEffect>(this.actor);
			this.actor.EffectControl = this.actor.CreateLogicComponent<EffectPlayComponent>(this.actor);
			this.actor.EquipComponent = this.actor.CreateLogicComponent<EquipComponent>(this.actor);
			this.actor.ShadowEffect = this.actor.CreateActorComponent<UpdateShadowPlane>(this.actor);
			this.actor.PetControl = this.actor.CreateLogicComponent<PetComponent>(this.actor);
			VCollisionShape.InitActorCollision(this.actor);
			this.actor.DefaultAttackModeControl = this.actor.CreateLogicComponent<DefaultAttackMode>(this.actor);
			this.actor.LockTargetAttackModeControl = this.actor.CreateLogicComponent<LockTargetAttackMode>(this.actor);
			this.m_heroProficiency = new HeroProficiency();
			this.m_heroProficiency.Init(this);
		}

		public override void Prepare()
		{
			if (this.actor.ActorAgent.IsNeedToPlayBornAge() == EBTStatus.BT_SUCCESS)
			{
				this.actor.Visible = false;
			}
		}

		public override string GetTypeName()
		{
			return "HeroWrapper";
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

		public void SetCallActor(ref PoolObjHandle<ActorRoot> _actor)
		{
			this.callActor = _actor;
		}

		public PoolObjHandle<ActorRoot> GetCallActor()
		{
			return this.callActor;
		}

		public override void SetHelpToAttackTarget(PoolObjHandle<ActorRoot> helpActor, PoolObjHandle<ActorRoot> enemyActor)
		{
			base.SetHelpToAttackTarget(helpActor, enemyActor);
			this.m_needToHelpOtherWait = (int)(1000 + FrameRandom.Random(8u) * 500);
		}

		public override void SetToAttackTarget(PoolObjHandle<ActorRoot> enemyActor, bool ignoreSrchRange = false)
		{
			base.SetToAttackTarget(enemyActor, ignoreSrchRange);
			this.m_needSwithTargetWait = (int)(1000 + FrameRandom.Random(8u) * 500);
		}

		protected override void OnDead()
		{
			base.OnDead();
			if (!this.m_reviveContext.bEnable)
			{
				PoolObjHandle<ActorRoot> ptr = this.myLastAtker ? this.myLastAtker.handle.ActorControl.GetOrignalActor() : this.myLastAtker;
				if ((ptr && ptr.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || base.IsKilledByHero())
				{
					this.contiKillNum = 0;
				}
				this.contiDeadNum++;
			}
			this.actor.SkillControl.SkillUseCache.Clear();
			BaseAttackMode currentAttackMode = this.GetCurrentAttackMode();
			if (currentAttackMode != null)
			{
				currentAttackMode.OnDead();
			}
		}

		protected override void OnRevive()
		{
			VInt3 vInt = VInt3.zero;
			VInt3 forward = VInt3.forward;
			if (this.autoRevived && this.m_reviveContext.bBaseRevive && !base.GetNoAbilityFlag(ObjAbilityType.ObjAbility_DeadControl))
			{
				Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref this.actor.TheActorMeta, false, out vInt, out forward);
				this.actor.EquipComponent.ResetHasLeftEquipBoughtArea();
			}
			else
			{
				Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
				vInt = ownerPlayer.Captain.handle.location;
				forward = ownerPlayer.Captain.handle.forward;
			}
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.actorPtr, this.actorPtr);
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, ref defaultGameEventParam);
			VInt groundY;
			if (PathfindingUtility.GetGroundY(vInt, out groundY))
			{
				this.actor.groundY = groundY;
				vInt.y = groundY.i;
			}
			this.actor.forward = forward;
			this.actor.location = vInt;
			this.actor.ObjLinker.SetForward(forward, -1);
			base.OnRevive();
			if (!this.actor.ActorAgent.IsAutoAI())
			{
				base.SetObjBehaviMode(ObjBehaviMode.State_Revive);
			}
			Player ownerPlayer2 = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
			if (ownerPlayer2 != null)
			{
				Singleton<EventRouter>.instance.BroadCastEvent<Player>(EventID.PlayerReviveTime, ownerPlayer2);
			}
			if (this.bDeadLevelUp)
			{
				this.OnAdvanceSkin();
				this.bDeadLevelUp = false;
			}
		}

		public override void Deactive()
		{
			if (this.m_heroProficiency != null)
			{
				this.m_heroProficiency.UnInit();
			}
			this.m_heroProficiency = null;
			base.Deactive();
		}

		protected override void OnSoulLvlChange()
		{
			if (this.m_skinId > 0u && this.actor.ValueComponent.actorSoulLevel > 1)
			{
				if (base.IsDeadState)
				{
					this.bDeadLevelUp = true;
				}
				else
				{
					this.OnAdvanceSkin();
				}
			}
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
			if (this.bGodMode)
			{
				return 0;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && hurt.atker && hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				OrganWrapper organWrapper = hurt.atker.handle.AsOrgan();
				if (organWrapper != null)
				{
					int attackCounter = organWrapper.GetAttackCounter(this.actorPtr);
					if (attackCounter > 1)
					{
						int num = (attackCounter - 1) * organWrapper.cfgInfo.iContiAttakAdd;
						if (num > organWrapper.cfgInfo.iContiAttakMax)
						{
							num = organWrapper.cfgInfo.iContiAttakMax;
						}
						hurt.adValue += num;
					}
				}
			}
			return base.TakeDamage(ref hurt);
		}

		public override void UpdateLogic(int nDelta)
		{
			this.actor.ActorAgent.UpdateLogic(nDelta);
			base.UpdateLogic(nDelta);
			this.m_heroProficiency.UpdateLogic(nDelta);
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
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			bool flag = curLvelContext != null && curLvelContext.IsMobaMode();
			if (flag == auto)
			{
				this.autoRevived = auto;
				base.Revive(auto);
			}
		}

		public override bool IsBossOrHeroAutoAI()
		{
			return this.myBehavior == ObjBehaviMode.State_AutoAI;
		}

		public override void CmdCommonLearnSkill(IFrameCommand cmd)
		{
			FrameCommand<LearnSkillCommand> frameCommand = (FrameCommand<LearnSkillCommand>)cmd;
			if (!Singleton<BattleLogic>.instance.IsMatchLearnSkillRule(this.actorPtr, (SkillSlotType)frameCommand.cmdData.bSlotType))
			{
				return;
			}
			if (this.actor.SkillControl.m_iSkillPoint > 0)
			{
				this.actor.SkillControl.m_iSkillPoint--;
				PoolObjHandle<ActorRoot> actorPtr = this.actorPtr;
				if (this.actor.SkillControl.m_iSkillPoint >= 0)
				{
					SkillSlot skillSlot;
					actorPtr.handle.SkillControl.TryGetSkillSlot((SkillSlotType)frameCommand.cmdData.bSlotType, out skillSlot);
					if (skillSlot != null)
					{
						int skillLevel = skillSlot.GetSkillLevel();
						if (skillLevel != (int)frameCommand.cmdData.bSkillLevel)
						{
							return;
						}
						skillSlot.SetSkillLevel(skillLevel + 1);
						if (this.callActor && this.callActor.handle.SkillControl != null)
						{
							this.callActor.handle.SkillControl.TryGetSkillSlot((SkillSlotType)frameCommand.cmdData.bSlotType, out skillSlot);
							if (skillSlot != null)
							{
								skillSlot.SetSkillLevel(skillLevel + 1);
							}
						}
						Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", actorPtr, frameCommand.cmdData.bSlotType, (byte)(skillLevel + 1));
					}
				}
				return;
			}
		}

		public override BaseAttackMode GetCurrentAttackMode()
		{
			if (this.CurOpMode == OperateMode.DefaultMode)
			{
				return this.actor.DefaultAttackModeControl;
			}
			return this.actor.LockTargetAttackModeControl;
		}
	}
}
