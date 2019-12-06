using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Data
{
    public class GoogleSheetRow : IEnumerable<object>
    {
        public int Index { get; private set; }

        public ICollection<object> Values { get; private set; }

        public GoogleSheetRow(int index, ICollection<object> values)
        {
            Index = index;
            Values = values;
        }

        public object this[int index]
        {
            get { return Values.ElementAtOrDefault(index); }
        }

        public JObject GetValue()
        {
            var valueProperty = new JProperty("stringValue", "test");
            var userEnteredValueProperty = new JProperty("userEnteredValue", new JObject(valueProperty));
            
            return new JObject(userEnteredValueProperty);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}