using System;
using System.Collections.Generic;
using NUnit.Framework;
using Photon.Hive;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class BasicTestsPlugin : PluginBase
    {
        private bool calledSetup;

        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public int CallsCount { get; private set; }

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            this.calledSetup = true;
            this.UseStrictMode = true;
            return base.SetupInstance(host, config, out errorMsg);
        }

        private void CheckBaseInfo<T>(ITypedCallInfo<T> info) where T : IOperationRequest
        {
            Assert.IsNotNull(info);

            Assert.IsTrue(this.calledSetup);

            Assert.IsNotNull(info.OperationRequest);
            Assert.IsNotNull(info.Request);
            if (!(info is IBeforeCloseGameCallInfo || info is ICloseGameCallInfo))
            {
                Assert.IsNotNull(info.Request.Parameters);
                Assert.AreNotEqual(0, info.Request.OperationCode);
                Assert.IsNotNull(info.UserId);
                Assert.IsNotNull(info.Nickname);
#if PLUGINS_0_9
                Assert.IsNotNull(info.Username);
#endif				
            }
            else
            {
                Assert.IsNull(info.UserId);
                Assert.IsNull(info.Nickname);
#if PLUGINS_0_9
                Assert.IsNotNull(info.Username);
#endif				
                Assert.IsNull(info.Request.Parameters);
                Assert.AreEqual(0, info.Request.OperationCode);
            }

            //TBD
            //Assert.IsNotNull(info.AuthResultsSecure);
        }

        private void CheckICreateGameCallInfo(ICreateGameCallInfo info)
        {
            this.CheckBaseInfo(info);
            Console.Write(info.CreateIfNotExists);
            Console.WriteLine(info.IsJoin);

            Assert.IsNotNull(info.CreateOptions);

            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.MaxPlayers.ToString()));

            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.IsOpen.ToString()));

            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.IsVisible.ToString()));

            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.LobbyId.ToString()));
            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.LobbyType.ToString()));

            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.CustomProperties.ToString()));

            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.EmptyRoomTTL.ToString()));
            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.PlayerTTL.ToString()));
            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.CheckUserOnJoin.ToString()));
            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.DeleteCacheOnLeave.ToString()));
            Assert.IsTrue(info.CreateOptions.ContainsKey(HiveHostGameState.SuppressRoomEvents.ToString()));

            Assert.AreEqual(0, info.Request.ActorNr);
            Assert.IsNotNull(info.Request.ActorProperties);
            Assert.IsNotNull(info.Request.GameProperties);
            Assert.IsNotNull(info.Request.GameId);
            Assert.AreEqual("Default", info.Request.LobbyName);
            Assert.AreEqual(0, info.Request.LobbyType);
            Assert.AreEqual(0, info.Request.EmptyRoomLiveTime);
            Assert.IsFalse(info.Request.CreateIfNotExists);
            Assert.IsFalse(info.Request.BroadcastActorProperties);
            Assert.IsFalse(info.Request.DeleteCacheOnLeave);
            Assert.IsFalse(info.Request.SuppressRoomEvents);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            ++this.CallsCount;
            try
            {
                this.CheckICreateGameCallInfo(info);
                this.CheckBeforeCreateGame();
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }

            try
            {
                base.OnCreateGame(info);
                this.CheckAfterCreateGame();
            }
            catch (Exception e)
            {
                var msg = e.ToString();
                this.PluginHost.BroadcastErrorInfoEvent(msg, info);
            }
        }

        private void CheckBeforeCreateGame()
        {
            Assert.IsNotNullOrEmpty("TestGame", this.PluginHost.GameId);
            Assert.AreEqual(3, this.PluginHost.GameProperties.Count);
            Assert.AreEqual(0, this.PluginHost.GameActors.Count);
            Assert.IsTrue(this.PluginHost.Environment.ContainsKey("AppId"));
            Assert.IsTrue(this.PluginHost.Environment.ContainsKey("AppVersion"));
            Assert.IsTrue(this.PluginHost.Environment.ContainsKey("Region"));
            Assert.IsTrue(this.PluginHost.Environment.ContainsKey("Cloud"));
        }

        private void CheckAfterCreateGame()
        {
            Assert.AreEqual(7, this.PluginHost.GameProperties.Count);
            Assert.AreEqual("GamePropertyValue", this.PluginHost.GameProperties["GameProperty"]);
            Assert.AreEqual("GamePropertyValue2", this.PluginHost.GameProperties["GameProperty2"]);
            Assert.AreEqual(1, this.PluginHost.GameActors.Count);
        }

        private void CheckIBeforeJoinGameCallInfo(IBeforeJoinGameCallInfo info)
        {
            this.CheckBaseInfo(info);
            CheckIJoinGameRequest(info.Request);
        }

        public override void BeforeJoin(IBeforeJoinGameCallInfo info)
        {
            ++this.CallsCount;

            try
            {
                this.CheckIBeforeJoinGameCallInfo(info);
                this.CheckBeforeJoinPreContinue();
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }

            try
            {
                base.BeforeJoin(info);
                this.CheckBeforeJoinPostContinue();
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        private void CheckBeforeJoinPreContinue()
        {
            Assert.AreEqual(1, this.PluginHost.GameActors.Count);
        }

        private void CheckBeforeJoinPostContinue()
        {
            Assert.AreEqual(2, this.PluginHost.GameActors.Count);

            var actor2 = this.PluginHost.GameActors[1];
            Assert.AreEqual(1, actor2.Properties.Count);
            Assert.AreEqual("Actor2PropertyValue", actor2.Properties.GetProperty("Actor2Property").Value);
        }

        private void CheckIJoinGameCallInfo(IJoinGameCallInfo info)
        {
            this.CheckBaseInfo(info);

            Assert.AreNotEqual(0, info.ActorNr);
            Assert.IsNotNull(info.JoinParams);
            Console.Write(info.JoinParams.PublishCache);
            Console.Write(info.JoinParams.PublishJoinEvents);
            Console.Write(info.JoinParams.ResponseExtraParameters);

            CheckIJoinGameRequest(info.Request);
        }

        private static void CheckIJoinGameRequest(IJoinGameRequest request)
        {
            Assert.IsNotNull(request.ActorProperties);
            Assert.IsNotNull(request.GameProperties);
            Assert.IsNotNull(request.LobbyName);
            Assert.IsFalse(request.BroadcastActorProperties);
            Assert.IsFalse(request.CreateIfNotExists);
            Assert.IsFalse(request.DeleteCacheOnLeave);
            Assert.IsFalse(request.SuppressRoomEvents);
            Assert.AreEqual(0, request.EmptyRoomLiveTime);
            Assert.AreEqual(0, request.LobbyType);
            Assert.IsNotNullOrEmpty(request.GameId);
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            ++this.CallsCount;

            try
            {
                this.CheckIJoinGameCallInfo(info);
                this.CheckBeforeOnJoin();
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }

            try
            {
                base.OnJoin(info);
                this.CheckAfterOnJoin();
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        private void CheckBeforeOnJoin()
        {
            Assert.AreEqual(2, this.PluginHost.GameActors.Count);
        }

        private void CheckAfterOnJoin()
        {
            Assert.AreEqual(2, this.PluginHost.GameActors.Count);
        }

        private void CheckIBeforeSetPropertiesCallInfo(IBeforeSetPropertiesCallInfo info)
        {
            this.CheckBaseInfo(info);
            Assert.AreEqual(1, info.ActorNr);
            CheckISetPropertiesRequest(info.Request);
        }

        private static void CheckISetPropertiesRequest(ISetPropertiesRequest request)
        {
            if (request.ActorNumber != 0)
            {
                Assert.AreEqual(1, request.ActorNumber);
            }
            Assert.IsNotNull(request.Properties);
            Assert.IsNull(request.ExpectedValues);
            Assert.IsFalse(request.Broadcast);
            Assert.IsFalse(request.HttpForward);
        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            ++this.CallsCount;

            try
            {
                this.CheckIBeforeSetPropertiesCallInfo(info);
                this.CheckBeforeBeforeSetProperties();
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }

            try
            {
                base.BeforeSetProperties(info);
                this.CheckAfterBeforeSetProperties();
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        private void CheckBeforeBeforeSetProperties()
        {
            if (this.PluginHost.GameActors.Count == 2)
            {
                var actor1 = this.PluginHost.GameActors[0];
                Assert.AreEqual(1, actor1.Properties.Count);
            }
        }

        private void CheckAfterBeforeSetProperties()
        {
            if (this.PluginHost.GameActors.Count == 2)
            {
                var actor1 = this.PluginHost.GameActors[0];
                Assert.AreEqual(2, actor1.Properties.Count);
            }
        }

        private void CheckISetPropertiesCallInfo(ISetPropertiesCallInfo info)
        {
            this.CheckBaseInfo(info);
            Console.Write(info.ActorNr);
            CheckISetPropertiesRequest(info.Request);
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            ++this.CallsCount;

            try
            {
                this.CheckISetPropertiesCallInfo(info);
                this.CheckBeforeOnSetProperties();
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }
            try
            {
                base.OnSetProperties(info);
                this.CheckAfterOnSetProperties();
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        private void CheckBeforeOnSetProperties()
        {
            if (this.PluginHost.GameActors.Count == 2)
            {
                var actor1 = this.PluginHost.GameActors[0];
                Assert.AreEqual(2, actor1.Properties.Count);
            }
        }

        private void CheckAfterOnSetProperties()
        {
            if (this.PluginHost.GameActors.Count == 2)
            {
                var actor1 = this.PluginHost.GameActors[0];
                Assert.AreEqual(2, actor1.Properties.Count);
            }
        }

        private void CheckIRaiseEventCallInfo(IRaiseEventCallInfo info)
        {
            this.CheckBaseInfo(info);
            Console.Write(info.ActorNr);
            if (info.Request.EvCode == 5)
            {
                Assert.AreNotEqual(0, info.Request.Cache);
                Assert.IsNotNull(info.Request.Actors);
                Assert.IsNotNull(info.Request.Data);
                Assert.IsNotNullOrEmpty(info.Request.GameId);
                Assert.AreEqual(123, info.Request.Group);
                Assert.AreEqual(123, info.Request.ReceiverGroup);
                Assert.IsTrue(info.Request.HttpForward);
                Assert.AreEqual(123, info.Request.CacheSliceIndex);
            }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            ++this.CallsCount;

            try
            {
                this.CheckIRaiseEventCallInfo(info);
                this.CheckBeforeOnRaiseEvent(info);
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }

            try
            {
                base.OnRaiseEvent(info);
                this.CheckAfterOnRaiseEvent(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        private void CheckBeforeOnRaiseEvent(IRaiseEventCallInfo info)
        {
            if (info.Request.EvCode == 1)
            {
                info.Request.Cache = (byte)CacheOperation.DoNotCache;
            }
            else if (info.Request.EvCode == 3)
            {
                info.Request.Cache = (byte) CacheOperation.SliceIncreaseIndex;
            }
        }

        private void CheckAfterOnRaiseEvent(IRaiseEventCallInfo info)
        {
            var game = (HiveGame) this.PluginHost;

            if (info.Request.EvCode == 1)
            {
                Assert.AreEqual(0, game.EventCache.GetSliceSize(0));
            }
            else if (info.Request.EvCode == 2)
            {
                Assert.AreEqual(1, game.EventCache.GetSliceSize(0));
            }
            else if (info.Request.EvCode == 3)
            {
                Assert.AreEqual(2, game.EventCache.Count);
                Assert.AreEqual(1, game.EventCache.GetSliceSize(0));
                Assert.AreEqual(0, game.EventCache.GetSliceSize(1));
            }
            else if (info.Request.EvCode == 4)
            {
                Assert.AreEqual(1, game.EventCache.GetSliceSize(1));
            }
        }

        private bool firstOnLeave = true;
        private void CheckILeaveGameCallInfo(ILeaveGameCallInfo info)
        {
            this.CheckBaseInfo(info);
            Assert.AreNotEqual(0, info.ActorNr);
            Console.Write(info.Details);
            if (this.firstOnLeave)
            {
                Assert.IsTrue(info.IsInactive);
                Assert.AreNotEqual(0, info.Reason);
                Assert.IsTrue(info.Request.IsCommingBack);
                this.firstOnLeave = false;
            }
            else
            {
                Assert.IsFalse(info.IsInactive);
                Assert.AreEqual(0, info.Reason);
                Assert.IsFalse(info.Request.IsCommingBack);
            }
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            ++this.CallsCount;

            try
            {
                this.CheckILeaveGameCallInfo(info);
                this.CheckBeforeOnLeave(info);
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }
            try
            {
                base.OnLeave(info);
                this.CheckAfterOnLeave(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        private void CheckBeforeOnLeave(ILeaveGameCallInfo info)
        {
            if (info.ActorNr == 2)
            {
                Assert.AreEqual(2, this.PluginHost.GameActors.Count);
            }
            else if (info.ActorNr == 1)
            {
                Assert.AreEqual(1, this.PluginHost.GameActors.Count);
            }
            else
            {
                Assert.Fail("Unexpected execution path");
            }
        }

        private void CheckAfterOnLeave(ILeaveGameCallInfo info)
        {
            if (info.ActorNr == 2)
            {
                Assert.AreEqual(1, this.PluginHost.GameActors.Count);
                Assert.AreNotEqual(info.ActorNr, this.PluginHost.GameActors[0].ActorNr);
            }
            else if (info.ActorNr == 1)
            {
                Assert.AreEqual(0, this.PluginHost.GameActors.Count);
            }
            else
            {
                Assert.Fail("Unexpected execution path");
            }
        }

        private void CheckIBeforeCloseGameCallInfo(IBeforeCloseGameCallInfo info)
        {
            this.CheckBaseInfo(info);
            Assert.AreEqual(0, info.Request.EmptyRoomTTL);
        }

        public override void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            ++this.CallsCount;

            try
            {
                this.CheckIBeforeCloseGameCallInfo(info);
                this.CheckBeforeBeforeCloseGame();
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }

            try
            {
                base.BeforeCloseGame(info);
                this.CheckAfterBeforeCloseGame();
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        private void CheckBeforeBeforeCloseGame()
        {
            Assert.AreEqual(0, this.PluginHost.GameActors.Count);
        }

        private void CheckAfterBeforeCloseGame()
        {
            
        }

        private void CheckICloseGameCallInfo(ICloseGameCallInfo info)
        {
            this.CheckBaseInfo(info);
            Assert.AreEqual(0, info.ActorCount);
            Assert.AreEqual(0, info.Request.EmptyRoomTTL);
        }

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            ++this.CallsCount;

            try
            {
                this.CheckICloseGameCallInfo(info);
                this.CheckBeforeOnCloseGame();
            }
            catch (Exception e)
            {
                info.Fail(e.ToString());
                return;
            }
            try
            {
                base.OnCloseGame(info);
                this.CheckAfterOnCloseGame();
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        private void CheckBeforeOnCloseGame()
        {
            
        }

        private void CheckAfterOnCloseGame()
        {
            
        }

        protected override void ReportError(short errorCode, Exception exception, object state)
        {
            string msg;
            if (errorCode == ErrorCodes.UnhandledException)
            {
                msg = string.Format("{0}", exception);
            }
            else
            {
                msg = string.Format("{0}", errorCode);
            }

            this.PluginHost.BroadcastErrorInfoEvent(msg);
        }
    }
}
