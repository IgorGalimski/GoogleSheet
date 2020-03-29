using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Google.GData.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Data
{
    public class Spreadsheet : IEnumerable<Sheet>
    {
        public string ID { get; }
        public string Name { get; }

        public ICollection<Sheet> Sheets { get; private set; } = new List<Sheet>();

        public Spreadsheet(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public Sheet this[string sheetName]
        {
            get
            {
                return Sheets.FirstOrDefault(item => item.Name.Equals(sheetName));
            }
        }

        public async Task LoadGoogleSheets()
        {
            Sheets.Clear();

            var urlBuilder = URLBuilder.GetSheets(ID)
                .AddApiKey(GoogleDataStorage.Instance.ApiKey)
                .AddFields("sheets(properties(sheetId,title))");
            
            using (var response = await Utils.HttpClient.GetAsync(urlBuilder.GetURL()))
            {
                var content = await response.Content.ReadAsStringAsync();

                var jContainer = JsonConvert.DeserializeObject(content) as JContainer;

                var sheetInfo = from sheet in jContainer["sheets"].Children()["properties"]
                    select new { title = sheet["title"], id = sheet["sheetId"] };

                foreach (var sheet in sheetInfo)
                {
                    Sheets.Add(new Sheet(Convert.ToInt32(sheet.id.ToString()), sheet.title.ToString()));
                }
            }

            await ReadGoogleSheets();
        }

        public async Task CreateSheets(ICollection<string> names)
        {
            var urlBuilder = URLBuilder.BatchUpdate(ID);
            var value = AddSheetRequestBodyAdapter.GetAddSheetRequestBody(names);
            
            var responseHandler = new Action<string>(str =>
            {
                var jObject = JObject.Parse(str);
                var replies = jObject["replies"];
                var repliesArray = replies.Select(t => t);

                foreach (var addSheet in repliesArray)
                {
                    var properties = addSheet["addSheet"]["properties"];

                    var id = properties["sheetId"];
                    var title = properties["title"];
                        
                    var googleSheet = new Sheet(id.Value<int>(), title.Value<string>());
                    Sheets.Add(googleSheet);
                }
                
                Debug.Log(nameof(CreateSheets) + string.Concat(names, "\n"));
            });

            await RequestExecutor.SendRequestAsync(urlBuilder, value, responseHandler);
        }

        public async Task Clear()
        {
            var urlBuilder = URLBuilder.ClearSpreadsheets(ID);
            var value = ClearRequestBodyAdapter.GetClearRequestBody(Sheets);

            var responseHandler = new Action<string>(str =>
            {
                Debug.Log(nameof(Clear));

                foreach (var googleSheet in Sheets)
                {
                    googleSheet.Clear();
                }
            });

            await RequestExecutor.SendRequestAsync(urlBuilder, value, responseHandler);
        }

        public async Task DeleteGoogleSheets(ICollection<int> ids)
        {
            var urlBuilder = URLBuilder.BatchUpdate(ID);
            var value = AddSheetRequestBodyAdapter.GetDeleteSheetRequestBody(ids);

            var responseHandler = new Action<string>(str =>
            {
                Debug.Log(nameof(DeleteGoogleSheets));
                
                foreach (var id in ids)
                {
                    var googleSheet = Sheets.First(item => item.ID == id);
                    Sheets.Remove(googleSheet);
                }
            });

            await RequestExecutor.SendRequestAsync(urlBuilder, value, responseHandler);
        }

        private async Task ReadGoogleSheets()
        {
            var urlBuilder = URLBuilder.GetSheetsValues(ID).
                AddApiKey(GoogleDataStorage.Instance.ApiKey).
                AddRanges(Sheets.Select(item => item.Name)).
                AddValueRenderOption("FORMULA");

            using (var response = await Utils.HttpClient.GetAsync(urlBuilder.GetURL()))
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

                    Sheets.ElementAt(i).Parse(valuesToken);
                }
            }
        }

        public async Task Save()
        {
            var urlBuilder = URLBuilder.WriteMultipleRanges(ID)
                .AddApiKey(GoogleDataStorage.Instance.ApiKey)
                .AddValueInputOption("USER_ENTERED");
            var value = GoogleSpreadsheetAdapter.GetBatchRequestBody(this);

            var responseHandler = new Action<string>(str =>
            {
                Debug.Log(nameof(Save));
            });

            await RequestExecutor.SendRequestAsync(urlBuilder, value, responseHandler);
        }

        public IEnumerator<Sheet> GetEnumerator()
        {
            return Sheets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}