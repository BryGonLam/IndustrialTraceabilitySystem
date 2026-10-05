06 — API Design
Industrial Traceability System (ITS)
Estado: Borrador inicial
Version: 0.1.0
Documento previo: docs/05-database-design.md

1. Proposito de este documento

Este documento define el diseno de la API REST del sistema: recursos, rutas, metodos HTTP, codigos de respuesta y estructura general de DTOs.

No se define todavia:
codigo de controladores
clases concretas de servicios
implementacion de repositorios
componentes de UI
detalles de JWT

El objetivo es tener un contrato claro antes de escribir el backend.

2. Principios de diseno de la API

La API es REST y usa JSON como formato.
Las rutas son en plural y en minusculas: /api/products, /api/units.
Los recursos siguen el modelo de dominio (03-domain-model.md).
Los identificadores expuestos son los Id internos del modelo relacional.
Los endpoints se versionan desde el inicio con prefijo /api/v1.
Los errores se devuelven con estructura JSON consistente.
Los DTOs de entrada y salida se separan de las entidades del dominio.
La autenticacion usa JWT en el header Authorization: Bearer <token>.

3. Convenciones generales

3.1 Versionado
/api/v1/<recurso>

3.2 Metodos HTTP usados
Metodo Uso
GET Consultar recursos
POST Crear recursos
PUT Actualizar recursos completos
PATCH Actualizar parcialmente (uso limitado)
DELETE No se usa en recursos append-only ni catalogos (se usa IsActive = 0)

3.3 Codigos de respuesta
Codigo Significado
200 OK
201 Created
204 No Content
400 Bad Request (validacion)
401 Unauthorized (sin token o token invalido)
403 Forbidden (sin permisos)
404 Not Found
409 Conflict (regla de negocio violada)
422 Unprocessable Entity (regla de dominio)
500 Internal Server Error

3.4 Estructura de error
{
"error": {
"code": "UNIT_ALREADY_SCRAPPED",
"message": "La unidad no puede registrar eventos porque esta en estado Scrapped.",
"details": null
}
}

3.5 Paginacion
Para listados grandes se usa paginacion por query string:
?page=1&pageSize=20

Respuesta con metadatos:
{
"items": [ ... ],
"page": 1,
"pageSize": 20,
"totalItems": 134,
"totalPages": 7
}

4. Recursos de la API

A alto nivel, la API expone estos recursos:
/api/v1/products
/api/v1/routes
/api/v1/production-lines
/api/v1/stations
/api/v1/production-orders
/api/v1/units
/api/v1/production-events
/api/v1/users
/api/v1/roles
/api/v1/auth

5. Endpoints — Production

5.1 Products
Metodo Ruta Descripcion Auth
GET /api/v1/products Lista productos Si
GET /api/v1/products/{id} Obtiene un producto Si
POST /api/v1/products Crea un producto Si (Admin, Supervisor)
PUT /api/v1/products/{id} Actualiza un producto Si (Admin, Supervisor)
DELETE /api/v1/products/{id} Marca IsActive = 0 Si (Admin)
5.2 Routes
Metodo Ruta Descripcion Auth
GET /api/v1/products/{productId}/routes Lista rutas de un producto Si
GET /api/v1/routes/{id} Obtiene una ruta con sus pasos Si
POST /api/v1/products/{productId}/routes Crea una ruta Si (Admin, Supervisor)
PUT /api/v1/routes/{id} Actualiza una ruta Si (Admin, Supervisor)
Nota: los pasos de la ruta se manejan como parte del recurso Route, no como recurso independiente en v1.

5.3 Production Lines
Metodo Ruta Descripcion Auth
GET /api/v1/production-lines Lista lineas Si
GET /api/v1/production-lines/{id} Obtiene una linea Si
POST /api/v1/production-lines Crea una linea Si (Admin)
PUT /api/v1/production-lines/{id} Actualiza una linea Si (Admin)
5.4 Stations
Metodo Ruta Descripcion Auth
GET /api/v1/production-lines/{lineId}/stations Lista estaciones de una linea Si
GET /api/v1/stations/{id} Obtiene una estacion Si
POST /api/v1/production-lines/{lineId}/stations Crea una estacion Si (Admin)
PUT /api/v1/stations/{id} Actualiza una estacion Si (Admin)
5.5 Production Orders
Metodo Ruta Descripcion Auth
GET /api/v1/production-orders Lista ordenes (filtros: status, productId, lineId) Si
GET /api/v1/production-orders/{id} Obtiene una orden Si
POST /api/v1/production-orders Crea una orden Si (Admin, Supervisor)
PUT /api/v1/production-orders/{id} Actualiza una orden Si (Admin, Supervisor)
PATCH /api/v1/production-orders/{id}/status Cambia el estado de la orden Si (Admin, Supervisor)
5.6 Units
Metodo Ruta Descripcion Auth
GET /api/v1/units Lista unidades (filtros: orderId, status, serial) Si
GET /api/v1/units/{id} Obtiene una unidad Si
GET /api/v1/units/{id}/history Obtiene el historial de eventos de la unidad Si
POST /api/v1/units Registra una unidad en una orden Si (Admin, Supervisor, Operator)
PATCH /api/v1/units/{id}/status Fuerza cambio de estado (solo Admin, con justificacion) Si (Admin)
Nota: el estado de la unidad normalmente se deriva de los eventos. El PATCH de estado existe solo como operacion administrativa excepcional.

6. Endpoints — Traceability

6.1 Production Events
Metodo Ruta Descripcion Auth
GET /api/v1/production-events Lista eventos (filtros: unitId, stationId, eventType, dateFrom, dateTo) Si
GET /api/v1/production-events/{id} Obtiene un evento Si
POST /api/v1/production-events Registra un evento Si (Admin, Supervisor, Operator)
Nota importante: no existen PUT, PATCH ni DELETE para production-events. La tabla es append-only.

Ejemplo de cuerpo para POST:
{
"unitId": 1,
"stationId": 3,
"eventType": "FAIL",
"result": "FAIL",
"notes": "Falla detectada en inspeccion optica."
}

7. Endpoints — Users y Auth

7.1 Auth
Metodo Ruta Descripcion Auth
POST /api/v1/auth/login Autentica y devuelve JWT No
POST /api/v1/auth/logout Cierra sesion (cliente descarta token) Si
GET /api/v1/auth/me Devuelve info del usuario autenticado Si
7.2 Users
Metodo Ruta Descripcion Auth
GET /api/v1/users Lista usuarios Si (Admin)
GET /api/v1/users/{id} Obtiene un usuario Si (Admin)
POST /api/v1/users Crea un usuario Si (Admin)
PUT /api/v1/users/{id} Actualiza un usuario Si (Admin)
PATCH /api/v1/users/{id}/status Activa o desactiva un usuario Si (Admin)
7.3 Roles
Metodo Ruta Descripcion Auth
GET /api/v1/roles Lista roles Si (Admin)
POST /api/v1/users/{userId}/roles Asigna un rol a un usuario Si (Admin)
DELETE /api/v1/users/{userId}/roles/{roleId} Quita un rol a un usuario Si (Admin)
Nota: aqui si se usa DELETE porque UserRoles no es append-only, es una tabla de asignacion.

8. Endpoints — Monitoring y Reports

Estos endpoints se implementan en la etapa de Monitoring y Reports.

8.1 Monitoring
Metodo Ruta Descripcion Auth
GET /api/v1/monitoring/dashboard Metricas generales de produccion Si
GET /api/v1/monitoring/lines Estado actual de las lineas Si
GET /api/v1/monitoring/units-summary Resumen de unidades por estado Si
8.2 Reports
Metodo Ruta Descripcion Auth
GET /api/v1/reports/production Reporte de produccion por orden Si
GET /api/v1/reports/traceability/{unitId} Reporte de trazabilidad por unidad Si
GET /api/v1/reports/failures Reporte de fallas por estacion y producto Si 9. DTOs — lineamientos generales
Los DTOs se separan por uso:

Request DTOs: lo que el cliente envia.
Response DTOs: lo que la API devuelve.
List DTOs: version reducida para listados.
Reglas:
No se exponen entidades del dominio directamente.
No se exponen campos internos como PasswordHash.
Los DTOs de request no incluyen Id ni CreatedAt (los genera el servidor).
Los DTOs de response si incluyen Id y CreatedAt cuando aplica.
Ejemplo conceptual (no codigo):

ProductResponse

- id
- code
- name
- description
- isActive
- createdAt

ProductCreateRequest

- code
- name
- description

UnitHistoryResponse

- unitId
- serialNumber
- currentStatus
- events: [ ProductionEventResponse ]

10. Autenticacion y autorizacion

Todos los endpoints requieren JWT, excepto POST /api/v1/auth/login.
El token se envia en el header Authorization: Bearer <token>.
La autorizacion por rol se verifica en el backend, no en el frontend.
El detalle de claims, expiracion y roles se define en la etapa de Auth.
Roles y permisos a alto nivel:
Rol Permisos generales
Admin Todo
Supervisor Produccion, ordenes, unidades, eventos
Operator Registrar eventos, consultar unidades
Quality Consultar, registrar eventos de inspeccion
