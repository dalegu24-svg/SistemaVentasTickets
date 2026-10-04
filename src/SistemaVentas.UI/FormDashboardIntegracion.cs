using System.Windows.Forms;
using SistemaVentas.Business.Services;

namespace SistemaVentas.UI;

public partial class Form1 : Form
{
    private Button btnDashboard;

    private void AgregarBotonDashboard()
    {
        btnDashboard = new Button
        {
            Text = "Dashboard",
            Size = new Size(120, 40),
            Location = new Point(1050, 620),
            BackColor = Color.FromArgb(147, 51, 234),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnDashboard.Click += btnDashboard_Click;
        this.Controls.Add(btnDashboard);
    }

    private void btnDashboard_Click(object sender, EventArgs e)
    {
        using var form = new FormDashboard();
        form.ShowDialog(this);
    }
}
