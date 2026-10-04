using SistemaVentas.Common.Models;

namespace SistemaVentas.Business.Services;

public class VentaService
{
    private readonly ProductoService _productoService = new();

    public Venta CrearVenta(int usuarioId, int? clienteId, List<DetalleVenta> detalles, decimal igvPorcentaje = 18m)
    {
        if (detalles is null || detalles.Count == 0)
            throw new InvalidOperationException("Debe agregar al menos un producto a la venta.");

        var venta = new Venta
        {
            UsuarioId = usuarioId,
            ClienteId = clienteId,
            Estado = "Pendiente",
            TipoComprobante = "Factura",
            Detalles = detalles
        };

        foreach (var detalle in detalles)
        {
            var producto = _productoService.ObtenerPorId(detalle.ProductoId);
            if (producto is null)
                throw new InvalidOperationException($"El producto con Id {detalle.ProductoId} no existe.");

            if (detalle.Cantidad <= 0)
                throw new InvalidOperationException("La cantidad debe ser mayor que cero.");

            if (detalle.Cantidad > producto.Stock)
                throw new InvalidOperationException($"No hay stock suficiente para {producto.Nombre}.");

            detalle.PrecioUnitario = producto.PrecioVenta;
            detalle.SubTotal = detalle.Cantidad * detalle.PrecioUnitario;
            venta.SubTotal += detalle.SubTotal;
        }

        venta.Igv = Math.Round(venta.SubTotal * (igvPorcentaje / 100m), 2);
        venta.Total = venta.SubTotal + venta.Igv;
        venta.NumeroDocumento = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        return venta;
    }
}
