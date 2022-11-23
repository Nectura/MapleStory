using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountRestrictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AccountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    IssuedByAccountId = table.Column<uint>(type: "int unsigned", nullable: true),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRestrictions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    PasswordSaltHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    PinHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    PinSaltHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    PicHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    PicSaltHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    HasAcceptedEula = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CharacterSlots = table.Column<uint>(type: "int unsigned", nullable: false),
                    LastWorldId = table.Column<int>(type: "int", nullable: true),
                    LastKnownIpAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastLoggedInAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RestrictionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountRestrictions_RestrictionId",
                        column: x => x.RestrictionId,
                        principalTable: "AccountRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    WorldId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Level = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Experience = table.Column<uint>(type: "int unsigned", nullable: false),
                    Job = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    SubJob = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Fame = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    GachaponExperience = table.Column<uint>(type: "int unsigned", nullable: false),
                    Strength = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Dexterity = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Luck = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Intelligence = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    MaxHitPoints = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    MaxManaPoints = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    HitPoints = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    ManaPoints = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Mesos = table.Column<uint>(type: "int unsigned", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    HairStyle = table.Column<uint>(type: "int unsigned", nullable: false),
                    HairColor = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    SkinColor = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Face = table.Column<uint>(type: "int unsigned", nullable: false),
                    AbilityPoints = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    SkillPoints = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    MapId = table.Column<uint>(type: "int unsigned", nullable: false),
                    SpawnPoint = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    BuddyLimit = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    EquipmentSlots = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    UsableSlots = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    SetupSlots = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    EtceteraSlots = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CashSlots = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    X = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Y = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Stance = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Foothold = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    ExperienceLocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LevelLocked = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RestrictionId",
                table: "Accounts",
                column: "RestrictionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AccountId",
                table: "Characters",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "AccountRestrictions");
        }
    }
}
