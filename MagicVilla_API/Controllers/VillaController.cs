using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db; 
            _mapper = mapper;
        }

        //creamos un endpoint un lista de l¿villa

        [HttpGet]//siempre debe tener su verbo cada metodo
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas() 
        {
            _logger.LogInformation("Obtener todas las villas");
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
        }


        [HttpGet("id",Name = "GetVilla")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id) 
        {

            if (id == 0)
            {
                _logger.LogError("Error al traer Villa con Id "+ id);
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //siemrpe el retorno async
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDto>(villa));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _db.Villas.FirstOrDefaultAsync(v=>v.Nombre.ToLower()== createdto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            
            }

            if (createdto == null)
            {
                return BadRequest(createdto);
            }
            //if (villadto.Id>0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            //villadto.Id = _db.villaList.OrderByDescending(v=>v.Id).FirstOrDefault().Id+1;
            //VillaStore.villaList.Add(villadto);

            Villa modelo = _mapper.Map<Villa>(createdto);

            //Villa modelo = new Villa
            //{
            //    //Id = villadto.Id,
            //    Nombre = villadto.Nombre,
            //    Detalles = villadto.Detalles,
            //    ImagenUrl = villadto.ImagenUrl,
            //    Ocupantes = villadto.Ocupantes,
            //    Tarifa = villadto.Tarifa,
            //    MetrosCuadrados = villadto.MetrosCuadrados,
            //    Amenidad = villadto.Amenidad

            //};
            await _db.Villas.AddAsync(modelo);
            await _db.SaveChangesAsync();
            return CreatedAtRoute("GetVilla",new { id=modelo.Id},modelo);//Ok(villadto);

        }

        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id) //no necesitaremos al modelos
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(v=> v.Id==id);
            if (villa == null) 
            {
                return NotFound();
            }
            //VillaStore.villaList.Remove(villa);
            _db.Villas.Remove(villa);//no es un metodo asynx
            await _db.SaveChangesAsync();
            return NoContent();


        }


        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v=> v.Id==id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            Villa modelo = _mapper.Map<Villa>(updateDto);

            //Villa modelo = new()
            //{
            //    Id = villadto.Id,
            //    Nombre = villadto.Nombre,
            //    Detalles = villadto.Detalles,
            //    ImagenUrl = villadto.ImagenUrl,
            //    Ocupantes = villadto.Ocupantes,
            //    Tarifa = villadto.Tarifa,
            //    MetrosCuadrados = villadto.MetrosCuadrados,
            //    Amenidad = villadto.Amenidad

            //};
            _db.Villas.Update(modelo);//no es un metodo async
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id ==0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa =await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            VillaUpdateDto villadto = _mapper.Map<VillaUpdateDto>(villa);

            //VillaUpdateDto villadto = new()
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Detalles = villa.Detalles,
            //    ImagenUrl = villa.ImagenUrl,
            //    Ocupantes = villa.Ocupantes,
            //    Tarifa = villa.Tarifa,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    Amenidad = villa.Amenidad
            //};

            if (villa == null) return BadRequest();
            

            

            patchDto.ApplyTo(villadto,ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villadto);

            //Villa modelo = new()
            //{
            //    Id = villadto.Id,
            //    Nombre = villadto.Nombre,
            //    Detalles = villadto.Detalles,
            //    ImagenUrl = villadto.ImagenUrl,
            //    Ocupantes = villadto.Ocupantes,
            //    Tarifa = villadto.Tarifa,
            //    MetrosCuadrados = villadto.MetrosCuadrados,
            //    Amenidad = villadto.Amenidad
            //};
            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();
            return NoContent();
        }


    }
}
