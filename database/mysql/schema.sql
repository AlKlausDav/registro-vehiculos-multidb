CREATE DATABASE IF NOT EXISTS registro_vehiculos
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE registro_vehiculos;

CREATE TABLE IF NOT EXISTS vehiculos
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