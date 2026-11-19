namespace Veterinaria.Negocio.Seguridad;

// Roles del negocio. Se usan en [Authorize(Roles = ...)] y al registrar usuarios.
public static class Roles
{
    // Dueño de mascota que se registra por su cuenta: solo puede consultar.
    public const string Cliente = "Cliente";

    // Atiende el mostrador: registra propietarios y mascotas.
    public const string Recepcionista = "Recepcionista";

    // Atiende las consultas: actualiza los datos clínicos de las mascotas (peso, edad).
    public const string Veterinario = "Veterinario";

    // Rol que recibe toda cuenta nueva. Recepcionista y Veterinario los asigna la clínica.
    public const string PorDefecto = Cliente;
}
