# Registro de Vehículos en Múltiples Bases de Datos

Aplicación de escritorio desarrollada en C# y Windows Forms para administrar el registro de una flotilla de vehículos utilizando MySQL, Microsoft SQL Server u Oracle Database.

La aplicación utiliza una sola interfaz gráfica y permite seleccionar el motor de base de datos sin modificar la lógica principal del sistema.

## Funcionalidades

- Registrar vehículos.
- Consultar los vehículos almacenados.
- Modificar vehículos existentes.
- Eliminar vehículos.
- Seleccionar entre MySQL, SQL Server y Oracle.
- Validar la información antes de almacenarla.
- Evitar placas duplicadas.
- Mostrar mensajes informativos y de error.
- Probar la conexión antes de realizar operaciones.

## Información administrada

Cada vehículo contiene:

- Placa.
- Marca.
- Modelo.
- Año.
- Color.

## Tecnologías

- C#.
- .NET 10.
- Windows Forms.
- ADO.NET.
- MySql.Data.
- Microsoft.Data.SqlClient.
- Oracle.ManagedDataAccess.Core.
- Git y GitHub.

Este proyecto no utiliza Docker.

## Arquitectura

La solución está dividida en tres proyectos:

### RegistroVehiculos.App

Contiene la interfaz gráfica Windows Forms y coordina las acciones del usuario.

### RegistroVehiculos.Core

Contiene:

- El modelo `Vehiculo`.
- Las validaciones.
- El enum de motores.
- La interfaz común `IVehiculoRepository`.

### RegistroVehiculos.Infrastructure

Contiene:

- La configuración de conexiones.
- La fábrica de repositorios.
- El repositorio de MySQL.
- El repositorio de SQL Server.
- El repositorio de Oracle.

## Estructura del proyecto

```text
PrimerProyecto/
├── database/
│   ├── mysql/
│   │   └── schema.sql
│   ├── sqlserver/
│   │   └── schema.sql
│   └── oracle/
│       └── schema.sql
├── docs/
├── src/
│   ├── RegistroVehiculos.App/
│   ├── RegistroVehiculos.Core/
│   └── RegistroVehiculos.Infrastructure/
├── .gitignore
├── appsettings.example.json
├── RegistroVehiculos.sln
└── README.md
```

## Requisitos

Para modificar y compilar el proyecto se necesita:

- Windows 10 u 11.
- .NET 10 SDK.
- Visual Studio Code con C# Dev Kit o Visual Studio Community.
- Git.
- Al menos uno de los siguientes motores para probar conexiones:
  - MySQL.
  - Microsoft SQL Server.
  - Oracle Database.

Para comprobar el funcionamiento completo deben estar disponibles los tres motores.

## Descargar el proyecto

Clonar el repositorio:

```powershell
git clone https://github.com/AlKlausDav/registro-vehiculos-multidb.git
```

Entrar en la carpeta:

```powershell
cd registro-vehiculos-multidb
```

Restaurar los paquetes:

```powershell
dotnet restore RegistroVehiculos.sln
```

Compilar:

```powershell
dotnet build RegistroVehiculos.sln
```

## Configuración de conexiones

El repositorio contiene el archivo:

```text
appsettings.example.json
```

Crear una copia privada:

```powershell
Copy-Item appsettings.example.json appsettings.local.json
```

Editar `appsettings.local.json` y sustituir `CAMBIAR` por las credenciales reales.

Ejemplo de estructura:

```json
{
  "ConnectionStrings": {
    "MySql": "Server=localhost;Port=3306;Database=registro_vehiculos;User ID=root;Password=CAMBIAR;",
    "SqlServer": "Server=localhost;Database=RegistroVehiculos;User ID=sa;Password=CAMBIAR;Encrypt=True;TrustServerCertificate=True;",
    "Oracle": "User ID=registro_vehiculos;Password=CAMBIAR;Data Source=localhost:1521/FREEPDB1;"
  }
}
```

`appsettings.local.json` está excluido de Git para evitar publicar contraseñas.

## Preparar MySQL

Ejecutar el archivo:

```text
database/mysql/schema.sql
```

Puede ejecutarse desde MySQL Workbench o una herramienta compatible.

El script crea:

- La base `registro_vehiculos`.
- La tabla `vehiculos`.
- La llave primaria sobre la placa.
- La validación del año.

## Preparar SQL Server

Ejecutar el archivo:

```text
database/sqlserver/schema.sql
```

Puede ejecutarse desde SQL Server Management Studio.

El script crea:

- La base `RegistroVehiculos`.
- La tabla `dbo.vehiculos`.
- La llave primaria sobre la placa.
- La validación del año.

## Preparar Oracle

Conectarse con el usuario que será propietario de la tabla y ejecutar:

```text
database/oracle/schema.sql
```

Puede ejecutarse desde Oracle SQL Developer.

El script crea:

- La tabla `vehiculos`.
- La llave primaria sobre la placa.
- La validación del año.

La base o usuario de Oracle debe existir antes de ejecutar el script.

## Ejecutar la aplicación

Desde la carpeta principal:

```powershell
dotnet run --project src\RegistroVehiculos.App
```

Dentro de la aplicación:

1. Seleccionar el motor.
2. Presionar **Conectar y cargar**.
3. Verificar el mensaje de conexión.
4. Registrar, consultar, modificar o eliminar vehículos.

## Validaciones

La aplicación valida:

- Placa obligatoria.
- Placa entre 3 y 15 caracteres.
- Placa formada por letras, números y guiones.
- Marca obligatoria.
- Modelo obligatorio.
- Año entre 1900 y el año siguiente al actual.
- Color obligatorio.
- Placas duplicadas.

Además, la placa es llave primaria en las tres bases de datos.

## Seguridad

- Las consultas utilizan parámetros.
- Las contraseñas no se incluyen en el código fuente.
- `appsettings.local.json` no se publica en GitHub.
- Las conexiones se configuran de forma local en cada equipo.

## Estado del proyecto

El código fuente y la arquitectura para los tres motores están implementados.

La validación final de conexiones debe realizarse en un equipo con MySQL, SQL Server y Oracle configurados.

## Repositorio

https://github.com/AlKlausDav/registro-vehiculos-multidb