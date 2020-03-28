using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Data;
using Google.GData.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class GoogleDataProvider
    { 
        public ICollection<GoogleSpreadsheet> GoogleSpreadsheets { get; private set; } = new List<GoogleSpreadsheet>();

        public GoogleSpreadsheet this[string spreadsheetName]
        {
            get { return GoogleSpreadsheets.FirstOrDefault(item => item.Name.Equals(spreadsheetName)); }
        }

        public async Task CreateSpreadsheet(string spreadsheetName)
        {
            var urlBuilder = URLBuilder.CreateSpreadSheet();
            var spreadsheetCreateRequest = new SpreadsheetCreateRequest(spreadsheetName);
            var responseHandler = new Action<string>(str =>
            {
                Debug.Log(nameof(CreateSpreadsheet) + spreadsheetName);
            });

            await RequestExecutor.SendRequestAsync(urlBuilder, spreadsheetCreateRequest, responseHandler);
        }
        
        public async Task LoadSpreadsheets()
        {
            await GoogleDataStorage.Instance.RefreshAccessTokenIfExpires();
            
            GoogleSpreadsheets.Clear();

            var urlBuilder = URLBuilder.GetSpreadsheets().AddOrderBy("createdTime").
                            AddRequest("mimeType = 'application/vnd.google-apps.spreadsheet' and 'me' in owners and trashed = false");

            Utils.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GoogleDataStorage.Instance.AccessToken);
            using (var response = await Utils.HttpClient.GetAsync(urlBuilder.GetURL()))
            {
                var content = await response.Content.ReadAsStringAsync();
                
                var jContainer = JsonConvert.DeserializeObject(content) as JContainer;

                foreach (var jToken in jContainer)
                {
                    var jProperty = jToken as JProperty;

                    if (jProperty.Name == "files")
                    {
                        foreach (var file in jProperty.Values())
                        {
                            var fileInfo = file as JObject;

                            GoogleSpreadsheets.Add(new GoogleSpreadsheet(fileInfo["id"].ToString(), fileInfo["name"].ToString()));
                        }
                    }
                }
            }
        }
    }
}