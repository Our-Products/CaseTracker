using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class Client
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

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation Properties
        public LawFirm? LawFirm { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<CaseClient> CaseClients { get; set; } = new List<CaseClient>();
    }
}
