using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Clients;

namespace CaseTrackerApplication.Interfaces.Services.Clients
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetClientsAsync(string? search, Guid? lawFirmId);
        Task<ClientDto> GetClientByIdAsync(Guid clientId);
        Task<ClientDto> CreateClientAsync(CreateClientRequest request, Guid currentUserId, Guid? lawFirmId);
        Task<ClientDto> UpdateClientAsync(Guid clientId, UpdateClientRequest request, Guid currentUserId);
        Task DeleteClientAsync(Guid clientId, Guid currentUserId);
        Task LinkClientToCaseAsync(Guid caseId, LinkClientToCaseRequest request, Guid currentUserId);
        Task UnlinkClientFromCaseAsync(Guid caseId, Guid clientId, Guid currentUserId);
    }
}
