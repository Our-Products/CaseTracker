using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaseTrackerApplication.DTOs
{
    /// <summary>
    /// Type of registration sent by the client. The mobile app may send the
    /// enum value as an integer (0,1,...) which model binding supports by default.
    /// </summary>
    public enum RegisterType
    {
        Individual = 0,
        Organization = 1
    }

    public class LawFirmDto
    {
        public string FirmName { get; set; } = null!;
        public string? RegistrationNumber { get; set; }
        // Stored as JSON or free-form string; change to a structured type if needed
        public string? AddressJson { get; set; }
    }

    /// <summary>
    /// Registration request coming from client apps.
    /// If RegisterType == Associates, LawFirm must be provided.
    /// </summary>
    public class RegisterRequest : IValidatableObject
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string MobileNumber { get; set; } = null!;

        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = null!;

        public RegisterType RegisterType { get; set; } = RegisterType.Individual;

        /// <summary>
        /// Optional: required when RegisterType == Associates.
        /// </summary>
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

            // Additional validations (mobile format, password strength) can be added here.
            yield break;
        }
    }
}
