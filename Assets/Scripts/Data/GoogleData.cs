using System;

namespace DefaultNamespace
{
    public class GoogleData
    {
        public string Email { get; set; }
        public string ApiKey { get; set; }
        public string CliendId { get; set; }
        public string CliendSecret { get; set; }
        public string RefreshToken { get; set; }
        public string AccessCode { get; set; }
        public string AccessToken { get; set; }

        public bool IsValid()
        {
            return RefreshToken != string.Empty;
        }

        public override string ToString()
        {
            return string.Format(
                "ClientId: {0}" + Environment.NewLine +
                "ClientSecret: {1}" + Environment.NewLine +
                "AccessCode: {2}" + Environment.NewLine +
                "AccessToken: {3}" + Environment.NewLine +
                "ApiKey: {4}" + Environment.NewLine +
                "Email: {5}" + Environment.NewLine +
                "RefreshToken: {6}",
                CliendId,  
                CliendSecret,
                AccessCode,
                AccessToken,
                ApiKey,
                Email,
                RefreshToken);
        }
    }
}