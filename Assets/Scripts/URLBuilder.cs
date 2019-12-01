using System.Collections.Generic;

namespace Google.GData.Client
{
    //TODO Replace it
    public class URLBuilder
    {
        private string _url;
            
        public URLBuilder(string url)
        {
            _url = url;
        }
        
        public void AddSpreadSheetId(string spreadsheetId)
        {
            _url = _url.Replace("SPREADSHEET_ID", spreadsheetId);
        }

        public void AddRange(string range)
        {
            _url = _url.Replace("RANGE", range);
        }
        
        public void AddRanges(IEnumerable<string> ranges)
        {
            foreach (string range in ranges)
            {
                _url = $"{_url}&ranges={range}";
            }
        }

        public void AddFields(string fields)
        {
            _url = _url.Replace("FIELDS", fields);
        }
        
        public void AddValueInputOption(string valueInputOption)
        {
            _url = _url.Replace("VALUE_INPUT_OPTION", valueInputOption);
        }
        
        public void AddValueRenderOption(string valueRenderOption)
        {
            _url = _url.Replace("VALUE_RENDER_OPTION", valueRenderOption);
        }
        
        public void AddKey(string key)
        {
            _url = _url.Replace("KEY", key);
        }

        public void AddFileId(string fileId)
        {
            _url = _url.Replace("FILE_ID", fileId);
        }

        public void AddAccessToken(string accessToken)
        {     
            _url = _url.Replace("ACCESS_TOKEN", accessToken);
        }

        public void AddCliendId(string clientId)
        {
            _url = _url.Replace("CLIENT_ID", clientId); 
        }
        
        public void AddCliendSecret(string clientSecret)
        {
            _url = _url.Replace("CLIENT_SECRET", clientSecret); 
        }

        public void AddRefreshToken(string refreshToken)
        {
            _url = _url.Replace("REFRESH_TOKEN", refreshToken);
        }

        public void AddOrder(string order)
        {
            _url = _url.Replace("ORDER_BY", order);
        }

        public void AddRequest(string request)
        {
            _url = _url.Replace("REQUEST", request);
        }

        public string GetURL()
        {
            return _url;
        }
    }
}