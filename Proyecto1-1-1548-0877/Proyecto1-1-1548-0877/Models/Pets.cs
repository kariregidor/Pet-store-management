using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto1_1_1548_0877.Models
{
    public class Pets
    {
        [Key]
        [Required(ErrorMessage = "Id is mandatory")]
        public int PetId { get; set; }

        //Relation with Customer
        [Required(ErrorMessage = "You have to select a customer")]
        [Display(Name = "Customer (Owner)")]
        [ForeignKey("Customer")]
        public int IdCustomer { get; set; }

        public virtual Customer? Customer { get; set; }

        [Required(ErrorMessage = "Pet name is mandatory")]
        [Display(Name = "Pet name")]
        public string PetName { get; set; } = string.Empty;

        [Display(Name = "Animal Species")]
        public AnimalSpecies AnimalSpecies { get; set; }

        [Required(ErrorMessage = "The breed is mandatory.")]
        public string Breed { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is mandatory")]
        [Range(0, 100, ErrorMessage = "Age must be between 0 and 100 years")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Color is mandatory")]
        public string? Color { get; set; }

        [Display(Name = "Last date of service")]
        [DataType(DataType.Date)]
        public DateTime? LastServiceDate { get; set; }

        [Display(Name = "Owners phone")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "The phone number must have 8 numbers.")]
        public string? PhoneOwner { get; set; }

        [Display(Name = "E-mail owner")]
        [EmailAddress(ErrorMessage = "The email format is invalid")]
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
