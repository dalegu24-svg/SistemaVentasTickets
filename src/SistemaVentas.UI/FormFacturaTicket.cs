using System.Diagnostics;
using System.Windows.Forms;
using SistemaVentas.Business.Services;
using SistemaVentas.Common.Models;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.UI;

public partial class FormFacturaTicket : Form
{
    private readonly Factura _factura;
    private readonly Ticket _ticket;
    private readonly TicketRepository _ticketRepository = new();

    public FormFacturaTicket(Factura factura, Ticket ticket)
    {
        _factura = factura;
        _ticket = ticket;
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        this.Text = "Factura y Ticket";
        this.Size = new Size(700, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(245, 247, 250);

        var panelTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(29, 78, 216)
        };

        var lblTitulo = new Label
        {
            Text = "Factura y Ticket de Venta",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(30, 12)
        };

        panelTop.Controls.Add(lblTitulo);
        this.Controls.Add(panelTop);

        var panelContenido = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20, 20, 20, 80),
            AutoScroll = true
        };

        var lblFactura = new Label
        {
            Text = "DATOS DE LA FACTURA",
            AutoSize = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(20, 20)
        };

        var txtFactura = new TextBox
        {
            ReadOnly = true,
            Dock = DockStyle.Top,
            Height = 150,
            Multiline = true,
            BorderStyle = BorderStyle.Fixed3D,
            BackColor = Color.White,
            Font = new Font("Courier New", 10, FontStyle.Regular),
            Text = GenerarTextoFactura()
        };

        var lblTicket = new Label
        {
            Text = "TICKET (para imprimir)",
            AutoSize = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(20, 200)
        };

        var txtTicket = new TextBox
        {
            ReadOnly = true,
            Dock = DockStyle.Top,
            Height = 200,
            Multiline = true,
            BorderStyle = BorderStyle.Fixed3D,
            BackColor = Color.White,
            Font = new Font("Courier New", 9, FontStyle.Regular),
            Text = _ticket.TextoTicket ?? "No hay contenido de ticket"
        };

        panelContenido.Controls.Add(txtTicket);
        panelContenido.Controls.Add(lblTicket);
        panelContenido.Controls.Add(txtFactura);
        panelContenido.Controls.Add(lblFactura);
        this.Controls.Add(panelContenido);

        var btnImprimirTicket = new Button
        {
            Text = "Imprimir Ticket",
            Size = new Size(130, 40),
            Location = new Point(250, 520),
            BackColor = Color.FromArgb(34, 197, 94),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnImprimirTicket.Click += btnImprimirTicket_Click;

        var btnCerrar = new Button
        {
            Text = "Cerrar",
            Size = new Size(130, 40),
            Location = new Point(420, 520),
            BackColor = Color.FromArgb(107, 114, 128),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnCerrar.Click += (s, e) => this.Close();

        this.Controls.Add(btnImprimirTicket);
        this.Controls.Add(btnCerrar);
    }

    private void CargarDatos()
    {
    }

    private string GenerarTextoFactura()
    {
        return $@"Número de Factura: {_factura.NumeroFactura}
Fecha de Emisión: {_factura.FechaEmision:dd/MM/yyyy HH:mm:ss}

Subtotal: S/ {_factura.SubTotal:F2}
IGV (18%): S/ {_factura.Igv:F2}

TOTAL: S/ {_factura.Total:F2}

Estado: {_factura.Estado}";
    }

    private void btnImprimirTicket_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_ticket.TextoTicket))
            {
                MessageBox.Show("No hay contenido de ticket para imprimir.", "Advertencia");
                return;
            }

            var archivoTemporal = Path.Combine(Path.GetTempPath(), "ticket_temp.txt");
            File.WriteAllText(archivoTemporal, _ticket.TextoTicket);

            var processInfo = new ProcessStartInfo
            {
                FileName = "notepad",
                Arguments = archivoTemporal,
                UseShellExecute = true
            };

            Process.Start(processInfo);

            MessageBox.Show("Ticket abierto en el editor. Selecciona Archivo > Imprimir para enviarlo a la impresora térmica.", "Información");

            _ticketRepository.MarcarComoImpreso(_ticket.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al imprimir: {ex.Message}", "Error");
        }
    }
}
