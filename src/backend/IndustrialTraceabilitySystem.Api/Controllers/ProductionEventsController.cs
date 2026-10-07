using IndustrialTraceabilitySystem.Api.Application.Dtos.ProductionEvents;
using IndustrialTraceabilitySystem.Api.Domain.Entities;
using IndustrialTraceabilitySystem.Api.Domain.Enums;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Controllers;

[ApiController]
[Route("api/v1/production-events")]
public class ProductionEventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductionEventsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductionEventResponse>>> GetAll(
        [FromQuery] int? unitId,
        [FromQuery] int? stationId,
        [FromQuery] ProductionEventType? eventType,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var query = _context.ProductionEvents
            .AsNoTracking()
            .Include(e => e.Unit)
            .Include(e => e.Station)
            .Include(e => e.User)
            .AsQueryable();

        if (unitId.HasValue)
        {
            query = query.Where(e => e.UnitId == unitId.Value);
        }

        if (stationId.HasValue)
        {
            query = query.Where(e => e.StationId == stationId.Value);
        }

        if (eventType.HasValue)
        {
            query = query.Where(e => e.EventType == eventType.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(e => e.OccurredAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(e => e.OccurredAt <= dateTo.Value);
        }

        var events = await query
            .OrderByDescending(e => e.OccurredAt)
            .Select(e => new ProductionEventResponse
            {
                Id = e.Id,
                UnitId = e.UnitId,
                SerialNumber = e.Unit.SerialNumber,
                StationId = e.StationId,
                StationCode = e.Station.Code,
                StationName = e.Station.Name,
                UserId = e.UserId,
                Username = e.User != null ? e.User.Username : null,
                EventType = e.EventType,
                Result = e.Result,
                Notes = e.Notes,
                OccurredAt = e.OccurredAt,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductionEventResponse>> GetById(long id)
    {
        var evt = await _context.ProductionEvents
            .AsNoTracking()
            .Include(e => e.Unit)
            .Include(e => e.Station)
            .Include(e => e.User)
            .Where(e => e.Id == id)
            .Select(e => new ProductionEventResponse
            {
                Id = e.Id,
                UnitId = e.UnitId,
                SerialNumber = e.Unit.SerialNumber,
                StationId = e.StationId,
                StationCode = e.Station.Code,
                StationName = e.Station.Name,
                UserId = e.UserId,
                Username = e.User != null ? e.User.Username : null,
                EventType = e.EventType,
                Result = e.Result,
                Notes = e.Notes,
                OccurredAt = e.OccurredAt,
                CreatedAt = e.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (evt is null)
        {
            return NotFound(new { error = new { code = "EVENT_NOT_FOUND", message = $"Production event with id {id} was not found." } });
        }

        return Ok(evt);
    }

    [HttpPost]
    public async Task<ActionResult<ProductionEventResponse>> Create([FromBody] CreateProductionEventRequest request)
    {
        // [B. G. L.] 07/10/2026 1. Validar que la unidad existe
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == request.UnitId);
        if (unit is null)
        {
            return NotFound(new { error = new { code = "UNIT_NOT_FOUND", message = $"Unit with id {request.UnitId} was not found." } });
        }

        // [B. G. L.] 07/10/2026 2. Validar que la estacion existe
        var station = await _context.Stations.FirstOrDefaultAsync(s => s.Id == request.StationId);
        if (station is null)
        {
            return NotFound(new { error = new { code = "STATION_NOT_FOUND", message = $"Station with id {request.StationId} was not found." } });
        }

        // [B. G. L.] 07/10/2026 3. Regla de negocio: una unidad Scrapped no puede registrar mas eventos
        if (unit.Status == UnitStatus.Scrapped)
        {
            return Conflict(new { error = new { code = "UNIT_ALREADY_SCRAPPED", message = "The unit is scrapped and cannot register new events." } });
        }

        // [B. G. L.] 07/10/2026 4. Validar que el usuario existe (si lo mandan)
        if (request.UserId.HasValue)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId.Value);
            if (!userExists)
            {
                return NotFound(new { error = new { code = "USER_NOT_FOUND", message = $"User with id {request.UserId} was not found." } });
            }
        }

        // [B. G. L.] 07/10/2026 5. Crear el evento
        var productionEvent = new ProductionEvent
        {
            UnitId = request.UnitId,
            StationId = request.StationId,
            UserId = request.UserId,
            EventType = request.EventType,
            Result = request.Result,
            Notes = request.Notes,
            OccurredAt = request.OccurredAt ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductionEvents.Add(productionEvent);

        // [B. G. L.] 07/10/2026 6. Actualizar el estado de la unidad segun el evento
        UpdateUnitStatusFromEvent(unit, request);

        unit.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // [B. G. L.] 07/10/2026 7. Recargar relaciones para devolver el response completo
        var response = await _context.ProductionEvents
            .AsNoTracking()
            .Include(e => e.Unit)
            .Include(e => e.Station)
            .Include(e => e.User)
            .Where(e => e.Id == productionEvent.Id)
            .Select(e => new ProductionEventResponse
            {
                Id = e.Id,
                UnitId = e.UnitId,
                SerialNumber = e.Unit.SerialNumber,
                StationId = e.StationId,
                StationCode = e.Station.Code,
                StationName = e.Station.Name,
                UserId = e.UserId,
                Username = e.User != null ? e.User.Username : null,
                EventType = e.EventType,
                Result = e.Result,
                Notes = e.Notes,
                OccurredAt = e.OccurredAt,
                CreatedAt = e.CreatedAt
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    private static void UpdateUnitStatusFromEvent(Unit unit, CreateProductionEventRequest request)
    {
        switch (request.EventType)
        {
            case ProductionEventType.Scrap:
                unit.Status = UnitStatus.Scrapped;
                break;

            case ProductionEventType.Hold:
                unit.Status = UnitStatus.OnHold;
                break;

            case ProductionEventType.Rework:
                unit.Status = UnitStatus.InRework;
                break;

            case ProductionEventType.Pass:
                // [B. G. L.] 07/10/2026 Solo marcamos como Passed si es un PASS en la estacion final.
                // [B. G. L.] 07/10/2026 Por ahora, si es PASS simplemente dejamos InProgress.
                // [B. G. L.] 07/10/2026 La deteccion de "estacion final" se implementa en una etapa posterior.
                if (unit.Status != UnitStatus.InRework)
                {
                    unit.Status = UnitStatus.InProgress;
                }
                else
                {
                    // [B. G. L.] 07/10/2026 Si estaba en rework y ahora pasa, pasa a InProgress otra vez
                    unit.Status = UnitStatus.InProgress;
                }
                break;

            case ProductionEventType.Fail:
                unit.Status = UnitStatus.InRework;
                break;

            case ProductionEventType.Start:
            case ProductionEventType.Stop:
            default:
                // [B. G. L.] 07/10/2026 No cambia el estado de la unidad
                break;
        }
    }
}