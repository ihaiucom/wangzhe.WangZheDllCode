using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class PlaySoundTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[AssetReference(AssetRefType.Sound)]
		public string eventName = string.Empty;

		public override BaseEvent Clone()
		{
			PlaySoundTick playSoundTick = ClassObjPool<PlaySoundTick>.Get();
			playSoundTick.CopyData(this);
			return playSoundTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PlaySoundTick playSoundTick = src as PlaySoundTick;
			this.targetId = playSoundTick.targetId;
			this.eventName = playSoundTick.eventName;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.eventName = string.Empty;
		}

		public override void Process(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			Singleton<CSoundManager>.instance.PlayBattleSound(this.eventName, actorHandle, gameObject);
		}
	}
}
