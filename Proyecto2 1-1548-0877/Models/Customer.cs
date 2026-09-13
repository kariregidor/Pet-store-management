using System.ComponentModel.DataAnnotations;

namespace Proyecto2_1_1548_0877.Models
{
    public class Customer
    {
        public int IdCustomer { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;



        public ContactPreference? ContactPreference { get; set; }

        // Navigation

    }

    public enum ContactPreference
    {
        Call,
        Whatsapp
    }
}
