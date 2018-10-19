using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class BurnExpeditionModel
	{
		public enum EDifficultyType
		{
			Normal = 1,
			Hard
		}

		public class HeroData
		{
			public int HP;

			public int maxHP;
		}

		public int curSelect_LevelIndex = -1;

		public int curSelect_BoxIndex = -1;

		public int curSelect_BuffIndex;

		public int lastUnlockLevelIndex = -1;

		private DictionaryView<uint, COMDT_BURNING_LEVEL_DETAIL> _mapDetails = new DictionaryView<uint, COMDT_BURNING_LEVEL_DETAIL>();

		public COMDT_BURNING_LEVEL_PROGRESS _data;

		private int[] robotIconInts;

		private ListView<int> levelRandomRobotIcon = new ListView<int>(6);

		private DictionaryView<uint, Dictionary<uint, int>> _hero_map = new DictionaryView<uint, Dictionary<uint, int>>();

		private DictionaryView<int, string> buff_ids = new DictionaryView<int, string>();

		private DictionaryView<int, string> buff_icons = new DictionaryView<int, string>();

		public BurnExpeditionModel.EDifficultyType curDifficultyType
		{
			get;
			set;
		}

		public BurnExpeditionModel()
		{
			this.curDifficultyType = BurnExpeditionModel.EDifficultyType.Normal;
			GameDataMgr.burnBuffMap.Accept(new Action<ResBurningBuff>(this.OnVisit));
			this.robotIconInts = new int[]
			{
				301050,
				301060,
				301070,
				301080,
				301090,
				301100,
				301110,
				301120,
				301130,
				301140,
				301150,
				301160,
				301170,
				301180,
				301190,
				301200,
				301210,
				301220,
				301270,
				301280,
				301290,
				301300,
				301330,
				301340,
				301350,
				301390,
				301440,
				301490,
				301500,
				301520,
				301050,
				301060,
				301070,
				301080,
				301090,
				301100,
				301110,
				301120,
				301130,
				301140,
				301150,
				301160,
				301170,
				301180,
				301190,
				301200,
				301210,
				301220,
				301270,
				301280,
				301290,
				301300,
				301330,
				301340,
				301350,
				301390,
				301440,
				301490,
				301500,
				301520
			};
		}

		public void ClearAll()
		{
			this._data = null;
			this.curSelect_LevelIndex = -1;
			this.curSelect_BoxIndex = -1;
			this.curSelect_BuffIndex = 0;
			this.lastUnlockLevelIndex = -1;
			this._hero_map.Clear();
			this._mapDetails.Clear();
		}

		public void RandomRobotIcon()
		{
			this.levelRandomRobotIcon.Clear();
			for (int i = 0; i < 6; i++)
			{
				int item = this.robotIconInts[UnityEngine.Random.Range(0, this.robotIconInts.Length)];
				this.levelRandomRobotIcon.Add(item);
			}
		}

		public int GetRandomRobotIcon(int index)
		{
			return this.levelRandomRobotIcon[index];
		}

		private void OnVisit(ResBurningBuff InBuff)
		{
			this.buff_ids.Add(InBuff.iSkillCombineID, StringHelper.UTF8BytesToString(ref InBuff.szBuffDesc));
			this.buff_icons.Add(InBuff.iSkillCombineID, StringHelper.UTF8BytesToString(ref InBuff.szBuffIcon));
		}

		public bool IsHeroInRecord(uint heroCfgID)
		{
			Dictionary<uint, int> dictionary = this._hero_map[(uint)this.curDifficultyType];
			return dictionary != null && dictionary.ContainsKey(heroCfgID);
		}

		public int Get_HeroHP_Percent(uint heroCfgID)
		{
			Dictionary<uint, int> dictionary = this._hero_map[(uint)this.curDifficultyType];
			if (dictionary == null)
			{
				return -1;
			}
			if (dictionary.ContainsKey(heroCfgID))
			{
				return dictionary[heroCfgID];
			}
			return -1;
		}

		public int Get_HeroHP(uint heroCfgID)
		{
			int num = this.Get_HeroHP_Percent(heroCfgID);
			return (int)((float)num / 10000f * (float)this.Get_HeroMaxHP(heroCfgID));
		}

		public int Get_HeroMaxHP(uint heroCfgID)
		{
			int result = -1;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CHeroInfo cHeroInfo;
			if (masterRoleInfo.GetHeroInfo(heroCfgID, out cHeroInfo, true))
			{
				if (cHeroInfo != null)
				{
					result = cHeroInfo.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
				}
			}
			else if (masterRoleInfo.IsFreeHero(heroCfgID))
			{
				result = ((CCustomHeroData)CHeroDataFactory.CreateCustomHeroData(heroCfgID)).heroMaxHP;
			}
			return result;
		}

		public void SetHero_Hp(uint heroCfgID, int hp)
		{
			Dictionary<uint, int> dictionary = this._hero_map[(uint)this.curDifficultyType];
			if (dictionary.ContainsKey(heroCfgID))
			{
				dictionary[heroCfgID] = hp;
			}
			else
			{
				dictionary.Add(heroCfgID, hp);
			}
		}

		public void Reset_Data()
		{
			DictionaryView<uint, Dictionary<uint, int>>.Enumerator enumerator = this._hero_map.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, Dictionary<uint, int>> current = enumerator.Current;
				if (current.Value != null)
				{
					KeyValuePair<uint, Dictionary<uint, int>> current2 = enumerator.Current;
					current2.Value.Clear();
				}
			}
			this.curSelect_LevelIndex = -1;
		}

		public void SetLevelDetail(BurnExpeditionModel.EDifficultyType type, COMDT_BURNING_LEVEL_DETAIL detail)
		{
			if (!this._mapDetails.ContainsKey((uint)type))
			{
				this._mapDetails.Add((uint)type, detail);
			}
			else
			{
				this._mapDetails[(uint)type] = detail;
			}
		}

		public void UnLockLevel(int levelIndex)
		{
			this.Set_Level_State(this.curDifficultyType, levelIndex, COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED);
		}

		public void FinishLevel(int levelIndex)
		{
			this.Set_Level_State(this.curDifficultyType, levelIndex, COM_LEVEL_STATUS.COM_LEVEL_STATUS_FINISHED);
		}

		public void UnLockBox(int levelIndex)
		{
			this.Set_Box_State(this.curDifficultyType, levelIndex, COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED);
		}

		public void FinishBox(int levelIndex)
		{
			this.Set_Box_State(this.curDifficultyType, levelIndex, COM_LEVEL_STATUS.COM_LEVEL_STATUS_FINISHED);
		}

		public string Get_Buff_Description(int buffid)
		{
			if (this.buff_ids.ContainsKey(buffid))
			{
				return this.buff_ids[buffid];
			}
			return string.Format(UT.GetText("Burn_Error_Find_Buff"), buffid);
		}

		public string Get_Buff_Icon(int buffid)
		{
			if (this.buff_icons.ContainsKey(buffid))
			{
				return this.buff_icons[buffid];
			}
			return string.Format(UT.GetText("Burn_Error_Find_Buff"), buffid);
		}

		public uint Get_CurSelected_BuffId()
		{
			return this.Get_Buffs(this.curSelect_LevelIndex)[this.curSelect_BuffIndex];
		}

		public void SetProgress(COMDT_BURNING_LEVEL_PROGRESS data)
		{
			if (data == null)
			{
				return;
			}
			this._data = data;
			this._mapDetails.Clear();
			for (int i = 0; i < (int)data.bDiffNum; i++)
			{
				COMDT_BURNING_LEVEL_DETAIL cOMDT_BURNING_LEVEL_DETAIL = data.astDiffDetail[i];
				BurnExpeditionModel.EDifficultyType bDifficultType = (BurnExpeditionModel.EDifficultyType)cOMDT_BURNING_LEVEL_DETAIL.bDifficultType;
				if (!this._mapDetails.ContainsKey((uint)bDifficultType))
				{
					this._mapDetails.Add((uint)bDifficultType, cOMDT_BURNING_LEVEL_DETAIL);
				}
				if (!this._hero_map.ContainsKey((uint)bDifficultType))
				{
					this._hero_map.Add((uint)bDifficultType, new Dictionary<uint, int>());
				}
				Dictionary<uint, int> dictionary = this._hero_map[(uint)cOMDT_BURNING_LEVEL_DETAIL.bDifficultType];
				if (dictionary != null)
				{
					for (int j = 0; j < (int)cOMDT_BURNING_LEVEL_DETAIL.stHeroDetail.wHeroNum; j++)
					{
						COMDT_BURNING_HERO_INFO cOMDT_BURNING_HERO_INFO = cOMDT_BURNING_LEVEL_DETAIL.stHeroDetail.astHeroList[j];
						if (dictionary.ContainsKey(cOMDT_BURNING_HERO_INFO.dwHeroID))
						{
							dictionary[cOMDT_BURNING_HERO_INFO.dwHeroID] = (int)cOMDT_BURNING_HERO_INFO.dwBloodTTH;
						}
						else
						{
							dictionary.Add(cOMDT_BURNING_HERO_INFO.dwHeroID, (int)cOMDT_BURNING_HERO_INFO.dwBloodTTH);
						}
					}
				}
			}
			this.CalcProgress();
		}

		public void CalcProgress()
		{
			this.lastUnlockLevelIndex = this.Get_LastUnlockLevelIndex(this.curDifficultyType);
		}

		public void Record_HeroData()
		{
			for (int i = 0; i < (int)this._data.bDiffNum; i++)
			{
				Dictionary<uint, int> dictionary = this._hero_map[(uint)i];
				COMDT_BURNING_LEVEL_DETAIL cOMDT_BURNING_LEVEL_DETAIL = this._data.astDiffDetail[i];
				for (int j = 0; j < (int)cOMDT_BURNING_LEVEL_DETAIL.stHeroDetail.wHeroNum; j++)
				{
					COMDT_BURNING_HERO_INFO cOMDT_BURNING_HERO_INFO = cOMDT_BURNING_LEVEL_DETAIL.stHeroDetail.astHeroList[j];
					if (dictionary.ContainsKey(cOMDT_BURNING_HERO_INFO.dwHeroID))
					{
						dictionary[cOMDT_BURNING_HERO_INFO.dwHeroID] = (int)cOMDT_BURNING_HERO_INFO.dwBloodTTH;
					}
					else
					{
						dictionary.Add(cOMDT_BURNING_HERO_INFO.dwHeroID, (int)cOMDT_BURNING_HERO_INFO.dwBloodTTH);
					}
				}
			}
		}

		public COMDT_BURNING_LEVEL_INFO[] Get_LevelArray(BurnExpeditionModel.EDifficultyType type)
		{
			return this._getLevelDetail(type).astLevelDetail;
		}

		public COMDT_BURNING_ENEMY_TEAM_INFO Get_CurLevel_ENEMY_TEAM_INFO()
		{
			return this.Get_ENEMY_TEAM_INFO(this.curDifficultyType, this.curSelect_LevelIndex);
		}

		public COMDT_BURNING_ENEMY_TEAM_INFO Get_ENEMY_TEAM_INFO(BurnExpeditionModel.EDifficultyType type, int levelNo)
		{
			return this._getLevelDetail(type).astLevelDetail[levelNo].stEnemyDetail;
		}

		public void Set_ENEMY_TEAM_INFO(BurnExpeditionModel.EDifficultyType type, int levelNo, COMDT_BURNING_ENEMY_TEAM_INFO info)
		{
			this._getLevelDetail(type).astLevelDetail[levelNo].stEnemyDetail = info;
		}

		public int Get_LevelID(BurnExpeditionModel.EDifficultyType type, int levelNo)
		{
			return this._getLevelDetail(type).astLevelDetail[levelNo].iLevelID;
		}

		public COM_LEVEL_STATUS Get_LevelStatus(int levelNo)
		{
			return (COM_LEVEL_STATUS)this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelNo].bLevelStatus;
		}

		public COM_LEVEL_STATUS Get_ChestRewardStatus(int levelNo)
		{
			return (COM_LEVEL_STATUS)this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelNo].bRewardStatus;
		}

		public int Get_LastUnlockLevelIndex(BurnExpeditionModel.EDifficultyType type)
		{
			COMDT_BURNING_LEVEL_INFO[] astLevelDetail = this._getLevelDetail(type).astLevelDetail;
			int num = this.Get_LevelNum(type);
			for (int i = num - 1; i >= 0; i--)
			{
				if (astLevelDetail[i].bLevelStatus == 1)
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsAllCompelte()
		{
			COMDT_BURNING_LEVEL_INFO[] astLevelDetail = this._getLevelDetail(this.curDifficultyType).astLevelDetail;
			for (int i = 0; i < astLevelDetail.Length; i++)
			{
				COMDT_BURNING_LEVEL_INFO cOMDT_BURNING_LEVEL_INFO = astLevelDetail[i];
				if (cOMDT_BURNING_LEVEL_INFO == null)
				{
					return false;
				}
				if (cOMDT_BURNING_LEVEL_INFO.bLevelStatus != 2)
				{
					return false;
				}
			}
			return true;
		}

		public List<uint> Get_Enemy_HeroIDS()
		{
			List<uint> list = new List<uint>(3);
			COMDT_PLAYERINFO current_Enemy_PlayerInfo = this.Get_Current_Enemy_PlayerInfo();
			if (current_Enemy_PlayerInfo != null)
			{
				for (int i = 0; i < current_Enemy_PlayerInfo.astChoiceHero.Length; i++)
				{
					COMDT_CHOICEHERO cOMDT_CHOICEHERO = current_Enemy_PlayerInfo.astChoiceHero[i];
					if (cOMDT_CHOICEHERO != null && cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID != 0u)
					{
						list.Add(cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID);
					}
				}
			}
			return list;
		}

		public COMDT_PLAYERINFO Get_Current_Enemy_PlayerInfo()
		{
			COMDT_BURNING_ENEMY_TEAM_INFO curLevel_ENEMY_TEAM_INFO = this.Get_CurLevel_ENEMY_TEAM_INFO();
			if (curLevel_ENEMY_TEAM_INFO == null)
			{
				return null;
			}
			COMDT_PLAYERINFO stEnemyDetail;
			if (curLevel_ENEMY_TEAM_INFO.bType == 1)
			{
				stEnemyDetail = curLevel_ENEMY_TEAM_INFO.stDetail.stRealMan.stEnemyDetail;
			}
			else
			{
				stEnemyDetail = curLevel_ENEMY_TEAM_INFO.stDetail.stRobot.stEnemyDetail;
			}
			return stEnemyDetail;
		}

		public string GetPlayerHearUrl()
		{
			COMDT_BURNING_ENEMY_TEAM_INFO curLevel_ENEMY_TEAM_INFO = this.Get_CurLevel_ENEMY_TEAM_INFO();
			if (curLevel_ENEMY_TEAM_INFO.bType == 1)
			{
				return UT.Bytes2String(curLevel_ENEMY_TEAM_INFO.stDetail.stRealMan.szHeadUrl);
			}
			return string.Empty;
		}

		public uint Get_LastPlayTime(BurnExpeditionModel.EDifficultyType type)
		{
			return this._getLevelDetail(type).dwLastPlayTime;
		}

		public int Get_ResetNum(BurnExpeditionModel.EDifficultyType type)
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(43u);
			if (dataByKey == null)
			{
				return 0;
			}
			int dwConfValue = (int)dataByKey.dwConfValue;
			if (this._getLevelDetail(type) == null)
			{
				return 0;
			}
			return dwConfValue - (int)this._getLevelDetail(type).bResetNum;
		}

		public int Get_LevelNum(BurnExpeditionModel.EDifficultyType type)
		{
			return (int)this._getLevelDetail(type).bLevelNum;
		}

		public bool Can_Reset()
		{
			return true;
		}

		public byte Get_LevelNo(int levelIndex)
		{
			return this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelIndex].bLevelNo;
		}

		public int Get_LevelID(int levelIndex)
		{
			return this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelIndex].iLevelID;
		}

		public uint[] Get_Buffs(int levelIndex)
		{
			return this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelIndex].stLuckyBuffDetail.SkillCombineID;
		}

		public bool Get_Box_Info(uint playerLevel, int levelIndex, out uint goldNum, out uint burnNum)
		{
			burnNum = 0u;
			object[] rawDatas = GameDataMgr.burnRewrad.RawDatas;
			int num = rawDatas.Length;
			for (int i = 0; i < num; i++)
			{
				ResBurningReward resBurningReward = rawDatas[i] as ResBurningReward;
				if (resBurningReward != null && resBurningReward.dwAcntLevel == playerLevel && resBurningReward.iLevelID == BurnExpeditionUT.GetLevelCFGID(levelIndex))
				{
					goldNum = resBurningReward.dwRewardCoin;
					burnNum = resBurningReward.dwBurningCoin;
					return true;
				}
			}
			goldNum = 0u;
			burnNum = 0u;
			return false;
		}

		private COMDT_BURNING_LEVEL_DETAIL _getLevelDetail(BurnExpeditionModel.EDifficultyType type)
		{
			if (this._mapDetails.ContainsKey((uint)type))
			{
				return this._mapDetails[(uint)type];
			}
			return null;
		}

		private void Set_Level_State(BurnExpeditionModel.EDifficultyType type, int levelIndex, COM_LEVEL_STATUS state)
		{
			this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelIndex].bLevelStatus = (byte)state;
		}

		private void Set_Box_State(BurnExpeditionModel.EDifficultyType type, int boxIndex, COM_LEVEL_STATUS state)
		{
			this._getLevelDetail(this.curDifficultyType).astLevelDetail[boxIndex].bRewardStatus = (byte)state;
		}
	}
}
