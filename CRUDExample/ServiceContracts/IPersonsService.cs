using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Person entity
/// </summary>
public interface IPersonsService
{
    /// <summary>
    /// Adds a new person into the list of persons
    /// </summary>
    /// <param name="personAddRequest">Person to add</param>
    /// <returns>Returns the same person details, along with newly generated PersonId</returns>
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    /// Returns all persons
    /// </summary>
    /// <returns>Returns a list of objects of PersonResponse</returns>
    Task<List<PersonResponse>> GetAllPersons();   

    /// <summary>
    /// Returns PersonResponse based on the personId
    /// </summary>
    /// <param name="personId">The person's personId</param>
    /// <returns>PersonResponse object</returns>
    Task<PersonResponse> GetPersonByPersonId(Guid? personId);

    /// <summary>
    /// Returns a list of PersonResponse that matches with the search results
    /// </summary>
    /// <param name="searchBy">Field to search</param>
    /// <param name="searchString">String to search</param>
    /// <returns>Returns all matching persons based on the search string and search by</returns>
    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

    /// <summary>
    /// Returns a list of sorted persons 
    /// </summary>
    /// <param name="allPersons">Represents a list of persons to sort</param>
    /// <param name="sortBy">Name of property, based on which the persons should be sorted</param>
    /// <param name="sortOrder">ASC or DESC</param>
    /// <returns>Returns sorted persons as PersonResponse list</returns>
    Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

    /// <summary>
    /// Updates teh specified person details based on the give person Id
    /// </summary>
    /// <param name="personUpdateRequest">Person details to update, including person id</param>
    /// <returns>Returns the person response object after the update</returns>
    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    /// <summary>
    /// Deletes a person identified by the personId
    /// </summary>
    /// <param name="personId">personId to Delete</param>
    /// <returns>Returns true if the deletion is completed, otherwise false</returns>
    Task<bool> DeletePerson(Guid?  personId);

    /// <summary>
    /// Returns persons as CSV
    /// </summary>
    /// <returns>Returns the memory stream with CSV data</returns>
    Task<MemoryStream> GetPersonCSV();
}