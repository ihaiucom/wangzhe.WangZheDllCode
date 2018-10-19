using System;
using UnityEngine;

public class AnimMarker : TemplateMarkerBase
{
	[Tooltip("贴图参数设置,填入该材质需要检测的贴图的张数")]
	public string[] m_animNameList;

	public override bool Check(GameObject targetObject, out string errorInfo)
	{
		errorInfo = string.Empty;
		Animation component = targetObject.GetComponent<Animation>();
		if (null == component)
		{
			errorInfo = "没有Animation组件";
			return false;
		}
		if (this.m_animNameList != null)
		{
			bool flag = false;
			string text = string.Empty;
			string[] animNameList = this.m_animNameList;
			for (int i = 0; i < animNameList.Length; i++)
			{
				string text2 = animNameList[i];
				if (!component.GetClip(text2))
				{
					flag = true;
					text = text + text2 + ",";
				}
			}
			if (flag)
			{
				errorInfo = "缺少必要的动画:" + text.TrimEnd(new char[]
				{
					','
				});
				return false;
			}
		}
		return true;
	}
}
