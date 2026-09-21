using System;

namespace CaseTrackerDomain.Models
{
    public class ECourtApiLog
    {
        public Guid LogId { get; set; }

        public string Endpoint { get; set; } = null!;

        public string HttpMethod { get; set; } = "GET";

        public string? CnrNumber { get; set; }

        public int StatusCode { get; set; }

        public string? RequestId { get; set; }

        public int CreditsCharged { get; set; }

        public long DurationMs { get; set; }

        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTimeOffset RequestedAt { get; set; }
    }
}
