using System.ComponentModel.DataAnnotations;

namespace CaseTrackerApplication.DTOs.Auth
{
    public enum RegisterType
    {
        Individual = 0,
        Organization = 1
    }

    public class LawFirmDto
    {
        public string? FirmName { get; set; }

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

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        // Bar Council details are optional.
        // Required only when the user is registered with Bar Council.
        public string? BarCouncilId { get; set; }

        public string? BarCouncilName { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        public RegisterType RegisterType { get; set; }
            = RegisterType.Individual;

        // Required only when RegisterType = Organization
        public LawFirmDto? LawFirm { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (RegisterType == RegisterType.Organization)
            {
                if (LawFirm == null)
                {
                    yield return new ValidationResult(
                        "Law firm details are required for organization registration.",
                        new[]
                        {
                            nameof(LawFirm)
                        });

                    yield break;
                }

                if (string.IsNullOrWhiteSpace(LawFirm.FirmName))
                {
                    yield return new ValidationResult(
                        "Firm name is required for organization registration.",
                        new[]
                        {
                            nameof(LawFirm)
                        });
                }

                if (string.IsNullOrWhiteSpace(
                    LawFirm.RegistrationNumber))
                {
                    yield return new ValidationResult(
                        "Registration number is required for organization registration.",
                        new[]
                        {
                            nameof(LawFirm)
                        });
                }
            }
        }
    }
}