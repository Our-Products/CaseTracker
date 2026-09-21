using System;

namespace CaseTrackerApplication.DTOs.Hearings
{
    public class HearingDto
    {
        public Guid HearingId { get; set; }
        public Guid CaseId { get; set; }
        public string? CaseNumber { get; set; }
        public string? CaseTitle { get; set; }
        public DateTime HearingDate { get; set; }
        public int? ItemNumber { get; set; }
        public string? CourtHall { get; set; }
        public string? JudgeName { get; set; }
        public string PurposeOfHearing { get; set; } = null!;
        public string? BusinessOnDate { get; set; }
        public DateTime? NextHearingDate { get; set; }
        public string? NextPurpose { get; set; }
        public string HearingStatus { get; set; } = "Scheduled";
        public string? DailyOrderSummary { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class CreateHearingRequest
    {
        public Guid CaseId { get; set; }
        public DateTime HearingDate { get; set; }
        public int? ItemNumber { get; set; }
        public string? CourtHall { get; set; }
        public string? JudgeName { get; set; }
        public string PurposeOfHearing { get; set; } = null!;
        public string? BusinessOnDate { get; set; }
        public DateTime? NextHearingDate { get; set; }
        public string? NextPurpose { get; set; }
        public string HearingStatus { get; set; } = "Scheduled";
        public string? DailyOrderSummary { get; set; }
    }

    public class UpdateHearingRequest
    {
        public DateTime HearingDate { get; set; }
        public int? ItemNumber { get; set; }
        public string? CourtHall { get; set; }
        public string? JudgeName { get; set; }
        public string PurposeOfHearing { get; set; } = null!;
        public string? BusinessOnDate { get; set; }
        public DateTime? NextHearingDate { get; set; }
        public string? NextPurpose { get; set; }
        public string HearingStatus { get; set; } = "Scheduled";
        public string? DailyOrderSummary { get; set; }
    }
}
