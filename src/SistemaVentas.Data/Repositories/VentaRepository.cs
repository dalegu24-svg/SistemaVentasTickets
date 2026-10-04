using System.Data;
using Microsoft.Data.SqlClient;
using SistemaVentas.Common.Models;

namespace SistemaVentas.Data.Repositories;

public class VentaRepository
{
    public int InsertarVenta(Venta venta)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"INSERT INTO dbo.Ventas (UsuarioId, ClienteId, NumeroDocumento, FechaVenta, SubTotal, Igv, Total, Estado, TipoComprobante, CreatedAt)
              VALUES (@UsuarioId, @ClienteId, @NumeroDocumento, @FechaVenta, @SubTotal, @Igv, @Total, @Estado, @TipoComprobante, @CreatedAt);
              SELECT SCOPE_IDENTITY();",
            connection);

        command.Parameters.AddWithValue("@UsuarioId", venta.UsuarioId);
        command.Parameters.AddWithValue("@ClienteId", venta.ClienteId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@NumeroDocumento", venta.NumeroDocumento ?? string.Empty);
        command.Parameters.AddWithValue("@FechaVenta", venta.FechaVenta);
        command.Parameters.AddWithValue("@SubTotal", venta.SubTotal);
        command.Parameters.AddWithValue("@Igv", venta.Igv);
        command.Parameters.AddWithValue("@Total", venta.Total);
        command.Parameters.AddWithValue("@Estado", venta.Estado ?? "Pendiente");
        command.Parameters.AddWithValue("@TipoComprobante", venta.TipoComprobante ?? "Factura");
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        connection.Open();
        var result = command.ExecuteScalar();

        return Convert.ToInt32(result);
    }
}
