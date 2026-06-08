using AspNetCore8MinimalApis.Mapping.Interfaces;
using AspNetCore8MinimalApis.Models;
using Domain.DTOs;

namespace AspNetCore8MinimalApis.Mapping;

public class CountryMapper : ICountryMapper
{
    public CountryDto Map(Country country)
    {
        if (country == null) return null;
        return new CountryDto
        {
            Id = country.Id ?? 0,
            Name = country.Name,
            Description = country.Description,
            FlagUri = country.FlagUri,
        };
    }

    public Country Map(CountryDto country)
    {
        if (country == null) return null;
        return new Country
        {
            Id = country.Id,
            Name = country.Name,
            Description = country.Description,
            FlagUri = country.FlagUri,
        };
    }

    public List<Country> Map(List<CountryDto> countries)
    {
        return countries.Select(Map).ToList();
    }
}
