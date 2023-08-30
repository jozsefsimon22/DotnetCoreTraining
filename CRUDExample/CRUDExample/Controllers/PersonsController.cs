using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string searchString, string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            // Search
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                {nameof(PersonResponse.Name), "Person Name"},
                {nameof(PersonResponse.Email), "Email"},
                {nameof(PersonResponse.Age), "Age"},
                {nameof(PersonResponse.Address), "Address"},
                {nameof(PersonResponse.DateOfBirth), "DOB"},
                {nameof(PersonResponse.Gender), "Gender"},
                {nameof(PersonResponse.ReceiveNewsLetters), "News Letter"},
                {nameof(PersonResponse.CountryName), "CountryName"}
            };
            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            // Sort
            persons =await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(persons);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countryList = await _countriesService.GetAllCountries();

            ViewBag.CountryList = countryList.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryId.ToString()
            });


            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {

            if (!ModelState.IsValid)
            {
                List<CountryResponse> countryList =await _countriesService.GetAllCountries();
                ViewBag.CountryList = countryList;
                ViewBag.errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

            _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse personResponse = await _personsService.GetPersonByPersonId(personId);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countryList = await _countriesService.GetAllCountries();

            ViewBag.CountryList = countryList.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryId.ToString()
            });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse personResponse = await _personsService.GetPersonByPersonId(personUpdateRequest.PersonId);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {

                _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countryList = await _countriesService.GetAllCountries();

                ViewBag.CountryList = countryList.Select(temp => new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryId.ToString()
                });

                ViewBag.errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid? personId)
        {
            PersonResponse personResponse = await _personsService.GetPersonByPersonId(personId);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public IActionResult Delete(PersonUpdateRequest personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                return RedirectToAction("Index");
            }
            _personsService.DeletePerson(personUpdateRequest.PersonId);

            return RedirectToAction("Index");
        }

        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonPDF()
        {
            // Get list of person
            List<PersonResponse> personList = await _personsService.GetAllPersons();

            // Return view as pdf
            return new ViewAsPdf("PersonsPDF", personList, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins()
                {
                    Top = 20,
                    Bottom = 20,
                    Left = 20,
                    Right = 20
                },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
            };
        }


        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsService.GetPersonCSV();

            // For CSV files the contentType is application/octet-stream
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }
    }
}
