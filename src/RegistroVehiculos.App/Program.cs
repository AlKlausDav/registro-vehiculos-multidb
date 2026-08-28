using RegistroVehiculos.Infrastructure.Configuration;
using RegistroVehiculos.Infrastructure.Factories;

namespace RegistroVehiculos.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            ConfiguracionAplicacion configuracion =
                CargadorConfiguracion.Cargar();

            var fabrica =
                new VehiculoRepositoryFactory(configuracion);

            Application.Run(new Form1(fabrica));
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No fue posible iniciar la aplicación."
                + Environment.NewLine
                + Environment.NewLine
                + ex.Message,
                "Error de configuración",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}