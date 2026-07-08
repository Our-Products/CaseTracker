using CaseTracker.Shared.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseTrackerApp.Services;

public class MockCaseApi : ICaseApi
{
    private readonly List<CaseDto> _items = new();
    private int _nextId = 1;

    public MockCaseApi()
    {
        _items.Add(new CaseDto { Id = _nextId++, CaseNumber = "CRL/254/2026", Title = "Rajesh vs State", Opponent = "State", NextHearingDate = DateTime.Today.AddDays(7), Status = "Active", Description = "Sample description 1" });
        _items.Add(new CaseDto { Id = _nextId++, CaseNumber = "CRL/255/2026", Title = "Anita vs State", Opponent = "State", NextHearingDate = DateTime.Today.AddDays(14), Status = "Active", Description = "Sample description 2" });
        _items.Add(new CaseDto { Id = _nextId++, CaseNumber = "CRL/256/2026", Title = "Suresh vs State", Opponent = "State", NextHearingDate = DateTime.Today.AddDays(21), Status = "Active", Description = "Sample description 3" });
    }

    public Task<CaseDto> CreateCaseAsync(CaseDto item)
    {
        var newItem = item with { Id = _nextId++ };
        _items.Add(newItem);
        return Task.FromResult(newItem);
    }

    public Task DeleteCaseAsync(int id)
    {
        var existing = _items.FirstOrDefault(x => x.Id == id);
        if (existing != null)
            _items.Remove(existing);
        return Task.CompletedTask;
    }

    public Task<CaseDto?> GetCaseAsync(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(item);
    }

    public Task<List<CaseDto>> GetCasesAsync()
    {
        return Task.FromResult(_items.ToList());
    }

    public Task UpdateCaseAsync(CaseDto item)
    {
        var existing = _items.FirstOrDefault(x => x.Id == item.Id);
        if (existing != null)
        {
            _items.Remove(existing);
            _items.Add(item);
        }
        return Task.CompletedTask;
    }
}
