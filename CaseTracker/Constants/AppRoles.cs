namespace CaseTracker.Constants
{
    /// <summary>
    /// Centralized role names and role combinations used for authorization.
    /// Hierarchy:
    /// - SuperAdmin: Company / Platform Level
    /// - Admin: Law Firm Level
    /// - Lawyer: Legal Practitioner
    /// - Staff: Auxiliary Staff
    /// </summary>
    public static class AppRoles
    {
        /// <summary>Company / Platform Level Administrator</summary>
        public const string SuperAdmin = "SuperAdmin";

        /// <summary>Law Firm Level Administrator</summary>
        public const string Admin = "Admin";

        /// <summary>Lawyer / Legal Practitioner</summary>
        public const string Lawyer = "Lawyer";

        /// <summary>Support & Administrative Staff</summary>
        public const string Staff = "Staff";

        // Combinations
        public const string SuperAdminOnly = "SuperAdmin";
        public const string SuperAdminOrAdmin = "SuperAdmin,Admin";
        public const string AdminOrLawyer = "SuperAdmin,Admin,Lawyer";
        public const string AdminOrStaff = "SuperAdmin,Admin,Staff";
        public const string All = "SuperAdmin,Admin,Lawyer,Staff";
    }
}
