using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Domain.Entities;

public class Unit
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public int ProductionOrderId { get; set; }
    public UnitStatus Status { get; set; } = UnitStatus.Created;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ProductionOrder ProductionOrder { get; set; } = null!;
    public ICollection<ProductionEvent> ProductionEvents { get; set; } = new List<ProductionEvent>();
}