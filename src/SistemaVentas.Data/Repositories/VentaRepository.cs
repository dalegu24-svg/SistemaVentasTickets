using System.Data;
using Microsoft.Data.SqlClient;
using SistemaVentas.Common.Models;

namespace SistemaVentas.Data.Repositories;

public class ProductoRepository
{
    public List<Producto> ObtenerProductos()
    {
        var productos = new List<Producto>();

        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT Id, Codigo, Nombre, Descripcion, CategoriaId, PrecioCompra, PrecioVenta, Stock, StockMinimo, Activo, CreatedAt
              FROM dbo.Productos
              WHERE Activo = 1
              ORDER BY Nombre;",
            connection);

        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            productos.Add(new Producto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                CategoriaId = reader.GetInt32(reader.GetOrdinal("CategoriaId")),
                PrecioCompra = reader.GetDecimal(reader.GetOrdinal("PrecioCompra")),
                PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                StockMinimo = reader.GetInt32(reader.GetOrdinal("StockMinimo")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        return productos;
    }

    public Producto? ObtenerPorCodigo(string codigo)
    {
        using var connection = new SqlConnection(SqlServerConnection.ConnectionString);
        using var command = new SqlCommand(
            @"SELECT Id, Codigo, Nombre, Descripcion, CategoriaId, PrecioCompra, PrecioVenta, Stock, StockMinimo, Activo, CreatedAt
              FROM dbo.Productos
              WHERE Codigo = @Codigo AND Activo = 1;",
            connection);

        command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 50).Value = codigo;
        connection.Open();

        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return new Producto
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
            CategoriaId = reader.GetInt32(reader.GetOrdinal("CategoriaId")),
            PrecioCompra = reader.GetDecimal(reader.GetOrdinal("PrecioCompra")),
            PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
            Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
            StockMinimo = reader.GetInt32(reader.GetOrdinal("StockMinimo")),
            Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}
