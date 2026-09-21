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
        public string Status { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }


        // Navigation Properties

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<Lawyer> Lawyers { get; set; } = new List<Lawyer>();

        public ICollection<UserLawFirm> UserLawFirms { get; set; } = new List<UserLawFirm>();

        public ICollection<Client> Clients { get; set; } = new List<Client>();

        public ICollection<Case> Cases { get; set; } = new List<Case>();
    }
}
