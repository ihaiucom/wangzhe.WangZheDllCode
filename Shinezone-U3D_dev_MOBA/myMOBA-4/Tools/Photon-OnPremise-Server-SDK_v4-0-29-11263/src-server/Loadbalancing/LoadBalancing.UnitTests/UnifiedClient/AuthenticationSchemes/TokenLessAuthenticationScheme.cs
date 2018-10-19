using Photon.UnitTest.Utils.Basic;

namespace Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes
{
    using System.Collections.Generic;

    public class TokenLessAuthenticationScheme : IAuthenticationScheme
    {
        public virtual void SetAuthenticateParameters(IAuthSchemeClient client, Dictionary<byte, object> requestParameters, Dictionary<byte, object> authParameter = null)
        {
            if (authParameter != null)
            {
                foreach (var p in authParameter)
                {
                    requestParameters[p.Key] = p.Value;
                }
            }
        }

        public virtual void HandleAuthenticateResponse(IAuthSchemeClient nunitClient, Dictionary<byte, object> response)
        {
            // do nothing with return values
        }
    }
}
