using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Hearings;

namespace CaseTrackerApplication.Interfaces.Services.Hearings
{
    public interface IHearingService
    {
        Task<IEnumerable<HearingDto>> GetHearingsByCaseAsync(Guid caseId);
        Task<IEnumerable<HearingDto>> GetDailyBoardAsync(DateTime date, Guid? lawFirmId, Guid? courtId);
        Task<HearingDto> GetHearingByIdAsync(Guid hearingId);
        Task<HearingDto> CreateHearingAsync(CreateHearingRequest request, Guid currentUserId);
        Task<HearingDto> UpdateHearingAsync(Guid hearingId, UpdateHearingRequest request, Guid currentUserId);
        Task DeleteHearingAsync(Guid hearingId, Guid currentUserId);
    }
}
