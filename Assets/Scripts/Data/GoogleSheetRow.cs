using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Data
{
    public class GoogleSheetRow : IEnumerable<GoogleSheetCell>
    {
        public int Index { get; }

        public ICollection<GoogleSheetCell> Values { get; }

        public GoogleSheetRow(int index, ICollection<GoogleSheetCell> values)
        {
            Index = index;
            Values = values;
        }

        public GoogleSheetCell this[int index] => Values.ElementAtOrDefault(index);

        public JObject GetValue()
        {
            var valueProperty = new JProperty("stringValue", "test");
            var userEnteredValueProperty = new JProperty("userEnteredValue", new JObject(valueProperty));
            
            return new JObject(userEnteredValueProperty);
        }

        public IEnumerator<GoogleSheetCell> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}