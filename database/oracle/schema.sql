BEGIN
    EXECUTE IMMEDIATE '
        CREATE TABLE vehiculos
        (
            placa VARCHAR2(15 CHAR) NOT NULL,
            marca VARCHAR2(50 CHAR) NOT NULL,
            modelo VARCHAR2(50 CHAR) NOT NULL,
            anio NUMBER(4) NOT NULL,
            color VARCHAR2(30 CHAR) NOT NULL,

            CONSTRAINT pk_vehiculos
                PRIMARY KEY (placa),

            CONSTRAINT ck_vehiculos_anio
                CHECK (anio BETWEEN 1900 AND 2100)
        )
    ';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -955 THEN
            RAISE;
        END IF;
END;
/