using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.Interfaces.Repositories.Clients;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories.Clients
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Client>> SearchClientsAsync(string query, Guid? lawFirmId)
        {
            var q = _dbSet.AsQueryable();
            if (lawFirmId.HasValue)
            {
                q = q.Where(c => c.LawFirmId == lawFirmId.Value);
            }

            string lower = query.ToLower();
            return await q
                .Where(c => c.Status == "Active" &&
                           (c.FullName.ToLower().Contains(lower) ||
                            c.PrimaryPhone.Contains(lower) ||
                            (c.Email != null && c.Email.ToLower().Contains(lower))))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetClientsByLawFirmAsync(Guid lawFirmId)
        {
            return await _dbSet
                .Where(c => c.LawFirmId == lawFirmId && c.Status == "Active")
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<CaseClient?> GetCaseClientLinkAsync(Guid caseId, Guid clientId)
        {
            return await _context.CaseClients
                .FirstOrDefaultAsync(cc => cc.CaseId == caseId && cc.ClientId == clientId);
        }

        public async Task AddCaseClientLinkAsync(CaseClient caseClient)
        {
            await _context.CaseClients.AddAsync(caseClient);
        }

        public Task RemoveCaseClientLinkAsync(CaseClient caseClient)
        {
            _context.CaseClients.Remove(caseClient);
            return Task.CompletedTask;
        }
    }
}
