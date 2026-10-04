namespace SistemaVentas.Common.Models;

public class Factura : BaseEntity
{
    public int VentaId { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public decimal Igv { get; set; }
    public decimal SubTotal { get; set; }
    public string Estado { get; set; } = "Emitida";
}
