using System.Reflection;
using System.Threading;
using ExitGames.Logging;
using NUnit.Framework;
using Photon.Common.Authentication.Configuration.Auth;
using Photon.LoadBalancing.MasterServer;
using Photon.LoadBalancing.UnifiedClient;
using Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes;
using Photon.LoadBalancing.UnitTests.UnifiedServer.OfflineExtra;
using Photon.SocketServer;
using Photon.SocketServer.UnitTesting;
using Photon.UnitTest.Utils.Basic;
using Photon.UnitTest.Utils.Basic.NUnitClients;

namespace Photon.LoadBalancing.UnitTests.UnifiedServer.Policy
{

    public class OfflineConnectPolicy : LBConnectPolicyBase
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private PhotonApplicationProxy<MasterApplication> masterServer;
        private PhotonApplicationProxy<TestApplication> gameServer;
        protected PhotonApplicationHoster photonHost;

        protected string configFileName = "Photon.LoadBalancing.UnitTests.dll.config";

        #endregion

        #region Constructors

        public OfflineConnectPolicy()
        {
        }

        public OfflineConnectPolicy(IAuthenticationScheme scheme, string configFileName = "")
        {
            this.AuthenticatonScheme = scheme;

            if (!string.IsNullOrEmpty(configFileName))
            {
                this.configFileName = configFileName;
            }
        }

        #endregion

        #region Properties

        protected virtual PhotonApplicationProxy MasterServer
        {
            get { return this.masterServer; }
        }

        protected virtual PhotonApplicationProxy GameServer
        {
            get { return this.gameServer; }
        }

        public override bool IsOffline
        {
            get { return true; }
        }

        public override bool IsInited
        {
            get { return (this.photonHost != null); }
        }

        #endregion

        #region Publics

        public override UnifiedClientBase CreateTestClient()
        {
            return new UnifiedTestClient(new OfflineNUnitClient(ConnectPolicy.WaitTime, this), this.AuthenticatonScheme);
        }

        public override bool Setup()
        {
            log.InfoFormat("Policy Setup");

            this.PreStartChecks();
            this.photonHost = new PhotonApplicationHoster();

            this.InitApplications();

            return true;
        }

        public override void ConnectToServer(INUnitClient client, string address)
        {
            ((OfflineNUnitClient) client).Connect(this.GetProxyByAddress(address));
        }

        public override void TearDown()
        {
            log.InfoFormat("Policy TearDown");
            Thread.Sleep(1500);

            this.CloseApplications();

            if (this.photonHost != null)
            {
                this.photonHost.Dispose();
                this.photonHost = null;
            }
        }

        #endregion

        #region Methods

        protected virtual void PreStartChecks()
        {
            if (this.AuthenticatonScheme.GetType() == typeof (TokenLessAuthenticationScheme))
            {
                if (AuthSettings.Default.Enabled)
                {
                    Assert.Ignore("Autentication enabled (AuthSettings Enabled=true) in Photon.LoadBalacing.config. Disable to run this tests");
                }
            }
            else
            {
                if (!AuthSettings.Default.Enabled)
                {
                    Assert.Ignore("Autentication disabled (AuthSettings Enabled=false) in Photon.LoadBalacing.config. Enable to run this tests");
                }
            }
        }

        protected virtual void InitApplications()
        {
            var codebase = Assembly.GetExecutingAssembly().CodeBase;
            var approotPath = codebase.Substring(0, codebase.IndexOf("LoadBalancing.UnitTests/bin/Debug/"));
            if (approotPath.StartsWith("file:///"))
            {
                approotPath = approotPath.Substring("file:///".Length);
            }

            this.masterServer = this.photonHost.AddApplication<MasterApplication>(
                MasterServerAppName, "127.0.0.1", 4530, this.configFileName, approotPath);
            this.photonHost.AddListenerToApplication(this.MasterServer, "127.0.0.1", 4520);

            this.gameServer = this.photonHost.AddApplication<TestApplication>(
                GameServerAppName, "127.0.0.1", 4531, this.configFileName, approotPath);

            this.masterServer.Start();
            this.gameServer.Start();

            // give the applications some time to connect to other servers
            Thread.Sleep(100);
        }

        protected static void StopServer<T>(ref PhotonApplicationProxy<T> server) where T : ApplicationBase
        {
            if (server != null)
            {
                server.Stop();
                server = null;
            }
        }

        protected virtual void CloseApplications()
        {
            StopServer(ref this.gameServer);
            StopServer(ref this.masterServer);
        }

        private PhotonApplicationProxy GetProxyByAddress(string address)
        {
            if (this.photonHost == null)
            {
                return null;
            }

            var parts = address.Split(':');

            int port = int.Parse(parts[1]);

            PhotonApplicationProxy proxy;

            this.photonHost.TryGetApplicationProxy(parts[0], port, out proxy);
            Assert.IsNotNull(proxy, "Proxy for address '{0}:{1}' is not found", parts[0], port);
            return proxy;
        }

        #endregion
    }
}