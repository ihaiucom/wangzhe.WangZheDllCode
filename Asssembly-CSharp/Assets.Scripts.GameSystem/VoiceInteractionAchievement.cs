using Assets.Scripts.GameLogic;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	[VoiceInteraction(4)]
	public class VoiceInteractionAchievement : VoiceInteraction
	{
		private int achievementType
		{
			get
			{
				return (int)this.InteractionCfg.SpecialTriggerConditions[0];
			}
		}

		public override void Init(ResVoiceInteraction InInteractionCfg)
		{
			base.Init(InInteractionCfg);
			Singleton<EventRouter>.instance.AddEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
		}

		public override void Unit()
		{
			Singleton<EventRouter>.instance.RemoveEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
			base.Unit();
		}

		private void OnAchievementEvent(KillDetailInfo DetailInfo)
		{
			if (!this.ForwardCheck())
			{
				return;
			}
			if (DetailInfo.Killer && DetailInfo.Killer.handle.TheActorMeta.ConfigId == base.groupID && this.achievementType == (int)DetailInfo.Type)
			{
				this.TryTrigger(ref DetailInfo.Killer, ref DetailInfo.Victim, ref DetailInfo.Killer);
			}
		}
	}
}
