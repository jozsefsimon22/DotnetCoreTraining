using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly PersonsDbContext _db;

    public CountriesService(PersonsDbContext personsDbContext)
    {
        _db = personsDbContext;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        // Validation: countryAddRequest can't be null
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }

        // Validation: CountryName can't be null
        if (countryAddRequest.CountryName == null)
        {
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        }

        // Validation: CountryName can't be duplicate
        if (await _db.Countries.CountAsync(c => c.CountryName == countryAddRequest.CountryName) > 0)
        {
            throw new ArgumentException("Given country name already exists");
        }

        // Convert object from countryAddRequest to Country
        Country country = countryAddRequest.ToCountry();

        // Generate CountryId
        country.CountryId = Guid.NewGuid();

        // Add country object into _db
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {

        if (countryId == null)
        {
            return null;
        }

        var countryResponseFromList = await _db.Countries.FirstOrDefaultAsync(country => country.CountryId == countryId);

        if (countryResponseFromList == null)
        {
            return null;
        }

        return countryResponseFromList.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
        int countriesInserted = 0;

        MemoryStream memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            // Reading "Countries" worksheet
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];

            // Checking number of rows in the file
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);

                if (!string.IsNullOrEmpty(cellValue))
                {
                    string? countryName = cellValue;

                    if (_db.Countries.Count(temp => temp.CountryName == countryName) == 0)
                    {
                        Country country = new Country()
                        {
                            CountryName = countryName
                        };

                        _db.Countries.Add(country);
                        await _db.SaveChangesAsync();

                        countriesInserted++;
                    }
                }
            }
        }
        return countriesInserted;
    }
}