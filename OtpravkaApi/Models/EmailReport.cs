namespace OtpravkaApi.Models
{
    public class EmailReport
    {
        public int Id { get; set; }

        public string RecipientEmail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string ReportFileName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
