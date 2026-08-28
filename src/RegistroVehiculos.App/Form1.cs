using RegistroVehiculos.Core.Enums;
using RegistroVehiculos.Core.Models;

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

    public Form1()
    {
        InitializeComponent();

        btnNuevo = CrearBoton(
            "Nuevo",
            System.Drawing.Color.FromArgb(108, 117, 125));

        btnGuardar = CrearBoton(
            "Guardar",
            System.Drawing.Color.FromArgb(25, 135, 84));

        btnModificar = CrearBoton(
            "Modificar",
            System.Drawing.Color.FromArgb(13, 110, 253));

        btnEliminar = CrearBoton(
            "Eliminar",
            System.Drawing.Color.FromArgb(220, 53, 69));

        ConfigurarVentana();
        ConstruirInterfaz();
        ConfigurarTabla();
    }

    private void ConfigurarVentana()
    {
        Text = "Registro de Vehículos";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 600);
        Size = new Size(1050, 700);
        BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
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
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(31, 41, 55),
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

        panelMotor.Controls.Add(new Label
        {
            Text = "Motor de base de datos:",
            AutoSize = true,
            Margin = new Padding(0, 7, 12, 0)
        });

        cboMotor.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMotor.Width = 220;
        cboMotor.DataSource = Enum.GetValues<MotorBaseDatos>();

        panelMotor.Controls.Add(cboMotor);
        contenedor.Controls.Add(panelMotor, 0, 1);

        var formulario = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3,
            BackColor = System.Drawing.Color.White,
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

        AgregarCampo(formulario, "Placa:", txtPlaca, 0, 0);
        AgregarCampo(formulario, "Marca:", txtMarca, 0, 2);
        AgregarCampo(formulario, "Modelo:", txtModelo, 1, 0);
        AgregarCampo(formulario, "Año:", nudAnio, 1, 2);
        AgregarCampo(formulario, "Color:", txtColor, 2, 0);

        contenedor.Controls.Add(formulario, 0, 2);

        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 8, 0, 0)
        };

        panelBotones.Controls.Add(btnNuevo);
        panelBotones.Controls.Add(btnGuardar);
        panelBotones.Controls.Add(btnModificar);
        panelBotones.Controls.Add(btnEliminar);

        contenedor.Controls.Add(panelBotones, 0, 3);
        contenedor.Controls.Add(dgvVehiculos, 0, 4);

        lblEstado.Text = "Aplicación preparada.";
        lblEstado.Dock = DockStyle.Fill;
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;
        lblEstado.ForeColor =
            System.Drawing.Color.FromArgb(75, 85, 99);

        contenedor.Controls.Add(lblEstado, 0, 5);
    }

    private void ConfigurarTabla()
    {
        dgvVehiculos.Dock = DockStyle.Fill;
        dgvVehiculos.BackgroundColor = System.Drawing.Color.White;
        dgvVehiculos.BorderStyle = BorderStyle.FixedSingle;
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

        dgvVehiculos.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Placa",
            DataPropertyName = nameof(Vehiculo.Placa)
        });

        dgvVehiculos.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Marca",
            DataPropertyName = nameof(Vehiculo.Marca)
        });

        dgvVehiculos.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Modelo",
            DataPropertyName = nameof(Vehiculo.Modelo)
        });

        dgvVehiculos.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Año",
            DataPropertyName = nameof(Vehiculo.Anio)
        });

        dgvVehiculos.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Color",
            DataPropertyName = nameof(Vehiculo.Color)
        });
    }

    private static void ConfigurarCampo(TextBox campo)
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
        panel.Controls.Add(new Label
        {
            Text = etiqueta,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, columna, fila);

        panel.Controls.Add(campo, columna + 1, fila);
    }

    private static Button CrearBoton(
        string texto,
        System.Drawing.Color color)
    {
        return new Button
        {
            Text = texto,
            Width = 125,
            Height = 38,
            BackColor = color,
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 10, 0)
        };
    }
}