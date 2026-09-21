using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Clients;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Cases;
using CaseTrackerApplication.Interfaces.Repositories.Clients;
using CaseTrackerApplication.Interfaces.Services.Clients;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services.Clients
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICaseRepository _caseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(
            IClientRepository clientRepository,
            ICaseRepository caseRepository,
            IUnitOfWork unitOfWork)
        {
            _clientRepository = clientRepository;
            _caseRepository = caseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ClientDto>> GetClientsAsync(string? search, Guid? lawFirmId)
        {
            IEnumerable<Client> list;
            if (!string.IsNullOrWhiteSpace(search))
            {
                list = await _clientRepository.SearchClientsAsync(search.Trim(), lawFirmId);
            }
            else if (lawFirmId.HasValue)
            {
                list = await _clientRepository.GetClientsByLawFirmAsync(lawFirmId.Value);
            }
            else
            {
                list = await _clientRepository.GetAllAsync();
            }

            return list.Select(MapToDto);
        }

        public async Task<ClientDto> GetClientByIdAsync(Guid clientId)
        {
            var entity = await _clientRepository.GetByIdAsync(clientId);
            if (entity == null)
            {
                throw new NotFoundException($"Client with ID '{clientId}' was not found.");
            }

            return MapToDto(entity);
        }

        public async Task<ClientDto> CreateClientAsync(CreateClientRequest request, Guid currentUserId, Guid? lawFirmId)
        {
            var now = DateTimeOffset.UtcNow;
            var entity = new Client
            {
                ClientId = Guid.NewGuid(),
                LawFirmId = request.LawFirmId ?? lawFirmId,
                ClientType = request.ClientType,
                FullName = request.FullName.Trim(),
                PrimaryPhone = request.PrimaryPhone.Trim(),
                SecondaryPhone = request.SecondaryPhone?.Trim(),
                Email = request.Email?.Trim(),
                AddressLine1 = request.AddressLine1?.Trim(),
                AddressLine2 = request.AddressLine2?.Trim(),
                City = request.City?.Trim(),
                State = request.State?.Trim(),
                Pincode = request.Pincode,
                ContactPerson = request.ContactPerson?.Trim(),
                GstNumber = request.GstNumber?.Trim(),
                PanNumber = request.PanNumber?.Trim(),
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId
            };

            await _clientRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<ClientDto> UpdateClientAsync(Guid clientId, UpdateClientRequest request, Guid currentUserId)
        {
            var entity = await _clientRepository.GetByIdAsync(clientId);
            if (entity == null)
            {
                throw new NotFoundException($"Client with ID '{clientId}' was not found.");
            }

            entity.ClientType = request.ClientType;
            entity.FullName = request.FullName.Trim();
            entity.PrimaryPhone = request.PrimaryPhone.Trim();
            entity.SecondaryPhone = request.SecondaryPhone?.Trim();
            entity.Email = request.Email?.Trim();
            entity.AddressLine1 = request.AddressLine1?.Trim();
            entity.AddressLine2 = request.AddressLine2?.Trim();
            entity.City = request.City?.Trim();
            entity.State = request.State?.Trim();
            entity.Pincode = request.Pincode;
            entity.ContactPerson = request.ContactPerson?.Trim();
            entity.GstNumber = request.GstNumber?.Trim();
            entity.PanNumber = request.PanNumber?.Trim();
            entity.Status = request.Status;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.UpdatedBy = currentUserId;

            await _clientRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task DeleteClientAsync(Guid clientId, Guid currentUserId)
        {
            var entity = await _clientRepository.GetByIdAsync(clientId);
            if (entity == null)
            {
                throw new NotFoundException($"Client with ID '{clientId}' was not found.");
            }

            entity.Status = "Archived";
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.UpdatedBy = currentUserId;

            await _clientRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task LinkClientToCaseAsync(Guid caseId, LinkClientToCaseRequest request, Guid currentUserId)
        {
            var caseEntity = await _caseRepository.GetByIdAsync(caseId);
            if (caseEntity == null)
            {
                throw new NotFoundException($"Case with ID '{caseId}' was not found.");
            }

            var clientEntity = await _clientRepository.GetByIdAsync(request.ClientId);
            if (clientEntity == null)
            {
                throw new NotFoundException($"Client with ID '{request.ClientId}' was not found.");
            }

            var existingLink = await _clientRepository.GetCaseClientLinkAsync(caseId, request.ClientId);
            if (existingLink != null)
            {
                existingLink.PartyType = request.PartyType;
                existingLink.PartySequence = request.PartySequence;
                existingLink.IsPrimary = request.IsPrimary;
                existingLink.UpdatedAt = DateTimeOffset.UtcNow;
                existingLink.UpdatedBy = currentUserId;
            }
            else
            {
                var now = DateTimeOffset.UtcNow;
                var link = new CaseClient
                {
                    CaseId = caseId,
                    ClientId = request.ClientId,
                    PartyType = request.PartyType,
                    PartySequence = request.PartySequence,
                    IsPrimary = request.IsPrimary,
                    Status = "Active",
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = currentUserId,
                    UpdatedBy = currentUserId
                };
                await _clientRepository.AddCaseClientLinkAsync(link);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnlinkClientFromCaseAsync(Guid caseId, Guid clientId, Guid currentUserId)
        {
            var link = await _clientRepository.GetCaseClientLinkAsync(caseId, clientId);
            if (link == null)
            {
                throw new NotFoundException("This client is not associated with the specified case.");
            }

            await _clientRepository.RemoveCaseClientLinkAsync(link);
            await _unitOfWork.SaveChangesAsync();
        }

        private static ClientDto MapToDto(Client entity)
        {
            return new ClientDto
            {
                ClientId = entity.ClientId,
                LawFirmId = entity.LawFirmId,
                ClientType = entity.ClientType,
                FullName = entity.FullName,
                PrimaryPhone = entity.PrimaryPhone,
                SecondaryPhone = entity.SecondaryPhone,
                Email = entity.Email,
                AddressLine1 = entity.AddressLine1,
                AddressLine2 = entity.AddressLine2,
                City = entity.City,
                State = entity.State,
                Pincode = entity.Pincode,
                ContactPerson = entity.ContactPerson,
                GstNumber = entity.GstNumber,
                PanNumber = entity.PanNumber,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
