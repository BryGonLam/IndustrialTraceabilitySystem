using static System.Collections.Specialized.BitVector32;

namespace IndustrialTraceabilitySystem.Api.Domain.Entities;

public class ProductionLine
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Station> Stations { get; set; } = new List<Station>();
    public ICollection<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();
}