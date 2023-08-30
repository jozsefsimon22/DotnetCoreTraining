using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Country entity
/// </summary>
/// 
public interface ICountriesService
{
    /// <summary>
    /// Ads a country object to the list of countries
    /// </summary>
    /// <param name="countryAddRequest">Country object to add</param>
    /// <returns>Returns the country object after adding it (including newly generated country id)</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest countryAddRequest);

    /// <summary>
    /// Returns all countries from the list
    /// </summary>
    /// <returns>Returns all countries from the list as List of CountryResponse</returns>
    Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    /// Returns a CountryResponse based on the countryId
    /// </summary>
    /// <param name="countryId">CountryId (guid) to search</param>
    /// <returns>CountryResponse</returns>
    Task<CountryResponse?> GetCountryByCountryId (Guid?  countryId);

    /// <summary>
    /// Uploads countries from excel file into database
    /// </summary>
    /// <param name="formFile">Excel file with list of countries</param>
    /// <returns>Returns number of countries added</returns>
    Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
}

