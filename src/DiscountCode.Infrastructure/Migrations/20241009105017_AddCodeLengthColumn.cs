using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountCode.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeLengthColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE ""AvailableDiscountCodes""
            ADD COLUMN ""CodeLength"" INT GENERATED ALWAYS AS (LENGTH(""Code"")) STORED;
        ");

            migrationBuilder.Sql(@"
            CREATE INDEX idx_code_length ON ""AvailableDiscountCodes"" (""CodeLength"");
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX idx_code_length;");
            migrationBuilder.Sql(@"ALTER TABLE ""AvailableDiscountCodes"" DROP COLUMN ""CodeLength"";");
        }
    }
}
