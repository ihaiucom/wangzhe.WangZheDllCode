using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class BattleMisc
	{
		private const int EliteBarDurationLost = 10000;

		private const int EliteBarDurationDead = 2500;

		private CUIFormScript map_fromScript;

		private GameObject buff_node;

		private Text buff_cd_text;

		private int total_second;

		private int second_timer = -1;

		private CUIAnimatorScript animationScript;

		private PoolObjHandle<ActorRoot> bossActorRoot = default(PoolObjHandle<ActorRoot>);

		private GameObject boss_hp_Node;

		private int boss_hp_timer = -1;

		private Text hpText;

		private Text nameText;

		private Image bloodImage;

		private Image blood_eft;

		private Image boss_icon;

		private PoolObjHandle<ActorRoot> eliteActor = default(PoolObjHandle<ActorRoot>);

		private string eliteActorName;

		private int eliteBarTimer = -1;

		public static PoolObjHandle<ActorRoot> BossRoot = default(PoolObjHandle<ActorRoot>);

		private bool bIn10;

		private bool bIn30;

		private bool bNormal = true;

		public void Init(GameObject node, CUIFormScript formScript)
		{
			this.map_fromScript = formScript;
			node.CustomSetActive(true);
			this.buff_node = Utility.FindChild(node, "buff");
			this.buff_cd_text = Utility.GetComponetInChild<Text>(this.buff_node, "Present/Text_Skill_1_CD");
			this.buff_node.CustomSetActive(false);
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitch));
			Transform transform = formScript.transform.Find("Flashing_RedHud");
			if (transform)
			{
				this.animationScript = transform.GetComponent<CUIAnimatorScript>();
			}
			this.bIn10 = false;
			this.bIn30 = false;
			this.bNormal = true;
			this.bossActorRoot.Release();
			this.boss_hp_Node = Utility.FindChild(node, "boss_blood");
			this.boss_hp_Node.CustomSetActive(false);
			if (this.boss_hp_timer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.boss_hp_timer);
			}
			this.boss_hp_timer = -1;
			this.hpText = Utility.GetComponetInChild<Text>(this.boss_hp_Node, "Bar/txt");
			this.nameText = Utility.GetComponetInChild<Text>(this.boss_hp_Node, "Bar/name");
			this.bloodImage = Utility.GetComponetInChild<Image>(this.boss_hp_Node, "Bar/Fore");
			this.blood_eft = Utility.GetComponetInChild<Image>(this.boss_hp_Node, "Bar/Fore/Effect_BloodBar_Front");
			this.boss_icon = Utility.GetComponetInChild<Image>(this.boss_hp_Node, "Bar/Icon");
			this.eliteActor.Release();
			this.eliteActorName = null;
			this.eliteBarTimer = -1;
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
		}

		public void Clear()
		{
			if (this.second_timer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.second_timer);
			}
			this.second_timer = -1;
			this.animationScript = null;
			this.buff_node = null;
			this.buff_cd_text = null;
			this.second_timer = -1;
			this.bIn10 = false;
			this.bIn30 = false;
			this.bNormal = true;
			this.bossActorRoot.Release();
			this.boss_hp_Node = null;
			if (this.boss_hp_timer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.boss_hp_timer);
			}
			this.boss_hp_timer = -1;
			this.hpText = null;
			this.bloodImage = null;
			this.blood_eft = null;
			this.boss_icon = null;
			BattleMisc.BossRoot.Release();
			this.map_fromScript = null;
		}

		public void Uninit()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
			this.OnCloseEliteBar(-1);
			this.eliteActor.Release();
			this.eliteActorName = null;
			this.eliteBarTimer = -1;
		}

		public void Show_BuffCD(int icon, int tsecond)
		{
			if (this.buff_cd_text == null)
			{
				this.Clear();
			}
			if (tsecond < 0)
			{
				return;
			}
			if (tsecond == 0)
			{
				this.buff_node.CustomSetActive(true);
				if (this.buff_cd_text != null)
				{
					this.buff_cd_text.gameObject.CustomSetActive(false);
				}
				return;
			}
			this.buff_node.CustomSetActive(true);
			this.total_second = tsecond / 1000;
			if (this.buff_cd_text != null)
			{
				this.buff_cd_text.set_text(this.total_second.ToString());
				this.buff_cd_text.gameObject.CustomSetActive(true);
			}
			if (this.total_second > 0)
			{
				this.second_timer = Singleton<CTimerManager>.instance.AddTimer(1000, -1, new CTimer.OnTimeUpHandler(this.On_Second_Time));
			}
		}

		private void On_Second_Time(int timer)
		{
			if (this.buff_cd_text == null)
			{
				this.Clear();
			}
			if (Singleton<CBattleSystem>.instance.FightForm == null)
			{
				return;
			}
			this.total_second--;
			if (this.buff_cd_text != null)
			{
				this.buff_cd_text.set_text(this.total_second.ToString());
			}
			if (this.total_second == 0)
			{
				this.buff_node.CustomSetActive(false);
				if (this.second_timer != -1)
				{
					Singleton<CTimerManager>.instance.RemoveTimer(this.second_timer);
				}
				this.second_timer = -1;
			}
		}

		public void BindHP()
		{
			ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = Singleton<GamePlayerCenter>.instance.GetHostPlayer().GetAllHeroes();
			DebugHelper.Assert(allHeroes.isValidReference, "invalid all heros list.");
			if (allHeroes.isValidReference)
			{
				for (int i = 0; i < allHeroes.Count; i++)
				{
					if (allHeroes[i])
					{
						if (allHeroes[i].handle.ValueComponent != null)
						{
							allHeroes[i].handle.ValueComponent.HpChgEvent -= new ValueChangeDelegate(this.OnHPChange);
						}
						else
						{
							DebugHelper.Assert(false, "invalid player.valuecomponent in player list at index {0}", new object[]
							{
								i
							});
						}
					}
					else
					{
						DebugHelper.Assert(false, "invalid player in player list at index {0}", new object[]
						{
							i
						});
					}
				}
			}
			this.Check_hp();
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.ValueComponent != null)
			{
				hostPlayer.Captain.handle.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnHPChange);
			}
			else if (hostPlayer == null)
			{
				DebugHelper.Assert(false, "invalid host player");
			}
			else if (!hostPlayer.Captain)
			{
				DebugHelper.Assert(false, "invalid host player captain");
			}
			else if (hostPlayer.Captain.handle.ValueComponent == null)
			{
				DebugHelper.Assert(false, "invalid host player captain->valuecomponent");
			}
		}

		private void OnHPChange()
		{
			this.Check_hp();
		}

		private void OnCaptainSwitch(ref DefaultGameEventParam prm)
		{
			this.BindHP();
		}

		private void Check_hp()
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle.ActorControl == null || hostPlayer.Captain.handle.ValueComponent == null)
			{
				this.Clear();
				return;
			}
			int roundInt = (hostPlayer.Captain.handle.ValueComponent.GetHpRate() * 100L).roundInt;
			if (hostPlayer.Captain.handle.ActorControl.IsDeadState || hostPlayer.Captain.handle.ValueComponent.actorHp <= 0)
			{
				this.bIn10 = (this.bIn30 = false);
				this.bNormal = true;
				if (this.animationScript != null)
				{
					this.animationScript.PlayAnimator("Rid_Close");
					this.animationScript.gameObject.CustomSetActive(false);
				}
				return;
			}
			if (roundInt > 30)
			{
				this.bIn10 = (this.bIn30 = false);
				if (!this.bNormal)
				{
					if (this.animationScript != null)
					{
						this.animationScript.PlayAnimator("Rid_Close");
						this.animationScript.gameObject.CustomSetActive(false);
					}
					this.bNormal = true;
				}
				return;
			}
			if (roundInt <= 10)
			{
				if (!this.bIn10)
				{
					this.bIn10 = true;
					this.bNormal = false;
					this.bIn30 = false;
					if (this.animationScript != null)
					{
						this.animationScript.gameObject.CustomSetActive(true);
						this.animationScript.PlayAnimator("Rid_02");
					}
				}
				return;
			}
			if (roundInt <= 30)
			{
				if (!this.bIn30)
				{
					this.bIn30 = true;
					this.bIn10 = false;
					this.bNormal = false;
					if (this.animationScript != null)
					{
						this.animationScript.gameObject.CustomSetActive(true);
						this.animationScript.PlayAnimator("Rid_Stat");
					}
				}
				return;
			}
		}

		public void BindBossMonster(PoolObjHandle<ActorRoot> owner)
		{
			if (!owner)
			{
				return;
			}
			this.bossActorRoot = owner;
			if (this.bossActorRoot.handle.ValueComponent != null)
			{
				this.bossActorRoot.handle.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnBossHpChange);
			}
			if (this.boss_hp_timer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.boss_hp_timer);
			}
			this.boss_hp_timer = Singleton<CTimerManager>.instance.AddTimer(1000, -1, new CTimer.OnTimeUpHandler(this.Check_Boss_InBattle));
		}

		private string QueryBossName()
		{
			DebugHelper.Assert(this.bossActorRoot);
			return UT.Bytes2String(MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(this.bossActorRoot.handle.TheActorMeta.ConfigId).szName);
		}

		private void ChangeToBoss()
		{
			if (this.nameText != null)
			{
				this.nameText.set_text(this.QueryBossName());
			}
			this.SetBossIcon(this.bossActorRoot);
		}

		private void ChangeToElite()
		{
			if (this.nameText != null && !string.IsNullOrEmpty(this.eliteActorName))
			{
				this.nameText.set_text(this.eliteActorName);
			}
			this.SetBossIcon(this.eliteActor);
		}

		private void SetBossIcon(ActorRoot inActor)
		{
			if (inActor == null || this.map_fromScript == null)
			{
				return;
			}
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(inActor.TheActorMeta.ConfigId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(dataCfgInfoByCurLevelDiff.szBossIcon))
			{
				this.boss_icon.SetSprite(dataCfgInfoByCurLevelDiff.szBossIcon, this.map_fromScript, true, false, false, false);
			}
			else
			{
				this.boss_icon.SetSprite("UGUI/Sprite/Dynamic/BustCircle/50001", this.map_fromScript, true, false, false, false);
			}
		}

		public void RebindBoss()
		{
			if (BattleMisc.BossRoot)
			{
				this.BindBossMonster(BattleMisc.BossRoot);
			}
		}

		private bool BindNewElite(PoolObjHandle<ActorRoot> inMonster)
		{
			if (!inMonster)
			{
				return false;
			}
			DebugHelper.Assert(!this.eliteActor);
			this.eliteActor = inMonster;
			this.eliteActorName = this.QueryEliteName(this.eliteActor);
			if (this.eliteActor.handle.ValueComponent != null)
			{
				this.eliteActor.handle.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnEliteHpChange);
			}
			this.ChangeToElite();
			this.OnEliteHpChange();
			this.OpenEliteBar();
			this.ToCloseEliteBar(10000);
			return true;
		}

		private void OpenEliteBar()
		{
			if (this.eliteBarTimer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.eliteBarTimer);
				this.eliteBarTimer = -1;
			}
			if (this.boss_hp_Node != null)
			{
				this.boss_hp_Node.CustomSetActive(true);
			}
		}

		private void ToCloseEliteBar(int inDelay)
		{
			if (this.eliteBarTimer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.eliteBarTimer);
				this.eliteBarTimer = -1;
			}
			if (inDelay > 0)
			{
				this.eliteBarTimer = Singleton<CTimerManager>.instance.AddTimer(inDelay, 1, new CTimer.OnTimeUpHandler(this.OnCloseEliteBar));
			}
			else
			{
				this.OnCloseEliteBar(-1);
			}
		}

		private void OnCloseEliteBar(int timer)
		{
			if (this.eliteBarTimer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.eliteBarTimer);
				this.eliteBarTimer = -1;
			}
			if (this.boss_hp_Node != null)
			{
				this.boss_hp_Node.CustomSetActive(false);
			}
			this.RebindBoss();
		}

		private string QueryEliteName(ActorRoot inActor)
		{
			string text = null;
			if (inActor == null)
			{
				return text;
			}
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(inActor.TheActorMeta.ConfigId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				return text;
			}
			string text2 = UT.Bytes2String(dataCfgInfoByCurLevelDiff.szName);
			text = text2;
			if (inActor.SkillControl == null || inActor.SkillControl.talentSystem == null)
			{
				return text;
			}
			string text3 = string.Empty;
			PassiveSkill[] array = inActor.SkillControl.talentSystem.QueryTalents();
			if (array != null)
			{
				PassiveSkill[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					PassiveSkill passiveSkill = array2[i];
					if (passiveSkill != null && passiveSkill.bShowAsElite)
					{
						text3 += passiveSkill.PassiveSkillName;
						text3 += "之";
					}
				}
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text = text3 + text;
			}
			return text;
		}

		private void UnbindCurrentElite()
		{
			if (this.eliteActor)
			{
				if (this.eliteActor.handle.ValueComponent != null)
				{
					this.eliteActor.handle.ValueComponent.HpChgEvent -= new ValueChangeDelegate(this.OnEliteHpChange);
				}
				this.eliteActor.Release();
				this.eliteActorName = null;
			}
			this.ToCloseEliteBar(2500);
		}

		private void onActorDamage(ref HurtEventResultInfo prm)
		{
			if (this.bossActorRoot && this.bossActorRoot.handle.ActorControl != null && this.bossActorRoot.handle.ActorControl.IsInBattle)
			{
				return;
			}
			PoolObjHandle<ActorRoot> src = prm.src;
			PoolObjHandle<ActorRoot> atker = prm.atker;
			if (!src || prm.hurtInfo.hurtType == HurtTypeDef.Therapic)
			{
				return;
			}
			if (src.handle.HudControl == null || !src.handle.HudControl.bBossHpBar)
			{
				return;
			}
			if (src.handle.ActorControl.IsDeadState)
			{
				if (src == this.eliteActor)
				{
					this.UnbindCurrentElite();
				}
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || atker != hostPlayer.Captain)
			{
				return;
			}
			if (src != this.eliteActor)
			{
				this.UnbindCurrentElite();
				this.BindNewElite(src);
			}
			else
			{
				this.ChangeToElite();
				if (this.eliteBarTimer != -1)
				{
					Singleton<CTimerManager>.instance.RemoveTimer(this.eliteBarTimer);
					this.eliteBarTimer = -1;
				}
				else
				{
					this.OnEliteHpChange();
					this.OpenEliteBar();
				}
				this.ToCloseEliteBar(10000);
			}
		}

		private void OnEliteHpChange()
		{
			if (!this.eliteActor)
			{
				return;
			}
			int actorHp = this.eliteActor.handle.ValueComponent.actorHp;
			int actorHpTotal = this.eliteActor.handle.ValueComponent.actorHpTotal;
			this.hpText.set_text(string.Format("{0}/{1}", actorHp, actorHpTotal));
			this.SetHp((float)actorHp / (float)actorHpTotal);
		}

		private void Check_Boss_InBattle(int timer)
		{
			if (!this.bossActorRoot || this.bossActorRoot.handle.ActorControl == null || this.boss_hp_Node == null)
			{
				this.Clear();
				return;
			}
			if (this.bossActorRoot.handle.ActorControl.IsInBattle)
			{
				this.UnbindCurrentElite();
				this.OnBossHpChange();
				this.OpenEliteBar();
				this.ChangeToBoss();
			}
		}

		private void OnBossHpChange()
		{
			if (!this.bossActorRoot || this.bossActorRoot.handle.ValueComponent == null || this.hpText == null)
			{
				this.Clear();
				return;
			}
			int actorHp = this.bossActorRoot.handle.ValueComponent.actorHp;
			int actorHpTotal = this.bossActorRoot.handle.ValueComponent.actorHpTotal;
			this.hpText.set_text(string.Format("{0}/{1}", actorHp, actorHpTotal));
			this.SetHp((float)actorHp / (float)actorHpTotal);
		}

		private void SetHp(float a)
		{
			this.bloodImage.CustomFillAmount(a);
			RectTransform rectTransform = this.blood_eft.transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(a * (this.bloodImage.transform as RectTransform).sizeDelta.x, rectTransform.anchoredPosition.y);
		}
	}
}
