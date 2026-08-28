using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using RegistroVehiculos.Core.Enums;
using RegistroVehiculos.Core.Interfaces;
using RegistroVehiculos.Core.Models;
using RegistroVehiculos.Core.Validation;
using RegistroVehiculos.Infrastructure.Factories;

namespace RegistroVehiculos.App;

public partial class Form1 : Form
{
    private static readonly Color ColorFondo =
        Color.FromArgb(238, 242, 247);

    private static readonly Color ColorPrincipal =
        Color.FromArgb(30, 41, 59);

    private static readonly Color ColorMorado =
        Color.FromArgb(109, 76, 233);

    private static readonly Color ColorVerde =
        Color.FromArgb(16, 185, 129);

    private static readonly Color ColorAzul =
        Color.FromArgb(37, 99, 235);

    private static readonly Color ColorRojo =
        Color.FromArgb(225, 55, 75);

    private static readonly Color ColorGris =
        Color.FromArgb(100, 116, 139);

    private readonly VehiculoRepositoryFactory fabrica;

    private IVehiculoRepository? repositorioActual;
    private Vehiculo? vehiculoSeleccionado;
    private string? placaOriginal;
    private bool operacionEnCurso;

    private readonly BindingList<Vehiculo> vehiculos = new();

    private readonly TabControl tabMotores = new();

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

    public Form1(VehiculoRepositoryFactory fabrica)
    {
        InitializeComponent();

        this.fabrica = fabrica
            ?? throw new ArgumentNullException(nameof(fabrica));

        btnConectar = CrearBoton(
            "Recargar",
            ColorMorado,
            150);

        btnNuevo = CrearBoton(
            "Nuevo",
            ColorGris);

        btnGuardar = CrearBoton(
            "Guardar",
            ColorVerde);

        btnModificar = CrearBoton(
            "Modificar",
            ColorAzul);

        btnEliminar = CrearBoton(
            "Eliminar",
            ColorRojo);

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
        MinimumSize = new Size(950, 650);
        Size = new Size(1100, 780);
        BackColor = ColorFondo;
        Font = new Font("Segoe UI", 10);
    }

    private void ConstruirInterfaz()
    {
        Controls.Clear();

        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            ColumnCount = 1,
            RowCount = 6,
            BackColor = ColorFondo
        };

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 100));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 78));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 175));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 72));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55));

        Controls.Add(contenedor);

        Panel encabezado = CrearTarjeta(
            ColorPrincipal,
            18,
            new Padding(10));

        var titulo = new Label
        {
            Text = "Administración de vehículos",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                23,
                FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter
        };

        encabezado.Controls.Add(titulo);
        contenedor.Controls.Add(encabezado, 0, 0);

        Panel tarjetaMotores = CrearTarjeta(
            Color.White,
            16,
            new Padding(10, 8, 10, 8));

        var panelMotores = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.White
        };

        panelMotores.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        panelMotores.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 180));

        ConfigurarPestanas();

        panelMotores.Controls.Add(
            tabMotores,
            0,
            0);

        btnConectar.Dock = DockStyle.None;
        btnConectar.Anchor = AnchorStyles.None;
        btnConectar.AutoSize = false;
        btnConectar.Size = new Size(150, 42);
        btnConectar.MinimumSize = new Size(150, 42);
        btnConectar.MaximumSize = new Size(150, 42);
        btnConectar.Margin = new Padding(8);
        btnConectar.Padding = new Padding(0);
        btnConectar.TextAlign =
            ContentAlignment.MiddleCenter;

        panelMotores.Controls.Add(
            btnConectar,
            1,
            0);

        tarjetaMotores.Controls.Add(panelMotores);
        contenedor.Controls.Add(tarjetaMotores, 0, 1);

        Panel tarjetaFormulario = CrearTarjeta(
            Color.White,
            16,
            new Padding(20));

        var formulario = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3,
            BackColor = Color.White
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
        nudAnio.Margin = new Padding(7);
        nudAnio.Font = new Font("Segoe UI", 10);
        nudAnio.BackColor =
            Color.FromArgb(248, 250, 252);
        nudAnio.BorderStyle =
            BorderStyle.FixedSingle;

        AgregarCampo(
            formulario,
            "Placa",
            txtPlaca,
            0,
            0);

        AgregarCampo(
            formulario,
            "Marca",
            txtMarca,
            0,
            2);

        AgregarCampo(
            formulario,
            "Modelo",
            txtModelo,
            1,
            0);

        AgregarCampo(
            formulario,
            "Año",
            nudAnio,
            1,
            2);

        AgregarCampo(
            formulario,
            "Color",
            txtColor,
            2,
            0);

        tarjetaFormulario.Controls.Add(formulario);
        contenedor.Controls.Add(
            tarjetaFormulario,
            0,
            2);

        var centradorBotones = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = ColorFondo
        };

        centradorBotones.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        centradorBotones.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        centradorBotones.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        var panelBotones = new FlowLayoutPanel
        {
            AutoSize = true,
            Anchor = AnchorStyles.None,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = ColorFondo,
            Padding = new Padding(0, 10, 0, 10),
            Margin = new Padding(0)
        };

        panelBotones.Controls.Add(btnNuevo);
        panelBotones.Controls.Add(btnGuardar);
        panelBotones.Controls.Add(btnModificar);
        panelBotones.Controls.Add(btnEliminar);

        centradorBotones.Controls.Add(
            panelBotones,
            1,
            0);

        contenedor.Controls.Add(
            centradorBotones,
            0,
            3);

        Panel tarjetaTabla = CrearTarjeta(
            Color.White,
            16,
            new Padding(12));

        var contenidoTabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.White
        };

        contenidoTabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 42));

        contenidoTabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var tituloTabla = new Label
        {
            Text = "Vehículos registrados",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                11,
                FontStyle.Bold),
            ForeColor = ColorPrincipal,
            BackColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0)
        };

        contenidoTabla.Controls.Add(
            tituloTabla,
            0,
            0);

        contenidoTabla.Controls.Add(
            dgvVehiculos,
            0,
            1);

        tarjetaTabla.Controls.Add(contenidoTabla);
        contenedor.Controls.Add(
            tarjetaTabla,
            0,
            4);

        Panel tarjetaEstado = CrearTarjeta(
            Color.White,
            14,
            new Padding(16, 7, 16, 7));

        lblEstado.Dock = DockStyle.Fill;
        lblEstado.TextAlign =
            ContentAlignment.MiddleLeft;
        lblEstado.ForeColor =
            Color.FromArgb(71, 85, 105);
        lblEstado.Font = new Font(
            "Segoe UI",
            9,
            FontStyle.Regular);

        tarjetaEstado.Controls.Add(lblEstado);
        contenedor.Controls.Add(
            tarjetaEstado,
            0,
            5);
    }

    private void ConfigurarPestanas()
    {
        tabMotores.Dock = DockStyle.Fill;
        tabMotores.Alignment = TabAlignment.Top;
        tabMotores.Appearance =
            TabAppearance.FlatButtons;
        tabMotores.DrawMode =
            TabDrawMode.OwnerDrawFixed;
        tabMotores.SizeMode =
            TabSizeMode.Fixed;
        tabMotores.ItemSize =
            new Size(175, 42);

        tabMotores.Font = new Font(
            "Segoe UI",
            10,
            FontStyle.Bold);

        tabMotores.TabPages.Clear();

        tabMotores.TabPages.Add(
            CrearPestanaMotor(
                "MySQL",
                MotorBaseDatos.MySql));

        tabMotores.TabPages.Add(
            CrearPestanaMotor(
                "SQL Server",
                MotorBaseDatos.SqlServer));

        tabMotores.TabPages.Add(
            CrearPestanaMotor(
                "Oracle",
                MotorBaseDatos.Oracle));

        tabMotores.SelectedIndex = 0;
    }

    private static TabPage CrearPestanaMotor(
        string texto,
        MotorBaseDatos motor)
    {
        return new TabPage
        {
            Text = texto,
            Tag = motor,
            BackColor = Color.White
        };
    }

    private void ConfigurarTabla()
    {
        dgvVehiculos.Dock = DockStyle.Fill;
        dgvVehiculos.BackgroundColor = Color.White;
        dgvVehiculos.BorderStyle = BorderStyle.None;

        dgvVehiculos.AllowUserToAddRows = false;
        dgvVehiculos.AllowUserToDeleteRows = false;
        dgvVehiculos.AllowUserToResizeRows = false;

        dgvVehiculos.ReadOnly = true;
        dgvVehiculos.MultiSelect = false;
        dgvVehiculos.RowHeadersVisible = false;

        dgvVehiculos.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;

        dgvVehiculos.AutoGenerateColumns = false;

        dgvVehiculos.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        dgvVehiculos.EnableHeadersVisualStyles = false;

        dgvVehiculos.ColumnHeadersHeight = 42;

        dgvVehiculos.ColumnHeadersBorderStyle =
            DataGridViewHeaderBorderStyle.None;

        dgvVehiculos.ColumnHeadersDefaultCellStyle =
            new DataGridViewCellStyle
            {
                BackColor = ColorPrincipal,
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),
                Alignment =
                    DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 8, 0)
            };

        dgvVehiculos.DefaultCellStyle =
            new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor =
                    Color.FromArgb(51, 65, 85),
                SelectionBackColor =
                    Color.FromArgb(224, 231, 255),
                SelectionForeColor = ColorPrincipal,
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(8, 0, 8, 0)
            };

        dgvVehiculos.AlternatingRowsDefaultCellStyle =
            new DataGridViewCellStyle
            {
                BackColor =
                    Color.FromArgb(248, 250, 252)
            };

        dgvVehiculos.RowTemplate.Height = 38;

        dgvVehiculos.CellBorderStyle =
            DataGridViewCellBorderStyle.SingleHorizontal;

        dgvVehiculos.GridColor =
            Color.FromArgb(226, 232, 240);

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
        Shown += CargarMotorInicialAsync;

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

        tabMotores.SelectedIndexChanged +=
            CambiarMotorAsync;

        tabMotores.DrawItem +=
            DibujarPestanaMotor;

        dgvVehiculos.SelectionChanged +=
            CargarVehiculoSeleccionado;
    }

    private void DibujarPestanaMotor(
        object? sender,
        DrawItemEventArgs e)
    {
        Graphics grafico = e.Graphics;

        grafico.SmoothingMode =
            SmoothingMode.AntiAlias;

        Rectangle area =
            tabMotores.GetTabRect(e.Index);

        area.Inflate(-4, -3);

        bool seleccionada =
            e.Index == tabMotores.SelectedIndex;

        Color fondo = seleccionada
            ? ColorMorado
            : Color.FromArgb(241, 245, 249);

        Color texto = seleccionada
            ? Color.White
            : Color.FromArgb(71, 85, 105);

        using GraphicsPath ruta =
            CrearRutaRedondeada(area, 10);

        using var brocha =
            new SolidBrush(fondo);

        grafico.FillPath(brocha, ruta);

        TextRenderer.DrawText(
            grafico,
            tabMotores.TabPages[e.Index].Text,
            tabMotores.Font,
            area,
            texto,
            TextFormatFlags.HorizontalCenter
            | TextFormatFlags.VerticalCenter
            | TextFormatFlags.EndEllipsis);
    }

    private async void CargarMotorInicialAsync(
        object? sender,
        EventArgs e)
    {
        await ConectarMotorSeleccionadoAsync();
    }

    private async void ConectarBaseDatosAsync(
        object? sender,
        EventArgs e)
    {
        await ConectarMotorSeleccionadoAsync();
    }

    private async void CambiarMotorAsync(
        object? sender,
        EventArgs e)
    {
        repositorioActual = null;
        vehiculos.Clear();

        LimpiarFormulario();

        MotorBaseDatos? motor =
            ObtenerMotorSeleccionado();

        lblEstado.Text = motor is null
            ? "Selecciona una pestaña."
            : $"Cambiando a "
              + $"{ObtenerNombreMotor(motor.Value)}...";

        await ConectarMotorSeleccionadoAsync();
    }

    private async Task ConectarMotorSeleccionadoAsync()
    {
        MotorBaseDatos? motorSeleccionado =
            ObtenerMotorSeleccionado();

        if (motorSeleccionado is null)
        {
            MostrarMensaje(
                "Selecciona una pestaña de base de datos.",
                "Motor no seleccionado",
                MessageBoxIcon.Information);

            return;
        }

        MotorBaseDatos motor =
            motorSeleccionado.Value;

        string nombreMotor =
            ObtenerNombreMotor(motor);

        try
        {
            EstablecerOperacionEnCurso(
                true,
                $"Conectando con {nombreMotor}...");

            IVehiculoRepository repositorio =
                fabrica.Crear(motor);

            bool conexionCorrecta =
                await repositorio
                    .ProbarConexionAsync();

            if (!conexionCorrecta)
            {
                repositorioActual = null;
                vehiculos.Clear();

                MostrarMensaje(
                    $"No fue posible conectar con "
                    + $"{nombreMotor}."
                    + Environment.NewLine
                    + "Revisa appsettings.local.json, "
                    + "el servidor y la tabla.",
                    "Error de conexión",
                    MessageBoxIcon.Error);

                lblEstado.Text =
                    $"Sin conexión con {nombreMotor}.";

                return;
            }

            repositorioActual = repositorio;

            await CargarVehiculosAsync();

            LimpiarFormulario();

            lblEstado.Text =
                $"Conectado correctamente con "
                + $"{nombreMotor}.";
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

    private MotorBaseDatos? ObtenerMotorSeleccionado()
    {
        if (tabMotores.SelectedTab?.Tag
            is MotorBaseDatos motor)
        {
            return motor;
        }

        return null;
    }

    private static string ObtenerNombreMotor(
        MotorBaseDatos motor)
    {
        return motor switch
        {
            MotorBaseDatos.MySql => "MySQL",
            MotorBaseDatos.SqlServer => "SQL Server",
            MotorBaseDatos.Oracle => "Oracle",
            _ => motor.ToString()
        };
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
                $"Vehículo "
                + $"{datosActualizados.Placa} modificado.";
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
            await repositorioActual
                .ObtenerTodosAsync();

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
                decimal.ToInt32(
                    nudAnio.Value),

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
            "Selecciona una pestaña de base de datos "
            + "y espera que termine la conexión.",
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

        MotorBaseDatos? motor =
            ObtenerMotorSeleccionado();

        lblEstado.Text =
            repositorioActual is null
                ? motor is null
                    ? "Selecciona una pestaña."
                    : $"Preparando "
                      + $"{ObtenerNombreMotor(motor.Value)}."
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

        tabMotores.Enabled = !operacionEnCurso;
        btnConectar.Enabled = !operacionEnCurso;

        btnNuevo.Enabled =
            !operacionEnCurso && conectado;

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

    private static Panel CrearTarjeta(
        Color color,
        int radio,
        Padding relleno)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = color,
            Padding = relleno,
            Margin = new Padding(0, 0, 0, 10)
        };

        AplicarBordesRedondeados(
            panel,
            radio);

        return panel;
    }

    private static void ConfigurarCampo(
        TextBox campo)
    {
        campo.Dock = DockStyle.Fill;
        campo.Margin = new Padding(7);
        campo.Font = new Font("Segoe UI", 10);
        campo.BackColor =
            Color.FromArgb(248, 250, 252);
        campo.BorderStyle =
            BorderStyle.FixedSingle;
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
            TextAlign =
                ContentAlignment.MiddleLeft,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold),
            ForeColor =
                Color.FromArgb(51, 65, 85)
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
            Height = 42,
            AutoSize = false,
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold),
            TextAlign =
                ContentAlignment.MiddleCenter,
            Margin = new Padding(5),
            Padding = new Padding(0),
            UseVisualStyleBackColor = false
        };

        boton.FlatAppearance.BorderSize = 0;

        boton.FlatAppearance.MouseDownBackColor =
            OscurecerColor(color, 0.75);

        boton.MouseEnter += (_, _) =>
        {
            if (boton.Enabled)
            {
                boton.BackColor =
                    OscurecerColor(
                        color,
                        0.88);
            }
        };

        boton.MouseLeave += (_, _) =>
        {
            boton.BackColor = color;
        };

        AplicarBordesRedondeados(
            boton,
            12);

        return boton;
    }

    private static Color OscurecerColor(
        Color color,
        double factor)
    {
        return Color.FromArgb(
            color.A,
            Math.Clamp(
                (int)(color.R * factor),
                0,
                255),
            Math.Clamp(
                (int)(color.G * factor),
                0,
                255),
            Math.Clamp(
                (int)(color.B * factor),
                0,
                255));
    }

    private static void AplicarBordesRedondeados(
        Control control,
        int radio)
    {
        control.Resize += (_, _) =>
            ActualizarRegionRedondeada(
                control,
                radio);

        ActualizarRegionRedondeada(
            control,
            radio);
    }

    private static void ActualizarRegionRedondeada(
        Control control,
        int radio)
    {
        if (control.Width <= 1
            || control.Height <= 1)
        {
            return;
        }

        var area = new Rectangle(
            0,
            0,
            control.Width,
            control.Height);

        using GraphicsPath ruta =
            CrearRutaRedondeada(
                area,
                radio);

        Region? regionAnterior =
            control.Region;

        control.Region =
            new Region(ruta);

        regionAnterior?.Dispose();
    }

    private static GraphicsPath CrearRutaRedondeada(
        Rectangle area,
        int radio)
    {
        var ruta = new GraphicsPath();

        int diametro = Math.Min(
            radio * 2,
            Math.Min(
                area.Width,
                area.Height));

        if (diametro <= 1)
        {
            ruta.AddRectangle(area);
            return ruta;
        }

        var arco = new Rectangle(
            area.X,
            area.Y,
            diametro,
            diametro);

        ruta.AddArc(
            arco,
            180,
            90);

        arco.X =
            area.Right - diametro;

        ruta.AddArc(
            arco,
            270,
            90);

        arco.Y =
            area.Bottom - diametro;

        ruta.AddArc(
            arco,
            0,
            90);

        arco.X = area.Left;

        ruta.AddArc(
            arco,
            90,
            90);

        ruta.CloseFigure();

        return ruta;
    }
}