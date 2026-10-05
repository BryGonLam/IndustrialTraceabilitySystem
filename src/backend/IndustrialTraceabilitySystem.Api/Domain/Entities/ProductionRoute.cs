namespace IndustrialTraceabilitySystem.Api.Domain.Entities;

public class ProductionRoute
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
    public ICollection<RouteStep> Steps { get; set; } = new List<RouteStep>();
}