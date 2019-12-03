using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class GoogleSheetRow
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
    }
}