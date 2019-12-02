namespace Data
{
    public class GoogleSheet
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public GoogleSheet(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}