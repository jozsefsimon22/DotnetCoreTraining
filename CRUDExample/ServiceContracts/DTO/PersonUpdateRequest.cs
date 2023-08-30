using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{

    public class PersonUpdateRequest
    {
        /// <summary>
        /// Act as a DTO for updating a user
        /// </summary>
        [Required(ErrorMessage = "Person Id can't be blank")]
        public Guid PersonId { get; set; }

        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email value should be valid")]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converts the current object of PersonUpdateRequest into a new object of Person type
        /// </summary>
        /// <returns>Person object</returns>
        public Person ToPerson()
        {
            return new Person()
            {
                PersonId = this.PersonId,
                Name = this.Name,
                DateOfBirth = this.DateOfBirth,
                CountryId = this.CountryId,
                Address = this.Address,
                Gender = this.Gender.ToString(),
                ReceiveNewsLetters = this.ReceiveNewsLetters,
                Email = this.Email,
            };
        }
    }
}
