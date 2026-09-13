using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto2_1_1548_0877.Models;

namespace Proyecto2_1_1548_0877.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly PetsContext _context;

        public PetsController(PetsContext context)
        {
            _context = context;
        }

        // GET: api/pets/readPets
        [HttpGet("readPets")]
        public async Task<ActionResult<IEnumerable<Pets>>> GetAll()
        {
            var pets = await _context.Pets.ToListAsync();
            return Ok(pets);
        }

        // POST: api/pets/createPets
        [HttpPost("createPets")]
        public async Task<ActionResult> Create([FromBody] Pets newPet)
        {
            if (await _context.Pets.AnyAsync(m => m.PetId == newPet.PetId))
                return Conflict("A pet with that ID already exists.");

            _context.Pets.Add(newPet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newPet.PetId }, newPet);
        }

        // PUT: api/pets/editPets/{id}
        [HttpPut("editPets/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Pets updatedPet)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(m => m.PetId == id);
            if (pet == null) return NotFound();

            pet.PetName = updatedPet.PetName;
            pet.AnimalSpecies = updatedPet.AnimalSpecies;
            pet.Breed = updatedPet.Breed;
            pet.Age = updatedPet.Age;
            pet.Color = updatedPet.Color;
            pet.LastServiceDate = updatedPet.LastServiceDate;
            pet.PhoneOwner = updatedPet.PhoneOwner;
            pet.EmailOwner = updatedPet.EmailOwner;
            pet.IdCustomer = updatedPet.IdCustomer;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/pets/deletePets/{id}
        [HttpDelete("deletePets/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(m => m.PetId == id);
            if (pet == null)
                return NotFound(new { message = "Pet not found." });

            try
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                // Detect related FKs
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_Procedures_Pets"))
                {
                    return BadRequest(new
                    {
                        message = "The pet cannot be deleted because it has associated procedures."
                    });
                }

                // Other error
                return StatusCode(500, new
                {
                    message = "Error deleting the pet.",
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


        // GET: api/pets/findById/{id}
        [HttpGet("findById/{id}")]
        public async Task<ActionResult<Pets>> GetById(int id)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(m => m.PetId == id);
            if (pet == null) return NotFound();

            return Ok(pet);
        }

        // GET: api/pets/findByAnimalSpecies?animalSpecies=Dog
        [HttpGet("findByAnimalSpecies")]
        public async Task<ActionResult<IEnumerable<Pets>>> Search([FromQuery] string animalSpecies)
        {
            if (!Enum.TryParse<AnimalSpecies>(animalSpecies, out var speciesEnum))
                return BadRequest("Invalid species.");

            var results = await _context.Pets
                .Where(m => m.AnimalSpecies == speciesEnum)
                .ToListAsync();

            return Ok(results);
        }


        // GET: api/pets/existingAnimalSpecies
        [HttpGet("existingAnimalSpecies")]
        public ActionResult<IEnumerable<string>> GetExistingSpecies()
        {
            var species = Enum.GetNames(typeof(AnimalSpecies));
            return Ok(species);
        }
    }
}
