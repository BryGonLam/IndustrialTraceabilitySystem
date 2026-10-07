using IndustrialTraceabilitySystem.Api.Application.Dtos.Units;
using IndustrialTraceabilitySystem.Api.Domain.Enums;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Controllers;

[ApiController]
[Route("api/v1/units")]
public class UnitsController : ControllerBase
{
    private readonly AppDbContext _context;

    public UnitsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitResponse>>> GetAll(
        [FromQuery] int? orderId,
        [FromQuery] UnitStatus? status,
        [FromQuery] string? serial)
    {
        var query = _context.Units
            .AsNoTracking()
            .Include(u => u.ProductionOrder)
            .AsQueryable();

        if (orderId.HasValue)
        {
            query = query.Where(u => u.ProductionOrderId == orderId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(u => u.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(serial))
        {
            query = query.Where(u => u.SerialNumber.Contains(serial));
        }

        var units = await query
            .OrderBy(u => u.SerialNumber)
            .Select(u => new UnitResponse
            {
                Id = u.Id,
                SerialNumber = u.SerialNumber,
                ProductionOrderId = u.ProductionOrderId,
                OrderNumber = u.ProductionOrder.OrderNumber,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
            })
            .ToListAsync();

        return Ok(units);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnitDetailResponse>> GetById(int id)
    {
        var unit = await _context.Units
            .AsNoTracking()
            .Include(u => u.ProductionOrder)
                .ThenInclude(o => o.Product)
            .Where(u => u.Id == id)
            .Select(u => new UnitDetailResponse
            {
                Id = u.Id,
                SerialNumber = u.SerialNumber,
                ProductionOrderId = u.ProductionOrderId,
                OrderNumber= u.ProductionOrder.OrderNumber,
                ProductId = u.ProductionOrder.ProductId,
                ProductCode = u.ProductionOrder.Product.Code,
                ProductName = u.ProductionOrder.Product.Name,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (unit is null)
        {
            return NotFound(new { error = new { code = "UNIT_NOT_FOUND", message = $"Unit with id {id} was not found." } });
        }

        return Ok(unit);
    }

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<UnitHistoryResponse>> GetHistory(int id)
    {
        var unit = await _context.Units
            .AsNoTracking()
            .Where (u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.SerialNumber,
                u.Status,
            })
            .FirstOrDefaultAsync();

        if (unit is null) 
        { 
            return NotFound(new { error = new { code = "UNIT_NOT_FOUND", message = $"Unit with id {id} was not found." } }); 
        }

        var events = await _context.ProductionEvents
            .AsNoTracking()
            .Include(e => e.Station)
            .Where(e => e.UnitId == id)
            .OrderBy(e => e.OccurredAt)
            .Select(e => new UnitHistoryEventResponse
            {
                Id = e.Id,
                StationId = e.StationId,
                StationCode = e.Station.Code,
                StationName = e.Station.Name,
                EventType = e.EventType,
                Result = e.Result,
                Notes = e.Notes,
                OccurredAt = e.OccurredAt,
            })
            .ToListAsync();

        var response = new UnitHistoryResponse
        {
            UnitId = unit.Id,
            SerialNumber = unit.SerialNumber,
            CurrentStatus = unit.Status,
            Events = events,
        };

        return Ok(response);
    }
}   