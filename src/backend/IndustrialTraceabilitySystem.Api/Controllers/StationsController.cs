using IndustrialTraceabilitySystem.Api.Application.Dtos.Stations;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Controllers;

[ApiController]
[Route("api/v1/stations")]
public class StationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StationResponse>> GetById(int id)
    {
        var station = await _context.Stations
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new StationResponse
            {
                Id = s.Id,
                ProductionLineId = s.ProductionLineId,
                Code = s.Code,
                Name = s.Name,
                IsActive = s.IsActive, 
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (station is null)
        {
            return NotFound(new { error = new { code = "STATION_NOT_FOUND", message = $"Station with id {id} was not found." } });
        }

        return Ok(station);
    }
}