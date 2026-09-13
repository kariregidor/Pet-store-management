using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{
    //PROCEDURES CONTROLLER
    public class ProceduresController : Controller
    {
        private readonly PetsContext _context;

        public ProceduresController(PetsContext context)
        {
            _context = context;
        }

        // List procedures
        public async Task<IActionResult> IndexProcedures()
        {
            var procedures = await _context.Procedures
                .Include(p => p.Pets)
                .ToListAsync();

            return View(procedures);
        }

        // GET: Create procedure
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await GetCustomersAsync();
            ViewBag.ProcedureType = GetProcedureType();
            ViewBag.ProcedureStatus = GetProcedureStatus();

            return View();
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Procedures procedure)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Customers = await GetCustomersAsync();
                    ViewBag.ProcedureType = GetProcedureType();
                    ViewBag.ProcedureStatus = GetProcedureStatus();
                    return View(procedure);
                }

                var petExists = await _context.Pets.AnyAsync(m => m.PetId == procedure.PetId);
                if (!petExists)
                {
                    ModelState.AddModelError("", "The specified pet does not exist");
                }
                else
                {
                    // Automatically assign the price based on the procedure type
                    procedure.BasePrice = GetBasePrice(procedure.ProcedureType);
                    procedure.Pets = null;

                    _context.Procedures.Add(procedure);
                    await _context.SaveChangesAsync();

                    // Automatically create a report after inserting
                    await CreateReportFromProcedure(procedure.Id);

                    TempData["Success"] = "Procedure created successfully";
                    return RedirectToAction("IndexProcedures");
                }
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException != null &&
                    (ex.InnerException.Message.Contains("PRIMARY KEY") || ex.InnerException.Message.Contains("duplicate"))
                    ? "A procedure with that number already exists"
                    : $"Error saving the procedure: {ex.InnerException?.Message ?? ex.Message}";

                ModelState.AddModelError("", message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            ViewBag.Customers = await GetCustomersAsync();
            ViewBag.ProcedureType = GetProcedureType();
            ViewBag.ProcedureStatus = GetProcedureStatus();
            return View(procedure);
        }

        // Get pets by customer
        [HttpGet]
        public async Task<JsonResult> GetPetsByCustomer(int idCustomer)
        {
            var customerPets = await GetPetsByCustomerAsync(idCustomer);
            return Json(customerPets);
        }

        // GET: Edit procedure
        public async Task<IActionResult> Edit(int id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null) return NotFound();

            ViewBag.Customers = await GetCustomersAsync();
            ViewBag.ProcedureType = GetProcedureType();
            ViewBag.ProcedureStatus = GetProcedureStatus();

            // Load pets for the associated customer
            ViewBag.Pets = await GetPetsByCustomerAsync(procedure.IdCustomer);

            return View(procedure);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Procedures procedure)
        {
            if (id != procedure.Id)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Customers = await GetCustomersAsync();
                    ViewBag.ProcedureType = GetProcedureType();
                    ViewBag.ProcedureStatus = GetProcedureStatus();
                    ViewBag.Pets = await GetPetsByCustomerAsync(procedure.IdCustomer);
                    return View(procedure);
                }

                var proc = await _context.Procedures.FindAsync(id);
                if (proc == null) return NotFound();

                // Verify the pet exists if it was changed
                if (proc.PetId != procedure.PetId)
                {
                    var petExists = await _context.Pets.AnyAsync(m => m.PetId == procedure.PetId);

                    if (!petExists)
                    {
                        ModelState.AddModelError("", "The specified pet does not exist");
                        ViewBag.Customers = await GetCustomersAsync();
                        ViewBag.ProcedureType = GetProcedureType();
                        ViewBag.ProcedureStatus = GetProcedureStatus();
                        ViewBag.Pets = await GetPetsByCustomerAsync(procedure.IdCustomer);
                        return View(procedure);
                    }
                }

                // Update only the necessary fields
                proc.IdCustomer = procedure.IdCustomer;
                proc.PetId = procedure.PetId;
                proc.ProcedureType = procedure.ProcedureType;
                proc.Weight = procedure.Weight;
                proc.BasePrice = procedure.BasePrice;
                proc.Status = procedure.Status;

                await _context.SaveChangesAsync();

                // Update or create the report
                await UpdateReportFromProcedure(proc.Id);

                TempData["Success"] = "Procedure updated successfully";
                return RedirectToAction("IndexProcedures");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"Error updating: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            ViewBag.Customers = await GetCustomersAsync();
            ViewBag.ProcedureType = GetProcedureType();
            ViewBag.ProcedureStatus = GetProcedureStatus();
            ViewBag.Pets = await GetPetsByCustomerAsync(procedure.IdCustomer);
            return View(procedure);
        }

        // GET: Delete procedure
        public async Task<IActionResult> Delete(int id)
        {
            var procedure = await _context.Procedures.Include(p => p.Pets).FirstOrDefaultAsync(p => p.Id == id);
            if (procedure == null) return NotFound();

            return View(procedure);
        }

        // POST:
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null) return NotFound(new { message = "Procedure not found" });

            try
            {
                // Delete the associated report first (if any) to avoid FK issues
                await DeleteReportByProcedure(id);

                _context.Procedures.Remove(procedure);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Procedure deleted successfully";
                return RedirectToAction(nameof(IndexProcedures));
            }
            catch (DbUpdateException ex)
            {
                // Handle FK errors or other database issues
                var inner = ex.InnerException?.Message ?? ex.Message;
                var message = inner.Contains("FK_") || inner.Contains("REFERENCE")
                    ? "The procedure cannot be removed because it has related data."
                    : $"Error deleting the procedure: {inner}";

                ModelState.AddModelError("", message);

                var procedureWithPet = await _context.Procedures.Include(p => p.Pets).FirstOrDefaultAsync(p => p.Id == id);
                return View("Delete", procedureWithPet);
            }
        }

        // Find procedures with filters
        public async Task<IActionResult> Buscar(
            int Id = 0,
            int IdCustomer = 0,
            int PetId = 0,
            ProcedureType? procedureType = null,
            double? weight = null,
            decimal? BasePrice = null,
            ProcedureStatus? procedureStatus = null)
        {
            try
            {
                var procedures = await _context.Procedures.Include(p => p.Pets).ToListAsync();
                var results = procedures.AsEnumerable();

                // Apply filters
                if (Id > 0) results = results.Where(p => p.Id == Id);
                if (IdCustomer > 0) results = results.Where(p => p.IdCustomer == IdCustomer);
                if (PetId > 0) results = results.Where(p => p.PetId == PetId);
                if (procedureType.HasValue) results = results.Where(p => p.ProcedureType == procedureType.Value);
                if (weight.HasValue) results = results.Where(p => p.Weight.HasValue && p.Weight.Value == weight.Value);
                if (BasePrice.HasValue) results = results.Where(p => p.BasePrice == BasePrice.Value);
                if (procedureStatus.HasValue) results = results.Where(p => p.Status == procedureStatus.Value);

                // Load dropdowns
                ViewBag.Customers = await GetCustomersAsync();
                ViewBag.ProcedureType = GetProcedureType();
                ViewBag.ProcedureStatus = GetProcedureStatus();

                return View(results.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Buscar: {ex.Message}");
                return View(new List<Procedures>());
            }
        }

        // ============ HELPER METHODS ============

        // Get customers list
        private async Task<List<SelectListItem>> GetCustomersAsync()
        {
            var customers = await _context.Customer.ToListAsync();

            return customers.Select(c => new SelectListItem
            {
                Value = c.IdCustomer.ToString(),
                Text = c.Name
            }).ToList();
        }

        // Get pets filtered by customer
        private async Task<List<SelectListItem>> GetPetsByCustomerAsync(int idCustomer)
        {
            if (idCustomer == 0)
                return new List<SelectListItem>();

            var pets = await _context.Pets.Where(m => m.IdCustomer == idCustomer).ToListAsync();

            return pets
                .Select(m => new SelectListItem
                {
                    Value = m.PetId.ToString(),
                    Text = m.PetName
                })
                .ToList();
        }

        // Get procedure type list
        private List<SelectListItem> GetProcedureType() =>
            Enum.GetValues(typeof(ProcedureType))
                .Cast<ProcedureType>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();

        // Get procedure status list
        private List<SelectListItem> GetProcedureStatus() =>
            Enum.GetValues(typeof(ProcedureStatus))
                .Cast<ProcedureStatus>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();

        // Helper method to get the base price for a procedure type
        private decimal GetBasePrice(ProcedureType type)
        {
            return type switch
            {
                ProcedureType.Consultation => 15000m,
                ProcedureType.ConsultationSpecialHours => 17000m,
                ProcedureType.Neutering_0_5 => 35000m,
                ProcedureType.Neutering_5_10 => 45000m,
                ProcedureType.Neutering_10_20kg => 55000m,
                ProcedureType.Neutering_20_30kg => 80000m,
                ProcedureType.Neutering_30_50kg => 100000m,
                ProcedureType.MinorSurgery => 15000m,
                ProcedureType.Surgery => 250000m,
                ProcedureType.Grooming_Small => 15000m,
                ProcedureType.Grooming_Medium => 20000m,
                ProcedureType.Grooming_Big => 125000m,
                ProcedureType.Grooming_ExtraBig => 35000m,
                ProcedureType.AnnualVaccines => 40000m,
                _ => 0m
            };
        }

        // METHODS
        // Create report
        private async Task CreateReportFromProcedure(int procedureId)
        {
            var procedure = await _context.Procedures
                .Include(p => p.Pets)
                .FirstOrDefaultAsync(p => p.Id == procedureId);

            if (procedure == null) return;

            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.IdCustomer == procedure.IdCustomer);

            var newReport = new Reports
            {
                Id = procedure.Id, // FK
                IdCustomer = procedure.IdCustomer,
                PetId = procedure.PetId,
                ProcedureType = procedure.ProcedureType,
                ProcedureStatus = procedure.Status,
                Weight = procedure.Weight,
                BasePrice = procedure.BasePrice,
                VAT = procedure.VAT,
                TotalPrice = procedure.TotalPrice,
                VaccinationDate = DateTime.Now.AddDays(7),
                Name = customer?.Name,
                PetName = procedure.Pets?.PetName
            };

            _context.Reports.Add(newReport);
            await _context.SaveChangesAsync();
        }

        // Update report
        private async Task UpdateReportFromProcedure(int procedureId)
        {
            // Find report by procedure Id
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == procedureId);

            if (report == null)
            {
                // If it doesn't exist, create a new one
                await CreateReportFromProcedure(procedureId);
                return;
            }

            var procedure = await _context.Procedures
                .Include(p => p.Pets)
                .FirstOrDefaultAsync(p => p.Id == procedureId);

            if (procedure == null) return;

            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.IdCustomer == procedure.IdCustomer);

            // Update report fields
            report.IdCustomer = procedure.IdCustomer;
            report.PetId = procedure.PetId;
            report.ProcedureType = procedure.ProcedureType;
            report.ProcedureStatus = procedure.Status;
            report.Weight = procedure.Weight;
            report.BasePrice = procedure.BasePrice;
            report.VAT = procedure.VAT;
            report.TotalPrice = procedure.TotalPrice;
            report.Name = customer?.Name;
            report.PetName = procedure.Pets?.PetName;

            await _context.SaveChangesAsync();
        }

        // Delete report
        private async Task DeleteReportByProcedure(int procedureId)
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == procedureId);

            if (report != null)
            {
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }
    }
}
