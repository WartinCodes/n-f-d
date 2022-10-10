namespace ConsoleApp5.Entities
{
    public class FileIdentifier
    {
        public FileIdentifier(int id, string name, int quantity, string fullName, bool exist)
        {
            this.Id = id;
            this.Name = name;
            this.Quantity = quantity;
            this.FullName = fullName;
            this.Exist = exist;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string FullName { get; set; }
        public bool Exist { get; set; }
    }
}
