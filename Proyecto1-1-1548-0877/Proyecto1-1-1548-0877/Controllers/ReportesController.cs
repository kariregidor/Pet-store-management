using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{
    public class ReportesController : Controller
    {

        //REPORTES CONTROLLER
        private readonly PetsContext _context;

        public ReportesController(PetsContext context)
        {
            _context = context;
        }


        // Listar reportes
        public async Task<IActionResult> IndexReportes()
        {
            try
            {
                var reportes = await _context.Reportes
                    .Include(r => r.Mascota)
                    .Include(r => r.Cliente)
                    .ToListAsync();

                return View(reportes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error IndexReportes: {ex.Message}");
                return View(new List<Reportes>());
            }
        }

        // GET: Eliminar reporte
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var reporte = await _context.Reportes.FindAsync(id);
                if (reporte == null) return NotFound();

                return View(reporte); // Delete.cshtml
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
                var reporte = await _context.Reportes.FindAsync(id);
                if (reporte != null)
                {
                    _context.Reportes.Remove(reporte);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Reporte eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el reporte.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar: {ex.Message}";
            }

            return RedirectToAction("IndexReportes");
        }
    }
}
