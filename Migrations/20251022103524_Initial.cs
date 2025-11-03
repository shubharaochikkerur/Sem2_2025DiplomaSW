using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentInfoLoginRoles.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseDetails",
                columns: table => new
                {
                    CourseCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    DurationInWeeks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDetails", x => x.CourseCode);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeePending = table.Column<decimal>(type: "decimal(8,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_Students_CourseDetails_CourseCode",
                        column: x => x.CourseCode,
                        principalTable: "CourseDetails",
                        principalColumn: "CourseCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeeTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "decimal(8,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_FeeTransactions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeeTransactions_StudentId",
                table: "FeeTransactions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CourseCode",
                table: "Students",
                column: "CourseCode");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_UpdateFeePending
                ON FeeTransactions
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE s
                    SET s.FeePending = s.FeePending - i.TransactionAmount
                    FROM Students s
                    INNER JOIN inserted i ON s.StudentId = i.StudentId;
                END
                ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_SetInitialFeePending
                ON Students
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE s
                    SET s.FeePending = cd.Price
                    FROM Students s
                    INNER JOIN inserted i ON s.StudentId = i.StudentId
                    INNER JOIN CourseDetails cd ON i.CourseCode = cd.CourseCode;
                END
                ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_PreventOverpayment
                ON FeeTransactions
                INSTEAD OF INSERT
                AS

               BEGIN
                    SET NOCOUNT ON;
                    IF EXISTS (
                        SELECT 1
                        FROM inserted i
                        JOIN Students s ON i.StudentId = s.StudentId
                        WHERE i.TransactionAmount > s.FeePending
                    )
                    BEGIN
                        RAISERROR('Transaction amount exceeds pending fee.', 16, 1);
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                    INSERT INTO FeeTransactions (StudentId, TransactionDate, Mode, TransactionAmount)
                    SELECT StudentId, TransactionDate, Mode, TransactionAmount
                    FROM inserted;
                END
                ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_UpdateFeePendingOnUpdate
                ON FeeTransactions
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE s
                    SET s.FeePending = s.FeePending - (i.TransactionAmount - d.TransactionAmount)
                    FROM Students s
                    INNER JOIN deleted d ON s.StudentId = d.StudentId
                    INNER JOIN inserted i ON s.StudentId = i.StudentId;
                END
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeeTransactions");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "CourseDetails");
        }
    }
}
