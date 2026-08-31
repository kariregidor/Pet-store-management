using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto1_1_1548_0877.Models
{
    public class Mascotas
    {
        [Key]
        [Required(ErrorMessage = "El número de Id es obligatorio")]
        public int MascotaId { get; set; }

        //Relación con Cliente
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente (Dueño)")]
        [ForeignKey("Cliente")]
        public int IdCliente { get; set; }

        public virtual Cliente? Cliente { get; set; } 

        [Required(ErrorMessage = "Nombre de la mascota es obligatorio")]
        [Display(Name = "Nombre de la mascota")]
        public string NombreMascota { get; set; } = string.Empty;

        [Display(Name = "Especie")]
        public Especie Especie { get; set; }

        [Required(ErrorMessage = "La raza es obligatoria")]
        public string Raza { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(0, 100, ErrorMessage = "La edad debe estar entre 0 y 100 años")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "El color es obligatorio")]
        public string? Color { get; set; }

        [Display(Name = "Última fecha de atención")]
        [DataType(DataType.Date)]
        public DateTime? UltimaFechaAtencion { get; set; }

        [Display(Name = "Teléfono del dueño/a")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe tener 8 dígitos numéricos")]
        public string? TelefonoDueno { get; set; }

        [Display(Name = "E-mail del dueño/a")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? EmailDueno { get; set; }
    }

    public enum Especie
    {
        Caballo,
        Perro,
        Gato,
        Pez,
        Cabra,
        Conejo,
        Vaca,
        Cerdo,
        Roedor
    }
}
