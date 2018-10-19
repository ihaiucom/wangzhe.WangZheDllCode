using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using Newtonsoft.Json;
using NUnit.Framework;
using Photon.Hive.Operations;
using Photon.LoadBalancing.UnifiedClient;
using Photon.LoadBalancing.UnitTests.UnifiedServer;
using Photon.UnitTest.Utils.Basic;
using OperationCode = ExitGames.Client.Photon.LoadBalancing.OperationCode;

namespace Photon.LoadBalancing.UnitTests.TestsImpl
{
    public abstract class SecureTestsImpl : LoadBalancingUnifiedTestsBase
    {
        protected SecureTestsImpl(ConnectPolicy policy) : base(policy)
        {
        }

        [Test]
        public void SecureParamsTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const int SleepTime = 350;
            try
            {
                client = (UnifiedTestClient)this.CreateTestClient();
                client.UserId = Player1;

                this.ConnectToServer(client, this.MasterAddress);

                var response = client.Authenticate(Player1, new Dictionary<byte, object>()
                                                                {
                                                                    {(byte)ParameterKey.ClientAuthenticationParams, "username=yes&token=yes"}
                                                                });

                Assert.AreEqual("nick", response[(byte)ParameterKey.Nickname]);
                Assert.IsNotNull(response[(byte)ParameterKey.Token]);


                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, "TestGame"},
                        {ParameterCode.Plugins, new string[]{"CheckSecurePlugin"}},
                    }
                };

                response = client.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId, new Dictionary<byte, object>()
                                                                {
                                                                    {(byte)ParameterKey.ClientAuthenticationParams, "username=yes&token=yes"}
                                                                });

                client.SendRequestAndWaitForResponse(request);

                Thread.Sleep(SleepTime);
                CheckSecure("CreateGameAuthCookie");

                client2 = this.CreateMasterClientAndAuthenticate("User2", new Dictionary<byte, object>()
                                                                {
                                                                    {(byte)ParameterKey.ClientAuthenticationParams, "username=yes&token=yes"}
                                                                });

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, "TestGame"},
                    }
                };

                response = client2.SendRequestAndWaitForResponse(request);

                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId, new Dictionary<byte, object>()
                                                                {
                                                                    {(byte)ParameterKey.ClientAuthenticationParams, "username=yes&token=yes"}
                                                                });

                client2.SendRequestAndWaitForResponse(request);

                Thread.Sleep(SleepTime);
                CheckSecure("JoinGameAuthCookie");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)1},
                        {ParameterCode.EventForward, (byte)3},
                    }
                };

                client2.SendRequest(request);

                Thread.Sleep(SleepTime);
                CheckSecure("RaiseEventAuthCookie");

                //just to ensure that there is nothing on server for RaiseEventAuthCookie
                CheckSecure("RaiseEventAuthCookie", expectToFail: true);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)1},
                        {ParameterCode.EventForward, (byte)1},// we send request but without secure
                    }
                };

                client2.SendRequest(request);
                CheckSecure("RaiseEventAuthCookie", expectToFail: true);


                client.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Properties, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.EventForward, (byte)3},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("SetPropertiesAuthCookie");

                client.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.WebRpc,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.EventForward, (byte)3},
                        {ParameterCode.UriPath, "RpcSecure"},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("RpcAuthCookie");

                client.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.WebRpc,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.UriPath, "RpcSecure"},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("RpcAuthCookie", true);

                var client3 = this.CreateMasterClientAndAuthenticate("User3", new Dictionary<byte, object>()
                                                                {
                                                                    {(byte)ParameterKey.ClientAuthenticationParams, "username=yes&token=yes"}
                                                                });
                client3.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.WebRpc,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.UriPath, "RpcSecure"},
                        {ParameterCode.EventForward, (byte)3},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("RpcAuthCookie");

                client3.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.WebRpc,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.UriPath, "RpcSecure"},
                        {ParameterCode.EventForward, (byte)1},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("RpcAuthCookie", expectToFail: true);


            }
            finally
            {
                DisposeClients(client, client2);
            }
        }

        private static void CheckSecure(string eventName, bool expectToFail = false)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://photon-forward.webscript.io/CheckSecure");
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(new {Check = eventName});

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

                    Assert.AreEqual(0, jsonResponse["ResultCode"]);
                    Assert.AreEqual(eventName, jsonResponse["Debug"]);
                    if (expectToFail)
                    {
                        Assert.IsFalse(jsonResponse.ContainsKey("Data"));
                    }
                    else
                    {
                        var expected = JsonConvert.DeserializeObject<Dictionary<string, object>>("{\"Param1\":1,\"Param2\":\"2\"}");
                        var got = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)jsonResponse["Data"]);

                        Assert.AreEqual(expected, got);
                    }
                }
            }
        }
    }
}
