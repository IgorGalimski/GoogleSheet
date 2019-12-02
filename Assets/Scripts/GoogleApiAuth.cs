using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Google.GData.Client;
using UnityEngine;

namespace DefaultNamespace
{
    public class GoogleApiAuth
    {
        private const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
        private const string SCOPE = "https://www.googleapis.com/auth/spreadsheets";

        private readonly string _clientId;
        private readonly string _clientSecret;
        
        private OAuth2Parameters _parameters;
        
        public GoogleApiAuth(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }
        
        public void GenerateAccessCode()
        {
            try
            {
                _parameters = new OAuth2Parameters
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    Scope = SCOPE,
                    RedirectUri = REDIRECT_URI,
                    AccessType = "offline",
                    TokenType = "Bearer"
                };

                var authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(_parameters);
                Application.OpenURL(authorizationUrl);
            }
            catch (Exception exception)
            {
                Debug.LogWarningFormat("GenerateAccessCode exception: {0}", exception.Message);
            }
        }

        public (string accessToken, string refreshToken) GenerateAccessToken(string accessCode)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

                _parameters.AccessCode = accessCode;

                OAuthUtil.GetAccessToken(_parameters);

                return (_parameters.AccessToken, _parameters.RefreshToken);
            }
            catch (Exception exception)
            {
                Debug.LogWarningFormat("GenerateAccesToken exception: {0}", exception.Message);
            }

            return (null, null);
        }

        private bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var isOk = true;
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (var i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        var chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
    }
}