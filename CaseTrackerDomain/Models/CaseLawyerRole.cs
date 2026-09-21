using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class CaseLawyerRole
    {
        public string RoleId { get; set; } = null!;

        public string RoleName { get; set; } = null!;

        public string Status { get; set; } = "Active";

        public ICollection<CaseLawyer> CaseLawyers { get; set; } = new List<CaseLawyer>();
    }
}
