using Microsoft.AspNetCore.Mvc;
using OtpravkaApi.Models;

namespace OtpravkaApi.Controllers
{
    // Помечаем класс как API-контроллер
    [ApiController]

    // Базовый маршрут: api/EmailReport
    [Route("api/[controller]")]
    public class EmailReportController : ControllerBase
    {
        // Временное хранилище данных (в памяти приложения)
        private static List<EmailReport> reports = new();

        // GET: api/EmailReport
        // Возвращает список всех отчетов
        [HttpGet]
        public IEnumerable<EmailReport> Get()
        {
            return reports;
        }

        // POST: api/EmailReport
        // Добавляет новый отчет
        [HttpPost]
        public IActionResult Post([FromBody] EmailReport report)
        {
            // Присваиваем простой идентификатор
            report.Id = reports.Count + 1;

            // Добавляем отчет в список
            reports.Add(report);

            // Возвращаем добавленный объект
            return Ok(report);
        }

        // DELETE: api/EmailReport
        // Очищает весь список отчетов
        [HttpDelete]
        public IActionResult DeleteAll()
        {
            reports.Clear();
            return Ok("All reports deleted");
        }

        // GET: api/EmailReport/search?recipient=test@mail.com
        // Поиск отчетов по получателю
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string recipient)
        {
            var result = reports
                .Where(r => r.Recipient != null &&
                            r.Recipient.Contains(recipient))
                .ToList();

            return Ok(result);
        }
    }
}
