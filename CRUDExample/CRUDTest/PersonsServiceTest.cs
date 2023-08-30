using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CRUDTest;

public class PersonsServiceTest
{
    // Private fields
    private readonly IPersonsService _sut;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;

    // Constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        _sut = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    [Fact]
    // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    public async Task AddPerson_NullPerson()
    {
        // Arrange
        PersonAddRequest? personAddRequest = null;

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.AddPerson(personAddRequest));
    }

    // When we supply null value as PersonName, it should throw ArgumentNullException
    [Fact]
    public void AddPerson_PersonNameIsNull()
    {
        // Arrange
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            Name = null
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _sut.AddPerson(personAddRequest));
    }

    // When we supply proper person details, it should insert the person into the persons list,
    // and it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        // Arrange
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            Name = "Jozsef",
            Email = "jozsef.simon@squarebook.com",
            Address = "Edinburgh",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("1992-02-21"),
            ReceiveNewsLetters = true
        };

        // Act
        PersonResponse personResponseFromAdd = await _sut.AddPerson(personAddRequest);
        List<PersonResponse> personsList = await _sut.GetAllPersons();

        // Assert
        Assert.True(personResponseFromAdd.PersonId != Guid.Empty);

        Assert.Contains(personResponseFromAdd, personsList);
    }

    #endregion

    #region GetPersonByPersonId

    // If we supply null as PersonId, it should return null as PersonResponse
    [Fact]
    public void GetPersonByPersonId_NullPersonId()
    {
        // Arrange
        Guid? personId = null;

        // Act
        var response = _sut.GetPersonByPersonId(personId);

        // Assert
        Assert.Null(response);
    }

    // If we supply a valid person id, it should return the valid person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonId_ShouldReturnCorrectPersonResponse()
    {
        // Arrange
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            CountryName = "UK"
        };
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);


        // Act
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            CountryId = countryResponse.CountryId,
            Email = "nex.simon@gmail.com",
            Address = "Edinburgh",
            DateOfBirth = DateTime.Parse("1992-02-21"),
            ReceiveNewsLetters = true,
            Gender = GenderOptions.Male,
            Name = "Jozsef"
        };
        PersonResponse personAddResponse = await _sut.AddPerson(personAddRequest);

        PersonResponse? response = await _sut.GetPersonByPersonId(personAddResponse.PersonId);

        // Assert
        Assert.Equal(personAddResponse, response);
    }

    #endregion

    #region GetAllPersons

    // The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        // Act
        List<PersonResponse> responseFromGetAllPersons = await _sut.GetAllPersons();

        // Assert
        Assert.Empty(responseFromGetAllPersons);
    }

    // It should return the same list of PersonResponse
    [Fact]
    public async Task GetAllPersons_ShouldReturnAllPersonResponses()
    {
        // Arrange
        CountryAddRequest countryCodeAddRequest1 = new CountryAddRequest()
        {
            CountryName = "Hungary"
        };
        CountryAddRequest countryCodeAddRequest2 = new CountryAddRequest()
        {
            CountryName = "Spain"
        };
        CountryAddRequest countryCodeAddRequest3 = new CountryAddRequest()
        {
            CountryName = "United Kingdom"
        };

        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryCodeAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryCodeAddRequest2);
        CountryResponse countryResponse3 = await _countriesService.AddCountry(countryCodeAddRequest3);

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            CountryId = countryResponse1.CountryId,
            Address = "Edinburgh",
            DateOfBirth = DateTime.Parse("1992-02-21"),
            Email = "test@test.com",
            Gender = GenderOptions.Male,
            Name = "Jozsef",
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            CountryId = countryResponse2.CountryId,
            Address = "Mallorca",
            DateOfBirth = DateTime.Parse("1988-08-08"),
            Email = "test@test.com",
            Gender = GenderOptions.Female,
            Name = "Paula",
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            CountryId = countryResponse3.CountryId,
            Address = "Winchester",
            DateOfBirth = DateTime.Parse("1992-01-29"),
            Email = "test@test.com",
            Gender = GenderOptions.Male,
            Name = "Dani",
            ReceiveNewsLetters = true,
        };

        PersonResponse personResponse1 = await _sut.AddPerson(personAddRequest1);
        PersonResponse personResponse2 = await _sut.AddPerson(personAddRequest2);
        PersonResponse personResponse3 = await _sut.AddPerson(personAddRequest3);

        List<PersonResponse> expectedPersonResponsesList = new List<PersonResponse>();
        expectedPersonResponsesList.Add(personResponse1);
        expectedPersonResponsesList.Add(personResponse2);
        expectedPersonResponsesList.Add(personResponse3);

        // Print list from expected
        _testOutputHelper.WriteLine("Expected:");
        foreach (var personResponse in expectedPersonResponsesList)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }

        // Act
        List<PersonResponse> actualPersonResponseList = await _sut.GetAllPersons();

        _testOutputHelper.WriteLine("Actual:");
        // Print list from actual
        foreach (var personResponse in actualPersonResponseList)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }

        // Assert
        Assert.Equal(expectedPersonResponsesList, actualPersonResponseList);

    }


    #endregion

    #region GetFilteredPersons

    // If the search text is empty and search by is 'PersonName', should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
        // Arrange
        CountryAddRequest countryCodeAddRequest1 = new CountryAddRequest()
        {
            CountryName = "Hungary"
        };
        CountryAddRequest countryCodeAddRequest2 = new CountryAddRequest()
        {
            CountryName = "Spain"
        };
        CountryAddRequest countryCodeAddRequest3 = new CountryAddRequest()
        {
            CountryName = "United Kingdom"
        };

        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryCodeAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryCodeAddRequest2);
        CountryResponse countryResponse3 = await _countriesService.AddCountry(countryCodeAddRequest3);

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            CountryId = countryResponse1.CountryId,
            Address = "Edinburgh",
            DateOfBirth = DateTime.Parse("1992-02-21"),
            Email = "test@test.com",
            Gender = GenderOptions.Male,
            Name = "Jozsef",
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            CountryId = countryResponse2.CountryId,
            Address = "Mallorca",
            DateOfBirth = DateTime.Parse("1988-08-08"),
            Email = "test@test.com",
            Gender = GenderOptions.Female,
            Name = "Paula",
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            CountryId = countryResponse3.CountryId,
            Address = "Winchester",
            DateOfBirth = DateTime.Parse("1992-01-29"),
            Email = "test@test.com",
            Gender = GenderOptions.Male,
            Name = "Dani",
            ReceiveNewsLetters = true,
        };

        PersonResponse personResponse1 = await _sut.AddPerson(personAddRequest1);
        PersonResponse personResponse2 = await _sut.AddPerson(personAddRequest2);
        PersonResponse personResponse3 = await _sut.AddPerson(personAddRequest3);

        List<PersonResponse> expectedPersonResponsesList = new List<PersonResponse>();
        expectedPersonResponsesList.Add(personResponse1);
        expectedPersonResponsesList.Add(personResponse2);
        expectedPersonResponsesList.Add(personResponse3);

        // Print list from expected
        _testOutputHelper.WriteLine("Expected:");
        foreach (var personResponse in expectedPersonResponsesList)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }

        // Act
        List<PersonResponse> responseFromGetFilteredPersons = await _sut.GetFilteredPersons(nameof(Person.Name), "");

        // Print list from actual 
        _testOutputHelper.WriteLine("Actual:");
        foreach (var personResponse in responseFromGetFilteredPersons)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }

        // Assert
        foreach (PersonResponse personResponse in expectedPersonResponsesList)
        {
            Assert.Contains(personResponse, responseFromGetFilteredPersons);
        }
    }

    // First we will add few persons; and then we will search based on person name with some search string.
    // It should return the matching person
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
        // Arrange
        CountryAddRequest countryCodeAddRequest1 = new CountryAddRequest()
        {
            CountryName = "Hungary"
        };
        CountryAddRequest countryCodeAddRequest2 = new CountryAddRequest()
        {
            CountryName = "Spain"
        };
        CountryAddRequest countryCodeAddRequest3 = new CountryAddRequest()
        {
            CountryName = "United Kingdom"
        };

        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryCodeAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryCodeAddRequest2);
        CountryResponse countryResponse3 = await _countriesService.AddCountry(countryCodeAddRequest3);

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            CountryId = countryResponse1.CountryId,
            Address = "Edinburgh",
            DateOfBirth = DateTime.Parse("1992-02-21"),
            Email = "test@test.com",
            Gender = GenderOptions.Male,
            Name = "Jozsef",
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            CountryId = countryResponse2.CountryId,
            Address = "Mallorca",
            DateOfBirth = DateTime.Parse("1988-08-08"),
            Email = "test@test.com",
            Gender = GenderOptions.Female,
            Name = "Paula",
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            CountryId = countryResponse3.CountryId,
            Address = "Winchester",
            DateOfBirth = DateTime.Parse("1992-01-29"),
            Email = "test@test.com",
            Gender = GenderOptions.Male,
            Name = "Joseph",
            ReceiveNewsLetters = true,
        };

        PersonResponse personResponse1 = await _sut.AddPerson(personAddRequest1);
        PersonResponse personResponse2 = await _sut.AddPerson(personAddRequest2);
        PersonResponse personResponse3 = await _sut.AddPerson(personAddRequest3);

        List<PersonResponse> expectedPersonResponsesList = new List<PersonResponse>();
        expectedPersonResponsesList.Add(personResponse1);
        expectedPersonResponsesList.Add(personResponse2);
        expectedPersonResponsesList.Add(personResponse3);

        // Print list from expected
        _testOutputHelper.WriteLine("Expected:");
        foreach (var personResponse in expectedPersonResponsesList)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }

        // Act
        List<PersonResponse> responseFromGetFilteredPersons = await _sut.GetFilteredPersons(nameof(Person.Name), "Jo");

        // Print list from actual 
        _testOutputHelper.WriteLine("Actual:");
        foreach (var personResponse in responseFromGetFilteredPersons)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }

        // Assert
        foreach (PersonResponse personResponse in responseFromGetFilteredPersons)
        {
            if (personResponse.Name != null)
            {
                if (personResponse.Name.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(personResponse, responseFromGetFilteredPersons);
                }
            }
        }
    }

    #endregion

    #region GetSortedPersons

    // When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons()
    {
        // Arrange
        CountryAddRequest countryCodeAddRequest1 = new CountryAddRequest()
        {
            CountryName = "Hungary"
        };
        CountryAddRequest countryCodeAddRequest2 = new CountryAddRequest()
        {
            CountryName = "Spain"
        };
        CountryAddRequest countryCodeAddRequest3 = new CountryAddRequest()
        {
            CountryName = "United Kingdom"
        };

        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryCodeAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryCodeAddRequest2);
        CountryResponse countryResponse3 = await _countriesService.AddCountry(countryCodeAddRequest3);

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            CountryId = countryResponse1.CountryId,
            Address = "Edinburgh",
            DateOfBirth = DateTime.Parse("1992-02-21"),
            Email = "test@test.com",
            Gender = GenderOptions.Male,
            Name = "Jozsef",
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            CountryId = countryResponse2.CountryId,
            Address = "Mallorca",
            DateOfBirth = DateTime.Parse("1988-08-08"),
            Email = "test@test.com",
            Gender = GenderOptions.Female,
            Name = "Paula",
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            CountryId = countryResponse3.CountryId,
            Address = "Winchester",
            DateOfBirth = DateTime.Parse("1992-01-29"),
            Email = "test@test.com",
            Gender = GenderOptions.Male,
            Name = "Dani",
            ReceiveNewsLetters = true,
        };

        PersonResponse personResponse1 = await _sut.AddPerson(personAddRequest1);
        PersonResponse personResponse2 = await _sut.AddPerson(personAddRequest2);
        PersonResponse personResponse3 = await _sut.AddPerson(personAddRequest3);

        List<PersonResponse> personResponseListToSort = new List<PersonResponse>();
        personResponseListToSort.Add(personResponse1);
        personResponseListToSort.Add(personResponse2);
        personResponseListToSort.Add(personResponse3);

        // Print list from expected
        _testOutputHelper.WriteLine("Expected:");
        foreach (var personResponse in personResponseListToSort)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }

        // Act
        List<PersonResponse> responseFromGetSortedPersons = await _sut.GetSortedPersons(personResponseListToSort, nameof(Person.Name), SortOrderOptions.DESC);

        // Print list from actual 
        _testOutputHelper.WriteLine("Actual:");
        foreach (var personResponse in responseFromGetSortedPersons)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }

        personResponseListToSort = personResponseListToSort.OrderByDescending(temp => temp.Name).ToList();

        // Assert
        for (int i = 0; i < personResponseListToSort.Count; i++)
        {
            Assert.Equal(personResponseListToSort[i], responseFromGetSortedPersons[i]);
        }
    }
    #endregion

    #region UpdatePerson

    // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public void UpdatePerson_NullPerson()
    {
        // Arrange
        PersonUpdateRequest? personUpdateRequest = null;

        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            await _sut.UpdatePerson(personUpdateRequest);
        });
    }

    // When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public void UpdatePerson_InvalidPersonId()
    {
        // Arrange
        PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
        {
            PersonId = Guid.NewGuid()
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _sut.UpdatePerson(personUpdateRequest);
        });
    }

    // When Person name is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_NullPersonName()
    {
        // Arrange
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            CountryName = "UK"
        };

        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            Name = "Jozsef",
            Email = "jozsef.simon@squarebook.com",
            Address = countryResponse.CountryName,
            CountryId = countryResponse.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("1992-02-21"),
            ReceiveNewsLetters = true
        };

        PersonResponse personAddResponse = await _sut.AddPerson(personAddRequest);


        PersonUpdateRequest personUpdateRequest = personAddResponse.ToPersonUpdateRequest();
        personUpdateRequest.Name = null;

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _sut.UpdatePerson(personUpdateRequest);
        });
    }

    // First, add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_PersonFullDetails()
    {
        // Arrange
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            CountryName = "UK"
        };

        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            Name = "Jozsef",
            Email = "jozsef.simon@squarebook.com",
            Address = countryResponse.CountryName,
            CountryId = countryResponse.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("1992-02-21"),
            ReceiveNewsLetters = true
        };

        PersonResponse personAddResponse = await _sut.AddPerson(personAddRequest);


        PersonUpdateRequest personUpdateRequest = personAddResponse.ToPersonUpdateRequest();
        personUpdateRequest.Name = "William";
        personUpdateRequest.Name = "william@gmail.com";


        PersonResponse personResponseFromUpdateRequest = await _sut.UpdatePerson(personUpdateRequest);

        PersonResponse personResponseFromGet = await _sut.GetPersonByPersonId(personResponseFromUpdateRequest.PersonId);

        // Assert
        Assert.Equal(personResponseFromGet, personResponseFromUpdateRequest);
    }

    #endregion

    #region DeletePerson

    // If we supply valid person id the results should be true
    [Fact]
    public async Task DeletePerson_ValidPersonId()
    {
        // Arrange
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Hungary"
        };

        CountryResponse countryResponse =await _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            Name = "Jozsef",
            Email = "jozsef.simon@squarebook.com",
            Address = "Edinburgh",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("1992-02-21"),
            ReceiveNewsLetters = true
        };

        PersonResponse personResponse = await _sut.AddPerson(personAddRequest);

        // Act
        bool isDeleted = await _sut.DeletePerson(personResponse.PersonId);

        // Assert
        Assert.True(isDeleted);
    }

    // If we supply invalid person id the results should be false
    [Fact]
    public async Task DeletePerson_InvalidPersonId()
    {

        // Act
        bool isDeleted = await _sut.DeletePerson(Guid.NewGuid());

        // Assert
        Assert.False(isDeleted);
    }

    #endregion
}