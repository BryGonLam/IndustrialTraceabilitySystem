using static System.Collections.Specialized.BitVector32;

namespace IndustrialTraceabilitySystem.Api.Domain.Entities;

public class RouteStep
{
    public int Id { get; set; }
    public int RouteId { get; set; }
    public int StationId { get; set; }
    public int StepOrder { get; set; }
    public bool IsMandatory { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ProductionRoute Route { get; set; } = null!;
    public Station Station { get; set; } = null!;
}