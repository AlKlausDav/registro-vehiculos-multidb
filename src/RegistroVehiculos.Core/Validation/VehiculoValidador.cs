using System.Text.RegularExpressions;
using RegistroVehiculos.Core.Models;

namespace RegistroVehiculos.Core.Validation;

public static class VehiculoValidador
{
    public static IReadOnlyList<string> Validar(Vehiculo? vehiculo)
    {
        var errores = new List<string>();

        if (vehiculo is null)
        {
            errores.Add("No se proporcionó información del vehículo.");
            return errores;
        }

        ValidarPlaca(vehiculo.Placa, errores);
        ValidarTexto(vehiculo.Marca, "La marca", 50, errores);
        ValidarTexto(vehiculo.Modelo, "El modelo", 50, errores);
        ValidarAnio(vehiculo.Anio, errores);
        ValidarTexto(vehiculo.Color, "El color", 30, errores);

        return errores;
    }

    private static void ValidarPlaca(string placa, List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            errores.Add("La placa es obligatoria.");
            return;
        }

        string placaLimpia = placa.Trim();

        if (placaLimpia.Length is < 3 or > 15)
        {
            errores.Add("La placa debe contener entre 3 y 15 caracteres.");
        }

        if (!Regex.IsMatch(placaLimpia, @"^[a-zA-Z0-9-]+$"))
        {
            errores.Add(
                "La placa solamente puede contener letras, números y guiones."
            );
        }
    }

    private static void ValidarTexto(
        string valor,
        string nombreCampo,
        int longitudMaxima,
        List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            errores.Add($"{nombreCampo} es obligatorio.");
            return;
        }

        if (valor.Trim().Length > longitudMaxima)
        {
            errores.Add(
                $"{nombreCampo} no puede superar {longitudMaxima} caracteres."
            );
        }
    }

    private static void ValidarAnio(int anio, List<string> errores)
    {
        int anioMaximo = DateTime.Now.Year + 1;

        if (anio < 1900 || anio > anioMaximo)
        {
            errores.Add(
                $"El año debe estar entre 1900 y {anioMaximo}."
            );
        }
    }
}