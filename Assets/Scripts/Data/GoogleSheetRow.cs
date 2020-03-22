using System.Collections;
using System.Collections.Generic;
using System.Linq;

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