using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Modelos.Dto
{
    //esto es para no trbajar directamente con el modelo
    public class VillaCreateDto
    {
        //public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; }
        public string Detalles { get; set; }

        [Required]
        public double Tarifa { get; set; }
        public int Ocupantes { get; set; }
        public int MetrosCuadrados { get; set; }
        public string ImagenUrl { get; set; }

        public string Amenidad { get; set; }
    }
}
