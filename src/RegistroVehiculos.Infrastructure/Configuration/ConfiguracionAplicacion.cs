using RegistroVehiculos.Core.Enums;

namespace RegistroVehiculos.Infrastructure.Configuration;

public sealed class ConfiguracionAplicacion
{
    public CadenasConexion ConnectionStrings { get; set; } = new();

    public string ObtenerCadenaConexion(
        MotorBaseDatos motor)
    {
        return motor switch
        {
            MotorBaseDatos.MySql =>
                ConnectionStrings.MySql,

            MotorBaseDatos.SqlServer =>
                ConnectionStrings.SqlServer,

            MotorBaseDatos.Oracle =>
                ConnectionStrings.Oracle,

            _ => throw new ArgumentOutOfRangeException(
                nameof(motor),
                motor,
                "El motor seleccionado no es válido.")
        };
    }
}

public sealed class CadenasConexion
{
    public string MySql { get; set; } = string.Empty;

    public string SqlServer { get; set; } = string.Empty;

    public string Oracle { get; set; } = string.Empty;
}