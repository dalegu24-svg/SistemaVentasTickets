using System.Data;
using Microsoft.Data.SqlClient;
using SistemaVentas.Common.Models;

namespace SistemaVentas.Data.Repositories;

public class VentaRepository
{
    public int InsertarVenta(Venta venta)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            using (var insertVenta = new SqlCommand(
                @"INSERT INTO dbo.Ventas (ClienteId, UsuarioId, FechaVenta, SubTotal, Igv, Total, Estado, TipoComprobante, NumeroSerie, NumeroDocumento)
                  OUTPUT INSERTED.Id
                  VALUES (@ClienteId, @UsuarioId, @FechaVenta, @SubTotal, @Igv, @Total, @Estado, @TipoComprobante, @NumeroSerie, @NumeroDocumento);",
                connection,
                transaction))
            {
                insertVenta.Parameters.Add("@ClienteId", SqlDbType.Int).Value = venta.ClienteId ?? (object)DBNull.Value;
                insertVenta.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = venta.UsuarioId;
                insertVenta.Parameters.Add("@FechaVenta", SqlDbType.DateTime).Value = venta.FechaVenta;
                insertVenta.Parameters.Add("@SubTotal", SqlDbType.Decimal).Value = venta.SubTotal;
                insertVenta.Parameters.Add("@Igv", SqlDbType.Decimal).Value = venta.Igv;
                insertVenta.Parameters.Add("@Total", SqlDbType.Decimal).Value = venta.Total;
                insertVenta.Parameters.Add("@Estado", SqlDbType.NVarChar, 50).Value = venta.Estado;
                insertVenta.Parameters.Add("@TipoComprobante", SqlDbType.NVarChar, 30).Value = venta.TipoComprobante;
                insertVenta.Parameters.Add("@NumeroSerie", SqlDbType.NVarChar, 50).Value = (object?)venta.NumeroSerie ?? DBNull.Value;
                insertVenta.Parameters.Add("@NumeroDocumento", SqlDbType.NVarChar, 50).Value = (object?)venta.NumeroDocumento ?? DBNull.Value;

                var saleId = Convert.ToInt32(insertVenta.ExecuteScalar());

                foreach (var detalle in venta.Detalles)
                {
                    using var insertDetalle = new SqlCommand(
                        @"INSERT INTO dbo.DetalleVenta (VentaId, ProductoId, Cantidad, PrecioUnitario, Descuento, SubTotal)
                          VALUES (@VentaId, @ProductoId, @Cantidad, @PrecioUnitario, @Descuento, @SubTotal);",
                        connection,
                        transaction);

                    insertDetalle.Parameters.Add("@VentaId", SqlDbType.Int).Value = saleId;
                    insertDetalle.Parameters.Add("@ProductoId", SqlDbType.Int).Value = detalle.ProductoId;
                    insertDetalle.Parameters.Add("@Cantidad", SqlDbType.Int).Value = detalle.Cantidad;
                    insertDetalle.Parameters.Add("@PrecioUnitario", SqlDbType.Decimal).Value = detalle.PrecioUnitario;
                    insertDetalle.Parameters.Add("@Descuento", SqlDbType.Decimal).Value = detalle.Descuento;
                    insertDetalle.Parameters.Add("@SubTotal", SqlDbType.Decimal).Value = detalle.SubTotal;
                    insertDetalle.ExecuteNonQuery();

                    using var updateStock = new SqlCommand(
                        @"UPDATE dbo.Productos
                          SET Stock = Stock - @Cantidad
                          WHERE Id = @ProductoId AND Stock >= @Cantidad;",
                        connection,
                        transaction);

                    updateStock.Parameters.Add("@Cantidad", SqlDbType.Int).Value = detalle.Cantidad;
                    updateStock.Parameters.Add("@ProductoId", SqlDbType.Int).Value = detalle.ProductoId;

                    int rows = updateStock.ExecuteNonQuery();
                    if (rows == 0)
                        throw new InvalidOperationException("No se pudo actualizar el stock del producto.");
                }

                transaction.Commit();
                return saleId;
            }
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
