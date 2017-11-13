using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	public class PlayerHead : MonoBehaviour
	{
		public enum HeadState
		{
			Normal,
			ReviveCDing,
			ReviveReady,
			ReviveForbid
		}

		public Text ReviveCdTxt;

		public Image HeroHeadImg;

		public Image HeroPickImg;

		public Text SoulLevelTxt;

		public GameObject SoulBgImgObj;

		public GameObject hpRadBar;

		public Image hpBarImgNormal;

		public Image hpBarImgPicked;

		private PlayerHead.HeadState _state;

		private PoolObjHandle<ActorRoot> _myHero;

		private HeroHeadHud _hudOwner;

		public HeroHeadHud HudOwner
		{
			get
			{
				return this._hudOwner;
			}
		}

		public PoolObjHandle<ActorRoot> MyHero
		{
			get
			{
				return this._myHero;
			}
		}

		public PlayerHead.HeadState state
		{
			get
			{
				return this._state;
			}
		}

		public void SetPrivates(PlayerHead.HeadState inHeadState, PoolObjHandle<ActorRoot> inHero, HeroHeadHud inHudOwner)
		{
			this._myHero = inHero;
			this._hudOwner = inHudOwner;
			this.SetState(inHeadState);
		}

		private void Awake()
		{
			if (this.ReviveCdTxt != null)
			{
				this.ReviveCdTxt.set_text(string.Empty);
			}
			this.HeroHeadImg = base.transform.Find("HeadMask/HeadImg").GetComponent<Image>();
			this.HeroPickImg = base.transform.Find("Select").GetComponent<Image>();
			this.SoulLevelTxt = base.transform.Find("soulLvlText").GetComponent<Text>();
			if (this.SoulLevelTxt != null)
			{
				this.SoulLevelTxt.set_text("1");
			}
			this.hpRadBar = base.transform.Find("progressBar").gameObject;
			this.hpBarImgNormal = this.hpRadBar.transform.Find("Normal").GetComponent<Image>();
			this.hpBarImgPicked = this.hpRadBar.transform.Find("Picked").GetComponent<Image>();
			this.SoulBgImgObj = base.transform.Find("soulBgImg").gameObject;
			bool bActive = Singleton<BattleLogic>.GetInstance().m_LevelContext.IsSoulGrow();
			this.SoulLevelTxt.gameObject.CustomSetActive(bActive);
			this.SoulBgImgObj.gameObject.CustomSetActive(bActive);
		}

		public void Init(HeroHeadHud hudOwner, PoolObjHandle<ActorRoot> myHero)
		{
			if (!myHero)
			{
				return;
			}
			this._hudOwner = hudOwner;
			this._myHero = myHero;
			this.SetState(PlayerHead.HeadState.Normal);
			uint configId = (uint)myHero.handle.TheActorMeta.ConfigId;
			this.HeroHeadImg.SetSprite(CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic(configId, 0u), Singleton<CBattleSystem>.GetInstance().FightFormScript, true, false, false, false);
			this.OnHeroHpChange(myHero.handle.ValueComponent.actorHp, myHero.handle.ValueComponent.actorHpTotal);
		}

		public void OnMyHeroDead()
		{
			if (!this._myHero)
			{
				return;
			}
			this.SetState(PlayerHead.HeadState.ReviveCDing);
			if (this.ReviveCdTxt != null)
			{
				this.ReviveCdTxt.set_text(string.Format("{0}", Mathf.RoundToInt((float)this.MyHero.handle.ActorControl.ReviveCooldown * 0.001f)));
				this.ReviveCdTxt.set_color(Color.white);
				this.ReviveCdTxt.set_fontSize(30);
			}
			if (this.HeroHeadImg != null)
			{
				this.HeroHeadImg.set_color(new Color(0.3f, 0.3f, 0.3f));
			}
			if (!Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() && !Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena())
			{
				base.InvokeRepeating("UpdateReviveCd", 1f, 1f);
			}
			else
			{
				this.ReviveCdTxt.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerHead_Dead"));
				this.ReviveCdTxt.set_color(Color.gray);
				this.ReviveCdTxt.set_fontSize(14);
				this.HeroHeadImg.set_color(new Color(0.3f, 0.3f, 0.3f));
			}
		}

		public void OnMyHeroRevive()
		{
			base.CancelInvoke("UpdateReviveCd");
			if (this.ReviveCdTxt != null)
			{
				this.ReviveCdTxt.set_text(string.Empty);
			}
			if (this.HeroHeadImg != null)
			{
				this.HeroHeadImg.set_color(new Color(1f, 1f, 1f));
			}
			this.SetState(PlayerHead.HeadState.Normal);
		}

		private void UpdateReviveCd()
		{
			if (!this.MyHero || this.MyHero.handle.ActorControl == null)
			{
				return;
			}
			int num = Mathf.RoundToInt((float)this.MyHero.handle.ActorControl.ReviveCooldown * 0.001f);
			if (num >= 0)
			{
				if (this.ReviveCdTxt != null)
				{
					this.ReviveCdTxt.set_text(string.Format("{0}", num));
				}
				this.SetState(PlayerHead.HeadState.ReviveCDing);
			}
			else if (!Singleton<BattleLogic>.instance.GetCurLvelContext().IsMobaMode())
			{
				Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this._myHero.handle.TheActorMeta.PlayerId);
				if (player == null)
				{
					return;
				}
				PlayerHead.HeadState headState = player.IsMyTeamOutOfBattle() ? PlayerHead.HeadState.ReviveReady : PlayerHead.HeadState.ReviveForbid;
				if (headState != this._state)
				{
					this.SetState(headState);
					if (this._state == PlayerHead.HeadState.ReviveReady)
					{
						this.ReviveCdTxt.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerHead_dianji"));
						this.ReviveCdTxt.set_color(Color.green);
						this.ReviveCdTxt.set_fontSize(14);
						this.HeroHeadImg.set_color(new Color(1f, 1f, 1f));
					}
					else
					{
						this.ReviveCdTxt.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerHead_tuozhan"));
						this.ReviveCdTxt.set_color(Color.gray);
						this.ReviveCdTxt.set_fontSize(14);
						this.HeroHeadImg.set_color(new Color(0.3f, 0.3f, 0.3f));
					}
				}
				Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
				if (hostPlayer != null && this._state == PlayerHead.HeadState.ReviveReady && hostPlayer.Captain.handle.ActorControl.m_isAutoAI)
				{
					this.MyHero.handle.ActorControl.Revive(false);
				}
			}
		}

		public void SetPicked(bool isPicked)
		{
			this.hpBarImgNormal.gameObject.CustomSetActive(!isPicked);
			this.hpBarImgPicked.gameObject.CustomSetActive(isPicked);
			if (this.HeroPickImg)
			{
				this.HeroPickImg.gameObject.CustomSetActive(isPicked);
			}
			base.GetComponent<RectTransform>().localScale = (isPicked ? this._hudOwner.pickedScale : Vector3.one);
		}

		public void OnHeroSoulLvlChange(int level)
		{
			if (this.SoulLevelTxt != null)
			{
				this.SoulLevelTxt.set_text(level.ToString());
			}
		}

		public void OnHeroHpChange(int curVal, int maxVal)
		{
			if (this.hpRadBar)
			{
				float value = (float)curVal / (float)maxVal;
				this.hpBarImgNormal.CustomFillAmount(value);
				this.hpBarImgPicked.CustomFillAmount(value);
			}
		}

		private void SetState(PlayerHead.HeadState hs)
		{
			if (hs != this._state)
			{
				this._state = hs;
				CUIEventScript component = base.GetComponent<CUIEventScript>();
				component.enabled = (this._state == PlayerHead.HeadState.Normal || this._state == PlayerHead.HeadState.ReviveReady);
			}
		}
	}
}
