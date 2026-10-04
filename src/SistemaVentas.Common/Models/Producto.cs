namespace SistemaVentas.Common.Models;

public class Cliente : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Apellido { get; set; }
    public string? Documento { get; set; }
    public string? TipoDocumento { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
}
