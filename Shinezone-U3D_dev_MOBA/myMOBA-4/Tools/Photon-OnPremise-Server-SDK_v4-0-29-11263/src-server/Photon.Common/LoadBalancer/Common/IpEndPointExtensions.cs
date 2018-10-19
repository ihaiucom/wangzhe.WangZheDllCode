using System.Net;

namespace Photon.Common.LoadBalancer.Common
{
    public static class IpEndPointExtensions
    {
        public static bool TryParseIpEndpoint(this string endPointString, out IPEndPoint endPoint)
        {
            endPoint = null;

            if (string.IsNullOrEmpty(endPointString))
            {
                return false;
            }

            var endPointParts = endPointString.Split(':');
            if (endPointParts.Length != 2)
            {
                return false;
            }

            IPAddress address;
            if (IPAddress.TryParse(endPointParts[0], out address) == false)
            {
                return false;
            }

            int port;
            if (int.TryParse(endPointParts[1], out port) == false)
            {
                return false;
            }

            endPoint = new IPEndPoint(address, port);
            return true;
        }
    }
}
