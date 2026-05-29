using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameCatalogue.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCatalogueSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VideoGames",
                columns: new[] { "Id", "Title", "Genre", "Platform", "ReleaseYear", "Developer", "Publisher", "CriticScore", "Summary", "UpdatedUtc" },
                values: new object[,]
                {
                    {
                        1,
                        "The Legend of Zelda: Tears of the Kingdom",
                        "Action Adventure",
                        "Nintendo Switch",
                        2023,
                        "Nintendo EPD",
                        "Nintendo",
                        96,
                        "A systems-driven open-world adventure with deep traversal and crafting.",
                        new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        2,
                        "Baldur's Gate 3",
                        "RPG",
                        "PlayStation 5",
                        2023,
                        "Larian Studios",
                        "Larian Studios",
                        96,
                        "A party-based role-playing game with reactive questing and turn-based combat.",
                        new DateTime(2024, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        3,
                        "Hades",
                        "Roguelike",
                        "PC",
                        2020,
                        "Supergiant Games",
                        "Supergiant Games",
                        93,
                        "A fast-paced action roguelike built around repeatable runs and rich character writing.",
                        new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        4,
                        "Alan Wake 2",
                        "Survival Horror",
                        "PC",
                        2023,
                        "Remedy Entertainment",
                        "Epic Games Publishing",
                        89,
                        "A cinematic horror thriller that alternates between investigation and combat.",
                        new DateTime(2024, 1, 13, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        5,
                        "Forza Horizon 5",
                        "Racing",
                        "Xbox Series X|S",
                        2021,
                        "Playground Games",
                        "Xbox Game Studios",
                        92,
                        "An open-world driving game with a large map, live events, and accessible progression.",
                        new DateTime(2024, 1, 14, 0, 0, 0, DateTimeKind.Utc)
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VideoGames",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
