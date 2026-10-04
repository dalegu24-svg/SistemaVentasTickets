namespace SistemaVentas.Common.Models;

public class Venta : BaseEntity
{
    public int UsuarioId { get; set; }
    public int? ClienteId { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public DateTime FechaVenta { get; set; } = DateTime.Now;
    public decimal SubTotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string TipoComprobante { get; set; } = "Factura";
    public List<DetalleVenta>? Detalles { get; set; }
}
