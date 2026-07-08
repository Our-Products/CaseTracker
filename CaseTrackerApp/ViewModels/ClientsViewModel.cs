using CommunityToolkit.Mvvm.ComponentModel;
using CaseTracker.Shared.Dtos;
using System.Collections.ObjectModel;

namespace CaseTrackerApp.ViewModels;

public partial class ClientsViewModel : ViewModelBase
{
    public ObservableCollection<ClientDto> Clients { get; } = new();

    public ClientsViewModel()
    {
        // Seed dummy clients
        Clients.Add(new ClientDto { Id = 1, Name = "Rajesh Kumar", Email = "rajesh@example.com", Phone = "1234567890" });
        Clients.Add(new ClientDto { Id = 2, Name = "Anita Sharma", Email = "anita@example.com", Phone = "0987654321" });
        Clients.Add(new ClientDto { Id = 3, Name = "Suresh Patel", Email = "suresh@example.com", Phone = "5555555555" });
    }
}
