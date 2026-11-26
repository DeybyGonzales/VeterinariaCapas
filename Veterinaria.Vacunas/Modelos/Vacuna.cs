namespace Veterinaria.Vacunas.Modelos;

// Modelo propio del microservicio: NO comparte clases con Veterinaria.Api
public record Vacuna(
    int Id,
    string Nombre,
    string Especie,          // Perro, Gato o Ambos
    string Previene,
    int EdadMinimaSemanas,
    int RefuerzoMeses,       // cada cuántos meses se repite
    decimal Precio);
