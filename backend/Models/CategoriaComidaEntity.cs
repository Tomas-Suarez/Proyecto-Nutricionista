using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Categoria_Comida")]
    public class CategoriaComidaEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Categoria_Comida { get; set; }

        [Required]
        public int Id_Comida { get; set; }

        [ForeignKey("Id_Comida")]
        public virtual ComidaEntity Comida { get; set; } = null!;

        [Required]
        public int Id_Categoria { get; set; }

        [ForeignKey("Id_Categoria")]
        public virtual CategoriaEntity Categoria { get; set; } = null!;

    }
}
