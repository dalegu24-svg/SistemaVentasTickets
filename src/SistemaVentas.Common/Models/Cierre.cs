namespace SistemaVentas.Common.Models;

public class Cierre : BaseEntity
{
    public int UsuarioId { get; set; }
    public DateTime FechaApertura { get; set; }
    public DateTime? FechaCierre { get; set; }
    public decimal MontoInicial { get; set; }
    public decimal MontoFinal { get; set; }
    public decimal TotalVentas { get; set; }
    public decimal Diferencia { get; set; }
    public string Estado { get; set; } = "Abierto";
    public string? Observaciones { get; set; }
}
