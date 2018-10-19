using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class CSkillInfo : ScriptableObject
	{
		public Texture2D SkillIcon;

		public string SkillName;

		public string SkillDesc;

		public string AgeActionPath;

		public int CooldownMs;

		public SkillUseRule UseRule;

		public SkillTargetRule DetectRule;

		public SkillRangeAppointType RangeAptType;

		public bool bIncludeSelf;

		public bool bIncludeEnemy;

		public int MaxAttackDistance;

		public int MaxSearchDistance;

		public CSkillCombineInfo SelfSkillCombo;

		public CSkillCombineInfo TargetSkillCombo;

		public CProjectileInfo ProjRef;
	}
}
