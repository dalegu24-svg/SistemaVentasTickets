using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using SistemaVentas.Common.Models;

namespace SistemaVentas.Data.Repositories;

public class ReporteRepository
{
    public DataTable ObtenerResumenVentasDiarias(DateTime desde, DateTime hasta)
    {
        var table = new DataTable();
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT CAST(FechaVenta AS date) AS Fecha,
                    COUNT(*) AS TotalVentas,
                    SUM(Total) AS MontoTotal,
                    COUNT(DISTINCT UsuarioId) AS CajerosCon
Ventas
              FROM dbo.Ventas
              WHERE FechaVenta >= @Desde AND FechaVenta < @HastaFin
              GROUP BY CAST(FechaVenta AS date)
              ORDER BY Fecha DESC;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        using var reader = command.ExecuteReader();
        table.Load(reader);
        return table;
    }

    public DataTable ObtenerProductosMasVendidos(DateTime desde, DateTime hasta, int top = 10)
    {
        var table = new DataTable();
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            $@"SELECT TOP {top}
                    p.Codigo,
                    p.Nombre,
                    SUM(dv.Cantidad) AS TotalCantidad,
                    SUM(dv.SubTotal) AS Ingresos,
                    p.PrecioVenta
              FROM dbo.DetalleVentas dv
              INNER JOIN dbo.Productos p ON dv.ProductoId = p.Id
              INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
              WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @HastaFin
              GROUP BY p.Id, p.Codigo, p.Nombre, p.PrecioVenta
              ORDER BY TotalCantidad DESC;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        using var reader = command.ExecuteReader();
        table.Load(reader);
        return table;
    }

    public DataTable ObtenerVentasPorCajero(DateTime desde, DateTime hasta)
    {
        var table = new DataTable();
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT u.NombreCompleto AS Cajero,
                    COUNT(*) AS TotalVentas,
                    SUM(v.Total) AS MontoTotal,
                    AVG(v.Total) AS PromedioVenta,
                    MAX(v.Total) AS VentaMaxima,
                    MIN(v.Total) AS VentaMinima
              FROM dbo.Ventas v
              INNER JOIN dbo.Usuarios u ON v.UsuarioId = u.Id
              WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @HastaFin
              GROUP BY v.UsuarioId, u.NombreCompleto
              ORDER BY MontoTotal DESC;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        using var reader = command.ExecuteReader();
        table.Load(reader);
        return table;
    }

    public DataTable ObtenerVentasPorCategoria(DateTime desde, DateTime hasta)
    {
        var table = new DataTable();
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT p.Categoria,
                    COUNT(DISTINCT v.Id) AS TotalVentas,
                    SUM(dv.Cantidad) AS TotalProductos,
                    SUM(dv.SubTotal) AS Ingresos,
                    CAST(SUM(dv.SubTotal) * 100.0 / (SELECT SUM(SubTotal) FROM dbo.DetalleVentas dv2
                        INNER JOIN dbo.Ventas v2 ON dv2.VentaId = v2.Id
                        WHERE v2.FechaVenta >= @Desde AND v2.FechaVenta < @HastaFin) AS decimal(5,2)) AS Porcentaje
              FROM dbo.DetalleVentas dv
              INNER JOIN dbo.Productos p ON dv.ProductoId = p.Id
              INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
              WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @HastaFin
              GROUP BY p.Categoria
              ORDER BY Ingresos DESC;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        using var reader = command.ExecuteReader();
        table.Load(reader);
        return table;
    }

    public decimal ObtenerIngresosTotales(DateTime desde, DateTime hasta)
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

    public int ObtenerTotalVentas(DateTime desde, DateTime hasta)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT COUNT(*)
              FROM dbo.Ventas
              WHERE FechaVenta >= @Desde AND FechaVenta < @HastaFin;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        var result = command.ExecuteScalar();
        return result == DBNull.Value || result is null ? 0 : Convert.ToInt32(result);
    }

    public int ObtenerTotalProductosVendidos(DateTime desde, DateTime hasta)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT ISNULL(SUM(Cantidad), 0)
              FROM dbo.DetalleVentas dv
              INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
              WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @HastaFin;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        var result = command.ExecuteScalar();
        return result == DBNull.Value || result is null ? 0 : Convert.ToInt32(result);
    }
}
