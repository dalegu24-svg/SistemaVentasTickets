# Fase 3: Facturación y Tickets

## Componentes implementados

### 1. FacturaService
- Generación de facturas con número único
- Generación de tickets con formato para impresora térmica
- Formato de ticket en ASCII art
- Números de comprobante secuenciales

### 2. Repositorios
- `FacturaRepository`: insertar y obtener facturas de SQL Server
- `TicketRepository`: insertar, obtener y marcar tickets como impresos

### 3. Formulario de Factura y Ticket
- Visualización de datos de factura
- Visualización de ticket con formato de impresora
- Botón para imprimir ticket
- Marca automática de ticket como impreso

### 4. Integración en Punto de Venta
- Al generar una venta, se crea factura y ticket automáticamente
- Se guardan en SQL Server
- Se muestra formulario de factura y ticket
- Se ofrece opción de imprimir ticket

## Flujo de venta completo

1. Usuario agrega productos al carrito
2. Calcula subtotal, IGV y total
3. Presiona "Generar venta y factura"
4. Sistema crea venta, factura y ticket
5. Se guardan en SQL Server
6. Se muestra diálogo con factura y ticket
7. Usuario puede imprimir ticket
8. Venta completada

## Números de comprobante

- Factura: `FAC-YYYYMM{VentaId}`
- Ticket: `TKT-YYYYMM{VentaId}`

## Impresión de ticket

El ticket se abre en editor de texto (Notepad) desde donde el usuario puede:
- Previsualizar el contenido
- Enviarlo a impresora térmica
- Guardarlo como archivo

## Mejoras futuras

- Integración directa con impresora térmica
- Generación de PDF para factura
- Logotipo de empresa en ticket
- Firma digital en factura
- Histórico de ventas con búsqueda
