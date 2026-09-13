using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto2_1_1548_0877.Data;
using Proyecto2_1_1548_0877.Models;

namespace Proyecto2_1_1548_0877.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProceduresController : ControllerBase
    {
        private readonly PetsContext _context;

        public ProceduresController(PetsContext context)
        {
            _context = context;
        }

        // GET: api/procedures/readProcedures
        [HttpGet("readProcedures")]
        public async Task<ActionResult<IEnumerable<Procedures>>> GetAll()
        {
            var procedures = await _context.Procedures
                .Include(p => p.Pets)
                .ToListAsync();

            return Ok(procedures);
        }


        [HttpPost("createProcedure")]
        public async Task<IActionResult> CreateProcedure([FromBody] Procedures procedure)
        {
            try
            {
                // Validate that the pet exists
                var petExists = await _context.Pets
                    .AnyAsync(m => m.PetId == procedure.PetId);

                if (!petExists)
                    return BadRequest(new { message = "The specified pet does not exist" });

                // Automatically assign the price based on the procedure type
                procedure.BasePrice = GetBasePrice(procedure.ProcedureType);
                procedure.Pets = null;

                // Save procedure
                _context.Procedures.Add(procedure);
                await _context.SaveChangesAsync();

                // Automatically create a report after inserting
                await CreateReportFromProcedure(procedure.Id);
                return Ok(new
                {
                    message = "Procedure created successfully",
                    Id = procedure.Id,
                    BasePrice = procedure.BasePrice,
                    VAT = procedure.VAT,
                    TotalPrice = procedure.TotalPrice
                });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null &&
                    (ex.InnerException.Message.Contains("PRIMARY KEY") ||
                     ex.InnerException.Message.Contains("duplicate")))
                {
                    return BadRequest(new
                    {
                        message = "A procedure with that number already exists",
                        detail = ex.InnerException.Message
                    });
                }

                return StatusCode(500, new
                {
                    message = "Error saving the procedure",
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // Helper method to get the base price based on the procedure type
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


        // PUT: api/procedures/editProcedure/{id}
        [HttpPut("editProcedure/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Procedures updatedProcedure)
        {
            try
            {
                var proc = await _context.Procedures.FindAsync(id);
                if (proc == null) return NotFound();

                // Verify the pet exists if it changed
                if (proc.PetId != updatedProcedure.PetId)
                {
                    var petExists = await _context.Pets
                        .AnyAsync(m => m.PetId == updatedProcedure.PetId);

                    if (!petExists)
                    {
                        return BadRequest(new { message = "The specified pet does not exist" });
                    }
                }

                // Update only the necessary fields
                proc.IdCustomer = updatedProcedure.IdCustomer;
                proc.PetId = updatedProcedure.PetId;
                proc.ProcedureType = updatedProcedure.ProcedureType;
                proc.Weight = updatedProcedure.Weight;
                proc.BasePrice = updatedProcedure.BasePrice;
                proc.Status = updatedProcedure.Status;

                await _context.SaveChangesAsync();

                // Update or create the report
                await UpdateReportFromProcedure(proc.Id);

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating the procedure",
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // DELETE: api/procedures/deleteProcedure/{id}
        [HttpDelete("deleteProcedure/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null)
                return NotFound(new { message = "Procedure not found" });

            try
            {
                // First delete the associated report (if it exists) to avoid FK issues
                await DeleteReportByProcedure(id);

                _context.Procedures.Remove(procedure);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                // Handle FK errors or other database issues
                var inner = ex.InnerException?.Message ?? ex.Message;
                if (inner.Contains("FK_") || inner.Contains("REFERENCE"))
                {
                    return BadRequest(new { message = "The procedure cannot be deleted because it has related data." });
                }

                return StatusCode(500, new
                {
                    message = "Error deleting the procedure",
                    detail = inner
                });
            }
        }


        // GET: api/procedures/findById/{id}
        [HttpGet("findById/{id}")]
        public async Task<ActionResult<Procedures>> GetById(int id)
        {
            var proc = await _context.Procedures
                .Include(p => p.Pets)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proc == null) return NotFound();

            return Ok(proc);
        }

        //METHODS
        //Create report
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

        //Update report
        private async Task UpdateReportFromProcedure(int procedureId)
        {
            // Find report by ProcedureId
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

        //Delete report
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
