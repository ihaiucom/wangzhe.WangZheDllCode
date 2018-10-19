using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class SkillInfoHud
	{
		public const int MAX_SKILL_HUD_COUNT = 7;

		private GameObject[] _skillItems;

		private Image[] _skillIcons;

		private GameObject[] _skillLevelRoots;

		private Text[] _skillLevels;

		private Image[] _skillCdBgs;

		private PoolObjHandle<ActorRoot> _curActor;

		public SkillInfoHud(GameObject root)
		{
			this._skillItems = new GameObject[7];
			this._skillIcons = new Image[7];
			this._skillLevelRoots = new GameObject[7];
			this._skillLevels = new Text[7];
			this._skillCdBgs = new Image[7];
			for (int i = 0; i < 7; i++)
			{
				GameObject gameObject = Utility.FindChild(root, "Skill_" + (i + 1));
				this._skillItems[i] = gameObject;
				this._skillIcons[i] = Utility.GetComponetInChild<Image>(gameObject, "Icon");
				this._skillLevelRoots[i] = Utility.FindChild(gameObject, "Level");
				this._skillLevels[i] = Utility.GetComponetInChild<Text>(gameObject, "Level/Text");
				this._skillCdBgs[i] = Utility.GetComponetInChild<Image>(gameObject, "CdBg");
			}
			this._curActor = new PoolObjHandle<ActorRoot>(null);
		}

		public void SwitchActor(ref PoolObjHandle<ActorRoot> actor)
		{
			if (actor == this._curActor)
			{
				return;
			}
			this._curActor = actor;
			SkillSlot[] skillSlotArray = actor.handle.SkillControl.SkillSlotArray;
			int num = 0;
			while (num < this._skillItems.Length && num + 1 < skillSlotArray.Length)
			{
				SkillSlotType slot = num + SkillSlotType.SLOT_SKILL_1;
				SkillSlot skillSlot = skillSlotArray[num + 1];
				GameObject gameObject = this._skillItems[num];
				if (gameObject)
				{
					if (skillSlot != null)
					{
						this.ValidateSkill(slot);
						this.ValidateLevel(slot);
						this.ValidateCD(slot);
					}
					else
					{
						gameObject.CustomSetActive(false);
					}
				}
				num++;
			}
		}

		public void ValidateLevel(SkillSlotType slot)
		{
			if (!this._curActor)
			{
				return;
			}
			int num = slot - SkillSlotType.SLOT_SKILL_1;
			if (num >= 0 && num < this._skillLevels.Length)
			{
				SkillSlot skillSlot = this._curActor.handle.SkillControl.SkillSlotArray[(int)slot];
				this._skillLevels[num].text = skillSlot.GetSkillLevel().ToString();
			}
		}

		public void ValidateSkill(SkillSlotType slot)
		{
			if (!this._curActor)
			{
				return;
			}
			int num = slot - SkillSlotType.SLOT_SKILL_1;
			if (this._skillItems == null || this._skillIcons == null || this._skillLevelRoots == null)
			{
				return;
			}
			if (num < 0 || num > this._curActor.handle.SkillControl.SkillSlotArray.Length - 1 || num > this._skillItems.Length - 1 || num > this._skillIcons.Length - 1 || num > this._skillLevelRoots.Length - 1)
			{
				return;
			}
			SkillSlot skillSlot = this._curActor.handle.SkillControl.SkillSlotArray[(int)slot];
			GameObject obj = this._skillItems[num];
			obj.CustomSetActive(true);
			this._skillIcons[num].SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + skillSlot.SkillObj.IconName, Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false, false);
			this._skillLevelRoots[num].CustomSetActive(slot >= SkillSlotType.SLOT_SKILL_1 && slot <= SkillSlotType.SLOT_SKILL_3);
		}

		public void ValidateCD(SkillSlotType slot)
		{
			if (!this._curActor)
			{
				return;
			}
			int num = slot - SkillSlotType.SLOT_SKILL_1;
			if (num >= 0 && num < this._skillCdBgs.Length)
			{
				SkillSlot skillSlot = this._curActor.handle.SkillControl.SkillSlotArray[(int)slot];
				if (this._skillCdBgs[num] != null)
				{
					this._skillCdBgs[num].fillAmount = (int)skillSlot.CurSkillCD / (float)skillSlot.GetSkillCDMax();
				}
			}
		}
	}
}
