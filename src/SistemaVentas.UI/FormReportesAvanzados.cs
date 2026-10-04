using System.Windows.Forms;
using System.Data;
using System.Globalization;
using System.Text;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.UI;

public class FormReportesAvanzados : Form
{
    private readonly ReporteRepository _reporteRepository = new();
    private DateTime _fechaDesde;
    private DateTime _fechaHasta;
    private TabControl tabControl;

    public FormReportesAvanzados()
    {
        _fechaDesde = DateTime.Now.AddDays(-30);
        _fechaHasta = DateTime.Now;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Reportes Avanzados";
        this.Size = new Size(1200, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(245, 247, 250);
        this.WindowState = FormWindowState.Maximized;

        var panelTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(220, 38, 38),
            Padding = new Padding(20)
        };

        var lblTitulo = new Label
        {
            Text = "REPORTES DETALLADOS",
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
            Height = 70,
            BackColor = Color.White,
            Padding = new Padding(20, 10, 20, 10),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblDesde = new Label { Text = "Desde:", AutoSize = true, Location = new Point(20, 15) };
        var dtpDesde = new DateTimePicker { Value = _fechaDesde, Width = 150, Location = new Point(80, 10) };

        var lblHasta = new Label { Text = "Hasta:", AutoSize = true, Location = new Point(280, 15) };
        var dtpHasta = new DateTimePicker { Value = _fechaHasta, Width = 150, Location = new Point(340, 10) };

        var btnActualizar = new Button
        {
            Text = "Actualizar",
            Size = new Size(120, 32),
            Location = new Point(540, 10),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnActualizar.Click += (s, e) =>
        {
            _fechaDesde = dtpDesde.Value;
            _fechaHasta = dtpHasta.Value;
            CargarReportes();
        };

        var btnExportarPDF = new Button
        {
            Text = "Exportar PDF",
            Size = new Size(120, 32),
            Location = new Point(680, 10),
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnExportarPDF.Click += (s, e) => ExportarPDF();

        var btnExportarExcel = new Button
        {
            Text = "Exportar Excel",
            Size = new Size(120, 32),
            Location = new Point(820, 10),
            BackColor = Color.FromArgb(34, 197, 94),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnExportarExcel.Click += (s, e) => ExportarExcel();

        panelFiltros.Controls.Add(lblDesde);
        panelFiltros.Controls.Add(dtpDesde);
        panelFiltros.Controls.Add(lblHasta);
        panelFiltros.Controls.Add(dtpHasta);
        panelFiltros.Controls.Add(btnActualizar);
        panelFiltros.Controls.Add(btnExportarPDF);
        panelFiltros.Controls.Add(btnExportarExcel);
        this.Controls.Add(panelFiltros);

        tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10)
        };

        // Tab 1: Resumen Diario
        var tabResumen = new TabPage { Text = "Resumen Diario", Padding = new Padding(10) };
        var dgvResumen = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            ReadOnly = true,
            AllowUserToAddRows = false
        };
        dgvResumen.Tag = "resumen";
        tabResumen.Controls.Add(dgvResumen);
        tabControl.TabPages.Add(tabResumen);

        // Tab 2: Productos Más Vendidos
        var tabProductos = new TabPage { Text = "Productos Más Vendidos", Padding = new Padding(10) };
        var dgvProductos = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            ReadOnly = true,
            AllowUserToAddRows = false
        };
        dgvProductos.Tag = "productos";
        tabProductos.Controls.Add(dgvProductos);
        tabControl.TabPages.Add(tabProductos);

        // Tab 3: Ventas por Cajero
        var tabCajeros = new TabPage { Text = "Ventas por Cajero", Padding = new Padding(10) };
        var dgvCajeros = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            ReadOnly = true,
            AllowUserToAddRows = false
        };
        dgvCajeros.Tag = "cajeros";
        tabCajeros.Controls.Add(dgvCajeros);
        tabControl.TabPages.Add(tabCajeros);

        // Tab 4: Ventas por Categoría
        var tabCategorias = new TabPage { Text = "Ventas por Categoría", Padding = new Padding(10) };
        var dgvCategorias = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            ReadOnly = true,
            AllowUserToAddRows = false
        };
        dgvCategorias.Tag = "categorias";
        tabCategorias.Controls.Add(dgvCategorias);
        tabControl.TabPages.Add(tabCategorias);

        this.Controls.Add(tabControl);
        CargarReportes();
    }

    private void CargarReportes()
    {
        try
        {
            foreach (TabPage tab in tabControl.TabPages)
            {
                var dgv = tab.Controls[0] as DataGridView;
                var tag = dgv?.Tag?.ToString();

                dgv.DataSource = tag switch
                {
                    "resumen" => _reporteRepository.ObtenerResumenVentasDiarias(_fechaDesde, _fechaHasta),
                    "productos" => _reporteRepository.ObtenerProductosMasVendidos(_fechaDesde, _fechaHasta, 20),
                    "cajeros" => _reporteRepository.ObtenerVentasPorCajero(_fechaDesde, _fechaHasta),
                    "categorias" => _reporteRepository.ObtenerVentasPorCategoria(_fechaDesde, _fechaHasta),
                    _ => new DataTable()
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
        }
    }

    private void ExportarPDF()
    {
        MessageBox.Show("Función de exportación a PDF disponible próximamente.\n\nPara implementar:
1. Instalar iTextSharp
2. Convertir datos a PDF
3. Guardar en carpeta de descargas", "Exportar PDF");
    }

    private void ExportarExcel()
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"Reporte_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Función de exportación a Excel disponible próximamente.\n\nPara implementar:
1. Instalar ClosedXML
2. Crear libro de trabajo
3. Agregar hojas por reporte
4. Guardar archivo", "Exportar Excel");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
        }
    }
}
