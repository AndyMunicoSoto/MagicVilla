using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
///esto es un controlador api vacia es lo primero que se crea
namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //ENDPOINTS son metodos de c#
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;   
        }

        //creamos un endpoint un lista de l¿villa

        [HttpGet]//siempre debe tener su verbo cada metodo
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas() 
        {
            _logger.LogInformation("Obtener todas las villas");
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("id",Name = "GetVilla")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public ActionResult<VillaDto> GetVilla(int id) 
        {

            if (id == 0)
            {
                _logger.LogError("Error al traer Villa con Id "+ id);
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villadto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_db.Villas.FirstOrDefault(v=>v.Nombre.ToLower()== villadto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            
            }

            if (villadto == null)
            {
                return BadRequest(villadto);
            }
            if (villadto.Id>0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            //villadto.Id = _db.villaList.OrderByDescending(v=>v.Id).FirstOrDefault().Id+1;
            //VillaStore.villaList.Add(villadto);
            Villa modelo = new Villa
            {
                Id = villadto.Id,
                Nombre = villadto.Nombre,
                Detalles = villadto.Detalles,
                ImagenUrl = villadto.ImagenUrl,
                Ocupantes = villadto.Ocupantes,
                Tarifa = villadto.Tarifa,
                MetrosCuadrados = villadto.MetrosCuadrados,
                Amenidad = villadto.Amenidad

            };
            _db.Villas.Add(modelo);
            _db.SaveChanges();
            return CreatedAtRoute("GetVilla",new { id=villadto.Id},villadto);//Ok(villadto);

        }

        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id) //no necesitaremos al modelos
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(v=> v.Id==id);
            if (villa == null) 
            {
                return NotFound();
            }
            //VillaStore.villaList.Remove(villa);
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();


        }


        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villadto)
        {
            if (villadto == null || id != villadto.Id)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v=> v.Id==id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;
            Villa modelo = new()
            {
                Id = villadto.Id,
                Nombre = villadto.Nombre,
                Detalles = villadto.Detalles,
                ImagenUrl = villadto.ImagenUrl,
                Ocupantes = villadto.Ocupantes,
                Tarifa = villadto.Tarifa,
                MetrosCuadrados = villadto.MetrosCuadrados,
                Amenidad = villadto.Amenidad

            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id ==0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

            VillaDto villadto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalles = villa.Detalles,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };

            if (villa == null) return BadRequest();
            

            

            patchDto.ApplyTo(villadto,ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = new()
            {
                Id = villadto.Id,
                Nombre = villadto.Nombre,
                Detalles = villadto.Detalles,
                ImagenUrl = villadto.ImagenUrl,
                Ocupantes = villadto.Ocupantes,
                Tarifa = villadto.Tarifa,
                MetrosCuadrados = villadto.MetrosCuadrados,
                Amenidad = villadto.Amenidad
            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();
            return NoContent();
        }


    }
}
