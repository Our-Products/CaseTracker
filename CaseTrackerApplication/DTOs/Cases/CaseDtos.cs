using System;

namespace CaseTrackerApplication.DTOs.Cases
{
    public class CaseDto
    {
        public Guid CaseId { get; set; }
        public Guid? LawFirmId { get; set; }
        public Guid CourtId { get; set; }
        public string? CourtName { get; set; }
        public string? CnrNumber { get; set; }
        public string CaseNumber { get; set; } = null!;
        public string CaseType { get; set; } = null!;
        public string? FilingNumber { get; set; }
        public DateTime? FilingDate { get; set; }
        public string? RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string CaseTitle { get; set; } = null!;
        public string CaseStage { get; set; } = null!;
        public string CaseStatus { get; set; } = "Pending";
        public string? ActsSections { get; set; }
        public string? PoliceStation { get; set; }
        public string? FirNumber { get; set; }
        public int? FirYear { get; set; }
        public bool IsEcourtSynced { get; set; }
        public DateTimeOffset? LastSyncedAt { get; set; }
        public DateTimeOffset? LastSuccessfulSyncAt { get; set; }
        public string? SyncStatus { get; set; }
        public string Status { get; set; } = "Active";
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class CreateCaseRequest
    {
        public Guid? LawFirmId { get; set; }
        public Guid CourtId { get; set; }
        public string? CnrNumber { get; set; }
        public string CaseNumber { get; set; } = null!;
        public string CaseType { get; set; } = null!;
        public string? FilingNumber { get; set; }
        public DateTime? FilingDate { get; set; }
        public string? RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string CaseTitle { get; set; } = null!;
        public string CaseStage { get; set; } = "Admission";
        public string? ActsSections { get; set; }
        public string? PoliceStation { get; set; }
        public string? FirNumber { get; set; }
        public int? FirYear { get; set; }
        public bool IsEcourtSynced { get; set; } = false;
    }

    public class UpdateCaseRequest
    {
        public Guid CourtId { get; set; }
        public string? CnrNumber { get; set; }
        public string CaseNumber { get; set; } = null!;
        public string CaseType { get; set; } = null!;
        public string? FilingNumber { get; set; }
        public DateTime? FilingDate { get; set; }
        public string? RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string CaseTitle { get; set; } = null!;
        public string CaseStage { get; set; } = null!;
        public string CaseStatus { get; set; } = "Pending";
        public string? ActsSections { get; set; }
        public string? PoliceStation { get; set; }
        public string? FirNumber { get; set; }
        public int? FirYear { get; set; }
        public bool IsEcourtSynced { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class CaseDetailDto : CaseDto
    {
        public System.Collections.Generic.List<CaseClientItemDto> Clients { get; set; } = new();
        public System.Collections.Generic.List<CaseLawyerItemDto> Lawyers { get; set; } = new();
        public System.Collections.Generic.List<CaseHearingItemDto> Hearings { get; set; } = new();
        public System.Collections.Generic.List<CaseOrderItemDto> Orders { get; set; } = new();
    }

    public class AssignCaseLawyerRequest
    {
        public Guid LawyerId { get; set; }
        public string? RoleId { get; set; } = "R001";
        public string? RoleName { get; set; } = "Lead Counsel";
    }

    public class CaseLawyerItemDto
    {
        public Guid CaseId { get; set; }
        public Guid LawyerId { get; set; }
        public string LawyerName { get; set; } = null!;
        public string? BarCouncilId { get; set; }
        public string? Specialization { get; set; }
        public string RoleName { get; set; } = "Lead Counsel";
        public string Status { get; set; } = "Active";
        public DateTimeOffset AssignedAt { get; set; }
    }

    public class CaseClientItemDto
    {
        public Guid ClientId { get; set; }
        public string FullName { get; set; } = null!;
        public string PartyType { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }

    public class CaseHearingItemDto
    {
        public Guid HearingId { get; set; }
        public DateTime HearingDate { get; set; }
        public int? ItemNumber { get; set; }
        public string PurposeOfHearing { get; set; } = null!;
        public string? BusinessOnDate { get; set; }
        public DateTime? NextHearingDate { get; set; }
        public string HearingStatus { get; set; } = null!;
    }

    public class CaseOrderItemDto
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; } = null!;
        public string? OrderUrl { get; set; }
        public bool IsCertified { get; set; }
    }
}
