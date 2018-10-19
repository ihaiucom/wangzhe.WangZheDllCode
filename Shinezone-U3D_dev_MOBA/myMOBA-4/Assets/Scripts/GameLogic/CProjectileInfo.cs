using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class CProjectileInfo : ScriptableObject
	{
		public string ProjName;

		public string ProjDesc;

		public string AgeActionPath;

		public CSkillCombineInfo SelfSkillCombo;

		public CSkillCombineInfo TargetSkillCombo;
	}
}
