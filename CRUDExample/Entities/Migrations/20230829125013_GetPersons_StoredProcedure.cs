using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetPersons_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetlAllPersons = @"
                CREATE PROCEDURE [dbo].[GetAllPersons]
                AS BEGIN
                    SELECT PersonId, Name, Email, DateOfBirth, Gender,
                    CountryId, Address, ReceiveNewsLetters FROM [dbo].[Persons] END";
            migrationBuilder.Sql(sp_GetlAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetlAllPersons = @"
                DROP PROCEDURE [dbo].[GetAllPersons]";
            migrationBuilder.Sql(sp_GetlAllPersons);
        }
    }
}