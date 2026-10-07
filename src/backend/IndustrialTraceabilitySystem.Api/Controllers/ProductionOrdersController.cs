using IndustrialTraceabilitySystem.Api.Application.Dtos.ProductionOrders;
using IndustrialTraceabilitySystem.Api.Domain.Enums;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Controllers;

[ApiController]
[Route("api/v1/production-orders")]
public class ProductionOrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductionOrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductionOrderResponse>>> GetAll(
        [FromQuery] ProductionOrderStatus? status,
        [FromQuery] int? productId,
        [FromQuery] int? lineId)
    {
        var query = _context.ProductionOrders
            .AsNoTracking()
            .Include(o => o.Product)
            .Include(o => o.ProductionLine)
            .AsQueryable();

        if (status.HasValue) 
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(o => o.ProductId == productId.Value);
        }

        if (lineId.HasValue)
        {
            query = query.Where(o => o.ProductionLineId == lineId.Value);
        }

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new ProductionOrderResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                ProductId = o.ProductId,
                ProductCode = o.Product.Code,
                ProductName = o.Product.Name,
                ProductionLineId = o.ProductionLineId,
                ProductionLineCode = o.ProductionLine.Code,
                ProductionLineName = o.ProductionLine.Name,
                PlannedQuantity = o.PlannedQuantity,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                TotalUnits = o.Units.Count
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductionOrderDetailResponse>> GetById(int id)
    {
        var order = await _context.ProductionOrders
            .AsNoTracking()
            .Include(o => o.Product)
            .Include(o => o.ProductionLine)
            .Include(o => o.Units)
            .Where(o => o.Id == id)
            .Select(o => new ProductionOrderDetailResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                ProductId = o.ProductId,
                ProductCode = o.Product.Code,
                ProductName = o.Product.Name,
                ProductionLineId = o.ProductionLineId,
                ProductionLineCode = o.ProductionLine.Code,
                ProductionLineName = o.ProductionLine.Name,
                PlannedQuantity = o.PlannedQuantity,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                TotalUnits = o.Units.Count,
                Units = o.Units
                    .OrderBy(u => u.SerialNumber)
                    .Select(u => new UnitSummaryResponse
                    {
                        Id = u.Id,
                        SerialNumber = u.SerialNumber,
                        Status = u.Status,
                        CreatedAt = u.CreatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (order is null)
        {
            return NotFound(new { error = new { code = "ORDER_NOT_FOUND", message = $"Production order with id {id} was not found." } });
        }
        return Ok(order);
    }
}