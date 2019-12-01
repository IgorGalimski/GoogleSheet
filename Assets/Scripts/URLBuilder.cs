using System.Collections.Generic;
using System.Text;

namespace Google.GData.Client
{
    //TODO Replace it
    public class URLBuilder
    {
        private const string CHECK_TOKEN_EXPIRES = "https://www.googleapis.com/oauth2/v1/tokeninfo?";
        private const string UPDATE_ACCESS_TOKEN =
            "https://www.googleapis.com/oauth2/v4/token?client_id=CLIENT_ID&client_secret=CLIENT_SECRET&refresh_token=REFRESH_TOKEN&";

        private readonly StringBuilder _url; 

        private bool _isAnyFieldAdded;

        public static URLBuilder CheckTokenExpires()
        {
            return new URLBuilder(CHECK_TOKEN_EXPIRES);
        }

        public static URLBuilder UpdateAccessToken()
        {
            return new URLBuilder(UPDATE_ACCESS_TOKEN);
        }

        public URLBuilder(string baseUrl)
        {
            _url = new StringBuilder(baseUrl);
        }
        
        public void AddSpreadSheetId(string spreadsheetId)
        {
            //_url = _url.Replace("SPREADSHEET_ID", spreadsheetId);
        }

        public void AddRange(string range)
        {
            //_url = _url.Replace("RANGE", range);
        }
        
        public void AddRanges(IEnumerable<string> ranges)
        {
            foreach (string range in ranges)
            {
                //_url = $"{_url}&ranges={range}";
            }
        }

        public void AddFields(string fields)
        {
            //_url = _url.Replace("FIELDS", fields);
        }
        
        public void AddValueInputOption(string valueInputOption)
        {
            //_url = _url.Replace("VALUE_INPUT_OPTION", valueInputOption);
        }
        
        public void AddValueRenderOption(string valueRenderOption)
        {
            //_url = _url.Replace("VALUE_RENDER_OPTION", valueRenderOption);
        }
        
        public void AddKey(string key)
        {
            //_url = _url.Replace("KEY", key);
        }

        public void AddFileId(string fileId)
        {
            //_url = _url.Replace("FILE_ID", fileId);
        }

        public URLBuilder AddAccessToken(string accessToken)
        {
            _url.Append("access_token=" + accessToken + "&");

            return this;
        }

        public URLBuilder AddClientId(string clientId)
        {
            _url.Append("clientId=" + clientId + "&");

            return this;
        }
        
        public URLBuilder AddClientSecret(string clientSecret)
        {
            _url.Append("client_secret=" + clientSecret + "&");

            return this;
        }

        public URLBuilder AddRefreshToken(string refreshToken)
        {
            _url.Append("refresh_token=" + refreshToken + "&");

            return this;
        }

        public void AddOrder(string order)
        {
            //_url = _url.Replace("ORDER_BY", order);
        }

        public void AddRequest(string request)
        {
            //_url = _url.Replace("REQUEST", request);
        }

        public string GetURL()
        {
            return _url.ToString();
        }

        public URLBuilder AddGrantType(string grantType)
        {
            _url.Append("grant_type=" + grantType + "&");
            
            return this;
        }
    }
}