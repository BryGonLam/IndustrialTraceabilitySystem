02 — Requirements
Industrial Traceability System (ITS)
Estado: Borrador inicial
Version: 0.1.0
Documento previo: docs/01-project-overview.md

1. Purpose of this document

Este documento traduce la vision del proyecto en requisitos verificables. Define que debe hacer el sistema, sin definir todavia como se implementara.

Aqui no se diseñan tablas, endpoints, clases, capas, UI ni autenticacion. Esos temas se tratan en etapas posteriores.
Las decisiones tomadas para acotar el alcance son:
El dominio tendra flujo definido por producto (ruta de estaciones).
Los eventos de produccion serán append-only (no se editan ni se borran).
Monitoring y Reports quedan dentro del scope, pero marcados como Later Stage para no saturar la v1.

2. Requirement levels

Para evitar sobrecargar la primera version, cada requisito se etiqueta con un nivel:

MVP → debe estar en la primera version funcional.
Later Stage → dentro del scope, pero se implementa despues del MVP.
Future Scope → fuera de la v1, documentado por si se retoma.

3.1 Production
ID Requisito Nivel
FR-P-01 El sistema debe permitir registrar productos / modelos. MVP
FR-P-02 El sistema debe permitir definir una ruta de estaciones por producto. MVP
FR-P-03 El sistema debe permitir registrar lineas de produccion. MVP
FR-P-04 El sistema debe permitir registrar estaciones. MVP
FR-P-05 El sistema debe permitir crear ordenes de produccion asociadas a un producto y una linea. MVP
FR-P-06 El sistema debe permitir registrar unidades serializadas dentro de una orden de produccion. MVP
FR-P-07 El sistema debe permitir consultar ordenes de produccion y su estado. MVP
FR-P-08 El sistema debe permitir consultar unidades por orden, linea o producto. MVP

3.2 Traceability
ID Requisito Nivel
FR-T-01 El sistema debe registrar eventos de produccion asociados a una unidad. MVP
FR-T-02 Cada evento debe registrar al menos: unidad, estacion, tipo de evento, resultado y timestamp. MVP
FR-T-03 El sistema debe soportar los tipos de evento: START, STOP, PASS, FAIL, REWORK, SCRAP, HOLD. MVP
FR-T-04 El sistema debe reconstruir el historial completo de una unidad en orden cronologico. MVP
FR-T-05 El sistema debe determinar el estado actual de una unidad a partir de sus eventos. MVP
FR-T-06 Los eventos deben ser append-only: no se editan ni se eliminan. MVP
FR-T-07 El sistema debe impedir registrar eventos que violen las reglas de negocio del flujo. MVP
FR-T-08 El sistema debe permitir consultar unidades por resultado (PASS, FAIL, REWORK, SCRAP, HOLD). Later Stage

3.3 Users
ID Requisito Nivel
FR-U-01 El sistema debe permitir autenticar usuarios mediante credenciales. MVP
FR-U-02 El sistema debe emitir un token JWT tras autenticacion exitosa. MVP
FR-U-03 El sistema debe permitir registrar usuarios. MVP
FR-U-04 El sistema debe asignar roles a los usuarios. MVP
FR-U-05 El sistema debe restringir operaciones según el rol del usuario. MVP
FR-U-06 El sistema debe permitir cerrar sesion invalidando el token del lado del cliente. Later Stage

3.4 Monitoring
ID Requisito Nivel
FR-M-01 El sistema debe mostrar un dashboard con metricas basicas de produccion. Later Stage
FR-M-02 El sistema debe mostrar el estado actual de cada linea de produccion. Later Stage
FR-M-03 El sistema debe mostrar unidades en proceso, en falla, en retrabajo y descartadas. Later Stage

3.5 Reports
ID Requisito Nivel
FR-R-01 El sistema debe generar un reporte de produccion por orden. Later Stage
FR-R-02 El sistema debe generar un reporte de trazabilidad por unidad. Later Stage
FR-R-03 El sistema debe generar un reporte de fallas agrupadas por estacion y producto. Later Stage

4. Non-Functional Requirements (NFR)

ID Requisito Nivel
NFR-01 El backend debe exponer una API REST documentada. MVP
NFR-02 El sistema debe validar entradas en backend antes de persistir. MVP
NFR-03 Las contraseñas deben almacenarse con hashing seguro, nunca en texto plano. MVP
NFR-04 El sistema no debe contener credenciales reales ni connection strings reales en el repositorio. MVP
NFR-05 El codigo debe ser legible y explicable por el autor en una entrevista tecnica. MVP
NFR-06 El proyecto debe incluir documentacion tecnica en docs/. MVP
NFR-07 El proyecto debe incluir pruebas automatizadas en las partes que aporten valor. MVP
NFR-08 El sistema debe manejar errores de forma consistente (codigos HTTP y mensajes claros). MVP
NFR-09 El proyecto debe poder ejecutarse localmente con instrucciones claras en el README. MVP
NFR-10 El sistema debe priorizar comprension sobre complejidad; no se agregan tecnologias sin justificacion. MVP
NFR-11 El sistema debe ser mantenible: separacion razonable de responsabilidades. MVP
NFR-12 El sistema debe registrar timestamps en UTC. MVP

5. Business Rules (BR)

Estas reglas definen el comportamiento del dominio ficticio. No describen implementacion.

ID Regla
BR-01 Cada unidad debe pertenecer a una sola orden de produccion.
BR-02 Cada orden de produccion debe estar asociada a un producto y a una linea.
BR-03 Cada producto define una ruta de estaciones permitidas.
BR-04 Una unidad solo puede registrar eventos en estaciones pertenecientes a la ruta de su producto.
BR-05 Los eventos son append-only: no se editan ni se eliminan.
BR-06 Una unidad no puede registrar eventos despues de quedar en estado SCRAP.
BR-07 Una unidad en estado HOLD no puede continuar su flujo hasta que sea liberada.
BR-08 Un evento FAIL en una estacion de inspeccion habilita el paso a REWORK.
BR-09 Después de REWORK, la unidad debe volver a pasar por la estacioon que fallo.
BR-10 Una unidad no puede alcanzar el estado final PASS sin haber completado su ruta.
BR-11 El estado actual de una unidad se deriva del ultimo evento registrado y de las reglas del flujo.
BR-12 Una unidad no puede tener dos eventos simultaneos en la misma estacion con el mismo tipo.
Estas reglas podran refinarse cuando se diseñe el modelo de dominio. Aqui solo se declaran a nivel de negocio.

6. Assumptions

El dominio es ficticio; los nombres de productos, lineas, estaciones y unidades son inventados.
Existe un flujo tipico de manufactura electronica: Printer → SPI → AOI → Rework → Final Inspection.
Los usuarios operan el sistema desde una red interna (contexto simulado).
Los timestamps se manejan en UTC y se muestran en la zona local del cliente.
El sistema no se conecta a maquinas reales ni a PLCs; los eventos se registran manualmente o via API.
No hay requisitos legales ni regulatorios reales que cumplir; el proyecto es de portafolio.

7. Constraints

Heredadas del 01-project-overview.md:
No usar informacion propietaria de empresas reales.
No copiar codigo de sistemas existentes.
No incluir credenciales reales.
No incluir connection strings reales.
Toda la informacion de produccion es ficticia.
Los nombres de lineas, estaciones, modelos y productos son ficticios.
El codigo debe ser entendible por el autor.
No implementar caracteristicas que no puedan explicarse en una entrevista tecnica.

8. Traceability Matrix (ligera)

Mapeo entre modulos del overview y los FR definidos aqui.

Modulo del overview FR asociados
Production FR-P-01 … FR-P-08
Traceability FR-T-01 … FR-T-08
Users FR-U-01 … FR-U-06
Monitoring FR-M-01 … FR-M-03
Reports FR-R-01 … FR-R-03

9. MVP summary

Para que quede claro que constituye la primera version funcional:

MVP incluye:
Produccion: productos, rutas, lineas, estaciones, ordenes, unidades.
Trazabilidad: eventos append-only, historial, estado actual, validacion de reglas.
Usuarios: autenticacion, JWT, usuarios, roles, autorizacion.
NFRs base: REST, validaciones, hashing, documentación, tests, README.

Later Stage:
Consultas por resultado.
Dashboard y monitoreo de lineas.
Reportes de produccion, trazabilidad y fallas.
Logout con invalidacion de token.

Future Scope (fuera de v1):

Integraciones con maquinas o PLCs.
Analitica avanzada o ML.
Microservicios.
App movil nativa.
