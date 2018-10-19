using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using NUnit.Framework;
using Photon.Hive.Common.Lobby;
using Photon.LoadBalancing.UnifiedClient;
using Photon.LoadBalancing.UnitTests.UnifiedServer;
using Photon.UnitTest.Utils.Basic;
using ErrorCode = Photon.Common.ErrorCode;
using EventCode = ExitGames.Client.Photon.LoadBalancing.EventCode;
using OperationCode = ExitGames.Client.Photon.LoadBalancing.OperationCode;

namespace Photon.LoadBalancing.UnitTests
{
    public abstract class PluginTestsImpl : LoadBalancingUnifiedTestsBase
    {
        protected PluginTestsImpl(ConnectPolicy policy) : base(policy)
        {
        }

        [Test]
        public void PluginsBasicsTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {

                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.PlayerTTL, 0},
                        {ParameterCode.CheckUserOnJoin, false},
                        {ParameterCode.CleanupCacheOnLeave, false},
                        {ParameterCode.SuppressRoomEvents, false},
                        {ParameterCode.LobbyName, "Default"},
                        {ParameterCode.LobbyType, (byte)0},
                        {ParameterCode.GameProperties, new Hashtable
                        {
                            {"GameProperty", "GamePropertyValue"},
                            {"GameProperty2", "GamePropertyValue2"},
                            {GamePropertyKey.IsVisible, false},
                            {GamePropertyKey.IsOpen, false},
                            {GamePropertyKey.MaxPlayers, 10},
                            {GamePropertyKey.PropsListedInLobby, new string[]{"LobbyProperty", "LobbyPropertyValue"}}
                        }
                        },
                        {ParameterCode.PlayerProperties, new Hashtable {{"Actor1Property1", "Actor1Property1Value"}}},
                        {ParameterCode.Plugins, new string[]{"BasicTestsPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                response = client.SendRequestAndWaitForResponse(request);

                Assert.AreEqual("BasicTestsPlugin", response[201]);
                Assert.AreEqual("1.0", response[200]);

                client.CheckThereIsNoErrorInfoEvent();

                client.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Properties, new Hashtable {{GamePropertyKey.IsOpen, true}}}
                    }
                });

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{"Actor2Property", "Actor2PropertyValue"}}},
                        {ParameterCode.GameProperties, new Hashtable()},
                        {ParameterCode.UserId, "User2"},
                        {ParameterCode.LobbyName, "Default"},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");
                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"Actor1Property2", "Actor1Property2Value"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)1},
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)2},
                        {ParameterCode.Cache, (byte)EventCaching.AddToRoomCache},
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)3},
                        {ParameterCode.Cache, (byte)EventCaching.AddToRoomCache},
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)4},
                        {ParameterCode.Cache, (byte)EventCaching.AddToRoomCache},
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)5},
                        {ParameterCode.Cache, (byte)EventCaching.AddToRoomCache},
                        {ParameterCode.ActorList, new[]{0, 1}},
                        {ParameterCode.Data, new[]{0, 1}},
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.ReceiverGroup, 123},
                        {ParameterCode.Group, 123},
                        {ParameterCode.EventForward, true},
                        {ParameterCode.CacheSliceIndex, 123},
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                client2.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.Leave,
                    Parameters = new Dictionary<byte, object> { { ParameterCode.IsInactive, true } }
                });
                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                client2.Disconnect();

                client.CheckThereIsNoErrorInfoEvent();

                client.Disconnect();
            }
            finally
            {
                if (client != null && client.Connected)
                {
                    client.Disconnect();
                    client.Dispose();
                }

                if (client2 != null && client2.Connected)
                {
                    client2.Disconnect();
                    client2.Dispose();
                }
            }
        }

        #region OnCreate Tests

        [Test]
        public void OnCreatePreConditionFail()
        {
            UnifiedTestClient client = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.Plugins, new string[]{"BasicTestsPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                response = client.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                Assert.IsNotNullOrEmpty(response.DebugMessage);
            }
            finally
            {
                if (client != null && client.Connected)
                {
                    client.Disconnect();
                    client.Dispose();
                }
            }
        }

        [Test]
        public void OnCreatePostConditionFail()
        {
            UnifiedTestClient client = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.PlayerTTL, 0},
                        {ParameterCode.CheckUserOnJoin, false},
                        {ParameterCode.CleanupCacheOnLeave, false},
                        {ParameterCode.SuppressRoomEvents, false},
                        {ParameterCode.LobbyName, "Default"},
                        {ParameterCode.LobbyType, (byte) 0},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "UnexpectedByPluginValue"},
                                {GamePropertyKey.IsVisible, false},
                                {GamePropertyKey.IsOpen, false},
                                {GamePropertyKey.MaxPlayers, 10},
                                {GamePropertyKey.PropsListedInLobby, new string[] {"LobbyProperty", "LobbyPropertyValue"}}
                            }
                        },
                        {ParameterCode.PlayerProperties, new Hashtable {{"Actor2Property", "Actor2PropertyValue"}}},
                        {ParameterCode.Plugins, new string[]{"BasicTestsPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                // connect to gameserver
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                // resend request to game server
                client.SendRequestAndWaitForResponse(request);


                Assert.AreEqual(OperationCode.CreateGame, response.OperationCode);
                Assert.AreEqual(0, response.ReturnCode);

                var ev = client.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                ev = client.WaitForEvent();
                Assert.AreEqual(EventCode.ErrorInfo, ev.Code);
                Assert.IsNotNullOrEmpty((string)ev[ParameterCode.Info]);
            }
            finally
            {
                if (client != null && client.Connected)
                {
                    client.Disconnect();
                    client.Dispose();
                }
            }
        }

        [Test]
        public void SetStateAfterContinueFailureTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "SetStateAfterContinueFailureTest";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"SetStateAfterContinueTestPlugin"}},
                        {ParameterCode.PlayerTTL, int.MaxValue},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                client.Disconnect();
                DisposeClient(client);

                Thread.Sleep(1000);

                client = this.CreateMasterClientAndAuthenticate("User1");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"SetStateAfterContinueTestPlugin"}},
                        {ParameterCode.ActorNr, 1},
                    }
                };

                response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                client.CheckThereIsEvent(123);
            }
            finally
            {
                DisposeClient(client);
            }
        }

        [Test]
        public void OnCreateWithErrorPluginTest()
        {
            UnifiedTestClient client = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.Plugins, new string[]{"ErrorPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                // connect to gameserver
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                // resend request to game server
                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                Assert.AreEqual(OperationCode.CreateGame, response.OperationCode);

                EventData ev;
                Assert.IsFalse(client.TryWaitForEvent(EventCode.Join, 3000, out ev));
            }
            finally
            {
                if (client != null && client.Connected)
                {
                    client.Disconnect();
                    client.Dispose();
                }
            }
        }

        [Test]
        public void OnCreateUsingStripedGameStateTest()
        {
            UnifiedTestClient client = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.Plugins, new string[]{"StripedGameStatePlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                // connect to gameserver
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                // resend request to game server
                client.SendRequestAndWaitForResponse(request);

                Assert.AreEqual(OperationCode.CreateGame, response.OperationCode);

                EventData ev;
                Assert.IsTrue(client.TryWaitForEvent(EventCode.Join, 3000, out ev));
            }
            finally
            {
                if (client != null && client.Connected)
                {
                    client.Disconnect();
                    client.Dispose();
                }
            }
        }

        #endregion

        #region Join Tests

        [Test]
        public void OnBeforeJoinPostConditionFail()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "OnBeforeJoinPostConditionFail";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},

                        {ParameterCode.Plugins, new string[]{"JoinFailuresCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                var ev = client.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);

                Thread.Sleep(200);
                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{"Actor2Property", "UnexpectedValue"}}},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request);

                ev = client.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                ev = client.WaitForEvent();
                Assert.AreEqual(EventCode.ErrorInfo, ev.Code);
                Assert.IsNotNullOrEmpty((string)ev[ParameterCode.Info]);

            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnBeforeJoinCallsFail()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "OnBeforeJoinCallsFail";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"FailBeforeJoinPreCondition", "true"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"JoinFailuresCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                var ev = client.WaitForEvent();
                Assert.AreEqual((byte)Hive.Operations.EventCode.Join, ev.Code);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                client.CheckThereIsNoErrorInfoEvent();

            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnJoinCallsFail()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;

            const string GameName = "OnJoinPreConditionFail";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"FailBeforeOnJoin", "true"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"JoinFailuresCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);
                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{"Actor2Property", "Actor2PropertyValue"}}},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                client.CheckThereIsEvent(123);
                client.CheckThereIsNoEvent(EventCode.Leave);
                client2.CheckThereIsNoErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnJoinPostConditionFail()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "JoinTests";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"FailAfterOnJoin", "true"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"JoinFailuresCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);
                client.CheckThereIsErrorInfoEvent();
                client2.CheckThereIsEvent(EventCode.Join);
                client2.CheckThereIsErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void JoinLogicFailTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "JoinLogicFailTest";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"FailAfterOnJoin", "true"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"JoinFailuresCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.CacheSliceIndex, 125}
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.OperationInvalid);

                client.CheckThereIsEvent(123);// we sent event from OnLeave
                client.CheckThereIsNoErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        //[Test]
        //public void OnJoinBlockJoinEventTest()
        //{
        //    TestClientBase client = null;
        //    TestClientBase client2 = null;
        //    const string GameName = "JoinTests";

        //    try
        //    {
        //        client = this.CreateMasterClientAndAuthenticate("User1");

        //        var request = new OperationRequest
        //        {
        //            OperationCode = OperationCode.CreateGame,
        //            Parameters = new Dictionary<byte, object>
        //            {
        //                {ParameterCode.RoomName, GameName},
        //                {ParameterCode.EmptyRoomTTL, 0},
        //                {
        //                    ParameterCode.GameProperties, new Hashtable
        //                    {
        //                        {"BlockJoinEvents", "true"},
        //                    }
        //                },
        //                {ParameterCode.Plugins, new string[]{"JoinFailuresCheckPlugin"}},
        //            }
        //        };

        //        var response = client.SendRequestAndWaitForResponse(request);
        //        this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
        //        client.SendRequestAndWaitForResponse(request);

        //        client.CheckThereIsEvent(EventCode.Join);

        //        client2 = this.CreateMasterClientAndAuthenticate("User2");

        //        request = new OperationRequest
        //        {
        //            OperationCode = OperationCode.JoinGame,
        //            Parameters = new Dictionary<byte, object>
        //            {
        //                {ParameterCode.RoomName, GameName},
        //            },
        //        };

        //        response = client2.SendRequestAndWaitForResponse(request);
        //        this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
        //        response = client2.SendRequestAndWaitForResponse(request);

        //        Assert.IsNotNull(response.Parameters);
        //        Assert.IsTrue(response.Parameters.ContainsKey(0));

        //        client.CheckThereIsNoEvent(EventCode.Join);
        //        client2.CheckThereIsNoEvent(EventCode.Join);
        //    }
        //    finally
        //    {
        //        DisposeClient(client);
        //        DisposeClient(client2);
        //    }
        //}

        //[Test]
        //public void OnJoinDoNotPublishCacheTest()
        //{
        //    TestClientBase client = null;
        //    TestClientBase client2 = null;
        //    const string GameName = "JoinTests";

        //    try
        //    {
        //        client = this.CreateMasterClientAndAuthenticate("User1");

        //        var request = new OperationRequest
        //        {
        //            OperationCode = OperationCode.CreateGame,
        //            Parameters = new Dictionary<byte, object>
        //            {
        //                {ParameterCode.RoomName, GameName},
        //                {ParameterCode.EmptyRoomTTL, 0},
        //                {ParameterCode.Plugins, new string[]{"JoinFailuresCheckPlugin"}},
        //            },
        //        };

        //        var response = client.SendRequestAndWaitForResponse(request);
        //        this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
        //        client.SendRequestAndWaitForResponse(request);

        //        client.CheckThereIsEvent(EventCode.Join);

        //        request = new OperationRequest
        //        {
        //            OperationCode = OperationCode.RaiseEvent,
        //            Parameters = new Dictionary<byte, object>
        //            {
        //                {ParameterCode.Code, (byte) 1},
        //                {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
        //            }
        //        };

        //        client.SendRequest(request);

        //        client2 = this.CreateMasterClientAndAuthenticate("User2");

        //        request = new OperationRequest
        //        {
        //            OperationCode = OperationCode.JoinGame,
        //            Parameters = new Dictionary<byte, object>
        //            {
        //                {ParameterCode.RoomName, GameName},
        //                {
        //                    ParameterCode.PlayerProperties, new Hashtable
        //                    {
        //                        {"Actor2Property", "Actor2PropertyValue"},
        //                        {"DoNotPublishCache", "true"},
        //                    }
        //                },
        //            },
        //        };

        //        response = client2.SendRequestAndWaitForResponse(request);
        //        this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
        //        client2.SendRequestAndWaitForResponse(request);

        //        client.CheckThereIsNoErrorInfoEvent();

        //        client2.CheckThereIsNoEvent(1);
        //        client2.CheckThereIsNoErrorInfoEvent();

        //        client2.Disconnect();

        //        client2.EventQueueClear();

        //        Thread.Sleep(1000);

        //        client2 = this.CreateMasterClientAndAuthenticate("User2");

        //        // now we should get cached events
        //        request = new OperationRequest
        //        {
        //            OperationCode = OperationCode.JoinGame,
        //            Parameters = new Dictionary<byte, object>
        //            {
        //                {ParameterCode.RoomName, GameName},
        //                {
        //                    ParameterCode.PlayerProperties, new Hashtable
        //                    {
        //                        {"Actor2Property", "Actor2PropertyValue"},
        //                    }
        //                },
        //            },
        //        };

        //        response = client2.SendRequestAndWaitForResponse(request);
        //        this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
        //        client2.SendRequestAndWaitForResponse(request);

        //        client.CheckThereIsNoErrorInfoEvent();
        //        client2.CheckThereIsEvent(1);
        //        client2.CheckThereIsNoErrorInfoEvent();

        //        client2.Disconnect();

        //    }
        //    finally
        //    {
        //        DisposeClient(client);
        //        DisposeClient(client2);
        //    }
        //}

        #endregion

        #region Join with Exceptions
        [Test]
        public void BeforeJoinBeforeContinueExceptionTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"JoinExceptionsPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{(byte)255, "BeforeJoinBeforeContinueFail"}}},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                var ev = client.WaitForEvent();
                Assert.AreEqual(124, ev.Code);// we sent it from ReportError

                Assert.IsFalse(client.TryWaitForEvent(123, 3000, out ev));

                //Assert.AreEqual(EventCode.ErrorInfo, ev.Code);
                //Assert.IsNotNullOrEmpty((string)ev[ParameterCode.Info]);

                client.CheckThereIsNoErrorInfoEvent();

                client2.Disconnect();
                client2.Dispose();

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                response = client2.SendRequestAndWaitForResponse(request);

                var actorsList = (int[])response.Parameters[ParameterCode.ActorList];
                // there is only two!!!
                Assert.AreEqual(2, actorsList.Length);

            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void BeforeJoinContinueExceptionTest()
        {
            if (this.IsOnline)
            {
                Assert.Ignore("Test is not supported  in 'online' mode");
            }
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"JoinExceptionsPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{"ProcessBeforeJoinException", ""}}},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                var ev = client.WaitForEvent();
                Assert.AreEqual(124, ev.Code);// we sent it from ReportError

                Assert.IsFalse(client.TryWaitForEvent(123, 3000, out ev));

                //Assert.AreEqual(EventCode.ErrorInfo, ev.Code);
                //Assert.IsNotNullOrEmpty((string)ev[ParameterCode.Info]);

                client.CheckThereIsNoErrorInfoEvent();

                client2.Disconnect();
                client2.Dispose();

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                response = client2.SendRequestAndWaitForResponse(request);

                var actorsList = (int[])response.Parameters[ParameterCode.ActorList];
                // there is only two!!!
                Assert.AreEqual(2, actorsList.Length);

            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnJoinBeforeContinueExceptionTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"JoinExceptionsPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{(byte)255, "OnJoinBeforeContinueFail"}}},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                var ev = client.WaitForEvent();
                Assert.AreEqual(124, ev.Code);// we sent it from ReportError

                ev = client.WaitForEvent();
                Assert.AreEqual(123, ev.Code);// we sent it from OnLeave

                //Assert.IsFalse(client.TryWaitForEvent(EventCode.ErrorInfo, 3000, out ev));

                //Assert.AreEqual(EventCode.ErrorInfo, ev.Code);
                //Assert.IsNotNullOrEmpty((string)ev[ParameterCode.Info]);

                client.CheckThereIsNoErrorInfoEvent();

                client2.Disconnect();
                client2.Dispose();

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                response = client2.SendRequestAndWaitForResponse(request);

                var actorsList = (int[]) response.Parameters[ParameterCode.ActorList];
                // there is only two!!!
                Assert.AreEqual(2, actorsList.Length);

            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void BeforeJoinAfterContinueExceptionTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"JoinExceptionsPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{(byte)255, "BeforeJoinAfterContinueFail"}}},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request);

                // message from ReportError should be get twice
                var ev = client.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);

                ev = client.WaitForEvent();
                Assert.AreEqual(124, ev.Code);// we sent it from ReportError

                client.CheckThereIsNoErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        #endregion

        #region Schduling Tests
        [Test]
        public void ScheduleBroadcastEvent()
        {
            UnifiedTestClient client = null;
            string GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"EventCode", 1},
                                {"Interval", 25}, // ms 
                                {"EventSize", 100}, // bytes
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"ScheduleBroadcastTestPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);
                client.CheckThereIsEvent(1);

                client.OpSetPropertiesOfRoom(new Hashtable
                                                 {
                                                    {"EventCode", 2},
                                                    {"Interval", 25}, // ms 
                                                    {"EventSize", 100}, // bytes
                                                 });

                //client.CheckThereIsEventAndFailOnTimout(EventCode.PropertiesChanged);

                client.CheckThereIsEvent(2);

            }
            finally
            {
                DisposeClient(client);
            }
        }

        [Test]
        public void ScheduleSetProperties()
        {
            UnifiedTestClient client = null;
            string GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"EventCode", 1},
                                {"Interval", 25}, // ms 
                                {"RoomIndex", 1}, // start value
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"ScheduleSetPropertiesTestPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);
                client.CheckThereIsEvent(EventCode.PropertiesChanged);

            }
            finally
            {
                DisposeClient(client);
            }
        }

        #endregion

        #region MasterClientId Tests

        [Test]
        public void MasterClientIdChange()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "MasterClientIdTests";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},

                        {ParameterCode.Plugins, new string[]{"MasterClientIdPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                var ev = client.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{"Actor2Property", "UnexpectedValue"}}},
                    },
                };

                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);
                client2.SendRequestAndWaitForResponse(request);

                ev = client.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);

                client.CheckThereIsNoErrorInfoEvent();

                client.Disconnect();

                client2.CheckThereIsEvent(123);

            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        #endregion

        #region Set properties tests

        private static void CheckActorPropertyValue(UnifiedTestClient client, string propertyName, string expectedPropertyValue)
        {
            // check property value
            var request = new OperationRequest
            {
                OperationCode = OperationCode.GetProperties,
                Parameters = new Dictionary<byte, object>
                {
                    {ParameterCode.ActorList, new int[] {1}},
                    {ParameterCode.PlayerProperties, new string[] {propertyName}},
                    {ParameterCode.Properties, (byte) 2}
                }
            };

            var response = client.SendRequestAndWaitForResponse(request);

            var properties = (Hashtable)response[ParameterCode.PlayerProperties];
            Assert.IsNotNull(properties);
            var actor1Properties = (Hashtable)properties[1];
            Assert.IsNotNull(actor1Properties);
            Assert.AreEqual(expectedPropertyValue, actor1Properties[propertyName]);
        }

        [Test]
        public void BeforeSetGamePropertiesPreCheck()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "SetProperties";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                // just set property in order to check it value later
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "PropertyValue"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();


                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "BeforeSetPropertiesPreCheckFail"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", "PropertyValue");
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void BeforeSetGamePropertiesExceptionInContinueCheck()
        {
            if (this.IsOnline)
            {
                Assert.Ignore("This test does not support 'Online' mode");
            }

            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "SetProperties";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                // just set property in order to check it value later
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "PropertyValue"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();


                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "BeforeSetPropertiesExceptionInContinue"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.InternalServerError);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", "PropertyValue");
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void BeforeSetGamePropertiesPostCheck()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "SetProperties";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");
                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "BeforeSetPropertiesPostCheckFail"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsErrorInfoEvent();
                client2.CheckThereIsErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", "BeforeSetPropertiesPostCheckFail");
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnSetPropertiesPreCheck()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "SetProperties";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                var propertyValue = "PropertyValue";
                var propertyKey = "ActorProperty";
                // just set property in order to check it value later
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {propertyKey, propertyValue}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();


                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {propertyKey, "OnSetPropertiesPreCheckFail"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                client.CheckThereIsNoEvent(EventCode.PropertiesChanged);
                client2.CheckThereIsNoEvent(EventCode.PropertiesChanged);
                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, propertyKey, propertyValue);
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnSetPropertiesFailTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "SetProperties";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                const string propertyValue = "PropertyValue";
                const string propertyKey = "ActorProperty";
                // just set property in order to check it value later
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {propertyKey, propertyValue}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();


                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {propertyKey, "OnSetPropertiesPreCheckFail"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                client.CheckThereIsNoEvent(EventCode.PropertiesChanged);
                client2.CheckThereIsNoEvent(EventCode.PropertiesChanged);
                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, propertyKey, propertyValue);

                const string newPropertyValue = "NewPropertyValue";
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {ParameterCode.Broadcast, true},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {propertyKey, newPropertyValue}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoEvent(EventCode.PropertiesChanged);
                client2.CheckThereIsEvent(EventCode.PropertiesChanged);
                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, propertyKey, newPropertyValue);

            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnSetGamePropertiesPostCheck()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "SetProperties";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");
                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "BeforeSetPropertiesPostCheckFail"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", "BeforeSetPropertiesPostCheckFail");
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnSetGamePropertiesExceptionInContinueCheck()
        {
            if (this.IsOnline)
            {
                Assert.Ignore("This test does not support 'Online' mode");
            }

            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "OnSetProperties";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                // just set property in order to check it value later
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "PropertyValue"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);
                client.CheckThereIsNoErrorInfoEvent();


                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "OnSetPropertiesExceptionInContinue"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.InternalServerError);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", "PropertyValue");
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnSetGamePropertiesCASFailureCheck()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "SetPropertiesCAS";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");
                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                const string PropertyValue = "PropertyValue";
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", PropertyValue}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", PropertyValue);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", "AnotherValueForCASCheck"}
                            }
                        },
                        {
                            ParameterCode.ExpectedValues, new Hashtable
                            {
                                {"ActorProperty", "NonExistingValue"}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.OperationInvalid);

                client.CheckThereIsEvent(124);
                client.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", PropertyValue);
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnSetGamePropertiesCASNotificationCheck()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string GameName = "SetPropertiesCAS";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {
                            ParameterCode.GameProperties, new Hashtable
                            {
                                {"GameProperty", "GamePropertyValue"},
                                {"GameProperty2", "GamePropertyValue2"},
                            }
                        },
                        {ParameterCode.Plugins, new string[]{"SetPropertiesCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");
                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                const string PropertyValue = "PropertyValue";
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", PropertyValue}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", PropertyValue);

                const string AnotherProperty = "AnotherValueForCASCheck";
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {
                            ParameterCode.Properties, new Hashtable
                            {
                                {"ActorProperty", AnotherProperty}
                            }
                        },
                        {
                            ParameterCode.ExpectedValues, new Hashtable
                            {
                                {"ActorProperty", PropertyValue}
                            }
                        },
                    }
                };

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.PropertiesChanged);
                client2.CheckThereIsEvent(EventCode.PropertiesChanged);

                client.CheckThereIsNoEvent(124);
                client.CheckThereIsNoErrorInfoEvent();

                CheckActorPropertyValue(client, "ActorProperty", AnotherProperty);
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void UpdateGamePropertiesOnMasterFromPlugin()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            UnifiedTestClient client3 = null;
            const string GameName = "SetPropertiesCAS";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.Plugins, new string[] {"ChangeGamePropertiesOnJoinPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string) response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    },
                };

                client2 = this.CreateMasterClientAndAuthenticate("User2");
                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string) response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);

                client.CheckThereIsNoErrorInfoEvent();
                client2.CheckThereIsNoErrorInfoEvent();

                Thread.Sleep(100);

                client3 = this.CreateMasterClientAndAuthenticate("User3");

                client3.JoinRandomGame(null, 0, null, MatchmakingMode.RandomMatching, "", AppLobbyType.Default, "", (short)ErrorCode.NoMatchFound);
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
                DisposeClient(client3);
            }
        }
        #endregion

        #region Raise Event tests
        [Test]
        public void RaiseEventCacheManagmentTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "RaiseEvent";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"RaiseEventChecksPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 1},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected in cache
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 2},
                        {ParameterCode.Cache, (byte) EventCaching.SliceIncreaseIndex},
                        {ParameterCode.Data, new Hashtable{{0, 2}}},// expected сaches count
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 1},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected in cache
                    }
                };

                client.SendRequest(request);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 1},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 2}}},// expected in cache
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 2},
                        {ParameterCode.Cache, (byte) EventCaching.SlicePurgeIndex},
                        {ParameterCode.CacheSliceIndex, 0},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected сaches count
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                for (var i = 0; i < 5; ++i)
                {
                    request = new OperationRequest
                    {
                        OperationCode = OperationCode.RaiseEvent,
                        Parameters = new Dictionary<byte, object>
                        {
                            {ParameterCode.Code, (byte) 2},
                            {ParameterCode.Cache, (byte) EventCaching.SliceIncreaseIndex},
                            {ParameterCode.Data, new Hashtable{{0, 2 + i}}},// expected сaches count
                        }
                    };

                    client.SendRequest(request);

                    client.CheckThereIsNoErrorInfoEvent();
                }

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 3},
                        {ParameterCode.Cache, (byte) EventCaching.SliceSetIndex},
                        {ParameterCode.CacheSliceIndex, 10},
                        {ParameterCode.Data, new Hashtable{{0, 10}}},// expected index
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 3},
                        {ParameterCode.Cache, (byte) EventCaching.SliceSetIndex},
                        {ParameterCode.CacheSliceIndex, 5},
                        {ParameterCode.Data, new Hashtable{{0, 5}}},// expected index
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                        {
                            {ParameterCode.Code, (byte) 2},
                            {ParameterCode.Cache, (byte) EventCaching.SlicePurgeUpToIndex},
                            {ParameterCode.CacheSliceIndex, 4},
                            {ParameterCode.Data, new Hashtable{{0, 4}}},// expected сaches count
                        }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
            }
        }
        #endregion

        #region Plugin Custom Type tests

        protected class CustomPluginType
        {
            public int intField;
            public byte byteField;
            public string stringField;

            public CustomPluginType()
            {
            }

            public CustomPluginType(byte[] bytes)
            {
                using (var s = new MemoryStream(bytes))
                using (var br = new BinaryReader(s))
                {
                    this.intField = br.ReadInt32();
                    this.byteField = br.ReadByte();
                    this.stringField = br.ReadString();
                }
            }

            public byte[] Serialize()
            {
                using (var s = new MemoryStream())
                using (var bw = new BinaryWriter(s))
                {
                    bw.Write(this.intField);
                    bw.Write(this.byteField);
                    bw.Write(this.stringField);

                    return s.ToArray();
                }
            }
            static public byte[] Serialize(object o)
            {
                return ((CustomPluginType) o).Serialize();
            }

            static public CustomPluginType Deserialize(byte[] data)
            {
                return new CustomPluginType(data);
            }
        }

        [Test]
        public void PluginCustomTypeTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "CustomType";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"CustomTypeCheckPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                var customObj = new CustomPluginType
                {
                    byteField = 1,
                    intField = 2,
                    stringField = "3",
                };
                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 1},
                        {ParameterCode.Data, new Hashtable{{0, customObj}}},// expected in cache
                    }
                };

                client.SendRequest(request);

                var ev = client.WaitForEvent();
                Assert.AreEqual(123, ev.Code);

                customObj = (CustomPluginType)ev[0];
                Assert.AreEqual(2, customObj.byteField);
                Assert.AreEqual(3, customObj.intField);
                Assert.AreEqual("4", customObj.stringField);
            }
            finally
            {
                DisposeClient(client);
            }
        }
        #endregion

        #region Sync/Assync Http Request test

        [Test]
        public void SyncAsyncHttpRequestTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "SyncAsyncHttpRequestTest";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"SyncAsyncHttpTestPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                // we send event which will be handled in sync http request
                var reRequest = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 0},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected in cache
                    }
                };

                client.SendRequest(reRequest);

                reRequest.Parameters[ParameterCode.Code] = (byte)3;
                client.SendRequest(reRequest);

                // we get response for first raise event request
                var ev = client.WaitForEvent();
                Assert.AreEqual(0, ev.Code);

                // and now for second raise event request
                ev = client.WaitForEvent();
                Assert.AreEqual(3, ev.Code);

                // we get response for first raise event request
                reRequest.Parameters[ParameterCode.Code] = (byte)1;
                client.SendRequest(reRequest);

                reRequest.Parameters[ParameterCode.Code] = (byte)3;
                client.SendRequest(reRequest);


                // we get response for first raise event request
                ev = client.WaitForEvent();
                Assert.AreEqual(3, ev.Code);

                // and now for second raise event request
                ev = client.WaitForEvent();
                Assert.AreEqual(1, ev.Code);
            }
            finally
            {
                DisposeClient(client);
            }
        }

        #endregion

        #region Custom Type mapper

        [Test]
        public void CustomTypeMapperPluginTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "CustomTypeMapperPlugin";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"CustomTypeMapperPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                const string json = "{\"array\":[1,2,3],\"null\":null,\"boolean\":true,\"number\":123,\"string\":\"Hello World\"}";
                //"{\"array\":[null, \"hause\",1,2,3],\"null\":null,\"boolean\":true,\"number\":123,\"object\":{\"a\":\"b\",\"c\":\"d\",\"e\":\"f\"},\"string\":\"Hello World\"}";

                // we send event which will be handled in sync http request
                var reRequest = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 0},
                        {ParameterCode.Data, new Hashtable{{0, json}}},// expected in cache
                    }
                };

                client.SendRequest(reRequest);

                // we get response for first raise event request
                var ev = client.WaitForEvent();
                Assert.AreEqual(123, ev.Code);

                var data = (Dictionary<string, object>)ev.Parameters[1];

                Assert.IsNull(data["null"]);
                Assert.IsTrue((bool) data["boolean"]);
                Assert.AreEqual("Hello World", data["string"]);
                Assert.AreEqual(123, data["number"]);
                Assert.AreEqual(new object[]{1, 2, 3},data["array"]);
            }
            finally
            {
                DisposeClient(client);
            }
        }
        #endregion

        #region Strict mode test
        [Test]
        public void RaiseEventStrictModeTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "RaiseEventStrictModeTest";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"StrictModeFailurePlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 0},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected in cache
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 1},
                        {ParameterCode.Cache, (byte) EventCaching.SliceIncreaseIndex},
                        {ParameterCode.Data, new Hashtable{{0, 2}}},// expected сaches count
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 2},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected in cache
                    }
                };

                client.SendRequest(request);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 3},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 2}}},// expected in cache
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 4},
                        {ParameterCode.Cache, (byte) EventCaching.SlicePurgeIndex},
                        {ParameterCode.CacheSliceIndex, 0},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected сaches count
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 5},
                        {ParameterCode.Cache, (byte) EventCaching.SlicePurgeIndex},
                        {ParameterCode.CacheSliceIndex, 0},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected сaches count
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
            }
        }

        [Test]
        public void BeforeSetPropertiesStrictModeTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "SetPropertiesStrictMode";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"StrictModeFailurePlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 0},
                        {ParameterCode.Properties, new Hashtable()}
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 1},
                        {ParameterCode.Properties, new Hashtable()}
                    }
                };
                client.SendRequest(request);

                client.CheckThereIsNoErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, 2},
                        {ParameterCode.Properties, new Hashtable()}
                    }
                };
                client.SendRequest(request);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 3},
                        {ParameterCode.Properties, new Hashtable()}
                    }
                };
                client.SendRequest(request);

                client.CheckThereIsErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 4},
                        {ParameterCode.Properties, new Hashtable()}
                    }
                };
                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
            }
        }

        [Test]
        public void OnSetPropertiesForgotCall()
        {
            UnifiedTestClient client = null;

            const string gameName = "OnSetPropertiesForgotCall";
            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, gameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[] {"StrictModeFailurePlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ActorNr, 0},
                        {ParameterCode.Properties, new Hashtable()}
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
            }
        }

        [TestCase("BeforeJoinForgotCall")]
        [TestCase("OnJoinForgotCall")]
        public void OnJoinStrictModeFail(string gameName)
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, gameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[] {"StrictModeFailurePlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(100);
                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, gameName},
                    }
                };

                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequest(request);

                Thread.Sleep(100);
                CheckErrorEvent(client, gameName);
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [Test]
        public void OnLeaveForgotCall()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const string gameName = "OnLeaveForgotCall";
            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, gameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[] {"StrictModeFailurePlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(100);
                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, gameName},
                    }
                };

                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequest(request);

                Thread.Sleep(100);
                client.CheckThereIsNoErrorInfoEvent();

                client2.Disconnect();
                Thread.Sleep(100);
                client.CheckThereIsErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        //[Test]
        //public void OnCreateForgotCall()
        //{
        //    TestClientBase client = null;
        //    TestClientBase client2 = null;
        //    const string gameName = "OnCreateForgotCall";
        //    try
        //    {
        //        client = this.CreateMasterClientAndAuthenticate("User1");

        //        var request = new OperationRequest
        //        {
        //            OperationCode = OperationCode.CreateGame,
        //            Parameters = new Dictionary<byte, object>
        //            {
        //                {ParameterCode.RoomName, gameName},
        //                {ParameterCode.EmptyRoomTTL, 0},
        //                {ParameterCode.Plugins, new string[] {"StrictModeFailurePlugin"}},
        //            }
        //        };

        //        var response = client.SendRequestAndWaitForResponse(request);

        //        this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

        //        client.SendRequestAndWaitForResponse(request);

        //        client.CheckThereIsEvent(EventCode.Join);

        //    }
        //    finally
        //    {
        //        DisposeClient(client);
        //    }
        //}

        #endregion

        #region Callback Exceptions test
        [Test]
        public void HttpAndTimerCallbackExceptionTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "HttpAndTimerCallbackExceptionTest";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"StrictModeFailurePlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 5},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected in cache
                    }
                };

                client.SendRequest(request);

                client.CheckThereIsErrorInfoEvent(5000);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 6},
                        {ParameterCode.Cache, (byte) EventCaching.SliceIncreaseIndex},
                        {ParameterCode.Data, new Hashtable{{0, 2}}},// expected сaches count
                    }
                };

                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                client.CheckThereIsErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 7},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected in cache
                    }
                };

                client.SendRequest(request);
                client.CheckThereIsErrorInfoEvent();

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 8},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 2}}},// expected in cache
                    }
                };

                client.SendRequest(request);
                client.CheckThereIsErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
            }
        }

        [Test]
        public void WrongUrlExceptionTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "WrongUrlExceptionTest";

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"WrongUrlTestPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                client.CheckThereIsEvent(EventCode.Join);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 5},
                        {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                        {ParameterCode.Data, new Hashtable{{0, 1}}},// expected in cache
                    }
                };

                client.WaitTimeout = 30000000;
                client.SendRequest(request);

                client.CheckThereIsErrorInfoEvent();
            }
            finally
            {
                DisposeClient(client);
            }
        }

        #endregion

        #region ErrorPlugin test
        [Test]
        public void ErrorPluginTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{"ErrorPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginReportedError);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{(byte)255, "ErrorPlugin"}}},
                    },
                };

                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.GameIdNotExists);
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        [TestCase("NullRefPlugin")]
        [TestCase("ExceptionPlugin")]
        public void FailedToCreatePluginTest(string pluginName)
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;
            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 0},
                        {ParameterCode.Plugins, new string[]{pluginName}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginMismatch);

                Thread.Sleep(200);

                client2 = this.CreateMasterClientAndAuthenticate("User2");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerProperties, new Hashtable {{(byte)255, pluginName}}},
                    },
                };

                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.GameIdNotExists);
            }
            finally
            {
                DisposeClient(client);
                DisposeClient(client2);
            }
        }

        #endregion

        #region Misc tests

        [Test]
        public void CodemastersRemoveInOnLeaveTest()
        {
            UnifiedTestClient client = null;
            const string GameName = "CodemastersRemoveInOnLeaveTest";

            const int TTL = 15000;
            try
            {
                client = this.CreateMasterClientAndAuthenticate("User1");

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.EmptyRoomTTL, 30000},
                        {ParameterCode.PlayerTTL, TTL},
                        {ParameterCode.Plugins, new string[] {"RemovingActorPlugin"}},
                    }
                };

                var response = client.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client, (string) response[ParameterCode.Address], client.UserId);
                client.SendRequestAndWaitForResponse(request);
                client.CheckThereIsEvent(EventCode.Join);


                request = new OperationRequest
                {
                    OperationCode = OperationCode.Leave,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.IsInactive, true}
                    }
                };
                client.SendRequest(request);

                Thread.Sleep(TTL + 5000);
            }
            finally
            {
                DisposeClient(client);
            }
        }

        [Test]
        public void BroadcastEventToNonExistingUser()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            UnifiedTestClient client3 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client1 = this.CreateMasterClientAndAuthenticate("Player1");
                client2 = this.CreateMasterClientAndAuthenticate("Player2");
                client3 = this.CreateMasterClientAndAuthenticate("Player3");


                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerTTL, 10000},
                        {ParameterCode.Plugins, new string[] {"BroadcastEventPlugin"}},
                    }
                };

                var response = client1.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client1, (string)response[ParameterCode.Address], client1.UserId);
                client1.SendRequestAndWaitForResponse(request);
                client1.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(100);
                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                    }
                };

                this.JoinGame(client2, request);
                this.JoinGame(client3, request);


                request = new OperationRequest
                {
                    OperationCode = OperationCode.Leave,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.IsInactive, true}
                    }
                };
                client3.SendRequest(request);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)1},
                    }
                };
                client1.SendRequest(request);

                client2.WaitForEvent((byte) 1);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)2},
                    }
                };
                client1.SendRequest(request);

                client2.WaitForEvent((byte)1);
            }
            finally
            {
                DisposeClients(client1, client2, client3);
            }
        }

        [Test]
        public void SameInstaceOfPluginTest()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            var GameName = MethodBase.GetCurrentMethod().Name;
            var GameName2 = MethodBase.GetCurrentMethod().Name + 2;

            try
            {
                client1 = this.CreateMasterClientAndAuthenticate("Player1");
                client2 = this.CreateMasterClientAndAuthenticate("Player2");


                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName},
                        {ParameterCode.PlayerTTL, 10000},
                        {ParameterCode.Plugins, new string[] {"SameInstancePlugin"}},
                    }
                };

                var response = client1.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client1, (string)response[ParameterCode.Address], client1.UserId);
                client1.SendRequestAndWaitForResponse(request);
                client1.CheckThereIsEvent(EventCode.Join);

                Thread.Sleep(100);
                request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, GameName2},
                        {ParameterCode.PlayerTTL, 10000},
                        {ParameterCode.Plugins, new string[] {"SameInstancePlugin"}},
                    }
                };
                response = client2.SendRequestAndWaitForResponse(request);
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address]);
                client2.SendRequestAndWaitForResponse(request, (short)ErrorCode.PluginMismatch);

            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }
        

        #endregion

        #region Helpers
        private static void CheckErrorEvent(UnifiedTestClient client, string gameName)
        {
            if (gameName.EndsWith("Fail") || gameName.EndsWith("ForgotCall"))
            {
                client.CheckThereIsErrorInfoEvent();
                return;
            }
            client.CheckThereIsNoErrorInfoEvent();
        }

        private static void DisposeClient(UnifiedTestClient client)
        {
            if (client == null)
            {
                return;
            }

            if (client.Connected)
            {
                client.Disconnect();
            }
            client.Dispose();
        }

        private void JoinGame(UnifiedTestClient client, OperationRequest request)
        {
            var response = client.SendRequestAndWaitForResponse(request);
            this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address]);
            client.SendRequestAndWaitForResponse(request);
            client.CheckThereIsEvent(EventCode.Join);
        }

        #endregion
    }
}
