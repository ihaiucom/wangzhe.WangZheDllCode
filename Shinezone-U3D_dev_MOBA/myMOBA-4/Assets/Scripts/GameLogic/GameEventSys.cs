using Assets.Scripts.Framework;
using System;

namespace Assets.Scripts.GameLogic
{
	public class GameEventSys : Singleton<GameEventSys>
	{
		private Delegate[] ActionTable = new Delegate[55];

		private ListView<IPostEventWrapper> postEventList = new ListView<IPostEventWrapper>();

		private bool CheckValidAdd(GameEventDef evt, Delegate handler)
		{
			Delegate @delegate = this.ActionTable[(int)evt];
			return @delegate == null || @delegate.GetType() == handler.GetType();
		}

		private bool CheckValidRmv(GameEventDef evt, Delegate handler)
		{
			Delegate @delegate = this.ActionTable[(int)evt];
			return @delegate != null && @delegate.GetType() == handler.GetType();
		}

		public void AddEventHandler(GameEventDef evt, Action handler)
		{
			if (this.CheckValidAdd(evt, handler))
			{
				this.ActionTable[(int)evt] = (Action)Delegate.Combine((Action)this.ActionTable[(int)evt], handler);
			}
		}

		public void RmvEventHandler(GameEventDef evt, Action handler)
		{
			if (this.CheckValidRmv(evt, handler))
			{
				this.ActionTable[(int)evt] = (Action)Delegate.Remove((Action)this.ActionTable[(int)evt], handler);
			}
		}

		public void AddEventHandler<ParamType>(GameEventDef evt, RefAction<ParamType> handler)
		{
			if (this.CheckValidAdd(evt, handler))
			{
				this.ActionTable[(int)evt] = (RefAction<ParamType>)Delegate.Combine((RefAction<ParamType>)this.ActionTable[(int)evt], handler);
			}
		}

		public void RmvEventHandler<ParamType>(GameEventDef evt, RefAction<ParamType> handler)
		{
			if (this.CheckValidRmv(evt, handler))
			{
				this.ActionTable[(int)evt] = (RefAction<ParamType>)Delegate.Remove((RefAction<ParamType>)this.ActionTable[(int)evt], handler);
			}
		}

		public void UpdateEvent()
		{
			if (this.postEventList.Count > 0)
			{
				int i = 0;
				while (i < this.postEventList.Count)
				{
					uint curFrameNum = Singleton<FrameSynchr>.instance.CurFrameNum;
					IPostEventWrapper postEventWrapper = this.postEventList[i];
					if (postEventWrapper.GetFrameNum() >= curFrameNum)
					{
						postEventWrapper.ExecCommand();
						this.postEventList.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		public void PostEvent<ParamType>(GameEventDef evt, ref ParamType prm)
		{
			RefAction<ParamType> refAction = this.ActionTable[(int)evt] as RefAction<ParamType>;
			if (refAction != null)
			{
				PostEventWrapper<ParamType> postEventWrapper = new PostEventWrapper<ParamType>(refAction, prm, 1u);
				this.postEventList.Add(postEventWrapper);
			}
		}

		public void SendEvent(GameEventDef evt)
		{
			Action action = this.ActionTable[(int)evt] as Action;
			if (action != null)
			{
				action();
			}
		}

		public void SendEvent<ParamType>(GameEventDef evt, ref ParamType prm)
		{
			RefAction<ParamType> refAction = this.ActionTable[(int)evt] as RefAction<ParamType>;
			if (refAction != null)
			{
				refAction(ref prm);
			}
		}
	}
}
