using System;

namespace CaseTrackerApplication.DTOs.Courts
{
    public class StateDto
    {
        public Guid StateId { get; set; }
        public string StateName { get; set; } = null!;
        public string StateCode { get; set; } = null!;
        public string Status { get; set; } = "Active";
    }

    public class DistrictDto
    {
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; } = null!;
        public Guid StateId { get; set; }
        public string? DistrictCode { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class CourtComplexDto
    {
        public Guid CourtComplexId { get; set; }
        public string ComplexName { get; set; } = null!;
        public Guid DistrictId { get; set; }
        public string? ComplexCode { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class CourtDto
    {
        public Guid CourtId { get; set; }
        public string CourtName { get; set; } = null!;
        public string? CourtCode { get; set; }
        public Guid CourtComplexId { get; set; }
        public string CourtTypeId { get; set; } = null!;
        public string? ComplexName { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class SyncEntityStats
    {
        public int Received { get; set; }
        public int Inserted { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
    }

    public class CourtMasterSyncExecutionSummaryDto
    {
        public List<string> States { get; set; } = new();
        public SyncEntityStats Districts { get; set; } = new();
        public SyncEntityStats CourtComplexes { get; set; } = new();
        public SyncEntityStats Courts { get; set; } = new();
        public int ApiRequests { get; set; }
        public string Status { get; set; } = "Completed"; // "Completed", "Partial", "Failed"
        public string? ErrorMessage { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset CompletedAt { get; set; }
        public long DurationMs { get; set; }

        public string ToFormattedSummary()
        {
            return $"Court Master Sync\n\n" +
                   $"States:\n{string.Join(", ", States)}\n\n" +
                   $"Districts:\nReceived: {Districts.Received}\nInserted: {Districts.Inserted}\nUpdated: {Districts.Updated}\nSkipped: {Districts.Skipped}\nFailed: {Districts.Failed}\n\n" +
                   $"Court Complexes:\nReceived: {CourtComplexes.Received}\nInserted: {CourtComplexes.Inserted}\nUpdated: {CourtComplexes.Updated}\nSkipped: {CourtComplexes.Skipped}\nFailed: {CourtComplexes.Failed}\n\n" +
                   $"Courts:\nReceived: {Courts.Received}\nInserted: {Courts.Inserted}\nUpdated: {Courts.Updated}\nSkipped: {Courts.Skipped}\nFailed: {Courts.Failed}\n\n" +
                   $"API Requests:\n{ApiRequests}\n\n" +
                   $"Status:\n{Status}\n" +
                   (string.IsNullOrWhiteSpace(ErrorMessage) ? "" : $"Error: {ErrorMessage}\n") +
                   $"Duration: {DurationMs}ms";
        }
    }

    public class CourtMasterSyncSettings
    {
        public bool Enabled { get; set; } = true;
        public List<string> TargetStates { get; set; } = new() { "TN", "PY" };
        public int MaxApiRequestsPerRun { get; set; } = 500;
        public int RetryCount { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 2;
        public string MonthlySchedule { get; set; } = "0 0 2 1 * *"; // 1st of every month at 2:00 AM UTC
        public bool DeactivateMissingEntities { get; set; } = false;
        public bool RunOnStartup { get; set; } = true;
    }
}
