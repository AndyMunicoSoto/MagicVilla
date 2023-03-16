using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Modelos.Dto
{
    //esto es para no trbajar directamente con el modelo
    public class VillaUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; }
        public string Detalles { get; set; }

        [Required]
        public double Tarifa { get; set; }
        [Required]
        public int Ocupantes { get; set; }
        [Required]
        public int MetrosCuadrados { get; set; }
        [Required]
        public string ImagenUrl { get; set; }

        public string Amenidad { get; set; }
    }
}
