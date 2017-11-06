using Assets.Scripts.UI;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FPSForm : CUIFormScript
{
	public Text m_fpsText;

	private Color color = Color.white;

	public static string sFPS = string.Empty;

	private string revision = string.Empty;

	public static string extMsg = string.Empty;

	private void Start()
	{
		CBinaryObject cBinaryObject = Singleton<CResourceManager>.GetInstance().GetResource("Revision.txt", typeof(TextAsset), enResourceType.Numeric, false, false).m_content as CBinaryObject;
		if (null != cBinaryObject)
		{
			this.revision = Encoding.get_UTF8().GetString(cBinaryObject.m_data);
		}
		Singleton<CResourceManager>.GetInstance().RemoveCachedResource("Revision.txt");
	}
}
