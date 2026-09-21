using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories.Clients
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<IEnumerable<Client>> SearchClientsAsync(string query, Guid? lawFirmId);
        Task<IEnumerable<Client>> GetClientsByLawFirmAsync(Guid lawFirmId);
        Task<CaseClient?> GetCaseClientLinkAsync(Guid caseId, Guid clientId);
        Task AddCaseClientLinkAsync(CaseClient caseClient);
        Task RemoveCaseClientLinkAsync(CaseClient caseClient);
    }
}
