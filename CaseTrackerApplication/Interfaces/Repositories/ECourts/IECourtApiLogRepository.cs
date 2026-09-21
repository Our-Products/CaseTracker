using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories.ECourts
{
    public interface IECourtApiLogRepository : IRepository<ECourtApiLog>
    {
        Task<ECourtUsageSummaryDto> GetUsageSummaryAsync(DateTimeOffset from, DateTimeOffset to);
        Task<IEnumerable<ECourtApiLog>> GetRecentLogsAsync(int count);
    }
}
