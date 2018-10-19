using System.Collections.Generic;

namespace Photon.UnitTest.Utils.Basic
{
    public interface IAuthSchemeClient
    {
        string Token { get; set; }
        string UserId { get; }
    }

    public interface IAuthenticationScheme
    {
        void SetAuthenticateParameters(
            IAuthSchemeClient secretHolder,
            Dictionary<byte, object> requestParameters, Dictionary<byte, object> authParameter = null);

        void HandleAuthenticateResponse(IAuthSchemeClient secretHolder, Dictionary<byte, object> response);
    }
}