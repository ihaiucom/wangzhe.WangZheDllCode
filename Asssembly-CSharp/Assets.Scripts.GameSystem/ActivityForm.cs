using Assets.Scripts.UI;
using System;

namespace Assets.Scripts.GameSystem
{
	public abstract class ActivityForm
	{
		private ActivitySys _sys;

		public ActivitySys Sys
		{
			get
			{
				return this._sys;
			}
		}

		public abstract CUIFormScript formScript
		{
			get;
		}

		public ActivityForm(ActivitySys sys)
		{
			this._sys = sys;
		}

		public abstract void Open();

		public abstract void Close();

		public abstract void Update();
	}
}
