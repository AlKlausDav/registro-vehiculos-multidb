using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RegistroVehiculos.Core.Enums;
using RegistroVehiculos.Core.Interfaces;
using RegistroVehiculos.Core.Models;
using RegistroVehiculos.Core.Validation;
using RegistroVehiculos.Infrastructure.Factories;

namespace RegistroVehiculos.App;

public partial class Form1 : Form
{
    private readonly VehiculoRepositoryFactory fabrica;

    private IVehiculoRepository? repositorioActual;
    private Vehiculo? vehiculoSeleccionado;
    private string? placaOriginal;
    private bool operacionEnCurso;

    private readonly BindingList<Vehiculo> vehiculos = new();

    private readonly ComboBox cboMotor = new();

    private readonly TextBox txtPlaca = new();
    private readonly TextBox txtMarca = new();
    private readonly TextBox txtModelo = new();
    private readonly NumericUpDown nudAnio = new();
    private readonly TextBox txtColor = new();

    private readonly Button btnConectar;
    private readonly Button btnNuevo;
    private readonly Button btnGuardar;
    private readonly Button btnModificar;
    private readonly Button btnEliminar;

    private readonly DataGridView dgvVehiculos = new();
    private readonly Label lblEstado = new();

    public Form1(
        VehiculoRepositoryFactory fabrica)
    {
        InitializeComponent();

        this.fabrica = fabrica
            ?? throw new ArgumentNullException(
                nameof(fabrica));

        btnConectar = CrearBoton(
            "Conectar y cargar",
            Color.FromArgb(111, 66, 193),
            160);

        btnNuevo = CrearBoton(
            "Nuevo",
            Color.FromArgb(108, 117, 125));

        btnGuardar = CrearBoton(
            "Guardar",
            Color.FromArgb(25, 135, 84));

        btnModificar = CrearBoton(
            "Modificar",
            Color.FromArgb(13, 110, 253));

        btnEliminar = CrearBoton(
            "Eliminar",
            Color.FromArgb(220, 53, 69));

        ConfigurarVentana();
        ConstruirInterfaz();
        ConfigurarTabla();
        ConectarEventos();
        LimpiarFormulario();
    }

    private void ConfigurarVentana()
    {
        Text = "Registro de Vehículos";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 600);
        Size = new Size(1050, 700);
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new Font("Segoe UI", 10);
    }

    private void ConstruirInterfaz()
    {
        Controls.Clear();

        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            ColumnCount = 1,
            RowCount = 6
        };

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 150));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 35));

        Controls.Add(contenedor);

        var titulo = new Label
        {
            Text = "Administración de vehículos",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                20,
                FontStyle.Bold),
            ForeColor = Color.FromArgb(31, 41, 55),
            TextAlign = ContentAlignment.MiddleLeft
        };

        contenedor.Controls.Add(titulo, 0, 0);

        var panelMotor = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };

        var etiquetaMotor = new Label
        {
            Text = "Motor de base de datos:",
            AutoSize = true,
            Margin = new Padding(0, 7, 12, 0)
        };

        cboMotor.DropDownStyle =
            ComboBoxStyle.DropDownList;

        cboMotor.Width = 190;

        cboMotor.DataSource =
            Enum.GetValues<MotorBaseDatos>();

        panelMotor.Controls.Add(etiquetaMotor);
        panelMotor.Controls.Add(cboMotor);
        panelMotor.Controls.Add(btnConectar);

        contenedor.Controls.Add(panelMotor, 0, 1);

        var formulario = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3,
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        formulario.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 90));

        formulario.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        formulario.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 90));

        formulario.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        formulario.RowStyles.Add(
            new RowStyle(SizeType.Percent, 33));

        formulario.RowStyles.Add(
            new RowStyle(SizeType.Percent, 33));

        formulario.RowStyles.Add(
            new RowStyle(SizeType.Percent, 34));

        ConfigurarCampo(txtPlaca);
        ConfigurarCampo(txtMarca);
        ConfigurarCampo(txtModelo);
        ConfigurarCampo(txtColor);

        nudAnio.Dock = DockStyle.Fill;
        nudAnio.Minimum = 1900;
        nudAnio.Maximum = DateTime.Now.Year + 1;
        nudAnio.Value = DateTime.Now.Year;
        nudAnio.Margin = new Padding(5);

        AgregarCampo(
            formulario,
            "Placa:",
            txtPlaca,
            0,
            0);

        AgregarCampo(
            formulario,
            "Marca:",
            txtMarca,
            0,
            2);

        AgregarCampo(
            formulario,
            "Modelo:",
            txtModelo,
            1,
            0);

        AgregarCampo(
            formulario,
            "Año:",
            nudAnio,
            1,
            2);

        AgregarCampo(
            formulario,
            "Color:",
            txtColor,
            2,
            0);

        contenedor.Controls.Add(formulario, 0, 2);

        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };

        panelBotones.Controls.Add(btnNuevo);
        panelBotones.Controls.Add(btnGuardar);
        panelBotones.Controls.Add(btnModificar);
        panelBotones.Controls.Add(btnEliminar);

        contenedor.Controls.Add(panelBotones, 0, 3);
        contenedor.Controls.Add(dgvVehiculos, 0, 4);

        lblEstado.Dock = DockStyle.Fill;
        lblEstado.TextAlign =
            ContentAlignment.MiddleLeft;
        lblEstado.ForeColor =
            Color.FromArgb(75, 85, 99);

        contenedor.Controls.Add(lblEstado, 0, 5);
    }

    private void ConfigurarTabla()
    {
        dgvVehiculos.Dock = DockStyle.Fill;
        dgvVehiculos.BackgroundColor = Color.White;
        dgvVehiculos.BorderStyle =
            BorderStyle.FixedSingle;

        dgvVehiculos.AllowUserToAddRows = false;
        dgvVehiculos.AllowUserToDeleteRows = false;
        dgvVehiculos.AllowUserToResizeRows = false;

        dgvVehiculos.ReadOnly = true;
        dgvVehiculos.MultiSelect = false;

        dgvVehiculos.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;

        dgvVehiculos.AutoGenerateColumns = false;
        dgvVehiculos.RowHeadersVisible = false;

        dgvVehiculos.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        AgregarColumna(
            "Placa",
            nameof(Vehiculo.Placa));

        AgregarColumna(
            "Marca",
            nameof(Vehiculo.Marca));

        AgregarColumna(
            "Modelo",
            nameof(Vehiculo.Modelo));

        AgregarColumna(
            "Año",
            nameof(Vehiculo.Anio));

        AgregarColumna(
            "Color",
            nameof(Vehiculo.Color));

        dgvVehiculos.DataSource = vehiculos;
    }

    private void AgregarColumna(
        string encabezado,
        string propiedad)
    {
        var columna = new DataGridViewTextBoxColumn
        {
            HeaderText = encabezado,
            DataPropertyName = propiedad
        };

        dgvVehiculos.Columns.Add(columna);
    }

    private void ConectarEventos()
    {
        btnConectar.Click +=
            ConectarBaseDatosAsync;

        btnNuevo.Click +=
            NuevoVehiculo;

        btnGuardar.Click +=
            GuardarVehiculoAsync;

        btnModificar.Click +=
            ModificarVehiculoAsync;

        btnEliminar.Click +=
            EliminarVehiculoAsync;

        cboMotor.SelectionChangeCommitted +=
            CambiarMotor;

        dgvVehiculos.SelectionChanged +=
            CargarVehiculoSeleccionado;
    }

    private async void ConectarBaseDatosAsync(
        object? sender,
        EventArgs e)
    {
        if (cboMotor.SelectedItem
            is not MotorBaseDatos motor)
        {
            MostrarMensaje(
                "Selecciona un motor de base de datos.",
                "Motor no seleccionado",
                MessageBoxIcon.Information);

            return;
        }

        try
        {
            EstablecerOperacionEnCurso(
                true,
                $"Conectando con {motor}...");

            IVehiculoRepository repositorio =
                fabrica.Crear(motor);

            bool conexionCorrecta =
                await repositorio.ProbarConexionAsync();

            if (!conexionCorrecta)
            {
                repositorioActual = null;
                vehiculos.Clear();

                MostrarMensaje(
                    $"No fue posible conectar con {motor}."
                    + Environment.NewLine
                    + "Revisa appsettings.local.json, "
                    + "el servidor y la tabla.",
                    "Error de conexión",
                    MessageBoxIcon.Error);

                lblEstado.Text =
                    $"Sin conexión con {motor}.";

                return;
            }

            repositorioActual = repositorio;

            await CargarVehiculosAsync();

            LimpiarFormulario();

            lblEstado.Text =
                $"Conectado correctamente con {motor}.";
        }
        catch (Exception ex)
        {
            repositorioActual = null;
            vehiculos.Clear();

            MostrarError(ex);
        }
        finally
        {
            EstablecerOperacionEnCurso(false);
        }
    }

    private void CambiarMotor(
        object? sender,
        EventArgs e)
    {
        repositorioActual = null;
        vehiculos.Clear();

        LimpiarFormulario();

        lblEstado.Text =
            "Motor cambiado. Presiona Conectar y cargar.";
    }

    private void NuevoVehiculo(
        object? sender,
        EventArgs e)
    {
        LimpiarFormulario();
    }

    private async void GuardarVehiculoAsync(
        object? sender,
        EventArgs e)
    {
        if (!ComprobarConexion())
        {
            return;
        }

        Vehiculo nuevoVehiculo =
            ObtenerVehiculoFormulario();

        if (!ValidarVehiculo(nuevoVehiculo))
        {
            return;
        }

        try
        {
            EstablecerOperacionEnCurso(
                true,
                "Guardando vehículo...");

            bool placaDuplicada =
                await repositorioActual!
                    .ExistePlacaAsync(
                        nuevoVehiculo.Placa);

            if (placaDuplicada)
            {
                MostrarMensaje(
                    "Ya existe un vehículo con esa placa.",
                    "Placa duplicada",
                    MessageBoxIcon.Warning);

                return;
            }

            await repositorioActual!
                .AgregarAsync(nuevoVehiculo);

            await CargarVehiculosAsync();

            LimpiarFormulario();

            lblEstado.Text =
                $"Vehículo {nuevoVehiculo.Placa} guardado.";
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
        finally
        {
            EstablecerOperacionEnCurso(false);
        }
    }

    private async void ModificarVehiculoAsync(
        object? sender,
        EventArgs e)
    {
        if (!ComprobarConexion())
        {
            return;
        }

        if (vehiculoSeleccionado is null
            || string.IsNullOrWhiteSpace(placaOriginal))
        {
            MostrarMensaje(
                "Selecciona un vehículo de la tabla.",
                "Vehículo no seleccionado",
                MessageBoxIcon.Information);

            return;
        }

        Vehiculo datosActualizados =
            ObtenerVehiculoFormulario();

        if (!ValidarVehiculo(datosActualizados))
        {
            return;
        }

        try
        {
            EstablecerOperacionEnCurso(
                true,
                "Modificando vehículo...");

            bool placaCambio =
                !string.Equals(
                    placaOriginal,
                    datosActualizados.Placa,
                    StringComparison.OrdinalIgnoreCase);

            if (placaCambio)
            {
                bool placaDuplicada =
                    await repositorioActual!
                        .ExistePlacaAsync(
                            datosActualizados.Placa);

                if (placaDuplicada)
                {
                    MostrarMensaje(
                        "Ya existe otro vehículo con esa placa.",
                        "Placa duplicada",
                        MessageBoxIcon.Warning);

                    return;
                }
            }

            await repositorioActual!
                .ActualizarAsync(
                    placaOriginal,
                    datosActualizados);

            await CargarVehiculosAsync();

            LimpiarFormulario();

            lblEstado.Text =
                $"Vehículo {datosActualizados.Placa} modificado.";
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
        finally
        {
            EstablecerOperacionEnCurso(false);
        }
    }

    private async void EliminarVehiculoAsync(
        object? sender,
        EventArgs e)
    {
        if (!ComprobarConexion())
        {
            return;
        }

        if (vehiculoSeleccionado is null)
        {
            MostrarMensaje(
                "Selecciona un vehículo de la tabla.",
                "Vehículo no seleccionado",
                MessageBoxIcon.Information);

            return;
        }

        string placa =
            vehiculoSeleccionado.Placa;

        DialogResult respuesta =
            MessageBox.Show(
                $"¿Deseas eliminar el vehículo {placa}?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

        if (respuesta != DialogResult.Yes)
        {
            return;
        }

        try
        {
            EstablecerOperacionEnCurso(
                true,
                "Eliminando vehículo...");

            await repositorioActual!
                .EliminarAsync(placa);

            await CargarVehiculosAsync();

            LimpiarFormulario();

            lblEstado.Text =
                $"Vehículo {placa} eliminado.";
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
        finally
        {
            EstablecerOperacionEnCurso(false);
        }
    }

    private async Task CargarVehiculosAsync()
    {
        if (repositorioActual is null)
        {
            return;
        }

        IReadOnlyList<Vehiculo> resultados =
            await repositorioActual.ObtenerTodosAsync();

        vehiculos.Clear();

        foreach (Vehiculo vehiculo in resultados)
        {
            vehiculos.Add(vehiculo);
        }

        dgvVehiculos.ClearSelection();
    }

    private Vehiculo ObtenerVehiculoFormulario()
    {
        return new Vehiculo
        {
            Placa =
                txtPlaca.Text
                    .Trim()
                    .ToUpperInvariant(),

            Marca = txtMarca.Text.Trim(),
            Modelo = txtModelo.Text.Trim(),

            Anio =
                decimal.ToInt32(nudAnio.Value),

            Color = txtColor.Text.Trim()
        };
    }

    private bool ValidarVehiculo(
        Vehiculo vehiculo)
    {
        IReadOnlyList<string> errores =
            VehiculoValidador.Validar(vehiculo);

        if (errores.Count == 0)
        {
            return true;
        }

        MostrarMensaje(
            string.Join(
                Environment.NewLine,
                errores),
            "Información incorrecta",
            MessageBoxIcon.Warning);

        return false;
    }

    private bool ComprobarConexion()
    {
        if (repositorioActual is not null)
        {
            return true;
        }

        MostrarMensaje(
            "Selecciona un motor y presiona "
            + "Conectar y cargar.",
            "Base de datos no conectada",
            MessageBoxIcon.Information);

        return false;
    }

    private void CargarVehiculoSeleccionado(
        object? sender,
        EventArgs e)
    {
        if (dgvVehiculos.SelectedRows.Count == 0)
        {
            return;
        }

        object? elemento =
            dgvVehiculos
                .SelectedRows[0]
                .DataBoundItem;

        if (elemento is not Vehiculo seleccionado)
        {
            return;
        }

        vehiculoSeleccionado = seleccionado;
        placaOriginal = seleccionado.Placa;

        txtPlaca.Text = seleccionado.Placa;
        txtMarca.Text = seleccionado.Marca;
        txtModelo.Text = seleccionado.Modelo;
        nudAnio.Value = seleccionado.Anio;
        txtColor.Text = seleccionado.Color;

        lblEstado.Text =
            $"Vehículo {seleccionado.Placa} seleccionado.";

        ActualizarEstadoBotones();
    }

    private void LimpiarFormulario()
    {
        txtPlaca.Clear();
        txtMarca.Clear();
        txtModelo.Clear();
        txtColor.Clear();

        nudAnio.Value = DateTime.Now.Year;

        vehiculoSeleccionado = null;
        placaOriginal = null;

        dgvVehiculos.ClearSelection();

        lblEstado.Text =
            repositorioActual is null
                ? "Selecciona un motor y conecta la base."
                : "Completa los datos del vehículo.";

        ActualizarEstadoBotones();

        txtPlaca.Focus();
    }

    private void EstablecerOperacionEnCurso(
        bool enCurso,
        string? mensaje = null)
    {
        operacionEnCurso = enCurso;
        UseWaitCursor = enCurso;

        if (!string.IsNullOrWhiteSpace(mensaje))
        {
            lblEstado.Text = mensaje;
        }

        ActualizarEstadoBotones();
    }

    private void ActualizarEstadoBotones()
    {
        bool conectado =
            repositorioActual is not null;

        bool seleccionado =
            vehiculoSeleccionado is not null;

        cboMotor.Enabled = !operacionEnCurso;
        btnConectar.Enabled = !operacionEnCurso;
        btnNuevo.Enabled = !operacionEnCurso;

        btnGuardar.Enabled =
            !operacionEnCurso && conectado;

        btnModificar.Enabled =
            !operacionEnCurso
            && conectado
            && seleccionado;

        btnEliminar.Enabled =
            !operacionEnCurso
            && conectado
            && seleccionado;
    }

    private void MostrarError(Exception ex)
    {
        lblEstado.Text =
            "Ocurrió un error durante la operación.";

        MostrarMensaje(
            ex.Message,
            "Error",
            MessageBoxIcon.Error);
    }

    private static void MostrarMensaje(
        string mensaje,
        string titulo,
        MessageBoxIcon icono)
    {
        MessageBox.Show(
            mensaje,
            titulo,
            MessageBoxButtons.OK,
            icono);
    }

    private static void ConfigurarCampo(
        TextBox campo)
    {
        campo.Dock = DockStyle.Fill;
        campo.Margin = new Padding(5);
    }

    private static void AgregarCampo(
        TableLayoutPanel panel,
        string etiqueta,
        Control campo,
        int fila,
        int columna)
    {
        var label = new Label
        {
            Text = etiqueta,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        panel.Controls.Add(
            label,
            columna,
            fila);

        panel.Controls.Add(
            campo,
            columna + 1,
            fila);
    }

    private static Button CrearBoton(
        string texto,
        Color color,
        int ancho = 125)
    {
        var boton = new Button
        {
            Text = texto,
            Width = ancho,
            Height = 38,
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 10, 0)
        };

        boton.FlatAppearance.BorderSize = 0;

        return boton;
    }
}