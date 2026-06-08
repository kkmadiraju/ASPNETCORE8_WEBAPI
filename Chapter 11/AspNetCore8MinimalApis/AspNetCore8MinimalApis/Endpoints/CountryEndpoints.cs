using AspNetCore8MinimalApis.Mapping.Interfaces;
using AspNetCore8MinimalApis.Models;
using Domain.DTOs;
using Domain.Services;

namespace AspNetCore8MinimalApis.Endpoints;

public static class CountryEndpoints
{
    /// <summary>
    /// Get a paged list of countries.
    /// </summary>
    /// <param name="pageIndex">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>List of countries</returns>
    public static async Task<IResult> GetCountries(int? pageIndex, int? pageSize, ICountryMapper mapper, ICountryService countryService) 
    {
        var paging = new PagingDto
        {
            PageIndex = pageIndex.HasValue ? pageIndex.Value : 1,
            PageSize = pageSize.HasValue ? pageSize.Value : 10
        };
        var countries = await countryService.GetAllAsync(paging);

        return Results.Ok(mapper.Map(countries));
    }

    /// <summary>
    /// Get country by id
    /// </summary>
    public static async Task<IResult> GetCountryById(int id, ICountryMapper mapper, ICountryService countryService)
    {
        var country = await countryService.GetByIdAsync(id);
        if (country == null) return Results.NotFound();
        return Results.Ok(mapper.Map(country));
    }

    /// <summary>
    /// Create a new country
    /// </summary>
    public static async Task<IResult> CreateCountry(Country country, ICountryMapper mapper, ICountryService countryService)
    {
        var dto = mapper.Map(country);
        var created = await countryService.CreateAsync(dto);
        return Results.Created($"/countries/{created.Id}", mapper.Map(created));
    }

    /// <summary>
    /// Update an existing country
    /// </summary>
    public static async Task<IResult> UpdateCountry(int id, Country country, ICountryMapper mapper, ICountryService countryService)
    {
        if (id != country.Id) return Results.BadRequest();
        var dto = mapper.Map(country);
        await countryService.UpdateAsync(dto);
        return Results.NoContent();
    }

    /// <summary>
    /// Delete a country by id
    /// </summary>
    public static async Task<IResult> DeleteCountry(int id, ICountryService countryService)
    {
        await countryService.DeleteAsync(id);
        return Results.NoContent();
    }
}
