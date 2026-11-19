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
public class MascotasController : ControllerBase
{
    private readonly IMascotaServicio _servicio;

    public MascotasController(IMascotaServicio servicio) => _servicio = servicio;

    // GET api/mascotas?especie=Perro&propietarioId=1&buscar=fir&edadMax=5
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> Get(
        string? especie, int? propietarioId, string? buscar, int? edadMax) =>
        Ok(await _servicio.ListarAsync(especie, propietarioId, buscar, edadMax));

    // GET api/mascotas/1
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<MascotaDto>> Get(int id)
    {
        var mascota = await _servicio.ObtenerPorIdAsync(id);
        if (mascota == null) return NotFound();
        return Ok(mascota);
    }

    // POST api/mascotas
    [Authorize(Roles = Roles.Recepcionista)]
    [HttpPost]
    public async Task<ActionResult<MascotaDto>> Post(MascotaCrearDto dto)
    {
        try
        {
            var creada = await _servicio.CrearAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = creada.Id }, creada);
        }
        catch (DatoInvalidoException ex)
        {
            ModelState.AddModelError(ex.Campo, ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // PUT api/mascotas/1
    [Authorize(Roles = Roles.Veterinario)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, MascotaActualizarDto dto)
    {
        try
        {
            if (!await _servicio.ActualizarAsync(id, dto)) return NotFound();
            return NoContent();
        }
        catch (DatoInvalidoException ex)
        {
            ModelState.AddModelError(ex.Campo, ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // DELETE api/mascotas/1
    [Authorize(Roles = Roles.Recepcionista)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _servicio.EliminarAsync(id)) return NotFound();
        return NoContent();
    }
}
