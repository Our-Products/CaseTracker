using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Cases;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Cases;
using CaseTrackerApplication.Interfaces.Repositories.Hearings;
using CaseTrackerApplication.Interfaces.Services.Cases;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services.Cases
{
    public class CaseService : ICaseService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IHearingRepository _hearingRepository;
        private readonly IECourtClient _ecourtClient;
        private readonly IUnitOfWork _unitOfWork;

        public CaseService(
            ICaseRepository caseRepository,
            IHearingRepository hearingRepository,
            IECourtClient ecourtClient,
            IUnitOfWork unitOfWork)
        {
            _caseRepository = caseRepository;
            _hearingRepository = hearingRepository;
            _ecourtClient = ecourtClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CaseDto>> GetAllCasesAsync(Guid? lawFirmId)
        {
            IEnumerable<Case> cases = lawFirmId.HasValue
                ? await _caseRepository.GetCasesByLawFirmAsync(lawFirmId.Value)
                : await _caseRepository.GetAllAsync();

            return cases.Select(MapToDto);
        }

        public async Task<CaseDetailDto> GetCaseByIdAsync(Guid caseId)
        {
            var entity = await _caseRepository.GetWithDetailsAsync(caseId);
            if (entity == null)
            {
                throw new NotFoundException($"Case with ID '{caseId}' was not found.");
            }

            var dto = new CaseDetailDto
            {
                CaseId = entity.CaseId,
                LawFirmId = entity.LawFirmId,
                CourtId = entity.CourtId,
                CourtName = entity.Court?.CourtName,
                CnrNumber = entity.CnrNumber,
                CaseNumber = entity.CaseNumber,
                CaseType = entity.CaseType,
                FilingNumber = entity.FilingNumber,
                FilingDate = entity.FilingDate,
                RegistrationNumber = entity.RegistrationNumber,
                RegistrationDate = entity.RegistrationDate,
                CaseTitle = entity.CaseTitle,
                CaseStage = entity.CaseStage,
                CaseStatus = entity.CaseStatus,
                ActsSections = entity.ActsSections,
                PoliceStation = entity.PoliceStation,
                FirNumber = entity.FirNumber,
                FirYear = entity.FirYear,
                IsEcourtSynced = entity.IsEcourtSynced,
                LastSyncedAt = entity.LastSyncedAt,
                LastSuccessfulSyncAt = entity.LastSuccessfulSyncAt,
                SyncStatus = entity.SyncStatus,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                Clients = entity.CaseClients?.Select(cc => new CaseClientItemDto
                {
                    ClientId = cc.ClientId,
                    FullName = cc.Client?.FullName ?? "Unknown",
                    PartyType = cc.PartyType,
                    IsPrimary = cc.IsPrimary
                }).ToList() ?? new(),
                Lawyers = entity.CaseLawyers?.Select(cl => new CaseLawyerItemDto
                {
                    CaseId = cl.CaseId,
                    LawyerId = cl.LawyerId,
                    LawyerName = cl.Lawyer?.FullName ?? "Advocate",
                    BarCouncilId = cl.Lawyer?.BarCouncilId,
                    RoleName = cl.LawyerRole?.RoleName ?? "Lead Counsel",
                    Status = cl.Status,
                    AssignedAt = cl.AssignedAt
                }).ToList() ?? new(),
                Hearings = entity.CaseHearings?.OrderByDescending(h => h.HearingDate).Select(h => new CaseHearingItemDto
                {
                    HearingId = h.HearingId,
                    HearingDate = h.HearingDate,
                    ItemNumber = h.ItemNumber,
                    PurposeOfHearing = h.PurposeOfHearing,
                    BusinessOnDate = h.BusinessOnDate,
                    NextHearingDate = h.NextHearingDate,
                    HearingStatus = h.HearingStatus
                }).ToList() ?? new(),
                Orders = entity.CaseOrders?.OrderByDescending(o => o.OrderDate).Select(o => new CaseOrderItemDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    OrderType = o.OrderType,
                    OrderUrl = o.OrderUrl,
                    IsCertified = o.IsCertified
                }).ToList() ?? new()
            };

            return dto;
        }

        public async Task<CaseDto> GetCaseByCnrAsync(string cnrNumber)
        {
            var entity = await _caseRepository.GetByCnrAsync(cnrNumber);
            if (entity == null)
            {
                throw new NotFoundException($"Case with CNR '{cnrNumber}' was not found.");
            }

            return MapToDto(entity);
        }

        public async Task<CaseDto> CreateCaseAsync(CreateCaseRequest request, Guid currentUserId, Guid? lawFirmId)
        {
            if (!string.IsNullOrWhiteSpace(request.CnrNumber))
            {
                var existing = await _caseRepository.GetByCnrAsync(request.CnrNumber.Trim());
                if (existing != null)
                {
                    throw new ConflictException($"A case with CNR '{request.CnrNumber}' already exists.");
                }
            }

            var now = DateTimeOffset.UtcNow;
            var entity = new Case
            {
                CaseId = Guid.NewGuid(),
                LawFirmId = request.LawFirmId ?? lawFirmId,
                CourtId = request.CourtId,
                CnrNumber = string.IsNullOrWhiteSpace(request.CnrNumber) ? null : request.CnrNumber.Trim().ToUpperInvariant(),
                CaseNumber = request.CaseNumber.Trim(),
                CaseType = request.CaseType.Trim(),
                FilingNumber = request.FilingNumber?.Trim(),
                FilingDate = request.FilingDate,
                RegistrationNumber = request.RegistrationNumber?.Trim(),
                RegistrationDate = request.RegistrationDate,
                CaseTitle = request.CaseTitle.Trim(),
                CaseStage = request.CaseStage.Trim(),
                CaseStatus = "Pending",
                ActsSections = request.ActsSections?.Trim(),
                PoliceStation = request.PoliceStation?.Trim(),
                FirNumber = request.FirNumber?.Trim(),
                FirYear = request.FirYear,
                IsEcourtSynced = request.IsEcourtSynced,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId
            };

            await _caseRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<CaseDto> UpdateCaseAsync(Guid caseId, UpdateCaseRequest request, Guid currentUserId)
        {
            var entity = await _caseRepository.GetByIdAsync(caseId);
            if (entity == null)
            {
                throw new NotFoundException($"Case with ID '{caseId}' was not found.");
            }

            entity.CourtId = request.CourtId;
            entity.CnrNumber = string.IsNullOrWhiteSpace(request.CnrNumber) ? null : request.CnrNumber.Trim().ToUpperInvariant();
            entity.CaseNumber = request.CaseNumber.Trim();
            entity.CaseType = request.CaseType.Trim();
            entity.FilingNumber = request.FilingNumber?.Trim();
            entity.FilingDate = request.FilingDate;
            entity.RegistrationNumber = request.RegistrationNumber?.Trim();
            entity.RegistrationDate = request.RegistrationDate;
            entity.CaseTitle = request.CaseTitle.Trim();
            entity.CaseStage = request.CaseStage.Trim();
            entity.CaseStatus = request.CaseStatus.Trim();
            entity.ActsSections = request.ActsSections?.Trim();
            entity.PoliceStation = request.PoliceStation?.Trim();
            entity.FirNumber = request.FirNumber?.Trim();
            entity.FirYear = request.FirYear;
            entity.IsEcourtSynced = request.IsEcourtSynced;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.UpdatedBy = currentUserId;

            await _caseRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task DeleteCaseAsync(Guid caseId, Guid currentUserId)
        {
            var entity = await _caseRepository.GetByIdAsync(caseId);
            if (entity == null)
            {
                throw new NotFoundException($"Case with ID '{caseId}' was not found.");
            }

            entity.Status = "Archived";
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.UpdatedBy = currentUserId;

            await _caseRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CaseDto> SyncCaseNowAsync(Guid caseId, Guid currentUserId)
        {
            var entity = await _caseRepository.GetByIdAsync(caseId);
            if (entity == null)
            {
                throw new NotFoundException($"Case with ID '{caseId}' was not found.");
            }

            if (string.IsNullOrWhiteSpace(entity.CnrNumber))
            {
                throw new ValidationException("Cannot synchronize a case that does not have an assigned eCourts CNR Number.");
            }

            var now = DateTimeOffset.UtcNow;
            entity.LastSyncAttemptAt = now;

            try
            {
                var payload = await _ecourtClient.GetCaseDetailAsync(entity.CnrNumber);
                if (payload?.CourtCaseData != null)
                {
                    var data = payload.CourtCaseData;
                    if (!string.IsNullOrWhiteSpace(data.CaseStatus))
                    {
                        entity.CaseStatus = data.CaseStatus;
                    }
                    if (!string.IsNullOrWhiteSpace(data.CaseNumber))
                    {
                        entity.CaseNumber = data.CaseNumber;
                    }
                    if (!string.IsNullOrWhiteSpace(data.RegistrationNumber))
                    {
                        entity.RegistrationNumber = data.RegistrationNumber;
                    }

                    // Ingest hearings if available
                    if (data.HistoryOfCaseHearings != null && data.HistoryOfCaseHearings.Any())
                    {
                        foreach (var h in data.HistoryOfCaseHearings)
                        {
                            if (DateTime.TryParse(h.HearingDate, out var hDate))
                            {
                                var existingHearings = await _hearingRepository.GetHearingsByCaseAsync(caseId);
                                if (!existingHearings.Any(eh => eh.HearingDate.Date == hDate.Date))
                                {
                                    await _hearingRepository.AddAsync(new CaseHearing
                                    {
                                        HearingId = Guid.NewGuid(),
                                        CaseId = caseId,
                                        HearingDate = hDate,
                                        JudgeName = h.Judge,
                                        PurposeOfHearing = string.IsNullOrWhiteSpace(h.PurposeOfListing) ? "Hearing" : h.PurposeOfListing,
                                        BusinessOnDate = h.BusinessOnDate,
                                        HearingStatus = "Adjourned",
                                        CreatedAt = now,
                                        UpdatedAt = now,
                                        CreatedBy = currentUserId
                                    });
                                }
                            }
                        }
                    }

                    entity.LastSuccessfulSyncAt = now;
                    entity.LastSyncedAt = now;
                    entity.SyncStatus = "Success";
                    entity.SyncError = null;
                }
                else
                {
                    entity.SyncStatus = "Failed";
                    entity.SyncError = "No docket data returned from eCourts.";
                }
            }
            catch (Exception ex)
            {
                entity.SyncStatus = "Failed";
                entity.SyncError = ex.Message;
            }

            entity.UpdatedAt = now;
            entity.UpdatedBy = currentUserId;

            await _caseRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        private static CaseDto MapToDto(Case entity)
        {
            return new CaseDto
            {
                CaseId = entity.CaseId,
                LawFirmId = entity.LawFirmId,
                CourtId = entity.CourtId,
                CourtName = entity.Court?.CourtName,
                CnrNumber = entity.CnrNumber,
                CaseNumber = entity.CaseNumber,
                CaseType = entity.CaseType,
                FilingNumber = entity.FilingNumber,
                FilingDate = entity.FilingDate,
                RegistrationNumber = entity.RegistrationNumber,
                RegistrationDate = entity.RegistrationDate,
                CaseTitle = entity.CaseTitle,
                CaseStage = entity.CaseStage,
                CaseStatus = entity.CaseStatus,
                ActsSections = entity.ActsSections,
                PoliceStation = entity.PoliceStation,
                FirNumber = entity.FirNumber,
                FirYear = entity.FirYear,
                IsEcourtSynced = entity.IsEcourtSynced,
                LastSyncedAt = entity.LastSyncedAt,
                LastSuccessfulSyncAt = entity.LastSuccessfulSyncAt,
                SyncStatus = entity.SyncStatus,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<IEnumerable<CaseLawyerItemDto>> GetCaseLawyersAsync(Guid caseId)
        {
            var caseLawyers = await _caseRepository.GetCaseLawyersAsync(caseId);
            return caseLawyers.Select(cl => new CaseLawyerItemDto
            {
                CaseId = cl.CaseId,
                LawyerId = cl.LawyerId,
                LawyerName = cl.Lawyer?.FullName ?? "Advocate",
                BarCouncilId = cl.Lawyer?.BarCouncilId,
                RoleName = cl.LawyerRole?.RoleName ?? "Lead Counsel",
                Status = cl.Status,
                AssignedAt = cl.AssignedAt
            });
        }

        public async Task<CaseLawyerItemDto> AssignLawyerAsync(Guid caseId, AssignCaseLawyerRequest request, Guid currentUserId)
        {
            var caseEntity = await _caseRepository.GetByIdAsync(caseId);
            if (caseEntity == null)
            {
                throw new NotFoundException($"Case with ID '{caseId}' was not found.");
            }

            var now = DateTimeOffset.UtcNow;
            var caseLawyer = new CaseLawyer
            {
                CaseId = caseId,
                LawyerId = request.LawyerId,
                LawyerRoleId = string.IsNullOrWhiteSpace(request.RoleId) ? "R001" : request.RoleId.Trim(),
                Status = "Active",
                AssignedAt = now,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId
            };

            await _caseRepository.AddCaseLawyerAsync(caseLawyer);
            await _unitOfWork.SaveChangesAsync();

            var assigned = (await _caseRepository.GetCaseLawyersAsync(caseId))
                .FirstOrDefault(cl => cl.LawyerId == request.LawyerId);

            return new CaseLawyerItemDto
            {
                CaseId = caseId,
                LawyerId = request.LawyerId,
                LawyerName = assigned?.Lawyer?.FullName ?? "Advocate",
                BarCouncilId = assigned?.Lawyer?.BarCouncilId,
                RoleName = request.RoleName ?? assigned?.LawyerRole?.RoleName ?? "Lead Counsel",
                Status = "Active",
                AssignedAt = now
            };
        }

        public async Task RemoveLawyerAsync(Guid caseId, Guid lawyerId, Guid currentUserId)
        {
            await _caseRepository.RemoveCaseLawyerAsync(caseId, lawyerId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
