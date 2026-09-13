using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{
    //Employes CONTROLLER
    public class EmployesController : Controller
    {
        private readonly PetsContext _context;

        public EmployesController(PetsContext context)
        {
            _context = context;
        }

        // List Employes
        public async Task<IActionResult> IndexEmployes()
        {
            var Employes = await _context.Employes.ToListAsync();
            return View(Employes);
        }

        // Create Employes
        public IActionResult Create()
        {
            ViewBag.TypeEmployes = GetTypeEmployes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employes EmployeNew)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TypeEmployes = GetTypeEmployes();
                return View(EmployeNew);
            }

            if (await _context.Employes.AnyAsync(e => e.Id == EmployeNew.Id))
            {
                ModelState.AddModelError("", "There is an employe already with that ID");
                ViewBag.TypeEmployes = GetTypeEmployes();
                return View(EmployeNew);
            }

            _context.Employes.Add(EmployeNew);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexEmployes));
        }

        // Edit Employes
        public async Task<IActionResult> Edit(string Id)
        {
            var Employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == Id);
            if (Employe == null) return NotFound();

            ViewBag.TypeEmployes = GetTypeEmployes();
            return View(Employe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, Employes EmployeEdited)
        {
            if (Id != EmployeEdited.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.TypeEmployes = GetTypeEmployes();
                return View(EmployeEdited);
            }

            var emp = await _context.Employes.FirstOrDefaultAsync(e => e.Id == Id);
            if (emp == null) return NotFound();

            emp.DateOfBirth = EmployeEdited.DateOfBirth;
            emp.StartDate = EmployeEdited.StartDate;
            emp.SalaryPerDay = EmployeEdited.SalaryPerDay;
            emp.RetirementDate = EmployeEdited.RetirementDate;
            emp.TypeEmploye = EmployeEdited.TypeEmploye;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexEmployes));
        }

        // GET: Delete Employes
        public async Task<IActionResult> Delete(string Id)
        {
            var Employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == Id);
            if (Employe == null) return NotFound();

            return View(Employe);
        }

        // POST:
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            var Employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == Id);
            if (Employe == null) return NotFound();

            try
            {
                _context.Employes.Remove(Employe);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexEmployes));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Employed was not deletes");
                return View("Delete", Employe);
            }
        }

        // Find employes with filters
        public async Task<IActionResult> Buscar(
            string Id,
            DateTime? DateOfBirth,
            DateTime? StartDate,
            decimal? SalaryPerDay,
            DateTime? RetirementDate,
            TypeEmploye? TypeEmploye)
        {
            var Employes = await _context.Employes.ToListAsync();
            var results = Employes.AsEnumerable();

            if (!string.IsNullOrEmpty(Id))
                results = results.Where(e => e.Id.Contains(Id));

            if (DateOfBirth.HasValue)
                results = results.Where(e => e.DateOfBirth.Date == DateOfBirth.Value.Date);

            if (StartDate.HasValue)
                results = results.Where(e => e.StartDate.Date == StartDate.Value.Date);

            if (SalaryPerDay.HasValue)
                results = results.Where(e => e.SalaryPerDay == SalaryPerDay.Value);

            if (RetirementDate.HasValue)
                results = results.Where(e => e.RetirementDate.HasValue && e.RetirementDate.Value.Date == RetirementDate.Value.Date);

            if (TypeEmploye.HasValue)
                results = results.Where(e => e.TypeEmploye == TypeEmploye.Value);

            ViewBag.TypeEmployes = GetTypeEmployes();
            return View(results.ToList());
        }

        // Get list of Type de Employes
        private List<SelectListItem> GetTypeEmployes() =>
            Enum.GetValues(typeof(TypeEmploye))
                .Cast<TypeEmploye>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();
    }
}
