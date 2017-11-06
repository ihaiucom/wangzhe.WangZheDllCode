using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.DataCenter
{
	public class ActorDataCenter : Singleton<ActorDataCenter>
	{
		private readonly DictionaryView<uint, IGameActorDataProvider> _providers = new DictionaryView<uint, IGameActorDataProvider>();

		private ActorServerDataProvider _serverDataProvider;

		public IGameActorDataProvider GetActorDataProvider(GameActorDataProviderType providerType)
		{
			IGameActorDataProvider result = null;
			this._providers.TryGetValue((uint)providerType, out result);
			return result;
		}

		public override void Init()
		{
			base.Init();
			this._serverDataProvider = new ActorServerDataProvider();
			this._providers.Add(1u, new ActorStaticLobbyDataProvider());
			this._providers.Add(2u, new ActorStaticBattleDataProvider());
			this._providers.Add(3u, this._serverDataProvider);
			DictionaryView<uint, IGameActorDataProvider>.Enumerator enumerator = this._providers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, IGameActorDataProvider> current = enumerator.Current;
				ActorDataProviderBase actorDataProviderBase = current.get_Value() as ActorDataProviderBase;
				if (actorDataProviderBase != null)
				{
					actorDataProviderBase.Init();
				}
			}
		}

		public void AddHeroServerInfo(uint playerId, COMDT_CHOICEHERO serverHeroInfo)
		{
			this._serverDataProvider.AddHeroServerInfo(playerId, serverHeroInfo);
		}

		public void AddHeroesServerData(uint playerId, COMDT_CHOICEHERO[] serverHeroInfos)
		{
			for (int i = 0; i < serverHeroInfos.Length; i++)
			{
				this.AddHeroServerInfo(playerId, serverHeroInfos[i]);
			}
		}

		public void ClearHeroServerData()
		{
			this._serverDataProvider.ClearHeroServerInfo();
		}
	}
}
