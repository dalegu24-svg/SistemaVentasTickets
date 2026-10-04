namespace SistemaVentas.Business.Services;

public class ReporteService
{
    public decimal CalcularTotalVentas(IEnumerable<decimal> valores)
    {
        return valores.Sum();
    }
}

