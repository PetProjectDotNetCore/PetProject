using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PetProject.Domain.Entities;
using PetProject.Application.Login;

namespace PetProject.Web.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    PasswordHash = table.Column<byte[]>(nullable: false),
                    PasswordSalt = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            var (passwordSaltForIvanov, passwordHashForIvanov) = PasswordHelper.CreatePasswordHash("qwerty");
            var (passwordSaltForPetrov, passwordHashForPetrov) = PasswordHelper.CreatePasswordHash("qwerty");
            var (passwordSaltForSidorov, passwordHashForSidorov) = PasswordHelper.CreatePasswordHash("qwerty");

            var users = new List<User>
            {
                new User
                {
                    FirstName = "Ivan",
                    LastName = "Ivanov",
                    Email = "IvanovIvan@gmail.com",
                    PasswordHash = passwordHashForIvanov,
                    PasswordSalt = passwordSaltForIvanov
                },
                new User
                {
                    FirstName = "Petr",
                    LastName = "Petrov",
                    Email = "PetrovPetr@gmail.com",
                    PasswordHash = passwordHashForPetrov,
                    PasswordSalt = passwordSaltForPetrov
                },
                new User
                {
                    FirstName = "Sidor",
                    LastName = "Sidorov",
                    Email = "SidorovSidor@gmail.com",
                    PasswordHash = passwordHashForSidorov,
                    PasswordSalt = passwordSaltForSidorov
                }
            };

            foreach (var user in users)
            {
                migrationBuilder.InsertData(
                    table: "Users",
                    columns: new[] { "FirstName", "LastName", "Email", "PasswordHash", "PasswordSalt" },
                    values: new object[] { user.FirstName, user.LastName, user.Email, user.PasswordHash, user.PasswordSalt });
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
