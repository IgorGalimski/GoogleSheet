using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DefaultNamespace;
using Google.GData.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Data
{
    public class GoogleSpreadsheet : IEnumerable<GoogleSheet>
    {
        private readonly GoogleDataStorage _googleDataStorage;
        
        public string ID { get; }
        public string Name { get; }

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
            
            using (var response = await Utils.HttpClient.GetAsync(urlBuilder.GetURL()))
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

        public async Task CreateGoogleSheets(ICollection<string> names)
        {
            var urlBuilder = URLBuilder.BatchUpdate(ID);
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _googleDataStorage.AccessToken);

            var json = JsonConvert.SerializeObject(AddSheetRequestBodyAdapter.GetAddSheetRequestBody(names));
            
            var content = new StringContent(json);

            using (var response = await httpClient.PostAsync(urlBuilder.GetURL(), content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    
                    var jObject = JObject.Parse(str);
                    var replies = jObject["replies"];
                    var repliesArray = replies.Select(t => t);

                    foreach (var addSheet in repliesArray)
                    {
                        var properties = addSheet["addSheet"]["properties"];

                        var id = properties["sheetId"];
                        var title = properties["title"];
                        
                        var googleSheet = new GoogleSheet(id.Value<int>(), title.Value<string>());
                        GoogleSheets.Add(googleSheet);
                    }
                }
            }
        }

        public async Task Clear()
        {
            var urlBuilder = URLBuilder.ClearSpreadsheets(ID);
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _googleDataStorage.AccessToken);
            
            var content = new StringContent(JsonConvert.SerializeObject(ClearRequestBodyAdapter.GetClearRequestBody(GoogleSheets)));

            using (var response = await httpClient.PostAsync(urlBuilder.GetURL(), content))
            {
                Debug.LogError(response.StatusCode.ToString());
            }
        }

        public async Task DeleteGoogleSheets(ICollection<int> ids)
        {
            var urlBuilder = URLBuilder.BatchUpdate(ID);
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _googleDataStorage.AccessToken);

            var json = JsonConvert.SerializeObject(AddSheetRequestBodyAdapter.GetDeleteSheetRequestBody(ids));
            
            var content = new StringContent(json);

            using (var response = await httpClient.PostAsync(urlBuilder.GetURL(), content))
            {
                if (response.IsSuccessStatusCode)
                {
                    foreach (var id in ids)
                    {
                        var googleSheet = GoogleSheets.First(item => item.ID == id);
                        GoogleSheets.Remove(googleSheet);
                    }
                }
            }
        }

        private async Task ReadGoogleSheets()
        {
            var urlBuilder = URLBuilder.GetSheetsValues(ID).
                AddApiKey(_googleDataStorage.ApiKey).
                AddRanges(GoogleSheets.Select(item => item.Name)).
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

                    GoogleSheets.ElementAt(i).Parse(valuesToken);
                }
            }
        }

        public async Task Save()
        {
            var urlBuilder = URLBuilder.WriteMultipleRanges(ID)
                .AddApiKey(_googleDataStorage.ApiKey)
                .AddValueInputOption("USER_ENTERED");

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _googleDataStorage.AccessToken);

            var batchRequestBody = JsonConvert.SerializeObject(GoogleSpreadsheetAdapter.GetBatchRequestBody(this));

            var content = new StringContent(batchRequestBody);
            
            var response = await httpClient.PostAsync(urlBuilder.GetURL(), content);
            Debug.Log("Save: " + response.StatusCode);
        }

        public IEnumerator<GoogleSheet> GetEnumerator()
        {
            return GoogleSheets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}