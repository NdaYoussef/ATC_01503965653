using System.Text.Json.Serialization;

namespace EventManagmentTask.Models
{
    public class RefreshToken
    {
        [JsonIgnore]
        public int id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsExpired => DateTime.UtcNow > ExpiresOn;
        public bool IsActive => !IsExpired && RevokedOn == null;
    }
}
