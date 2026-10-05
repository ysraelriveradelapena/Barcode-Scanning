namespace BarcodeApi.Models
{
    public class Terminal
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public Store? Store { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
