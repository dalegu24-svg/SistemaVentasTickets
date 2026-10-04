using System.Data;
using Microsoft.Data.SqlClient;
using SistemaVentas.Common.Models;

namespace SistemaVentas.Data.Repositories;

public class TicketRepository
{
    public int InsertarTicket(Ticket ticket)
    {
        if (ticket is null)
            throw new ArgumentNullException(nameof(ticket));

        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"INSERT INTO dbo.Tickets (VentaId, NumeroTicket, FechaEmision, TextoTicket, Impreso)
              OUTPUT INSERTED.Id
              VALUES (@VentaId, @NumeroTicket, @FechaEmision, @TextoTicket, @Impreso);",
            connection);

        command.Parameters.Add("@VentaId", SqlDbType.Int).Value = ticket.VentaId;
        command.Parameters.Add("@NumeroTicket", SqlDbType.NVarChar, 50).Value = ticket.NumeroTicket;
        command.Parameters.Add("@FechaEmision", SqlDbType.DateTime).Value = ticket.FechaEmision;
        command.Parameters.Add("@TextoTicket", SqlDbType.NVarChar).Value = ticket.TextoTicket ?? (object)DBNull.Value;
        command.Parameters.Add("@Impreso", SqlDbType.Bit).Value = ticket.Impreso;

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public Ticket? ObtenerPorNumero(string numeroTicket)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT Id, VentaId, NumeroTicket, FechaEmision, TextoTicket, Impreso, CreatedAt
              FROM dbo.Tickets
              WHERE NumeroTicket = @NumeroTicket;",
            connection);

        command.Parameters.Add("@NumeroTicket", SqlDbType.NVarChar, 50).Value = numeroTicket;
        connection.Open();

        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return new Ticket
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            VentaId = reader.GetInt32(reader.GetOrdinal("VentaId")),
            NumeroTicket = reader.GetString(reader.GetOrdinal("NumeroTicket")),
            FechaEmision = reader.GetDateTime(reader.GetOrdinal("FechaEmision")),
            TextoTicket = reader.IsDBNull(reader.GetOrdinal("TextoTicket")) ? null : reader.GetString(reader.GetOrdinal("TextoTicket")),
            Impreso = reader.GetBoolean(reader.GetOrdinal("Impreso")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }

    public void MarcarComoImpreso(int ticketId)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"UPDATE dbo.Tickets
              SET Impreso = 1
              WHERE Id = @Id;",
            connection);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = ticketId;
        connection.Open();
        command.ExecuteNonQuery();
    }
}
