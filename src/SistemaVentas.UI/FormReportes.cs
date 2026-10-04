using System.Data;
using Microsoft.Data.SqlClient;

namespace SistemaVentas.Data.Repositories;

public class ReporteRepository
{
    public DataTable ObtenerVentasPorFecha(DateTime desde, DateTime hasta)
    {
        var table = new DataTable();
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT v.Id AS VentaId,
                    CONVERT(date, v.FechaVenta) AS Fecha,
                    v.NumeroDocumento AS Documento,
                    ISNULL(c.Nombre + ' ' + c.Apellido, 'Cliente general') AS Cliente,
                    u.NombreCompleto AS Usuario,
                    v.SubTotal,
                    v.Igv,
                    v.Total
              FROM dbo.Ventas v
              LEFT JOIN dbo.Clientes c ON c.Id = v.ClienteId
              LEFT JOIN dbo.Usuarios u ON u.Id = v.UsuarioId
              WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @HastaFin
              ORDER BY v.FechaVenta DESC;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        using var reader = command.ExecuteReader();
        table.Load(reader);
        return table;
    }

    public DataTable ObtenerTopProductos(DateTime desde, DateTime hasta)
    {
        var table = new DataTable();
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT p.Nombre,
                    SUM(d.Cantidad) AS CantidadVendida,
                    SUM(d.SubTotal) AS TotalVendido
              FROM dbo.DetalleVenta d
              INNER JOIN dbo.Productos p ON p.Id = d.ProductoId
              INNER JOIN dbo.Ventas v ON v.Id = d.VentaId
              WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @HastaFin
              GROUP BY p.Nombre
              ORDER BY CantidadVendida DESC, TotalVendido DESC;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        using var reader = command.ExecuteReader();
        table.Load(reader);
        return table;
    }

    public decimal ObtenerTotalVentasPeriodo(DateTime desde, DateTime hasta)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT ISNULL(SUM(Total), 0)
              FROM dbo.Ventas
              WHERE FechaVenta >= @Desde AND FechaVenta < @HastaFin;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        var result = command.ExecuteScalar();
        return result == DBNull.Value || result is null ? 0m : Convert.ToDecimal(result);
    }
}

