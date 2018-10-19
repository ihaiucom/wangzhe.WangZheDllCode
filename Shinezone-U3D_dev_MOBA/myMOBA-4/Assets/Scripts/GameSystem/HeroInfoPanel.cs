using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class HeroInfoPanel
	{
		private enum enHeroInfoItemWidget
		{
			None = -1,
			HeroIcon,
			IconCircle,
			IconCircleEnemy,
			HpImage,
			HpImageBg,
			ReviveCntDownText,
			SkillCanUse,
			SkillCanUseBg
		}

		private enum enHeroInfoChangeType
		{
			None = -1,
			Hp,
			ReviveCntDown,
			DaZhao
		}

		private const int HERO_MAX_NUM = 5;

		private Dictionary<uint, int> m_HeroToIndexDic = new Dictionary<uint, int>();

		private int[] m_heroInfoChangeBitsList = new int[4];

		private CUIListScript heroInfoList;

		private ListView<Text> m_reviveTextCacheList = new ListView<Text>();

		private ListView<Image> m_hpImageCacheList = new ListView<Image>();

		private List<PoolObjHandle<ActorRoot>> m_enemyHeroList = new List<PoolObjHandle<ActorRoot>>();

		private List<PoolObjHandle<ActorRoot>> m_teamHeroList = new List<PoolObjHandle<ActorRoot>>();

		public void Init(GameObject heroInfoPanelObj)
		{
			if (null == heroInfoPanelObj)
			{
				return;
			}
			this.heroInfoList = heroInfoPanelObj.GetComponent<CUIListScript>();
			if (null == this.heroInfoList)
			{
				return;
			}
			this.heroInfoList.SetElementAmount(10);
			this.InitTextImageCacheList();
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				DebugHelper.Assert(false, "HeroInfoPanel Init hostPlayer is null");
				return;
			}
			this.m_HeroToIndexDic.Clear();
			this.m_enemyHeroList.Clear();
			this.m_teamHeroList.Clear();
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				bool flag = i >= 5;
				CUIListElementScript elemenet = this.heroInfoList.GetElemenet(i);
				if (elemenet != null)
				{
					elemenet.gameObject.CustomSetActive(false);
					GameObject widget = elemenet.GetWidget(1);
					GameObject widget2 = elemenet.GetWidget(2);
					widget.CustomSetActive(!flag);
					widget2.CustomSetActive(flag);
					GameObject widget3 = elemenet.GetWidget(6);
					GameObject widget4 = elemenet.GetWidget(7);
					widget3.CustomSetActive(false);
					widget4.CustomSetActive(!flag);
				}
			}
			List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
			int num2 = 0;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				if (allPlayers[i].PlayerCamp == hostPlayer.PlayerCamp && allPlayers[i] != hostPlayer)
				{
					int listIndex = num2;
					CUIListElementScript elemenet2 = this.heroInfoList.GetElemenet(num2);
					if (elemenet2 != null && allPlayers[i].Captain)
					{
						elemenet2.gameObject.CustomSetActive(true);
						this.InitHeroItemData(elemenet2, allPlayers[i].Captain);
						this.AddHeroToDic(allPlayers[i].Captain, listIndex);
						this.m_teamHeroList.Add(allPlayers[i].Captain);
					}
					num2++;
				}
			}
			this.InitEventListener();
		}

		private void InitTextImageCacheList()
		{
			this.m_reviveTextCacheList.Clear();
			this.m_hpImageCacheList.Clear();
			if (null == this.heroInfoList)
			{
				return;
			}
			int elementAmount = this.heroInfoList.GetElementAmount();
			for (int i = 0; i < elementAmount; i++)
			{
				CUIListElementScript elemenet = this.heroInfoList.GetElemenet(i);
				if (elemenet != null)
				{
					GameObject widget = elemenet.GetWidget(3);
					if (widget != null)
					{
						this.m_hpImageCacheList.Add(widget.GetComponent<Image>());
					}
					GameObject widget2 = elemenet.GetWidget(5);
					if (widget2 != null)
					{
						this.m_reviveTextCacheList.Add(widget2.GetComponent<Text>());
					}
				}
			}
		}

		private Text GetReviveText(int index)
		{
			if (index >= 0 && index < this.m_reviveTextCacheList.Count)
			{
				return this.m_reviveTextCacheList[index];
			}
			return null;
		}

		private Image GetHpImage(int index)
		{
			if (index >= 0 && index < this.m_hpImageCacheList.Count)
			{
				return this.m_hpImageCacheList[index];
			}
			return null;
		}

		private void AddHeroToDic(PoolObjHandle<ActorRoot> actor, int listIndex)
		{
			if (!actor)
			{
				return;
			}
			if (!this.m_HeroToIndexDic.ContainsKey(actor.handle.ObjID))
			{
				this.m_HeroToIndexDic.Add(actor.handle.ObjID, listIndex);
			}
			else
			{
				this.m_HeroToIndexDic[actor.handle.ObjID] = listIndex;
			}
		}

		public void Clear()
		{
			this.UnInitEventListener();
			this.m_HeroToIndexDic.Clear();
			this.m_enemyHeroList.Clear();
			this.m_hpImageCacheList.Clear();
			this.m_reviveTextCacheList.Clear();
		}

		public void Update()
		{
			if (null == this.heroInfoList)
			{
				return;
			}
			int count = this.m_teamHeroList.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = this.m_teamHeroList[i];
				CUIListElementScript elemenet = this.heroInfoList.GetElemenet(i);
				if (ptr && ptr.handle.SkillControl != null && !(null == elemenet))
				{
					SkillSlot skillSlot = null;
					if (ptr.handle.SkillControl.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_3, out skillSlot) && skillSlot.IsUnLock() && skillSlot.IsCDReady)
					{
						elemenet.GetWidget(6).CustomSetActive(true);
					}
					else
					{
						elemenet.GetWidget(6).CustomSetActive(false);
					}
					int num = this.m_heroInfoChangeBitsList[i];
					if (num != 0)
					{
						if ((num & 1) != 0)
						{
							Image hpImage = this.GetHpImage(i);
							if (hpImage != null)
							{
								hpImage.fillAmount = ptr.handle.ValueComponent.GetHpRate().single;
							}
							num &= -2;
						}
						Text reviveText = this.GetReviveText(i);
						if (reviveText != null && (num & 2) != 0)
						{
							reviveText.text = SimpleNumericString.GetNumeric(Mathf.RoundToInt((float)ptr.handle.ActorControl.ReviveCooldown * 0.001f));
						}
						this.m_heroInfoChangeBitsList[i] = num;
					}
				}
			}
			int count2 = this.m_enemyHeroList.Count;
			for (int i = 0; i < count2; i++)
			{
				CUIListElementScript elemenet = this.heroInfoList.GetElemenet(i + 5);
				if (elemenet != null && this.m_enemyHeroList[i])
				{
					Text reviveText2 = this.GetReviveText(i + 5);
					if (reviveText2 != null)
					{
						reviveText2.text = SimpleNumericString.GetNumeric(Mathf.RoundToInt((float)this.m_enemyHeroList[i].handle.ActorControl.ReviveCooldown * 0.001f));
					}
				}
			}
		}

		private void InitEventListener()
		{
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
		}

		private void UnInitEventListener()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
		}

		private void InitHeroItemData(CUIListElementScript elementScript, PoolObjHandle<ActorRoot> actor)
		{
			if (null == elementScript || !actor || actor.handle.ValueComponent == null)
			{
				return;
			}
			bool flag = actor.handle.IsHostCamp();
			elementScript.gameObject.CustomSetActive(flag || actor.handle.ActorControl.IsDeadState);
			this.RefreshHeroIconImage(elementScript, actor);
			this.RefreshHeroIconImageColor(elementScript, actor);
			GameObject widget = elementScript.GetWidget(3);
			GameObject widget2 = elementScript.GetWidget(4);
			if (widget != null && widget2 != null)
			{
				widget.CustomSetActive(flag);
				widget2.CustomSetActive(flag);
				if (flag)
				{
					widget.GetComponent<Image>().fillAmount = actor.handle.ValueComponent.GetHpRate().single;
				}
			}
			GameObject widget3 = elementScript.GetWidget(5);
			if (widget3 != null)
			{
				if (actor.handle.ActorControl.IsDeadState)
				{
					widget3.CustomSetActive(true);
					widget3.GetComponent<Text>().text = string.Format("{0}", Mathf.RoundToInt((float)actor.handle.ActorControl.ReviveCooldown * 0.001f));
				}
				else
				{
					widget3.CustomSetActive(false);
				}
			}
		}

		private void RefreshHeroIconImage(CUIListElementScript elementScript, PoolObjHandle<ActorRoot> actor)
		{
			if (null == elementScript || !actor)
			{
				return;
			}
			GameObject widget = elementScript.GetWidget(0);
			if (widget != null)
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)actor.handle.TheActorMeta.ConfigId);
				Image component = widget.GetComponent<Image>();
				if (component != null && dataByKey != null)
				{
					component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustCircle_Dir, dataByKey.szImagePath), Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
				}
			}
		}

		private void RefreshHeroIconImageColor(CUIListElementScript elementScript, PoolObjHandle<ActorRoot> actor)
		{
			if (null == elementScript || !actor)
			{
				return;
			}
			GameObject widget = elementScript.GetWidget(0);
			if (widget != null)
			{
				Image component = widget.GetComponent<Image>();
				if (actor.handle.IsHostCamp())
				{
					component.color = ((!actor.handle.ActorControl.IsDeadState) ? CUIUtility.s_Color_White : CUIUtility.s_Color_GrayShader);
				}
				else
				{
					component.color = CUIUtility.s_Color_GrayShader;
				}
			}
		}

		private void RefreshHeroReviveCntDown(CUIListElementScript elementScript, PoolObjHandle<ActorRoot> actor)
		{
			if (null == elementScript || !actor)
			{
				return;
			}
			GameObject widget = elementScript.GetWidget(5);
			if (widget != null && actor.handle.ActorControl.IsDeadState)
			{
				widget.GetComponent<Text>().text = SimpleNumericString.GetNumeric(Mathf.RoundToInt((float)actor.handle.ActorControl.ReviveCooldown * 0.001f));
			}
		}

		private void SetHeroChangeFlag(PoolObjHandle<ActorRoot> actor, HeroInfoPanel.enHeroInfoChangeType changeType, bool bSetFlag = true)
		{
			int num = 0;
			if (!actor || !this.m_HeroToIndexDic.TryGetValue(actor.handle.ObjID, out num))
			{
				return;
			}
			int num2 = this.m_heroInfoChangeBitsList[num];
			if (bSetFlag)
			{
				num2 |= 1 << (int)changeType;
			}
			else
			{
				num2 &= ~(1 << (int)changeType);
			}
			this.m_heroInfoChangeBitsList[num] = num2;
		}

		private void OnActorDead(ref GameDeadEventParam eventParam)
		{
			if (eventParam.src && eventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				bool flag = eventParam.src.handle.IsHostCamp();
				int num = 0;
				CUIListElementScript cUIListElementScript = null;
				if (!flag)
				{
					this.m_enemyHeroList.Add(eventParam.src);
					num = this.m_enemyHeroList.Count - 1;
					cUIListElementScript = this.GetHeroListElement(num + 5);
				}
				if (this.m_HeroToIndexDic.TryGetValue(eventParam.src.handle.ObjID, out num))
				{
					cUIListElementScript = this.GetHeroListElement(num);
				}
				if (cUIListElementScript != null)
				{
					if (!flag)
					{
						cUIListElementScript.gameObject.CustomSetActive(true);
						this.InitHeroItemData(cUIListElementScript, eventParam.src);
					}
					else
					{
						this.RefreshHeroIconImageColor(cUIListElementScript, eventParam.src);
						cUIListElementScript.GetWidget(5).CustomSetActive(true);
						this.SetHeroChangeFlag(eventParam.src, HeroInfoPanel.enHeroInfoChangeType.ReviveCntDown, true);
					}
				}
			}
		}

		private CUIListElementScript GetHeroListElement(int listIndex)
		{
			if (this.heroInfoList != null)
			{
				return this.heroInfoList.GetElemenet(listIndex);
			}
			return null;
		}

		private void OnActorRevive(ref DefaultGameEventParam eventParam)
		{
			if (eventParam.src && eventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				bool flag = eventParam.src.handle.IsHostCamp();
				int listIndex = 0;
				if (flag)
				{
					if (this.m_HeroToIndexDic.TryGetValue(eventParam.src.handle.ObjID, out listIndex))
					{
						CUIListElementScript heroListElement = this.GetHeroListElement(listIndex);
						if (heroListElement != null)
						{
							this.RefreshHeroIconImageColor(heroListElement, eventParam.src);
							heroListElement.GetWidget(5).CustomSetActive(false);
						}
						this.SetHeroChangeFlag(eventParam.src, HeroInfoPanel.enHeroInfoChangeType.ReviveCntDown, false);
					}
				}
				else
				{
					this.m_enemyHeroList.Remove(eventParam.src);
					this.RefreshEnemyHeroListUI();
				}
			}
		}

		private void RefreshEnemyHeroListUI()
		{
			int count = this.m_enemyHeroList.Count;
			for (int i = 0; i < 5; i++)
			{
				CUIListElementScript heroListElement = this.GetHeroListElement(i + 5);
				if (heroListElement != null)
				{
					heroListElement.gameObject.CustomSetActive(i < count);
					if (i < count)
					{
						this.RefreshHeroIconImage(heroListElement, this.m_enemyHeroList[i]);
						this.RefreshHeroReviveCntDown(heroListElement, this.m_enemyHeroList[i]);
					}
				}
			}
		}

		private void OnHeroHpChange(PoolObjHandle<ActorRoot> actor, int curHp, int totalHp)
		{
			if (actor && actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && actor.handle.IsHostCamp())
			{
				this.SetHeroChangeFlag(actor, HeroInfoPanel.enHeroInfoChangeType.Hp, true);
			}
		}
	}
}
