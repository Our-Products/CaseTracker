using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.Interfaces.Repositories.Courts;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories.Courts
{
    public class CourtRepository : ICourtRepository
    {
        private readonly ApplicationDbContext _context;

        public CourtRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<State>> GetAllStatesAsync()
        {
            return await _context.States
                .Where(s => s.Status == "Active")
                .OrderBy(s => s.StateName)
                .ToListAsync();
        }

        public async Task<State?> GetStateByCodeAsync(string stateCode)
        {
            return await _context.States
                .FirstOrDefaultAsync(s => s.StateCode == stateCode);
        }

        public async Task<State> UpsertStateAsync(string stateCode, string stateName)
        {
            var existing = await _context.States.FirstOrDefaultAsync(s => s.StateCode == stateCode);
            var now = DateTimeOffset.UtcNow;

            if (existing != null)
            {
                if (existing.StateName != stateName)
                {
                    existing.StateName = stateName;
                    existing.UpdatedAt = now;
                    _context.States.Update(existing);
                    await _context.SaveChangesAsync();
                }
                return existing;
            }

            var entity = new State
            {
                StateId = Guid.NewGuid(),
                StateCode = stateCode,
                StateName = stateName,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            };

            await _context.States.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<District>> GetDistrictsByStateAsync(Guid stateId)
        {
            return await _context.Districts
                .Where(d => d.StateId == stateId && d.Status == "Active")
                .OrderBy(d => d.DistrictName)
                .ToListAsync();
        }

        public async Task<District?> GetDistrictByCodeAsync(Guid stateId, string districtCode)
        {
            return await _context.Districts
                .FirstOrDefaultAsync(d => d.StateId == stateId && d.DistrictCode == districtCode);
        }

        public async Task<District> UpsertDistrictAsync(Guid stateId, string districtCode, string districtName)
        {
            var existing = await _context.Districts
                .FirstOrDefaultAsync(d => d.StateId == stateId && (d.DistrictCode == districtCode || d.DistrictName == districtName));
            var now = DateTimeOffset.UtcNow;

            if (existing != null)
            {
                if (existing.DistrictName != districtName || existing.DistrictCode != districtCode)
                {
                    existing.DistrictName = districtName;
                    existing.DistrictCode = districtCode;
                    existing.UpdatedAt = now;
                    _context.Districts.Update(existing);
                    await _context.SaveChangesAsync();
                }
                return existing;
            }

            var entity = new District
            {
                DistrictId = Guid.NewGuid(),
                StateId = stateId,
                DistrictCode = districtCode,
                DistrictName = districtName,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            };

            await _context.Districts.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<CourtComplex>> GetComplexesByDistrictAsync(Guid districtId)
        {
            return await _context.CourtComplexes
                .Where(c => c.DistrictId == districtId && c.Status == "Active")
                .OrderBy(c => c.ComplexName)
                .ToListAsync();
        }

        public async Task<CourtComplex?> GetComplexByCodeAsync(Guid districtId, string complexCode)
        {
            return await _context.CourtComplexes
                .FirstOrDefaultAsync(c => c.DistrictId == districtId && c.ComplexCode == complexCode);
        }

        public async Task<CourtComplex> UpsertComplexAsync(Guid districtId, string complexCode, string complexName)
        {
            var existing = await _context.CourtComplexes
                .FirstOrDefaultAsync(c => c.DistrictId == districtId && (c.ComplexCode == complexCode || c.ComplexName == complexName));
            var now = DateTimeOffset.UtcNow;

            if (existing != null)
            {
                if (existing.ComplexName != complexName || existing.ComplexCode != complexCode)
                {
                    existing.ComplexName = complexName;
                    existing.ComplexCode = complexCode;
                    existing.UpdatedAt = now;
                    _context.CourtComplexes.Update(existing);
                    await _context.SaveChangesAsync();
                }
                return existing;
            }

            var entity = new CourtComplex
            {
                CourtComplexId = Guid.NewGuid(),
                DistrictId = districtId,
                ComplexCode = complexCode,
                ComplexName = complexName,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            };

            await _context.CourtComplexes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<Court>> GetCourtsByComplexAsync(Guid complexId)
        {
            return await _context.Courts
                .Include(c => c.CourtComplex)
                .Where(c => c.CourtComplexId == complexId && c.Status == "Active")
                .OrderBy(c => c.CourtName)
                .ToListAsync();
        }

        public async Task<Court?> GetCourtByCodeAsync(Guid complexId, string courtCode)
        {
            return await _context.Courts
                .FirstOrDefaultAsync(c => c.CourtComplexId == complexId && c.CourtCode == courtCode);
        }

        public async Task<Court> UpsertCourtAsync(Guid complexId, string courtCode, string courtName, string? courtNo, string? judgeName)
        {
            var existing = await _context.Courts
                .FirstOrDefaultAsync(c => c.CourtComplexId == complexId && (c.CourtCode == courtCode || c.CourtName == courtName));
            var now = DateTimeOffset.UtcNow;

            // Ensure default CourtType exists
            var courtType = await _context.CourtTypes.FirstOrDefaultAsync(t => t.CourtTypeId == "CT001");
            if (courtType == null)
            {
                courtType = new CourtType
                {
                    CourtTypeId = "CT001",
                    CourtTypeName = "District Court",
                    Status = "Active",
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _context.CourtTypes.AddAsync(courtType);
                await _context.SaveChangesAsync();
            }

            if (existing != null)
            {
                if (existing.CourtName != courtName || existing.CourtCode != courtCode)
                {
                    existing.CourtName = courtName;
                    existing.CourtCode = courtCode;
                    existing.UpdatedAt = now;
                    _context.Courts.Update(existing);
                    await _context.SaveChangesAsync();
                }
                return existing;
            }

            var entity = new Court
            {
                CourtId = Guid.NewGuid(),
                CourtComplexId = complexId,
                CourtTypeId = courtType.CourtTypeId,
                CourtCode = courtCode,
                CourtName = courtName,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            };

            await _context.Courts.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Court?> GetCourtByIdAsync(Guid courtId)
        {
            return await _context.Courts
                .Include(c => c.CourtComplex)
                .FirstOrDefaultAsync(c => c.CourtId == courtId);
        }

        public async Task UpdateDistrictStatusAsync(Guid districtId, string status)
        {
            var district = await _context.Districts.FirstOrDefaultAsync(d => d.DistrictId == districtId);
            if (district != null && district.Status != status)
            {
                district.Status = status;
                district.UpdatedAt = DateTimeOffset.UtcNow;
                _context.Districts.Update(district);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateComplexStatusAsync(Guid courtComplexId, string status)
        {
            var complex = await _context.CourtComplexes.FirstOrDefaultAsync(c => c.CourtComplexId == courtComplexId);
            if (complex != null && complex.Status != status)
            {
                complex.Status = status;
                complex.UpdatedAt = DateTimeOffset.UtcNow;
                _context.CourtComplexes.Update(complex);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddSyncHistoryAsync(CourtMasterSyncHistory history)
        {
            await _context.CourtMasterSyncHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourtMasterSyncHistory>> GetSyncHistoriesAsync(int count = 10)
        {
            return await _context.CourtMasterSyncHistories
                .OrderByDescending(h => h.StartedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
