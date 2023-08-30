using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    // Private fields
    private readonly PersonsDbContext _db;
    private readonly ICountriesService _countriesService;

    // Constructor
    public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
    {
        _db = personsDbContext;
        _countriesService = countriesService;

    }


    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        // Check if personAddRequest is not null
        if (personAddRequest == null)
        {
            throw new ArgumentNullException(nameof(personAddRequest));
        }

        // Validate PersonName
        ValidationHelper.ModelValidation(personAddRequest);

        // Convert personAddRequest into Person type
        Person person = personAddRequest.ToPerson();

        // Generate PersonId
        person.PersonId = Guid.NewGuid();

        // Add person to persons list
        _db.Add(person);
        await _db.SaveChangesAsync();

        // Convert person object to personResponse
        PersonResponse personResponse = person.ToPersonResponse();

        return personResponse;

    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var persons = await _db.Persons.Include("Country").ToListAsync();
        return persons.Select(temp => temp.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse> GetPersonByPersonId(Guid? personId)
    {
        if (personId == null)
        {
            return null;
        }

        Person? matchPerson = await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonId == personId);

        if (matchPerson == null)
        {
            return null;
        }

        PersonResponse response = matchPerson.ToPersonResponse();

        return response;
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersonResponses = await GetAllPersons();
        List<PersonResponse> matchingPersonResponses = allPersonResponses;

        if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
        {
            return matchingPersonResponses;
        }

        switch (searchBy)
        {
            case nameof(PersonResponse.Name):
                matchingPersonResponses = allPersonResponses.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Name) ? temp.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            case nameof(PersonResponse.Email):
                matchingPersonResponses = allPersonResponses.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            case nameof(PersonResponse.DateOfBirth):
                matchingPersonResponses = allPersonResponses.Where(temp =>
                    ((temp.DateOfBirth != null) ? temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            case nameof(PersonResponse.Gender):
                matchingPersonResponses = allPersonResponses.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            case nameof(PersonResponse.CountryId):
                matchingPersonResponses = allPersonResponses.Where(temp =>
                    (!string.IsNullOrEmpty(temp.CountryName) ? temp.CountryName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            case nameof(PersonResponse.Address):
                matchingPersonResponses = allPersonResponses.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            default:
                matchingPersonResponses = allPersonResponses;
                break;
        }
        return matchingPersonResponses;
    }

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return allPersons;
        }

        List<PersonResponse> sortedPersonResponses = (sortBy, sortOrder) switch
        {
            // By name
            (nameof(PersonResponse.Name), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Name), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),

            // By Email
            (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

            // By DateOfBirth
            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

            // By Age
            (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
            (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

            // By Gender
            (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender).ToList(),
            (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender).ToList(),

            // By Country
            (nameof(PersonResponse.CountryName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.CountryName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

            // By Address
            (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

            // By Newsletter
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

            // Default case
            _ => allPersons
        };

        return sortedPersonResponses;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(personUpdateRequest));
        }

        // Validation
        ValidationHelper.ModelValidation(personUpdateRequest);

        // Get matching person object to update
        Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personUpdateRequest.PersonId);

        if (matchingPerson == null)
        {
            throw new ArgumentException("Given person id doesn't exits");
        }

        // Update all details
        matchingPerson.Name = personUpdateRequest.Name;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString();
        matchingPerson.CountryId = personUpdateRequest.CountryId;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        await _db.SaveChangesAsync();
        return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
        {
            return false;
        }

        Person person = _db.Persons.FirstOrDefault(x => x.PersonId == personId);

        if (person == null)
        {
            return false;
        }

        _db.Persons.Remove(_db.Persons.First(temp => temp.PersonId == personId));
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<MemoryStream> GetPersonCSV()
    {
        // Setup
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream);


        CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
        CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);


        List<PersonResponse> persons = await _db.Persons
            .Include("Country")
            .Select(temp => temp.ToPersonResponse()).ToListAsync();

        // WriteHeader writes the headers automatically based on the model properties 
        csvWriter.WriteField((nameof(PersonResponse.PersonId)));
        csvWriter.WriteField((nameof(PersonResponse.Name)));
        csvWriter.WriteField((nameof(PersonResponse.Email)));
        csvWriter.WriteField((nameof(PersonResponse.DateOfBirth)));
        csvWriter.WriteField((nameof(PersonResponse.Age)));
        csvWriter.WriteField((nameof(PersonResponse.Gender)));
        csvWriter.WriteField((nameof(PersonResponse.CountryName)));
        csvWriter.WriteField((nameof(PersonResponse.Address)));
        csvWriter.WriteField((nameof(PersonResponse.ReceiveNewsLetters)));
        await csvWriter.NextRecordAsync(); // Moving to the next record

        foreach (var person in persons)
        {
            csvWriter.WriteField((person.PersonId));
            csvWriter.WriteField((person.Name));
            csvWriter.WriteField((person.Email));
            if (person.DateOfBirth.HasValue)
            {
                csvWriter.WriteField((person.DateOfBirth.Value.ToString("yyyy-MM-dd")));
            }
            else
            {
                csvWriter.WriteField("");
            }
            csvWriter.WriteField((person.Age));
            csvWriter.WriteField((person.Gender));
            csvWriter.WriteField((person.CountryName));
            csvWriter.WriteField((person.Address));
            csvWriter.WriteField((person.ReceiveNewsLetters));
            await csvWriter.NextRecordAsync();
            await csvWriter.FlushAsync();
        }

        // Moving the cursors back to the beginning of the stream
        memoryStream.Position = 0;

        return memoryStream;
    }
}