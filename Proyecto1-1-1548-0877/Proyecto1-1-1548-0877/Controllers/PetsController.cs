using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{

    //Pets CONTROLLER
    public class PetsController : Controller
    {
        private readonly PetsContext _context;

        public PetsController(PetsContext context)
        {
            _context = context;
        }

        // List Pets
        public async Task<IActionResult> IndexPets()
        {
            var Pets = await _context.Pets.Include(m => m.Customer).ToListAsync();
            return View(Pets);
        }

        // GET: Pets/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await GetCustomersAsync();
            ViewBag.AnimalSpecies = GetAnimalSpecies();
            return View();
        }

        // POST: Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pets pets)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await GetCustomersAsync();
                ViewBag.AnimalSpecies = GetAnimalSpecies();
                return View(pets);
            }

            _context.Pets.Add(pets);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexPets));
        }

        // GET: Pets/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var pets = await _context.Pets.FindAsync(id);
            if (pets == null) return NotFound();

            ViewBag.Customers = await GetCustomersAsync();
            ViewBag.AnimalSpecies = GetAnimalSpecies();
            return View(pets);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pets petsEdited)
        {
            if (id != petsEdited.PetId) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await GetCustomersAsync();
                ViewBag.AnimalSpecies = GetAnimalSpecies();
                return View(petsEdited);
            }

            var masc = await _context.Pets.FindAsync(id);
            if (masc == null) return NotFound();

            masc.PetName = petsEdited.PetName;
            masc.AnimalSpecies = petsEdited.AnimalSpecies;
            masc.Breed = petsEdited.Breed;
            masc.Age = petsEdited.Age;
            masc.Color = petsEdited.Color;
            masc.LastServiceDate = petsEdited.LastServiceDate;
            masc.PhoneOwner = petsEdited.PhoneOwner;
            masc.EmailOwner = petsEdited.EmailOwner;
            masc.IdCustomer = petsEdited.IdCustomer;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexPets));
        }

        // GET : Delete Pets
        public async Task<IActionResult> Delete(int id)
        {
            var pets = await _context.Pets.Include(m => m.Customer).FirstOrDefaultAsync(m => m.PetId == id);
            if (pets == null) return NotFound();

            return View(pets);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pets = await _context.Pets.FindAsync(id);
            if (pets == null) return NotFound();

            try
            {
                _context.Pets.Remove(pets);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexPets));
            }
            catch (DbUpdateException ex)
            {
                string mensajeError = ex.InnerException != null && ex.InnerException.Message.Contains("FK_Procedures_Pets")
                    ? "The pet could not be removed because it has an associated procedure"
                    : "The pets could not be removed.";

                ModelState.AddModelError("", mensajeError);

                var petsConCustomer = await _context.Pets.Include(m => m.Customer).FirstOrDefaultAsync(m => m.PetId == id);
                return View("Delete", petsConCustomer);
            }
        }


        // Find customer with filters
        public async Task<IActionResult> Buscar(
        int Id = 0,
        int IdCustomer = 0,
        string PetName = "",
        AnimalSpecies? AnimalSpecies = null,
        string Breed = "",
        int? Age = null,
        string Color = "",
        DateTime? LastServiceDate = null,
        string PhoneOwner = "",
        string EmailOwner = ""
    )
        {
            var Pets = await _context.Pets.ToListAsync();
            var results = Pets.AsEnumerable();

            // Aplicar filtros dinámicos
            if (Id > 0)
                results = results.Where(p => p.PetId == Id);

            if (IdCustomer > 0)
                results = results.Where(p => p.IdCustomer == IdCustomer);

            if (!string.IsNullOrEmpty(PetName))
                results = results.Where(p => p.PetName.Contains(PetName, StringComparison.OrdinalIgnoreCase));

            if (AnimalSpecies.HasValue)
                results = results.Where(p => p.AnimalSpecies == AnimalSpecies.Value);

            if (!string.IsNullOrEmpty(Breed))
                results = results.Where(p => p.Breed.Contains(Breed, StringComparison.OrdinalIgnoreCase));

            if (Age.HasValue)
                results = results.Where(p => p.Age == Age.Value);

            if (!string.IsNullOrEmpty(Color))
                results = results.Where(p => p.Color != null && p.Color.Contains(Color, StringComparison.OrdinalIgnoreCase));

            if (LastServiceDate.HasValue)
                results = results.Where(p => p.LastServiceDate.HasValue && p.LastServiceDate.Value.Date == LastServiceDate.Value.Date);

            if (!string.IsNullOrEmpty(PhoneOwner))
                results = results.Where(p => p.PhoneOwner != null && p.PhoneOwner.Contains(PhoneOwner));

            if (!string.IsNullOrEmpty(EmailOwner))
                results = results.Where(p => p.EmailOwner != null && p.EmailOwner.Contains(EmailOwner, StringComparison.OrdinalIgnoreCase));


            ViewBag.Customers = await GetCustomersAsync();
            ViewBag.AnimalSpecies = GetAnimalSpecies();

            return View(results.ToList());
        }



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

        // Get AnimalSpeciess list
        private List<SelectListItem> GetAnimalSpecies()
        {
            return Enum.GetValues(typeof(AnimalSpecies))
                       .Cast<AnimalSpecies>()
                       .Select(e => new SelectListItem
                       {
                           Value = e.ToString(),
                           Text = e.ToString()
                       }).ToList();
        }
    }
}
