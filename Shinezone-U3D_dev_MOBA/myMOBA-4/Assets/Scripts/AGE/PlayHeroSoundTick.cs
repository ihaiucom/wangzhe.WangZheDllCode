using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class PlayHeroSoundTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[AssetReference(AssetRefType.Sound)]
		public string eventName;

		protected string CachedEventName;

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.eventName = string.Empty;
			this.CachedEventName = string.Empty;
		}

		public override BaseEvent Clone()
		{
			PlayHeroSoundTick playHeroSoundTick = ClassObjPool<PlayHeroSoundTick>.Get();
			playHeroSoundTick.CopyData(this);
			return playHeroSoundTick;
		}

		public override void OnLoaded()
		{
			this.CachedEventName = this.eventName + "_Down";
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PlayHeroSoundTick playHeroSoundTick = src as PlayHeroSoundTick;
			this.targetId = playHeroSoundTick.targetId;
			this.eventName = playHeroSoundTick.eventName;
			this.CachedEventName = playHeroSoundTick.CachedEventName;
		}

		protected bool ShouldUseNormal(Action _action, Track _track, ref PoolObjHandle<ActorRoot> Actor)
		{
			bool flag = Actor && ActorHelper.IsHostCtrlActor(ref Actor);
			if (flag)
			{
				return true;
			}
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			return refParamObject != null && refParamObject.Originator && ActorHelper.IsHostCtrlActor(ref refParamObject.Originator);
		}

		public override void Process(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (this.ShouldUseNormal(_action, _track, ref actorHandle))
			{
				Singleton<CSoundManager>.instance.PlayBattleSound(this.eventName, actorHandle, gameObject);
			}
			else
			{
				Singleton<CSoundManager>.instance.PlayBattleSound(this.CachedEventName, actorHandle, gameObject);
			}
		}
	}
}
