using IndustrialTraceabilitySystem.Api.Application.Dtos.Products;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
    {
        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Code)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return NotFound(new { error = new { code = "PRODUCT_NOT_FOUND", message = $"Product with id {id} was not found." } });
        }

        return Ok(product);
    }
}