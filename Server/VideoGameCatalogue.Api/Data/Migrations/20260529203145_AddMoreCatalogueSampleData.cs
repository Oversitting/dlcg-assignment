using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameCatalogue.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreCatalogueSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VideoGames",
                columns: new[] { "Id", "Title", "Genre", "Platform", "ReleaseYear", "Developer", "Publisher", "CriticScore", "Summary", "UpdatedUtc" },
                values: new object[,]
                {
                    { 2001, "Celeste", "Platformer", "Nintendo Switch", 2018, "Extremely OK Games", "Matt Makes Games", 92, "A precision platformer about climbing a mountain and pushing through setbacks.", new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
                    { 2002, "Hollow Knight", "Metroidvania", "PC", 2017, "Team Cherry", "Team Cherry", 90, "A hand-drawn action adventure set in the ruined underground kingdom of Hallownest.", new DateTime(2024, 1, 16, 0, 0, 0, DateTimeKind.Utc) },
                    { 2003, "God of War Ragnarok", "Action Adventure", "PlayStation 5", 2022, "Santa Monica Studio", "Sony Interactive Entertainment", 94, "A mythic action game that continues Kratos and Atreus' journey through the Nine Realms.", new DateTime(2024, 1, 17, 0, 0, 0, DateTimeKind.Utc) },
                    { 2004, "Metroid Dread", "Action Adventure", "Nintendo Switch", 2021, "MercurySteam", "Nintendo", 88, "A tense side-scrolling return for Samus built around stealth, movement, and boss encounters.", new DateTime(2024, 1, 18, 0, 0, 0, DateTimeKind.Utc) },
                    { 2005, "Stardew Valley", "Simulation", "PC", 2016, "ConcernedApe", "ConcernedApe", 89, "A farming and life simulation built around routines, relationships, and long-term progression.", new DateTime(2024, 1, 19, 0, 0, 0, DateTimeKind.Utc) },
                    { 2006, "Resident Evil 4", "Survival Horror", "PlayStation 5", 2023, "Capcom", "Capcom", 93, "A modern remake that sharpens the pacing, combat, and horror of a classic campaign.", new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
                    { 2007, "Persona 5 Royal", "RPG", "Xbox Series X|S", 2022, "P-Studio", "Atlus", 95, "A stylish turn-based RPG about balancing school life, dungeon runs, and party bonds.", new DateTime(2024, 1, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { 2008, "Tetris Effect: Connected", "Puzzle", "PC", 2021, "Monstars Inc.", "Enhance", 89, "A sensory-driven take on Tetris with striking audiovisual presentation and multiplayer modes.", new DateTime(2024, 1, 22, 0, 0, 0, DateTimeKind.Utc) },
                    { 2009, "Hi-Fi Rush", "Action", "Xbox Series X|S", 2023, "Tango Gameworks", "Bethesda Softworks", 87, "A rhythm-synced character action game with comic-book presentation and expressive combat.", new DateTime(2024, 1, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { 2010, "Returnal", "Roguelike", "PC", 2023, "Housemarque", "Sony Interactive Entertainment", 86, "A fast third-person roguelike shooter built around mobility, looping runs, and atmosphere.", new DateTime(2024, 1, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { 2011, "Slay the Spire", "Deckbuilder", "PC", 2019, "Mega Crit", "Mega Crit", 89, "A run-based card battler that combines drafting, combat, and route planning.", new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc) },
                    { 2012, "Control", "Action Adventure", "PC", 2019, "Remedy Entertainment", "505 Games", 85, "A paranormal action game set inside a shifting brutalist headquarters full of strange objects.", new DateTime(2024, 1, 26, 0, 0, 0, DateTimeKind.Utc) },
                    { 2013, "Street Fighter 6", "Fighting", "PlayStation 5", 2023, "Capcom", "Capcom", 92, "A deep and accessible fighting game with strong onboarding and a robust competitive system.", new DateTime(2024, 1, 27, 0, 0, 0, DateTimeKind.Utc) },
                    { 2014, "Elden Ring", "Action RPG", "PC", 2022, "FromSoftware", "Bandai Namco Entertainment", 96, "A large-scale action RPG that merges open-world exploration with punishing combat.", new DateTime(2024, 1, 28, 0, 0, 0, DateTimeKind.Utc) },
                    { 2015, "Neon White", "Shooter", "Nintendo Switch", 2022, "Angel Matrix", "Annapurna Interactive", 90, "A speedrunning-focused first-person action game built around card-based movement and weapons.", new DateTime(2024, 1, 29, 0, 0, 0, DateTimeKind.Utc) },
                    { 2016, "Mario Kart 8 Deluxe", "Racing", "Nintendo Switch", 2017, "Nintendo EPD", "Nintendo", 92, "An arcade racer with polished track design, approachable controls, and reliable multiplayer.", new DateTime(2024, 1, 30, 0, 0, 0, DateTimeKind.Utc) },
                    { 2017, "Cocoon", "Puzzle Adventure", "Xbox Series X|S", 2023, "Geometric Interactive", "Annapurna Interactive", 89, "A compact puzzle adventure that layers world-within-world mechanics and environmental discovery.", new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc) },
                    { 2018, "Final Fantasy VII Rebirth", "RPG", "PlayStation 5", 2024, "Square Enix", "Square Enix", 92, "A large-scale party RPG that expands the world, combat systems, and side content of the remake project.", new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { 2019, "Tunic", "Action Adventure", "PC", 2022, "Isometricorp Games", "Finji", 85, "An isometric adventure that hides its systems inside exploration, combat, and in-world manuals.", new DateTime(2024, 2, 2, 0, 0, 0, DateTimeKind.Utc) },
                    { 2020, "Prince of Persia: The Lost Crown", "Metroidvania", "PC", 2024, "Ubisoft Montpellier", "Ubisoft", 87, "A fast and fluid action platformer centered on time powers and carefully tuned traversal.", new DateTime(2024, 2, 3, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VideoGames",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    2001, 2002, 2003, 2004, 2005,
                    2006, 2007, 2008, 2009, 2010,
                    2011, 2012, 2013, 2014, 2015,
                    2016, 2017, 2018, 2019, 2020
                });
        }
    }
}
