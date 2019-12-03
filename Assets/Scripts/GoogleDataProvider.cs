using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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

            var googleSpreadsheets = await GetAllSpreadsheets();
            foreach (var VARIABLE in googleSpreadsheets)
            {
                Debug.LogError(VARIABLE.ID + " " + VARIABLE.Name);

                await VARIABLE.LoadGoogleSheets();
                
                break;
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

        public async Task<ICollection<GoogleSpreadsheet>> GetAllSpreadsheets()
        {
            await _googleDataProvider.RefreshAccessTokenIfExpires();

            var collection = new List<GoogleSpreadsheet>();

            var urlBuilder = URLBuilder.GetSpreadsheets().AddOrderBy("createdTime").
                            AddRequest("mimeType = 'application/vnd.google-apps.spreadsheet' and 'me' in owners and trashed = false");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _googleDataProvider.AccessToken);
            using (var response = await _httpClient.GetAsync(urlBuilder.GetURL()))
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

                            collection.Add(new GoogleSpreadsheet(fileInfo["id"].ToString(), fileInfo["name"].ToString(), _googleDataProvider));
                        }
                    }
                }
            }

            return collection;
        }
    }
}