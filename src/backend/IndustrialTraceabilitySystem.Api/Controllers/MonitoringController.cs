using IndustrialTraceabilitySystem.Api.Application.Dtos.Monitoring;
using IndustrialTraceabilitySystem.Api.Domain.Enums;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Controllers;

[ApiController]
[Route("api/v1/monitoring")]
public class MonitoringController : ControllerBase
{
    private readonly AppDbContext _context;

    public MonitoringController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardResponse>> GetDashboard()
    {
        var response = new DashboardResponse
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalLines = await _context.ProductionLines.CountAsync(),
            TotalStations = await _context.Stations.CountAsync(),
            TotalOrders = await _context.ProductionOrders.CountAsync(),
            ActiveOrders = await _context.ProductionOrders
                .CountAsync(o => o.Status == ProductionOrderStatus.InProgress),
            TotalUnits = await _context.Units.CountAsync(),
            TotalEvents = await _context.ProductionEvents.CountAsync(),
            GeneratedAt = DateTime.UtcNow
        };

        return Ok(response);
    }

    [HttpGet("lines")]
    public async Task<ActionResult<IEnumerable<LineStatusResponse>>> GetLinesStatus()
    {
        var lines = await _context.ProductionLines
            .AsNoTracking()
            .OrderBy(l => l.Code)
            .Select(l => new LineStatusResponse
            {
                LineId = l.Id,
                LineCode = l.Code,
                LineName = l.Name,
                TotalStations = l.Stations.Count,
                ActiveOrders = l.ProductionOrders
                    .Count(o => o.Status == ProductionOrderStatus.InProgress),
                TotalUnits = l.ProductionOrders
                    .SelectMany(o => o.Units)
                    .Count()
            })
            .ToListAsync();

        return Ok(lines);
    }

    [HttpGet("units-summary")]
    public async Task<ActionResult<UnitsSummaryResponse>> GetUnitsSummary()
    {
        var statusCounts = await _context.Units
            .AsNoTracking()
            .GroupBy(u => u.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var eventsByType = await _context.ProductionEvents
            .AsNoTracking()
            .GroupBy(e => e.EventType)
            .Select(g => new { EventType = g.Key, Count = g.Count() })
            .ToListAsync();

        var eventsByStation = await _context.ProductionEvents
            .AsNoTracking()
            .GroupBy(e => new { e.StationId, e.Station.Code, e.Station.Name })
            .Select(g => new UnitCountByStation
            {
                StationId = g.Key.StationId,
                StationCode = g.Key.Code,
                StationName = g.Key.Name,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        var response = new UnitsSummaryResponse
        {
            Total = statusCounts.Sum(x => x.Count),
            Created = statusCounts.FirstOrDefault(x => x.Status == UnitStatus.Created)?.Count ?? 0,
            InProgress = statusCounts.FirstOrDefault(x => x.Status == UnitStatus.InProgress)?.Count ?? 0,
            OnHold = statusCounts.FirstOrDefault(x => x.Status == UnitStatus.OnHold)?.Count ?? 0,
            InRework = statusCounts.FirstOrDefault(x => x.Status == UnitStatus.InRework)?.Count ?? 0,
            Passed = statusCounts.FirstOrDefault(x => x.Status == UnitStatus.Passed)?.Count ?? 0,
            Scrapped = statusCounts.FirstOrDefault(x => x.Status == UnitStatus.Scrapped)?.Count ?? 0,
            EventsByType = eventsByType
                .Select(x => new UnitCountByEventType
                {
                    EventType = x.EventType.ToString(),
                    Count = x.Count
                })
                .OrderByDescending(x => x.Count)
                .ToList(),
            EventsByStation = eventsByStation
        };

        return Ok(response);
    }
}