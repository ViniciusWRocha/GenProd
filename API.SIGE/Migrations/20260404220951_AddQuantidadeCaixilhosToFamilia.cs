using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.SIGE.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantidadeCaixilhosToFamilia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantidadeCaixilhos",
                table: "FamiliaCaixilho",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantidadeCaixilhos",
                table: "FamiliaCaixilho");
        }
    }
}
