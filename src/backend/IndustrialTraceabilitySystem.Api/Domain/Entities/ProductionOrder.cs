using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Domain.Entities;

public class ProductionOrder
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int ProductionLineId { get; set; }
    public int PlannedQuantity { get; set; }
    public ProductionOrderStatus Status { get; set; } = ProductionOrderStatus.Planned;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
    public ProductionLine ProductionLine { get; set; } = null!;
    public ICollection<Unit> Units { get; set; } = new List<Unit>();
}