using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.DamagePassiveCondition)]
	public class PassiveHurtCondition : PassiveCondition
	{
		private bool bHurt;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bHurt = false;
			base.Init(_source, _event, ref _config);
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
		}

		public override void UnInit()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
		}

		public override void Reset()
		{
			this.bHurt = false;
		}

		private bool CheckSkillSlot(ref HurtEventResultInfo info)
		{
			bool result = false;
			if (this.localParams[1] == (int)info.hurtInfo.atkSlot)
			{
				result = true;
			}
			return result;
		}

		private bool CheckAttackType(ref HurtEventResultInfo info)
		{
			bool result = false;
			if (this.localParams[0] == 0)
			{
				result = true;
			}
			else if (this.localParams[0] == 1)
			{
				if (info.atker && info.atker.handle.TheStaticData.TheHeroOnlyInfo.AttackDistanceType == 1)
				{
					result = this.CheckSkillSlot(ref info);
				}
			}
			else if (this.localParams[0] == 2 && info.atker && info.atker.handle.TheStaticData.TheHeroOnlyInfo.AttackDistanceType == 2)
			{
				result = this.CheckSkillSlot(ref info);
			}
			return result;
		}

		private bool CheckDamageRateForHp(ref HurtEventResultInfo info)
		{
			bool result = false;
			int num = this.localParams[3];
			if (num <= 0)
			{
				result = true;
			}
			else if (info.src && info.src.handle.ValueComponent != null)
			{
				int num2 = -info.hpChanged;
				if (num2 > num * (info.src.handle.ValueComponent.actorHp + num2) / 100)
				{
					result = true;
				}
			}
			return result;
		}

		private void onActorDamage(ref HurtEventResultInfo info)
		{
			if (info.src != this.sourceActor)
			{
				return;
			}
			if (info.hpChanged < 0)
			{
				if (info.hurtInfo.bBounceHurt && this.localParams[2] == 1)
				{
					return;
				}
				bool flag = this.CheckDamageRateForHp(ref info);
				if (flag)
				{
					flag = this.CheckAttackType(ref info);
					if (flag)
					{
						this.bHurt = true;
						if (this.localParams[4] == 1)
						{
							return;
						}
						this.rootEvent.SetTriggerActor(info.atker);
					}
				}
			}
		}

		public override bool Fit()
		{
			return !this.sourceActor.handle.ActorControl.IsDeadState && this.bHurt;
		}
	}
}
