using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using CSProtocol;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	internal class SkillHidingDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[AssetReference(AssetRefType.SkillCombine)]
		public int skillCombineID;

		private PoolObjHandle<ActorRoot> targetActor;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillHidingDuration skillHidingDuration = src as SkillHidingDuration;
			this.targetId = skillHidingDuration.targetId;
			this.skillCombineID = skillHidingDuration.skillCombineID;
		}

		public override BaseEvent Clone()
		{
			SkillHidingDuration skillHidingDuration = ClassObjPool<SkillHidingDuration>.Get();
			skillHidingDuration.CopyData(this);
			return skillHidingDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetActor.Release();
		}

		public override void Enter(Action _action, Track _track)
		{
			this.targetActor = _action.GetActorHandle(this.targetId);
			if (!this.targetActor)
			{
				return;
			}
			this.targetActor.handle.HorizonMarker.AddHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.HideMark.Skill, 1, false);
			this.targetActor.handle.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, true, false);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (!this.targetActor)
			{
				return;
			}
			COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(this.targetActor.handle.TheActorMeta.ActorCamp);
			for (int i = 0; i < othersCmp.Length; i++)
			{
				if (this.targetActor.handle.HorizonMarker.HasHideMark(othersCmp[i], HorizonConfig.HideMark.Skill))
				{
					this.targetActor.handle.HorizonMarker.AddHideMark(othersCmp[i], HorizonConfig.HideMark.Skill, -1, false);
				}
			}
			this.targetActor.handle.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, false, false);
			if (this.skillCombineID != 0)
			{
				SkillUseParam skillUseParam = default(SkillUseParam);
				skillUseParam.SetOriginator(this.targetActor);
				this.targetActor.handle.SkillControl.SpawnBuff(this.targetActor, ref skillUseParam, this.skillCombineID, true);
			}
		}
	}
}
