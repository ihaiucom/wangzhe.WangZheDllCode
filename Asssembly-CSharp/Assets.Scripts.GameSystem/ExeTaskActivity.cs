using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class ExeTaskActivity : Activity
	{
		private ResWealCondition _config;

		public override uint ID
		{
			get
			{
				return this._config.dwID;
			}
		}

		public override COM_WEAL_TYPE Type
		{
			get
			{
				return COM_WEAL_TYPE.COM_WEAL_CONDITION;
			}
		}

		public override int Target
		{
			get
			{
				return base.PhaseList[base.PhaseList.Count - 1].Target;
			}
		}

		public override int Current
		{
			get
			{
				ExeTaskPhase exeTaskPhase = null;
				for (int i = 0; i < base.PhaseList.Count; i++)
				{
					ExeTaskPhase exeTaskPhase2 = base.PhaseList[i] as ExeTaskPhase;
					if (!exeTaskPhase2.Achieved)
					{
						exeTaskPhase = exeTaskPhase2;
						break;
					}
				}
				if (exeTaskPhase == null && base.PhaseList.Count > 0)
				{
					exeTaskPhase = (base.PhaseList[base.PhaseList.Count - 1] as ExeTaskPhase);
				}
				return (exeTaskPhase != null) ? exeTaskPhase.Current : 0;
			}
		}

		public ExeTaskActivity(ActivitySys mgr, ResWealCondition config) : base(mgr, config.stCommon)
		{
			this._config = config;
			ushort num = 0;
			while (num < this._config.wConNum && (int)num < this._config.astConInfo.Length)
			{
				ExeTaskPhase ap = new ExeTaskPhase(this, (uint)num, this._config.astConInfo[(int)num]);
				base.AddPhase(ap);
				if (this._config.astConInfo[(int)num].dwConType == 14u)
				{
					mgr.IsShareTask = true;
				}
				num += 1;
			}
		}

		public void LoadInfo(COMDT_WEAL_CON_DATA_DETAIL conData)
		{
			for (int i = 0; i < base.PhaseList.Count; i++)
			{
				ExeTaskPhase exeTaskPhase = (ExeTaskPhase)base.PhaseList[i];
				exeTaskPhase.SetAchiveve((conData.dwReachMask & 1u << i) > 0u, (conData.dwLimitReachMask & 1u << i) > 0u);
				if (i < (int)conData.wConNum)
				{
					exeTaskPhase.SetCurrent((int)conData.astConData[i].dwValue);
				}
			}
			base.SetPhaseMarks((ulong)conData.dwRewardMask);
		}
	}
}
