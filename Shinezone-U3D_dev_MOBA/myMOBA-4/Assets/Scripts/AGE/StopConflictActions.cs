using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[EventCategory("ActionControl")]
	public class StopConflictActions : TickEvent
	{
		private int[] gameObjectIds = new int[0];

		public override BaseEvent Clone()
		{
			StopConflictActions stopConflictActions = ClassObjPool<StopConflictActions>.Get();
			stopConflictActions.CopyData(this);
			return stopConflictActions;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			StopConflictActions stopConflictActions = src as StopConflictActions;
			this.gameObjectIds = stopConflictActions.gameObjectIds;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.gameObjectIds = new int[0];
		}

		public override void Process(Action _action, Track _track)
		{
			List<GameObject> list = new List<GameObject>();
			int[] array = this.gameObjectIds;
			for (int i = 0; i < array.Length; i++)
			{
				int index = array[i];
				list.Add(_action.GetGameObject(index));
			}
			ListView<Action> listView = new ListView<Action>();
			foreach (GameObject current in list)
			{
				foreach (Action current2 in ActionManager.Instance.objectReferenceSet[current])
				{
					if (current2 != _action && !current2.unstoppable)
					{
						listView.Add(current2);
					}
				}
			}
			foreach (Action current3 in listView)
			{
				current3.Stop(false);
			}
		}
	}
}
