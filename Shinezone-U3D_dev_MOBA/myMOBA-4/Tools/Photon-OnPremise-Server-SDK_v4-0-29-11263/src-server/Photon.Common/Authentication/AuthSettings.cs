
namespace Photon.Common.Authentication
{
    /// <summary>
    /// Represents the result from the authenticaton handler (i.e., from the account service). 
    /// </summary>
    public class AuthSettings 
    {
        public bool IsAnonymousAccessAllowed { get; set; }
    }
}
