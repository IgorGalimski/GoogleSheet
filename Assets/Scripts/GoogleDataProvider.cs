using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Data;
using Google.GData.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    public class GoogleDataProvider : MonoBehaviour
    {
        private GoogleDataStorage _googleDataProvider;
        private readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler(), false);

        public async void Start()
        {
            await Init();
            var result = await GetAllSheets("1Mli-eOVgCKh_adOqzVnOK9VYuuxCaPp29wTB45GEoLg");

            foreach (var VARIABLE in result)
            {
                Debug.LogError(VARIABLE.ID + " " + VARIABLE.Name);
            }
        }

        public async Task Init()
        {
            var loadOperation = Resources.LoadAsync<GoogleDataStorage>("GoogleDataStorage");
            while (!loadOperation.isDone)
            {
                await Task.Yield();
            }

            _googleDataProvider = (GoogleDataStorage) loadOperation.asset;
        }

        public async Task<ICollection<GoogleSheet>> GetAllSheets(string spreadsheetId)
        {
            await _googleDataProvider.RefreshAccessTokenIfExpires();

            var urlBuilder = URLBuilder.GetSheets(spreadsheetId)
                .AddApiKey(_googleDataProvider.ApiKey)
                .AddFields("sheets(properties(sheetId,title))");

            var collection = new List<GoogleSheet>();

            using (var response = await _httpClient.GetAsync(urlBuilder.GetURL()))
            {
                var content = await response.Content.ReadAsStringAsync();

                var jContainer = JsonConvert.DeserializeObject(content) as JContainer;

                var sheetInfo = from sheet in jContainer["sheets"].Children()["properties"]
                    select new { title = sheet["title"], id = sheet["sheetId"] };

                foreach (var sheet in sheetInfo)
                {
                    collection.Add(new GoogleSheet(Convert.ToInt32(sheet.id.ToString()), sheet.title.ToString()));
                }
            }
            
            return collection;
        }
    }
}