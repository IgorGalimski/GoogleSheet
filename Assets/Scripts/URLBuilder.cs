using System.Collections.Generic;
using System.Text;

namespace Google.GData.Client
{
    //TODO Replace it
    public class URLBuilder
    {
        private const string CHECK_TOKEN_EXPIRES = "https://www.googleapis.com/oauth2/v1/tokeninfo?";
        private const string UPDATE_ACCESS_TOKEN =
            "https://www.googleapis.com/oauth2/v4/token?";
        
        private const string GET_SHEETS = "https://sheets.googleapis.com/v4/spreadsheets/";
        
        private const string GET_SHEETS_VALUES =
            "https://sheets.googleapis.com/v4/spreadsheets/";

        private const string GET_SPREADSHEETS = "https://www.googleapis.com/drive/v3/files?";

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

        public static URLBuilder GetSheets(string spreadsheetId)
        {
            return new URLBuilder(GET_SHEETS + spreadsheetId + "?");
        }

        public static URLBuilder GetSheetsValues(string spreadsheetId)
        {
            return new URLBuilder(GET_SHEETS_VALUES + spreadsheetId + "/values:batchGet?");
        }

        public static URLBuilder GetSpreadsheets()
        {
            return new URLBuilder(GET_SPREADSHEETS);
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
        
        public URLBuilder AddRanges(IEnumerable<string> ranges)
        {
            foreach (var range in ranges)
            {
                _url.Append("ranges=" + range + "&");
            }

            return this;
        }

        public void AddValueInputOption(string valueInputOption)
        {
            //_url = _url.Replace("VALUE_INPUT_OPTION", valueInputOption);
        }
        
        public URLBuilder AddValueRenderOption(string valueRenderOption)
        {
            _url.Append("valueRenderOption=" + valueRenderOption + "&");

            return this;
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

        public URLBuilder AddApiKey(string apiKey)
        {
            _url.Append("key=" + apiKey + "&");
            
            return this;
        }

        public URLBuilder AddFields(string fields)
        {
            _url.Append("fields=" + fields + "&");
            
            return this;
        }

        public URLBuilder AddOrderBy(string orderBy)
        {
            _url.Append("orderBy=" + orderBy + "&");
            
            return this;
        }

        public URLBuilder AddRequest(string request)
        {
            _url.Append("q=" + request + "&");

            return this;
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