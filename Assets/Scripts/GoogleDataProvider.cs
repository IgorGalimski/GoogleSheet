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
            var result = await GetAllSheets("1Aw0mU10Dyj5AnhUyER8MEng5uNREHVcE4zyFa8Gncxw"); 
            await GetSheetsValues("1Aw0mU10Dyj5AnhUyER8MEng5uNREHVcE4zyFa8Gncxw", result);
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

        public async Task GetSheetsValues(string spreadsheetId, ICollection<GoogleSheet> googleSheets)
        {
            await _googleDataProvider.RefreshAccessTokenIfExpires();
            
            var urlBuilder = URLBuilder.GetSheetsValues(spreadsheetId).
                    AddApiKey(_googleDataProvider.ApiKey).
                    AddRanges(googleSheets.Select(item => item.Name)).
                    AddValueRenderOption("FORMULA");

            using (var response = await _httpClient.GetAsync(urlBuilder.GetURL()))
            {
                var content = await response.Content.ReadAsStringAsync();

                JObject jObject = JObject.Parse(content);
                IEnumerable<JToken> valueRanges = jObject["valueRanges"].Select(t => t);  

                foreach (JToken value in valueRanges)
                {
                    if (value["values"] == null) 
                        continue;
                    
                    string sheetName = string.Empty;
                    IEnumerable<JToken> valuesToken = value["values"].Select(t => t);

                    sheetName = value["values"].ToString();
                        
                    int indexExclamationPoint = value["range"].ToString().LastIndexOf("!");
                    if (indexExclamationPoint > 0)
                    {
                        sheetName = value["range"].ToString().Substring(0, indexExclamationPoint);
                    }

                    int firstIndexOfQuotes = value["range"].ToString().IndexOf("'");
                    int lastIndexOfQuotes = value["range"].ToString().LastIndexOf("'");

                    if (firstIndexOfQuotes >= 0 && lastIndexOfQuotes > 0)
                    {
                        sheetName = sheetName.Substring(++firstIndexOfQuotes, --lastIndexOfQuotes);
                    }

                    Debug.LogError(sheetName + " " + ParseObjectValues(valuesToken).Count);
                }
            }
        }
        
        private List<List<object>> ParseObjectValues(IEnumerable<JToken> valuesToken)
        {
            List<List<object>> values = new List<List<object>>();
            
            foreach (JToken jToken in valuesToken)
            {
                List<object> row = new List<object>();
                foreach (JToken value in jToken)
                {
                    row.Add(GetJValueByGoogleSheetType(value));
                }
                values.Add(row);
            }

            return values;
        }
        
        
        private JValue GetJValueByGoogleSheetType(JToken obj)
        {
            string objString = obj.ToString();
            JTokenType type = obj.Type;
            
            switch (type)
            {
                case JTokenType.Boolean:
                {
                    bool result;

                    if (!Boolean.TryParse(objString, out result))
                    {
                        Debug.LogWarningFormat("Unable to parse: {0}", objString);   
                    }
            
                    return new JValue(result);
                }
                
                case JTokenType.Integer:
                {
                    int result;

                    if (!int.TryParse(objString, out result))
                    {
                        Debug.LogWarningFormat("Unable to parse: {0}", objString);   
                    }
            
                    return new JValue(result);
                }
                    
                case JTokenType.Float:
                {
                    float result;
    
                    if (!float.TryParse(objString, out result))                
                    {
                        Debug.LogWarningFormat("Unable to parse: {0}", objString);   
                    }
                    
                    return new JValue(result);
                } 
                    
                case JTokenType.Date:
                {
                    DateTime result;
    
                    if (!DateTime.TryParse(objString, out result))                
                    {
                        Debug.LogWarningFormat("Unable to parse: {0}", objString);   
                    }
                    
                    return new JValue(result);
                }

                default:
                {
                    return new JValue(objString);
                }
            }
        }
    }
}