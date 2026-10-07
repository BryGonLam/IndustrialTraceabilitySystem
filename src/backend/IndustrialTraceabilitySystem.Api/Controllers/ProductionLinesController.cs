using IndustrialTraceabilitySystem.Api.Application.Dtos.ProductionLines;
using IndustrialTraceabilitySystem.Api.Application.Dtos.Stations;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Controllers;

[ApiController]
[Route("api/v1/production-lines")]
public class ProductionLinesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductionLinesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductionLineResponse>>> GetAll()
    {
        var lines = await _context.ProductionLines
            .AsNoTracking()
            .OrderBy(l => l.Code)
            .Select(l => new ProductionLineResponse
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                IsActive = l.IsActive,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Ok(lines);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductionLineResponse>> GetById(int id)
    {
        var line = await _context.ProductionLines
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new ProductionLineResponse
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                IsActive = l.IsActive,
                CreatedAt = l.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (line is null)
        {
            return NotFound(new { error = new { code = "LINE_NOT_FOUND", message = $"Production line with id {id} was not found." } });
        }

        return Ok(line);
    }

    [HttpGet("{id:int}/stations")]
    public async Task<ActionResult<IEnumerable<StationResponse>>> GetStations(int id)
    {
        var lineExists = await _context.ProductionLines.AnyAsync(l => l.Id == id);
        if (!lineExists)
        {
            return NotFound(new { error = new { code = "LINE_NOT_FOUND", message = $"Production line with id {id} was not found." } });
        }

        var stations = await _context.Stations
            .AsNoTracking()
            .Where(s => s.ProductionLineId == id)
            .OrderBy(s => s.Code)
            .Select(s => new StationResponse
            {
                Id = s.Id,
                ProductionLineId = s.ProductionLineId,
                Code = s.Code,
                Name = s.Name,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return Ok(stations);
    }
}