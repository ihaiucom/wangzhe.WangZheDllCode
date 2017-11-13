using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class HeroInfoItem
	{
		public COM_PLAYERCAMP listCamp;

		public int listIndex;

		private HeroKDA _heroInfo;

		private GameObject _baseInfoItem;

		private GameObject _equipInfoItem;

		private Image _headImg;

		private Text _reviveTxt;

		private Text _levelText;

		private Text _kdaText;

		private Text _killText;

		private Text _deadText;

		private Text _assiText;

		private Text _moneyText;

		private GameObject _emptyGo;

		private Image[] _equipIcons;

		private GameObject _hostGo;

		private GameObject _SkillGroupGo;

		private Image[] _skillIcons;

		private Image[] _skillCDs;

		private GameObject[] _skillGos;

		public HeroKDA HeroInfo
		{
			get
			{
				return this._heroInfo;
			}
		}

		public HeroInfoItem(COM_PLAYERCAMP _listCamp, int _listIndex, HeroKDA _heroKDA, GameObject baseInfoItem, GameObject equipInfoItem)
		{
			this.listCamp = _listCamp;
			this.listIndex = _listIndex;
			this._heroInfo = _heroKDA;
			this._baseInfoItem = baseInfoItem;
			this._equipInfoItem = equipInfoItem;
			this._baseInfoItem.GetComponent<CUIEventScript>().enabled = true;
			this._headImg = Utility.GetComponetInChild<Image>(baseInfoItem, "Head");
			this._reviveTxt = Utility.GetComponetInChild<Text>(baseInfoItem, "Revive");
			this._levelText = Utility.GetComponetInChild<Text>(baseInfoItem, "Level");
			this._killText = Utility.GetComponetInChild<Text>(baseInfoItem, "Kill");
			this._deadText = Utility.GetComponetInChild<Text>(baseInfoItem, "Dead");
			this._assiText = Utility.GetComponetInChild<Text>(baseInfoItem, "Help");
			this._kdaText = Utility.GetComponetInChild<Text>(baseInfoItem, "KDA");
			this._moneyText = Utility.GetComponetInChild<Text>(baseInfoItem, "Money");
			this._hostGo = Utility.FindChild(baseInfoItem, "Host");
			this._SkillGroupGo = Utility.FindChild(this._baseInfoItem, "SkillGroup");
			this._hostGo.CustomSetActive(this._heroInfo.actorHero.handle.ObjID == Singleton<CBattleSystem>.GetInstance().WatchForm.TargetHeroId);
			this._emptyGo = Utility.FindChild(baseInfoItem, "Empty");
			this._emptyGo.CustomSetActive(false);
			this._equipInfoItem.GetComponent<CUIEventScript>().enabled = true;
			this._equipIcons = new Image[6];
			for (int i = 0; i < 6; i++)
			{
				this._equipIcons[i] = Utility.GetComponetInChild<Image>(equipInfoItem, "Icon_" + i);
				this._equipIcons[i].gameObject.CustomSetActive(true);
			}
			this._skillIcons = new Image[7];
			this._skillCDs = new Image[7];
			this._skillGos = new GameObject[7];
			for (int j = 0; j < 7; j++)
			{
				this._skillGos[j] = Utility.FindChild(this._SkillGroupGo, "Skill_" + (j + 1));
				if (this._skillGos[j] != null)
				{
					this._skillIcons[j] = Utility.GetComponetInChild<Image>(this._skillGos[j], "Icon");
					this._skillCDs[j] = Utility.GetComponetInChild<Image>(this._skillGos[j], "CdBg");
				}
			}
			this._headImg.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustCircle_Dir, CSkinInfo.GetHeroSkinPic((uint)this._heroInfo.HeroId, 0u)), Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false, false);
			this.ValidateLevel();
			this.ValidateKDA();
			this.ValidateMoney();
			this.ValidateEquip();
			this.ValidateReviceCD();
			this.ValidateSkillGroup();
		}

		public void ValidateLevel()
		{
			if (this._levelText != null)
			{
				this._levelText.set_text(this._heroInfo.actorHero.handle.ValueComponent.actorSoulLevel.ToString());
			}
		}

		public void ValidateKDA()
		{
			if (this._kdaText != null)
			{
				this._kdaText.set_text(string.Concat(new object[]
				{
					this._heroInfo.numKill,
					" / ",
					this._heroInfo.numDead,
					" / ",
					this._heroInfo.numAssist
				}));
			}
			if (this._killText != null)
			{
				this._killText.set_text(this._heroInfo.numKill.ToString());
			}
			if (this._deadText != null)
			{
				this._deadText.set_text(this._heroInfo.numDead.ToString());
			}
			if (this._assiText != null)
			{
				this._assiText.set_text(this._heroInfo.numAssist.ToString());
			}
		}

		public void ValidateMoney()
		{
			if (this._moneyText == null)
			{
				return;
			}
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				this._moneyText.set_text(string.Format("{0:N1}k", (float)this._heroInfo.TotalCoin * 0.001f));
			}
			else
			{
				this._moneyText.set_text(this._heroInfo.TotalCoin.ToString());
			}
		}

		public void ValidateEquip()
		{
			stEquipInfo[] equips = this._heroInfo.actorHero.handle.EquipComponent.GetEquips();
			for (int i = 0; i < 6; i++)
			{
				Image image = this._equipIcons[i];
				ushort equipID = equips[i].m_equipID;
				bool flag = false;
				if (equipID > 0)
				{
					ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipID);
					if (dataByKey != null)
					{
						image.gameObject.CustomSetActive(true);
						string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_System_BattleEquip_Dir, dataByKey.szIcon);
						CUIUtility.SetImageSprite(image, prefabPath, Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false, false);
						flag = true;
					}
				}
				if (!flag)
				{
					image.SetSprite(string.Format((this.listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "{0}EquipmentSpace" : "{0}EquipmentSpaceRed", CUIUtility.s_Sprite_Dynamic_Talent_Dir), Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false, false);
				}
			}
		}

		public void ValidateReviceCD()
		{
			if (this._heroInfo != null && this._heroInfo.actorHero && this._heroInfo.actorHero.handle.ActorControl.IsDeadState)
			{
				if (this._reviveTxt != null)
				{
					this._reviveTxt.set_text(string.Format("{0}", Mathf.FloorToInt((float)this._heroInfo.actorHero.handle.ActorControl.ReviveCooldown * 0.001f)));
				}
				this._headImg.set_color(CUIUtility.s_Color_Grey);
			}
			else
			{
				if (this._reviveTxt != null)
				{
					this._reviveTxt.set_text(string.Empty);
				}
				this._headImg.set_color(CUIUtility.s_Color_Full);
			}
		}

		public void ValidateSkillGroup()
		{
			if (this._SkillGroupGo == null)
			{
				return;
			}
			if (this._heroInfo != null && this._heroInfo.actorHero)
			{
				this._SkillGroupGo.CustomSetActive(true);
				SkillSlot[] skillSlotArray = this._heroInfo.actorHero.handle.SkillControl.SkillSlotArray;
				int num = 0;
				while (num < 7 && num + 1 < skillSlotArray.Length)
				{
					SkillSlotType slot = num + SkillSlotType.SLOT_SKILL_1;
					SkillSlot skillSlot = skillSlotArray[num + 1];
					if (this._skillGos[num])
					{
						if (skillSlot != null)
						{
							this._skillGos[num].CustomSetActive(true);
							this._skillIcons[num].SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + skillSlot.SkillObj.IconName, Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false, false);
							this.ValidateCD(slot, this._heroInfo.actorHero);
						}
						else
						{
							this._skillGos[num].CustomSetActive(false);
						}
					}
					num++;
				}
			}
			else
			{
				this._SkillGroupGo.CustomSetActive(false);
			}
		}

		public void ValidateSkill(SkillSlotType slot, PoolObjHandle<ActorRoot> _curActor)
		{
			if (!_curActor)
			{
				return;
			}
			int num = slot - SkillSlotType.SLOT_SKILL_1;
			if (this._skillGos == null || this._skillIcons == null)
			{
				return;
			}
			if (num < 0 || num > _curActor.handle.SkillControl.SkillSlotArray.Length - 1 || num > this._skillGos.Length - 1 || num > this._skillIcons.Length - 1)
			{
				return;
			}
			SkillSlot skillSlot = _curActor.handle.SkillControl.SkillSlotArray[(int)slot];
			GameObject gameObject = this._skillGos[num];
			if (gameObject != null)
			{
				gameObject.CustomSetActive(true);
			}
			this._skillIcons[num].SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + skillSlot.SkillObj.IconName, Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false, false);
		}

		public void ValidateCD(SkillSlotType slot, PoolObjHandle<ActorRoot> actor)
		{
			if (this._SkillGroupGo != null && this._heroInfo != null && this._heroInfo.actorHero)
			{
				int num = slot - SkillSlotType.SLOT_SKILL_1;
				if (num >= 0 && num < this._skillCDs.Length)
				{
					SkillSlot skillSlot = actor.handle.SkillControl.SkillSlotArray[(int)slot];
					if (this._skillCDs[num] != null)
					{
						this._skillCDs[num].set_fillAmount((float)skillSlot.CurSkillCD / (float)skillSlot.GetSkillCDMax());
					}
				}
			}
		}

		public void LateUpdate()
		{
			this.ValidateReviceCD();
		}

		public static void MakeEmpty(GameObject baseInfoItem, GameObject equipInfoItem)
		{
			baseInfoItem.GetComponent<CUIEventScript>().enabled = false;
			Utility.FindChild(baseInfoItem, "Empty").CustomSetActive(true);
			equipInfoItem.GetComponent<CUIEventScript>().enabled = false;
			for (int i = 0; i < 6; i++)
			{
				Utility.FindChild(equipInfoItem, "Icon_" + i).CustomSetActive(false);
			}
		}
	}
}
