using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using ExitGames.Logging;
using Photon.SocketServer.Security;

namespace Photon.Common.Authentication
{
    public class AuthTokenFactory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ICryptoProvider CryptoProvider { get; private set; }

        public byte[] SharedKey { get; private set; }

        public TimeSpan ExpirationTime { get; set; }

        public void Initialize(string secret, TimeSpan expirationtime)
        {
            var sharedKey = System.Text.Encoding.Default.GetBytes(secret);
            this.ExpirationTime = expirationtime;

            byte[] shaHash;
            using (SHA256 hashProvider = SHA256.Create())
            {
                shaHash = hashProvider.ComputeHash(sharedKey);
            }

            this.SharedKey = shaHash;
            this.CryptoProvider = new RijndaelCryptoProvider(shaHash, PaddingMode.PKCS7);
        }

        protected virtual void SetupToken(AuthenticationToken token)
        {
            token.ValidToTicks = DateTime.UtcNow.Add(this.ExpirationTime).Ticks;
            token.SessionId = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        }

        public virtual AuthenticationToken CreateAuthenticationToken(IAuthenticateRequest authRequest, 
            AuthSettings authSettings, 
            string userId, 
            Dictionary<string, object> authCookie)
        {
            var token = new AuthenticationToken
                            {
                                UserId = userId,
                                AuthCookie = authCookie,
                                Flags = authRequest.Flags,
                            };
            this.SetupToken(token);
            return token;
        }

        /// <summary>
        /// Create a renewed Authentication Token on Master server - to be validated on GS
        /// </summary>
        /// <param name="userId"> </param>
        /// <param name="authRequest"></param>
        /// <returns></returns>
        public AuthenticationToken CreateAuthenticationToken(string userId, IAuthenticateRequest authRequest)
        {
            var token = new AuthenticationToken
                            {
                                ValidToTicks = DateTime.UtcNow.Add(this.ExpirationTime).Ticks,
                                UserId = userId,
                                AuthCookie = new Dictionary<string, object>(),
                                SessionId = Guid.NewGuid().ToString(),
                                Flags = authRequest.Flags,
                            };
            return token;
        }
       
        public virtual string EncryptAuthenticationToken(AuthenticationToken token, bool renew)
        {
            if (renew)
            {
                token.ValidToTicks = DateTime.UtcNow.Add(this.ExpirationTime).Ticks;
            }

            var tokenData = token.Serialize();
            tokenData = this.CryptoProvider.Encrypt(tokenData);
            return Convert.ToBase64String(tokenData);
        }

        public virtual string CreateEncryptedAuthenticationToken(IAuthenticateRequest authRequest, AuthSettings authSettings, string userId)
        {
            var token = this.CreateAuthenticationToken(authRequest, authSettings, userId, null);
            return this.EncryptAuthenticationToken(token, false);
        }

        public virtual string CreateEncryptedAuthenticationToken(IAuthenticateRequest authRequest, AuthSettings authSettings, string userId, Dictionary<string, object> customParameter)
        {
            var token = this.CreateAuthenticationToken(authRequest, authSettings, userId, customParameter);
            return this.EncryptAuthenticationToken(token, false);
        }

        public virtual bool DecryptAuthenticationToken(string authTokenEncrypted, out AuthenticationToken authToken)
        {
            try
            {
                var tokenData = Convert.FromBase64String(authTokenEncrypted);
                tokenData = this.CryptoProvider.Decrypt(tokenData);
                return AuthenticationToken.TryDeserialize(tokenData, out authToken);
            }
            catch (Exception ex)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("DecryptAuthenticationToken failed: {0}", ex);
                }

                authToken = null;
                return false;
            }
        }
    }
}
