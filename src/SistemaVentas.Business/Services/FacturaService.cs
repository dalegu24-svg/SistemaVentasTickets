using SistemaVentas.Common.Models;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.Business.Services;

public class FacturaService
{
    private readonly VentaRepository _ventaRepository = new();

    public Factura GenerarFactura(Venta venta, int ventaId)
    {
        if (venta is null)
            throw new ArgumentNullException(nameof(venta));

        if (ventaId <= 0)
            throw new InvalidOperationException("El ID de la venta debe ser válido.");

        var numeroFactura = GenerarNumeroFactura(ventaId);

        var factura = new Factura
        {
            VentaId = ventaId,
            NumeroFactura = numeroFactura,
            FechaEmision = DateTime.Now,
            SubTotal = venta.SubTotal,
            Igv = venta.Igv,
            Total = venta.Total,
            Estado = "Emitida"
        };

        return factura;
    }

    public Ticket GenerarTicket(Venta venta, int ventaId, string nombreEmpresa = "Mi Punto de Venta", string ruc = "20123456789")
    {
        if (venta is null)
            throw new ArgumentNullException(nameof(venta));

        if (ventaId <= 0)
            throw new InvalidOperationException("El ID de la venta debe ser válido.");

        var numeroTicket = GenerarNumeroTicket(ventaId);
        var textoTicket = GenerarTextoTicket(venta, numeroTicket, nombreEmpresa, ruc);

        var ticket = new Ticket
        {
            VentaId = ventaId,
            NumeroTicket = numeroTicket,
            FechaEmision = DateTime.Now,
            TextoTicket = textoTicket,
            Impreso = false
        };

        return ticket;
    }

    private static string GenerarNumeroFactura(int ventaId)
    {
        return $"FAC-{DateTime.Now.Year}{DateTime.Now.Month:D2}{ventaId:D6}";
    }

    private static string GenerarNumeroTicket(int ventaId)
    {
        return $"TKT-{DateTime.Now.Year}{DateTime.Now.Month:D2}{ventaId:D6}";
    }

    private static string GenerarTextoTicket(Venta venta, string numeroTicket, string nombreEmpresa, string ruc)
    {
        var ticket = new System.Text.StringBuilder();
        ticket.AppendLine("=" * 40);
        ticket.AppendLine(nombreEmpresa.PadCenter(40));
        ticket.AppendLine("=" * 40);
        ticket.AppendLine();
        ticket.AppendLine($"RUC: {ruc}");
        ticket.AppendLine($"Ticket: {numeroTicket}");
        ticket.AppendLine($"Fecha: {venta.FechaVenta:dd/MM/yyyy HH:mm:ss}");
        ticket.AppendLine();
        ticket.AppendLine("-" * 40);
        ticket.AppendLine("DETALLE DE VENTA");
        ticket.AppendLine("-" * 40);

        if (venta.Detalles?.Count > 0)
        {
            foreach (var detalle in venta.Detalles)
            {
                ticket.AppendLine($"Producto: {detalle.ProductoId}");
                ticket.AppendLine($"Cantidad: {detalle.Cantidad} x S/ {detalle.PrecioUnitario:F2}");
                ticket.AppendLine($"Subtotal: S/ {detalle.SubTotal:F2}");
                ticket.AppendLine();
            }
        }

        ticket.AppendLine("=" * 40);
        ticket.AppendLine($"Subtotal: S/ {venta.SubTotal:F2}");
        ticket.AppendLine($"IGV (18%): S/ {venta.Igv:F2}");
        ticket.AppendLine("=" * 40);
        ticket.AppendLine($"TOTAL: S/ {venta.Total:F2}");
        ticket.AppendLine("=" * 40);
        ticket.AppendLine();
        ticket.AppendLine("Gracias por su compra!");
        ticket.AppendLine("Vuelva pronto!");
        ticket.AppendLine();
        ticket.AppendLine($"Emitido: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

        return ticket.ToString();
    }
}

public static class StringExtensions
{
    public static string PadCenter(this string str, int width)
    {
        if (string.IsNullOrEmpty(str)) return str;
        int padding = (width - str.Length) / 2;
        return str.PadLeft(str.Length + padding).PadRight(width);
    }
}
