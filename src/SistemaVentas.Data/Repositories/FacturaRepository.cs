using System.Data;
using Microsoft.Data.SqlClient;
using SistemaVentas.Common.Models;

namespace SistemaVentas.Data.Repositories;

public class FacturaRepository
{
    public int InsertarFactura(Factura factura)
    {
        if (factura is null)
            throw new ArgumentNullException(nameof(factura));

        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"INSERT INTO dbo.Facturas (VentaId, NumeroFactura, FechaEmision, SubTotal, Igv, Total, Estado)
              OUTPUT INSERTED.Id
              VALUES (@VentaId, @NumeroFactura, @FechaEmision, @SubTotal, @Igv, @Total, @Estado);",
            connection);

        command.Parameters.Add("@VentaId", SqlDbType.Int).Value = factura.VentaId;
        command.Parameters.Add("@NumeroFactura", SqlDbType.NVarChar, 50).Value = factura.NumeroFactura;
        command.Parameters.Add("@FechaEmision", SqlDbType.DateTime).Value = factura.FechaEmision;
        command.Parameters.Add("@SubTotal", SqlDbType.Decimal).Value = factura.SubTotal;
        command.Parameters.Add("@Igv", SqlDbType.Decimal).Value = factura.Igv;
        command.Parameters.Add("@Total", SqlDbType.Decimal).Value = factura.Total;
        command.Parameters.Add("@Estado", SqlDbType.NVarChar, 50).Value = factura.Estado;

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public Factura? ObtenerPorNumero(string numeroFactura)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT Id, VentaId, NumeroFactura, FechaEmision, SubTotal, Igv, Total, Estado, CreatedAt
              FROM dbo.Facturas
              WHERE NumeroFactura = @NumeroFactura;",
            connection);

        command.Parameters.Add("@NumeroFactura", SqlDbType.NVarChar, 50).Value = numeroFactura;
        connection.Open();

        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return new Factura
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            VentaId = reader.GetInt32(reader.GetOrdinal("VentaId")),
            NumeroFactura = reader.GetString(reader.GetOrdinal("NumeroFactura")),
            FechaEmision = reader.GetDateTime(reader.GetOrdinal("FechaEmision")),
            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
            Igv = reader.GetDecimal(reader.GetOrdinal("Igv")),
            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
            Estado = reader.GetString(reader.GetOrdinal("Estado")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}
