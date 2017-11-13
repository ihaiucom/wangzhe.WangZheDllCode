using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class MinimapView
	{
		private class SpawnGroupCounter
		{
			public int CountdownTime;

			public int AlertTime;

			public GameObject TextObj;

			public bool bAlertPreroll;

			public bool bDidAlert;

			public int timer;

			public Text timerText;
		}

		private DictionaryView<CTailsman, GameObject> m_mapPointer = new DictionaryView<CTailsman, GameObject>();

		private CUIContainerScript m_mapPointerContainer;

		private ListView<MinimapView.SpawnGroupCounter> m_spawnGroupCounter = new ListView<MinimapView.SpawnGroupCounter>();

		private CUIContainerScript m_spawnGroupCounterContainer;

		private CUIContainerScript GetMapPointerContainer()
		{
			return null;
		}

		private CUIContainerScript GetSpawnGroupTextContainer()
		{
			return null;
		}

		private void AddSpawnGroupCounter(int inCountdown, int inAlertPreroll, Vector3 inInitPos, SpawnerWrapper.ESpawnObjectType inObjType)
		{
			this.m_spawnGroupCounterContainer = this.GetSpawnGroupTextContainer();
			if (this.m_spawnGroupCounterContainer != null)
			{
				int element = this.m_spawnGroupCounterContainer.GetElement();
				if (element >= 0)
				{
					MinimapView.SpawnGroupCounter spawnGroupCounter = new MinimapView.SpawnGroupCounter();
					spawnGroupCounter.CountdownTime = inCountdown;
					spawnGroupCounter.timer = spawnGroupCounter.CountdownTime;
					spawnGroupCounter.AlertTime = spawnGroupCounter.timer - inAlertPreroll;
					spawnGroupCounter.bAlertPreroll = (inAlertPreroll > 0);
					spawnGroupCounter.bDidAlert = false;
					RectTransform rectTransform = null;
					GameObject element2 = this.m_spawnGroupCounterContainer.GetElement(element);
					if (element2 != null)
					{
						rectTransform = (element2.transform as RectTransform);
						spawnGroupCounter.TextObj = element2;
						spawnGroupCounter.timerText = Utility.FindChild(element2, "TimerText").GetComponent<Text>();
					}
					if (rectTransform != null)
					{
						rectTransform.SetAsFirstSibling();
					}
					SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
					if (curLvelContext != null && curLvelContext.IsMobaMode())
					{
						this.UpdateUIMap(rectTransform, inInitPos, (float)curLvelContext.m_mapWidth, (float)curLvelContext.m_mapHeight);
					}
					this.m_spawnGroupCounter.Add(spawnGroupCounter);
				}
			}
		}

		private void AddCharmIcon(CTailsman inCharm)
		{
			if (inCharm == null)
			{
				return;
			}
			if (this.m_mapPointer.ContainsKey(inCharm))
			{
				return;
			}
			this.m_mapPointerContainer = this.GetMapPointerContainer();
			if (this.m_mapPointerContainer != null)
			{
				int element = this.m_mapPointerContainer.GetElement();
				if (element >= 0)
				{
					RectTransform rectTransform = null;
					GameObject element2 = this.m_mapPointerContainer.GetElement(element);
					if (element2 != null)
					{
						rectTransform = (element2.transform as RectTransform);
						Image component = element2.GetComponent<Image>();
						component.SetSprite(string.Format("{0}{1}", "UGUI/Sprite/Battle/", "Img_Map_Base_Green"), Singleton<CBattleSystem>.instance.FormScript, true, false, false, false);
					}
					if (rectTransform != null)
					{
						rectTransform.SetAsFirstSibling();
					}
					Vector3 actorPosition = Vector3.zero;
					if (inCharm.Presentation != null)
					{
						actorPosition = inCharm.Presentation.transform.position;
					}
					SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
					if (curLvelContext != null && curLvelContext.IsMobaMode())
					{
						this.UpdateUIMap(rectTransform, actorPosition, (float)curLvelContext.m_mapWidth, (float)curLvelContext.m_mapHeight);
					}
					this.m_mapPointer.Add(inCharm, element2);
				}
			}
		}

		private void UpdateUIMap(RectTransform mapPointerRectTransform, Vector3 actorPosition, float mapWidth, float mapHeight)
		{
			if (mapPointerRectTransform != null && mapWidth != 0f && mapHeight != 0f)
			{
				float x = actorPosition.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x;
				float y = actorPosition.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y;
				mapPointerRectTransform.anchoredPosition = new Vector2(x, y);
			}
		}

		public void RemoveCharmIcon(CTailsman inCharm)
		{
			if (inCharm == null)
			{
				return;
			}
			if (!this.m_mapPointer.ContainsKey(inCharm))
			{
				return;
			}
			GameObject gameObject = this.m_mapPointer[inCharm];
			this.m_mapPointer.Remove(inCharm);
			if (gameObject != null && this.m_mapPointerContainer != null)
			{
				this.m_mapPointerContainer.RecycleElement(gameObject);
			}
		}

		public void Init(GameObject dragonInfo, SpawnGroup dragonSpawnGroup)
		{
			Singleton<GameEventSys>.instance.AddEventHandler<SCommonSpawnEventParam>(GameEventDef.Event_SpawnGroupStartCount, new RefAction<SCommonSpawnEventParam>(this.OnSpawnGroupStartCount));
			Singleton<GameEventSys>.instance.AddEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanSpawn, new RefAction<STailsmanEventParam>(this.OnTailsmanSpawn));
			Singleton<GameEventSys>.instance.AddEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, new RefAction<STailsmanEventParam>(this.OnTailsmanUsed));
		}

		public void Clear()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<SCommonSpawnEventParam>(GameEventDef.Event_SpawnGroupStartCount, new RefAction<SCommonSpawnEventParam>(this.OnSpawnGroupStartCount));
			Singleton<GameEventSys>.instance.RmvEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanSpawn, new RefAction<STailsmanEventParam>(this.OnTailsmanSpawn));
			Singleton<GameEventSys>.instance.RmvEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, new RefAction<STailsmanEventParam>(this.OnTailsmanUsed));
			this.m_mapPointerContainer = null;
			this.m_spawnGroupCounterContainer = null;
		}

		private void OnSpawnGroupStartCount(ref SCommonSpawnEventParam param)
		{
			this.AddSpawnGroupCounter(param.LeftTime, param.AlertPreroll, (Vector3)param.SpawnPos, param.SpawnObjType);
		}

		private void OnTailsmanSpawn(ref STailsmanEventParam param)
		{
			this.AddCharmIcon(param.tailsman.handle);
		}

		private void OnTailsmanUsed(ref STailsmanEventParam param)
		{
			this.RemoveCharmIcon(param.tailsman.handle);
		}

		private void Draw()
		{
			ListView<MinimapView.SpawnGroupCounter>.Enumerator enumerator = this.m_spawnGroupCounter.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MinimapView.SpawnGroupCounter current = enumerator.Current;
				if (current != null && current.timerText != null)
				{
					int num = current.timer / 1000;
					int num2 = num / 60;
					int num3 = num - num2 * 60;
					current.timerText.set_text(string.Format("{0:D2}:{1:D2}", num2, num3));
				}
			}
		}

		public void UpdateLogic(int inDelta)
		{
			if (this.m_spawnGroupCounter.Count > 0)
			{
				int count = this.m_spawnGroupCounter.Count;
				for (int i = count - 1; i >= 0; i--)
				{
					MinimapView.SpawnGroupCounter spawnGroupCounter = this.m_spawnGroupCounter[i];
					spawnGroupCounter.timer -= inDelta;
					if (spawnGroupCounter.bAlertPreroll && !spawnGroupCounter.bDidAlert && spawnGroupCounter.timer <= spawnGroupCounter.AlertTime)
					{
						spawnGroupCounter.bDidAlert = true;
					}
					if (spawnGroupCounter.timer <= 0)
					{
						if (this.m_spawnGroupCounterContainer != null && spawnGroupCounter.TextObj != null)
						{
							this.m_spawnGroupCounterContainer.RecycleElement(spawnGroupCounter.TextObj);
						}
						spawnGroupCounter.TextObj = null;
						spawnGroupCounter.timerText = null;
						this.m_spawnGroupCounter.RemoveAt(i);
					}
				}
			}
			this.Draw();
		}
	}
}
