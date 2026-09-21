using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Repositories.ECourts;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories.ECourts
{
    public class ECourtApiLogRepository : Repository<ECourtApiLog>, IECourtApiLogRepository
    {
        public ECourtApiLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ECourtUsageSummaryDto> GetUsageSummaryAsync(DateTimeOffset from, DateTimeOffset to)
        {
            var logs = await _dbSet
                .Where(l => l.RequestedAt >= from && l.RequestedAt <= to)
                .ToListAsync();

            if (!logs.Any())
            {
                return new ECourtUsageSummaryDto
                {
                    TotalRequests = 0,
                    SuccessfulRequests = 0,
                    FailedRequests = 0,
                    TotalCreditsCharged = 0,
                    SuccessRatePercentage = 100.0,
                    AverageLatencyMs = 0
                };
            }

            int total = logs.Count;
            int successful = logs.Count(l => l.IsSuccess);
            int failed = total - successful;
            int credits = logs.Sum(l => l.CreditsCharged);
            double avgLatency = logs.Average(l => (double)l.DurationMs);
            double successRate = total > 0 ? (successful * 100.0) / total : 100.0;

            return new ECourtUsageSummaryDto
            {
                TotalRequests = total,
                SuccessfulRequests = successful,
                FailedRequests = failed,
                TotalCreditsCharged = credits,
                SuccessRatePercentage = Math.Round(successRate, 2),
                AverageLatencyMs = Math.Round(avgLatency, 2)
            };
        }

        public async Task<IEnumerable<ECourtApiLog>> GetRecentLogsAsync(int count)
        {
            return await _dbSet
                .OrderByDescending(l => l.RequestedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
