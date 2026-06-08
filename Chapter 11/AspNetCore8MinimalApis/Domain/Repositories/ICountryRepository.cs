using Domain.DTOs;

namespace Domain.Repositories;

public interface ICountryRepository
{
    Task<List<CountryDto>> GetAllAsync(PagingDto paging);
    Task<CountryDto?> GetByIdAsync(int id);
    Task<CountryDto> CreateAsync(CountryDto dto);
    Task UpdateAsync(CountryDto dto);
    Task DeleteAsync(int id);
    Task LongRunningQueryAsync(CancellationToken cancellationToken);
}