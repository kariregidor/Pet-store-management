using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{
    public class ReportsController : Controller
    {

        //REPORTS CONTROLLER
        private readonly PetsContext _context;

        public ReportsController(PetsContext context)
        {
            _context = context;
        }


        // List reports
        public async Task<IActionResult> IndexReports()
        {
            try
            {
                var reports = await _context.Reports
                    .Include(r => r.Pets)
                    .Include(r => r.Customer)
                    .ToListAsync();

                return View(reports);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error IndexReports: {ex.Message}");
                return View(new List<Reports>());
            }
        }

        // GET: Delete report
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var report = await _context.Reports.FindAsync(id);
                if (report == null) return NotFound();

                return View(report); // Delete.cshtml
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Delete: {ex.Message}");
                return NotFound();
            }
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var report = await _context.Reports.FindAsync(id);
                if (report != null)
                {
                    _context.Reports.Remove(report);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Report deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "The report could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting: {ex.Message}";
            }

            return RedirectToAction("IndexReports");
        }
    }
}
