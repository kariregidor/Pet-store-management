using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{
    //EMPLEADOS CONTROLLER
    public class EmpleadosController : Controller
    {
        private readonly PetsContext _context;

        public EmpleadosController(PetsContext context)
        {
            _context = context;
        }

        // Listar empleados
        public async Task<IActionResult> IndexEmpleados()
        {
            var empleados = await _context.Empleados.ToListAsync();
            return View(empleados);
        }

        // Crear empleados
        public IActionResult Create()
        {
            ViewBag.TiposEmpleados = GetTiposEmpleados();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empleados empleadoNuevo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TiposEmpleados = GetTiposEmpleados();
                return View(empleadoNuevo);
            }

            if (await _context.Empleados.AnyAsync(e => e.Cedula == empleadoNuevo.Cedula))
            {
                ModelState.AddModelError("", "Ya existe un empleado con esa cédula.");
                ViewBag.TiposEmpleados = GetTiposEmpleados();
                return View(empleadoNuevo);
            }

            _context.Empleados.Add(empleadoNuevo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexEmpleados));
        }

        // Editar empleados
        public async Task<IActionResult> Edit(string cedula)
        {
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Cedula == cedula);
            if (empleado == null) return NotFound();

            ViewBag.TiposEmpleados = GetTiposEmpleados();
            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string cedula, Empleados empleadoEditado)
        {
            if (cedula != empleadoEditado.Cedula) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.TiposEmpleados = GetTiposEmpleados();
                return View(empleadoEditado);
            }

            var emp = await _context.Empleados.FirstOrDefaultAsync(e => e.Cedula == cedula);
            if (emp == null) return NotFound();

            emp.FechaNacimiento = empleadoEditado.FechaNacimiento;
            emp.FechaIngreso = empleadoEditado.FechaIngreso;
            emp.SalarioDia = empleadoEditado.SalarioDia;
            emp.FechaRetiro = empleadoEditado.FechaRetiro;
            emp.TipoEmpleado = empleadoEditado.TipoEmpleado;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexEmpleados));
        }

        // GET: Eliminar empleados
        public async Task<IActionResult> Delete(string cedula)
        {
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Cedula == cedula);
            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // POST:
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string cedula)
        {
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Cedula == cedula);
            if (empleado == null) return NotFound();

            try
            {
                _context.Empleados.Remove(empleado);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexEmpleados));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo eliminar el empleado.");
                return View("Delete", empleado);
            }
        }

        // Buscar empleados con filtros
        public async Task<IActionResult> Buscar(
            string cedula,
            DateTime? fechaNacimiento,
            DateTime? fechaIngreso,
            decimal? salarioDia,
            DateTime? fechaRetiro,
            TipoEmpleado? tipoEmpleado)
        {
            var empleados = await _context.Empleados.ToListAsync();
            var resultados = empleados.AsEnumerable();

            if (!string.IsNullOrEmpty(cedula))
                resultados = resultados.Where(e => e.Cedula.Contains(cedula));

            if (fechaNacimiento.HasValue)
                resultados = resultados.Where(e => e.FechaNacimiento.Date == fechaNacimiento.Value.Date);

            if (fechaIngreso.HasValue)
                resultados = resultados.Where(e => e.FechaIngreso.Date == fechaIngreso.Value.Date);

            if (salarioDia.HasValue)
                resultados = resultados.Where(e => e.SalarioDia == salarioDia.Value);

            if (fechaRetiro.HasValue)
                resultados = resultados.Where(e => e.FechaRetiro.HasValue && e.FechaRetiro.Value.Date == fechaRetiro.Value.Date);

            if (tipoEmpleado.HasValue)
                resultados = resultados.Where(e => e.TipoEmpleado == tipoEmpleado.Value);

            ViewBag.TiposEmpleados = GetTiposEmpleados();
            return View(resultados.ToList());
        }

        // Obtener lista de tipos de empleados
        private List<SelectListItem> GetTiposEmpleados() =>
            Enum.GetValues(typeof(TipoEmpleado))
                .Cast<TipoEmpleado>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();
    }
}
