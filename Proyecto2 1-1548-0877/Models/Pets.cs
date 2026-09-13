
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto2_1_1548_0877.Models
{
    public class Pets
    {
        [Key]
        [Required(ErrorMessage = "The Id number is mandatory")]
        public int PetId { get; set; }

        // Relation with Customer - uses IdCustomer as Foreign Key
        [Required(ErrorMessage = "You must select a customer")]
        [Display(Name = "Customer (Owner)")]
        [ForeignKey("Customer")]
        public int IdCustomer { get; set; }

        public virtual Customer? Customer { get; set; }

        [Required(ErrorMessage = "The pet's name is mandatory")]
        [Display(Name = "PetName")]
        public string PetName { get; set; } = string.Empty;


        [Display(Name = "Species")]
        public AnimalSpecies AnimalSpecies { get; set; }  // Horse, dog, cat, etc.

        [Required(ErrorMessage = "The breed is mandatory")]
        [Display(Name = "Breed")]
        public string Breed { get; set; } = string.Empty;

        [Required(ErrorMessage = "The age is mandatory")]
        [Range(0, 100, ErrorMessage = "The age must be between 0 and 100 years")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "The color is mandatory")]
        public string? Color { get; set; }

        [Display(Name = "Last service date")]
        [DataType(DataType.Date)]
        public DateTime? LastServiceDate { get; set; }

        [Display(Name = "Owner's phone")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "The phone number must have 8 digits")]
        public string? PhoneOwner { get; set; }

        [Display(Name = "Owner's e-mail")]
        [EmailAddress(ErrorMessage = "The email format is not valid")]
        public string? EmailOwner { get; set; }




    }
    public enum AnimalSpecies
    {
        Horse,
        Dog,
        Cat,
        Fish,
        Goat,
        Rabbit,
        Cow,
        Pig,
        Rodent

    }
}

