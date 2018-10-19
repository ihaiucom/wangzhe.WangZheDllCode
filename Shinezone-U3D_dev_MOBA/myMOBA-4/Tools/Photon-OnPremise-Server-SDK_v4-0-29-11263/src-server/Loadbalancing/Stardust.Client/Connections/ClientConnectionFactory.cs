// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientConnectionFactory.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.Connections
{
    public class ClientConnectionFactory
    {
        public static ClientConnection GetClientConnection(string application, string gameName, int number)
        {
            switch (application.ToLower())
            {
                case "master":
                    return new LoadBalancingClientConnection(gameName, number);

                default:
                    return new LiteClientConnection(gameName, number); 
            }
        }
    }
}
