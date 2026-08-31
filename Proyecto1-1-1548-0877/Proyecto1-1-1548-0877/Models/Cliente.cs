using System.ComponentModel.DataAnnotations;

namespace Proyecto1_1_1548_0877.Models
{
    public class Cliente
    {
        [Key]
        [Display(Name = "Identificación")]
        [Required(ErrorMessage = "La identificación es obligatoria")]
        public int IdCliente { get; set; }

        [Display(Name = "Nombre completo")]
        [Required(ErrorMessage = "El nombre y apellidos son obligatorios")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Provincia")]
        [Required(ErrorMessage = "La provincia es obligatoria")]
        public string Provincia { get; set; } = string.Empty;

        [Display(Name = "Cantón")]
        [Required(ErrorMessage = "El cantón es obligatorio")]
        public string Canton { get; set; } = string.Empty;

        [Display(Name = "Distrito")]
        [Required(ErrorMessage = "El distrito es obligatorio")]
        public string Distrito { get; set; } = string.Empty;

        [Display(Name = "Dirección exacta")]
        [Required(ErrorMessage = "La dirección exacta es obligatoria")]
        public string DireccionExacta { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string Telefono { get; set; } = string.Empty;

        [Display(Name = "Preferencia de contacto")]
        public PreferenciaContacto PreferenciaContacto { get; set; }
    }

    public enum PreferenciaContacto
    {
        Llamada,
        Whatsapp
    }
}
   
