using System.Globalization;
using System.Windows.Forms;
using SistemaVentas.Business.Services;
using SistemaVentas.Common.Models;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.UI;

public class FormCierre : Form
{
    private readonly CierreService _cierreService = new();
    private readonly CierreRepository _cierreRepository = new();
    private readonly UsuarioService _usuarioService;
    private Cierre? _cierreActivo;
    private Label lblEstado;
    private Label lblMontoInicial;
    private Label lblTotalVentas;
    private Label lblMontoEsperado;
    private Label lblMontoIngresado;
    private Label lblDiferencia;
    private NumericUpDown nudMontoFinal;
    private TextBox txtObservaciones;
    private Button btnAbrirCaja;
    private Button btnCerrarCaja;

    public FormCierre(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
        InitializeComponent();
        CargarEstadoCaja();
    }

    private void InitializeComponent()
    {
        this.Text = "Cierre de Caja";
        this.Size = new Size(600, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(245, 247, 250);

        var panelTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(220, 38, 38)
        };

        var lblTitulo = new Label
        {
            Text = "CIERRE DE CAJA",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(30, 15)
        };
        panelTop.Controls.Add(lblTitulo);
        this.Controls.Add(panelTop);

        var panelContenido = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(25, 25, 25, 25),
            AutoScroll = true
        };

        lblEstado = new Label
        {
            Text = "Estado: Caja cerrada",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            AutoSize = true,
            Location = new Point(25, 90)
        };
        panelContenido.Controls.Add(lblEstado);

        var panelDatos = new Panel
        {
            Location = new Point(25, 130),
            Size = new Size(520, 200),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblMontoInicialTexto = new Label
        {
            Text = "Monto inicial:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(15, 15)
        };
        lblMontoInicial = new Label
        {
            Text = "S/ 0.00",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(350, 15)
        };

        var lblTotalVentasTexto = new Label
        {
            Text = "Total ventas del día:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(15, 45)
        };
        lblTotalVentas = new Label
        {
            Text = "S/ 0.00",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(350, 45)
        };

        var lblMontoEsperadoTexto = new Label
        {
            Text = "Monto esperado:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(15, 75)
        };
        lblMontoEsperado = new Label
        {
            Text = "S/ 0.00",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(350, 75)
        };

        var lblMontoIngresadoTexto = new Label
        {
            Text = "Monto ingresado:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(15, 105)
        };
        lblMontoIngresado = new Label
        {
            Text = "S/ 0.00",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(350, 105)
        };

        var lblDiferenciaTexto = new Label
        {
            Text = "Diferencia:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(15, 135)
        };
        lblDiferencia = new Label
        {
            Text = "S/ 0.00",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(34, 197, 94),
            AutoSize = true,
            Location = new Point(350, 135)
        };

        panelDatos.Controls.Add(lblMontoInicialTexto);
        panelDatos.Controls.Add(lblMontoInicial);
        panelDatos.Controls.Add(lblTotalVentasTexto);
        panelDatos.Controls.Add(lblTotalVentas);
        panelDatos.Controls.Add(lblMontoEsperadoTexto);
        panelDatos.Controls.Add(lblMontoEsperado);
        panelDatos.Controls.Add(lblMontoIngresadoTexto);
        panelDatos.Controls.Add(lblMontoIngresado);
        panelDatos.Controls.Add(lblDiferenciaTexto);
        panelDatos.Controls.Add(lblDiferencia);
        panelContenido.Controls.Add(panelDatos);

        var lblMontoFinalTexto = new Label
        {
            Text = "Ingrese monto final:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Location = new Point(25, 350)
        };
        panelContenido.Controls.Add(lblMontoFinalTexto);

        nudMontoFinal = new NumericUpDown
        {
            Location = new Point(25, 375),
            Size = new Size(200, 35),
            DecimalPlaces = 2,
            Minimum = 0,
            Maximum = 999999,
            Font = new Font("Segoe UI", 11)
        };
        panelContenido.Controls.Add(nudMontoFinal);

        var lblObservacionesTexto = new Label
        {
            Text = "Observaciones:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Location = new Point(25, 420)
        };
        panelContenido.Controls.Add(lblObservacionesTexto);

        txtObservaciones = new TextBox
        {
            Location = new Point(25, 445),
            Size = new Size(520, 60),
            Multiline = true,
            Font = new Font("Segoe UI", 10)
        };
        panelContenido.Controls.Add(txtObservaciones);

        this.Controls.Add(panelContenido);

        btnAbrirCaja = new Button
        {
            Text = "Abrir caja",
            Size = new Size(140, 40),
            Location = new Point(140, 420),
            BackColor = Color.FromArgb(34, 197, 94),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnAbrirCaja.Click += btnAbrirCaja_Click;

        btnCerrarCaja = new Button
        {
            Text = "Cerrar caja",
            Size = new Size(140, 40),
            Location = new Point(310, 420),
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnCerrarCaja.Click += btnCerrarCaja_Click;

        this.Controls.Add(btnAbrirCaja);
        this.Controls.Add(btnCerrarCaja);
    }

    private void CargarEstadoCaja()
    {
        var usuarioId = _usuarioService.UsuarioActivo?.Id ?? 0;
        _cierreActivo = _cierreService.ObtenerCierreActivo(usuarioId);

        if (_cierreActivo is null)
        {
            lblEstado.Text = "Estado: Caja cerrada";
            lblEstado.ForeColor = Color.FromArgb(239, 68, 68);
            btnAbrirCaja.Enabled = true;
            btnCerrarCaja.Enabled = false;
            nudMontoFinal.Enabled = false;
            txtObservaciones.Enabled = false;
        }
        else
        {
            lblEstado.Text = $"Estado: Caja abierta desde {_cierreActivo.FechaApertura:dd/MM/yyyy HH:mm:ss}";
            lblEstado.ForeColor = Color.FromArgb(34, 197, 94);
            lblMontoInicial.Text = _cierreActivo.MontoInicial.ToString("C2", CultureInfo.GetCultureInfo("es-PE"));
            lblTotalVentas.Text = _cierreRepository.ObtenerTotalVentasDelDia(usuarioId, _cierreActivo.FechaApertura).ToString("C2", CultureInfo.GetCultureInfo("es-PE"));
            var montoEsperado = _cierreActivo.MontoInicial + _cierreRepository.ObtenerTotalVentasDelDia(usuarioId, _cierreActivo.FechaApertura);
            lblMontoEsperado.Text = montoEsperado.ToString("C2", CultureInfo.GetCultureInfo("es-PE"));
            btnAbrirCaja.Enabled = false;
            btnCerrarCaja.Enabled = true;
            nudMontoFinal.Enabled = true;
            txtObservaciones.Enabled = true;
        }
    }

    private void btnAbrirCaja_Click(object sender, EventArgs e)
    {
        var montoInicial = (decimal)nudMontoFinal.Value;
        if (montoInicial < 0)
        {
            MessageBox.Show("El monto inicial debe ser mayor o igual a cero.", "Validación");
            return;
        }

        try
        {
            var usuarioId = _usuarioService.UsuarioActivo?.Id ?? 0;
            _cierreService.AbrirCaja(usuarioId, montoInicial);
            MessageBox.Show("Caja abierta correctamente.", "Éxito");
            nudMontoFinal.Value = 0;
            CargarEstadoCaja();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
        }
    }

    private void btnCerrarCaja_Click(object sender, EventArgs e)
    {
        if (_cierreActivo is null)
        {
            MessageBox.Show("No hay una caja abierta para cerrar.", "Advertencia");
            return;
        }

        var montoFinal = (decimal)nudMontoFinal.Value;
        var observaciones = txtObservaciones.Text.Trim();

        if (montoFinal < 0)
        {
            MessageBox.Show("El monto final debe ser mayor o igual a cero.", "Validación");
            return;
        }

        try
        {
            var usuarioId = _usuarioService.UsuarioActivo?.Id ?? 0;
            var cierre = _cierreService.CerrarCaja(usuarioId, montoFinal, observaciones);

            var culture = CultureInfo.GetCultureInfo("es-PE");
            var mensaje = $"Caja cerrada correctamente.\n\n" +
                         $"Monto inicial: {cierre.MontoInicial.ToString("C2", culture)}\n" +
                         $"Total ventas: {cierre.TotalVentas.ToString("C2", culture)}\n" +
                         $"Monto esperado: {(cierre.MontoInicial + cierre.TotalVentas).ToString("C2", culture)}\n" +
                         $"Monto ingresado: {cierre.MontoFinal.ToString("C2", culture)}\n" +
                         $"Diferencia: {cierre.Diferencia.ToString("C2", culture)}";

            MessageBox.Show(mensaje, "Cierre de caja completado");
            nudMontoFinal.Value = 0;
            txtObservaciones.Clear();
            CargarEstadoCaja();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
        }
    }
}
