using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{
    //Customer CONTROLLER
    public class CustomersController : Controller
    {
        private readonly PetsContext _context;

        public CustomersController(PetsContext context)
        {
            _context = context;
        }

        // List Customers
        public async Task<IActionResult> IndexCustomers()
        {
            var Customers = await _context.Customer.ToListAsync();
            return View(Customers);
        }

        // Get list contact preference
        private List<SelectListItem> GetContactPreference() =>
            Enum.GetValues(typeof(ContactPreference))
                .Cast<ContactPreference>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();

        // Create customer
        public IActionResult Create()
        {
            ViewBag.ContactPreference = GetContactPreference();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer CustomerNew)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ContactPreference = GetContactPreference();
                    return View(CustomerNew);
                }

                if (await _context.Customer.AnyAsync(c => c.IdCustomer == CustomerNew.IdCustomer))
                {
                    ModelState.AddModelError("", "A customer with this Id already exist.");
                    ViewBag.ContactPreference = GetContactPreference();
                    return View(CustomerNew);
                }

                _context.Customer.Add(CustomerNew);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexCustomers));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Excepción: {ex.Message}");
                ViewBag.ContactPreference = GetContactPreference();
                return View(CustomerNew);
            }
        }

        // Edit Customer - GET
        public async Task<IActionResult> Edit(int id)
        {
            var Customer = await _context.Customer.FindAsync(id);
            if (Customer == null) return NotFound();

            ViewBag.ContactPreference = GetContactPreference();
            return View(Customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer CustomerEdited)
        {
            if (id != CustomerEdited.IdCustomer) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.ContactPreference = GetContactPreference();
                return View(CustomerEdited);
            }

            var existing = await _context.Customer.FindAsync(id);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(CustomerEdited);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexCustomers));
        }

        // Delete Customers
        public async Task<IActionResult> Delete(int id)
        {
            var Customer = await _context.Customer.FindAsync(id);
            if (Customer == null) return NotFound();

            return View(Customer);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Customer = await _context.Customer.FindAsync(id);
            if (Customer == null) return NotFound();

            try
            {
                _context.Customer.Remove(Customer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexCustomers));
            }
            catch (DbUpdateException ex)
            {
                string message = ex.InnerException != null && ex.InnerException.Message.Contains("FK_Pets_Customers")
                    ? "The Customer cannot be removed because it has associated pets."
                    : $"Error deleting Customer: {ex.InnerException?.Message ?? ex.Message}";

                ModelState.AddModelError("", message);
                return View("Delete", Customer);
            }
        }

        // Find customers with filters
        public async Task<IActionResult> Buscar(
            int IdCustomer,
            string Name,
            string City,
            string State,
            string Country,
            string Address,
            string Phone,
            ContactPreference? ContactPreference)
        {
            var Customers = await _context.Customer.ToListAsync();
            var results = Customers.AsEnumerable();

            // Dinamic filters
            if (IdCustomer > 0)
                results = results.Where(m => m.IdCustomer == IdCustomer);

            if (!string.IsNullOrEmpty(Name))
                results = results.Where(m => m.Name.Contains(Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(City))
                results = results.Where(m => m.City.Contains(City, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(State))
                results = results.Where(m => m.State.Contains(State, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(Country))
                results = results.Where(m => m.Country.Contains(Country, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(Address))
                results = results.Where(m => m.Address.Contains(Address, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(Phone))
                results = results.Where(m => m.Phone.Contains(Phone, StringComparison.OrdinalIgnoreCase));

            if (ContactPreference.HasValue)
                results = results.Where(m => m.ContactPreference == ContactPreference.Value);

            ViewBag.ContactPreference = GetContactPreference();
            return View(results.ToList());
        }

    }
}
