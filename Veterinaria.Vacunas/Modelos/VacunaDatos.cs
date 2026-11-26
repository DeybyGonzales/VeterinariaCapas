namespace Veterinaria.Vacunas.Modelos;

// Datos en memoria (sin base de datos): catálogo de vacunas de la veterinaria
public static class VacunaDatos
{
    public static readonly IReadOnlyList<Vacuna> Lista =
    [
        new(1, "Quíntuple canina (DHPPi+L)", "Perro", "Moquillo, hepatitis, parvovirus, parainfluenza y leptospirosis", 6, 12, 85.00m),
        new(2, "Antirrábica", "Ambos", "Rabia", 12, 12, 40.00m),
        new(3, "Tos de las perreras (Bordetella)", "Perro", "Traqueobronquitis infecciosa canina", 8, 12, 70.00m),
        new(4, "Triple felina (FVRCP)", "Gato", "Rinotraqueítis, calicivirus y panleucopenia felina", 8, 12, 80.00m),
        new(5, "Leucemia felina (FeLV)", "Gato", "Virus de la leucemia felina", 8, 12, 95.00m),
        new(6, "Parvovirus canino (refuerzo cachorro)", "Perro", "Parvovirus canino", 6, 0, 55.00m)
    ];
}
