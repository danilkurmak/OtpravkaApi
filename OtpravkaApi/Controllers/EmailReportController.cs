using Microsoft.AspNetCore.Mvc;
using OtpravkaApi.Models;

namespace OtpravkaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailReportController : ControllerBase
    {
        private static readonly List<EmailReport> reports = new();
        private static int nextId = 1;

        // GET /api/EmailReport
        [HttpGet]
        public ActionResult<IEnumerable<EmailReport>> GetAll()
        {
            return Ok(reports);
        }

        // GET /api/EmailReport/{id}
        [HttpGet("{id:int}")]
        public ActionResult<EmailReport> GetById(int id)
        {
            var report = reports.FirstOrDefault(r => r.Id == id);
            if (report == null) return NotFound($"Report with id={id} not found");
            return Ok(report);
        }

        // GET /api/EmailReport/search?recipient=...&status=...
        [HttpGet("search")]
        public ActionResult<IEnumerable<EmailReport>> Search([FromQuery] string? recipient, [FromQuery] string? status)
        {
            var query = reports.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(recipient))
                query = query.Where(r => r.Recipient.Contains(recipient, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

            return Ok(query.ToList());
        }

        // POST /api/EmailReport
        [HttpPost]
        public ActionResult<EmailReport> Create([FromBody] EmailReport report)
        {
            if (string.IsNullOrWhiteSpace(report.Recipient))
                return BadRequest("Recipient is required");

            report.Id = nextId++;
            report.CreatedAt = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(report.Status))
                report.Status = "Created";

            reports.Add(report);

            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
        }

        // PUT /api/EmailReport/{id}
        [HttpPut("{id:int}")]
        public ActionResult<EmailReport> Update(int id, [FromBody] EmailReport update)
        {
            var report = reports.FirstOrDefault(r => r.Id == id);
            if (report == null) return NotFound($"Report with id={id} not found");

            // обновляем поля (Id и CreatedAt не трогаем)
            if (!string.IsNullOrWhiteSpace(update.Recipient)) report.Recipient = update.Recipient;
            if (!string.IsNullOrWhiteSpace(update.Subject)) report.Subject = update.Subject;
            if (!string.IsNullOrWhiteSpace(update.Body)) report.Body = update.Body;
            if (!string.IsNullOrWhiteSpace(update.Status)) report.Status = update.Status;

            return Ok(report);
        }

        // DELETE /api/EmailReport/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var report = reports.FirstOrDefault(r => r.Id == id);
            if (report == null) return NotFound($"Report with id={id} not found");

            reports.Remove(report);
            return NoContent();
        }

        // DELETE /api/EmailReport
        // очистка списка (удобно для лабы)
        [HttpDelete]
        public IActionResult Clear()
        {
            reports.Clear();
            nextId = 1;
            return NoContent();
        }
    }
}
