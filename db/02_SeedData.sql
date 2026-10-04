-- =============================================
-- 01_CreateDatabase.sql
-- =============================================
USE master;
GO

IF DB_ID('SistemaVentasDb') IS NOT NULL
BEGIN
    ALTER DATABASE SistemaVentasDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SistemaVentasDb;
END
GO

CREATE DATABASE SistemaVentasDb;
GO

USE SistemaVentasDb;
GO

CREATE TABLE dbo.ConfiguracionEmpresa (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreEmpresa NVARCHAR(200) NOT NULL,
    Ruc NVARCHAR(20) NULL,
    Direccion NVARCHAR(250) NULL,
    Telefono NVARCHAR(30) NULL,
    Correo NVARCHAR(100) NULL,
    Moneda NVARCHAR(10) DEFAULT 'PEN',
    IgvPorcentaje DECIMAL(5,2) DEFAULT 18.00,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Contrasena NVARCHAR(255) NOT NULL,
    NombreCompleto NVARCHAR(150) NOT NULL,
    Rol NVARCHAR(50) NOT NULL DEFAULT 'Cajero',
    Activo BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.Clientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Apellido NVARCHAR(150) NULL,
    Documento NVARCHAR(20) NULL,
    TipoDocumento NVARCHAR(20) NULL,
    Telefono NVARCHAR(30) NULL,
    Correo NVARCHAR(150) NULL,
    Direccion NVARCHAR(250) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(50) NOT NULL UNIQUE,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    CategoriaId INT NOT NULL,
    PrecioCompra DECIMAL(18,2) NOT NULL DEFAULT 0,
    PrecioVenta DECIMAL(18,2) NOT NULL DEFAULT 0,
    Stock INT NOT NULL DEFAULT 0,
    StockMinimo INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias(Id)
);

CREATE TABLE dbo.Ventas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NULL,
    UsuarioId INT NOT NULL,
    FechaVenta DATETIME NOT NULL DEFAULT GETDATE(),
    SubTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    Igv DECIMAL(18,2) NOT NULL DEFAULT 0,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Pendiente',
    TipoComprobante NVARCHAR(30) NOT NULL DEFAULT 'Factura',
    NumeroSerie NVARCHAR(50) NULL,
    NumeroDocumento NVARCHAR(50) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Ventas_Clientes FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes(Id),
    CONSTRAINT FK_Ventas_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id)
);

CREATE TABLE dbo.DetalleVenta (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VentaId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    Descuento DECIMAL(18,2) NOT NULL DEFAULT 0,
    SubTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DetalleVenta_Ventas FOREIGN KEY (VentaId) REFERENCES dbo.Ventas(Id),
    CONSTRAINT FK_DetalleVenta_Productos FOREIGN KEY (ProductoId) REFERENCES dbo.Productos(Id)
);

CREATE TABLE dbo.Facturas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VentaId INT NOT NULL,
    NumeroFactura NVARCHAR(50) NOT NULL UNIQUE,
    FechaEmision DATETIME NOT NULL DEFAULT GETDATE(),
    Total DECIMAL(18,2) NOT NULL,
    Igv DECIMAL(18,2) NOT NULL,
    SubTotal DECIMAL(18,2) NOT NULL,
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Emitida',
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Facturas_Ventas FOREIGN KEY (VentaId) REFERENCES dbo.Ventas(Id)
);

CREATE TABLE dbo.Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VentaId INT NOT NULL,
    NumeroTicket NVARCHAR(50) NOT NULL UNIQUE,
    FechaEmision DATETIME NOT NULL DEFAULT GETDATE(),
    TextoTicket NVARCHAR(MAX) NULL,
    Impreso BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Tickets_Ventas FOREIGN KEY (VentaId) REFERENCES dbo.Ventas(Id)
);

GO

CREATE INDEX IX_Productos_CategoriaId ON dbo.Productos(CategoriaId);
CREATE INDEX IX_Ventas_Fecha ON dbo.Ventas(FechaVenta);
CREATE INDEX IX_DetalleVenta_VentaId ON dbo.DetalleVenta(VentaId);
CREATE INDEX IX_Facturas_NumeroFactura ON dbo.Facturas(NumeroFactura);
CREATE INDEX IX_Tickets_NumeroTicket ON dbo.Tickets(NumeroTicket);
GO

PRINT 'Base de datos SistemaVentasDb creada correctamente.';
GO
