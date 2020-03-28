using System;

namespace Data
{
    [Serializable]
    public class Properties
    {
        public string title;

        public Properties(string title)
        {
            this.title = title;
        }
    }
    
    [Serializable]
    public class SpreadsheetCreateRequest
    {
        public Properties properties;

        public SpreadsheetCreateRequest(string name)
        {
            properties = new Properties(name);
        }
    }
}