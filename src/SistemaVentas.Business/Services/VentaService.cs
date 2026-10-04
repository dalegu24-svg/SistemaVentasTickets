using SistemaVentas.Common.Models;

namespace SistemaVentas.Business.Services;

public class ProductoService
{
    private readonly List<Producto> _productos = new()
    {
        new Producto { Id = 1, Codigo = "B001", Nombre = "Agua Mineral 500ml", PrecioVenta = 3.00m, Stock = 40, CategoriaId = 1 },
        new Producto { Id = 2, Codigo = "B002", Nombre = "Gaseosa Cola 600ml", PrecioVenta = 5.00m, Stock = 30, CategoriaId = 1 },
        new Producto { Id = 3, Codigo = "S001", Nombre = "Chips de papa", PrecioVenta = 4.50m, Stock = 25, CategoriaId = 2 },
        new Producto { Id = 4, Codigo = "L001", Nombre = "Detergente 1L", PrecioVenta = 9.00m, Stock = 18, CategoriaId = 3 },
        new Producto { Id = 5, Codigo = "H001", Nombre = "Escoba", PrecioVenta = 12.00m, Stock = 12, CategoriaId = 4 }
    };

    public List<Producto> ObtenerProductos()
    {
        return _productos;
    }

    public Producto? ObtenerPorId(int id)
    {
        return _productos.FirstOrDefault(p => p.Id == id);
    }

    public Producto? ObtenerPorCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return null;

        return _productos.FirstOrDefault(p => p.Codigo.Equals(codigo.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void RegistrarProducto(Producto producto)
    {
        if (producto == null) throw new ArgumentNullException(nameof(producto));
        if (string.IsNullOrWhiteSpace(producto.Nombre)) throw new InvalidOperationException("El nombre del producto es obligatorio.");

        if (_productos.Any(p => p.Codigo == producto.Codigo && p.Id != producto.Id))
            throw new InvalidOperationException("Ya existe un producto con ese código.");

        if (producto.Id == 0)
        {
            producto.Id = _productos.Count + 1;
            _productos.Add(producto);
            return;
        }

        var item = _productos.FirstOrDefault(p => p.Id == producto.Id);
        if (item is null) throw new InvalidOperationException("Producto no encontrado.");

        item.Codigo = producto.Codigo;
        item.Nombre = producto.Nombre;
        item.Descripcion = producto.Descripcion;
        item.PrecioCompra = producto.PrecioCompra;
        item.PrecioVenta = producto.PrecioVenta;
        item.CategoriaId = producto.CategoriaId;
        item.Stock = producto.Stock;
        item.Activo = producto.Activo;
    }
}
