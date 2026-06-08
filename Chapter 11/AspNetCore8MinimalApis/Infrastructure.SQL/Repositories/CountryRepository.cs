using Domain.DTOs;
using Domain.Repositories;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Database.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly DemoContext _demoContext;

    public CountryRepository(DemoContext demoContext)
    {
        _demoContext = demoContext;
    }

    public async Task<List<CountryDto>> GetAllAsync(PagingDto paging)
    {
        return (await _demoContext.Countries
                                 .AsNoTracking()
                                 .Select(x => new CountryDto
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     Description = x.Description,
                                     FlagUri = x.FlagUri
                                 })
                                 .Skip((paging.PageIndex - 1) * paging.PageSize)
                                 .Take(paging.PageSize)
                                 .ToListAsync());
    }

    public async Task<CountryDto?> GetByIdAsync(int id)
    {
        var entity = await _demoContext.Countries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return null;
        return new CountryDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, FlagUri = entity.FlagUri };
    }

    public async Task<CountryDto> CreateAsync(CountryDto dto)
    {
        var entity = new CountryEntity { Name = dto.Name, Description = dto.Description, FlagUri = dto.FlagUri };
        _demoContext.Countries.Add(entity);
        await _demoContext.SaveChangesAsync();
        dto.Id = entity.Id;
        return dto;
    }

    public async Task UpdateAsync(CountryDto dto)
    {
        var entity = await _demoContext.Countries.FindAsync(dto.Id);
        if (entity == null) throw new KeyNotFoundException("Country not found");
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.FlagUri = dto.FlagUri;
        await _demoContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _demoContext.Countries.FindAsync(id);
        if (entity == null) return;
        _demoContext.Countries.Remove(entity);
        await _demoContext.SaveChangesAsync();
    }

    public async Task LongRunningQueryAsync(CancellationToken cancellationToken)
    {
        await _demoContext.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:10'", cancellationToken: cancellationToken);
    }
}
