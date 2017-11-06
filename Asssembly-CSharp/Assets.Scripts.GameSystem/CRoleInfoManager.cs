using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CRoleInfoManager : Singleton<CRoleInfoManager>, IUpdateLogic
	{
		private CRoleInfoContainer s_roleInfoContainer;

		public ulong masterUUID
		{
			get;
			private set;
		}

		public bool hasMasterUUID
		{
			get
			{
				return this.masterUUID != 0uL;
			}
		}

		public override void Init()
		{
			this.s_roleInfoContainer = new CRoleInfoContainer();
			this.masterUUID = 0uL;
		}

		public void Clean()
		{
			this.s_roleInfoContainer.Clear();
		}

		public void SetMaterUUID(ulong InMaterUUID)
		{
			this.masterUUID = InMaterUUID;
		}

		public CRoleInfo CreateRoleInfo(enROLEINFO_TYPE type, ulong uuID, int logicWorldID = 0)
		{
			ulong uuID2 = this.s_roleInfoContainer.AddRoleInfoByType(type, uuID, logicWorldID);
			return this.GetRoleInfoByUUID(uuID2);
		}

		public CRoleInfo GetMasterRoleInfo()
		{
			return this.GetRoleInfoByUUID(this.masterUUID);
		}

		public void ClearMasterRoleInfo()
		{
			CRoleInfo cRoleInfo = this.hasMasterUUID ? this.GetMasterRoleInfo() : null;
			if (cRoleInfo != null)
			{
				cRoleInfo.Clear();
			}
		}

		public CRoleInfo GetRoleInfoByUUID(ulong uuID)
		{
			return this.s_roleInfoContainer.FindRoleInfoByID(uuID);
		}

		public void UpdateLogic(int delta)
		{
			if (this.hasMasterUUID)
			{
				CRoleInfo masterRoleInfo = this.GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					masterRoleInfo.UpdateLogic(delta);
				}
			}
		}

		public void CalculateWins(COMDT_PVPBATTLE_INFO battleInfo, int bGameResult)
		{
			battleInfo.dwTotalNum += 1u;
			if (bGameResult == 1)
			{
				battleInfo.dwWinNum += 1u;
			}
		}

		public void CalculateKDA(COMDT_GAME_INFO gameInfo)
		{
			CRoleInfo masterRoleInfo = this.GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "masterRoleInfo is null");
			if (masterRoleInfo == null)
			{
				return;
			}
			PlayerKDA hostKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetHostKDA();
			if (hostKDA != null)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
						num += enumerator.Current.LegendaryNum;
						num2 += enumerator.Current.PentaKillNum;
						num3 += enumerator.Current.QuataryKillNum;
						num4 += enumerator.Current.TripleKillNum;
						num5 += enumerator.Current.DoubleKillNum;
						num8 += (enumerator.Current.bHurtMost ? 1 : 0);
						num9 += (enumerator.Current.bHurtTakenMost ? 1 : 0);
						num10 += (enumerator.Current.bGetCoinMost ? 1 : 0);
						num11 += (enumerator.Current.bAsssistMost ? 1 : 0);
						num12 += (enumerator.Current.bKillMost ? 1 : 0);
					}
				}
				if (gameInfo.bGameResult == 1)
				{
					uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(hostKDA.PlayerCamp, true);
					if (mvpPlayer != 0u)
					{
						num6 = ((mvpPlayer == hostKDA.PlayerId) ? 1 : 0);
					}
				}
				else if (gameInfo.bGameResult == 2)
				{
					uint mvpPlayer2 = Singleton<BattleStatistic>.instance.GetMvpPlayer(hostKDA.PlayerCamp, false);
					if (mvpPlayer2 != 0u)
					{
						num7 = ((mvpPlayer2 == hostKDA.PlayerId) ? 1 : 0);
					}
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				bool flag8 = false;
				bool flag9 = false;
				bool flag10 = false;
				bool flag11 = false;
				bool flag12 = false;
				int num13 = 0;
				ListView<COMDT_STATISTIC_KEY_VALUE_INFO> listView = new ListView<COMDT_STATISTIC_KEY_VALUE_INFO>();
				while ((long)num13 < (long)((ulong)masterRoleInfo.pvpDetail.stKVDetail.dwNum))
				{
					COMDT_STATISTIC_KEY_VALUE_INFO cOMDT_STATISTIC_KEY_VALUE_INFO = masterRoleInfo.pvpDetail.stKVDetail.astKVDetail[num13];
					switch (cOMDT_STATISTIC_KEY_VALUE_INFO.dwKey)
					{
					case 13u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num6;
						flag6 = true;
						break;
					case 14u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num7;
						flag7 = true;
						break;
					case 15u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num;
						flag5 = true;
						break;
					case 16u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num5;
						flag = true;
						break;
					case 17u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num4;
						flag2 = true;
						break;
					case 27u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num3;
						flag3 = true;
						break;
					case 28u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num2;
						flag4 = true;
						break;
					case 29u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num8;
						flag8 = true;
						break;
					case 30u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num10;
						flag10 = true;
						break;
					case 31u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num9;
						flag9 = true;
						break;
					case 32u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num11;
						flag11 = true;
						break;
					case 33u:
						cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue += (uint)num12;
						flag12 = true;
						break;
					}
					num13++;
				}
				if (!flag)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 16u,
						dwValue = (uint)num5
					});
				}
				if (!flag2)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 17u,
						dwValue = (uint)num4
					});
				}
				if (!flag3)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 27u,
						dwValue = (uint)num3
					});
				}
				if (!flag4)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 28u,
						dwValue = (uint)num2
					});
				}
				if (!flag5)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 15u,
						dwValue = (uint)num
					});
				}
				if (!flag6)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 13u,
						dwValue = (uint)num6
					});
				}
				if (!flag7)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 14u,
						dwValue = (uint)num7
					});
				}
				if (!flag8)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 29u,
						dwValue = (uint)num8
					});
				}
				if (!flag9)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 31u,
						dwValue = (uint)num9
					});
				}
				if (!flag10)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 30u,
						dwValue = (uint)num10
					});
				}
				if (!flag11)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 32u,
						dwValue = (uint)num11
					});
				}
				if (!flag12)
				{
					listView.Add(new COMDT_STATISTIC_KEY_VALUE_INFO
					{
						dwKey = 33u,
						dwValue = (uint)num12
					});
				}
				if (listView.Count > 0)
				{
					masterRoleInfo.pvpDetail.stKVDetail.dwNum += (uint)listView.Count;
					listView.AddRange(masterRoleInfo.pvpDetail.stKVDetail.astKVDetail);
					masterRoleInfo.pvpDetail.stKVDetail.astKVDetail = LinqS.ToArray<COMDT_STATISTIC_KEY_VALUE_INFO>(listView);
				}
			}
		}

		public void InsertHonorOnDuplicateUpdate(ref Dictionary<int, COMDT_HONORINFO> dic, int type, int defaultPoint = 1)
		{
			COMDT_HONORINFO cOMDT_HONORINFO = new COMDT_HONORINFO();
			if (!dic.ContainsKey(type))
			{
				cOMDT_HONORINFO.iHonorID = type;
				cOMDT_HONORINFO.iHonorLevel = 0;
				cOMDT_HONORINFO.iHonorPoint = defaultPoint;
				this.JudgeHonorLevelUp(cOMDT_HONORINFO);
				dic.Add(type, cOMDT_HONORINFO);
			}
			else if (dic.TryGetValue(type, ref cOMDT_HONORINFO))
			{
				cOMDT_HONORINFO.iHonorPoint = defaultPoint;
				this.JudgeHonorLevelUp(cOMDT_HONORINFO);
				dic.set_Item(type, cOMDT_HONORINFO);
			}
		}

		private void JudgeHonorLevelUp(COMDT_HONORINFO honorInfo)
		{
			int iHonorID = honorInfo.iHonorID;
			int iHonorPoint = honorInfo.iHonorPoint;
			int num = -1;
			ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long)iHonorID);
			if (dataByKey != null)
			{
				for (int i = 0; i < dataByKey.astHonorLevel.Length; i++)
				{
					if (iHonorPoint < dataByKey.astHonorLevel[i].iMaxPoint)
					{
						break;
					}
					num++;
				}
				if (num >= dataByKey.astHonorLevel.Length)
				{
					num--;
				}
			}
			honorInfo.iHonorLevel = num;
		}

		public uint GetSelfRankClass()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				return (uint)((byte)Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankClass);
			}
			return 0u;
		}
	}
}
