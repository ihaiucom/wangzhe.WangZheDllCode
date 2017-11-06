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
			using (Dictionary<Action, bool>.Enumerator enumerator = _actionSet.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<Action, bool> current = enumerator.get_Current();
					this.actionSet.Add(current.get_Key(), current.get_Value());
				}
			}
		}

		public static ActionSet InvertSet(ActionSet all, ActionSet exclusion)
		{
			ActionSet actionSet = new ActionSet();
			using (Dictionary<Action, bool>.KeyCollection.Enumerator enumerator = all.actionSet.get_Keys().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Action current = enumerator.get_Current();
					if (!exclusion.actionSet.ContainsKey(current))
					{
						actionSet.actionSet.Add(current, true);
					}
				}
			}
			return actionSet;
		}

		public static ActionSet AndSet(ActionSet src1, ActionSet src2)
		{
			ActionSet actionSet = new ActionSet();
			using (Dictionary<Action, bool>.KeyCollection.Enumerator enumerator = src1.actionSet.get_Keys().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Action current = enumerator.get_Current();
					if (src2.actionSet.ContainsKey(current))
					{
						actionSet.actionSet.Add(current, true);
					}
				}
			}
			return actionSet;
		}

		public static ActionSet OrSet(ActionSet src1, ActionSet src2)
		{
			ActionSet actionSet = new ActionSet();
			using (Dictionary<Action, bool>.KeyCollection.Enumerator enumerator = src1.actionSet.get_Keys().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Action current = enumerator.get_Current();
					actionSet.actionSet.Add(current, true);
				}
			}
			using (Dictionary<Action, bool>.KeyCollection.Enumerator enumerator2 = src2.actionSet.get_Keys().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Action current2 = enumerator2.get_Current();
					if (!actionSet.actionSet.ContainsKey(current2))
					{
						actionSet.actionSet.Add(current2, true);
					}
				}
			}
			return actionSet;
		}
	}
}
