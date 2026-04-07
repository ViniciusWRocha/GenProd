using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.SIGE.Migrations
{
    /// <inheritdoc />
    public partial class FixSenhaColumnSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Usuario\" ALTER COLUMN \"Senha\" TYPE character varying(200);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
