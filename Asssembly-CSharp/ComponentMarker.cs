using System;
using UnityEngine;

public class ComponentMarker : TemplateMarkerBase
{
	public enum eComponentMarkerType
	{
		And,
		Or
	}

	private ComponentMarker.eComponentMarkerType m_type;

	public Component[] m_components;

	public override bool Check(GameObject targetObject, out string errorInfo)
	{
		errorInfo = string.Empty;
		bool flag = false;
		if (this.m_type == ComponentMarker.eComponentMarkerType.And)
		{
			flag = true;
		}
		Component[] components = this.m_components;
		for (int i = 0; i < components.Length; i++)
		{
			Component component = components[i];
			if (!(component == null))
			{
				bool flag2 = targetObject.GetComponent(component.GetType());
				if (!flag2 && this.m_type == ComponentMarker.eComponentMarkerType.And)
				{
					flag = false;
					break;
				}
				if (flag2 && this.m_type == ComponentMarker.eComponentMarkerType.Or)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			errorInfo = "不符合组件要求规范";
		}
		return flag;
	}
}
