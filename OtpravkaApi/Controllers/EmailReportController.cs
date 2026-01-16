using Microsoft.AspNetCore.Mvc;
using OtpravkaApi.Models;

// MailKit/MimeKit — для SMTP и формирования письма + вложений
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

using System.Text;
using System.Linq;

namespace OtpravkaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailReportController : ControllerBase
    {
        // Читаем SMTP-настройки из appsettings.json
        private readonly IConfiguration _config;

        // Журнал отправок (в памяти приложения)
        private static readonly List<EmailReport> reports = new();

        public EmailReportController(IConfiguration config)
        {
            _config = config;
        }

        // Метод GET: /api/EmailReport 
        // Возвращает историю (журнал) всех отправок
        [HttpGet]
        public ActionResult<IEnumerable<EmailReport>> Get()
        {
            return Ok(reports);
        }

        // DTO для отправки письма (то, что вводим в Swagger)
        public class SendEmailRequest
        {
            public string Recipient { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }

        // Вспомогательная функция для CSV (чтобы Excel/CSV не ломался от ; и кавычек)
        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            var mustQuote = value.Contains(';') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
            if (!mustQuote) return value;

            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        
        // POST: /api/EmailReport/send
        // Главный бизнес-метод:
        // - принимает данные письма
        // - отправляет email через SMTP
        // - формирует CSV-отчёт и прикрепляет как вложение
        // - сохраняет запись в журнал отправок
        [HttpPost("send")]
        public IActionResult Send([FromBody] SendEmailRequest request)
        {
            // 0) Проверяем входные данные
            if (request == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.Recipient))
                return BadRequest("Recipient is required.");

            // 1) Создаём запись в журнале (в памяти)
            var report = new EmailReport
            {
                Id = reports.Count == 0 ? 1 : reports.Max(r => r.Id) + 1,
                Recipient = request.Recipient,
                Subject = request.Subject,
                Body = request.Body,
                CreatedAt = DateTime.Now,
                Status = "Created",
                ErrorMessage = null
            };

            reports.Add(report);

            // 2) Берём SMTP-настройки из appsettings.json
            var smtpHost = _config["Smtp:Host"];
            var smtpPortStr = _config["Smtp:Port"];
            var smtpUser = _config["Smtp:Username"];
            var smtpPass = _config["Smtp:Password"];
            var fromName = _config["Smtp:FromName"] ?? "OtpravkaApi";
            var useSslStr = _config["Smtp:UseSsl"];

            if (string.IsNullOrWhiteSpace(smtpHost) ||
                string.IsNullOrWhiteSpace(smtpPortStr) ||
                string.IsNullOrWhiteSpace(smtpUser) ||
                string.IsNullOrWhiteSpace(smtpPass))
            {
                report.Status = "Failed";
                report.ErrorMessage = "SMTP settings are missing in appsettings.json";
                return StatusCode(500, report);
            }

            if (!int.TryParse(smtpPortStr, out var smtpPort))
            {
                report.Status = "Failed";
                report.ErrorMessage = "Smtp:Port must be a number";
                return StatusCode(500, report);
            }

            var useSsl = bool.TryParse(useSslStr, out var ssl) && ssl;

            try
            {
                // 3) Собираем письмо
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, smtpUser));
                message.To.Add(MailboxAddress.Parse(request.Recipient));
                message.Subject = request.Subject ?? "";

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = request.Body ?? ""
                };

                // 4) Формируем CSV-вложение (в памяти)
                // Excel на Windows лучше открывает UTF-8, если есть BOM и разделитель ";"
                var csv =
                    "Id;Recipient;Subject;CreatedAt;Status\r\n" +
                    $"{report.Id};{EscapeCsv(report.Recipient)};{EscapeCsv(report.Subject)};{report.CreatedAt:O};{report.Status}\r\n";

                var bytes = Encoding.UTF8.GetPreamble()
                    .Concat(Encoding.UTF8.GetBytes(csv))
                    .ToArray();

                var fileName = $"email_report_{report.Id}.csv";
                bodyBuilder.Attachments.Add(fileName, bytes, ContentType.Parse("text/csv; charset=utf-8"));

                message.Body = bodyBuilder.ToMessageBody();

                // 5) Отправка через SMTP (MailKit)
                using var client = new SmtpClient();
                var secure = useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

                client.Connect(smtpHost, smtpPort, secure);
                client.Authenticate(smtpUser, smtpPass);
                client.Send(message);
                client.Disconnect(true);

                // 6) Успех
                report.Status = "Sent";
                report.ErrorMessage = null;

                return Ok(report);
            }
            catch (Exception ex)
            {
                // 7) Ошибка отправки
                report.Status = "Failed";
                report.ErrorMessage = ex.Message;

                return StatusCode(500, report);
            }
        }
        // GET: /api/EmailReport/stats
        // Возвращает простую статистику по журналу отправок
        [HttpGet("stats")]
        public IActionResult Stats()
        {
            var total = reports.Count;
            var sent = reports.Count(r => r.Status == "Sent");
            var failed = reports.Count(r => r.Status == "Failed");

            var last = reports
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault();

            return Ok(new
            {
                total,
                sent,
                failed,
                lastCreatedAt = last?.CreatedAt,
                lastRecipient = last?.Recipient
            });
        }


    }
}
