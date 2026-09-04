using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaseTrackerApplication.DTOs
{
    public enum RegisterType
    {
        Individual = 0,
        Organization = 1
    }

    public class LawFirmDto
    {
        [Required]
        public string FirmName { get; set; } = null!;
        public string? RegistrationNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public int? Pincode { get; set; }
    }

    public class RegisterRequest : IValidatableObject
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string MobileNumber { get; set; } = null!;

        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = null!;

        public string? BarCouncilId { get; set; }

        public string? BarCouncilName { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        public RegisterType RegisterType { get; set; } = RegisterType.Individual;

        public LawFirmDto? LawFirm { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RegisterType == RegisterType.Organization)
            {
                if (LawFirm == null)
                {
                    yield return new ValidationResult(
                        "LawFirm information is required when registering as Organization.",
                        new[] { nameof(LawFirm) });
                }
                else if (string.IsNullOrWhiteSpace(LawFirm.FirmName))
                {
                    yield return new ValidationResult(
                        "LawFirm.FirmName is required.",
                        new[] { nameof(LawFirm) + "." + nameof(LawFirmDto.FirmName) });
                }
            }

            yield break;
        }
    }
}
