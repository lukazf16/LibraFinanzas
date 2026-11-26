/* =====================================================================
   Libra — Finanzas Personales | Script de base de datos (SQL Server)
   OPCIONAL: la aplicación crea la base de datos automáticamente desde
   C# con Entity Framework Core (EnsureCreatedAsync en Data/DbSeeder.cs).
   ===================================================================== */

IF DB_ID(N'LibraFinanzas') IS NULL
    CREATE DATABASE LibraFinanzas;
GO

USE LibraFinanzas;
GO

CREATE TABLE Usuario (
    id_usuario    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
    nombre        NVARCHAR(100)     NOT NULL,
    correo        NVARCHAR(100)     NOT NULL,
    telefono      NVARCHAR(20)      NOT NULL,
    contrasena    NVARCHAR(100)     NOT NULL,
    saldo_inicial DECIMAL(12,2)     NOT NULL CONSTRAINT DF_Usuario_saldo DEFAULT 0,
    activo        BIT               NOT NULL CONSTRAINT DF_Usuario_activo DEFAULT 1
);
CREATE UNIQUE INDEX IX_Usuario_Correo ON Usuario (correo);

CREATE TABLE Movimiento (
    id_movimiento INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Movimiento PRIMARY KEY,
    id_usuario    INT               NOT NULL,
    tipo          NVARCHAR(20)      NOT NULL,
    descripcion   NVARCHAR(120)     NOT NULL,
    monto         DECIMAL(12,2)     NOT NULL,
    categoria     NVARCHAR(40)      NOT NULL,
    clasificacion NVARCHAR(30)      NOT NULL,
    fecha         DATETIME2         NOT NULL,
    CONSTRAINT FK_Movimiento_Usuario FOREIGN KEY (id_usuario)
        REFERENCES Usuario (id_usuario) ON DELETE CASCADE,
    CONSTRAINT CK_Movimiento_Monto CHECK (monto > 0)
);
CREATE INDEX IX_Movimiento_id_usuario ON Movimiento (id_usuario);
CREATE INDEX IX_Movimiento_fecha      ON Movimiento (fecha DESC);

CREATE TABLE MetaAhorro (
    id_meta        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MetaAhorro PRIMARY KEY,
    id_usuario     INT               NOT NULL,
    nombre         NVARCHAR(100)     NOT NULL,
    monto_objetivo DECIMAL(12,2)     NOT NULL,
    monto_actual   DECIMAL(12,2)     NOT NULL CONSTRAINT DF_MetaAhorro_actual DEFAULT 0,
    fecha_limite   DATETIME2         NOT NULL,
    color_barra    NVARCHAR(20)      NOT NULL CONSTRAINT DF_MetaAhorro_color DEFAULT N'emerald',
    CONSTRAINT FK_MetaAhorro_Usuario FOREIGN KEY (id_usuario)
        REFERENCES Usuario (id_usuario) ON DELETE CASCADE,
    CONSTRAINT CK_MetaAhorro_Objetivo CHECK (monto_objetivo > 0)
);
CREATE INDEX IX_MetaAhorro_id_usuario ON MetaAhorro (id_usuario);
GO
