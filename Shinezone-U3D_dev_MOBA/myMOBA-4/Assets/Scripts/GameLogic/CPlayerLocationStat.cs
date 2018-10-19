using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class CPlayerLocationStat
	{
		protected DictionaryView<uint, List<VInt2>> StatData = new DictionaryView<uint, List<VInt2>>();

		protected bool bShouldCare;

		public void StartRecord()
		{
			this.Clear();
			if (this.ShouldStatInThisGameMode())
			{
				this.bShouldCare = true;
				this.OnStat(0);
			}
		}

		public void Clear()
		{
			if (this.ShouldStatInThisGameMode())
			{
				this.StatData.Clear();
			}
			this.bShouldCare = false;
		}

		public void UpdateLogic(int DeltaTime)
		{
			if (this.bShouldCare && Singleton<FrameSynchr>.instance.CurFrameNum % 450u == 0u && Singleton<BattleLogic>.instance.isFighting)
			{
				this.OnStat(0);
			}
		}

		public bool ShouldStatInThisGameMode()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			return curLvelContext.IsMobaModeWithOutGuide() && curLvelContext.m_pvpPlayerNum == 10;
		}

		private void OnStat(int TimeSeq)
		{
			try
			{
				List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
				List<Player>.Enumerator enumerator = allPlayers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Player current = enumerator.Current;
					if (current != null && current.Captain)
					{
						List<VInt2> list = null;
						if (!this.StatData.TryGetValue(current.PlayerId, out list))
						{
							list = new List<VInt2>();
							this.StatData.Add(current.PlayerId, list);
						}
						VInt3 location = current.Captain.handle.location;
						list.Add(new VInt2(location.x, location.z));
					}
				}
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "exception in player location stat:{0}", new object[]
				{
					ex.Message
				});
			}
		}

		public VInt2 GetTimeLocation(uint playerID, int Index)
		{
			List<VInt2> list = null;
			if (this.StatData.TryGetValue(playerID, out list) && Index < list.Count)
			{
				return list[Index];
			}
			return default(VInt2);
		}
	}
}
