using System;

namespace CaseTrackerDomain.Models
{
    public class LawFirm
    {
        public Guid LawFirmId { get; set; }

        public string FirmName { get; set; } = null!;

        public string RegistrationNumber { get; set; } = null!;

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? City { get; set; }

        public string? District { get; set; }

        public string? State { get; set; }

        public int? Pincode { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }


        // Navigation Properties

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

    }
}
