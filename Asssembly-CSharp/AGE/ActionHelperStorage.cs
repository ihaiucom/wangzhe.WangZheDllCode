using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[Serializable]
	public class ActionHelperStorage
	{
		public string helperName = string.Empty;

		public string actionName = string.Empty;

		public bool playOnStart;

		public string detectStatePath = string.Empty;

		public bool waitForEvents;

		public bool autoPlay = true;

		public bool stopConflictActions;

		public GameObject[] targets = new GameObject[0];

		private int detectStatePathHash;

		private Action lastAction;

		private int lastActionFrame = -1;

		public int GetDetectStatePathHash()
		{
			if (this.detectStatePathHash == 0)
			{
				this.detectStatePathHash = Animator.StringToHash(this.detectStatePath);
			}
			return this.detectStatePathHash;
		}

		public bool IsLastActionActive()
		{
			if (!ActionManager.Instance.IsActionValid(this.lastAction))
			{
				this.lastAction = null;
			}
			return this.lastAction != null;
		}

		public void StopLastAction()
		{
			if (this.lastAction != null)
			{
				if (ActionManager.Instance.IsActionValid(this.lastAction))
				{
					this.lastAction.Stop(false);
				}
				this.lastAction = null;
			}
		}

		public Action PlayAction()
		{
			if (Time.frameCount <= this.lastActionFrame + 1)
			{
				return null;
			}
			this.lastAction = ActionManager.Instance.PlayAction(this.actionName, this.autoPlay, this.stopConflictActions, this.targets);
			this.lastActionFrame = Time.frameCount;
			return this.lastAction;
		}

		public Action PlayActionEx(params GameObject[] _gameObjects)
		{
			if (Time.frameCount <= this.lastActionFrame + 1)
			{
				return null;
			}
			this.lastAction = ActionManager.Instance.PlayAction(this.actionName, this.autoPlay, this.stopConflictActions, _gameObjects);
			this.lastActionFrame = Time.frameCount;
			return this.lastAction;
		}

		public Action PlayAction(DictionaryView<string, GameObject> dictionary)
		{
			Action action = ActionManager.Instance.LoadActionResource(this.actionName);
			if (action == null)
			{
				return null;
			}
			GameObject[] array = (GameObject[])this.targets.Clone();
			foreach (KeyValuePair<string, GameObject> current in dictionary)
			{
				int num = -1;
				bool flag = action.TemplateObjectIds.TryGetValue(current.get_Key(), ref num);
				if (flag)
				{
					array[num] = current.get_Value();
				}
			}
			if (Time.frameCount <= this.lastActionFrame + 1)
			{
				return null;
			}
			this.lastAction = ActionManager.Instance.PlayAction(this.actionName, this.autoPlay, this.stopConflictActions, array);
			this.lastActionFrame = Time.frameCount;
			return this.lastAction;
		}
	}
}
