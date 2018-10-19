using System;
using System.Collections.Generic;

namespace AGE
{
	public class ActionSet
	{
		public Dictionary<Action, bool> actionSet = new Dictionary<Action, bool>();

		public ActionSet()
		{
			this.actionSet = new Dictionary<Action, bool>();
		}

		public ActionSet(Dictionary<Action, bool> _actionSet)
		{
			this.actionSet = new Dictionary<Action, bool>();
			foreach (KeyValuePair<Action, bool> current in _actionSet)
			{
				this.actionSet.Add(current.Key, current.Value);
			}
		}

		public static ActionSet InvertSet(ActionSet all, ActionSet exclusion)
		{
			ActionSet actionSet = new ActionSet();
			foreach (Action current in all.actionSet.Keys)
			{
				if (!exclusion.actionSet.ContainsKey(current))
				{
					actionSet.actionSet.Add(current, true);
				}
			}
			return actionSet;
		}

		public static ActionSet AndSet(ActionSet src1, ActionSet src2)
		{
			ActionSet actionSet = new ActionSet();
			foreach (Action current in src1.actionSet.Keys)
			{
				if (src2.actionSet.ContainsKey(current))
				{
					actionSet.actionSet.Add(current, true);
				}
			}
			return actionSet;
		}

		public static ActionSet OrSet(ActionSet src1, ActionSet src2)
		{
			ActionSet actionSet = new ActionSet();
			foreach (Action current in src1.actionSet.Keys)
			{
				actionSet.actionSet.Add(current, true);
			}
			foreach (Action current2 in src2.actionSet.Keys)
			{
				if (!actionSet.actionSet.ContainsKey(current2))
				{
					actionSet.actionSet.Add(current2, true);
				}
			}
			return actionSet;
		}
	}
}
