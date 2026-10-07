07 — API Implementation
Industrial Traceability System (ITS)
Estado: Borrador inicial
Version: 0.1.0
Documento previo: docs/06-api-design.md

1. Proposito de este documento

Este documento describe la implementacion real de la API REST del proyecto, y su estado actual comparado con el diseno definido en docs/06-api-design.md.

No repite el diseno teorico. Solo documenta:
que endpoints existen hoy
como se comportan
que decisiones se tomaron durante la implementacion
que quedo pendiente

2. Estado general

Version de la API: v1
Prefijo: /api/v1
Framework: ASP.NET Core 8.0
ORM: Entity Framework Core 8.0.10
Base de datos: SQL Server Express
Autenticacion: no implementada todavia (v1 publica)
Serializacion de enums: numerica (pendiente cambiar a string)

3. Estructura del backend

src/backend/IndustrialTraceabilitySystem.Api/
├── Application/
│ └── Dtos/
│ ├── Products/
│ ├── ProductionLines/
│ ├── Stations/
│ ├── ProductionOrders/
│ ├── Units/
│ └── ProductionEvents/
├── Controllers/
├── Domain/
│ ├── Entities/
│ └── Enums/
├── Infrastructure/
│ ├── Persistence/
│ └── Seeding/
├── Migrations/
├── Program.cs
└── appsettings.json

Nota sobre capas: la arquitectura definida en el doc 04 contempla 4 capas. En v1 se implementaron como carpetas dentro del mismo proyecto en lugar de proyectos separados. Es una decision consciente para mantener la simplicidad. Si el proyecto crece, se puede refactorizar a multiples proyectos sin cambiar la logica.

4. Endpoints implementados

4.1 Products
Metodo Ruta Descripcion Estado
GET /api/v1/products Lista todos los productos ordenados por codigo Implementado
GET /api/v1/products/{id} Obtiene un producto por id Implementado
Decisiones:

Solo lectura en v1. Los POST/PUT quedan pendientes hasta que exista autenticacion y roles.
Usa AsNoTracking() porque son consultas de lectura.
Proyecta directamente a ProductResponse desde el query, sin cargar la entidad completa.

4.2 ProductionLines
Metodo Ruta Descripcion Estado
GET /api/v1/production-lines Lista todas las lineas ordenadas por codigo Implementado
GET /api/v1/production-lines/{id} Obtiene una linea por id Implementado
GET /api/v1/production-lines/{id}/stations Lista las estaciones de una linea Implementado
Decisiones:

{id}/stations valida primero que la linea exista. Si no existe, devuelve 404 con codigo LINE_NOT_FOUND.
Las estaciones se devuelven con el DTO StationResponse.

4.3 Stations
Metodo Ruta Descripcion Estado
GET /api/v1/stations/{id} Obtiene una estacion por id Implementado
Decisiones:

No hay endpoint GET /api/v1/stations (listar todas) porque las estaciones siempre se consultan en contexto de una linea. Si hace falta despues, se agrega.

4.4 ProductionOrders
Metodo Ruta Descripcion Estado
GET /api/v1/production-orders Lista ordenes con filtros Implementado
GET /api/v1/production-orders/{id} Obtiene una orden con sus unidades Implementado
Filtros soportados en el listado:

status (enum ProductionOrderStatus)
productId
lineId
Decisiones:
El listado incluye productCode, productName, productionLineCode, productionLineName y totalUnits aplanados en el response. Esto evita que el frontend tenga que hacer multiples llamadas.

El detalle ({id}) incluye la lista de unidades asociadas en units[].

Ordenamiento por CreatedAt descendente en el listado.

4.5 Units
Metodo Ruta Descripcion Estado
GET /api/v1/units Lista unidades con filtros Implementado
GET /api/v1/units/{id} Obtiene una unidad con datos del producto Implementado
GET /api/v1/units/{id}/history Devuelve el historial completo de una unidad Implementado
Filtros soportados en el listado:

orderId
status (enum UnitStatus)
serial (busqueda parcial, contiene)
Decisiones:
El listado incluye orderNumber aplanado.
El detalle incluye datos del producto (productCode, productName) navegando a traves de la orden.
El endpoint history devuelve los eventos en orden cronologico ascendente (OrderBy(OccurredAt)). Es el endpoint mas importante del sistema: reconstruye la trazabilidad completa de una unidad.

4.6 ProductionEvents
Metodo Ruta Descripcion Estado
GET /api/v1/production-events Lista eventos con filtros Implementado
GET /api/v1/production-events/{id} Obtiene un evento por id Implementado
POST /api/v1/production-events Registra un nuevo evento Implementado
Filtros soportados en el listado:

unitId
stationId
eventType (enum ProductionEventType)
dateFrom (fecha minima de ocurrencia)
dateTo (fecha maxima de ocurrencia)
Decisiones:
Los eventos son append-only. No existen PUT, PATCH ni DELETE.
El listado ordena por OccurredAt descendente (mas reciente primero).
El POST no requiere OccurredAt; si no se manda, se usa DateTime.UtcNow.
El POST no requiere UserId todavia. En la etapa de autenticacion se tomara del token JWT.
Validaciones implementadas en el POST:
La unidad debe existir (UNIT_NOT_FOUND).
La estacion debe existir (STATION_NOT_FOUND).
La unidad no debe estar en estado Scrapped (UNIT_ALREADY_SCRAPPED, devuelve 409).
Si se envia UserId, el usuario debe existir (USER_NOT_FOUND).
Validaciones pendientes (para etapa posterior):
Que la estacion pertenezca a la ruta del producto de la unidad.
Que el orden de las estaciones siga el flujo definido por la ruta.
Que un REWORK apunte a la estacion que fallo.
Que una unidad solo llegue a Passed al completar toda la ruta.
Actualizacion automatica del estado de la unidad:
El POST actualiza el estado de la unidad segun el tipo de evento:
EventType Nuevo estado de la unidad
Scrap Scrapped
Hold OnHold
Rework InRework
Fail InRework
Pass InProgress (por ahora)
Start / Stop Sin cambio
Nota importante: la logica de cuando una unidad pasa a Passed (completar la ruta) no esta implementada todavia. Se hara junto con la validacion completa del flujo. Por ahora, Pass deja la unidad en InProgress.

5. Estructura de respuestas

5.1 Respuesta exitosa (200)
Los endpoints devuelven el DTO directamente (sin envolver en un objeto). Ejemplo:

{
"id": 1,
"code": "CTRL-BOARD-A",
"name": "Control Board A",
"description": "Fictitious control board used for demos.",
"isActive": true,
"createdAt": "2026-10-05T22:14:55.51"
}

5.2 Respuesta exitosa (201 Created)
Para el POST de eventos, se devuelve el evento creado con header Location:

{
"id": 10,
"unitId": 2,
"serialNumber": "PCB000002",
"stationId": 3,
"stationCode": "AOI",
"stationName": "Automated Optical Inspection",
"userId": null,
"username": null,
"eventType": 3,
"result": 1,
"notes": "Manual test from swagger",
"occurredAt": "2026-10-07T17:37:40.90",
"createdAt": "2026-10-07T17:37:40.90"
}

5.3 Respuesta de error
Todos los errores usan la estructura definida en el doc 06:

{
"error": {
"code": "UNIT_NOT_FOUND",
"message": "Unit with id 999 was not found."
}
}

5.4 Codigos de respuesta usados
Codigo Cuando
200 Consulta exitosa
201 Recurso creado (POST de eventos)
404 Recurso no encontrado
409 Conflicto con regla de negocio (unidad scrapped)
500 Error interno (manejado por ASP.NET Core por defecto)

6. Decisiones tecnicas tomadas durante la implementacion
   6.1 Enums como numeros (temporal)
   Los enums ProductionOrderStatus, UnitStatus, ProductionEventType y ProductionEventResult se serializan como numeros enteros en las respuestas JSON.

Razon: es el comportamiento por defecto de System.Text.Json.

Cambio pendiente: agregar JsonStringEnumConverter en Program.cs para serializarlos como strings ("Pass", "Fail", etc). Se hara en una etapa posterior para mejorar la legibilidad del frontend.

6.2 Mapeo manual en lugar de AutoMapper
Todas las conversiones de entidad a DTO se hacen con .Select() en LINQ, sin AutoMapper ni perfiles.

Razon: el mapeo manual es explicito, no requiere dependencias extra, y permite al compilador de EF Core traducirlo a SQL. AutoMapper agrega magia y complejidad innecesaria para el tamano actual del proyecto.

6.3 AsNoTracking() en consultas de lectura
Todas las consultas GET usan .AsNoTracking().

Razon: sin tracking, EF Core no guarda las entidades en el ChangeTracker. Menos memoria, mas rapido. Es una buena practica en endpoints de solo lectura.

6.4 Proyeccion directa en LINQ
En lugar de cargar entidades completas y luego mapear en memoria, se hace .Select() directo para que EF Core genere un SELECT con solo las columnas necesarias.

Razon: menos datos viajan entre SQL Server y la API, menos alocaciones en memoria.

6.5 Filtros opcionales con [FromQuery] nullable
Los filtros de listado se reciben como parametros nullable (int?, UnitStatus?, etc.) y solo se aplican si tienen valor.

Razon: un solo endpoint cubre todos los casos de filtrado sin necesidad de endpoints separados.

6.6 No hay paginacion todavia
Los listados devuelven todos los resultados.

Razon: con datos ficticios pequenos no hace falta. Cuando el dataset crezca (o cuando el frontend lo requiera), se agrega paginacion con los parametros page y pageSize definidos en el doc 06.

Aviso: esto se debe implementar antes de que los endpoints se usen con volumenes reales.

6.7 ProductionRoute en lugar de Route
La entidad Route se renombro a ProductionRoute para evitar conflicto con Microsoft.AspNetCore.Routing.Route.

Razon: la clase Route de ASP.NET Core esta en el namespace implicitamente importado, y causaba ambiguedad en AppDbContext y controllers.

Nota: la tabla en la base de datos sigue llamandose Routes (ToTable("Routes")). El nombre solo cambio en C#.

7. Estado comparado con el diseno (doc 06)

Recurso Disenado Implementado Pendiente
Products GET, GET/{id}, POST, PUT, DELETE GET, GET/{id} POST, PUT, DELETE
Routes GET, GET/{id}, POST, PUT — Todos
ProductionLines GET, GET/{id}, POST, PUT GET, GET/{id} POST, PUT
Stations GET, GET/{id}, POST, PUT GET/{id} GET (listar), POST, PUT
ProductionOrders GET, GET/{id}, POST, PUT, PATCH GET, GET/{id} POST, PUT, PATCH
Units GET, GET/{id}, GET/{id}/history, POST, PATCH GET, GET/{id}, GET/{id}/history POST, PATCH
ProductionEvents GET, GET/{id}, POST GET, GET/{id}, POST —
Auth POST login, POST logout, GET me — Todos
Users GET, GET/{id}, POST, PUT, PATCH — Todos
Roles GET, POST, DELETE — Todos
Monitoring 3 endpoints — Todos
Reports 3 endpoints — Todos

8. Pendientes conocidos

Lista consolidada de cosas que quedaron fuera de esta etapa:

Paginacion en listados.
Serializacion de enums como string (JsonStringEnumConverter).
Autenticacion JWT y endpoints de login.
Autorizacion por rol en cada endpoint.
POST/PUT/DELETE de recursos de catalogo (Products, Lines, Stations, Routes).
POST de Units y PATCH de status de ordenes y unidades.
Validacion completa del flujo en POST de eventos (ruta, orden, rework).
Endpoint de registro de Routes (crear ruta con sus pasos).
Monitoring (dashboard, line status, units summary).
Reports (produccion, trazabilidad, fallas).
Manejo de errores global con middleware (por ahora cada controller devuelve el error directamente).
Logging estructurado con Serilog o similar.
