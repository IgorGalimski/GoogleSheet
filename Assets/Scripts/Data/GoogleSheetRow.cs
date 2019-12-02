using System.Collections.Generic;

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
    }
}