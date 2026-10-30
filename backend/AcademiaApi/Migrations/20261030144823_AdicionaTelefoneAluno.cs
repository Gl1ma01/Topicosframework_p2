using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaApi.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaTelefoneAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Alunos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Alunos");
        }
    }
}
