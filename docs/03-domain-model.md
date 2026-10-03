03 — Domain Model
Industrial Traceability System (ITS)
Estado: Borrador inicial
Version: 0.1.0
Documento previo: docs/02-requirements.md

1. Proposito de este documento

Este documento traduce los requisitos funcionales en un modelo conceptual de dominio. Define las entidades principales, sus responsabilidades y las relaciones entre ellas.

No se diseñan todavia:
tablas
columnas
tipos de datos
endpoints
clases del backend
UI

El objetivo es tener un vocabulario comun antes de pasar a arquitectura y base de datos.

2. Idea central del dominio

El sistema gira alrededor de un concepto: la unidad trazable.

Una unidad es un objeto serializado (por ejemplo, un PCB ficticio) que:
pertenece a una orden de produccion
sigue una ruta de estaciones definida por su producto
acumula eventos a lo largo de su vida
tiene un estado actual derivado de esos eventos

Todo lo demas en el sistema existe para dar contexto a esa unidad: productos, rutas, lineas, estaciones, ordenes, usuarios y eventos.
Ejemplo:
Orden: OP-2026-0001
Producto: ControlBoard-A
Linea: Linea-SMT-01

Unidad: PCB000001

Printer -> PASS
SPI -> PASS
AOI -> FAIL
Rework -> COMPLETE
AOI -> PASS
Final Inspection -> PASS

3. Entidades del dominio

A continuacion se describen las entidades conceptuales. Cada una se presenta con: proposito, responsabilidad y relaciones principales.

3.1 Product
Proposito: representar un modelo o tipo de producto fabricable.

Responsabilidad:
definir la identidad del producto (codigo, nombre, descripcion)
definir la ruta de estaciones que deben seguir sus unidades

Relaciones:
un Product tiene muchas Routes (o una Route asociada, segun se decida en el modelo detallado)
un Product puede aparecer en muchas Production Orders
un Product se asocia indirectamente a muchas Units a traves de sus ordenes

3.2 Route

Proposito: representar la secuencia ordenada de estaciones que un producto debe seguir.

Responsabilidad:
definir el orden de las estaciones
definir que estaciones son obligatorias
permitir reglas de negocio basadas en la posicion dentro de la ruta (por ejemplo, Rework vuelve a la estacion que fallo)

Relaciones:
una Route pertenece a un Product
una Route contiene muchas Route Steps
cada Route Step referencia una Station

Nota: Route existe como entidad separada para evitar mezclar "producto" con "flujo". Es una decision de claridad, no de complejidad.

3.3 Route Step

Proposito: representar un paso dentro de una ruta.

Responsabilidad:
indicar el orden del paso
referenciar la estacion correspondiente
indicar si el paso es obligatorio

Relaciones:
un Route Step pertenece a una Route
un Route Step referencia una Station

3.4 Production Line

Proposito: representar una linea fisica de produccion.

Responsabilidad:
agrupar estaciones
ser el contexto fisico donde se ejecutan las ordenes

Relaciones:
una Production Line contiene muchas Stations
una Production Line puede tener muchas Production Orders a lo largo del tiempo

3.5 Station

Proposito: representar un punto de trabajo dentro de una linea.

Responsabilidad:
registrar eventos de produccion
pertenecer a una linea
aparecer en una o varias rutas

Relaciones:
una Station pertenece a una Production Line
una Station puede aparecer en muchos Route Steps
una Station acumula muchos Production Events

3.6 Production Order

Proposito: representar una corrida de produccion para un producto en una linea.

Responsabilidad:
agrupar unidades producidas bajo la misma corrida
tener un estado (planificada, en progreso, cerrada, cancelada)
tener una fecha de creacion y una cantidad planificada

Relaciones:
una Production Order referencia un Product
una Production Order referencia una Production Line
una Production Order contiene muchas Units

3.7 Unit

Proposito: representar la unidad trazable. Es la entidad central del sistema.

Responsabilidad:
tener un identificador unico (serial)
pertenecer a una orden de produccion
acumular Production Events
tener un estado actual derivado de sus eventos
seguir la ruta definida por el producto de su orden

Relaciones:
una Unit pertenece a una Production Order
una Unit acumula muchos Production Events (append-only)
una Unit sigue indirectamente la Route de su Product

Regla clave: el estado de una Unit no se guarda como fuente de verdad; se deriva de sus eventos. Esto puede materializarse despues por rendimiento, pero conceptualmente es derivado.

3.8 Production Event

Proposito: representar un hecho ocurrido sobre una unidad en una estacion.

Responsabilidad:
registrar que paso, donde, cuando y con que resultado
ser inmutable (append-only)
servir como base para reconstruir el historial de una unidad

Atributos conceptuales minimos:
Unit referenciada
Station referenciada
tipo de evento (START, STOP, PASS, FAIL, REWORK, SCRAP, HOLD)
timestamp
usuario que lo registro (opcional en el modelo conceptual, obligatorio en el modelo detallado)

Relaciones:
un Production Event pertenece a una Unit
un Production Event ocurre en una Station
un Production Event fue registrado por un User (a definir en el modelo detallado)

Regla clave: los eventos no se editan ni se eliminan. Esto aplica a nivel de dominio, no solo de base de datos.

3.9 User

Proposito: representar a una persona que opera el sistema.

Responsabilidad:
autenticarse
registrar eventos
consultar informacion segun su rol

Relaciones:
un User tiene uno o varios Roles
un User puede registrar muchos Production Events (en el modelo detallado)

3.10 Role

Proposito: representar un conjunto de permisos.

Responsabilidad:
agrupar permisos
determinar que puede hacer un usuario

Relaciones:
un Role se asigna a muchos Users
un Role agrupa permisos (a detallar en la etapa de autenticacion)

Relaciones principales (resumen)
Product 1 --- N Route
Route 1 --- N Route Step
Route Step N --- 1 Station

Production Line 1 --- N Station
Production Line 1 --- N Production Order

Production Order N --- 1 Product
Production Order N --- 1 Production Line
Production Order 1 --- N Unit

Unit 1 --- N Production Event
Production Event N --- 1 Station
Production Event N --- 1 User (a detallar)

User N --- N Role

5. Estados conceptuales de una Unit

El estado de una Unit se deriva de sus eventos y de las reglas de negocio. A nivel conceptual se manejan los siguientes estados:

Estado Significado
Created Unidad registrada, sin eventos aun.
In Progress Unidad con eventos activos, avanzando por la ruta.
On Hold Unidad detenida temporalmente.
In Rework Unidad enviada a retrabajo tras un FAIL.
Passed Unidad completo su ruta con resultado final PASS.
Scrapped Unidad descartada. No puede seguir registrando eventos.
Nota: estos estados no son una tabla. Son una abstraccion del dominio. Su calculo exacto se definira en el modelo detallado.

6. Reglas de dominio relevantes

Estas reglas provienen del documento de requisitos y se reflejan directamente en el modelo conceptual:
Cada Unit pertenece a una sola Production Order.
Cada Production Order esta asociada a un Product y a una Production Line.
Cada Product define una Route.
Una Unit solo puede registrar eventos en Stations pertenecientes a la Route de su Product.
Los Production Events son append-only.
Una Unit en estado Scrapped no puede registrar nuevos eventos.
Una Unit en estado On Hold no puede avanzar hasta ser liberada.
Un FAIL en una estacion de inspeccion habilita el paso a Rework.
Despues de Rework, la Unit debe volver a pasar por la estacion que fallo.
Una Unit no puede llegar a Passed sin haber completado su ruta.

Estas reglas se refinaran al disenar el modelo detallado y la base de datos.

7. Fuera de este documento

Este documento no cubre:
tablas ni columnas
tipos de datos
indices
endpoints
DTOs
clases del backend
componentes de UI
detalle de autenticacion y permisos
estrategia de migraciones
testing

Todo eso se trata en etapas posteriores.
