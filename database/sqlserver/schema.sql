IF DB_ID(N'RegistroVehiculos') IS NULL
BEGIN
    CREATE DATABASE RegistroVehiculos;
END;
GO

USE RegistroVehiculos;
GO

IF OBJECT_ID(N'dbo.vehiculos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.vehiculos
    (
        placa VARCHAR(15) NOT NULL,
        marca VARCHAR(50) NOT NULL,
        modelo VARCHAR(50) NOT NULL,
        anio INT NOT NULL,
        color VARCHAR(30) NOT NULL,

        CONSTRAINT pk_vehiculos
            PRIMARY KEY (placa),

        CONSTRAINT ck_vehiculos_anio
            CHECK (anio BETWEEN 1900 AND 2100)
    );
END;
GO