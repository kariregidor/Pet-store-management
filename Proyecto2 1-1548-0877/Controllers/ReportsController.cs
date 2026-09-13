using Microsoft.AspNetCore.Mvc;
using Proyecto2_1_1548_0877.Data;
using Proyecto2_1_1548_0877.Models;
using Microsoft.EntityFrameworkCore;

namespace Proyecto2_1_1548_0877.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly PetsContext _context;

        public ReportsController(PetsContext context)
        {
            _context = context;
        }

        [HttpGet("readReports")]
        public async Task<ActionResult<IEnumerable<Reports>>> GetAll()
        {
            var reports = await _context.Reports
                .Include(r => r.Pets)
                .Include(r => r.Customer)
                .ToListAsync();

            return Ok(reports);
        }


        // DELETE: api/reports/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound(new { message = "Report not found." });
            }
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Report deleted successfully." });
        }

        // GET: api/reports/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Reports>> GetById(int id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null) return NotFound();

            return Ok(report);
        }


    }
}
