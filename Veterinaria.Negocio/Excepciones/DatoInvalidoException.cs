namespace Veterinaria.Negocio.Excepciones;

// Se lanza cuando un dato de entrada apunta a algo que no existe (por ejemplo, un PropietarioId inexistente).
// El controlador la convierte en 400 Bad Request (ValidationProblem) indicando el campo.
public class DatoInvalidoException : Exception
{
    public string Campo { get; }

    public DatoInvalidoException(string campo, string mensaje) : base(mensaje) => Campo = campo;
}
