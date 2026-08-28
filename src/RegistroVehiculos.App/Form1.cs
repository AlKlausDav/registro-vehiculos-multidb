using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RegistroVehiculos.Core.Enums;
using RegistroVehiculos.Core.Models;
using RegistroVehiculos.Core.Validation;

namespace RegistroVehiculos.App;

public partial class Form1 : Form
{
    private readonly ComboBox cboMotor = new();

    private readonly TextBox txtPlaca = new();
    private readonly TextBox txtMarca = new();
    private readonly TextBox txtModelo = new();
    private readonly NumericUpDown nudAnio = new();
    private readonly TextBox txtColor = new();

    private readonly Button btnNuevo;
    private readonly Button btnGuardar;
    private readonly Button btnModificar;
    private readonly Button btnEliminar;

    private readonly DataGridView dgvVehiculos = new();
    private readonly Label lblEstado = new();

    private readonly BindingList<Vehiculo> vehiculos = new();

    private Vehiculo? vehiculoSeleccionado;

    public Form1()
    {
        InitializeComponent();

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

        cboMotor.Width = 220;

        cboMotor.DataSource =
            Enum.GetValues<MotorBaseDatos>();

        panelMotor.Controls.Add(etiquetaMotor);
        panelMotor.Controls.Add(cboMotor);

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
        btnNuevo.Click += NuevoVehiculo;
        btnGuardar.Click += GuardarVehiculo;
        btnModificar.Click += ModificarVehiculo;
        btnEliminar.Click += EliminarVehiculo;

        dgvVehiculos.SelectionChanged +=
            CargarVehiculoSeleccionado;
    }

    private void NuevoVehiculo(
        object? sender,
        EventArgs e)
    {
        LimpiarFormulario();
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

        MessageBox.Show(
            string.Join(
                Environment.NewLine,
                errores),
            "Información incorrecta",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);

        return false;
    }

    private void GuardarVehiculo(
        object? sender,
        EventArgs e)
    {
        Vehiculo nuevoVehiculo =
            ObtenerVehiculoFormulario();

        if (!ValidarVehiculo(nuevoVehiculo))
        {
            return;
        }

        bool placaDuplicada =
            vehiculos.Any(vehiculo =>
                string.Equals(
                    vehiculo.Placa,
                    nuevoVehiculo.Placa,
                    StringComparison.OrdinalIgnoreCase));

        if (placaDuplicada)
        {
            MessageBox.Show(
                "Ya existe un vehículo con esa placa.",
                "Placa duplicada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        vehiculos.Add(nuevoVehiculo);

        LimpiarFormulario();

        lblEstado.Text =
            $"Vehículo con placa " +
            $"{nuevoVehiculo.Placa} guardado.";
    }

    private void ModificarVehiculo(
        object? sender,
        EventArgs e)
    {
        if (vehiculoSeleccionado is null)
        {
            MessageBox.Show(
                "Selecciona un vehículo de la tabla.",
                "Vehículo no seleccionado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        Vehiculo datosActualizados =
            ObtenerVehiculoFormulario();

        if (!ValidarVehiculo(datosActualizados))
        {
            return;
        }

        bool placaDuplicada =
            vehiculos.Any(vehiculo =>
                !ReferenceEquals(
                    vehiculo,
                    vehiculoSeleccionado)
                &&
                string.Equals(
                    vehiculo.Placa,
                    datosActualizados.Placa,
                    StringComparison.OrdinalIgnoreCase));

        if (placaDuplicada)
        {
            MessageBox.Show(
                "Ya existe otro vehículo con esa placa.",
                "Placa duplicada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        vehiculoSeleccionado.Placa =
            datosActualizados.Placa;

        vehiculoSeleccionado.Marca =
            datosActualizados.Marca;

        vehiculoSeleccionado.Modelo =
            datosActualizados.Modelo;

        vehiculoSeleccionado.Anio =
            datosActualizados.Anio;

        vehiculoSeleccionado.Color =
            datosActualizados.Color;

        vehiculos.ResetBindings();

        LimpiarFormulario();

        lblEstado.Text =
            $"Vehículo con placa " +
            $"{datosActualizados.Placa} modificado.";
    }

    private void EliminarVehiculo(
        object? sender,
        EventArgs e)
    {
        if (vehiculoSeleccionado is null)
        {
            MessageBox.Show(
                "Selecciona un vehículo de la tabla.",
                "Vehículo no seleccionado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        DialogResult respuesta =
            MessageBox.Show(
                $"¿Deseas eliminar el vehículo " +
                $"{vehiculoSeleccionado.Placa}?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

        if (respuesta != DialogResult.Yes)
        {
            return;
        }

        string placaEliminada =
            vehiculoSeleccionado.Placa;

        vehiculos.Remove(vehiculoSeleccionado);

        LimpiarFormulario();

        lblEstado.Text =
            $"Vehículo con placa " +
            $"{placaEliminada} eliminado.";
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

        txtPlaca.Text = seleccionado.Placa;
        txtMarca.Text = seleccionado.Marca;
        txtModelo.Text = seleccionado.Modelo;
        nudAnio.Value = seleccionado.Anio;
        txtColor.Text = seleccionado.Color;

        btnModificar.Enabled = true;
        btnEliminar.Enabled = true;

        lblEstado.Text =
            $"Vehículo {seleccionado.Placa} seleccionado.";
    }

    private void LimpiarFormulario()
    {
        txtPlaca.Clear();
        txtMarca.Clear();
        txtModelo.Clear();
        txtColor.Clear();

        nudAnio.Value = DateTime.Now.Year;

        vehiculoSeleccionado = null;

        dgvVehiculos.ClearSelection();

        btnModificar.Enabled = false;
        btnEliminar.Enabled = false;

        lblEstado.Text =
            "Completa los datos del vehículo.";

        txtPlaca.Focus();
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
        Color color)
    {
        var boton = new Button
        {
            Text = texto,
            Width = 125,
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