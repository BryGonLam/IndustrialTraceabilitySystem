namespace IndustrialTraceabilitySystem.Api.Application.Dtos.Stations;

public class StationResponse
{
    public int Id { get; set; }
    public int ProductionLineId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}