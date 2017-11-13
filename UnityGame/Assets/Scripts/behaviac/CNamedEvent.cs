using System;
using System.Reflection;

namespace behaviac
{
	public class CNamedEvent : CMethodBase
	{
		public CNamedEvent(MethodBase m, EventMetaInfoAttribute a, string eventName) : base(m, a, eventName)
		{
		}

		private CNamedEvent(CNamedEvent copy) : base(copy)
		{
		}

		public override CMethodBase clone()
		{
			return new CNamedEvent(this);
		}

		public override bool IsNamedEvent()
		{
			return true;
		}

		public void SetFired(Agent pAgent, bool bFired)
		{
			if (bFired)
			{
				pAgent.btonevent(base.Name);
			}
		}

		public void SetParam<ParamType>(Agent pAgent, ParamType param)
		{
			string variableName = string.Format("{0}::{1}::param0", base.GetClassNameString().Replace(".", "::"), base.Name);
			pAgent.SetVariable<ParamType>(variableName, param);
		}

		public void SetParam<ParamType1, ParamType2>(Agent pAgent, ParamType1 param1, ParamType2 param2)
		{
			string variableName = string.Format("{0}::{1}::param0", base.GetClassNameString().Replace(".", "::"), base.Name);
			pAgent.SetVariable<ParamType1>(variableName, param1);
			string variableName2 = string.Format("{0}::{1}::param1", base.GetClassNameString().Replace(".", "::"), base.Name);
			pAgent.SetVariable<ParamType2>(variableName2, param2);
		}

		public void SetParam<ParamType1, ParamType2, ParamType3>(Agent pAgent, ParamType1 param1, ParamType2 param2, ParamType3 param3)
		{
			string variableName = string.Format("{0}::{1}::param0", base.GetClassNameString().Replace(".", "::"), base.Name);
			pAgent.SetVariable<ParamType1>(variableName, param1);
			string variableName2 = string.Format("{0}::{1}::param1", base.GetClassNameString().Replace(".", "::"), base.Name);
			pAgent.SetVariable<ParamType2>(variableName2, param2);
			string variableName3 = string.Format("{0}::{1}::param2", base.GetClassNameString().Replace(".", "::"), base.Name);
			pAgent.SetVariable<ParamType3>(variableName3, param3);
		}
	}
}
