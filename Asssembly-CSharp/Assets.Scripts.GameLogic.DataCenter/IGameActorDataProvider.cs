using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	public interface IGameActorDataProvider
	{
		bool GetActorStaticData(ref ActorMeta actorMeta, ref ActorStaticData actorData);

		bool GetActorStaticSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData);

		bool GetActorStaticPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData);

		bool GetActorServerData(ref ActorMeta actorMeta, ref ActorServerData actorData);

		bool GetActorServerSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorServerSkillData skillData);

		bool GetActorServerEquipData(ref ActorMeta actorMeta, ActorEquiplSlot equipSlot, ref ActorServerEquipData skillData);

		bool GetActorServerRuneData(ref ActorMeta actorMeta, ActorRunelSlot runeSlot, ref ActorServerRuneData runeData);

		bool GetActorServerCommonSkillData(ref ActorMeta actorMeta, out uint skillID);

		int Fast_GetActorServerDataBornIndex(ref ActorMeta actorMeta);
	}
}
