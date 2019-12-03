using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Data
{
    public class GoogleSheet
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        
        public ICollection<GoogleSheetRow> GoogleSheetRows { get; private set; } = new List<GoogleSheetRow>();

        public GoogleSheet(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public GoogleSheetRow this[int index]
        {
            get
            {
                return GoogleSheetRows.ElementAtOrDefault(index);
            }
        }

        public void Parse(IEnumerable<JToken> valuesToken)
        {
            GoogleSheetRows.Clear();

            for (int i = 0; i < valuesToken.Count(); i++)
            {
                var jToken = valuesToken.ElementAt(i);
                
                var row = new List<object>();
                foreach (var value in jToken)
                {
                    row.Add(GetJValueByGoogleSheetType(value));
                }
                GoogleSheetRows.Add(new GoogleSheetRow(i, row));
            }
        }
        
        private JValue GetJValueByGoogleSheetType(JToken obj)
        {
            string objString = obj.ToString();
            JTokenType type = obj.Type;
            
            switch (type)
            {
                case JTokenType.Boolean:
                {
                    if (!bool.TryParse(objString, out var result));
                    return new JValue(result);
                }
                
                case JTokenType.Integer:
                {
                    int.TryParse(objString, out var result);
                    return new JValue(result);
                }
                    
                case JTokenType.Float:
                {
                    float.TryParse(objString, out var result);
                    return new JValue(result);
                } 
                    
                case JTokenType.Date:
                {
                    DateTime.TryParse(objString, out var result);
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