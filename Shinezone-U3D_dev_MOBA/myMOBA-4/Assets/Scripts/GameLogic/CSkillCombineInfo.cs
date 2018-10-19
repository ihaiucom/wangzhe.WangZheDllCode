using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class CSkillCombineInfo : ScriptableObject
	{
		public int SkillComboId;

		public string ComboName;

		public string ComboDesc;

		public string AgeActionPath;
	}
}
