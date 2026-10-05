namespace BarcodeApi.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public List<Terminal> Terminals { get; set; } = new();
    }
}
