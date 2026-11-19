using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.DTOs;

// DTO de lectura: devuelve el nombre del propietario en lugar del objeto Propietario
public class MascotaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Especie { get; set; } = "";
    public string? Raza { get; set; }
    public int Edad { get; set; }
    public decimal Peso { get; set; }
    public int PropietarioId { get; set; }
    public string Propietario { get; set; } = "";
}

// DTO de entrada para crear
public class MascotaCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "La especie debe tener entre 3 y 30 caracteres.")]
    public string Especie { get; set; } = "";

    [StringLength(50)]
    public string? Raza { get; set; }

    [Range(0, 40, ErrorMessage = "La edad debe estar entre 0 y 40 años.")]
    public int Edad { get; set; }

    [Range(0.1, 200, ErrorMessage = "El peso debe estar entre 0.1 y 200 kg.")]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El propietario es obligatorio.")]
    public int PropietarioId { get; set; }
}

// DTO de entrada para actualizar (el Id viene en la ruta)
public class MascotaActualizarDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "La especie debe tener entre 3 y 30 caracteres.")]
    public string Especie { get; set; } = "";

    [StringLength(50)]
    public string? Raza { get; set; }

    [Range(0, 40, ErrorMessage = "La edad debe estar entre 0 y 40 años.")]
    public int Edad { get; set; }

    [Range(0.1, 200, ErrorMessage = "El peso debe estar entre 0.1 y 200 kg.")]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El propietario es obligatorio.")]
    public int PropietarioId { get; set; }
}
