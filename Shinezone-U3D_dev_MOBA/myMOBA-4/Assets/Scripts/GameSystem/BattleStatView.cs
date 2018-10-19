using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class BattleStatView
	{
		private class HeroItem
		{
			public GameObject rootHeroView;

			public GameObject rootMatchInfo;

			public GameObject rootValueInfo;

			public Image icon;

			public GameObject mineBg;

			public Text level;

			public Text playerName;

			public Text heroName;

			public Text reviveTime;

			public Text killNum;

			public Text deadNum;

			public Text killMon;

			public Text assistNum;

			public GameObject talentSkill;

			public Image talentSkillImage;

			public Text talentSkillCD;

			public Image[] equipList = new Image[6];

			public Image horizonEquipImg;

			public Text heroHP;

			public Text heroAD;

			public Text heroAP;

			public Text heroADDef;

			public Text heroAPDef;

			public GameObject voiceIconsNode;

			public GameObject ClickGameObject;

			public HeroKDA kdaData;

			public bool Visible
			{
				get
				{
					return this.rootHeroView != null && this.rootMatchInfo != null && this.rootValueInfo != null && (this.rootHeroView.activeSelf && this.rootMatchInfo.activeSelf) && this.rootValueInfo.activeSelf;
				}
				set
				{
					if (this.rootHeroView != null && this.rootMatchInfo != null && this.rootValueInfo != null)
					{
						this.rootHeroView.CustomSetActive(value);
						this.rootMatchInfo.CustomSetActive(value);
						this.rootValueInfo.CustomSetActive(value);
					}
				}
			}

			public HeroItem(GameObject heroNode, GameObject matchNode, GameObject valueNode)
			{
				this.rootHeroView = heroNode;
				this.rootMatchInfo = matchNode;
				this.rootValueInfo = valueNode;
				this.ClickGameObject = Utility.FindChild(heroNode, "clickimg");
				this.icon = Utility.GetComponetInChild<Image>(heroNode, "HeadIcon");
				this.mineBg = Utility.FindChild(heroNode, "MineBg");
				this.level = Utility.GetComponetInChild<Text>(heroNode, "Level");
				this.playerName = Utility.GetComponetInChild<Text>(heroNode, "PlayerName");
				this.heroName = Utility.GetComponetInChild<Text>(heroNode, "HeroName");
				this.reviveTime = Utility.GetComponetInChild<Text>(heroNode, "ReviveTime");
				this.voiceIconsNode = Utility.FindChild(heroNode, "Voice");
				this.killNum = Utility.GetComponetInChild<Text>(matchNode, "KillNum");
				this.deadNum = Utility.GetComponetInChild<Text>(matchNode, "DeadNum");
				this.killMon = Utility.GetComponetInChild<Text>(matchNode, "KillMon");
				this.assistNum = Utility.GetComponetInChild<Text>(matchNode, "AssistNum");
				GameObject p = Utility.FindChild(matchNode, "TalentIcon");
				this.equipList[0] = Utility.GetComponetInChild<Image>(p, "img1");
				this.equipList[1] = Utility.GetComponetInChild<Image>(p, "img2");
				this.equipList[2] = Utility.GetComponetInChild<Image>(p, "img3");
				this.equipList[3] = Utility.GetComponetInChild<Image>(p, "img4");
				this.equipList[4] = Utility.GetComponetInChild<Image>(p, "img5");
				this.equipList[5] = Utility.GetComponetInChild<Image>(p, "img6");
				this.horizonEquipImg = Utility.GetComponetInChild<Image>(p, "img7");
				this.talentSkill = Utility.FindChild(matchNode, "TalentSkill");
				this.talentSkillImage = Utility.GetComponetInChild<Image>(this.talentSkill, "Image");
				this.talentSkillCD = Utility.GetComponetInChild<Text>(this.talentSkill, "TimeCD");
				this.heroHP = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/HP");
				this.heroAD = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/AD");
				this.heroAP = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/AP");
				this.heroADDef = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/ADDef");
				this.heroAPDef = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/APDef");
				GameObject gameObject = heroNode.transform.FindChild("ReviveTime").gameObject;
				gameObject.SetActive(true);
				GameObject gameObject2 = matchNode.transform.FindChild("TalentSkill/TimeCD").gameObject;
				gameObject2.SetActive(true);
				this.kdaData = null;
			}

			public void Validate(HeroKDA kdaData)
			{
				if (kdaData != null)
				{
					this.kdaData = kdaData;
				}
				if (this.kdaData == null)
				{
					return;
				}
				this.icon.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint)this.kdaData.HeroId, 0u)), Singleton<CBattleSystem>.instance.FormScript, true, false, false, false);
				Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.kdaData.actorHero);
				this.playerName.text = ownerPlayer.Name;
				this.heroName.text = this.kdaData.actorHero.handle.TheStaticData.TheResInfo.Name;
				this.level.text = this.kdaData.actorHero.handle.ValueComponent.actorSoulLevel.ToString();
				this.killNum.text = this.kdaData.numKill.ToString();
				this.deadNum.text = this.kdaData.numDead.ToString();
				this.killMon.text = (this.kdaData.numKillMonster + this.kdaData.numKillSoldier).ToString();
				this.killMon.text = this.kdaData.TotalCoin.ToString();
				this.assistNum.text = this.kdaData.numAssist.ToString();
				int num = 0;
				for (int i = 0; i < 6; i++)
				{
					ushort equipID = this.kdaData.Equips[i].m_equipID;
					if (equipID != 0)
					{
						CUICommonSystem.SetEquipIcon(equipID, this.equipList[num++].gameObject, Singleton<CBattleSystem>.instance.FormScript);
					}
				}
				for (int j = num; j < 6; j++)
				{
					this.equipList[j].gameObject.GetComponent<Image>().SetSprite(string.Format("{0}BattleSettle_EquipmentSpaceNew", CUIUtility.s_Sprite_Dynamic_Talent_Dir), Singleton<CBattleSystem>.instance.FormScript, true, false, false, false);
				}
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.m_bEnableOrnamentSlot && curLvelContext.m_bEnableShopHorizonTab)
				{
					ushort horizonEquipId = this.kdaData.actorHero.handle.EquipComponent.m_horizonEquipId;
					if (horizonEquipId == 0)
					{
						this.horizonEquipImg.SetSprite(string.Format("{0}BattleSettle_EquipmentSpaceNew", CUIUtility.s_Sprite_Dynamic_Talent_Dir), Singleton<CBattleSystem>.instance.FormScript, true, false, false, false);
					}
					else
					{
						CUICommonSystem.SetEquipIcon(horizonEquipId, this.horizonEquipImg.gameObject, Singleton<CBattleSystem>.instance.FormScript);
					}
				}
				if (ownerPlayer == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer())
				{
					this.playerName.color = CUIUtility.s_Text_Color_Self;
					this.mineBg.CustomSetActive(true);
				}
				else
				{
					if (ownerPlayer.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						this.playerName.color = CUIUtility.s_Text_Color_Camp_1;
					}
					else
					{
						this.playerName.color = CUIUtility.s_Text_Color_Camp_2;
					}
					this.mineBg.CustomSetActive(false);
				}
			}

			public void updateReviceCD(Player curPlayer)
			{
				if (curPlayer == null || !curPlayer.Captain || curPlayer.Captain.handle.ActorControl == null)
				{
					return;
				}
				if (curPlayer.Captain.handle.ActorControl.IsDeadState)
				{
					this.reviveTime.text = SimpleNumericString.GetNumeric(Mathf.FloorToInt((float)curPlayer.Captain.handle.ActorControl.ReviveCooldown * 0.001f));
					this.icon.color = CUIUtility.s_Color_Grey;
				}
				else
				{
					this.reviveTime.text = string.Empty;
					this.icon.color = CUIUtility.s_Color_Full;
				}
			}

			public void updateTalentSkillCD(Player curPlayer, CUIFormScript parentFormScript)
			{
				if (curPlayer == null || !curPlayer.Captain || curPlayer.Captain.handle.SkillControl == null)
				{
					return;
				}
				SkillSlot skillSlot = curPlayer.Captain.handle.SkillControl.SkillSlotArray[5];
				if (skillSlot == null)
				{
					this.talentSkill.CustomSetActive(false);
				}
				else
				{
					if (!string.IsNullOrEmpty(skillSlot.SkillObj.IconName))
					{
						this.talentSkillImage.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + skillSlot.SkillObj.IconName, parentFormScript, true, false, false, false);
					}
					this.talentSkill.CustomSetActive(true);
					if (skillSlot.CurSkillCD > 0)
					{
						this.talentSkillCD.text = SimpleNumericString.GetNumeric(Mathf.FloorToInt((int)skillSlot.CurSkillCD * 0.001f));
						this.talentSkillImage.color = CUIUtility.s_Color_Grey;
					}
					else
					{
						this.talentSkillCD.text = string.Empty;
						this.talentSkillImage.color = CUIUtility.s_Color_Full;
					}
				}
			}

			public void updateHeroValue(Player curPlayer)
			{
				if (curPlayer != null && curPlayer.Captain && curPlayer.Captain.handle.ValueComponent.mActorValue != null)
				{
					ValueDataInfo[] actorValue = curPlayer.Captain.handle.ValueComponent.mActorValue.GetActorValue();
					this.heroHP.text = string.Format("{0}", actorValue[5].totalValue);
					this.heroAD.text = string.Format("{0}", actorValue[1].totalValue);
					this.heroAP.text = string.Format("{0}", actorValue[2].totalValue);
					this.heroADDef.text = string.Format("{0}", actorValue[3].totalValue);
					this.heroAPDef.text = string.Format("{0}", actorValue[4].totalValue);
				}
			}

			public void updateHeroVoiceState(Player curPlayer)
			{
				if (curPlayer != null && curPlayer.Captain && curPlayer.Captain.handle.ValueComponent.mActorValue != null)
				{
					this.SetEventParam(curPlayer.PlayerUId);
					Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
					if (hostPlayer == null)
					{
						return;
					}
					bool flag = Singleton<GamePlayerCenter>.instance.IsAtSameCamp(hostPlayer.PlayerId, curPlayer.PlayerId);
					if (flag && !Singleton<WatchController>.GetInstance().IsWatching)
					{
						this.voiceIconsNode.CustomSetActive(true);
						if (curPlayer.Computer && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_isWarmBattle)
						{
							this.voiceIconsNode.CustomSetActive(false);
						}
						if (hostPlayer.PlayerUId == curPlayer.PlayerUId)
						{
							this._updateVoiceIcon(this.voiceIconsNode, MonoSingleton<VoiceSys>.instance.curVoiceState, false);
						}
						else
						{
							CS_VOICESTATE_TYPE value = MonoSingleton<VoiceSys>.GetInstance().TryGetVoiceState(curPlayer.PlayerUId);
							this._updateVoiceIcon(this.voiceIconsNode, value, MonoSingleton<VoiceSys>.instance.IsForbid(curPlayer.PlayerUId));
						}
					}
					else
					{
						this.voiceIconsNode.CustomSetActive(false);
					}
				}
			}

			private void SetEventParam(ulong uid)
			{
				if (this.ClickGameObject == null)
				{
					return;
				}
				CUIEventScript component = this.ClickGameObject.GetComponent<CUIEventScript>();
				if (component == null)
				{
					return;
				}
				component.m_onClickEventParams.commonUInt64Param1 = uid;
			}

			private void _updateVoiceIcon(GameObject node, CS_VOICESTATE_TYPE value, bool bForbidden = false)
			{
				if (node == null)
				{
					return;
				}
				GameObject gameObject = node.transform.Find("AllOpen").gameObject;
				GameObject gameObject2 = node.transform.Find("AllClose").gameObject;
				GameObject gameObject3 = node.transform.Find("HalfOpen").gameObject;
				GameObject gameObject4 = node.transform.Find("Forbidden").gameObject;
				if (bForbidden)
				{
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(false);
					gameObject3.CustomSetActive(false);
					gameObject4.CustomSetActive(true);
					return;
				}
				gameObject4.CustomSetActive(false);
				switch (value)
				{
				case CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE:
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(true);
					gameObject3.CustomSetActive(false);
					return;
				case CS_VOICESTATE_TYPE.CS_VOICESTATE_PART:
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(false);
					gameObject3.CustomSetActive(true);
					return;
				case CS_VOICESTATE_TYPE.CS_VOICESTATE_FULL:
					gameObject.CustomSetActive(true);
					gameObject2.CustomSetActive(false);
					gameObject3.CustomSetActive(false);
					return;
				default:
					return;
				}
			}
		}

		private class ChartView
		{
			public enum ChartType
			{
				None,
				ExpTrend,
				MoneyTrend
			}

			private const int X_TICK_MAX = 4;

			private const int Y_TICK_MAX = 8;

			private const int Y_SECS_NUM = 5;

			private const int Y_SECS_MUL = 100;

			private GameObject root;

			private Text title;

			private GameObject bannerExp;

			private GameObject bannerMoney;

			private Text camp1Data;

			private Text camp2Data;

			private Text[] xAxis;

			private Text[] yAxis;

			private CUIGraphLineScript drawArea;

			public ChartView(GameObject root)
			{
				this.root = root;
				this.title = Utility.GetComponetInChild<Text>(root, "Title");
				this.bannerExp = Utility.FindChild(root, "Banner/Exp");
				this.bannerMoney = Utility.FindChild(root, "Banner/Coin");
				this.camp1Data = Utility.GetComponetInChild<Text>(root, "CmpBar/Camp1Data");
				this.camp2Data = Utility.GetComponetInChild<Text>(root, "CmpBar/Camp2Data");
				this.drawArea = Utility.GetComponetInChild<CUIGraphLineScript>(root, "DrawArea");
				if (this.drawArea)
				{
					this.drawArea.SetLineSegEndCallback(new GrapLineSegmentDelegate(BattleStatView.PaintLineEnd));
				}
				this.xAxis = new Text[5];
				for (int i = 0; i <= 4; i++)
				{
					this.xAxis[i] = Utility.GetComponetInChild<Text>(root, "xAxis/x_" + i);
				}
				this.yAxis = new Text[9];
				for (int j = 0; j <= 8; j++)
				{
					this.yAxis[j] = Utility.GetComponetInChild<Text>(root, "yAxis/y_" + j);
				}
			}

			public void Show(BattleStatView.ChartView.ChartType chartType, SampleData data, CUIFormScript form, ArrayList dragonKillInfos)
			{
				this.root.CustomSetActive(true);
				GameObject widget = form.GetWidget(0);
				float height = (widget.transform as RectTransform).rect.height;
				string str = chartType.ToString();
				this.title.text = Singleton<CTextManager>.GetInstance().GetText(str + "_Title");
				this.bannerExp.CustomSetActive(chartType == BattleStatView.ChartView.ChartType.ExpTrend);
				this.bannerMoney.CustomSetActive(chartType == BattleStatView.ChartView.ChartType.MoneyTrend);
				this.camp1Data.text = data.curDataLeft.ToString();
				this.camp2Data.text = data.curDataRight.ToString();
				float num = (data.count <= 1) ? 1f : (data.step * (float)(data.count - 1));
				int num2 = Mathf.CeilToInt(num / 4f);
				for (int i = 0; i <= 4; i++)
				{
					int num3 = i * num2;
					this.xAxis[i].text = string.Format("{0:D2}:{1:D2}", num3 / 60, num3 % 60);
				}
				int num4 = Math.Max(Math.Abs(data.min), Math.Abs(data.max));
				int num5 = Mathf.CeilToInt((float)num4 / 5f);
				bool flag = num5 % 100 == 0;
				num5 /= 100;
				if (!flag)
				{
					num5++;
				}
				if (num5 < 1)
				{
					num5 = 1;
				}
				num5 *= 100;
				int num6 = 4;
				for (int j = 1; j <= num6; j++)
				{
					string text = "+" + num5 * j;
					this.yAxis[num6 + j].text = text;
					this.yAxis[num6 - j].text = text;
				}
				RectTransform component = this.drawArea.GetComponent<RectTransform>();
				Vector3 vector = CUIUtility.WorldToScreenPoint(form.GetCamera(), component.position);
				float num7 = form.ChangeFormValueToScreen(component.rect.width);
				float num8 = form.ChangeFormValueToScreen(component.rect.height);
				vector.x -= num7 * 0.5f;
				float num9 = num7 / (float)(num2 * 4);
				float num10 = num8 * 0.5f / (float)(num5 * 5);
				Vector3[][] array = new Vector3[dragonKillInfos.Count + 1][];
				Color[] array2 = new Color[dragonKillInfos.Count + 1];
				array[0] = new Vector3[data.count];
				array2[0] = ((chartType != BattleStatView.ChartView.ChartType.ExpTrend) ? Color.yellow : Color.green);
				int num11 = 0;
				for (int k = 0; k < data.count; k++)
				{
					float num12 = (float)data[k] * num10;
					float x = vector.x + data.step * (float)k * num9;
					float num13 = vector.y + num12;
					array[0][k] = new Vector3(x, num13);
					if (k > 0 && num11 < dragonKillInfos.Count)
					{
						DragonKillInfo dragonKillInfo = dragonKillInfos[num11] as DragonKillInfo;
						if ((float)k * data.step > (float)dragonKillInfo.time && (float)(k - 1) * data.step <= (float)dragonKillInfo.time)
						{
							int num14 = 1;
							COM_PLAYERCAMP actorCamp = dragonKillInfo.killer.handle.TheActorMeta.ActorCamp;
							if (actorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1)
							{
								if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
								{
									num14 = -1;
								}
							}
							else
							{
								num14 = 1;
							}
							int num15 = (int)Math.Abs(num8 / 2f + 4f - height / 2f - num12 * (float)num14);
							if (num15 == 0)
							{
								num15 = 1 * num14;
							}
							array[num11 + 1] = new Vector3[num15];
							int dragonType = dragonKillInfo.dragonType;
							if (dragonType != 0)
							{
								if (dragonType == 1)
								{
									array2[num11 + 1] = new Color(0.870588243f, 0.31764707f, 0.8156863f);
								}
							}
							else
							{
								array2[num11 + 1] = new Color(0.819607854f, 0.6431373f, 0.2784314f);
							}
							for (int l = 0; l < num15; l++)
							{
								array[num11 + 1][l] = new Vector3(x, num13 + (float)(l * num14));
							}
							num11++;
						}
					}
				}
				this.drawArea.thickness = 2f;
				this.drawArea.drawSpeed = 1000f;
				this.drawArea.SetVertexs(array, array2);
			}

			public void Hide()
			{
				this.root.CustomSetActive(false);
			}
		}

		private const string k_dragonIconPrefix = "DragonIcon";

		private const int HERO_MAX_NUM = 5;

		public static string s_battleStateViewUIForm = "UGUI/Form/Battle/Form_BattleStateView.prefab";

		private bool m_battleKDAChanged;

		private bool m_battleHeroPropertyChange;

		private int _defaultSelectIndex;

		private bool m_bListCampInited;

		private List<HeroKDA> m_heroListCamp1 = new List<HeroKDA>();

		private List<HeroKDA> m_heroListCamp2 = new List<HeroKDA>();

		private List<Player> m_playerListCamp1 = new List<Player>();

		private List<Player> m_playerListCamp2 = new List<Player>();

		private bool m_sortByCoin;

		private GameObject _root;

		private GameObject heroView;

		private GameObject matchInfo;

		private GameObject valueInfo;

		private GameObject sortByCoinBtn;

		private CUIListScript list;

		private BattleStatView.ChartView chartView;

		private BattleStatView.HeroItem[] _heroList0;

		private BattleStatView.HeroItem[] _heroList1;

		private CUIFormScript m_statViewFormScript;

		public bool SortByCoin
		{
			get
			{
				return this.m_sortByCoin;
			}
			set
			{
				this.m_sortByCoin = value;
				this.m_battleHeroPropertyChange = true;
				this.UpdateSortBtn();
				this.SortHeroAndPlayer();
				this.UpdateKDAView();
				this.UpdateBattleState(null);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					return;
				}
				PlayerPrefs.SetInt(string.Format("Sgmae_Battle_SortByCoin_{0}", masterRoleInfo.playerUllUID), (!value) ? 0 : 1);
				PlayerPrefs.Save();
				CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.SortBYCoinBtnClick);
			}
		}

		public bool Visible
		{
			get
			{
				CUIFormScript statViewFormScript = this.m_statViewFormScript;
				return statViewFormScript != null && !statViewFormScript.IsHided();
			}
		}

		public void Init()
		{
			if (this.m_statViewFormScript != null)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(BattleStatView.s_battleStateViewUIForm, false, true);
			this.m_statViewFormScript = cUIFormScript;
			cUIFormScript.GetWidget(0).CustomSetActive(false);
			cUIFormScript.GetWidget(1).CustomSetActive(false);
			this._root = cUIFormScript.gameObject.transform.Find("BattleStatView").gameObject;
			if (this._root == null)
			{
				return;
			}
			this._heroList0 = new BattleStatView.HeroItem[5];
			this._heroList1 = new BattleStatView.HeroItem[5];
			this.heroView = Utility.FindChild(this._root, "HeroView");
			this.matchInfo = Utility.FindChild(this._root, "BattleMatchInfo");
			this.valueInfo = Utility.FindChild(this._root, "HeroValueInfo");
			GameObject gameObject = Utility.FindChild(this._root, "ChartView");
			gameObject.CustomSetActive(false);
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				this.chartView = new BattleStatView.ChartView(gameObject);
			}
			this.sortByCoinBtn = Utility.FindChild(cUIFormScript.gameObject, "TopCommon/SortByCoin");
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				this.m_sortByCoin = (PlayerPrefs.GetInt(string.Format("Sgmae_Battle_SortByCoin_{0}", masterRoleInfo.playerUllUID)) > 0);
			}
			this.UpdateSortBtn();
			if (this.heroView == null || this.matchInfo == null || this.valueInfo == null)
			{
				return;
			}
			Animation component = this.matchInfo.transform.GetComponent<Animation>();
			if (component != null)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && (!curLvelContext.m_bEnableOrnamentSlot || !curLvelContext.m_bEnableShopHorizonTab))
				{
					component.enabled = true;
					component.Play();
				}
				else
				{
					component.enabled = false;
				}
			}
			for (int i = 0; i < 5; i++)
			{
				this._heroList0[i] = new BattleStatView.HeroItem(Utility.FindChild(this.heroView, "HeroList_0/" + i), Utility.FindChild(this.matchInfo, "HeroList_0/" + i), Utility.FindChild(this.valueInfo, "HeroList_0/" + i));
				this._heroList1[i] = new BattleStatView.HeroItem(Utility.FindChild(this.heroView, "HeroList_1/" + i), Utility.FindChild(this.matchInfo, "HeroList_1/" + i), Utility.FindChild(this.valueInfo, "HeroList_1/" + i));
			}
			GameObject gameObject2 = cUIFormScript.gameObject.transform.Find("TopCommon/Panel_Menu/ListMenu").gameObject;
			if (gameObject2 == null)
			{
				return;
			}
			this.list = gameObject2.GetComponent<CUIListScript>();
			string[] titleList;
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				titleList = new string[]
				{
					Singleton<CTextManager>.GetInstance().GetText("BattleStateView_MatchInfo"),
					Singleton<CTextManager>.GetInstance().GetText("BattleStateView_HeroInfo"),
					Singleton<CTextManager>.GetInstance().GetText("BattleStateView_ExpTrend"),
					Singleton<CTextManager>.GetInstance().GetText("BattleStateView_EcoTrend")
				};
			}
			else
			{
				titleList = new string[]
				{
					Singleton<CTextManager>.GetInstance().GetText("BattleStateView_MatchInfo"),
					Singleton<CTextManager>.GetInstance().GetText("BattleStateView_HeroInfo")
				};
			}
			this._defaultSelectIndex = 0;
			CUICommonSystem.InitMenuPanel(gameObject2, titleList, this._defaultSelectIndex, true);
			this.heroView.CustomSetActive(true);
			this.matchInfo.CustomSetActive(true);
			this.valueInfo.CustomSetActive(false);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CloseStatView, new CUIEventManager.OnUIEventHandler(this.onCloseClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleMatchInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleMatchInfoTabChange));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED, new Action(this.OnBattleKDAChanged));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_HERO_PROPERTY_CHANGED, new Action(this.OnBattleHeroPropertyChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_BattleStatViewSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_StateViewClickHeroIcon, new CUIEventManager.OnUIEventHandler(this.OnBattle_StateViewClickHeroIcon));
			this.Hide();
		}

		private void OnBattle_StateViewClickHeroIcon(CUIEvent uievent)
		{
			if (uievent.m_eventParams.commonUInt64Param1 == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerUId)
			{
				return;
			}
			MonoSingleton<VoiceSys>.instance.SwitchForbidden(uievent.m_eventParams.commonUInt64Param1);
			this.RefreshVoiceStateIfNess();
		}

		public void Clear()
		{
			MonoSingleton<VoiceSys>.instance.ClearInBattleForbidMember();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_BattleStatViewSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CloseStatView, new CUIEventManager.OnUIEventHandler(this.onCloseClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleMatchInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleMatchInfoTabChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_StateViewClickHeroIcon, new CUIEventManager.OnUIEventHandler(this.OnBattle_StateViewClickHeroIcon));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED, new Action(this.OnBattleKDAChanged));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_HERO_PROPERTY_CHANGED, new Action(this.OnBattleHeroPropertyChange));
			Singleton<CUIManager>.GetInstance().CloseForm(BattleStatView.s_battleStateViewUIForm);
			this._root = null;
			this._heroList0 = null;
			this._heroList1 = null;
			this.m_heroListCamp1.Clear();
			this.m_heroListCamp2.Clear();
			this.m_playerListCamp1.Clear();
			this.m_playerListCamp2.Clear();
			this.m_bListCampInited = false;
			this.sortByCoinBtn = null;
			this._defaultSelectIndex = 0;
			this.m_statViewFormScript = null;
		}

		public void LateUpdate()
		{
			if (this.m_battleKDAChanged)
			{
				this.UpdateKDAView();
				this.m_battleKDAChanged = false;
			}
		}

		public void Show()
		{
			if (null == this._root)
			{
				return;
			}
			CUIFormScript statViewFormScript = this.m_statViewFormScript;
			if (statViewFormScript == null)
			{
				return;
			}
			statViewFormScript.Appear(enFormHideFlag.HideByCustom, true);
			if (this.list != null)
			{
				this.list.SelectElement(-1, true);
				this.list.SelectElement(this._defaultSelectIndex, true);
			}
			this.SortHeroAndPlayer();
			this.UpdateBattleState(null);
			this.UpdateKDAView();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ReviveTimeChange, new CUIEventManager.OnUIEventHandler(this.UpdateBattleState));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.onSoulLvlChange));
			this.RefreshVoiceStateIfNess();
			Singleton<CUIParticleSystem>.instance.Hide(null);
		}

		public void RefreshVoiceStateIfNess()
		{
			CUIFormScript statViewFormScript = this.m_statViewFormScript;
			if (statViewFormScript == null)
			{
				return;
			}
			if (!statViewFormScript.IsHided())
			{
				for (int i = 0; i < this.m_playerListCamp1.Count; i++)
				{
					if (i >= this._heroList0.Length)
					{
						break;
					}
					Player curPlayer = this.m_playerListCamp1[i];
					BattleStatView.HeroItem heroItem = this._heroList0[i];
					heroItem.updateHeroVoiceState(curPlayer);
				}
				for (int i = 0; i < this.m_playerListCamp2.Count; i++)
				{
					if (i >= this._heroList1.Length)
					{
						break;
					}
					Player curPlayer = this.m_playerListCamp2[i];
					BattleStatView.HeroItem heroItem = this._heroList1[i];
					heroItem.updateHeroVoiceState(curPlayer);
				}
			}
		}

		private void UpdateBattleState(CUIEvent evt = null)
		{
			if (null == this._root)
			{
				return;
			}
			CUIFormScript statViewFormScript = this.m_statViewFormScript;
			if (statViewFormScript == null)
			{
				return;
			}
			if (!statViewFormScript.IsHided())
			{
				for (int i = 0; i < this.m_playerListCamp1.Count; i++)
				{
					if (i >= this._heroList0.Length)
					{
						break;
					}
					Player curPlayer = this.m_playerListCamp1[i];
					BattleStatView.HeroItem heroItem = this._heroList0[i];
					heroItem.updateReviceCD(curPlayer);
					heroItem.updateTalentSkillCD(curPlayer, this.m_statViewFormScript);
					if (this.m_battleHeroPropertyChange)
					{
						heroItem.updateHeroValue(curPlayer);
					}
				}
				for (int i = 0; i < this.m_playerListCamp2.Count; i++)
				{
					if (i >= this._heroList1.Length)
					{
						break;
					}
					Player curPlayer = this.m_playerListCamp2[i];
					BattleStatView.HeroItem heroItem = this._heroList1[i];
					heroItem.updateReviceCD(curPlayer);
					heroItem.updateTalentSkillCD(curPlayer, this.m_statViewFormScript);
					if (this.m_battleHeroPropertyChange)
					{
						heroItem.updateHeroValue(curPlayer);
					}
				}
				if (this.m_battleHeroPropertyChange)
				{
					this.m_battleHeroPropertyChange = false;
				}
			}
		}

		private void SortHeroAndPlayer()
		{
			bool forceUpdate = true;
			if (this.m_heroListCamp1.Count > 0 || this.m_heroListCamp2.Count > 0 || this.m_playerListCamp1.Count > 0 || this.m_playerListCamp2.Count > 0)
			{
				forceUpdate = false;
			}
			this.UpdateListCamp(forceUpdate);
			if (this.m_sortByCoin)
			{
				this.m_heroListCamp1.Sort(new Comparison<HeroKDA>(this.SortByCoinAndPos));
				this.m_heroListCamp2.Sort(new Comparison<HeroKDA>(this.SortByCoinAndPos));
				this.m_playerListCamp1.Sort(new Comparison<Player>(this.SortByCoinAndPos));
				this.m_playerListCamp2.Sort(new Comparison<Player>(this.SortByCoinAndPos));
			}
			else
			{
				this.m_heroListCamp1.Sort(new Comparison<HeroKDA>(this.SortByPos));
				this.m_heroListCamp2.Sort(new Comparison<HeroKDA>(this.SortByPos));
				this.m_playerListCamp1.Sort(new Comparison<Player>(this.SortByPos));
				this.m_playerListCamp2.Sort(new Comparison<Player>(this.SortByPos));
			}
		}

		private void UpdateKDAView()
		{
			if (null == this._root)
			{
				return;
			}
			CUIFormScript statViewFormScript = this.m_statViewFormScript;
			if (statViewFormScript != null && !statViewFormScript.IsHided())
			{
				int i;
				for (i = 0; i < this.m_heroListCamp1.Count; i++)
				{
					if (i < this._heroList0.Length)
					{
						this._heroList0[i].Visible = true;
						this._heroList0[i].Validate(this.m_heroListCamp1[i]);
					}
				}
				while (i < this._heroList0.Length)
				{
					this._heroList0[i].Visible = false;
					i++;
				}
				for (i = 0; i < this.m_heroListCamp2.Count; i++)
				{
					if (i < this._heroList1.Length)
					{
						this._heroList1[i].Visible = true;
						this._heroList1[i].Validate(this.m_heroListCamp2[i]);
					}
				}
				while (i < this._heroList1.Length)
				{
					this._heroList1[i].Visible = false;
					i++;
				}
			}
		}

		public void Hide()
		{
			if (null == this._root)
			{
				return;
			}
			CUIFormScript statViewFormScript = this.m_statViewFormScript;
			if (statViewFormScript == null)
			{
				return;
			}
			statViewFormScript.Hide(enFormHideFlag.HideByCustom, true);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ReviveTimeChange, new CUIEventManager.OnUIEventHandler(this.UpdateBattleState));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.onSoulLvlChange));
			if (this.chartView != null)
			{
				this.chartView.Hide();
			}
			Singleton<CUIParticleSystem>.instance.Show(null);
		}

		private void OnBttleMatchInfoTabChange(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			if (component == null)
			{
				return;
			}
			int selectedIndex = component.GetSelectedIndex();
			if (selectedIndex < 0 || selectedIndex >= component.GetElementAmount())
			{
				return;
			}
			this._defaultSelectIndex = selectedIndex;
			CUIFormScript statViewFormScript = this.m_statViewFormScript;
			if (statViewFormScript == null)
			{
				return;
			}
			if (statViewFormScript != null)
			{
				if (this.heroView == null && this.matchInfo == null && this.valueInfo == null)
				{
					return;
				}
				this.SortHeroAndPlayer();
				if (this._defaultSelectIndex == 0)
				{
					this.heroView.CustomSetActive(true);
					this.matchInfo.CustomSetActive(true);
					this.valueInfo.CustomSetActive(false);
					this.sortByCoinBtn.CustomSetActive(true);
					this.UpdateKDAView();
					if (this.chartView != null)
					{
						this.chartView.Hide();
					}
				}
				else if (this._defaultSelectIndex == 1)
				{
					this.heroView.CustomSetActive(true);
					this.matchInfo.CustomSetActive(false);
					this.valueInfo.CustomSetActive(true);
					this.sortByCoinBtn.CustomSetActive(true);
					this.m_battleHeroPropertyChange = true;
					this.UpdateBattleState(null);
					if (this.chartView != null)
					{
						this.chartView.Hide();
					}
				}
				else
				{
					this.heroView.CustomSetActive(false);
					this.matchInfo.CustomSetActive(false);
					this.valueInfo.CustomSetActive(false);
					this.sortByCoinBtn.CustomSetActive(false);
					this.m_battleHeroPropertyChange = false;
					WatchForm watchForm = Singleton<CBattleSystem>.GetInstance().WatchForm;
					if (watchForm != null && this.chartView != null)
					{
						this.chartView.Show((this._defaultSelectIndex != 2) ? BattleStatView.ChartView.ChartType.MoneyTrend : BattleStatView.ChartView.ChartType.ExpTrend, (this._defaultSelectIndex != 2) ? watchForm.moneySample : watchForm.expSample, Singleton<CUIManager>.GetInstance().GetForm(BattleStatView.s_battleStateViewUIForm), watchForm.dragonKillInfos);
						GameObject widget = statViewFormScript.GetWidget(0);
						for (int i = 0; i < widget.transform.parent.childCount; i++)
						{
							GameObject gameObject = widget.transform.parent.GetChild(i).gameObject;
							if (gameObject.name.Contains("DragonIcon"))
							{
								gameObject.CustomSetActive(false);
							}
						}
					}
				}
			}
		}

		public void Toggle()
		{
			if (this.Visible)
			{
				this.Hide();
			}
			else
			{
				this.Show();
			}
		}

		private void onCloseClick(CUIEvent evt)
		{
			this.Hide();
		}

		private void onSoulLvlChange(PoolObjHandle<ActorRoot> act, int curVal)
		{
			if (!this._root)
			{
				return;
			}
			BattleStatView.HeroItem[] array = this._heroList0;
			for (int i = 0; i < array.Length; i++)
			{
				BattleStatView.HeroItem heroItem = array[i];
				if (heroItem != null && heroItem.Visible && heroItem.kdaData != null && heroItem.kdaData.actorHero == act)
				{
					heroItem.level.text = curVal.ToString();
				}
				if (i + 1 == array.Length && array == this._heroList0)
				{
					array = this._heroList1;
					i = -1;
				}
			}
		}

		private void OnBattleKDAChanged()
		{
			this.m_battleKDAChanged = true;
		}

		private void OnBattleHeroPropertyChange()
		{
			this.m_battleHeroPropertyChange = true;
		}

		private void UpdateListCamp(bool forceUpdate)
		{
			if (forceUpdate || !this.m_bListCampInited)
			{
				this.m_playerListCamp1.Clear();
				this.m_playerListCamp2.Clear();
				List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
				for (int i = 0; i < allPlayers.Count; i++)
				{
					if (allPlayers[i].PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						this.m_playerListCamp1.Add(allPlayers[i]);
					}
					else if (allPlayers[i].PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
					{
						this.m_playerListCamp2.Add(allPlayers[i]);
					}
				}
				this.m_heroListCamp1.Clear();
				this.m_heroListCamp2.Clear();
				CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
				DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
					PlayerKDA value = current.Value;
					if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						ListView<HeroKDA>.Enumerator enumerator2 = value.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							this.m_heroListCamp1.Add(enumerator2.Current);
						}
					}
					else if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
					{
						ListView<HeroKDA>.Enumerator enumerator3 = value.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							this.m_heroListCamp2.Add(enumerator3.Current);
						}
					}
				}
			}
		}

		private int SortByCoinAndPos(HeroKDA left, HeroKDA right)
		{
			return (right.TotalCoin - left.TotalCoin) * 10 + (left.CampPos - right.CampPos);
		}

		private int SortByPos(HeroKDA left, HeroKDA right)
		{
			return left.CampPos - right.CampPos;
		}

		private void UpdateSortBtn()
		{
			if (this.sortByCoinBtn != null)
			{
				Text componetInChild = Utility.GetComponetInChild<Text>(this.sortByCoinBtn, "Text");
				if (componetInChild != null)
				{
					if (this.m_sortByCoin)
					{
						componetInChild.text = Singleton<CTextManager>.instance.GetText("Battle_Statistic_Sort_Coin");
					}
					else
					{
						componetInChild.text = Singleton<CTextManager>.instance.GetText("Battle_Statistic_Sort_Common");
					}
				}
			}
		}

		private int SortByCoinAndPos(Player left, Player right)
		{
			PlayerKDA playerKDA = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetPlayerKDA(left.PlayerId);
			PlayerKDA playerKDA2 = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetPlayerKDA(right.PlayerId);
			return (playerKDA2.TotalCoin - playerKDA.TotalCoin) * 10 + (left.CampPos - right.CampPos);
		}

		private int SortByPos(Player left, Player right)
		{
			return left.CampPos - right.CampPos;
		}

		private void OnSortClick(CUIEvent uievent)
		{
			this.SortByCoin = !this.SortByCoin;
		}

		public void PaintLineBegin(int segIdx)
		{
		}

		public static void PaintLineEnd(int segIdx, Vector3 pos)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(BattleStatView.s_battleStateViewUIForm);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			GameObject widget2 = form.GetWidget(0);
			GameObject widget3 = form.GetWidget(1);
			GameObject gameObject = widget2;
			Vector2 vector = CUIUtility.WorldToScreenPoint(form.GetCamera(), (widget.transform as RectTransform).position);
			RectTransform rectTransform = widget.transform.Find("TopMark") as RectTransform;
			RectTransform rectTransform2 = widget.transform.Find("BottomMark") as RectTransform;
			if (segIdx > 0)
			{
				string name = "DragonIcon" + segIdx;
				WatchForm watchForm = Singleton<CBattleSystem>.GetInstance().WatchForm;
				if (segIdx <= watchForm.dragonKillInfos.Count)
				{
					DragonKillInfo dragonKillInfo = watchForm.dragonKillInfos[segIdx - 1] as DragonKillInfo;
					if (dragonKillInfo.dragonType == 0)
					{
						gameObject = widget3;
					}
				}
				Transform transform = widget2.transform.parent.FindChild(name);
				GameObject gameObject2;
				if (transform != null)
				{
					gameObject2 = transform.gameObject;
					gameObject2.GetComponent<Image>().SetSprite(gameObject.GetComponent<Image>());
				}
				else
				{
					gameObject2 = CUIComponent.DuplicateGO(gameObject);
					gameObject2.name = "DragonIcon" + segIdx;
				}
				gameObject2.CustomSetActive(true);
				Vector2 screenPoint = default(Vector2);
				screenPoint.x = pos.x;
				screenPoint.y = pos.y;
				bool flag = screenPoint.y > vector.y;
				Vector3 position = CUIUtility.ScreenToWorldPoint(form.GetCamera(), screenPoint, gameObject.transform.position.z);
				position.y = ((!flag) ? rectTransform2.position.y : rectTransform.position.y);
				gameObject2.transform.position = position;
			}
		}
	}
}
