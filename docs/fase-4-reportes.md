using System.Data;
using System.Globalization;
using System.Windows.Forms;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.UI;

public class FormReportes : Form
{
    private readonly ReporteRepository _reporteRepository = new();
    private DateTimePicker dtpDesde;
    private DateTimePicker dtpHasta;
    private ComboBox cboTipoReporte;
    private DataGridView dgvReportes;
    private Label lblResumen;
    private Button btnConsultar;

    public FormReportes()
    {
        InitializeComponent();
        CargarReporteActual();
    }

    private void InitializeComponent()
    {
        this.Text = "Reportes del sistema";
        this.Size = new Size(980, 620);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(245, 247, 250);

        var panelTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(15, 118, 110)
        };

        var lblTitulo = new Label
        {
            Text = "REPORTES",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(24, 24)
        };
        panelTop.Controls.Add(lblTitulo);
        this.Controls.Add(panelTop);

        var panelFiltros = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            Padding = new Padding(15)
        };

        var lblDesde = new Label
        {
            Text = "Desde:",
            AutoSize = true,
            Location = new Point(15, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        dtpDesde = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Width = 120,
            Location = new Point(75, 15)
        };

        var lblHasta = new Label
        {
            Text = "Hasta:",
            AutoSize = true,
            Location = new Point(230, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        dtpHasta = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Width = 120,
            Location = new Point(285, 15)
        };

        var lblTipo = new Label
        {
            Text = "Tipo:",
            AutoSize = true,
            Location = new Point(445, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        cboTipoReporte = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 180,
            Location = new Point(500, 15),
            Font = new Font("Segoe UI", 10)
        };
        cboTipoReporte.Items.Add("Ventas por fecha");
        cboTipoReporte.Items.Add("Productos más vendidos");
        cboTipoReporte.SelectedIndex = 0;

        btnConsultar = new Button
        {
            Text = "Consultar",
            Width = 120,
            Height = 35,
            Location = new Point(710, 12),
            BackColor = Color.FromArgb(59, 130, 246),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnConsultar.Click += btnConsultar_Click;

        panelFiltros.Controls.Add(lblDesde);
        panelFiltros.Controls.Add(dtpDesde);
        panelFiltros.Controls.Add(lblHasta);
        panelFiltros.Controls.Add(dtpHasta);
        panelFiltros.Controls.Add(lblTipo);
        panelFiltros.Controls.Add(cboTipoReporte);
        panelFiltros.Controls.Add(btnConsultar);
        this.Controls.Add(panelFiltros);

        lblResumen = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(15, 110)
        };
        this.Controls.Add(lblResumen);

        dgvReportes = new DataGridView
        {
            Location = new Point(15, 145),
            Size = new Size(930, 390),
            AutoGenerateColumns = true,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            BorderStyle = BorderStyle.Fixed3D,
            BackgroundColor = Color.White,
            Font = new Font("Segoe UI", 10)
        };
        this.Controls.Add(dgvReportes);
    }

    private void btnConsultar_Click(object sender, EventArgs e)
    {
        CargarReporteActual();
    }

    private void CargarReporteActual()
    {
        var desde = dtpDesde.Value.Date;
        var hasta = dtpHasta.Value.Date;

        if (desde > hasta)
        {
            MessageBox.Show("La fecha de inicio no puede ser mayor que la fecha final.", "Validación");
            return;
        }

        if (cboTipoReporte.SelectedIndex == 0)
        {
            var data = _reporteRepository.ObtenerVentasPorFecha(desde, hasta);
            dgvReportes.DataSource = data;

            var total = _reporteRepository.ObtenerTotalVentasPeriodo(desde, hasta);
            lblResumen.Text = $"Total de ventas en el periodo: {total.ToString("C2", CultureInfo.GetCultureInfo("es-PE"))}";
        }
        else
        {
            var data = _reporteRepository.ObtenerTopProductos(desde, hasta);
            dgvReportes.DataSource = data;

            var total = _reporteRepository.ObtenerTotalVentasPeriodo(desde, hasta);
            lblResumen.Text = $"Total vendido en el periodo: {total.ToString("C2", CultureInfo.GetCultureInfo("es-PE"))}";
        }
    }
}

