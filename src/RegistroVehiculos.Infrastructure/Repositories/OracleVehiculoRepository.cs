using Oracle.ManagedDataAccess.Client;
using RegistroVehiculos.Core.Interfaces;
using RegistroVehiculos.Core.Models;

namespace RegistroVehiculos.Infrastructure.Repositories;

public sealed class OracleVehiculoRepository
    : IVehiculoRepository
{
    private readonly string cadenaConexion;

    public OracleVehiculoRepository(
        string cadenaConexion)
    {
        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new ArgumentException(
                "La cadena de conexión de Oracle es obligatoria.",
                nameof(cadenaConexion));
        }

        this.cadenaConexion = cadenaConexion;
    }

    public async Task<bool> ProbarConexionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using OracleConnection conexion =
                CrearConexion();

            await conexion.OpenAsync(cancellationToken);

            return true;
        }
        catch (OracleException)
        {
            return false;
        }
    }

    public async Task<IReadOnlyList<Vehiculo>>
        ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                placa,
                marca,
                modelo,
                anio,
                color
            FROM vehiculos
            ORDER BY placa
            """;

        var vehiculos = new List<Vehiculo>();

        using OracleConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using OracleCommand comando =
            CrearComando(conexion, sql);

        using var lector =
            await comando.ExecuteReaderAsync(
                cancellationToken);

        int columnaPlaca =
            lector.GetOrdinal("PLACA");

        int columnaMarca =
            lector.GetOrdinal("MARCA");

        int columnaModelo =
            lector.GetOrdinal("MODELO");

        int columnaAnio =
            lector.GetOrdinal("ANIO");

        int columnaColor =
            lector.GetOrdinal("COLOR");

        while (await lector.ReadAsync(
            cancellationToken))
        {
            vehiculos.Add(new Vehiculo
            {
                Placa =
                    lector.GetString(columnaPlaca),

                Marca =
                    lector.GetString(columnaMarca),

                Modelo =
                    lector.GetString(columnaModelo),

                Anio =
                    Convert.ToInt32(
                        lector.GetValue(columnaAnio)),

                Color =
                    lector.GetString(columnaColor)
            });
        }

        return vehiculos;
    }

    public async Task<Vehiculo?>
        ObtenerPorPlacaAsync(
            string placa,
            CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                placa,
                marca,
                modelo,
                anio,
                color
            FROM vehiculos
            WHERE placa = :placa
              AND ROWNUM = 1
            """;

        using OracleConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using OracleCommand comando =
            CrearComando(conexion, sql);

        comando.Parameters.Add(
            "placa",
            OracleDbType.Varchar2,
            15).Value = NormalizarPlaca(placa);

        using var lector =
            await comando.ExecuteReaderAsync(
                cancellationToken);

        if (!await lector.ReadAsync(
            cancellationToken))
        {
            return null;
        }

        return new Vehiculo
        {
            Placa =
                lector.GetString(
                    lector.GetOrdinal("PLACA")),

            Marca =
                lector.GetString(
                    lector.GetOrdinal("MARCA")),

            Modelo =
                lector.GetString(
                    lector.GetOrdinal("MODELO")),

            Anio =
                Convert.ToInt32(
                    lector.GetValue(
                        lector.GetOrdinal("ANIO"))),

            Color =
                lector.GetString(
                    lector.GetOrdinal("COLOR"))
        };
    }

    public async Task<bool> ExistePlacaAsync(
        string placa,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM vehiculos
            WHERE placa = :placa
            """;

        using OracleConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using OracleCommand comando =
            CrearComando(conexion, sql);

        comando.Parameters.Add(
            "placa",
            OracleDbType.Varchar2,
            15).Value = NormalizarPlaca(placa);

        object? resultado =
            await comando.ExecuteScalarAsync(
                cancellationToken);

        return Convert.ToInt32(resultado) > 0;
    }

    public async Task AgregarAsync(
        Vehiculo vehiculo,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(vehiculo);

        const string sql = """
            INSERT INTO vehiculos
            (
                placa,
                marca,
                modelo,
                anio,
                color
            )
            VALUES
            (
                :placa,
                :marca,
                :modelo,
                :anio,
                :color
            )
            """;

        using OracleConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using OracleCommand comando =
            CrearComando(conexion, sql);

        AgregarParametrosVehiculo(
            comando,
            vehiculo);

        await comando.ExecuteNonQueryAsync(
            cancellationToken);
    }

    public async Task ActualizarAsync(
        string placaOriginal,
        Vehiculo vehiculo,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(vehiculo);

        const string sql = """
            UPDATE vehiculos
            SET
                placa = :placa,
                marca = :marca,
                modelo = :modelo,
                anio = :anio,
                color = :color
            WHERE placa = :placaOriginal
            """;

        using OracleConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using OracleCommand comando =
            CrearComando(conexion, sql);

        AgregarParametrosVehiculo(
            comando,
            vehiculo);

        comando.Parameters.Add(
            "placaOriginal",
            OracleDbType.Varchar2,
            15).Value =
                NormalizarPlaca(placaOriginal);

        int filasAfectadas =
            await comando.ExecuteNonQueryAsync(
                cancellationToken);

        if (filasAfectadas == 0)
        {
            throw new KeyNotFoundException(
                $"No existe el vehículo con placa " +
                $"{placaOriginal}.");
        }
    }

    public async Task EliminarAsync(
        string placa,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM vehiculos
            WHERE placa = :placa
            """;

        using OracleConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using OracleCommand comando =
            CrearComando(conexion, sql);

        comando.Parameters.Add(
            "placa",
            OracleDbType.Varchar2,
            15).Value = NormalizarPlaca(placa);

        int filasAfectadas =
            await comando.ExecuteNonQueryAsync(
                cancellationToken);

        if (filasAfectadas == 0)
        {
            throw new KeyNotFoundException(
                $"No existe el vehículo con placa " +
                $"{placa}.");
        }
    }

    private OracleConnection CrearConexion()
    {
        return new OracleConnection(
            cadenaConexion);
    }

    private static OracleCommand CrearComando(
        OracleConnection conexion,
        string sql)
    {
        OracleCommand comando =
            conexion.CreateCommand();

        comando.CommandText = sql;
        comando.BindByName = true;

        return comando;
    }

    private static void AgregarParametrosVehiculo(
        OracleCommand comando,
        Vehiculo vehiculo)
    {
        comando.Parameters.Add(
            "placa",
            OracleDbType.Varchar2,
            15).Value =
                NormalizarPlaca(vehiculo.Placa);

        comando.Parameters.Add(
            "marca",
            OracleDbType.Varchar2,
            50).Value =
                vehiculo.Marca.Trim();

        comando.Parameters.Add(
            "modelo",
            OracleDbType.Varchar2,
            50).Value =
                vehiculo.Modelo.Trim();

        comando.Parameters.Add(
            "anio",
            OracleDbType.Int32).Value =
                vehiculo.Anio;

        comando.Parameters.Add(
            "color",
            OracleDbType.Varchar2,
            30).Value =
                vehiculo.Color.Trim();
    }

    private static string NormalizarPlaca(
        string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            throw new ArgumentException(
                "La placa es obligatoria.",
                nameof(placa));
        }

        return placa
            .Trim()
            .ToUpperInvariant();
    }
}