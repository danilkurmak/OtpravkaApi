namespace OtpravkaApi.Models
{
    public class EmailReport
    {
        public int Id { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Created"; // Created / Sent / Failed
        public string? ErrorMessage { get; set; }
    }
}
