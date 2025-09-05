using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pepper.Cards.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeckStyles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsFourColour = table.Column<bool>(type: "bit", nullable: false),
                    HasFourCornerLegend = table.Column<bool>(type: "bit", nullable: false),
                    DarkMode = table.Column<bool>(type: "bit", nullable: false),
                    LargePrint = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackDesignKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckStyles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TablePositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatNumber = table.Column<int>(type: "int", nullable: true),
                    NormallySecret = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TablePositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    TagUid = table.Column<byte[]>(type: "varbinary(900)", nullable: false),
                    Suit = table.Column<int>(type: "int", nullable: false),
                    DeckStyleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_DeckStyles_DeckStyleId",
                        column: x => x.DeckStyleId,
                        principalTable: "DeckStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_GameTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "GameTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TablePositionReaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReaderId = table.Column<int>(type: "int", nullable: false),
                    AntennaId = table.Column<int>(type: "int", nullable: false),
                    TablePositionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TablePositionReaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TablePositionReaders_TablePositions_TablePositionId",
                        column: x => x.TablePositionId,
                        principalTable: "TablePositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hands_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HandId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FromPositionId = table.Column<int>(type: "int", nullable: true),
                    ToPositionId = table.Column<int>(type: "int", nullable: false),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandSteps_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandSteps_Hands_HandId",
                        column: x => x.HandId,
                        principalTable: "Hands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandSteps_TablePositions_FromPositionId",
                        column: x => x.FromPositionId,
                        principalTable: "TablePositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HandSteps_TablePositions_ToPositionId",
                        column: x => x.ToPositionId,
                        principalTable: "TablePositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DeckStyleId",
                table: "Cards",
                column: "DeckStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_TagUid",
                table: "Cards",
                column: "TagUid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_TypeId",
                table: "Games",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTypes_Name",
                table: "GameTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hands_GameId",
                table: "Hands",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_HandSteps_CardId",
                table: "HandSteps",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_HandSteps_FromPositionId",
                table: "HandSteps",
                column: "FromPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_HandSteps_HandId",
                table: "HandSteps",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_HandSteps_ToPositionId",
                table: "HandSteps",
                column: "ToPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_TablePositionReaders_TablePositionId",
                table: "TablePositionReaders",
                column: "TablePositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HandSteps");

            migrationBuilder.DropTable(
                name: "TablePositionReaders");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Hands");

            migrationBuilder.DropTable(
                name: "TablePositions");

            migrationBuilder.DropTable(
                name: "DeckStyles");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "GameTypes");
        }
    }
}
