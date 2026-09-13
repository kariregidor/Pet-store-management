using System.ComponentModel.DataAnnotations;

namespace Proyecto1_1_1548_0877.Models
{
    public class Customer
    {
        [Key]
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is mandatory")]
        public int IdCustomer { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name and last name are mandatory")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is mandatory")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "State")]
        [Required(ErrorMessage = "State is mandatory")]
        public string State { get; set; } = string.Empty;

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is mandatory")]
        public string Country { get; set; } = string.Empty;

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Adress is mandatory")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Phone number")]
        [Required(ErrorMessage = "Phone number is mandatory")]
        [Phone(ErrorMessage = "Phone number is not valid")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Contact preference")]
        public ContactPreference ContactPreference { get; set; }
    }

    public enum ContactPreference
    {
        Call,
        Whatsapp
    }
}

