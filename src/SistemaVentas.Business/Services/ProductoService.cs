using System.Globalization;
using System.Windows.Forms;
using SistemaVentas.Business.Services;
using SistemaVentas.Common.Models;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.UI;

public partial class Form1 : Form
{
    private readonly ProductoService _productoService = new();
    private readonly VentaService _ventaService = new();
    private readonly List<DetalleVenta> _carrito = new();
    private readonly BindingSource _productoBinding = new();
    private readonly BindingSource _carritoBinding = new();

    private DataGridView dgvProductos;
    private DataGridView dgvCarrito;
    private TextBox txtBuscarProducto;
    private NumericUpDown nudCantidad;
    private TextBox txtCliente;
    private Label lblSubtotal;
    private Label lblIgv;
    private Label lblTotal;
    private Button btnBuscar;
    private Button btnAgregar;
    private Button btnQuitar;
    private Button btnGenerarVenta;
    private Button btnCancelar;

    public Form1()
    {
        InitializeComponent();
        CargarProductos();
        ActualizarCarrito();
    }

    private void InitializeComponent()
    {
        this.Text = "Sistema de Ventas";
        this.Size = new Size(1100, 720);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(980, 620);
        this.BackColor = Color.FromArgb(245, 247, 250);

        var panelTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 110,
            BackColor = Color.FromArgb(29, 78, 216)
        };

        var lblTitulo = new Label
        {
            Text = "PUNTO DE VENTA",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(30, 30)
        };

        var lblSubtitulo = new Label
        {
            Text = "Sistema de ventas con facturación y tickets",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(220, 230, 255),
            AutoSize = true,
            Location = new Point(30, 68)
        };

        panelTop.Controls.Add(lblTitulo);
        panelTop.Controls.Add(lblSubtitulo);
        this.Controls.Add(panelTop);

        var panelBusqueda = new Panel
        {
            Dock = DockStyle.Top,
            Height = 95,
            Padding = new Padding(20, 12, 20, 10),
            BackColor = Color.FromArgb(245, 247, 250)
        };

        var lblBuscar = new Label
        {
            Text = "Buscar producto:",
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            Location = new Point(20, 18)
        };

        txtBuscarProducto = new TextBox
        {
            Width = 260,
            Height = 30,
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            Location = new Point(140, 12)
        };

        btnBuscar = new Button
        {
            Text = "Buscar",
            Width = 100,
            Height = 32,
            Location = new Point(420, 12),
            BackColor = Color.FromArgb(59, 130, 246),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnBuscar.Click += btnBuscar_Click;

        var lblCliente = new Label
        {
            Text = "Cliente:",
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            Location = new Point(560, 18)
        };

        txtCliente = new TextBox
        {
            Width = 220,
            Height = 30,
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            Text = "Cliente general",
            Location = new Point(640, 12)
        };

        var lblCantidad = new Label
        {
            Text = "Cantidad:",
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            Location = new Point(900, 18)
        };

        nudCantidad = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 100,
            Value = 1,
            Width = 70,
            Height = 30,
            Location = new Point(980, 12),
            Font = new Font("Segoe UI", 11, FontStyle.Regular)
        };

        panelBusqueda.Controls.Add(lblBuscar);
        panelBusqueda.Controls.Add(txtBuscarProducto);
        panelBusqueda.Controls.Add(btnBuscar);
        panelBusqueda.Controls.Add(lblCliente);
        panelBusqueda.Controls.Add(txtCliente);
        panelBusqueda.Controls.Add(lblCantidad);
        panelBusqueda.Controls.Add(nudCantidad);
        this.Controls.Add(panelBusqueda);

        dgvProductos = new DataGridView
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Location = new Point(20, 220),
            Size = new Size(560, 390),
            AutoGenerateColumns = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            BorderStyle = BorderStyle.Fixed3D,
            BackgroundColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false
        };

        dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = "Codigo", Width = 90 });
        dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "Nombre", Width = 190 });
        dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = "Stock", Width = 70 });
        dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio", DataPropertyName = "PrecioVenta", Width = 95 });
        dgvProductos.DataSource = _productoBinding;
        this.Controls.Add(dgvProductos);

        var panelCarrito = new Panel
        {
            Location = new Point(610, 220),
            Size = new Size(450, 390),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblCarrito = new Label
        {
            Text = "Carrito de compras",
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(15, 12)
        };

        panelCarrito.Controls.Add(lblCarrito);

        dgvCarrito = new DataGridView
        {
            Location = new Point(15, 48),
            Size = new Size(420, 210),
            AutoGenerateColumns = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            BorderStyle = BorderStyle.None,
            BackgroundColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false
        };

        dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = "Codigo", Width = 70 });
        dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Producto", DataPropertyName = "Nombre", Width = 165 });
        dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cant.", DataPropertyName = "Cantidad", Width = 55 });
        dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Subtotal", DataPropertyName = "SubTotal", Width = 90 });
        dgvCarrito.DataSource = _carritoBinding;
        panelCarrito.Controls.Add(dgvCarrito);

        var panelTotales = new Panel
        {
            Location = new Point(15, 275),
            Size = new Size(420, 95),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        var lblSubtotalTexto = new Label
        {
            Text = "Subtotal:",
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            Location = new Point(15, 14)
        };

        lblSubtotal = new Label
        {
            Text = "S/ 0.00",
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(200, 14)
        };

        var lblIgvTexto = new Label
        {
            Text = "IGV (18%):",
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            Location = new Point(15, 42)
        };

        lblIgv = new Label
        {
            Text = "S/ 0.00",
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(200, 42)
        };

        var lblTotalTexto = new Label
        {
            Text = "Total:",
            AutoSize = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(15, 70)
        };

        lblTotal = new Label
        {
            Text = "S/ 0.00",
            AutoSize = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(34, 197, 94),
            Location = new Point(200, 70)
        };

        panelTotales.Controls.Add(lblSubtotalTexto);
        panelTotales.Controls.Add(lblSubtotal);
        panelTotales.Controls.Add(lblIgvTexto);
        panelTotales.Controls.Add(lblIgv);
        panelTotales.Controls.Add(lblTotalTexto);
        panelTotales.Controls.Add(lblTotal);
        panelCarrito.Controls.Add(panelTotales);
        this.Controls.Add(panelCarrito);

        btnAgregar = new Button
        {
            Text = "Agregar",
            Size = new Size(130, 40),
            Location = new Point(20, 150),
            BackColor = Color.FromArgb(34, 197, 94),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnAgregar.Click += btnAgregar_Click;

        btnQuitar = new Button
        {
            Text = "Quitar",
            Size = new Size(130, 40),
            Location = new Point(170, 150),
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnQuitar.Click += btnQuitar_Click;

        btnGenerarVenta = new Button
        {
            Text = "Generar venta",
            Size = new Size(160, 40),
            Location = new Point(620, 620),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnGenerarVenta.Click += btnGenerarVenta_Click;

        btnCancelar = new Button
        {
            Text = "Cancelar",
            Size = new Size(120, 40),
            Location = new Point(800, 620),
            BackColor = Color.FromArgb(107, 114, 128),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnCancelar.Click += btnCancelar_Click;

        this.Controls.Add(btnAgregar);
        this.Controls.Add(btnQuitar);
        this.Controls.Add(btnGenerarVenta);
        this.Controls.Add(btnCancelar);
    }

    private void btnBuscar_Click(object sender, EventArgs e)
    {
        var texto = txtBuscarProducto.Text.Trim();

        if (string.IsNullOrWhiteSpace(texto))
        {
            CargarProductos();
            return;
        }

        var producto = _productoService.ObtenerPorCodigo(texto);
        if (producto is null)
        {
            MessageBox.Show("No se encontró ningún producto con ese código.", "Producto no encontrado");
            return;
        }

        _productoBinding.DataSource = new List<Producto> { producto };
    }

    private void btnAgregar_Click(object sender, EventArgs e)
    {
        if (dgvProductos.CurrentRow is null || dgvProductos.CurrentRow.DataBoundItem is not Producto producto)
        {
            MessageBox.Show("Seleccione un producto de la lista.", "Advertencia");
            return;
        }

        var cantidad = (int)nudCantidad.Value;
        if (cantidad <= 0)
        {
            MessageBox.Show("La cantidad debe ser mayor que cero.", "Validación");
            return;
        }

        if (cantidad > producto.Stock)
        {
            MessageBox.Show($"No hay stock suficiente para {producto.Nombre}. Stock disponible: {producto.Stock}", "Stock insuficiente");
            return;
        }

        var detalleExistente = _carrito.FirstOrDefault(d => d.ProductoId == producto.Id);
        if (detalleExistente is not null)
        {
            detalleExistente.Cantidad += cantidad;
            detalleExistente.SubTotal = detalleExistente.Cantidad * detalleExistente.PrecioUnitario;
        }
        else
        {
            _carrito.Add(new DetalleVenta
            {
                ProductoId = producto.Id,
                Cantidad = cantidad,
                PrecioUnitario = producto.PrecioVenta,
                SubTotal = producto.PrecioVenta * cantidad
            });
        }

        ActualizarCarrito();
        MessageBox.Show($"Se agregó {cantidad} unidad(es) de {producto.Nombre} al carrito.", "Producto agregado");
    }

    private void btnQuitar_Click(object sender, EventArgs e)
    {
        if (dgvCarrito.CurrentRow is null || dgvCarrito.CurrentRow.DataBoundItem is not CarritoItem item)
        {
            MessageBox.Show("Seleccione un producto del carrito para quitarlo.", "Advertencia");
            return;
        }

        var detalle = _carrito.FirstOrDefault(d => d.ProductoId == item.ProductoId);
        if (detalle is not null)
        {
            _carrito.Remove(detalle);
            ActualizarCarrito();
            MessageBox.Show("Producto eliminado del carrito.", "Eliminado");
        }
    }

    private void btnGenerarVenta_Click(object sender, EventArgs e)
    {
        if (_carrito.Count == 0)
        {
            MessageBox.Show("Debe agregar al menos un producto al carrito.", "Venta vacía");
            return;
        }

        try
        {
            var venta = _ventaService.CrearVenta(1, null, _carrito.ToList());
            var repository = new VentaRepository();
            var ventaId = repository.InsertarVenta(venta);

            var numeroVenta = venta.NumeroDocumento ?? ventaId.ToString();
            MessageBox.Show($"Venta registrada correctamente. Código: {numeroVenta}", "Venta completada");

            _carrito.Clear();
            ActualizarCarrito();
            CargarProductos();
            txtBuscarProducto.Clear();
            nudCantidad.Value = 1;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error en la venta");
        }
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        _carrito.Clear();
        ActualizarCarrito();
        txtBuscarProducto.Clear();
        nudCantidad.Value = 1;
        MessageBox.Show("Se canceló la venta actual.", "Operación cancelada");
    }

    private void CargarProductos()
    {
        var productos = _productoService.ObtenerProductos();
        _productoBinding.DataSource = productos;
    }

    private void ActualizarCarrito()
    {
        var lista = _carrito
            .Select(d =>
            {
                var producto = _productoService.ObtenerPorId(d.ProductoId);
                return new CarritoItem
                {
                    ProductoId = d.ProductoId,
                    Codigo = producto?.Codigo ?? string.Empty,
                    Nombre = producto?.Nombre ?? string.Empty,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    SubTotal = d.SubTotal
                };
            })
            .ToList();

        _carritoBinding.DataSource = lista;
        dgvCarrito.DataSource = _carritoBinding;

        decimal subtotal = lista.Sum(x => x.SubTotal);
        decimal igv = Math.Round(subtotal * 0.18m, 2);
        decimal total = subtotal + igv;

        var culture = CultureInfo.GetCultureInfo("es-PE");
        lblSubtotal.Text = subtotal.ToString("C2", culture);
        lblIgv.Text = igv.ToString("C2", culture);
        lblTotal.Text = total.ToString("C2", culture);
    }

    private sealed class CarritoItem
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
    }
}
