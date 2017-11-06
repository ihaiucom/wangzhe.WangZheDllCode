using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct HurtAttackerInfo
	{
		public int iActorLvl;

		public int iActorATT;

		public int iActorINT;

		public int iActorMaxHp;

		public int iDEFStrike;

		public int iRESStrike;

		public int iDEFStrikeRate;

		public int iRESStrikeRate;

		public int iFinalHurt;

		public int iCritStrikeRate;

		public int iCritStrikeValue;

		public int iReduceCritStrikeRate;

		public int iReduceCritStrikeValue;

		public int iCritStrikeEff;

		public int iPhysicsHemophagiaRate;

		public int iMagicHemophagiaRate;

		public int iPhysicsHemophagia;

		public int iMagicHemophagia;

		public int iHurtOutputRate;

		public ActorTypeDef actorType;

		public void Init(PoolObjHandle<ActorRoot> _atker, PoolObjHandle<ActorRoot> _target)
		{
			PoolObjHandle<ActorRoot> poolObjHandle = _atker;
			if (_atker)
			{
				MonsterWrapper monsterWrapper = _atker.handle.ActorControl as MonsterWrapper;
				if (monsterWrapper != null && monsterWrapper.isCalledMonster && monsterWrapper.UseHostValueProperty)
				{
					poolObjHandle = monsterWrapper.hostActor;
				}
				this.iActorLvl = poolObjHandle.handle.ValueComponent.mActorValue.actorLvl;
				this.iActorATT = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
				this.iActorINT = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
				this.iActorMaxHp = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
				this.iDEFStrike = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYARMORHURT].totalValue;
				this.iRESStrike = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCARMORHURT].totalValue;
				this.iDEFStrikeRate = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_PHYARMORHURT_RATE].totalValue;
				this.iRESStrikeRate = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MGCARMORHURT_RATE].totalValue;
				this.iFinalHurt = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURT].totalValue;
				this.iCritStrikeRate = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITRATE].totalValue;
				this.iCritStrikeValue = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_CRITICAL].totalValue;
				this.iCritStrikeEff = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITEFT].totalValue;
				this.iMagicHemophagia = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAGICHEM].totalValue;
				this.iPhysicsHemophagia = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_PHYSICSHEM].totalValue;
				this.iMagicHemophagiaRate = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCVAMP].totalValue;
				this.iPhysicsHemophagiaRate = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYVAMP].totalValue;
				this.iHurtOutputRate = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_HURTOUTPUTRATE].totalValue;
				this.actorType = poolObjHandle.handle.TheActorMeta.ActorType;
			}
			else if (_target)
			{
				this.iReduceCritStrikeRate = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ANTICRIT].totalValue;
				this.iReduceCritStrikeValue = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_REDUCECRITICAL].totalValue;
			}
		}
	}
}
