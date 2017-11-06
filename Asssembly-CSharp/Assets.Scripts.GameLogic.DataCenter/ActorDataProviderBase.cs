using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	internal class ActorDataProviderBase : IGameActorDataProvider
	{
		public virtual bool GetActorStaticData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			return false;
		}

		public virtual bool GetActorStaticSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
		{
			return false;
		}

		public virtual bool GetActorStaticPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
		{
			return false;
		}

		public virtual bool GetActorServerData(ref ActorMeta actorMeta, ref ActorServerData actorData)
		{
			return false;
		}

		public virtual bool GetActorServerSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorServerSkillData skillData)
		{
			return false;
		}

		public virtual bool GetActorServerEquipData(ref ActorMeta actorMeta, ActorEquiplSlot equipSlot, ref ActorServerEquipData equipData)
		{
			return false;
		}

		public virtual bool GetActorServerRuneData(ref ActorMeta actorMeta, ActorRunelSlot runeSlot, ref ActorServerRuneData runeData)
		{
			return false;
		}

		public virtual bool GetActorServerCommonSkillData(ref ActorMeta actorMeta, out uint skillID)
		{
			skillID = 0u;
			return false;
		}

		public virtual void Init()
		{
		}

		protected void ErrorMissingHeroConfig(uint configId)
		{
		}

		protected void ErrorMissingMonsterConfig(uint configId)
		{
		}

		protected void ErrorMissingOrganConfig(uint configId)
		{
		}

		protected void ErrorMissingLevelConfig(uint configId)
		{
		}

		public virtual int Fast_GetActorServerDataBornIndex(ref ActorMeta actorMeta)
		{
			return -1;
		}
	}
}
