using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class CCampKDAStat
	{
		private uint[] m_campTotalDamage = new uint[2];

		private uint[] m_campTotalToHeroDamage = new uint[2];

		private uint[] m_campTotalTakenDamage = new uint[2];

		private uint[] m_campTotalTakenHeroDamage = new uint[2];

		public uint camp1TotalDamage
		{
			get
			{
				return this.GetTeamTotalDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			}
		}

		public uint camp1TotalTakenDamage
		{
			get
			{
				return this.GetTeamTotalTakenDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			}
		}

		public uint camp1TotalToHeroDamage
		{
			get
			{
				return this.GetTeamTotalToHeroDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			}
		}

		public uint camp2TotalDamage
		{
			get
			{
				return this.GetTeamTotalDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			}
		}

		public uint camp2TotalTakenDamage
		{
			get
			{
				return this.GetTeamTotalTakenDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			}
		}

		public uint camp2TotalToHeroDamage
		{
			get
			{
				return this.GetTeamTotalToHeroDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			}
		}

		public void Initialize(DictionaryView<uint, PlayerKDA> playerKDAStat)
		{
			for (int i = 1; i <= 2; i++)
			{
				this.m_campTotalDamage[i - 1] = 0u;
				this.m_campTotalToHeroDamage[i - 1] = 0u;
				this.m_campTotalTakenDamage[i - 1] = 0u;
				this.m_campTotalTakenHeroDamage[i - 1] = 0u;
			}
			this.GetTeamKDA(playerKDAStat);
		}

		private void GetTeamInfoByPlayerKda(PlayerKDA kda)
		{
			if (kda == null)
			{
				return;
			}
			if (kda.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1 && kda.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return;
			}
			ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					this.m_campTotalDamage[kda.PlayerCamp - COM_PLAYERCAMP.COM_PLAYERCAMP_1] += (uint)current.hurtToEnemy;
					this.m_campTotalToHeroDamage[kda.PlayerCamp - COM_PLAYERCAMP.COM_PLAYERCAMP_1] += (uint)current.hurtToHero;
					this.m_campTotalTakenDamage[kda.PlayerCamp - COM_PLAYERCAMP.COM_PLAYERCAMP_1] += (uint)current.hurtTakenByEnemy;
					this.m_campTotalTakenHeroDamage[kda.PlayerCamp - COM_PLAYERCAMP.COM_PLAYERCAMP_1] += (uint)current.hurtTakenByHero;
				}
			}
		}

		private void GetTeamKDA(DictionaryView<uint, PlayerKDA> playerKDAStat)
		{
			if (playerKDAStat == null)
			{
				return;
			}
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.Value;
				this.GetTeamInfoByPlayerKda(value);
			}
		}

		public uint GetTeamTotalDamage(COM_PLAYERCAMP camp)
		{
			if (camp < COM_PLAYERCAMP.COM_PLAYERCAMP_1 || camp > COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return 0u;
			}
			return this.m_campTotalDamage[camp - COM_PLAYERCAMP.COM_PLAYERCAMP_1];
		}

		public uint GetTeamTotalToHeroDamage(COM_PLAYERCAMP camp)
		{
			if (camp < COM_PLAYERCAMP.COM_PLAYERCAMP_1 || camp > COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return 0u;
			}
			return this.m_campTotalToHeroDamage[camp - COM_PLAYERCAMP.COM_PLAYERCAMP_1];
		}

		public uint GetTeamTotalTakenDamage(COM_PLAYERCAMP camp)
		{
			if (camp < COM_PLAYERCAMP.COM_PLAYERCAMP_1 || camp > COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return 0u;
			}
			return this.m_campTotalTakenDamage[camp - COM_PLAYERCAMP.COM_PLAYERCAMP_1];
		}

		public uint GetTeamTotalTakenHeroDamage(COM_PLAYERCAMP camp)
		{
			if (camp < COM_PLAYERCAMP.COM_PLAYERCAMP_1 || camp > COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return 0u;
			}
			return this.m_campTotalTakenHeroDamage[camp - COM_PLAYERCAMP.COM_PLAYERCAMP_1];
		}
	}
}
