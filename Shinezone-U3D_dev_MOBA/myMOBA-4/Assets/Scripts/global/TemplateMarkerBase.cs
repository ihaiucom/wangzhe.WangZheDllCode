using System;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public abstract class TemplateMarkerBase : MonoBehaviour
{
	[Serializable]
	public class NamePattern
	{
		public string namePattern = string.Empty;

		public bool ignoreCase = true;

		public string IgnoreCaseStr
		{
			get
			{
				return (!this.ignoreCase) ? "区分大小写" : "不区分大小写";
			}
		}
	}

	public enum MarkerImportanceType
	{
		eEssential,
		eOptional
	}

	public enum MarkerType
	{
		eMainMarker,
		eUniqueMainMarker,
		eSubMarker
	}

	public TemplateMarkerBase.MarkerImportanceType m_markerImportanceType;

	public TemplateMarkerBase.MarkerType m_markerType = TemplateMarkerBase.MarkerType.eSubMarker;

	public abstract bool Check(GameObject targetObject, out string errorInfo);

	public static bool IsMainMarker(TemplateMarkerBase marker)
	{
		return marker != null && (marker.m_markerType == TemplateMarkerBase.MarkerType.eMainMarker || marker.m_markerType == TemplateMarkerBase.MarkerType.eUniqueMainMarker);
	}

	public static bool IsUniqueMarker(TemplateMarkerBase marker)
	{
		return marker != null && marker.m_markerType == TemplateMarkerBase.MarkerType.eUniqueMainMarker;
	}

	protected string wildcard2Regex(string wildCard)
	{
		return "^" + Regex.Escape(wildCard).Replace("\\*", ".*").Replace("\\?", ".") + "$";
	}

	protected bool isWildCardMatch(string targetString, string wildCard, bool ignoreCase)
	{
		string pattern = this.wildcard2Regex(wildCard);
		if (ignoreCase)
		{
			return Regex.IsMatch(targetString, pattern, RegexOptions.IgnoreCase);
		}
		return Regex.IsMatch(targetString, pattern);
	}
}
