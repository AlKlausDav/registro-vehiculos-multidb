using RegistroVehiculos.Core.Enums;
using RegistroVehiculos.Core.Interfaces;
using RegistroVehiculos.Infrastructure.Configuration;
using RegistroVehiculos.Infrastructure.Repositories;

namespace RegistroVehiculos.Infrastructure.Factories;

public sealed class VehiculoRepositoryFactory
{
    private readonly ConfiguracionAplicacion configuracion;

    public VehiculoRepositoryFactory(
        ConfiguracionAplicacion configuracion)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        this.configuracion = configuracion;
    }

    public IVehiculoRepository Crear(
        MotorBaseDatos motor)
    {
        string cadenaConexion =
            configuracion.ObtenerCadenaConexion(motor);

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new InvalidOperationException(
                $"No existe una cadena de conexión " +
                $"configurada para {motor}.");
        }

        return motor switch
        {
            MotorBaseDatos.MySql =>
                new MySqlVehiculoRepository(
                    cadenaConexion),

            MotorBaseDatos.SqlServer =>
                new SqlServerVehiculoRepository(
                    cadenaConexion),

            MotorBaseDatos.Oracle =>
                new OracleVehiculoRepository(
                    cadenaConexion),

            _ => throw new ArgumentOutOfRangeException(
                nameof(motor),
                motor,
                "El motor seleccionado no es válido.")
        };
    }
}