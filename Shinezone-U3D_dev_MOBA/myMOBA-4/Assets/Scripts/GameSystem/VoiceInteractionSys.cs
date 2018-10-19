using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class VoiceInteractionSys : MonoSingleton<VoiceInteractionSys>
	{
		private StarSystemFactory Factory = new StarSystemFactory(typeof(VoiceInteractionAttribute), typeof(VoiceInteraction));

		private bool bActiveSys;

		private DictionaryView<int, ListView<VoiceInteraction>> Interactions = new DictionaryView<int, ListView<VoiceInteraction>>();

		private Dictionary<int, HashSet<int>> HeroStatInfo = new Dictionary<int, HashSet<int>>();

		protected override void Init()
		{
			base.Init();
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
		}

		protected override void OnDestroy()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
			base.OnDestroy();
		}

		private void onFightStart(ref DefaultGameEventParam prm)
		{
			this.Clear();
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			this.bActiveSys = curLvelContext.IsMobaModeWithOutGuide();
			if (!this.bActiveSys)
			{
				return;
			}
			this.StartupSys();
		}

		public void OnEndGame()
		{
			this.Clear();
			this.bActiveSys = false;
		}

		private void StartupSys()
		{
			List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.Captain)
				{
					int configId = player.Captain.handle.TheActorMeta.ConfigId;
					int actorCamp = (int)player.Captain.handle.TheActorMeta.ActorCamp;
					HashSet<int> hashSet = null;
					if (!this.HeroStatInfo.TryGetValue(configId, out hashSet))
					{
						hashSet = new HashSet<int>();
						this.HeroStatInfo.Add(configId, hashSet);
					}
					if (!hashSet.Contains(actorCamp))
					{
						hashSet.Add(actorCamp);
					}
				}
			}
			GameDataMgr.voiceInteractionDatabin.Accept(new Action<ResVoiceInteraction>(this.FilterInteractionCfg));
		}

		private bool HasReceiver(int InCfgID)
		{
			return this.HeroStatInfo.ContainsKey(InCfgID);
		}

		private bool HasReceiverInCamp(int InCfgID, int InTestCamp)
		{
			HashSet<int> hashSet = null;
			return this.HeroStatInfo.TryGetValue(InCfgID, out hashSet) && hashSet.Contains(InTestCamp);
		}

		private bool HasReceiverNotInCamp(int InCfgID, int InTestCamp)
		{
			HashSet<int> hashSet = null;
			if (this.HeroStatInfo.TryGetValue(InCfgID, out hashSet))
			{
				HashSet<int>.Enumerator enumerator = hashSet.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != InTestCamp)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool CheckReceiver(ResVoiceInteraction InInteractionCfg, HashSet<int> InOwnerCamps)
		{
			int hostPlayerCamp = (int)Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
			if (InInteractionCfg.bSpecialReceive != 0)
			{
				for (int i = 0; i < InInteractionCfg.SpecialReceiveConditions.Length; i++)
				{
					if (InInteractionCfg.SpecialReceiveConditions[i] == 0u)
					{
						break;
					}
					int inCfgID = (int)InInteractionCfg.SpecialReceiveConditions[i];
					if (InInteractionCfg.bReceiveType == 100)
					{
						if (this.HasReceiver(inCfgID))
						{
							return true;
						}
					}
					else if (InInteractionCfg.bReceiveType == 0)
					{
						if (this.HasReceiverInCamp(inCfgID, hostPlayerCamp))
						{
							return true;
						}
					}
					else if (InInteractionCfg.bReceiveType == 1 && this.HasReceiverNotInCamp(inCfgID, hostPlayerCamp))
					{
						return true;
					}
				}
			}
			else
			{
				if (InInteractionCfg.bReceiveType == 100)
				{
					return true;
				}
				if (InInteractionCfg.bReceiveType == 0)
				{
					if (InOwnerCamps.Contains(hostPlayerCamp))
					{
						return true;
					}
				}
				else if (InInteractionCfg.bReceiveType == 1)
				{
					HashSet<int>.Enumerator enumerator = InOwnerCamps.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (enumerator.Current != hostPlayerCamp)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void FilterInteractionCfg(ResVoiceInteraction InInteractionCfg)
		{
			HashSet<int> inOwnerCamps = null;
			if (this.HeroStatInfo.TryGetValue((int)InInteractionCfg.dwGroupID, out inOwnerCamps))
			{
				if (!this.CheckReceiver(InInteractionCfg, inOwnerCamps))
				{
					return;
				}
				VoiceInteraction voiceInteraction = this.CreateVoiceInteraction(InInteractionCfg);
				if (voiceInteraction != null)
				{
					voiceInteraction.Init(InInteractionCfg);
					ListView<VoiceInteraction> listView = null;
					if (!this.Interactions.TryGetValue(voiceInteraction.groupID, out listView))
					{
						listView = new ListView<VoiceInteraction>();
						this.Interactions.Add(voiceInteraction.groupID, listView);
					}
					listView.Add(voiceInteraction);
				}
			}
		}

		private VoiceInteraction CreateVoiceInteraction(ResVoiceInteraction InInteractionCfg)
		{
			VoiceInteraction voiceInteraction = this.Factory.Create((int)InInteractionCfg.bTriggerType) as VoiceInteraction;
			DebugHelper.Assert(voiceInteraction != null, "Failed create Interaction for {0}:{1}", new object[]
			{
				InInteractionCfg.dwConfigID,
				InInteractionCfg.bTriggerType
			});
			return voiceInteraction;
		}

		private void Clear()
		{
			DictionaryView<int, ListView<VoiceInteraction>>.Enumerator enumerator = this.Interactions.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, ListView<VoiceInteraction>> current = enumerator.Current;
				ListView<VoiceInteraction> value = current.Value;
				for (int i = 0; i < value.Count; i++)
				{
					value[i].Unit();
				}
			}
			this.Interactions.Clear();
			this.HeroStatInfo.Clear();
		}

		private void LateUpdate()
		{
			if (!this.bActiveSys || MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
			{
				return;
			}
			DictionaryView<int, ListView<VoiceInteraction>>.Enumerator enumerator = this.Interactions.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, ListView<VoiceInteraction>> current = enumerator.Current;
				ListView<VoiceInteraction> value = current.Value;
				int num = -1;
				int num2 = 0;
				for (int i = 0; i < value.Count; i++)
				{
					VoiceInteraction voiceInteraction = value[i];
					if (voiceInteraction.isBeginTrigger && voiceInteraction.priority > num)
					{
						num = voiceInteraction.priority;
						num2++;
					}
				}
				if (num2 > 0)
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					for (int j = 0; j < value.Count; j++)
					{
						VoiceInteraction voiceInteraction2 = value[j];
						if (voiceInteraction2.isBeginTrigger && voiceInteraction2.priority == num)
						{
							voiceInteraction2.DoTrigger();
						}
						voiceInteraction2.FinishTrigger(realtimeSinceStartup);
					}
				}
			}
		}
	}
}
