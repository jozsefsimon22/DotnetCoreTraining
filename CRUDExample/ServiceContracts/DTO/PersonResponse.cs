using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        /// <summary>
        /// Compares the current object data with the parameter object
        /// </summary>
        /// <param name="obj">The PersonResponse Object to compare</param>
        /// <returns>True or false, indicating whether all person details are matched wit the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse person = (PersonResponse)obj;

            return PersonId == person.PersonId && Name == person.Name && Email == person.Email &&
                   DateOfBirth == person.DateOfBirth && Gender == person.Gender && CountryId == person.CountryId &&
                   Address == person.Address && ReceiveNewsLetters == person.ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonId}, Person Name: {Name}, Email: {Email}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonId = this.PersonId,
                Name = this.Name,
                DateOfBirth = this.DateOfBirth,
                CountryId = this.CountryId,
                Address = this.Address,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                ReceiveNewsLetters = this.ReceiveNewsLetters,
                Email = this.Email,
            };
        }
    }



    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object of Person class into PersonResponse class
        /// </summary>
        /// <param name="person">Returns the converted PersonResponse object</param>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                Name = person.Name,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryId = person.CountryId,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null)
                    ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25)
                    : null,
                CountryName = person.Country?.CountryName
            };
        }
    }
}