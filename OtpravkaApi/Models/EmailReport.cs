namespace OtpravkaApi.Models
{
    public class EmailReport
    {
        public int Id { get; set; }

        // Кому отправляли
        public string Recipient { get; set; } = string.Empty;

        // Тема письма
        public string Subject { get; set; } = string.Empty;

        // Тело письма/сообщение
        public string Body { get; set; } = string.Empty;

        // Дата создания записи
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Статус (например: Created, Sent, Failed)
        public string Status { get; set; } = "Created";
    }
}
