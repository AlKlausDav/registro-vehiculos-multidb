# Manual de Usuario

## Registro de Vehículos en Múltiples Bases de Datos

## 1. Introducción

La aplicación Registro de Vehículos permite administrar la información de una flotilla utilizando una única interfaz gráfica.

El usuario puede seleccionar uno de los siguientes motores:

- MySQL.
- Microsoft SQL Server.
- Oracle Database.

La aplicación permite registrar, consultar, modificar y eliminar vehículos.

## 2. Información del vehículo

Cada vehículo contiene los siguientes datos:

- Placa.
- Marca.
- Modelo.
- Año.
- Color.

La placa identifica de forma única a cada vehículo.

## 3. Inicio de la aplicación

Para iniciar la aplicación:

1. Abra una terminal en la carpeta principal del proyecto.
2. Ejecute:

```powershell
dotnet run --project src\RegistroVehiculos.App