namespace CaseTracker.Shared.Dtos;

public record CaseDto
{
    public int Id { get; init; }
    public string CaseNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Opponent { get; init; } = string.Empty;
    public DateTime? NextHearingDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
