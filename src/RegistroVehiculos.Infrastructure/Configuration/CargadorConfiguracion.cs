using System.Text.Json;

namespace RegistroVehiculos.Infrastructure.Configuration;

public static class CargadorConfiguracion
{
    private const string NombreArchivo =
        "appsettings.local.json";

    public static ConfiguracionAplicacion Cargar(
        string? rutaArchivo = null)
    {
        string ruta = rutaArchivo ?? BuscarArchivo();

        if (!File.Exists(ruta))
        {
            throw new FileNotFoundException(
                $"No se encontró el archivo {NombreArchivo}. " +
                "Copia appsettings.example.json y cambia su " +
                "nombre a appsettings.local.json.",
                ruta);
        }

        string contenido = File.ReadAllText(ruta);

        var opciones = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        ConfiguracionAplicacion? configuracion =
            JsonSerializer.Deserialize<ConfiguracionAplicacion>(
                contenido,
                opciones);

        if (configuracion is null)
        {
            throw new InvalidOperationException(
                "No fue posible leer la configuración.");
        }

        Validar(configuracion);

        return configuracion;
    }

    private static string BuscarArchivo()
    {
        string rutaDirectorioActual =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                NombreArchivo);

        if (File.Exists(rutaDirectorioActual))
        {
            return rutaDirectorioActual;
        }

        return Path.Combine(
            AppContext.BaseDirectory,
            NombreArchivo);
    }

    private static void Validar(
        ConfiguracionAplicacion configuracion)
    {
        if (string.IsNullOrWhiteSpace(
            configuracion.ConnectionStrings.MySql))
        {
            throw new InvalidOperationException(
                "Falta la conexión de MySQL.");
        }

        if (string.IsNullOrWhiteSpace(
            configuracion.ConnectionStrings.SqlServer))
        {
            throw new InvalidOperationException(
                "Falta la conexión de SQL Server.");
        }

        if (string.IsNullOrWhiteSpace(
            configuracion.ConnectionStrings.Oracle))
        {
            throw new InvalidOperationException(
                "Falta la conexión de Oracle.");
        }
    }
}