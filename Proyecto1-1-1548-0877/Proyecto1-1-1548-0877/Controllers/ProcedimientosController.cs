using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto1_1_1548_0877.Models;

namespace Proyecto1_1_1548_0877.Controllers
{
    //PROCEDIMIENTOS CONTROLLER
    public class ProcedimientosController : Controller
    {
        private readonly PetsContext _context;

        public ProcedimientosController(PetsContext context)
        {
            _context = context;
        }

        // Listar procedimientos
        public async Task<IActionResult> IndexProcedimientos()
        {
            var procedimientos = await _context.Procedimientos
                .Include(p => p.Mascota)
                .ToListAsync();

            return View(procedimientos);
        }

        // GET: Crear PROCEDIMIENTO
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await GetClientesAsync();
            ViewBag.TipoProcedimiento = GetTipoProcedimiento();
            ViewBag.EstadoProcedimiento = GetEstadoProcedimiento();

            return View();
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Procedimientos procedimiento)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Clientes = await GetClientesAsync();
                    ViewBag.TipoProcedimiento = GetTipoProcedimiento();
                    ViewBag.EstadoProcedimiento = GetEstadoProcedimiento();
                    return View(procedimiento);
                }

                var mascotaExiste = await _context.Mascotas.AnyAsync(m => m.MascotaId == procedimiento.MascotaId);
                if (!mascotaExiste)
                {
                    ModelState.AddModelError("", "La mascota especificada no existe");
                }
                else
                {
                    // Asignar precio automáticamente según el tipo de procedimiento
                    procedimiento.PrecioBase = ObtenerPrecioBase(procedimiento.TipoProcedimiento);
                    procedimiento.Mascota = null;

                    _context.Procedimientos.Add(procedimiento);
                    await _context.SaveChangesAsync();

                    // Crear reporte automáticamente después de insertar
                    await CrearReporteDesdeProcedimiento(procedimiento.Id);

                    TempData["Success"] = "Procedimiento creado exitosamente";
                    return RedirectToAction("IndexProcedimientos");
                }
            }
            catch (DbUpdateException ex)
            {
                var mensaje = ex.InnerException != null &&
                    (ex.InnerException.Message.Contains("PRIMARY KEY") || ex.InnerException.Message.Contains("duplicate"))
                    ? "Ya existe un procedimiento con ese número"
                    : $"Error al guardar el procedimiento: {ex.InnerException?.Message ?? ex.Message}";

                ModelState.AddModelError("", mensaje);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            ViewBag.Clientes = await GetClientesAsync();
            ViewBag.TipoProcedimiento = GetTipoProcedimiento();
            ViewBag.EstadoProcedimiento = GetEstadoProcedimiento();
            return View(procedimiento);
        }

        // Obtener mascotas por cliente
        [HttpGet]
        public async Task<JsonResult> GetMascotasByCliente(int idCliente)
        {
            var mascotasCliente = await GetMascotasByClienteAsync(idCliente);
            return Json(mascotasCliente);
        }

        // GET: Editar procedimiento
        public async Task<IActionResult> Edit(int id)
        {
            var procedimiento = await _context.Procedimientos.FindAsync(id);
            if (procedimiento == null) return NotFound();

            ViewBag.Clientes = await GetClientesAsync();
            ViewBag.TipoProcedimiento = GetTipoProcedimiento();
            ViewBag.EstadoProcedimiento = GetEstadoProcedimiento();

            // Cargar mascotas del cliente asociado
            ViewBag.Mascotas = await GetMascotasByClienteAsync(procedimiento.IdCliente);

            return View(procedimiento);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Procedimientos procedimiento)
        {
            if (id != procedimiento.Id)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Clientes = await GetClientesAsync();
                    ViewBag.TipoProcedimiento = GetTipoProcedimiento();
                    ViewBag.EstadoProcedimiento = GetEstadoProcedimiento();
                    ViewBag.Mascotas = await GetMascotasByClienteAsync(procedimiento.IdCliente);
                    return View(procedimiento);
                }

                var pro = await _context.Procedimientos.FindAsync(id);
                if (pro == null) return NotFound();

                // Verificar que la mascota existe si se cambia
                if (pro.MascotaId != procedimiento.MascotaId)
                {
                    var mascotaExiste = await _context.Mascotas.AnyAsync(m => m.MascotaId == procedimiento.MascotaId);

                    if (!mascotaExiste)
                    {
                        ModelState.AddModelError("", "La mascota especificada no existe");
                        ViewBag.Clientes = await GetClientesAsync();
                        ViewBag.TipoProcedimiento = GetTipoProcedimiento();
                        ViewBag.EstadoProcedimiento = GetEstadoProcedimiento();
                        ViewBag.Mascotas = await GetMascotasByClienteAsync(procedimiento.IdCliente);
                        return View(procedimiento);
                    }
                }

                // Actualizar solo los campos necesarios
                pro.IdCliente = procedimiento.IdCliente;
                pro.MascotaId = procedimiento.MascotaId;
                pro.TipoProcedimiento = procedimiento.TipoProcedimiento;
                pro.Peso = procedimiento.Peso;
                pro.PrecioBase = procedimiento.PrecioBase;
                pro.Estado = procedimiento.Estado;

                await _context.SaveChangesAsync();

                // Actualizar o crear el reporte
                await ActualizarReporteDesdeProcedimiento(pro.Id);

                TempData["Success"] = "Procedimiento actualizado exitosamente";
                return RedirectToAction("IndexProcedimientos");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            ViewBag.Clientes = await GetClientesAsync();
            ViewBag.TipoProcedimiento = GetTipoProcedimiento();
            ViewBag.EstadoProcedimiento = GetEstadoProcedimiento();
            ViewBag.Mascotas = await GetMascotasByClienteAsync(procedimiento.IdCliente);
            return View(procedimiento);
        }

        // GET: Eliminar procedimiento
        public async Task<IActionResult> Delete(int id)
        {
            var procedimiento = await _context.Procedimientos.Include(p => p.Mascota).FirstOrDefaultAsync(p => p.Id == id);
            if (procedimiento == null) return NotFound();

            return View(procedimiento);
        }

        // POST:
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var procedimiento = await _context.Procedimientos.FindAsync(id);
            if (procedimiento == null) return NotFound(new { mensaje = "Procedimiento no encontrado" });

            try
            {
                // Primero eliminar reporte asociado (si existe) para evitar FK issues
                await EliminarReportePorProcedimiento(id);

                _context.Procedimientos.Remove(procedimiento);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Procedimiento eliminado exitosamente";
                return RedirectToAction(nameof(IndexProcedimientos));
            }
            catch (DbUpdateException ex)
            {
                // Manejar errores de FK u otros problemas de BD
                var inner = ex.InnerException?.Message ?? ex.Message;
                var mensaje = inner.Contains("FK_") || inner.Contains("REFERENCE")
                    ? "No se puede eliminar el procedimiento porque tiene datos relacionados."
                    : $"Error al eliminar el procedimiento: {inner}";

                ModelState.AddModelError("", mensaje);

                var procedimientoConMascota = await _context.Procedimientos.Include(p => p.Mascota).FirstOrDefaultAsync(p => p.Id == id);
                return View("Delete", procedimientoConMascota);
            }
        }

        // Buscar procedimientos con filtros
        public async Task<IActionResult> Buscar(
            int Id = 0,
            int IdCliente = 0,
            int MascotaId = 0,
            TipoProcedimiento? tipoProcedimiento = null,
            double? peso = null,
            decimal? PrecioBase = null,
            decimal? IVA = null,
            decimal? PrecioTotal = null,
            EstadoProcedimiento? estadoProcedimiento = null)
        {
            try
            {
                var procedimientos = await _context.Procedimientos.Include(p => p.Mascota).ToListAsync();
                var resultados = procedimientos.AsEnumerable();

                // Aplicar filtros
                if (Id > 0) resultados = resultados.Where(p => p.Id == Id);
                if (IdCliente > 0) resultados = resultados.Where(p => p.IdCliente == IdCliente);
                if (MascotaId > 0) resultados = resultados.Where(p => p.MascotaId == MascotaId);
                if (tipoProcedimiento.HasValue) resultados = resultados.Where(p => p.TipoProcedimiento == tipoProcedimiento.Value);
                if (peso.HasValue) resultados = resultados.Where(p => p.Peso.HasValue && p.Peso.Value == peso.Value);
                if (PrecioBase.HasValue) resultados = resultados.Where(p => p.PrecioBase == PrecioBase.Value);
                if (IVA.HasValue) resultados = resultados.Where(p => p.IVA == IVA.Value);
                if (PrecioTotal.HasValue) resultados = resultados.Where(p => p.PrecioTotal == PrecioTotal.Value);
                if (estadoProcedimiento.HasValue) resultados = resultados.Where(p => p.Estado == estadoProcedimiento.Value);

                // Cargar dropdowns
                ViewBag.Clientes = await GetClientesAsync();
                ViewBag.TipoProcedimiento = GetTipoProcedimiento();
                ViewBag.EstadoProcedimiento = GetEstadoProcedimiento();

                return View(resultados.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Buscar: {ex.Message}");
                return View(new List<Procedimientos>());
            }
        }

        // ============ MÉTODOS AUXILIARES ============

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

        // Obtener mascotas filtradas por cliente
        private async Task<List<SelectListItem>> GetMascotasByClienteAsync(int idCliente)
        {
            if (idCliente == 0)
                return new List<SelectListItem>();

            var mascotas = await _context.Mascotas.Where(m => m.IdCliente == idCliente).ToListAsync();

            return mascotas
                .Select(m => new SelectListItem
                {
                    Value = m.MascotaId.ToString(),
                    Text = m.NombreMascota
                })
                .ToList();
        }

        // Obtener lista de tipo de procedimiento
        private List<SelectListItem> GetTipoProcedimiento() =>
            Enum.GetValues(typeof(TipoProcedimiento))
                .Cast<TipoProcedimiento>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();

        // Obtener lista de estado procedimiento
        private List<SelectListItem> GetEstadoProcedimiento() =>
            Enum.GetValues(typeof(EstadoProcedimiento))
                .Cast<EstadoProcedimiento>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();

        // Método auxiliar para obtener el precio base según el tipo de procedimiento
        private decimal ObtenerPrecioBase(TipoProcedimiento tipo)
        {
            return tipo switch
            {
                TipoProcedimiento.Consulta => 15000m,
                TipoProcedimiento.ConsultaHorarioEspecial => 17000m,
                TipoProcedimiento.Castracion_0_5kg => 35000m,
                TipoProcedimiento.Castracion_5_10kg => 45000m,
                TipoProcedimiento.Castracion_10_20kg => 55000m,
                TipoProcedimiento.Castracion_20_30kg => 80000m,
                TipoProcedimiento.Castracion_30_50kg => 100000m,
                TipoProcedimiento.CirugiaMenor => 15000m,
                TipoProcedimiento.CirugiaMayor => 250000m,
                TipoProcedimiento.Grooming_Pequeña => 15000m,
                TipoProcedimiento.Grooming_Mediana => 20000m,
                TipoProcedimiento.Grooming_Grande => 125000m,
                TipoProcedimiento.Grooming_ExtraGrande => 35000m,
                TipoProcedimiento.VacunasAnuales => 40000m,
                _ => 0m
            };
        }

        //METODOS
        //Crear reporte
        private async Task CrearReporteDesdeProcedimiento(int procedimientoId)
        {
            var procedimiento = await _context.Procedimientos
                .Include(p => p.Mascota)
                .FirstOrDefaultAsync(p => p.Id == procedimientoId);

            if (procedimiento == null) return;

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == procedimiento.IdCliente);

            var nuevoReporte = new Reportes
            {
                ProcedimientoId = procedimiento.Id, // FK
                IdCliente = procedimiento.IdCliente,
                MascotaId = procedimiento.MascotaId,
                TipoProcedimiento = procedimiento.TipoProcedimiento,
                EstadoProcedimiento = procedimiento.Estado,
                Peso = procedimiento.Peso,
                PrecioBase = procedimiento.PrecioBase,
                IVA = procedimiento.IVA,
                PrecioTotal = procedimiento.PrecioTotal,
                FechaVacunacion = DateTime.Now.AddDays(7),
                NombreCliente = cliente?.NombreCompleto,
                NombreMascota = procedimiento.Mascota?.NombreMascota
            };

            _context.Reportes.Add(nuevoReporte);
            await _context.SaveChangesAsync();
        }

        //Actualizar reporte
        private async Task ActualizarReporteDesdeProcedimiento(int procedimientoId)
        {
            // Buscar reporte por ProcedimientoId
            var reporte = await _context.Reportes
                .FirstOrDefaultAsync(r => r.ProcedimientoId == procedimientoId);

            if (reporte == null)
            {
                // Si no existe, crear uno nuevo
                await CrearReporteDesdeProcedimiento(procedimientoId);
                return;
            }

            var procedimiento = await _context.Procedimientos
                .Include(p => p.Mascota)
                .FirstOrDefaultAsync(p => p.Id == procedimientoId);

            if (procedimiento == null) return;

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == procedimiento.IdCliente);

            // Actualizar campos del reporte
            reporte.IdCliente = procedimiento.IdCliente;
            reporte.MascotaId = procedimiento.MascotaId;
            reporte.TipoProcedimiento = procedimiento.TipoProcedimiento;
            reporte.EstadoProcedimiento = procedimiento.Estado;
            reporte.Peso = procedimiento.Peso;
            reporte.PrecioBase = procedimiento.PrecioBase;
            reporte.IVA = procedimiento.IVA;
            reporte.PrecioTotal = procedimiento.PrecioTotal;
            reporte.NombreCliente = cliente?.NombreCompleto;
            reporte.NombreMascota = procedimiento.Mascota?.NombreMascota;

            await _context.SaveChangesAsync();
        }

        //Eliminar reporte
        private async Task EliminarReportePorProcedimiento(int procedimientoId)
        {
            var reporte = await _context.Reportes
                .FirstOrDefaultAsync(r => r.ProcedimientoId == procedimientoId);

            if (reporte != null)
            {
                _context.Reportes.Remove(reporte);
                await _context.SaveChangesAsync();
            }
        }
    }
}
