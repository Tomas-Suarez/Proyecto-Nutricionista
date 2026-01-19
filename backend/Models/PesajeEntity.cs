using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Pesaje")]
    public class PesajeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Pesaje { get; set; }

        [Required]
        public int Id_Paciente { get; set; }

        [ForeignKey("Id_Paciente")]
        public virtual PacienteEntity Paciente { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Peso_Kg { get; set; }

        [Column(TypeName = "decimal(4,1)")]
        public decimal? Porcentaje_Grasa { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Masa_Muscular_Kg { get; set; }

        [Required]
        public DateTime Fecha_Pesaje { get; set; } = DateTime.Now;

        [StringLength(256)]
        public string? Nota { get; set; } = string.Empty;
    }
}
