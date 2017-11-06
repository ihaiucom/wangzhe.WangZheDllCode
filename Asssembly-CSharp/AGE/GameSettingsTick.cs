using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/GameSettings")]
	public class GameSettingsTick : TickEvent
	{
		public CastType UseCastType;

		public CommonAttactType CommonAttactType;

		public bool bResetCastType;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			GameSettingsTick gameSettingsTick = src as GameSettingsTick;
			this.UseCastType = gameSettingsTick.UseCastType;
			this.bResetCastType = gameSettingsTick.bResetCastType;
			this.CommonAttactType = gameSettingsTick.CommonAttactType;
		}

		public override BaseEvent Clone()
		{
			GameSettingsTick gameSettingsTick = ClassObjPool<GameSettingsTick>.Get();
			gameSettingsTick.CopyData(this);
			return gameSettingsTick;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			if (this.bResetCastType)
			{
				Singleton<GameInput>.instance.SetSmartUse(GameSettings.TheCastType == CastType.SmartCast);
			}
			else
			{
				Singleton<GameInput>.instance.SetSmartUse(this.UseCastType == CastType.SmartCast);
			}
			GameSettings.TheCommonAttackType = this.CommonAttactType;
		}
	}
}
