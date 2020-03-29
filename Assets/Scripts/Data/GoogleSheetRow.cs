using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class GoogleSheetRow : IEnumerable<Cell>
    {
        public int Index { get; }

        public ICollection<Cell> Values { get; }

        public GoogleSheetRow(int index, ICollection<Cell> values)
        {
            Index = index;
            Values = values;
        }

        public Cell this[int index] => Values.ElementAtOrDefault(index);

        public IEnumerator<Cell> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}