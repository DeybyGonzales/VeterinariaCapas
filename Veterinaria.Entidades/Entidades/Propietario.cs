using System;
using System.Collections.Generic;

namespace Veterinaria.Entidades.Entidades;

public partial class Propietario
{
    public int Id { get; set; }

    public string Nombres { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? Email { get; set; }

    public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
}
