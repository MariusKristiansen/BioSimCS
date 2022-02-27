using Microsoft.EntityFrameworkCore.Migrations;

namespace Biosim.Migrations
{
    public partial class database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carnivores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarnivoreID = table.Column<int>(type: "int", nullable: true),
                    HerbivoreID = table.Column<int>(type: "int", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    NumberOfBirths = table.Column<int>(type: "int", nullable: false),
                    Fitness = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carnivores", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Herbivores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HerbivoreID = table.Column<int>(type: "int", nullable: true),
                    CarnivoreID = table.Column<int>(type: "int", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    NumberOfBirths = table.Column<int>(type: "int", nullable: false),
                    Fitness = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herbivores", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Herbivores = table.Column<int>(type: "int", nullable: false),
                    Carnivores = table.Column<int>(type: "int", nullable: false),
                    HerbivoreAvgFitness = table.Column<double>(type: "float", nullable: false),
                    CarnivoreAvgFitness = table.Column<double>(type: "float", nullable: false),
                    HerbivoreAvgAge = table.Column<double>(type: "float", nullable: false),
                    CarnivoreAvgAge = table.Column<double>(type: "float", nullable: false),
                    HerbivoreAvgWeight = table.Column<double>(type: "float", nullable: false),
                    CarnivoreAvgWeight = table.Column<double>(type: "float", nullable: false),
                    HerbivoreBirths = table.Column<int>(type: "int", nullable: false),
                    CarnivoreBirths = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Carnivores");

            migrationBuilder.DropTable(
                name: "Herbivores");

            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
