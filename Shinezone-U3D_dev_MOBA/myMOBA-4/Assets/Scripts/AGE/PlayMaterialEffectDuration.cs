using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Material")]
	public class PlayMaterialEffectDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public MaterialEffectType effectType;

		private PoolObjHandle<ActorRoot> actor_;

		private int playingId = -1;

		public Vector3 highLitColor;

		private int hlcId;

		public override void OnRelease()
		{
			base.OnRelease();
			this.actor_.Release();
			this.playingId = -1;
			this.targetId = 0;
			this.hlcId = 0;
		}

		public override BaseEvent Clone()
		{
			PlayMaterialEffectDuration playMaterialEffectDuration = ClassObjPool<PlayMaterialEffectDuration>.Get();
			playMaterialEffectDuration.CopyData(this);
			return playMaterialEffectDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PlayMaterialEffectDuration playMaterialEffectDuration = src as PlayMaterialEffectDuration;
			this.targetId = playMaterialEffectDuration.targetId;
			this.effectType = playMaterialEffectDuration.effectType;
			this.highLitColor = playMaterialEffectDuration.highLitColor;
		}

		public override void Enter(Action _action, Track _track)
		{
			this.actor_ = _action.GetActorHandle(this.targetId);
			if (!this.actor_)
			{
				return;
			}
			MaterialHurtEffect matHurtEffect = this.actor_.handle.MatHurtEffect;
			if (matHurtEffect == null)
			{
				this.actor_.Release();
				return;
			}
			switch (this.effectType)
			{
			case MaterialEffectType.Freeze:
				this.playingId = matHurtEffect.PlayFreezeEffect();
				break;
			case MaterialEffectType.Stone:
				this.playingId = matHurtEffect.PlayStoneEffect();
				break;
			case MaterialEffectType.Translucent:
				matHurtEffect.SetTranslucent(true, false);
				break;
			case MaterialEffectType.HighLit:
				this.hlcId = matHurtEffect.PlayHighLitEffect(this.highLitColor);
				break;
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (!this.actor_)
			{
				return;
			}
			MaterialHurtEffect matHurtEffect = this.actor_.handle.MatHurtEffect;
			if (matHurtEffect == null)
			{
				return;
			}
			switch (this.effectType)
			{
			case MaterialEffectType.Freeze:
				matHurtEffect.StopFreezeEffect(this.playingId);
				break;
			case MaterialEffectType.Stone:
				matHurtEffect.StopStoneEffect(this.playingId);
				break;
			case MaterialEffectType.Translucent:
				matHurtEffect.SetTranslucent(false, false);
				break;
			case MaterialEffectType.HighLit:
				matHurtEffect.StopHighLitEffect(this.hlcId);
				break;
			}
		}
	}
}
