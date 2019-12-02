namespace Data
{
    public class GoogleSpreadsheet
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public GoogleSpreadsheet(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}