using System;
using System.Collections.Generic;
using Photon.LoadBalancing.UnifiedClient;
using Photon.LoadBalancing.UnitTests.UnifiedServer.Policy;
using Photon.UnitTest.Utils.Basic;

namespace Photon.LoadBalancing.UnitTests.UnifiedServer
{
    public class LoadBalancingUnifiedTestsBase : UnifiedTestsBase
    {
        public string MasterAddress
        {
            get
            {
                return ((LBConnectPolicyBase) this.connectPolicy).MasterServerAddress;
            }
        }
        public string GameServerAddress
        {
            get
            {
                return ((LBConnectPolicyBase)this.connectPolicy).GameServerAddress;
            }
        }

        public bool UsePlugins { get; set; }

        protected string Player1 = "Player1";

        protected string Player2 = "Player2";

        protected string Player3 = "Player3";

        public LoadBalancingUnifiedTestsBase(ConnectPolicy policy) : base(policy)
        {
            UsePlugins = true;
        }

        /// <summary>
        /// Creates a TestClientBase and connects to the master server.
        /// Sends an Authenticate request after connection is completed. The TestClientBase's IAuthenticationScheme determines which parameters are used for Authenticate. 
        /// </summary>
        public virtual UnifiedTestClient CreateMasterClientAndAuthenticate(string userId = null, Dictionary<byte, object> authParameter = null)
        {
            var client = (UnifiedTestClient)this.CreateTestClient();
            client.UserId = userId;

            this.ConnectAndAuthenticate(client, this.MasterAddress, userId, authParameter);
            return client;
        }

        public virtual void ConnectAndAuthenticate(UnifiedTestClient client, string address, Dictionary<byte, object> authParameter = null)
        {
            this.ConnectAndAuthenticate(client, address, client.UserId, authParameter);
        }

        public virtual void ConnectAndAuthenticate(UnifiedTestClient client, string address, string userName, Dictionary<byte, object> authParameter = null, bool reuseToken  = false)
        {
            if (client.Connected)
            {
                client.Disconnect();
            }

            if (!reuseToken && address == this.MasterAddress)
            {
                client.Token = String.Empty;
            }

            client.OperationResponseQueueClear();
            client.EventQueueClear();

            this.ConnectToServer(client, address);

            if (authParameter == null)
            {
                authParameter = new Dictionary<byte, object>();
            }

            client.Authenticate(userName, authParameter);
        }

        protected void ConnectToServer(UnifiedClientBase client, string address)
        {
            client.Connect(address);
        }
    }
}
