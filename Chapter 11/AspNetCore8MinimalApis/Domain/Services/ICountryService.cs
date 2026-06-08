using Domain.DTOs;
using System.Reflection;

namespace Domain.Services;

public interface ICountryService
{
    Task<List<CountryDto>> GetAllAsync(PagingDto paging);
    Task<CountryDto?> GetByIdAsync(int id);
    Task<CountryDto> CreateAsync(CountryDto dto);
    Task UpdateAsync(CountryDto dto);
    Task DeleteAsync(int id);
    Task LongRunningQueryAsync(CancellationToken cancellationToken);
    Task<bool> IngestFileAsync(Stream countryFileContent);
    Task<(byte[], string, string)> GetFileAsync();
}
