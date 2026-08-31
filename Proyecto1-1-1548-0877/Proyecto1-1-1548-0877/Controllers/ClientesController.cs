using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{
    //CLIENTE CONTROLLER
    public class ClientesController : Controller
    {
        private readonly PetsContext _context;

        public ClientesController(PetsContext context)
        {
            _context = context;
        }

        // Listar clientes
        public async Task<IActionResult> IndexClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return View(clientes);
        }

        // Obtener lista de preferencia contacto
        private List<SelectListItem> GetPreferenciaContacto() =>
            Enum.GetValues(typeof(PreferenciaContacto))
                .Cast<PreferenciaContacto>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();

        // Crear cliente
        public IActionResult Create()
        {
            ViewBag.PreferenciaContacto = GetPreferenciaContacto();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente clienteNuevo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.PreferenciaContacto = GetPreferenciaContacto();
                    return View(clienteNuevo);
                }

                if (await _context.Clientes.AnyAsync(c => c.IdCliente == clienteNuevo.IdCliente))
                {
                    ModelState.AddModelError("", "Ya existe un cliente con esa identificación.");
                    ViewBag.PreferenciaContacto = GetPreferenciaContacto();
                    return View(clienteNuevo);
                }

                _context.Clientes.Add(clienteNuevo);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexClientes));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Excepción: {ex.Message}");
                ViewBag.PreferenciaContacto = GetPreferenciaContacto();
                return View(clienteNuevo);
            }
        }

        // Editar cliente - GET
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            ViewBag.PreferenciaContacto = GetPreferenciaContacto();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente clienteEditado)
        {
            if (id != clienteEditado.IdCliente) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.PreferenciaContacto = GetPreferenciaContacto();
                return View(clienteEditado);
            }

            var existente = await _context.Clientes.FindAsync(id);
            if (existente == null) return NotFound();

            _context.Entry(existente).CurrentValues.SetValues(clienteEditado);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexClientes));
        }

        // Eliminar clientes
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            try
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexClientes));
            }
            catch (DbUpdateException ex)
            {
                string mensaje = ex.InnerException != null && ex.InnerException.Message.Contains("FK_Mascotas_Clientes")
                    ? "No se puede eliminar el cliente porque tiene mascotas asociadas."
                    : $"Error al eliminar el cliente: {ex.InnerException?.Message ?? ex.Message}";

                ModelState.AddModelError("", mensaje);
                return View("Delete", cliente);
            }
        }

        // Buscar clientes con filtros
        public async Task<IActionResult> Buscar(
            int IdCliente,
            string NombreCompleto,
            string Provincia,
            string canton,
            string distrito,
            string direccionExacta,
            string telefono,
            PreferenciaContacto? PreferenciaContacto)
        {
            var clientes = await _context.Clientes.ToListAsync();
            var resultados = clientes.AsEnumerable();

            // Aplicar filtros dinámicos
            if (IdCliente > 0)
                resultados = resultados.Where(m => m.IdCliente == IdCliente);

            if (!string.IsNullOrEmpty(NombreCompleto))
                resultados = resultados.Where(m => m.NombreCompleto.Contains(NombreCompleto, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(Provincia))
                resultados = resultados.Where(m => m.Provincia.Contains(Provincia, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(canton))
                resultados = resultados.Where(m => m.Canton.Contains(canton, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(distrito))
                resultados = resultados.Where(m => m.Distrito.Contains(distrito, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(direccionExacta))
                resultados = resultados.Where(m => m.DireccionExacta.Contains(direccionExacta, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(telefono))
                resultados = resultados.Where(m => m.Telefono.Contains(telefono, StringComparison.OrdinalIgnoreCase));

            if (PreferenciaContacto.HasValue)
                resultados = resultados.Where(m => m.PreferenciaContacto == PreferenciaContacto.Value);

            ViewBag.PreferenciaContacto = GetPreferenciaContacto();
            return View(resultados.ToList());
        }

    }
}
