using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto2_1_1548_0877.Models;

namespace Proyecto2_1_1548_0877.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly PetsContext _context;

        public CustomersController(PetsContext context)
        {
            _context = context;
        }

        // GET: api/customers/readCustomer
        [HttpGet("readCustomer")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            var customers = await _context.Customer.ToListAsync();
            return Ok(customers);
        }

        // GET: api/customers/findByIdCustomer/{IdCustomer}
        [HttpGet("findByIdCustomer/{IdCustomer}")]
        public async Task<ActionResult<Customer>> GetByIdCustomer(int IdCustomer)
        {
            var customer = await _context.Customer.FindAsync(IdCustomer);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        // POST: api/customers/createCustomer
        [HttpPost("createCustomer")]
        public async Task<ActionResult> Create([FromBody] Customer newCustomer)
        {
            if (await _context.Customer.AnyAsync(c => c.IdCustomer == newCustomer.IdCustomer))
                return Conflict("A customer with that ID already exists.");

            _context.Customer.Add(newCustomer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdCustomer), new { IdCustomer = newCustomer.IdCustomer }, newCustomer);
        }

        // PUT: api/customers/editCustomer/{IdCustomer}
        [HttpPut("editCustomer/{IdCustomer}")]
        public async Task<ActionResult> Update(int IdCustomer, [FromBody] Customer updatedCustomer)
        {
            if (IdCustomer != updatedCustomer.IdCustomer)
                return BadRequest();

            var existing = await _context.Customer.FindAsync(IdCustomer);
            if (existing == null)
                return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(updatedCustomer);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/customers/deleteCustomer/{IdCustomer}
        [HttpDelete("deleteCustomer/{IdCustomer}")]
        public async Task<IActionResult> Delete(int IdCustomer)
        {
            var customer = await _context.Customer.FindAsync(IdCustomer);
            if (customer == null)
                return NotFound("Customer not found.");

            try
            {
                _context.Customer.Remove(customer);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Customer deleted successfully." });
            }
            catch (DbUpdateException ex)
            {
                // Detect related FKs
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_Pets_Customers"))
                {
                    return BadRequest(new { message = "The customer cannot be deleted because it has associated pets." });
                }
                // Other error
                return StatusCode(500, new { message = $"Error deleting the customer: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Exception: {ex.Message}" });
            }
        }



        // GET: api/customers/findByContactPreference?contactPreference=Whatsapp
        [HttpGet("findByContactPreference")]
        public async Task<ActionResult<IEnumerable<Customer>>> Search([FromQuery] ContactPreference? contactPreference)
        {
            var query = _context.Customer.AsQueryable();

            if (contactPreference.HasValue)
                query = query.Where(c => c.ContactPreference == contactPreference);

            return Ok(await query.ToListAsync());
        }
    }
}
