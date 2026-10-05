namespace BarcodeApi.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class ScanRequest
    {
        public Guid ClientScanId { get; set; }
        public string Code { get; set; } = "";
        public string? Format { get; set; }
        public DateTime ScannedAt { get; set; }
        public string StoreCode { get; set; } = "";
        public string TerminalCode { get; set; } = "";
    }
}
