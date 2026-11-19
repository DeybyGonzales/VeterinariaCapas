namespace Veterinaria.Entidades.Entidades;

// Usuario del sistema. La contraseña nunca se guarda en texto: solo su hash BCrypt.
public partial class Usuario
{
    public int Id { get; set; }

    public string Correo { get; set; } = null!;

    public string ClaveHash { get; set; } = null!;

    // Cliente, Recepcionista o Veterinario. Lo decide el servidor, nunca el cliente.
    public string Rol { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }
}
