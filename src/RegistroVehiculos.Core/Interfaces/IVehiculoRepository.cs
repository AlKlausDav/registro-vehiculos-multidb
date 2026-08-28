using RegistroVehiculos.Core.Models;

namespace RegistroVehiculos.Core.Interfaces;

public interface IVehiculoRepository
{
    Task<bool> ProbarConexionAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Vehiculo>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default);

    Task<Vehiculo?> ObtenerPorPlacaAsync(
        string placa,
        CancellationToken cancellationToken = default);

    Task<bool> ExistePlacaAsync(
        string placa,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Vehiculo vehiculo,
        CancellationToken cancellationToken = default);

    Task ActualizarAsync(
        string placaOriginal,
        Vehiculo vehiculo,
        CancellationToken cancellationToken = default);

    Task EliminarAsync(
        string placa,
        CancellationToken cancellationToken = default);
}