using SistemaVentas.Common.Models;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.Business.Services;

public class ProductoService
{
    private readonly ProductoRepository _repository = new();

    public List<Producto> ObtenerProductos()
    {
        return _repository.ObtenerProductos();
    }

    public Producto? ObtenerPorId(int id)
    {
        return _repository.ObtenerPorId(id);
    }

    public Producto? ObtenerPorCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return null;

        return _repository.ObtenerPorCodigo(codigo.Trim());
    }

    public void RegistrarProducto(Producto producto)
    {
        if (producto == null)
            throw new ArgumentNullException(nameof(producto));

        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new InvalidOperationException("El nombre del producto es obligatorio.");

        if (string.IsNullOrWhiteSpace(producto.Codigo))
            throw new InvalidOperationException("El código del producto es obligatorio.");
    }
}
