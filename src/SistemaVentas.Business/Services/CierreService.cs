using SistemaVentas.Common.Models;
using SistemaVentas.Data.Repositories;

namespace SistemaVentas.Business.Services;

public class CierreService
{
    private readonly CierreRepository _cierreRepository = new();

    public Cierre AbrirCaja(int usuarioId, decimal montoInicial)
    {
        if (usuarioId <= 0)
            throw new InvalidOperationException("El ID de usuario debe ser válido.");

        if (montoInicial < 0)
            throw new InvalidOperationException("El monto inicial no puede ser negativo.");

        var cierre = new Cierre
        {
            UsuarioId = usuarioId,
            FechaApertura = DateTime.Now,
            MontoInicial = montoInicial,
            MontoFinal = 0,
            TotalVentas = 0,
            Diferencia = 0,
            Estado = "Abierto"
        };

        var cierreId = _cierreRepository.InsertarCierre(cierre);
        cierre.Id = cierreId;
        return cierre;
    }

    public Cierre CerrarCaja(int usuarioId, decimal montoFinal, string observaciones = "")
    {
        if (usuarioId <= 0)
            throw new InvalidOperationException("El ID de usuario debe ser válido.");

        var cierreActivo = _cierreRepository.ObtenerCierreActivo(usuarioId);
        if (cierreActivo is null)
            throw new InvalidOperationException("No hay una caja abierta para este usuario.");

        if (montoFinal < 0)
            throw new InvalidOperationException("El monto final no puede ser negativo.");

        var totalVentas = _cierreRepository.ObtenerTotalVentasDelDia(usuarioId, cierreActivo.FechaApertura);
        var montoEsperado = cierreActivo.MontoInicial + totalVentas;
        var diferencia = montoFinal - montoEsperado;

        cierreActivo.FechaCierre = DateTime.Now;
        cierreActivo.MontoFinal = montoFinal;
        cierreActivo.TotalVentas = totalVentas;
        cierreActivo.Diferencia = diferencia;
        cierreActivo.Estado = "Cerrado";
        cierreActivo.Observaciones = observaciones;

        _cierreRepository.InsertarCierre(cierreActivo);
        return cierreActivo;
    }

    public Cierre? ObtenerCierreActivo(int usuarioId)
    {
        return _cierreRepository.ObtenerCierreActivo(usuarioId);
    }
}
