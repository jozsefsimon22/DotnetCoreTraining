using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public PersonsDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries"); // Here instead of countries we can use any name we want
            modelBuilder.Entity<Person>().ToTable("Persons");

            // Seed to Countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countryList = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (var country in countryList)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            // Seed to Countries
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> personList = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (var person in personList)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            //Fluent API
            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            modelBuilder.Entity<Person>()
                .HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");
        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@PersonId", person.PersonId),
                new SqlParameter("@Name", person.Name),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryId", person.CountryId),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonId, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters", parameters);
        }

    }
}
