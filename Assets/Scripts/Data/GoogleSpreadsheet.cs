using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.GData.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Data
{
    public class GoogleSpreadsheet
    {
        private readonly GoogleDataStorage _googleDataStorage;
        
        public string ID { get; private set; }
        public string Name { get; private set; }

        public ICollection<GoogleSheet> GoogleSheets { get; private set; } = new List<GoogleSheet>();

        public GoogleSpreadsheet(string id, string name, GoogleDataStorage googleDataStorage)
        {
            ID = id;
            Name = name;
            _googleDataStorage = googleDataStorage;
        }

        public GoogleSheet this[string sheetName]
        {
            get
            {
                return GoogleSheets.FirstOrDefault(item => item.Name.Equals(sheetName));
            }
        }

        public async Task LoadGoogleSheets()
        {
            GoogleSheets.Clear();
            
            await _googleDataStorage.RefreshAccessTokenIfExpires();

            var urlBuilder = URLBuilder.GetSheets(ID)
                .AddApiKey(_googleDataStorage.ApiKey)
                .AddFields("sheets(properties(sheetId,title))");
            
            using (var response = await new HttpClient().GetAsync(urlBuilder.GetURL()))
            {
                var content = await response.Content.ReadAsStringAsync();

                var jContainer = JsonConvert.DeserializeObject(content) as JContainer;

                var sheetInfo = from sheet in jContainer["sheets"].Children()["properties"]
                    select new { title = sheet["title"], id = sheet["sheetId"] };

                foreach (var sheet in sheetInfo)
                {
                    GoogleSheets.Add(new GoogleSheet(Convert.ToInt32(sheet.id.ToString()), sheet.title.ToString()));
                }
            }

            await ReadGoogleSheets();
        }

        public async Task ReadGoogleSheets()
        {
            var urlBuilder = URLBuilder.GetSheetsValues(ID).
                AddApiKey(_googleDataStorage.ApiKey).
                AddRanges(GoogleSheets.Select(item => item.Name)).
                AddValueRenderOption("FORMULA");

            using (var response = await new HttpClient().GetAsync(urlBuilder.GetURL()))
            {
                var content = await response.Content.ReadAsStringAsync();

                var jObject = JObject.Parse(content);

                var valueRanges = jObject["valueRanges"].Select(t => t);

                for (int i = 0; i < valueRanges.Count(); i++)
                {
                    var range = valueRanges.ElementAt(i);
                    
                    if (range["values"] == null) 
                        continue;
                    
                    var valuesToken = range["values"].Select(t => t);

                    GoogleSheets.ElementAt(i).Parse(valuesToken);
                }
            }
        }
    }
}