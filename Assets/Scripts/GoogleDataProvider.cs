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
        public ICollection<Spreadsheet> Spreadsheets { get; } = new List<Spreadsheet>();
        
        public Task Init() => GoogleDataStorage.Instance.RefreshAccessToken();

        public Spreadsheet this[string spreadsheetName]
        {
            get { return Spreadsheets.FirstOrDefault(item => item.Name.Equals(spreadsheetName)); }
        }

        public async Task CreateSpreadsheet(string spreadsheetName)
        {
            var urlBuilder = URLBuilder.CreateSpreadSheet();
            var spreadsheetCreateRequest = new SpreadsheetCreateRequest(spreadsheetName);
            var responseHandler = new Action<string>(str =>
            {
                Debug.Log(nameof(CreateSpreadsheet) + spreadsheetName);
                
                var jContainer = JsonConvert.DeserializeObject(str) as JContainer;
                var id = jContainer["spreadsheetId"];

                Spreadsheets.Add(new Spreadsheet(id.ToString(), spreadsheetName));
            });

            await RequestExecutor.SendRequestAsync(urlBuilder, spreadsheetCreateRequest, responseHandler);
        }
        
        public async Task LoadSpreadsheets()
        {
            Spreadsheets.Clear();

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

                            Spreadsheets.Add(new Spreadsheet(fileInfo["id"].ToString(), fileInfo["name"].ToString()));
                        }
                    }
                }
            }
        }
    }
}