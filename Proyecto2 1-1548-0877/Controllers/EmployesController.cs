using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto2_1_1548_0877.Models;

namespace Proyecto2_1_1548_0877.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployesController : ControllerBase
    {
        private readonly PetsContext _context;

        public EmployesController(PetsContext context)
        {
            _context = context;
        }

        // GET: api/employes/readEmployes
        [HttpGet("readEmployes")]
        public async Task<ActionResult<IEnumerable<Employes>>> GetAll()
        {
            var employes = await _context.Employes.ToListAsync();
            return Ok(employes);
        }

        // POST: api/employes/createEmployes
        [HttpPost("createEmployes")]
        public async Task<ActionResult> Create([FromBody] Employes newEmploye)
        {
            if (await _context.Employes.AnyAsync(e => e.Id == newEmploye.Id))
                return Conflict("An employee with that ID number already exists.");

            _context.Employes.Add(newEmploye);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newEmploye.Id }, newEmploye);
        }

        // PUT: api/employes/editEmployes/{id}
        [HttpPut("editEmployes/{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Employes updatedEmploye)
        {
            var emp = await _context.Employes.FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null) return NotFound();

            emp.DateOfBirth = updatedEmploye.DateOfBirth;
            emp.StartDate = updatedEmploye.StartDate;
            emp.SalaryPerDay = updatedEmploye.SalaryPerDay;
            emp.RetirementDate = updatedEmploye.RetirementDate;
            emp.TypeEmploye = updatedEmploye.TypeEmploye;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/employes/deleteEmployes/{id}
        [HttpDelete("deleteEmployes/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var emp = await _context.Employes.FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null) return NotFound();

            _context.Employes.Remove(emp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/employes/findById/{id}
        [HttpGet("findById/{id}")]
        public async Task<ActionResult<Employes>> GetById(string id)
        {
            var emp = await _context.Employes.FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null) return NotFound();

            return Ok(emp);
        }

        // GET: api/employes/findByTypeEmploye?typeEmploye=Veterinary
        [HttpGet("findByTypeEmploye")]
        public async Task<ActionResult<IEnumerable<Employes>>> Search([FromQuery] string typeEmploye)
        {
            var results = _context.Employes.AsQueryable();

            if (!string.IsNullOrEmpty(typeEmploye))
            {
                if (Enum.TryParse<TypeEmploye>(typeEmploye, true, out var typeEnum))
                {
                    results = results.Where(e => e.TypeEmploye == typeEnum);
                }
                else
                {
                    return BadRequest("Invalid employee type.");
                }
            }

            return Ok(await results.ToListAsync());
        }
    }
}
