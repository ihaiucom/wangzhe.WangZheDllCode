using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using System;

namespace Assets.Scripts.GameLogic
{
	public class GameSkillEventSys : Singleton<GameSkillEventSys>
	{
		public Delegate[] skillEventTable = new Delegate[30];

		public void AddEventHandler<ParamType>(GameSkillEventDef _event, GameSkillEvent<ParamType> _handler)
		{
			this.skillEventTable[(int)_event] = (GameSkillEvent<ParamType>)Delegate.Combine((GameSkillEvent<ParamType>)this.skillEventTable[(int)_event], _handler);
		}

		public void RmvEventHandler<ParamType>(GameSkillEventDef _event, GameSkillEvent<ParamType> _handler)
		{
			this.skillEventTable[(int)_event] = (GameSkillEvent<ParamType>)Delegate.Remove((GameSkillEvent<ParamType>)this.skillEventTable[(int)_event], _handler);
		}

		private void SendEvent<ParamType>(GameSkillEventDef _event, ref ParamType _param)
		{
			GameSkillEvent<ParamType> gameSkillEvent = this.skillEventTable[(int)_event] as GameSkillEvent<ParamType>;
			if (gameSkillEvent != null)
			{
				gameSkillEvent(ref _param);
			}
		}

		public void SendEvent<ParamType>(GameSkillEventDef _event, PoolObjHandle<ActorRoot> _src, ref ParamType _param, GameSkillEventChannel _channel = GameSkillEventChannel.Channel_HostCtrlActor)
		{
			if (!_src)
			{
				return;
			}
			if (_channel == GameSkillEventChannel.Channel_HostCtrlActor)
			{
				if (ActorHelper.IsHostCtrlActor(ref _src) || Singleton<WatchController>.GetInstance().IsWatching)
				{
					this.SendEvent<ParamType>(_event, ref _param);
				}
			}
			else if (_channel == GameSkillEventChannel.Channel_HostActor)
			{
				if (ActorHelper.IsHostActor(ref _src) || Singleton<WatchController>.GetInstance().IsWatching)
				{
					this.SendEvent<ParamType>(_event, ref _param);
				}
			}
			else if (_channel == GameSkillEventChannel.Channel_AllActor)
			{
				this.SendEvent<ParamType>(_event, ref _param);
			}
		}
	}
}
