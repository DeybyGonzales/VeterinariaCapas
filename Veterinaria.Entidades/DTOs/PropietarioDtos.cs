using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.DTOs;

// DTO de lectura: lo que la API devuelve de un propietario
public class PropietarioDto
{
    public int Id { get; set; }
    public string Nombres { get; set; } = "";
    public string Dni { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string? Email { get; set; }
    public int CantidadMascotas { get; set; }
}

// DTO de entrada para crear
public class PropietarioCrearDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Los nombres deben tener entre 3 y 100 caracteres.")]
    public string Nombres { get; set; } = "";

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI solo admite números.")]
    public string Dni { get; set; } = "";

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "El teléfono debe tener entre 6 y 15 caracteres.")]
    public string Telefono { get; set; } = "";

    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(100)]
    public string? Email { get; set; }
}

// DTO de entrada para actualizar (el Id viene en la ruta, no en el cuerpo)
public class PropietarioActualizarDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Los nombres deben tener entre 3 y 100 caracteres.")]
    public string Nombres { get; set; } = "";

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI solo admite números.")]
    public string Dni { get; set; } = "";

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "El teléfono debe tener entre 6 y 15 caracteres.")]
    public string Telefono { get; set; } = "";

    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(100)]
    public string? Email { get; set; }
}
