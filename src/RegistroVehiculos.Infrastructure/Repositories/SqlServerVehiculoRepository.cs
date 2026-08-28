using System.Data;
using Microsoft.Data.SqlClient;
using RegistroVehiculos.Core.Interfaces;
using RegistroVehiculos.Core.Models;

namespace RegistroVehiculos.Infrastructure.Repositories;

public sealed class SqlServerVehiculoRepository
    : IVehiculoRepository
{
    private readonly string cadenaConexion;

    public SqlServerVehiculoRepository(
        string cadenaConexion)
    {
        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new ArgumentException(
                "La cadena de conexión de SQL Server es obligatoria.",
                nameof(cadenaConexion));
        }

        this.cadenaConexion = cadenaConexion;
    }

    public async Task<bool> ProbarConexionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using SqlConnection conexion =
                CrearConexion();

            await conexion.OpenAsync(cancellationToken);

            return true;
        }
        catch (SqlException)
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
            FROM dbo.vehiculos
            ORDER BY placa;
            """;

        var vehiculos = new List<Vehiculo>();

        using SqlConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using SqlCommand comando =
            new(sql, conexion);

        using SqlDataReader lector =
            await comando.ExecuteReaderAsync(
                cancellationToken);

        int columnaPlaca =
            lector.GetOrdinal("placa");

        int columnaMarca =
            lector.GetOrdinal("marca");

        int columnaModelo =
            lector.GetOrdinal("modelo");

        int columnaAnio =
            lector.GetOrdinal("anio");

        int columnaColor =
            lector.GetOrdinal("color");

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
                    lector.GetInt32(columnaAnio),

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
            SELECT TOP (1)
                placa,
                marca,
                modelo,
                anio,
                color
            FROM dbo.vehiculos
            WHERE placa = @placa;
            """;

        using SqlConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using SqlCommand comando =
            new(sql, conexion);

        comando.Parameters.Add(
            "@placa",
            SqlDbType.VarChar,
            15).Value = NormalizarPlaca(placa);

        using SqlDataReader lector =
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
                    lector.GetOrdinal("placa")),

            Marca =
                lector.GetString(
                    lector.GetOrdinal("marca")),

            Modelo =
                lector.GetString(
                    lector.GetOrdinal("modelo")),

            Anio =
                lector.GetInt32(
                    lector.GetOrdinal("anio")),

            Color =
                lector.GetString(
                    lector.GetOrdinal("color"))
        };
    }

    public async Task<bool> ExistePlacaAsync(
        string placa,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM dbo.vehiculos
            WHERE placa = @placa;
            """;

        using SqlConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using SqlCommand comando =
            new(sql, conexion);

        comando.Parameters.Add(
            "@placa",
            SqlDbType.VarChar,
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
            INSERT INTO dbo.vehiculos
            (
                placa,
                marca,
                modelo,
                anio,
                color
            )
            VALUES
            (
                @placa,
                @marca,
                @modelo,
                @anio,
                @color
            );
            """;

        using SqlConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using SqlCommand comando =
            new(sql, conexion);

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
            UPDATE dbo.vehiculos
            SET
                placa = @placa,
                marca = @marca,
                modelo = @modelo,
                anio = @anio,
                color = @color
            WHERE placa = @placaOriginal;
            """;

        using SqlConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using SqlCommand comando =
            new(sql, conexion);

        AgregarParametrosVehiculo(
            comando,
            vehiculo);

        comando.Parameters.Add(
            "@placaOriginal",
            SqlDbType.VarChar,
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
            DELETE FROM dbo.vehiculos
            WHERE placa = @placa;
            """;

        using SqlConnection conexion =
            CrearConexion();

        await conexion.OpenAsync(cancellationToken);

        using SqlCommand comando =
            new(sql, conexion);

        comando.Parameters.Add(
            "@placa",
            SqlDbType.VarChar,
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

    private SqlConnection CrearConexion()
    {
        return new SqlConnection(
            cadenaConexion);
    }

    private static void AgregarParametrosVehiculo(
        SqlCommand comando,
        Vehiculo vehiculo)
    {
        comando.Parameters.Add(
            "@placa",
            SqlDbType.VarChar,
            15).Value =
                NormalizarPlaca(vehiculo.Placa);

        comando.Parameters.Add(
            "@marca",
            SqlDbType.VarChar,
            50).Value =
                vehiculo.Marca.Trim();

        comando.Parameters.Add(
            "@modelo",
            SqlDbType.VarChar,
            50).Value =
                vehiculo.Modelo.Trim();

        comando.Parameters.Add(
            "@anio",
            SqlDbType.Int).Value =
                vehiculo.Anio;

        comando.Parameters.Add(
            "@color",
            SqlDbType.VarChar,
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