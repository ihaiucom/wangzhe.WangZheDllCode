using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.ActorInGrassCondition)]
	public class ActorInGrassCondition : PassiveCondition
	{
		private bool bTrigger;

		private int InGrassCount;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = false;
			this.InGrassCount = 0;
			base.Init(_source, _event, ref _config);
			Singleton<GameEventSys>.instance.AddEventHandler<ActorInGrassParam>(GameEventDef.Event_ActorInGrass, new RefAction<ActorInGrassParam>(this.SetInGrassStatusByTrigger));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<GameEventSys>.instance.RmvEventHandler<ActorInGrassParam>(GameEventDef.Event_ActorInGrass, new RefAction<ActorInGrassParam>(this.SetInGrassStatusByTrigger));
		}

		public override void Reset()
		{
		}

		public void SetInGrassStatusByFog()
		{
			if (Singleton<GameFowManager>.instance.m_pFieldObj != null)
			{
				this.bTrigger = (Singleton<GameFowManager>.instance.QueryAttr(this.sourceActor.handle.location) == FieldObj.EViewBlockType.Grass);
			}
		}

		public void SetInGrassStatusByTrigger(ref ActorInGrassParam param)
		{
			if (param._src == this.sourceActor)
			{
				this.InGrassCount += (param._bInGrass ? 1 : -1);
				if (this.InGrassCount < 0)
				{
					this.InGrassCount = 0;
				}
				this.bTrigger = (this.InGrassCount > 0);
			}
		}

		public override bool Fit()
		{
			if (FogOfWar.enable)
			{
				this.SetInGrassStatusByFog();
			}
			return this.bTrigger;
		}
	}
}
