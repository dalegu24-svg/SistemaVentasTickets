using System.Windows.Forms;
using System.Data;
using System.Globalization;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.UI;

public class FormDashboard : Form
{
    private readonly ReporteRepository _reporteRepository = new();
    private DateTime _fechaDesde;
    private DateTime _fechaHasta;

    private Label lblIngresosTotales;
    private Label lblTotalVentas;
    private Label lblTotalProductos;
    private Label lblVentaPromedio;
    private DataGridView dgvVentasDiarias;
    private DataGridView dgvProductosMasVendidos;
    private DataGridView dgvVentasPorCajero;
    private DataGridView dgvVentasPorCategoria;
    private Button btnActualizar;
    private DateTimePicker dtpDesde;
    private DateTimePicker dtpHasta;

    public FormDashboard()
    {
        _fechaDesde = DateTime.Now.AddDays(-30);
        _fechaHasta = DateTime.Now;
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        this.Text = "Dashboard - Reportes Avanzados";
        this.Size = new Size(1400, 900);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(245, 247, 250);
        this.WindowState = FormWindowState.Maximized;

        var panelTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(29, 78, 216),
            Padding = new Padding(20)
        };

        var lblTitulo = new Label
        {
            Text = "DASHBOARD DE VENTAS",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(20, 15)
        };
        panelTop.Controls.Add(lblTitulo);
        this.Controls.Add(panelTop);

        var panelFiltros = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.White,
            Padding = new Padding(20, 10, 20, 10),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblDesdeTexto = new Label
        {
            Text = "Desde:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 15)
        };

        dtpDesde = new DateTimePicker
        {
            Value = _fechaDesde,
            Width = 150,
            Height = 32,
            Location = new Point(80, 10),
            Font = new Font("Segoe UI", 10)
        };

        var lblHastaTexto = new Label
        {
            Text = "Hasta:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(280, 15)
        };

        dtpHasta = new DateTimePicker
        {
            Value = _fechaHasta,
            Width = 150,
            Height = 32,
            Location = new Point(340, 10),
            Font = new Font("Segoe UI", 10)
        };

        btnActualizar = new Button
        {
            Text = "Actualizar",
            Size = new Size(120, 32),
            Location = new Point(540, 10),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnActualizar.Click += btnActualizar_Click;

        panelFiltros.Controls.Add(lblDesdeTexto);
        panelFiltros.Controls.Add(dtpDesde);
        panelFiltros.Controls.Add(lblHastaTexto);
        panelFiltros.Controls.Add(dtpHasta);
        panelFiltros.Controls.Add(btnActualizar);
        this.Controls.Add(panelFiltros);

        var panelKPIs = new Panel
        {
            Dock = DockStyle.Top,
            Height = 120,
            BackColor = Color.FromArgb(245, 247, 250),
            Padding = new Padding(20, 15, 20, 15)
        };

        // KPI 1: Ingresos Totales
        var panelKPI1 = CrearPanelKPI("Ingresos Totales", "S/ 0.00", Color.FromArgb(34, 197, 94), 20, 10);
        lblIngresosTotales = panelKPI1.Controls[1] as Label;
        panelKPIs.Controls.Add(panelKPI1);

        // KPI 2: Total Ventas
        var panelKPI2 = CrearPanelKPI("Total Ventas", "0", Color.FromArgb(59, 130, 246), 320, 10);
        lblTotalVentas = panelKPI2.Controls[1] as Label;
        panelKPIs.Controls.Add(panelKPI2);

        // KPI 3: Total Productos
        var panelKPI3 = CrearPanelKPI("Productos Vendidos", "0", Color.FromArgb(168, 85, 247), 620, 10);
        lblTotalProductos = panelKPI3.Controls[1] as Label;
        panelKPIs.Controls.Add(panelKPI3);

        // KPI 4: Venta Promedio
        var panelKPI4 = CrearPanelKPI("Venta Promedio", "S/ 0.00", Color.FromArgb(236, 72, 153), 920, 10);
        lblVentaPromedio = panelKPI4.Controls[1] as Label;
        panelKPIs.Controls.Add(panelKPI4);

        this.Controls.Add(panelKPIs);

        var panelContenido = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            AutoScroll = true,
            BackColor = Color.FromArgb(245, 247, 250)
        };

        // Tabla 1: Ventas Diarias
        var lblVentasDiarias = new Label
        {
            Text = "Ventas Diarias",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(0, 0)
        };
        panelContenido.Controls.Add(lblVentasDiarias);

        dgvVentasDiarias = CrearDataGridView();
        dgvVentasDiarias.Location = new Point(0, 30);
        dgvVentasDiarias.Size = new Size(650, 200);
        panelContenido.Controls.Add(dgvVentasDiarias);

        // Tabla 2: Productos Más Vendidos
        var lblProductosMasVendidos = new Label
        {
            Text = "Productos Más Vendidos",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(700, 0)
        };
        panelContenido.Controls.Add(lblProductosMasVendidos);

        dgvProductosMasVendidos = CrearDataGridView();
        dgvProductosMasVendidos.Location = new Point(700, 30);
        dgvProductosMasVendidos.Size = new Size(650, 200);
        panelContenido.Controls.Add(dgvProductosMasVendidos);

        // Tabla 3: Ventas Por Cajero
        var lblVentasPorCajero = new Label
        {
            Text = "Ventas por Cajero",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(0, 250)
        };
        panelContenido.Controls.Add(lblVentasPorCajero);

        dgvVentasPorCajero = CrearDataGridView();
        dgvVentasPorCajero.Location = new Point(0, 280);
        dgvVentasPorCajero.Size = new Size(650, 220);
        panelContenido.Controls.Add(dgvVentasPorCajero);

        // Tabla 4: Ventas Por Categoría
        var lblVentasPorCategoria = new Label
        {
            Text = "Ventas por Categoría",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(700, 250)
        };
        panelContenido.Controls.Add(lblVentasPorCategoria);

        dgvVentasPorCategoria = CrearDataGridView();
        dgvVentasPorCategoria.Location = new Point(700, 280);
        dgvVentasPorCategoria.Size = new Size(650, 220);
        panelContenido.Controls.Add(dgvVentasPorCategoria);

        this.Controls.Add(panelContenido);
    }

    private Panel CrearPanelKPI(string titulo, string valor, Color color, int x, int y)
    {
        var panel = new Panel
        {
            Size = new Size(280, 100),
            Location = new Point(x, y),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblTitulo = new Label
        {
            Text = titulo,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(107, 114, 128),
            AutoSize = true,
            Location = new Point(15, 12)
        };

        var lblValor = new Label
        {
            Text = valor,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = color,
            AutoSize = true,
            Location = new Point(15, 40)
        };

        var lineaColor = new Panel
        {
            BackColor = color,
            Size = new Size(280, 3),
            Location = new Point(0, 97)
        };

        panel.Controls.Add(lblTitulo);
        panel.Controls.Add(lblValor);
        panel.Controls.Add(lineaColor);

        return panel;
    }

    private DataGridView CrearDataGridView()
    {
        return new DataGridView
        {
            AutoGenerateColumns = true,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            BorderStyle = BorderStyle.FixedSingle,
            BackgroundColor = Color.White,
            Font = new Font("Segoe UI", 9, FontStyle.Regular),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false
        };
    }

    private void btnActualizar_Click(object sender, EventArgs e)
    {
        _fechaDesde = dtpDesde.Value;
        _fechaHasta = dtpHasta.Value;
        CargarDatos();
    }

    private void CargarDatos()
    {
        try
        {
            // KPIs
            var ingresosTotales = _reporteRepository.ObtenerIngresosTotales(_fechaDesde, _fechaHasta);
            var totalVentas = _reporteRepository.ObtenerTotalVentas(_fechaDesde, _fechaHasta);
            var totalProductos = _reporteRepository.ObtenerTotalProductosVendidos(_fechaDesde, _fechaHasta);
            var ventaPromedio = totalVentas > 0 ? ingresosTotales / totalVentas : 0;

            var culture = CultureInfo.GetCultureInfo("es-PE");
            lblIngresosTotales.Text = ingresosTotales.ToString("C2", culture);
            lblTotalVentas.Text = totalVentas.ToString();
            lblTotalProductos.Text = totalProductos.ToString();
            lblVentaPromedio.Text = ventaPromedio.ToString("C2", culture);

            // Tablas
            dgvVentasDiarias.DataSource = _reporteRepository.ObtenerResumenVentasDiarias(_fechaDesde, _fechaHasta);
            dgvProductosMasVendidos.DataSource = _reporteRepository.ObtenerProductosMasVendidos(_fechaDesde, _fechaHasta, 10);
            dgvVentasPorCajero.DataSource = _reporteRepository.ObtenerVentasPorCajero(_fechaDesde, _fechaHasta);
            dgvVentasPorCategoria.DataSource = _reporteRepository.ObtenerVentasPorCategoria(_fechaDesde, _fechaHasta);

            AjustarAnchosColumnas();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error");
        }
    }

    private void AjustarAnchosColumnas()
    {
        foreach (var dgv in new[] { dgvVentasDiarias, dgvProductosMasVendidos, dgvVentasPorCajero, dgvVentasPorCategoria })
        {
            if (dgv.DataSource is DataTable table)
            {
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.AutoHeaderHeight = true;
                    column.Width = Math.Max(column.Width, 80);
                }
            }
        }
    }
}
