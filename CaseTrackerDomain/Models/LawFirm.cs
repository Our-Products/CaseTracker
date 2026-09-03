using System;

namespace CaseTrackerDomain.Models
{
    public class LawFirm
    {
        public Guid LawFirmId { get; set; }

        public string FirmName { get; set; } = null!;

        public string? RegistrationNumber { get; set; }

        // Stored as JSON string (customer can change to a complex type later)
        public string AddressLine1 { get; set; } = null!;

        public string? AddressLine2 { get; set; }

        public string City { get; set; } = null!;

        public string District { get; set; } = null!;

        public string State { get; set; } = null!;

        public int Pincode { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
