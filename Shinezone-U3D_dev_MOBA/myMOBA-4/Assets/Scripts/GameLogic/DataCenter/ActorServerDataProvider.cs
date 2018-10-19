using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic.DataCenter
{
	internal class ActorServerDataProvider : ActorDataProviderBase
	{
		private readonly DictionaryView<uint, ListView<COMDT_CHOICEHERO>> _serverCachedInfo = new DictionaryView<uint, ListView<COMDT_CHOICEHERO>>();

		protected COMDT_CHOICEHERO Find(ListView<COMDT_CHOICEHERO> InSearchList, Predicate<COMDT_CHOICEHERO> InPredicate)
		{
			for (int i = 0; i < InSearchList.Count; i++)
			{
				if (InPredicate(InSearchList[i]))
				{
					return InSearchList[i];
				}
			}
			return null;
		}

		public bool GetCallActorServerData(ref ActorMeta actorMeta, ref ActorServerData actorData)
		{
			actorData.TheActorMeta = actorMeta;
			ListView<COMDT_CHOICEHERO> inSearchList = null;
			if (!this._serverCachedInfo.TryGetValue(actorMeta.PlayerId, out inSearchList))
			{
				return false;
			}
			int configId = actorMeta.HostConfigId;
			this.ConvertServerHeroInfo(ref actorData, this.Find(inSearchList, (COMDT_CHOICEHERO hero) => (ulong)hero.stBaseInfo.stCommonInfo.dwHeroID == (ulong)((long)configId)));
			return true;
		}

		public override bool GetActorServerData(ref ActorMeta actorMeta, ref ActorServerData actorData)
		{
			actorData.TheActorMeta = actorMeta;
			ListView<COMDT_CHOICEHERO> inSearchList = null;
			if (!this._serverCachedInfo.TryGetValue(actorMeta.PlayerId, out inSearchList))
			{
				return false;
			}
			int configId = actorMeta.ConfigId;
			this.ConvertServerHeroInfo(ref actorData, this.Find(inSearchList, (COMDT_CHOICEHERO hero) => (ulong)hero.stBaseInfo.stCommonInfo.dwHeroID == (ulong)((long)configId)));
			return true;
		}

		public override bool GetActorServerSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorServerSkillData skillData)
		{
			skillData.TheActorMeta = actorMeta;
			ListView<COMDT_CHOICEHERO> inSearchList = null;
			if (!this._serverCachedInfo.TryGetValue(actorMeta.PlayerId, out inSearchList))
			{
				return false;
			}
			int configId = actorMeta.ConfigId;
			return this.ConvertServerHeroSkillInfo(ref skillData, skillSlot, this.Find(inSearchList, (COMDT_CHOICEHERO hero) => (ulong)hero.stBaseInfo.stCommonInfo.dwHeroID == (ulong)((long)configId)));
		}

		public override bool GetActorServerCommonSkillData(ref ActorMeta actorMeta, out uint skillID)
		{
			skillID = 0u;
			ListView<COMDT_CHOICEHERO> inSearchList = null;
			if (!this._serverCachedInfo.TryGetValue(actorMeta.PlayerId, out inSearchList))
			{
				return false;
			}
			int configId = actorMeta.ConfigId;
			COMDT_CHOICEHERO cOMDT_CHOICEHERO = this.Find(inSearchList, (COMDT_CHOICEHERO hero) => (ulong)hero.stBaseInfo.stCommonInfo.dwHeroID == (ulong)((long)configId));
			if (cOMDT_CHOICEHERO != null)
			{
				skillID = cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.stSkill.dwSelSkillID;
				return true;
			}
			return false;
		}

		public override bool GetActorServerEquipData(ref ActorMeta actorMeta, ActorEquiplSlot equipSlot, ref ActorServerEquipData equipData)
		{
			equipData.TheActorMeta = actorMeta;
			ListView<COMDT_CHOICEHERO> inSearchList = null;
			if (!this._serverCachedInfo.TryGetValue(actorMeta.PlayerId, out inSearchList))
			{
				return false;
			}
			int configId = actorMeta.ConfigId;
			return this.ConvertServerHeroEquipInfo(ref equipData, equipSlot, this.Find(inSearchList, (COMDT_CHOICEHERO hero) => (ulong)hero.stBaseInfo.stCommonInfo.dwHeroID == (ulong)((long)configId)));
		}

		public override bool GetActorServerRuneData(ref ActorMeta actorMeta, ActorRunelSlot runeSlot, ref ActorServerRuneData runeData)
		{
			runeData.TheActorMeta = actorMeta;
			ListView<COMDT_CHOICEHERO> inSearchList = null;
			if (!this._serverCachedInfo.TryGetValue(actorMeta.PlayerId, out inSearchList))
			{
				return false;
			}
			int configId = actorMeta.ConfigId;
			return this.ConvertServerHeroRuneInfo(ref runeData, runeSlot, this.Find(inSearchList, (COMDT_CHOICEHERO hero) => (ulong)hero.stBaseInfo.stCommonInfo.dwHeroID == (ulong)((long)configId)));
		}

		public void AddHeroServerInfo(uint playerId, COMDT_CHOICEHERO serverHeroInfo)
		{
			if (serverHeroInfo == null || serverHeroInfo.stBaseInfo.stCommonInfo.dwHeroID == 0u)
			{
				return;
			}
			ListView<COMDT_CHOICEHERO> listView = null;
			if (!this._serverCachedInfo.ContainsKey(playerId))
			{
				this._serverCachedInfo.Add(playerId, new ListView<COMDT_CHOICEHERO>());
			}
			if (!this._serverCachedInfo.TryGetValue(playerId, out listView))
			{
				return;
			}
			this.ApplyExtraInfoRule(playerId, serverHeroInfo);
			listView.Add(serverHeroInfo);
		}

		internal void ApplyExtraInfoRule(uint playerId, COMDT_CHOICEHERO serverHeroInfo)
		{
			int num = serverHeroInfo.stHeroExtral.iHeroPos;
			if (num != 0)
			{
				return;
			}
			Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(playerId);
			num = player.CampPos;
			if (num != 0)
			{
				serverHeroInfo.stHeroExtral.iHeroPos = num;
				return;
			}
			int heroTeamPosIndex = player.GetHeroTeamPosIndex(serverHeroInfo.stBaseInfo.stCommonInfo.dwHeroID);
			if (heroTeamPosIndex <= 0)
			{
				return;
			}
			ListView<COMDT_CHOICEHERO> listView = null;
			bool flag = true;
			if (this._serverCachedInfo.TryGetValue(playerId, out listView))
			{
				for (int i = heroTeamPosIndex - 1; i >= 0; i--)
				{
					if (listView[i].stHeroExtral.iHeroPos != i)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				serverHeroInfo.stHeroExtral.iHeroPos = heroTeamPosIndex;
			}
		}

		public void ClearHeroServerInfo()
		{
			this._serverCachedInfo.Clear();
		}

		internal void ConvertServerHeroInfo(ref ActorServerData serverData, COMDT_CHOICEHERO serverHeroInfo)
		{
			serverData.SymbolID = new uint[30];
			if (serverHeroInfo == null)
			{
				Debug.Log(string.Format("COMDT_CHOICEHERO is null when try to use hero Id {0}, playerId is {1}", serverData.TheActorMeta.ConfigId, serverData.TheActorMeta.PlayerId));
				DictionaryView<uint, ListView<COMDT_CHOICEHERO>>.Enumerator enumerator = this._serverCachedInfo.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, ListView<COMDT_CHOICEHERO>> current = enumerator.Current;
					uint key = current.Key;
					KeyValuePair<uint, ListView<COMDT_CHOICEHERO>> current2 = enumerator.Current;
					ListView<COMDT_CHOICEHERO> value = current2.Value;
					if (value.Count > 0)
					{
						Debug.Log(string.Format("_serverCachedInfo key {0} value {1}", key, value[0].stBaseInfo.stCommonInfo.dwHeroID));
					}
					else
					{
						Debug.Log(string.Format("_serverCachedInfo key {0} value empty", key));
					}
				}
				return;
			}
			if (serverHeroInfo.stBaseInfo != null && serverHeroInfo.stBaseInfo.stCommonInfo != null)
			{
				serverData.Exp = serverHeroInfo.stBaseInfo.stCommonInfo.dwExp;
				serverData.Level = (uint)serverHeroInfo.stBaseInfo.stCommonInfo.wLevel;
				serverData.Star = (uint)serverHeroInfo.stBaseInfo.stCommonInfo.wStar;
				if (serverHeroInfo.stBaseInfo.stCommonInfo.stQuality != null)
				{
					serverData.TheQualityInfo.Quality = (uint)serverHeroInfo.stBaseInfo.stCommonInfo.stQuality.wQuality;
					serverData.TheQualityInfo.SubQuality = (uint)serverHeroInfo.stBaseInfo.stCommonInfo.stQuality.wSubQuality;
				}
				if (serverHeroInfo.stBaseInfo.stCommonInfo.stProficiency != null)
				{
					serverData.TheProficiencyInfo.Level = (uint)serverHeroInfo.stBaseInfo.stCommonInfo.stProficiency.bLv;
					serverData.TheProficiencyInfo.Proficiency = serverHeroInfo.stBaseInfo.stCommonInfo.stProficiency.dwProficiency;
				}
				serverData.SkinId = (uint)serverHeroInfo.stBaseInfo.stCommonInfo.wSkinID;
			}
			if (serverHeroInfo.stBurningInfo != null)
			{
				serverData.TheBurnInfo.HeroRemainingHp = serverHeroInfo.stBurningInfo.dwBloodTTH;
				serverData.TheBurnInfo.IsDead = (serverHeroInfo.stBurningInfo.bIsDead != 0);
			}
			if (serverHeroInfo.stHeroExtral != null)
			{
				serverData.TheExtraInfo.BornPointIndex = serverHeroInfo.stHeroExtral.iHeroPos;
			}
			for (int i = 0; i < serverHeroInfo.SymbolID.Length; i++)
			{
				serverData.SymbolID[i] = serverHeroInfo.SymbolID[i];
			}
			serverData.m_customRecommendEquips = CCustomRcmdEquipInfo.ConvertCltRcmdEquipListInfo(serverHeroInfo.stNewHeroEquipList);
		}

		internal bool ConvertServerHeroSkillInfo(ref ActorServerSkillData serverData, ActorSkillSlot skillSlot, COMDT_CHOICEHERO serverHeroInfo)
		{
			if (skillSlot >= ActorSkillSlot.SlotMaxCount)
			{
				return false;
			}
			serverData.SkillSlot = skillSlot;
			serverData.IsUnlocked = (serverHeroInfo.stBaseInfo.stCommonInfo.stSkill.astSkillInfo[(int)((UIntPtr)skillSlot)].bUnlocked != 0);
			serverData.SkillLevel = (uint)serverHeroInfo.stBaseInfo.stCommonInfo.stSkill.astSkillInfo[(int)((UIntPtr)skillSlot)].wLevel;
			return true;
		}

		internal bool ConvertServerHeroEquipInfo(ref ActorServerEquipData serverData, ActorEquiplSlot equipSlot, COMDT_CHOICEHERO serverHeroInfo)
		{
			if (equipSlot >= ActorEquiplSlot.SlotMaxCount)
			{
				return false;
			}
			serverData.EquipSlot = equipSlot;
			return true;
		}

		internal bool ConvertServerHeroRuneInfo(ref ActorServerRuneData serverData, ActorRunelSlot runeSlot, COMDT_CHOICEHERO serverHeroInfo)
		{
			if (runeSlot >= ActorRunelSlot.SlotMaxCount)
			{
				return false;
			}
			serverData.RuneSlot = runeSlot;
			if (serverHeroInfo != null)
			{
				serverData.RuneId = serverHeroInfo.SymbolID[(int)((UIntPtr)runeSlot)];
			}
			return true;
		}

		public override int Fast_GetActorServerDataBornIndex(ref ActorMeta actorMeta)
		{
			ListView<COMDT_CHOICEHERO> listView = null;
			if (!this._serverCachedInfo.TryGetValue(actorMeta.PlayerId, out listView) || listView.Count < 1)
			{
				return 0;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				if ((ulong)listView[i].stBaseInfo.stCommonInfo.dwHeroID == (ulong)((long)actorMeta.ConfigId))
				{
					return listView[i].stHeroExtral.iHeroPos;
				}
			}
			return 0;
		}
	}
}
