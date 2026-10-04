namespace SistemaVentas.Common.Models;

public class Venta : BaseEntity
{
    public int? ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaVenta { get; set; } = DateTime.Now;
    public decimal SubTotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string TipoComprobante { get; set; } = "Factura";
    public string? NumeroSerie { get; set; }
    public string? NumeroDocumento { get; set; }
    public List<DetalleVenta> Detalles { get; set; } = new();
}
