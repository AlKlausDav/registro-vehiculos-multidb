# Manual Técnico

## Registro de Vehículos en Múltiples Bases de Datos

## 1. Descripción general

Registro de Vehículos es una aplicación de escritorio desarrollada en C# con Windows Forms y .NET 10.

Permite administrar vehículos mediante una única interfaz gráfica conectada a uno de los siguientes motores:

- MySQL.
- Microsoft SQL Server.
- Oracle Database.

La lógica principal no cambia cuando el usuario selecciona otro motor. Cada implementación utiliza el contrato común `IVehiculoRepository`.

## 2. Tecnologías utilizadas

- C#.
- .NET 10.
- Windows Forms.
- ADO.NET.
- System.Text.Json.
- MySql.Data.
- Microsoft.Data.SqlClient.
- Oracle.ManagedDataAccess.Core.
- Git.
- GitHub.

El proyecto no utiliza Docker.

## 3. Requisitos de desarrollo

- Windows 10 u 11.
- .NET 10 SDK.
- Visual Studio Code con C# Dev Kit o Visual Studio Community.
- Git.
- Acceso a Internet para restaurar paquetes NuGet.

Para validar todas las conexiones también se requiere acceso a:

- MySQL.
- SQL Server.
- Oracle Database.

## 4. Repositorio

```text
https://github.com/AlKlausDav/registro-vehiculos-multidb
```

Clonar:

```powershell
git clone https://github.com/AlKlausDav/registro-vehiculos-multidb.git
```

Entrar en la carpeta:

```powershell
cd registro-vehiculos-multidb
```

Restaurar dependencias:

```powershell
dotnet restore RegistroVehiculos.sln
```

Compilar:

```powershell
dotnet build RegistroVehiculos.sln
```

## 5. Estructura de la solución

```text
RegistroVehiculos.sln
│
├── RegistroVehiculos.App
├── RegistroVehiculos.Core
└── RegistroVehiculos.Infrastructure
```

### RegistroVehiculos.App

Responsabilidades:

- Crear la ventana principal.
- Mostrar controles gráficos.
- Recibir acciones del usuario.
- Mostrar resultados y errores.
- Seleccionar el motor.
- Ejecutar operaciones asíncronas.
- Actualizar la tabla.

### RegistroVehiculos.Core

Responsabilidades:

- Definir el modelo de vehículo.
- Definir los motores disponibles.
- Validar los datos.
- Definir el contrato del repositorio.

No contiene referencias a proveedores específicos.

### RegistroVehiculos.Infrastructure

Responsabilidades:

- Leer la configuración.
- Crear repositorios.
- Abrir conexiones.
- Ejecutar sentencias SQL.
- Convertir resultados a objetos.
- Implementar el CRUD para cada motor.

## 6. Estructura de carpetas

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
│   ├── manual-usuario.md
│   └── manual-tecnico.md
├── src/
│   ├── RegistroVehiculos.App/
│   │   ├── Form1.cs
│   │   ├── Form1.Designer.cs
│   │   ├── Program.cs
│   │   └── RegistroVehiculos.App.csproj
│   ├── RegistroVehiculos.Core/
│   │   ├── Enums/
│   │   ├── Interfaces/
│   │   ├── Models/
│   │   ├── Validation/
│   │   └── RegistroVehiculos.Core.csproj
│   └── RegistroVehiculos.Infrastructure/
│       ├── Configuration/
│       ├── Factories/
│       ├── Repositories/
│       └── RegistroVehiculos.Infrastructure.csproj
├── .gitignore
├── appsettings.example.json
├── README.md
└── RegistroVehiculos.sln
```

## 7. Modelo de dominio

El archivo `Vehiculo.cs` representa un registro.

Propiedades:

- `Placa`: texto.
- `Marca`: texto.
- `Modelo`: texto.
- `Anio`: entero.
- `Color`: texto.

La placa se utiliza como identificador principal.

No se utiliza una columna numérica adicional porque la placa identifica naturalmente al vehículo en los tres motores.

## 8. Motores disponibles

El enum `MotorBaseDatos` contiene:

```text
MySql
SqlServer
Oracle
```

El valor seleccionado en la interfaz determina qué repositorio será creado.

## 9. Validación

`VehiculoValidador` valida los datos antes de enviarlos al repositorio.

Reglas principales:

- Placa obligatoria.
- Longitud de placa entre 3 y 15.
- Placa formada por letras, números y guiones.
- Marca obligatoria y máximo de 50 caracteres.
- Modelo obligatorio y máximo de 50 caracteres.
- Color obligatorio y máximo de 30 caracteres.
- Año entre 1900 y el año siguiente al actual.

Los errores se devuelven como una lista de mensajes.

## 10. Contrato de repositorios

`IVehiculoRepository` define las operaciones comunes:

- `ProbarConexionAsync`.
- `ObtenerTodosAsync`.
- `ObtenerPorPlacaAsync`.
- `ExistePlacaAsync`.
- `AgregarAsync`.
- `ActualizarAsync`.
- `EliminarAsync`.

La interfaz gráfica trabaja con este contrato y no con los proveedores directamente.

## 11. Implementaciones

### MySqlVehiculoRepository

Utiliza:

```text
MySql.Data.MySqlClient
```

Características:

- Parámetros con prefijo `@`.
- Consulta limitada mediante `LIMIT 1`.
- Tipos `MySqlDbType`.
- Conexión mediante `MySqlConnection`.

### SqlServerVehiculoRepository

Utiliza:

```text
Microsoft.Data.SqlClient
```

Características:

- Parámetros con prefijo `@`.
- Consulta limitada mediante `TOP (1)`.
- Tipos `SqlDbType`.
- Conexión mediante `SqlConnection`.

### OracleVehiculoRepository

Utiliza:

```text
Oracle.ManagedDataAccess.Client
```

Características:

- Parámetros con prefijo `:`.
- Parámetros enlazados por nombre.
- Consulta limitada mediante `ROWNUM`.
- Tipos `OracleDbType`.
- Conexión mediante `OracleConnection`.

## 12. Fábrica de repositorios

`VehiculoRepositoryFactory` recibe la configuración de la aplicación.

Su método `Crear` recibe un `MotorBaseDatos` y devuelve:

```text
MySql → MySqlVehiculoRepository
SqlServer → SqlServerVehiculoRepository
Oracle → OracleVehiculoRepository
```

Esto permite cambiar de motor sin modificar la lógica de la interfaz.

## 13. Configuración

El archivo público es:

```text
appsettings.example.json
```

El archivo privado es:

```text
appsettings.local.json
```

El archivo privado se crea con:

```powershell
Copy-Item appsettings.example.json appsettings.local.json
```

Su estructura es:

```json
{
  "ConnectionStrings": {
    "MySql": "cadena de MySQL",
    "SqlServer": "cadena de SQL Server",
    "Oracle": "cadena de Oracle"
  }
}
```

`CargadorConfiguracion`:

1. Busca `appsettings.local.json`.
2. Lee el contenido.
3. Deserializa el JSON.
4. Valida que existan las tres cadenas.
5. Devuelve `ConfiguracionAplicacion`.

## 14. Protección de credenciales

`appsettings.local.json` está incluido en `.gitignore`.

Nunca se deben colocar credenciales reales en:

- `appsettings.example.json`.
- `README.md`.
- Manuales.
- Código fuente.
- Commits.
- Capturas públicas.

Antes de cada commit se recomienda ejecutar:

```powershell
git status
```

Y comprobar que `appsettings.local.json` no aparece.

## 15. Inicialización de la aplicación

`Program.cs` realiza:

1. Inicialización de Windows Forms.
2. Carga de la configuración.
3. Creación de `VehiculoRepositoryFactory`.
4. Creación de `Form1`.
5. Inicio del ciclo gráfico.

Si ocurre un error de configuración, se muestra un mensaje y la aplicación no continúa.

## 16. Flujo de conexión

1. El usuario selecciona el motor.
2. Presiona **Conectar y cargar**.
3. La interfaz solicita un repositorio a la fábrica.
4. El repositorio intenta abrir una conexión.
5. Si la conexión funciona, se consultan los vehículos.
6. La tabla se actualiza.
7. Si falla, se muestra un mensaje.

## 17. Flujo de registro

1. La interfaz construye un objeto `Vehiculo`.
2. El validador revisa los campos.
3. Se consulta si la placa existe.
4. Si está disponible, se ejecuta `AgregarAsync`.
5. El repositorio utiliza una consulta parametrizada.
6. Se recarga la tabla.
7. Se muestra el resultado.

## 18. Flujo de modificación

1. El usuario selecciona una fila.
2. La aplicación conserva la placa original.
3. El usuario cambia los datos.
4. La información se valida.
5. Si cambió la placa, se comprueba que no exista.
6. Se ejecuta `ActualizarAsync`.
7. La tabla se recarga.

La placa original permite encontrar el registro incluso si el usuario modifica la placa.

## 19. Flujo de eliminación

1. El usuario selecciona un vehículo.
2. La aplicación solicita confirmación.
3. Se ejecuta `EliminarAsync`.
4. El repositorio elimina mediante la placa.
5. La tabla se recarga.

## 20. Consultas parametrizadas

Las sentencias SQL no concatenan valores escritos por el usuario.

Los valores se envían mediante parámetros, reduciendo el riesgo de inyección SQL y evitando errores con caracteres especiales.

## 21. Estructura de las bases

Las tres bases contienen una tabla lógica equivalente:

```text
vehiculos
├── placa
├── marca
├── modelo
├── anio
└── color
```

Restricciones:

- `placa` es llave primaria.
- Todos los campos son obligatorios.
- `anio` debe estar entre 1900 y 2100.

## 22. Script de MySQL

Ubicación:

```text
database/mysql/schema.sql
```

Crea:

- Base `registro_vehiculos`.
- Tabla `vehiculos`.
- Llave primaria.
- Restricción del año.

## 23. Script de SQL Server

Ubicación:

```text
database/sqlserver/schema.sql
```

Crea:

- Base `RegistroVehiculos`.
- Tabla `dbo.vehiculos`.
- Llave primaria.
- Restricción del año.

## 24. Script de Oracle

Ubicación:

```text
database/oracle/schema.sql
```

Crea la tabla dentro del esquema conectado.

El usuario o esquema debe existir antes de ejecutar el script.

El código Oracle ignora el error `-955` cuando la tabla ya existe.

## 25. Manejo de errores

La aplicación controla:

- Archivo de configuración inexistente.
- Configuración incompleta.
- Error de conexión.
- Placa duplicada.
- Datos inválidos.
- Vehículo no seleccionado.
- Registro inexistente.
- Excepciones generadas por los proveedores.

Las operaciones visuales se realizan de forma asíncrona para evitar bloquear la ventana.

## 26. Compilación

Ejecutar:

```powershell
dotnet build RegistroVehiculos.sln
```

Una compilación correcta debe finalizar con cero errores.

## 27. Ejecución

Ejecutar desde la raíz:

```powershell
dotnet run --project src\RegistroVehiculos.App
```

La carpeta desde donde se inicia debe contener `appsettings.local.json`.

## 28. Prueba manual recomendada

Para cada motor:

1. Ejecutar el script correspondiente.
2. Configurar la conexión.
3. Iniciar la aplicación.
4. Seleccionar el motor.
5. Conectar.
6. Registrar un vehículo.
7. Intentar repetir la placa.
8. Modificar el vehículo.
9. Cerrar y abrir la aplicación.
10. Confirmar que el registro permanece.
11. Eliminar el vehículo.
12. Confirmar que desaparece.

## 29. Publicación

Para generar una versión para Windows de 64 bits:

```powershell
dotnet publish src\RegistroVehiculos.App -c Release -r win-x64 --self-contained true
```

La salida se genera dentro de:

```text
src/RegistroVehiculos.App/bin/Release/net10.0-windows/win-x64/publish
```

La configuración privada debe colocarse junto al ejecutable o en el directorio desde donde se inicia.

## 30. Control de versiones

Flujo recomendado:

```powershell
git status
git add .
git commit -m "descripción del cambio"
git push
```

No deben agregarse:

- `bin`.
- `obj`.
- Contraseñas.
- `appsettings.local.json`.
- Archivos temporales.