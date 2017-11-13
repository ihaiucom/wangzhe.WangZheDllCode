using System;
using UnityEngine;

namespace AGE
{
	public class ActionHelper : MonoBehaviour
	{
		private Animator animator;

		public ActionHelperStorage[] actionHelpers = new ActionHelperStorage[0];

		private DictionaryView<string, ActionHelperStorage> actionHelperMap = new DictionaryView<string, ActionHelperStorage>();

		public void ForceStart()
		{
			this.actionHelperMap.Clear();
			ActionHelperStorage[] array = this.actionHelpers;
			for (int i = 0; i < array.Length; i++)
			{
				ActionHelperStorage actionHelperStorage = array[i];
				this.actionHelperMap.Add(actionHelperStorage.helperName, actionHelperStorage);
			}
		}

		private void Start()
		{
			this.animator = base.gameObject.GetComponent<Animator>();
			this.actionHelperMap.Clear();
			ActionHelperStorage[] array = this.actionHelpers;
			for (int i = 0; i < array.Length; i++)
			{
				ActionHelperStorage actionHelperStorage = array[i];
				this.actionHelperMap.Add(actionHelperStorage.helperName, actionHelperStorage);
				actionHelperStorage.autoPlay = true;
				if (actionHelperStorage.playOnStart)
				{
					actionHelperStorage.PlayAction();
				}
			}
		}

		private void Update()
		{
			if (this.animator)
			{
				ActionHelperStorage[] array = this.actionHelpers;
				for (int i = 0; i < array.Length; i++)
				{
					ActionHelperStorage actionHelperStorage = array[i];
					if (actionHelperStorage.detectStatePath.get_Length() > 0)
					{
						bool flag = false;
						for (int j = 0; j < this.animator.layerCount; j++)
						{
							if (this.animator.GetCurrentAnimatorStateInfo(j).nameHash == actionHelperStorage.GetDetectStatePathHash())
							{
								flag = true;
								break;
							}
							AnimatorStateInfo nextAnimatorStateInfo = this.animator.GetNextAnimatorStateInfo(j);
							if (this.animator.IsInTransition(j) && nextAnimatorStateInfo.nameHash == actionHelperStorage.GetDetectStatePathHash())
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							if (!actionHelperStorage.waitForEvents && !actionHelperStorage.IsLastActionActive())
							{
								actionHelperStorage.PlayAction();
							}
						}
						else
						{
							actionHelperStorage.StopLastAction();
						}
					}
				}
			}
		}

		public void BeginAction(string _actionHelperName)
		{
			ActionHelperStorage actionHelperStorage = null;
			if (!this.actionHelperMap.TryGetValue(_actionHelperName, out actionHelperStorage))
			{
				return;
			}
			if (!actionHelperStorage.waitForEvents)
			{
				return;
			}
			Animator component = base.gameObject.GetComponent<Animator>();
			if (component == null)
			{
				return;
			}
			if (actionHelperStorage.detectStatePath.get_Length() > 0)
			{
				bool flag = false;
				for (int i = 0; i < component.layerCount; i++)
				{
					if (component.GetCurrentAnimatorStateInfo(i).nameHash == actionHelperStorage.GetDetectStatePathHash())
					{
						flag = true;
						break;
					}
					AnimatorStateInfo nextAnimatorStateInfo = component.GetNextAnimatorStateInfo(i);
					if (component.IsInTransition(i) && nextAnimatorStateInfo.nameHash == actionHelperStorage.GetDetectStatePathHash())
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					actionHelperStorage.PlayAction();
				}
			}
			else
			{
				actionHelperStorage.PlayAction();
			}
		}

		public void EndAction(string _actionHelperName)
		{
			ActionHelperStorage actionHelperStorage = null;
			if (!this.actionHelperMap.TryGetValue(_actionHelperName, out actionHelperStorage))
			{
				return;
			}
			if (!actionHelperStorage.waitForEvents)
			{
				return;
			}
			Animator component = base.gameObject.GetComponent<Animator>();
			if (component == null)
			{
				return;
			}
			actionHelperStorage.StopLastAction();
		}

		public Action PlayAction(string _actionHelperName)
		{
			ActionHelperStorage actionHelperStorage = null;
			if (!this.actionHelperMap.TryGetValue(_actionHelperName, out actionHelperStorage))
			{
				return null;
			}
			actionHelperStorage.autoPlay = true;
			return actionHelperStorage.PlayAction();
		}

		public Action PlayAction(string _actionHelperName, DictionaryView<string, GameObject> dictionary)
		{
			ActionHelperStorage actionHelperStorage = null;
			if (!this.actionHelperMap.TryGetValue(_actionHelperName, out actionHelperStorage))
			{
				return null;
			}
			actionHelperStorage.autoPlay = true;
			return actionHelperStorage.PlayAction(dictionary);
		}

		public Action PlayAction(int index)
		{
			if (index < 0 || index > this.actionHelpers.Length)
			{
				return null;
			}
			ActionHelperStorage actionHelperStorage = this.actionHelpers[index];
			if (actionHelperStorage == null)
			{
				return null;
			}
			actionHelperStorage.autoPlay = true;
			return actionHelperStorage.PlayAction();
		}

		public ActionHelperStorage GetAction(string _actionHelperName)
		{
			ActionHelperStorage result = null;
			if (!this.actionHelperMap.TryGetValue(_actionHelperName, out result))
			{
				return null;
			}
			return result;
		}

		public ActionHelperStorage GetAction(int index)
		{
			if (index < 0 || index > this.actionHelpers.Length)
			{
				return null;
			}
			return this.actionHelpers[index];
		}

		public void Restart()
		{
			ActionHelperStorage[] array = this.actionHelpers;
			for (int i = 0; i < array.Length; i++)
			{
				ActionHelperStorage actionHelperStorage = array[i];
				if (actionHelperStorage.autoPlay)
				{
					ActionManager.Instance.PlayAction(actionHelperStorage.actionName, actionHelperStorage.autoPlay, actionHelperStorage.stopConflictActions, actionHelperStorage.targets);
				}
			}
		}
	}
}
