// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Global type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.GameServer
{
    #region using directives

    using System.Collections.Generic;
    using System.Net;

    #endregion

    public static class Global
    {
        public static readonly Dictionary<string, object> Games = new Dictionary<string, object>();
    
        public static bool TryParseIpEndpoint(string value, out IPEndPoint endPoint)
        {
            endPoint = null;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            var parts = value.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            IPAddress address;
            if (IPAddress.TryParse(parts[0], out address) == false)
            {
                return false;
            }

            int port;
            if (int.TryParse(parts[1], out port) == false)
            {
                return false;
            }

            endPoint = new IPEndPoint(address, port);
            return true;
        }
    }
}