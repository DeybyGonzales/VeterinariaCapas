using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.DTOs;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Seguridad;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[Authorize]   // por defecto: todo el controlador exige token
[ApiController]
[Route("api/[controller]")]
public class PropietariosController : ControllerBase
{
    private readonly IPropietarioServicio _servicio;

    public PropietariosController(IPropietarioServicio servicio) => _servicio = servicio;

    // GET api/propietarios?buscar=lopez   (filtra por nombres o DNI)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropietarioDto>>> Get(string? buscar) =>
        Ok(await _servicio.ListarAsync(buscar));

    // GET api/propietarios/1
    [HttpGet("{id}")]
    public async Task<ActionResult<PropietarioDto>> Get(int id)
    {
        var propietario = await _servicio.ObtenerPorIdAsync(id);
        if (propietario == null) return NotFound();
        return Ok(propietario);
    }

    // GET api/propietarios/1/mascotas   (ruta anidada)
    [HttpGet("{id}/mascotas")]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> GetMascotas(int id)
    {
        var mascotas = await _servicio.ListarMascotasAsync(id);
        if (mascotas == null) return NotFound();
        return Ok(mascotas);
    }

    // POST api/propietarios
    [Authorize(Roles = Roles.Recepcionista)]
    [HttpPost]
    public async Task<ActionResult<PropietarioDto>> Post(PropietarioCrearDto dto)
    {
        try
        {
            var creado = await _servicio.CrearAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = creado.Id }, creado);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // PUT api/propietarios/1
    [Authorize(Roles = Roles.Recepcionista)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, PropietarioActualizarDto dto)
    {
        try
        {
            if (!await _servicio.ActualizarAsync(id, dto)) return NotFound();
            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // DELETE api/propietarios/1
    [Authorize(Roles = Roles.Recepcionista)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (!await _servicio.EliminarAsync(id)) return NotFound();
            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
}
