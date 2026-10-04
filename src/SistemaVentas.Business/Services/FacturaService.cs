using SistemaVentas.Common.Models;

namespace SistemaVentas.Business.Services;

public class FacturaService
{
    public Factura GenerarFactura(Venta venta, int ventaId)
    {
        return new Factura
        {
            VentaId = ventaId,
            NumeroFactura = $"F-{venta.NumeroDocumento}",
            FechaEmision = DateTime.Now,
            MontoExonerado = 0,
            MontoGratuito = 0,
            MontoAfecto = venta.SubTotal,
            TotalIgv = venta.Igv,
            TotalVenta = venta.Total
        };
    }

    public Ticket GenerarTicket(Venta venta, int ventaId)
    {
        return new Ticket
        {
            VentaId = ventaId,
            NumeroTicket = $"T-{venta.NumeroDocumento}",
            FechaEmision = DateTime.Now,
            Impreso = false
        };
    }
}
