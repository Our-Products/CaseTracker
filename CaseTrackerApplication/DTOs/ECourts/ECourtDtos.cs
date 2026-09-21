using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CaseTrackerApplication.DTOs.ECourts
{
    // ========================================================
    // APPLICATION RESULTS & AUDIT DTOS
    // ========================================================

    public class ECourtSyncResultDto
    {
        public string JobName { get; set; } = "CaseStatusSync";
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset CompletedAt { get; set; }
        public int TotalEligible { get; set; }
        public int Processed { get; set; }
        public int Succeeded { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public int CreditsCharged { get; set; }
        public long DurationMs { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }

    public class CourtMasterSyncResultDto
    {
        public string TargetState { get; set; } = "TN";
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset CompletedAt { get; set; }
        public int StatesProcessed { get; set; }
        public int DistrictsProcessed { get; set; }
        public int ComplexesProcessed { get; set; }
        public int CourtsProcessed { get; set; }
        public long DurationMs { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ECourtUsageSummaryDto
    {
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public int TotalCreditsCharged { get; set; }
        public double SuccessRatePercentage { get; set; }
        public double AverageLatencyMs { get; set; }
    }

    // ========================================================
    // EXTERNAL ECOURTS API CONTRACTS (Mirroring webapi.ecourtsindia.com)
    // ========================================================

    public class ECourtsResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("meta")]
        public ECourtsMeta? Meta { get; set; }
    }

    public class ECourtsMeta
    {
        [JsonPropertyName("request_id")]
        public string? RequestId { get; set; }
    }

    public class ECourtsErrorResponse
    {
        [JsonPropertyName("error")]
        public ECourtsError? Error { get; set; }

        [JsonPropertyName("meta")]
        public ECourtsMeta? Meta { get; set; }
    }

    public class ECourtsError
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = null!;

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;

        [JsonPropertyName("details")]
        public List<string>? Details { get; set; }
    }

    public class ECourtsStateItem
    {
        [JsonPropertyName("state")]
        public string State { get; set; } = null!;

        [JsonPropertyName("stateName")]
        public string StateName { get; set; } = null!;
    }

    public class ECourtsDistrictItem
    {
        [JsonPropertyName("districtCode")]
        public string DistrictCode { get; set; } = null!;

        [JsonPropertyName("districtName")]
        public string DistrictName { get; set; } = null!;
    }

    public class ECourtsComplexItem
    {
        [JsonPropertyName("courtComplexCode")]
        public string CourtComplexCode { get; set; } = null!;

        [JsonPropertyName("courtComplexName")]
        public string CourtComplexName { get; set; } = null!;
    }

    public class ECourtsCourtItem
    {
        [JsonPropertyName("court")]
        public string Court { get; set; } = null!;

        [JsonPropertyName("courtNo")]
        public string? CourtNo { get; set; }

        [JsonPropertyName("courtName")]
        public string CourtName { get; set; } = null!;

        [JsonPropertyName("courtDivision")]
        public string? CourtDivision { get; set; }

        [JsonPropertyName("judgeName")]
        public string? JudgeName { get; set; }
    }

    public class ECourtsCaseDetailPayload
    {
        [JsonPropertyName("courtCaseData")]
        public ECourtsCourtCaseData? CourtCaseData { get; set; }
    }

    public class ECourtsCourtCaseData
    {
        [JsonPropertyName("cnr")]
        public string? Cnr { get; set; }

        [JsonPropertyName("caseNumber")]
        public string? CaseNumber { get; set; }

        [JsonPropertyName("registrationNumber")]
        public string? RegistrationNumber { get; set; }

        [JsonPropertyName("caseType")]
        public string? CaseType { get; set; }

        [JsonPropertyName("caseStatus")]
        public string? CaseStatus { get; set; }

        [JsonPropertyName("filingDate")]
        public string? FilingDate { get; set; }

        [JsonPropertyName("registrationDate")]
        public string? RegistrationDate { get; set; }

        [JsonPropertyName("petitioners")]
        public List<string>? Petitioners { get; set; }

        [JsonPropertyName("respondents")]
        public List<string>? Respondents { get; set; }

        [JsonPropertyName("historyOfCaseHearings")]
        public List<ECourtsHearingHistoryItem>? HistoryOfCaseHearings { get; set; }

        [JsonPropertyName("judgmentOrders")]
        public List<ECourtsOrderInfoItem>? JudgmentOrders { get; set; }

        [JsonPropertyName("interimOrders")]
        public List<ECourtsOrderInfoItem>? InterimOrders { get; set; }
    }

    public class ECourtsHearingHistoryItem
    {
        [JsonPropertyName("hearingDate")]
        public string? HearingDate { get; set; }

        [JsonPropertyName("judge")]
        public string? Judge { get; set; }

        [JsonPropertyName("businessOnDate")]
        public string? BusinessOnDate { get; set; }

        [JsonPropertyName("purposeOfListing")]
        public string? PurposeOfListing { get; set; }
    }

    public class ECourtsOrderInfoItem
    {
        [JsonPropertyName("orderDate")]
        public string? OrderDate { get; set; }

        [JsonPropertyName("orderType")]
        public string? OrderType { get; set; }

        [JsonPropertyName("orderUrl")]
        public string? OrderUrl { get; set; }
    }
}
