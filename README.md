# Sistema de Ventas con Reportes y Facturación de Tickets

Este repositorio contiene una base sólida para un sistema de ventas desarrollado en C# con Visual Studio, orientado a Microsoft SQL Server. Está pensado para servir como punto de partida para un proyecto real de punto de venta (POS), con módulos de clientes, productos, ventas, facturación, tickets, reportes y gestión de usuarios.

## Características principales

- Venta de productos con detalle por artículo
- Facturación y generación de tickets
- Control de inventario
- Gestión de clientes
- Reportes por ventas, productos y clientes
- Panel principal de administración
- Conexión a SQL Server
- Arquitectura por capas

## Tecnologías

- C#
- .NET 8
- Visual Studio 2022
- SQL Server / SQL Server LocalDB
- Windows Forms
- Microsoft.Data.SqlClient

## Estructura del proyecto

- `db/` : scripts SQL para base de datos y datos semilla
- `src/` : solución y proyectos en C#
- `docs/` : documentación del sistema (opcional)

## Proyecto base incluido

La solución incluye:

- `SistemaVentas.Common` : modelos y entidades
- `SistemaVentas.Data` : acceso a datos y conexión SQL Server
- `SistemaVentas.Business` : lógica de negocio
- `SistemaVentas.UI` : interfaz gráfica principal (Windows Forms)

## Base de datos

El script principal crea la base de datos `SistemaVentasDb` y las tablas:

- Usuarios
- Clientes
- Categorias
- Productos
- Ventas
- DetalleVenta
- Facturas
- Tickets
- ConfiguracionEmpresa

## Configuración rápida

1. Crear la base de datos usando el script SQL en `db/01_CreateDatabase.sql`
2. Ejecutar `db/02_SeedData.sql` para datos iniciales
3. Abrir `src/SistemaVentas.sln` en Visual Studio
4. Establecer `SistemaVentas.UI` como proyecto principal
5. Ajustar la cadena de conexión en `SistemaVentas.Data/SqlServerConnection.cs`
6. Ejecutar la aplicación

## Cadena de conexión por defecto

```csharp
Server=(localdb)\\MSSQLLocalDB;Database=SistemaVentasDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Si usas una instancia local de SQL Server, cambia el servidor según tu entorno.

## Siguiente evolución sugerida

- Formulario de gestión de clientes
- Formulario de productos
- Carrito de compras
- Generación de factura en PDF
- Impresión de tickets térmicos
- Reportes RDLC
- Login con permisos por rol
- Dashboard de ventas

## Repositorio

Este proyecto está pensado como base de arranque para un sistema completo de ventas.
