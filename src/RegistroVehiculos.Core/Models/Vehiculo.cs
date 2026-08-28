namespace RegistroVehiculos.Core.Models;

public sealed class Vehiculo
{
    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public int Anio { get; set; }

    public string Color { get; set; } = string.Empty;
}