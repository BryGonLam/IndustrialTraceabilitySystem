05 — Database Design
Industrial Traceability System (ITS)
Estado: Borrador inicial
Version: 0.1.0
Documento previo: docs/04-architecture-overview.md

1. Proposito de este documento

Este documento define el esquema relacional de la base de datos del sistema, a partir del modelo de dominio (03-domain-model.md) y de la arquitectura definida (04-architecture-overview.md).

Aqui se definen:
tablas
columnas
tipos de datos
llaves primarias y foraneas
indices
restricciones

No se definen todavia:

endpoints
DTOs
clases del backend
migraciones de EF Core (se generan en la etapa de setup)
componentes de UI

Motor elegido: SQL Server.

2. Convenciones

Para mantener consistencia en todo el esquema:

Nombres de tablas en plural, PascalCase: Products, Units, ProductionEvents.
Nombres de columnas en PascalCase: CreatedAt, ProductId.
Llave primaria: columna Id de tipo int con IDENTITY en la mayoria de tablas.
Llaves foraneas: <Entidad>Id, por ejemplo ProductId, StationId.
Fechas en UTC, tipo datetime2.
Campos de auditoria base: CreatedAt, UpdatedAt (nullable si aplica).
Borrado logico: columna IsActive (bit) en tablas de catalogo, no en eventos.

No se usa DELETE fisico en ProductionEvents.

3. Vista general de tablas

Catalogo y produccion:
Products
Routes
RouteSteps
ProductionLines
Stations
ProductionOrders
Units

Trazabilidad:
ProductionEvents

Usuarios:
Users
Roles
UserRoles

Products 1 --- N Routes
Routes 1 --- N RouteSteps
RouteSteps N --- 1 Stations

ProductionLines 1 --- N Stations
ProductionLines 1 --- N ProductionOrders

ProductionOrders N --- 1 Products
ProductionOrders N --- 1 ProductionLines
ProductionOrders 1 --- N Units

Units 1 --- N ProductionEvents
ProductionEvents N --- 1 Stations
ProductionEvents N --- 1 Users

Users N --- N Roles (via UserRoles)

4. Definicion de tablas

4.1 Products
Representa un modelo o tipo de producto fabricable.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
Code nvarchar(50) NOT NULL, UNIQUE Codigo ficticio del producto (ej. CTRL-BOARD-A)
Name nvarchar(150) NOT NULL Nombre del producto
Description nvarchar(500) NULL Descripcion opcional
IsActive bit NOT NULL, default 1 Borrado logico
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
UpdatedAt datetime2 NULL Fecha de ultima modificacion en UTC
Indices:

PK en Id
UNIQUE en Code

4.2 Routes
Representa la ruta de estaciones asociada a un producto.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
ProductId int FK -> Products.Id, NOT NULL Producto al que pertenece
Name nvarchar(150) NOT NULL Nombre de la ruta
IsActive bit NOT NULL, default 1 Borrado logico
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
UpdatedAt datetime2 NULL Fecha de ultima modificacion en UTC
Indices:

PK en Id
FK en ProductId
UNIQUE en (ProductId, Name) para evitar rutas duplicadas por producto

Nota: se asume una ruta activa por producto en la v1. Si se requieren multiples rutas por producto, el modelo lo permite, pero la logica de seleccion se define en la capa de aplicacion.

4.3 RouteSteps
Representa un paso dentro de una ruta.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
RouteId int FK -> Routes.Id, NOT NULL Ruta a la que pertenece
StationId int FK -> Stations.Id, NOT NULL Estacion del paso
StepOrder int NOT NULL Orden del paso dentro de la ruta
IsMandatory bit NOT NULL, default 1 Indica si el paso es obligatorio
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
Indices:

PK en Id
FK en RouteId
FK en StationId
UNIQUE en (RouteId, StepOrder) para evitar pasos duplicados en la misma posicion

4.4 ProductionLines
Representa una linea fisica de produccion.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
Code nvarchar(50) NOT NULL, UNIQUE Codigo ficticio de la linea (ej. LINE-SMT-01)
Name nvarchar(150) NOT NULL Nombre de la linea
IsActive bit NOT NULL, default 1 Borrado logico
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
UpdatedAt datetime2 NULL Fecha de ultima modificacion en UTC
Indices:

PK en Id
UNIQUE en Code

4.5 Stations
Representa un punto de trabajo dentro de una linea.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
ProductionLineId int FK -> ProductionLines.Id, NOT NULL Linea a la que pertenece
Code nvarchar(50) NOT NULL Codigo ficticio de la estacion (ej. AOI)
Name nvarchar(150) NOT NULL Nombre de la estacion
IsActive bit NOT NULL, default 1 Borrado logico
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
UpdatedAt datetime2 NULL Fecha de ultima modificacion en UTC
Indices:

PK en Id
FK en ProductionLineId
UNIQUE en (ProductionLineId, Code) para evitar estaciones duplicadas por linea

4.6 ProductionOrders
Representa una corrida de produccion.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
OrderNumber nvarchar(50) NOT NULL, UNIQUE Numero ficticio de orden (ej. OP-2026-0001)
ProductId int FK -> Products.Id, NOT NULL Producto asociado
ProductionLineId int FK -> ProductionLines.Id, NOT NULL Linea asociada
PlannedQuantity int NOT NULL Cantidad planificada
Status nvarchar(30) NOT NULL Estado de la orden
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
UpdatedAt datetime2 NULL Fecha de ultima modificacion en UTC
Valores validos para Status (en v1):

Planned
InProgress
Closed
Cancelled

Indices:
PK en Id
UNIQUE en OrderNumber
FK en ProductId
FK en ProductionLineId
INDEX en Status para consultas de monitoreo

Nota: se puede agregar un CHECK CONSTRAINT para limitar los valores de Status. Se define en la migracion.

4.7 Units
Representa la unidad trazable. Es la entidad central del sistema.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
SerialNumber nvarchar(50) NOT NULL, UNIQUE Serial ficticio de la unidad (ej. PCB000001)
ProductionOrderId int FK -> ProductionOrders.Id, NOT NULL Orden a la que pertenece
Status nvarchar(30) NOT NULL Estado actual derivado
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
UpdatedAt datetime2 NULL Fecha de ultima modificacion en UTC
Valores validos para Status (en v1):

Created
InProgress
OnHold
InRework
Passed
Scrapped

Indices:
PK en Id
UNIQUE en SerialNumber
FK en ProductionOrderId
INDEX en Status para consultas de monitoreo

Nota sobre Status: conceptualmente el estado se deriva de los eventos. En la base de datos se materializa como columna para consultas rapidas. La fuente de verdad siguen siendo los ProductionEvents. La logica de actualizacion se define en la capa de aplicacion.

4.8 ProductionEvents
Representa un evento de produccion sobre una unidad. Es append-only.

Columna Tipo Restricciones Descripcion
Id bigint PK, IDENTITY Identificador interno
UnitId int FK -> Units.Id, NOT NULL Unidad afectada
StationId int FK -> Stations.Id, NOT NULL Estacion donde ocurrio
UserId int FK -> Users.Id, NULL Usuario que registro (nullable en v1)
EventType nvarchar(30) NOT NULL Tipo de evento
Result nvarchar(30) NULL Resultado asociado, si aplica
Notes nvarchar(500) NULL Notas opcionales
OccurredAt datetime2 NOT NULL Fecha del evento en UTC
CreatedAt datetime2 NOT NULL Fecha de registro en UTC
Valores validos para EventType:

START
STOP
PASS
FAIL
REWORK
SCRAP
HOLD

Valores validos para Result (cuando aplique):
PASS
FAIL
COMPLETE
RELEASED

Indices:
PK en Id
FK en UnitId
FK en StationId
FK en UserId
INDEX en (UnitId, OccurredAt) para reconstruir historial de una unidad
INDEX en (StationId, OccurredAt) para reportes por estacion
INDEX en EventType para filtros

Nota sobre append-only: no se permite UPDATE ni DELETE en esta tabla desde la aplicacion. Se puede reforzar con permisos a nivel de base de datos o con reglas en la capa de aplicacion.

Nota sobre Id bigint: se usa bigint porque esta tabla crecera mas rapido que las demas.

4.9 Users
Representa a un usuario del sistema.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
Username nvarchar(100) NOT NULL, UNIQUE Nombre de usuario ficticio
PasswordHash nvarchar(500) NOT NULL Hash de contrasena
DisplayName nvarchar(150) NOT NULL Nombre para mostrar
IsActive bit NOT NULL, default 1 Borrado logico
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
UpdatedAt datetime2 NULL Fecha de ultima modificacion en UTC
Indices:

PK en Id
UNIQUE en Username

Nota: nunca se almacena contrasena en texto plano. El algoritmo de hashing se define en la etapa de autenticacion.

4.10 Roles
Representa un rol del sistema.

Columna Tipo Restricciones Descripcion
Id int PK, IDENTITY Identificador interno
Name nvarchar(50) NOT NULL, UNIQUE Nombre del rol (ej. Operator)
Description nvarchar(300) NULL Descripcion opcional
CreatedAt datetime2 NOT NULL Fecha de creacion en UTC
Roles iniciales sugeridos (ficticios):

Admin
Supervisor
Operator
Quality

4.11 UserRoles
Tabla intermedia para la relacion N a N entre Users y Roles.

Columna Tipo Restricciones Descripcion
UserId int PK, FK -> Users.Id Usuario
RoleId int PK, FK -> Roles.Id Rol
AssignedAt datetime2 NOT NULL Fecha de asignacion en UTC
Indices:

PK compuesta en (UserId, RoleId)
FK en UserId
FK en RoleId

5. Diagrama relacional simplificado

Products
Id (PK)
Code (UQ)
Name
...
|
| 1
v
N
Routes
Id (PK)
ProductId (FK)
...
|
| 1
v
N
RouteSteps
Id (PK)
RouteId (FK)
StationId (FK)
StepOrder
...

ProductionLines
Id (PK)
Code (UQ)
...
|
+---- 1 --- N ---> Stations
| Id (PK)
| ProductionLineId (FK)
| Code
| ...
|
+---- 1 --- N ---> ProductionOrders
Id (PK)
OrderNumber (UQ)
ProductId (FK)
ProductionLineId (FK)
Status
...
|
| 1
v
N
Units
Id (PK)
SerialNumber (UQ)
ProductionOrderId (FK)
Status
...
|
| 1
v
N
ProductionEvents
Id (PK, bigint)
UnitId (FK)
StationId (FK)
UserId (FK)
EventType
Result
OccurredAt
...

Users
Id (PK)
Username (UQ)
PasswordHash
...
|
| N
v
N
Roles
Id (PK)
Name (UQ)
...

(relacion via UserRoles)

6. Reglas de integridad

Ademas de las llaves, se aplican las siguientes reglas:

ProductionEvents es append-only. No se permite UPDATE ni DELETE desde la aplicacion.
Una Unit no puede registrar eventos si su Status es Scrapped.
Una Unit no puede registrar eventos en Stations que no pertenezcan a la Route de su Product.
Una ProductionOrder no puede tener Units si esta en estado Cancelled.
Un User no puede tener Username duplicado.
Un Product no puede tener Code duplicado.
Una ProductionLine no puede tener Code duplicado.
No se elimina fisicamente ningun registro de catalogo; se marca IsActive = 0.

Algunas de estas reglas se validan en la capa de aplicacion. Otras pueden reforzarse con constraints en la base de datos.

7. Consideraciones de rendimiento

La tabla ProductionEvents es la que mas crecera. Se indexa por (UnitId, OccurredAt) para reconstruir historial.
Units se indexa por Status para consultas de monitoreo.
ProductionOrders se indexa por Status para dashboards futuros.
No se agregan indices adicionales hasta que exista una necesidad real medida. La regla es: primero medir, luego optimizar.

8. Datos ficticios iniciales (seed)

Para que el sistema sea demostrable, se cargaran datos ficticios en la etapa de setup:
3 productos ficticios con sus rutas.
2 lineas de produccion ficticias.
Estaciones tipicas: Printer, SPI, AOI, Rework, Final Inspection.
1 usuario admin ficticio.
Roles base: Admin, Supervisor, Operator, Quality.
1 o 2 ordenes de produccion ficticias.
Unidades ficticias con eventos de ejemplo.
El detalle exacto se define cuando se implementen las migraciones y el seeding.

9. Fuera de este documento

Este documento no cubre:
migraciones de EF Core
scripts SQL versionados
configuracion de conexion
estrategia de backups
seguridad a nivel de base de datos
endpoints
DTOs
clases del backend

Todo eso se trata en etapas posteriores.
