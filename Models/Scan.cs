using Microsoft.AspNetCore.Identity;

namespace BarcodeApi.Models
{
    public class Scan
    {
        public int Id { get; set; }
        public Guid ClientScanId { get; set; }          // unique ID made by the app, prevents duplicates
        public string Code { get; set; } = "";
        public string? Format { get; set; }
        public DateTime ScannedAtUtc { get; set; }      // when the phone scanned it
        public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;  // when the server got it
        public string UserId { get; set; } = "";
        public IdentityUser? User { get; set; }
        public int StoreId { get; set; }
        public Store? Store { get; set; }
        public int TerminalId { get; set; }
        public Terminal? Terminal { get; set; }
    }
}
