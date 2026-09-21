using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Hearings;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Hearings;
using CaseTrackerApplication.Interfaces.Services.Hearings;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services.Hearings
{
    public class HearingService : IHearingService
    {
        private readonly IHearingRepository _hearingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HearingService(
            IHearingRepository hearingRepository,
            IUnitOfWork unitOfWork)
        {
            _hearingRepository = hearingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<HearingDto>> GetHearingsByCaseAsync(Guid caseId)
        {
            var list = await _hearingRepository.GetHearingsByCaseAsync(caseId);
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<HearingDto>> GetDailyBoardAsync(DateTime date, Guid? lawFirmId, Guid? courtId)
        {
            var list = await _hearingRepository.GetDailyBoardAsync(date, lawFirmId, courtId);
            return list.Select(MapToDto);
        }

        public async Task<HearingDto> GetHearingByIdAsync(Guid hearingId)
        {
            var entity = await _hearingRepository.GetByIdAsync(hearingId);
            if (entity == null)
            {
                throw new NotFoundException($"Hearing with ID '{hearingId}' was not found.");
            }

            return MapToDto(entity);
        }

        public async Task<HearingDto> CreateHearingAsync(CreateHearingRequest request, Guid currentUserId)
        {
            var now = DateTimeOffset.UtcNow;
            var entity = new CaseHearing
            {
                HearingId = Guid.NewGuid(),
                CaseId = request.CaseId,
                HearingDate = request.HearingDate,
                ItemNumber = request.ItemNumber,
                CourtHall = request.CourtHall?.Trim(),
                JudgeName = request.JudgeName?.Trim(),
                PurposeOfHearing = request.PurposeOfHearing.Trim(),
                BusinessOnDate = request.BusinessOnDate?.Trim(),
                NextHearingDate = request.NextHearingDate,
                NextPurpose = request.NextPurpose?.Trim(),
                HearingStatus = request.HearingStatus ?? "Scheduled",
                DailyOrderSummary = request.DailyOrderSummary?.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId
            };

            await _hearingRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<HearingDto> UpdateHearingAsync(Guid hearingId, UpdateHearingRequest request, Guid currentUserId)
        {
            var entity = await _hearingRepository.GetByIdAsync(hearingId);
            if (entity == null)
            {
                throw new NotFoundException($"Hearing with ID '{hearingId}' was not found.");
            }

            entity.HearingDate = request.HearingDate;
            entity.ItemNumber = request.ItemNumber;
            entity.CourtHall = request.CourtHall?.Trim();
            entity.JudgeName = request.JudgeName?.Trim();
            entity.PurposeOfHearing = request.PurposeOfHearing.Trim();
            entity.BusinessOnDate = request.BusinessOnDate?.Trim();
            entity.NextHearingDate = request.NextHearingDate;
            entity.NextPurpose = request.NextPurpose?.Trim();
            entity.HearingStatus = request.HearingStatus ?? entity.HearingStatus;
            entity.DailyOrderSummary = request.DailyOrderSummary?.Trim();
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.UpdatedBy = currentUserId;

            await _hearingRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task DeleteHearingAsync(Guid hearingId, Guid currentUserId)
        {
            var entity = await _hearingRepository.GetByIdAsync(hearingId);
            if (entity == null)
            {
                throw new NotFoundException($"Hearing with ID '{hearingId}' was not found.");
            }

            await _hearingRepository.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private static HearingDto MapToDto(CaseHearing entity)
        {
            return new HearingDto
            {
                HearingId = entity.HearingId,
                CaseId = entity.CaseId,
                CaseNumber = entity.Case?.CaseNumber,
                CaseTitle = entity.Case?.CaseTitle,
                HearingDate = entity.HearingDate,
                ItemNumber = entity.ItemNumber,
                CourtHall = entity.CourtHall,
                JudgeName = entity.JudgeName,
                PurposeOfHearing = entity.PurposeOfHearing,
                BusinessOnDate = entity.BusinessOnDate,
                NextHearingDate = entity.NextHearingDate,
                NextPurpose = entity.NextPurpose,
                HearingStatus = entity.HearingStatus,
                DailyOrderSummary = entity.DailyOrderSummary,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
