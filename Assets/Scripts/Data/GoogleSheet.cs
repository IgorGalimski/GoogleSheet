using System.Collections.Generic;

namespace Data
{
    public class GoogleSheet
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        
        public ICollection<GoogleSheetRow> GoogleSheetRows { get; private set; }

        public GoogleSheet(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public void UpdateRow(ICollection<GoogleSheetRow> googleSheetRows)
        {
            GoogleSheetRows = googleSheetRows;
        }
    }
}