using Domain.DTOs;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System;


namespace BLL.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }
 
    public async Task<List<CountryDto>> GetAllAsync(PagingDto paging)
    {
        return await _countryRepository.GetAllAsync(paging);
    }

    public async Task<CountryDto?> GetByIdAsync(int id)
    {
        return await _countryRepository.GetByIdAsync(id);
    }

    public async Task<CountryDto> CreateAsync(CountryDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        // basic validation
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required", nameof(dto.Name));
        if (string.IsNullOrWhiteSpace(dto.Description)) throw new ArgumentException("Description is required", nameof(dto.Description));

        // prevent duplicate name (case-insensitive)
        var existing = await _countryRepository.GetAllAsync(new PagingDto { PageIndex = 1, PageSize = 1000 });
        if (existing.Any(x => string.Equals(x.Name, dto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A country with name '{dto.Name}' already exists.");
        }

        return await _countryRepository.CreateAsync(dto);
    }

    public async Task UpdateAsync(CountryDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        var existing = await _countryRepository.GetByIdAsync(dto.Id);
        if (existing == null) throw new KeyNotFoundException("Country not found");

        // check name conflict with other records
        var all = await _countryRepository.GetAllAsync(new PagingDto { PageIndex = 1, PageSize = 1000 });
        if (all.Any(x => x.Id != dto.Id && string.Equals(x.Name, dto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Another country with name '{dto.Name}' already exists.");
        }

        await _countryRepository.UpdateAsync(dto);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _countryRepository.GetByIdAsync(id);
        if (existing == null) return;
        await _countryRepository.DeleteAsync(id);
    }

    public async Task LongRunningQueryAsync(CancellationToken cancellationToken)
    {
        await _countryRepository.LongRunningQueryAsync(cancellationToken);
    }

    public async Task<bool> IngestFileAsync(Stream countryFileContent)
    {
        throw new NotImplementedException();
    }

    public async Task<(byte[], string, string)> GetFileAsync()
    {
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"beach.png");
        return (await File.ReadAllBytesAsync(path), "image/png", "beach.png");
    }
}
