using SistemaVentas.Business.Services;
using SistemaVentas.Common.Models;

namespace SistemaVentas.UI;

public partial class Form1 : Form
{
    private readonly ProductoService _productoService = new();
    private readonly VentaService _ventaService = new();
    private readonly List<DetalleVenta> _carrito = new();

    public Form1()
    {
        InitializeComponent();
        CargarProductos();
    }

    private void InitializeComponent()
    {
        this.Text = "Sistema de Ventas";
        this.Size = new Size(1100, 700);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblTitulo = new Label
        {
            Text = "Sistema de Ventas - Punto de Venta",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(30, 20)
        };

        var dgvProductos = new DataGridView
        {
            Name = "dgvProductos",
            Location = new Point(30, 70),
            Size = new Size(520, 260),
            ReadOnly = true,
            AutoGenerateColumns = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        };

        dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = "Codigo" });
        dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "Nombre" });
        dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio", DataPropertyName = "PrecioVenta" });
        dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = "Stock" });

        var btnAgregar = new Button
        {
            Text = "Agregar al carrito",
            Location = new Point(560, 120),
            Size = new Size(150, 35)
        };

        btnAgregar.Click += (sender, e) =>
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            var producto = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;
            var detalle = new DetalleVenta
            {
                ProductoId = producto.Id,
                Cantidad = 1,
                PrecioUnitario = producto.PrecioVenta,
                SubTotal = producto.PrecioVenta
            };

            _carrito.Add(detalle);
            MessageBox.Show($"Se agregó {producto.Nombre} al carrito.");
        };

        var btnGenerarVenta = new Button
        {
            Text = "Generar venta",
            Location = new Point(560, 180),
            Size = new Size(150, 35)
        };

        btnGenerarVenta.Click += (sender, e) =>
        {
            try
            {
                var venta = _ventaService.CrearVenta(1, null, _carrito);
                MessageBox.Show($"Venta creada correctamente. Total: {venta.Total:C}");
                _carrito.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        };

        Controls.Add(lblTitulo);
        Controls.Add(dgvProductos);
        Controls.Add(btnAgregar);
        Controls.Add(btnGenerarVenta);

        var bindingSource = new BindingSource();
        bindingSource.DataSource = _productoService.ObtenerProductos();
        dgvProductos.DataSource = bindingSource;
        dgvProductos.MultiSelect = false;
        dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    private void CargarProductos()
    {
    }
}
