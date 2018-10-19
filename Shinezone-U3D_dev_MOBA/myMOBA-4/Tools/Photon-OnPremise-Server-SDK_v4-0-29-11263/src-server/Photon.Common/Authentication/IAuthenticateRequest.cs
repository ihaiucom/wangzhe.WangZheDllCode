
namespace Photon.Common.Authentication
{
    public interface IAuthenticateRequest
    {
        string ApplicationId { get; }

        string ApplicationVersion { get; }

        string Token { get; set; }

        string UserId { get; set; }

        string Region { get; set; }

        byte ClientAuthenticationType { get; }

        string ClientAuthenticationParams { get; }

        object ClientAuthenticationData { get; }

        int Flags { get; }
    }
}
