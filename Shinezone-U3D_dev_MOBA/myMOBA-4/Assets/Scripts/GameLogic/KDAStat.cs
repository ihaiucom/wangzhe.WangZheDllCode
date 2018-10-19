using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class KDAStat
	{
		public enum GET_COIN_CHANNEL_TYPE
		{
			GET_COIN_CHANNEL_TYPE_ALL,
			GET_COIN_CHANNEL_TYPE_SYSTEM,
			GET_COIN_CHANNEL_TYPE_KILLSOLIDER,
			GET_COIN_CHANNEL_TYPE_KILLMONSTER,
			GET_COIN_CHANNEL_TYPE_KILLTOWER,
			GET_COIN_CHANNEL_TYPE_KILLDRAGON,
			GET_COIN_CHANNEL_TYPE_KILLHERO,
			GET_COIN_CHANNEL_TYPE_MAX
		}

		public struct COIN_RECORD_INFO
		{
			public uint uiCount;
		}

		private const float KDA_FACTOR = 1f;

		public const int JUDGE_RECORD_NUM = 30;

		protected int m_numKill;

		protected int m_numAssist;

		protected int m_numDead;

		public COMDT_STATISTIC_POS[] m_arrDeadPosition = new COMDT_STATISTIC_POS[50];

		protected JudgeStatc m_stJudgeStat;

		protected ListValueView<JudgeStatc> m_JudgeRecords;

		protected int _totalCoin;

		protected uint uiLastRecordCoinIndex;

		public Dictionary<uint, uint>[] m_arrCoinInfos = new Dictionary<uint, uint>[7];

		protected int m_iCoinFromKillSolider;

		protected int m_iCoinFromKillMonster;

		protected int m_iCoinFromKillTower;

		protected int m_iCoinFromKillDragon;

		protected int m_iCoinFromKillHero;

		protected int m_iCoinFromSystem;

		protected int m_iCoinFromSystemNoAI;

		protected int m_numKillDragon;

		protected int m_numKillBaron;

		protected int m_numKillBlueBa;

		protected int m_numKillRedBa;

		protected int m_numKillMonster;

		protected int m_numKillFakeMonster;

		protected int m_numKillSoldier;

		protected int m_numKillOrgan;

		protected int m_numDestroyBase;

		protected int m_iHurtToOrgan;

		protected int m_hurtToEnemy;

		protected int m_Skill0HurtToEnemy;

		protected int m_hurtToHero;

		protected int m_iPhysHurtToHero;

		protected int m_iMagicHurtToHero;

		protected int m_iRealHurtToHero;

		protected int m_hurtTakenByEnemy;

		protected int m_hurtTakenByHero;

		protected int m_heal;

		protected int m_iBeHeal;

		protected int m_LegendaryNum;

		protected bool m_bKillMost;

		protected bool m_bHurtMost;

		protected bool m_bHurtTakenMost;

		protected bool m_bAsssistMost;

		protected int m_DoubleKillNum;

		protected int m_TripleKillNum;

		protected int m_QuataryKillNum;

		protected int m_PentaKillNum;

		protected bool m_bGetCoinMost;

		protected bool m_bKillOrganMost;

		protected bool m_bHurtToHeroMost;

		protected uint m_uiHurtToEnemyNum;

		protected uint m_uiHurtToHeroNum;

		protected uint m_uiHealNum;

		protected uint m_uiBeAttackedNum;

		protected uint m_uiBeChosenAsAttackTargetNum;

		protected uint m_uiBeChosenAsHealTargetNum;

		protected int m_BeHurtMax;

		protected int m_BeHurtMin = -1;

		protected int m_BeHealMax;

		protected int m_BeHealMin = -1;

		protected int m_iSelfHealNum;

		protected int m_iSelfHealMax;

		protected int m_iSelfHealMin = -1;

		protected int m_iSelfHealCount;

		protected uint m_iKillHeroUnderTenPercent;

		protected uint m_iEquipeHurtValue;

		public uint EquipeHurtValue
		{
			get
			{
				return this.m_iEquipeHurtValue;
			}
		}

		public int CountSelfHealMax
		{
			get
			{
				return this.m_iSelfHealMax;
			}
		}

		public int CountSelfHealMin
		{
			get
			{
				return this.m_iSelfHealMin;
			}
		}

		public int NumSelfHeal
		{
			get
			{
				return this.m_iSelfHealNum;
			}
		}

		public int TotalCountSelfHeal
		{
			get
			{
				return this.m_iSelfHealCount;
			}
		}

		public int CountBeHurtMax
		{
			get
			{
				return this.m_BeHurtMax;
			}
		}

		public int CountBeHurtMin
		{
			get
			{
				return this.m_BeHurtMin;
			}
		}

		public int CountBeHealMax
		{
			get
			{
				return this.m_BeHealMax;
			}
		}

		public int CountBeHealMin
		{
			get
			{
				return this.m_BeHealMin;
			}
		}

		public int numKill
		{
			get
			{
				return this.m_numKill;
			}
		}

		public int numAssist
		{
			get
			{
				return this.m_numAssist;
			}
		}

		public int numDead
		{
			get
			{
				return this.m_numDead;
			}
		}

		public int TotalCoin
		{
			get
			{
				return this._totalCoin;
			}
		}

		public int CoinFromKillSolider
		{
			get
			{
				return this.m_iCoinFromKillSolider;
			}
		}

		public int CoinFromKillMonster
		{
			get
			{
				return this.m_iCoinFromKillMonster;
			}
		}

		public int CoinFromKillTower
		{
			get
			{
				return this.m_iCoinFromKillTower;
			}
		}

		public int CoinFromKillDragon
		{
			get
			{
				return this.m_iCoinFromKillDragon;
			}
		}

		public int CoinFromKillHero
		{
			get
			{
				return this.m_iCoinFromKillHero;
			}
		}

		public int CoinFromSystem
		{
			get
			{
				return this.m_iCoinFromSystem;
			}
		}

		public int numKillMonster
		{
			get
			{
				return this.m_numKillMonster;
			}
		}

		public int numKillFakeMonster
		{
			get
			{
				return this.m_numKillFakeMonster;
			}
		}

		public int numKillSoldier
		{
			get
			{
				return this.m_numKillSoldier;
			}
		}

		public int numKillOrgan
		{
			get
			{
				return this.m_numKillOrgan;
			}
		}

		public int numDestroyBase
		{
			get
			{
				return this.m_numDestroyBase;
			}
		}

		public int numKillDragon
		{
			get
			{
				return this.m_numKillDragon;
			}
		}

		public int numKillBaron
		{
			get
			{
				return this.m_numKillBaron;
			}
		}

		public int numKillBlueBa
		{
			get
			{
				return this.m_numKillBlueBa;
			}
		}

		public int numKillRedBa
		{
			get
			{
				return this.m_numKillRedBa;
			}
		}

		public int hurtToEnemy
		{
			get
			{
				return this.m_hurtToEnemy;
			}
		}

		public int Skill0HurtToEnemy
		{
			get
			{
				return this.m_Skill0HurtToEnemy;
			}
		}

		public int hurtToHero
		{
			get
			{
				return this.m_hurtToHero + (int)this.GetBeShieldProtectedValueToHeroPhys() + (int)this.GetBeShieldProtectedValueToHeroMagic() + (int)this.GetBeShieldProtectedValueToHeroReal();
			}
		}

		public int physHurtToHero
		{
			get
			{
				return this.m_iPhysHurtToHero + (int)this.GetBeShieldProtectedValueToHeroPhys();
			}
		}

		public int magicHurtToHero
		{
			get
			{
				return this.m_iMagicHurtToHero + (int)this.GetBeShieldProtectedValueToHeroMagic();
			}
		}

		public int realHurtToHero
		{
			get
			{
				return this.m_iRealHurtToHero + (int)this.GetBeShieldProtectedValueToHeroReal();
			}
		}

		public int hurtTakenByEnemy
		{
			get
			{
				return this.m_hurtTakenByEnemy + this.GetTotalShieldProtectedValue();
			}
		}

		public int hurtToOrgan
		{
			get
			{
				return this.m_iHurtToOrgan;
			}
		}

		public int hurtTakenByHero
		{
			get
			{
				return this.m_hurtTakenByHero + (int)this.GetShieldProtectedValueFormHero();
			}
		}

		public int heal
		{
			get
			{
				return this.m_heal;
			}
		}

		public int beHeal
		{
			get
			{
				return this.m_iBeHeal;
			}
		}

		public int LegendaryNum
		{
			get
			{
				return this.m_LegendaryNum;
			}
		}

		public int DoubleKillNum
		{
			get
			{
				return this.m_DoubleKillNum;
			}
		}

		public int TripleKillNum
		{
			get
			{
				return this.m_TripleKillNum;
			}
		}

		public int QuataryKillNum
		{
			get
			{
				return this.m_QuataryKillNum;
			}
		}

		public int PentaKillNum
		{
			get
			{
				return this.m_PentaKillNum;
			}
		}

		public float KDAValue
		{
			get
			{
				return (float)(this.numKill + this.numAssist) * 1f / Math.Max((float)this.numDead, 1f);
			}
		}

		public bool bKillMost
		{
			get
			{
				return this.m_bKillMost;
			}
			set
			{
				this.m_bKillMost = value;
			}
		}

		public bool bHurtMost
		{
			get
			{
				return this.m_bHurtMost;
			}
			set
			{
				this.m_bHurtMost = value;
			}
		}

		public bool bHurtTakenMost
		{
			get
			{
				return this.m_bHurtTakenMost;
			}
			set
			{
				this.m_bHurtTakenMost = value;
			}
		}

		public bool bAsssistMost
		{
			get
			{
				return this.m_bAsssistMost;
			}
			set
			{
				this.m_bAsssistMost = value;
			}
		}

		public bool bGetCoinMost
		{
			get
			{
				return this.m_bGetCoinMost;
			}
			set
			{
				this.m_bGetCoinMost = value;
			}
		}

		public bool bKillOrganMost
		{
			get
			{
				return this.m_bKillOrganMost;
			}
			set
			{
				this.m_bKillOrganMost = value;
			}
		}

		public bool bHurtToHeroMost
		{
			get
			{
				return this.m_bHurtToHeroMost;
			}
			set
			{
				this.m_bHurtToHeroMost = value;
			}
		}

		public uint HurtToEnemyNum
		{
			get
			{
				return this.m_uiHurtToEnemyNum;
			}
		}

		public uint HurtToHeroNum
		{
			get
			{
				return this.m_uiHurtToHeroNum;
			}
		}

		public uint TotalBeHealNum
		{
			get
			{
				return this.m_uiHealNum;
			}
		}

		public uint TotalBeAttackNum
		{
			get
			{
				return this.m_uiBeAttackedNum;
			}
		}

		public uint TotalBeChosenAsAttackTargetNum
		{
			get
			{
				return this.m_uiBeChosenAsAttackTargetNum;
			}
		}

		public uint TotalBeChosenAsHealTargetNum
		{
			get
			{
				return this.m_uiBeChosenAsHealTargetNum;
			}
		}

		public uint NumKillUnderTenPercent
		{
			get
			{
				return this.m_iKillHeroUnderTenPercent;
			}
		}

		public JudgeStatc JudgeStat
		{
			get
			{
				return this.m_stJudgeStat;
			}
		}

		protected virtual int GetTotalShieldProtectedValue()
		{
			return 0;
		}

		protected virtual uint GetShieldProtectedValueFormHero()
		{
			return 0u;
		}

		protected virtual uint GetBeShieldProtectedValueFormHero()
		{
			return 0u;
		}

		protected virtual uint GetBeShieldProtectedValueToHeroPhys()
		{
			return 0u;
		}

		protected virtual uint GetBeShieldProtectedValueToHeroMagic()
		{
			return 0u;
		}

		protected virtual uint GetBeShieldProtectedValueToHeroReal()
		{
			return 0u;
		}

		public bool GetJudgeStatcByIndex(int i, ref JudgeStatc js)
		{
			if (i >= 0 && i < this.m_JudgeRecords.Count)
			{
				js = this.m_JudgeRecords[i];
				return true;
			}
			return false;
		}

		protected void recordDead(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			if (!atker)
			{
				return;
			}
			this.m_numDead++;
			if (src && !src.handle.ActorAgent.IsAutoAI())
			{
				this.m_stJudgeStat.DeadNum = (ushort)(this.m_stJudgeStat.DeadNum + (ushort)1);
			}
			if (src && this.m_arrDeadPosition != null && this.m_numDead <= 50)
			{
				if (this.m_arrDeadPosition[this.m_numDead - 1] == null)
				{
					this.m_arrDeadPosition[this.m_numDead - 1] = new COMDT_STATISTIC_POS();
				}
				if (this.m_arrDeadPosition[this.m_numDead - 1] != null)
				{
					this.m_arrDeadPosition[this.m_numDead - 1].iTime = (int)(Singleton<FrameSynchr>.instance.LogicFrameTick / 1000uL);
					this.m_arrDeadPosition[this.m_numDead - 1].iXPos = src.handle.location.x;
					this.m_arrDeadPosition[this.m_numDead - 1].iZPos = src.handle.location.z;
				}
			}
		}

		protected void recordAssist(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, PoolObjHandle<ActorRoot> self, PoolObjHandle<ActorRoot> killer)
		{
			DebugHelper.Assert(src && src.handle.ActorControl != null, "invalid source data.");
			if (src && src.handle.ActorControl != null)
			{
				List<KeyValuePair<uint, ulong>>.Enumerator enumerator = src.handle.ActorControl.hurtSelfActorList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (!atker)
					{
						goto IL_8D;
					}
					KeyValuePair<uint, ulong> current = enumerator.Current;
					if (current.Key != atker.handle.ObjID)
					{
						goto IL_8D;
					}
					continue;
					IL_8D:
					if (killer)
					{
						KeyValuePair<uint, ulong> current2 = enumerator.Current;
						if (current2.Key == killer.handle.ObjID)
						{
							continue;
						}
					}
					KeyValuePair<uint, ulong> current3 = enumerator.Current;
					if (current3.Key == self.handle.ObjID)
					{
						this.m_numAssist++;
						if (src && !src.handle.ActorAgent.IsAutoAI())
						{
							this.m_stJudgeStat.AssitNum = (ushort)(this.m_stJudgeStat.AssitNum + (ushort)1);
						}
						return;
					}
				}
			}
			if (killer && killer.handle.ActorControl != null)
			{
				List<KeyValuePair<uint, ulong>>.Enumerator enumerator2 = killer.handle.ActorControl.helpSelfActorList.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (!atker)
					{
						goto IL_194;
					}
					KeyValuePair<uint, ulong> current4 = enumerator2.Current;
					if (current4.Key != atker.handle.ObjID)
					{
						goto IL_194;
					}
					continue;
					IL_194:
					KeyValuePair<uint, ulong> current5 = enumerator2.Current;
					if (current5.Key != killer.handle.ObjID)
					{
						KeyValuePair<uint, ulong> current6 = enumerator2.Current;
						if (current6.Key == self.handle.ObjID)
						{
							this.m_numAssist++;
							if (src && !src.handle.ActorAgent.IsAutoAI())
							{
								this.m_stJudgeStat.AssitNum = (ushort)(this.m_stJudgeStat.AssitNum + (ushort)1);
							}
							return;
						}
					}
				}
			}
		}
	}
}
