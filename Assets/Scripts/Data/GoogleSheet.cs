using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Newtonsoft.Json.Linq;

namespace Data
{
    public class GoogleSheet : IEnumerable<GoogleSheetRow>
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        
        public ICollection<GoogleSheetRow> GoogleSheetRows { get; private set; } = new List<GoogleSheetRow>();

        public GoogleSheet(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public GoogleSheetCell this[string range]
        {
            get
            {
                if (TryGetCell(range, out var googleSheetCell))
                {
                    return googleSheetCell;
                }

                return null;
            }
            set
            {
                if (TryGetCell(range, out var googleSheetCell))
                {
                    googleSheetCell.Value = value;
                }
            }
        }

        private bool TryGetCell(string range, out GoogleSheetCell googleSheetCell)
        {
            googleSheetCell = null;
            
            if (TryParseRange(range, out var columnIndex, out var rowIndex))
            {
                var row = GoogleSheetRows.ElementAtOrDefault(rowIndex - 1);
                if (row != null)
                {
                    googleSheetCell = row[columnIndex - 1];

                    return true;
                }
            }

            return false;
        }

        private bool TryParseRange(string range, out int columnIndex, out int rowIndex)
        {
            columnIndex = -1;
            rowIndex = -1;

            if (!string.IsNullOrEmpty(range) && range.Length == 2)
            {
                columnIndex = char.ToUpper(range[0]) - 64;
                if (int.TryParse(range[1].ToString(), out rowIndex))
                {
                    return true;
                }
            }

            return false;
        }

        public void Parse(IEnumerable<JToken> valuesToken)
        {
            GoogleSheetRows.Clear();

            for (int i = 0; i < valuesToken.Count(); i++)
            {
                var jToken = valuesToken.ElementAt(i);
                
                var row = new List<GoogleSheetCell>();
                foreach (var value in jToken)
                {
                    var cell = new GoogleSheetCell
                    {
                        Value = GetJValueByGoogleSheetType(value)
                    };

                    row.Add(cell);
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

        public IEnumerator<GoogleSheetRow> GetEnumerator()
        {
            return GoogleSheetRows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}