using System;

namespace CaseTrackerApplication.DTOs.Clients
{
    public class ClientDto
    {
        public Guid ClientId { get; set; }
        public Guid? LawFirmId { get; set; }
        public string ClientType { get; set; } = "Individual";
        public string FullName { get; set; } = null!;
        public string PrimaryPhone { get; set; } = null!;
        public string? SecondaryPhone { get; set; }
        public string? Email { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? Pincode { get; set; }
        public string? ContactPerson { get; set; }
        public string? GstNumber { get; set; }
        public string? PanNumber { get; set; }
        public string Status { get; set; } = "Active";
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class CreateClientRequest
    {
        public Guid? LawFirmId { get; set; }
        public string ClientType { get; set; } = "Individual";
        public string FullName { get; set; } = null!;
        public string PrimaryPhone { get; set; } = null!;
        public string? SecondaryPhone { get; set; }
        public string? Email { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? Pincode { get; set; }
        public string? ContactPerson { get; set; }
        public string? GstNumber { get; set; }
        public string? PanNumber { get; set; }
    }

    public class UpdateClientRequest
    {
        public string ClientType { get; set; } = "Individual";
        public string FullName { get; set; } = null!;
        public string PrimaryPhone { get; set; } = null!;
        public string? SecondaryPhone { get; set; }
        public string? Email { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? Pincode { get; set; }
        public string? ContactPerson { get; set; }
        public string? GstNumber { get; set; }
        public string? PanNumber { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class LinkClientToCaseRequest
    {
        public Guid ClientId { get; set; }
        public string PartyType { get; set; } = "Petitioner";
        public int PartySequence { get; set; } = 1;
        public bool IsPrimary { get; set; } = true;
    }
}
