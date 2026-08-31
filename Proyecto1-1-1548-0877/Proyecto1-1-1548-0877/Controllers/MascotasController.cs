using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{

    //MASCOTAS CONTROLLER
    public class MascotasController : Controller
    {
        private readonly PetsContext _context;

        public MascotasController(PetsContext context)
        {
            _context = context;
        }

        // LISTAR MASCOTAS
        public async Task<IActionResult> IndexMascotas()
        {
            var mascotas = await _context.Mascotas.Include(m => m.Cliente).ToListAsync();
            return View(mascotas);
        }

        // GET: Mascotas/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await GetClientesAsync();
            ViewBag.Especie = GetEspecie();
            return View();
        }

        // POST: Mascotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mascotas mascota)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = await GetClientesAsync();
                ViewBag.Especie = GetEspecie();
                return View(mascota);
            }

            if (await _context.Mascotas.AnyAsync(m => m.MascotaId == mascota.MascotaId))
            {
                ModelState.AddModelError("", "Ya existe una mascota con ese ID.");
                ViewBag.Clientes = await GetClientesAsync();
                ViewBag.Especie = GetEspecie();
                return View(mascota);
            }

            _context.Mascotas.Add(mascota);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexMascotas));
        }

        // GET: Mascotas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null) return NotFound();

            ViewBag.Clientes = await GetClientesAsync();
            ViewBag.Especie = GetEspecie();
            return View(mascota);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mascotas mascotaEditada)
        {
            if (id != mascotaEditada.MascotaId) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = await GetClientesAsync();
                ViewBag.Especie = GetEspecie();
                return View(mascotaEditada);
            }

            var masc = await _context.Mascotas.FindAsync(id);
            if (masc == null) return NotFound();

            masc.NombreMascota = mascotaEditada.NombreMascota;
            masc.Especie = mascotaEditada.Especie;
            masc.Raza = mascotaEditada.Raza;
            masc.Edad = mascotaEditada.Edad;
            masc.Color = mascotaEditada.Color;
            masc.UltimaFechaAtencion = mascotaEditada.UltimaFechaAtencion;
            masc.TelefonoDueno = mascotaEditada.TelefonoDueno;
            masc.EmailDueno = mascotaEditada.EmailDueno;
            masc.IdCliente = mascotaEditada.IdCliente;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexMascotas));
        }

        // GET : Eliminar mascotas
        public async Task<IActionResult> Delete(int id)
        {
            var mascota = await _context.Mascotas.Include(m => m.Cliente).FirstOrDefaultAsync(m => m.MascotaId == id);
            if (mascota == null) return NotFound();

            return View(mascota);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null) return NotFound();

            try
            {
                _context.Mascotas.Remove(mascota);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexMascotas));
            }
            catch (DbUpdateException ex)
            {
                string mensajeError = ex.InnerException != null && ex.InnerException.Message.Contains("FK_Procedimientos_Mascotas")
                    ? "No se pudo eliminar la mascota por que tiene un procedimiento asociado"
                    : "No se pudo eliminar la mascota.";

                ModelState.AddModelError("", mensajeError);

                var mascotaConCliente = await _context.Mascotas.Include(m => m.Cliente).FirstOrDefaultAsync(m => m.MascotaId == id);
                return View("Delete", mascotaConCliente);
            }
        }


        // Buscar clientes con filtros
        public async Task<IActionResult> Buscar(
        int Id = 0,
        int IdCliente = 0,
        string NombreMascota = "",
        Especie? especie = null,
        string Raza = "",
        int? Edad = null,
        string Color = "",
        DateTime? UltimaFechaAtencion = null,
        string TelefonoDueno = "",
        string EmailDueno = ""
    )
        {
            var mascotas = await _context.Mascotas.ToListAsync();
            var resultados = mascotas.AsEnumerable();

            // Aplicar filtros dinámicos
            if (Id > 0)
                resultados = resultados.Where(m => m.MascotaId == Id);

            if (IdCliente > 0)
                resultados = resultados.Where(m => m.IdCliente == IdCliente);

            if (!string.IsNullOrEmpty(NombreMascota))
                resultados = resultados.Where(m => m.NombreMascota.Contains(NombreMascota, StringComparison.OrdinalIgnoreCase));

            if (especie.HasValue)
                resultados = resultados.Where(m => m.Especie == especie.Value);

            if (!string.IsNullOrEmpty(Raza))
                resultados = resultados.Where(m => m.Raza.Contains(Raza, StringComparison.OrdinalIgnoreCase));

            if (Edad.HasValue)
                resultados = resultados.Where(m => m.Edad == Edad.Value);

            if (!string.IsNullOrEmpty(Color))
                resultados = resultados.Where(m => m.Color != null && m.Color.Contains(Color, StringComparison.OrdinalIgnoreCase));

            if (UltimaFechaAtencion.HasValue)
                resultados = resultados.Where(m => m.UltimaFechaAtencion.HasValue && m.UltimaFechaAtencion.Value.Date == UltimaFechaAtencion.Value.Date);

            if (!string.IsNullOrEmpty(TelefonoDueno))
                resultados = resultados.Where(m => m.TelefonoDueno != null && m.TelefonoDueno.Contains(TelefonoDueno));

            if (!string.IsNullOrEmpty(EmailDueno))
                resultados = resultados.Where(m => m.EmailDueno != null && m.EmailDueno.Contains(EmailDueno, StringComparison.OrdinalIgnoreCase));

            // Si quieres llenar dropdowns en la vista, puedes usar ViewBag
            ViewBag.Clientes = await GetClientesAsync();
            ViewBag.Especie = GetEspecie();

            return View(resultados.ToList());
        }



        // Obtener lista de clientes
        private async Task<List<SelectListItem>> GetClientesAsync()
        {
            var clientes = await _context.Clientes.ToListAsync();

            return clientes.Select(c => new SelectListItem
            {
                Value = c.IdCliente.ToString(),
                Text = c.NombreCompleto
            }).ToList();
        }

        // Obtener lista de especies
        private List<SelectListItem> GetEspecie()
        {
            return Enum.GetValues(typeof(Especie))
                       .Cast<Especie>()
                       .Select(e => new SelectListItem
                       {
                           Value = e.ToString(),
                           Text = e.ToString()
                       }).ToList();
        }
    }
}
