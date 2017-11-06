using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class HeroInfoSideItem
	{
		public COM_PLAYERCAMP listCamp;

		public int listIndex;

		private Image _headImg;

		private Text _reviveTxt;

		private Text _levelText;

		private Text _nameText;

		private HeroKDA _heroInfo;

		private GameObject _skill3Bg;

		private GameObject _skill3Canbe;

		public HeroInfoSideItem(COM_PLAYERCAMP _listCamp, HeroKDA _heroKDA, int _listIndex, GameObject goNode)
		{
			this.listCamp = _listCamp;
			this.listIndex = _listIndex;
			this._headImg = Utility.GetComponetInChild<Image>(goNode, "Head");
			this._reviveTxt = Utility.GetComponetInChild<Text>(goNode, "Revive");
			this._levelText = Utility.GetComponetInChild<Text>(goNode, "Level");
			this._skill3Bg = Utility.FindChild(goNode, "Skill-bg");
			this._skill3Canbe = Utility.FindChild(goNode, "Skill-canbe");
			this._nameText = Utility.GetComponetInChild<Text>(goNode, "Name");
			this._heroInfo = _heroKDA;
			this._skill3Bg.CustomSetActive(true);
			this._nameText.gameObject.CustomSetActive(false);
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			bool flag = false;
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				if (value.PlayerCamp == _listCamp)
				{
					ListView<HeroKDA>.Enumerator enumerator2 = value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.HeroId == _heroKDA.HeroId)
						{
							this._nameText.gameObject.CustomSetActive(true);
							this._nameText.set_text(value.PlayerName);
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			this._headImg.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustCircle_Dir, CSkinInfo.GetHeroSkinPic((uint)this._heroInfo.HeroId, 0u)), Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false, false);
			this.ValidateLevel();
			this.ValidateReviceCD();
			this.ValidateSkill3();
		}

		public void ValidateLevel()
		{
			this._levelText.set_text(this._heroInfo.actorHero.handle.ValueComponent.actorSoulLevel.ToString());
		}

		public void ValidateReviceCD()
		{
			if (this._heroInfo != null && this._heroInfo.actorHero && this._heroInfo.actorHero.handle.ActorControl.IsDeadState)
			{
				this._reviveTxt.set_text(string.Format("{0}", Mathf.FloorToInt((float)this._heroInfo.actorHero.handle.ActorControl.ReviveCooldown * 0.001f)));
				this._headImg.set_color(CUIUtility.s_Color_Grey);
			}
			else
			{
				this._reviveTxt.set_text(string.Empty);
				this._headImg.set_color(CUIUtility.s_Color_Full);
			}
		}

		public void ValidateSkill3()
		{
			if (this._heroInfo != null && this._heroInfo.actorHero)
			{
				this._skill3Bg.CustomSetActive(true);
				SkillSlot[] skillSlotArray = this._heroInfo.actorHero.handle.SkillControl.SkillSlotArray;
				SkillSlot skillSlot = skillSlotArray[3];
				if (skillSlot != null && skillSlot.IsUnLock() && skillSlot.IsCDReady)
				{
					this._skill3Canbe.CustomSetActive(true);
				}
				else
				{
					this._skill3Canbe.CustomSetActive(false);
				}
			}
			else
			{
				this._skill3Bg.CustomSetActive(false);
				this._skill3Canbe.CustomSetActive(false);
			}
		}

		public void LateUpdate()
		{
			this.ValidateReviceCD();
		}

		public static void MakeEmpty(GameObject infoItem)
		{
			infoItem.GetComponent<CUIEventScript>().enabled = false;
			Utility.FindChild(infoItem, "Empty").CustomSetActive(true);
			Text componetInChild = Utility.GetComponetInChild<Text>(infoItem, "Name");
			if (componetInChild)
			{
				componetInChild.gameObject.CustomSetActive(false);
			}
		}
	}
}
