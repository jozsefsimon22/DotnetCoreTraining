using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTest;

public class CountriesServiceTest
{
    private readonly ICountriesService _sut;

    // Constructor
    public CountriesServiceTest()
    {
        _sut = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
    }

    #region AddCountry
    // When CountryAddRequest is null, it should ArgumentNullException
    [Fact]
    public void AddCountry_NullCountry()
    {
        // Arrange
        CountryAddRequest? request = null;

        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            await _sut.AddCountry(request);
        });
    }

    // When the CountryName is null, it should throw ArgumentException
    [Fact]
    public void AddCountry_CountryNameIsNull()
    {
        // Arrange
        CountryAddRequest? request = new CountryAddRequest()
        {
            CountryName = null
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _sut.AddCountry(request);
        });
    }

    // When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsDuplicate()
    {
        // Arrange
        CountryAddRequest? request = new CountryAddRequest()
        {
            CountryName = "UK"
        };

        CountryAddRequest? request2 = new CountryAddRequest()
        {
            CountryName = "UK"
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _sut.AddCountry(request);
            await _sut.AddCountry(request2);
        });
    }

    // When you supply proper CountryName, it should insert the same into the existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        // Arrange
        CountryAddRequest? request = new CountryAddRequest()
        {
            CountryName = "Japan"
        };

        // Act
        CountryResponse response = await _sut.AddCountry(request);
        List<CountryResponse> countriesFromGetAllCountries = await _sut.GetAllCountries();

        // Assert
        Assert.True(response.CountryId != Guid.Empty);
        Assert.Contains(response, countriesFromGetAllCountries);
    }
    #endregion

    #region GetAllCountries

    [Fact]
    // The list of countries should be empty by default (before adding any countries)
    public async Task GetAllCountries_EmptyList()
    {
        // Acts
        List<CountryResponse> actualCountryResponsesList = await _sut.GetAllCountries();

        // Assert
        Assert.Empty(actualCountryResponsesList);
    }

    [Fact]
    // 
    public async Task GetAllCountries_AddFewCountries()
    {
        // Arrange 
        List<CountryAddRequest> countryAddRequestList = new List<CountryAddRequest>()
        {
            new CountryAddRequest() { CountryName = "United Kingdom" },
            new CountryAddRequest() { CountryName = "USA" },
        };

        // Act
        List<CountryResponse> countriesListFromAddCountryRequestList = new List<CountryResponse>();

        foreach (var countryAddRequest in countryAddRequestList)
        {
            countriesListFromAddCountryRequestList.Add(await _sut.AddCountry(countryAddRequest));
        }

        List<CountryResponse> actualCountryResponses = await _sut.GetAllCountries();

        foreach (var expectedCountry in countriesListFromAddCountryRequestList)
        {
            Assert.Contains(expectedCountry, actualCountryResponses);
        }
    }

    #endregion

    #region GetCountryByCountryId

    [Fact]
    public async Task GetCountryByCountryId_ShouldReturnNull()
    {
        // Arrange
        Guid? countryId = null;

        // Act
        CountryResponse? countryResponseFromGetMethod = await _sut.GetCountryByCountryId(countryId);

        // Assert
        Assert.Null(countryResponseFromGetMethod);
    }

    [Fact]
    // If we supply a valid country id, it should return the matching country details as CountryResponse object
    public async Task GetCountryByCountryId_ValidCountryId()
    {
        // Arrange
        CountryAddRequest? countryAddRequest = new CountryAddRequest()
        {
            CountryName = "China"
        };

        CountryResponse responseFromAdd = await _sut.AddCountry(countryAddRequest);

        // Act
        CountryResponse? responseFromGet = await _sut.GetCountryByCountryId(responseFromAdd.CountryId);

        // Assert
        Assert.Equal(responseFromAdd, responseFromGet);

    }

    #endregion
}