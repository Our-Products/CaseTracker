using System;
using System.Collections.Generic;
using System.Text;

namespace CaseTrackerApplication.DTOs.UserLawFirms
{
    public class AddUserLawFirmRequest
    {
        public Guid UserId { get; set; }

        public Guid LawFirmId { get; set; }
    }
}
