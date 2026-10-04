using System.Data;
using Microsoft.Data.SqlClient;
using SistemaVentas.Common.Models;

namespace SistemaVentas.Data.Repositories;

public class CierreRepository
{
    public int InsertarCierre(Cierre cierre)
    {
        if (cierre is null)
            throw new ArgumentNullException(nameof(cierre));

        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"INSERT INTO dbo.Cierres (UsuarioId, FechaApertura, FechaCierre, MontoInicial, MontoFinal, TotalVentas, Diferencia, Estado, Observaciones)
              OUTPUT INSERTED.Id
              VALUES (@UsuarioId, @FechaApertura, @FechaCierre, @MontoInicial, @MontoFinal, @TotalVentas, @Diferencia, @Estado, @Observaciones);",
            connection);

        command.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = cierre.UsuarioId;
        command.Parameters.Add("@FechaApertura", SqlDbType.DateTime).Value = cierre.FechaApertura;
        command.Parameters.Add("@FechaCierre", SqlDbType.DateTime).Value = cierre.FechaCierre;
        command.Parameters.Add("@MontoInicial", SqlDbType.Decimal).Value = cierre.MontoInicial;
        command.Parameters.Add("@MontoFinal", SqlDbType.Decimal).Value = cierre.MontoFinal;
        command.Parameters.Add("@TotalVentas", SqlDbType.Decimal).Value = cierre.TotalVentas;
        command.Parameters.Add("@Diferencia", SqlDbType.Decimal).Value = cierre.Diferencia;
        command.Parameters.Add("@Estado", SqlDbType.NVarChar, 50).Value = cierre.Estado;
        command.Parameters.Add("@Observaciones", SqlDbType.NVarChar).Value = cierre.Observaciones ?? (object)DBNull.Value;

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public Cierre? ObtenerCierreActivo(int usuarioId)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT TOP 1 Id, UsuarioId, FechaApertura, FechaCierre, MontoInicial, MontoFinal, TotalVentas, Diferencia, Estado, Observaciones, CreatedAt
              FROM dbo.Cierres
              WHERE UsuarioId = @UsuarioId AND Estado = 'Abierto'
              ORDER BY FechaApertura DESC;",
            connection);

        command.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
        connection.Open();

        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return new Cierre
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
            FechaApertura = reader.GetDateTime(reader.GetOrdinal("FechaApertura")),
            FechaCierre = reader.IsDBNull(reader.GetOrdinal("FechaCierre")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaCierre")),
            MontoInicial = reader.GetDecimal(reader.GetOrdinal("MontoInicial")),
            MontoFinal = reader.GetDecimal(reader.GetOrdinal("MontoFinal")),
            TotalVentas = reader.GetDecimal(reader.GetOrdinal("TotalVentas")),
            Diferencia = reader.GetDecimal(reader.GetOrdinal("Diferencia")),
            Estado = reader.GetString(reader.GetOrdinal("Estado")),
            Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones"))
        };
    }

    public DataTable ObtenerHistorialCierres(DateTime desde, DateTime hasta)
    {
        var table = new DataTable();
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT c.Id,
                    u.NombreCompleto AS Usuario,
                    c.FechaApertura,
                    c.FechaCierre,
                    c.MontoInicial,
                    c.MontoFinal,
                    c.TotalVentas,
                    c.Diferencia,
                    c.Estado
              FROM dbo.Cierres c
              INNER JOIN dbo.Usuarios u ON u.Id = c.UsuarioId
              WHERE c.FechaApertura >= @Desde AND c.FechaApertura < @HastaFin
              ORDER BY c.FechaApertura DESC;",
            connection);

        command.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
        command.Parameters.Add("@HastaFin", SqlDbType.DateTime).Value = hasta.AddDays(1);

        connection.Open();
        using var reader = command.ExecuteReader();
        table.Load(reader);
        return table;
    }

    public decimal ObtenerTotalVentasDelDia(int usuarioId, DateTime fecha)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT ISNULL(SUM(Total), 0)
              FROM dbo.Ventas
              WHERE UsuarioId = @UsuarioId AND CAST(FechaVenta AS date) = @Fecha;",
            connection);

        command.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
        command.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;

        connection.Open();
        var result = command.ExecuteScalar();
        return result == DBNull.Value || result is null ? 0m : Convert.ToDecimal(result);
    }
}
