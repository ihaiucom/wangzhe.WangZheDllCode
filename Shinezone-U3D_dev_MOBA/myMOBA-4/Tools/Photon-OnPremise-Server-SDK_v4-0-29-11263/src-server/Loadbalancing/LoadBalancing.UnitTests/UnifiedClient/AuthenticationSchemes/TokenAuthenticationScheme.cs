using Photon.UnitTest.Utils.Basic;

namespace Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes
{
    using System.Collections.Generic;

    using ExitGames.Client.Photon.LoadBalancing;


    public class TokenAuthenticationScheme : TokenLessAuthenticationScheme
    {
        public override void SetAuthenticateParameters(IAuthSchemeClient client, 
            Dictionary<byte, object> requestParameters, Dictionary<byte, object> authParameter = null)
        {
            if (authParameter == null)
            {
                authParameter = new Dictionary<byte, object>();
            }

            if (!string.IsNullOrEmpty(client.Token))
            {
                // we already have a token (from nameserver / master) - just use that
                if (!authParameter.ContainsKey(ParameterCode.Secret))
                {
                    authParameter[ParameterCode.Secret] = client.Token;
                }
            }
            else
            {
                // no token yet - pass UserId into authentication: 
                if (!authParameter.ContainsKey(ParameterCode.UserId))
                {
                    authParameter[ParameterCode.UserId] = client.UserId; 
                }
            }

            base.SetAuthenticateParameters(client, requestParameters, authParameter);
        }

        public override void HandleAuthenticateResponse(IAuthSchemeClient client, Dictionary<byte, object> response)
        {
            // Master: returns a secret - store and re-use it! 
            // GS: does not return a secret. 
            if (response.ContainsKey(ParameterCode.Secret))
            {
                client.Token = (string)response[ParameterCode.Secret];    
            }
        }
    }
}
