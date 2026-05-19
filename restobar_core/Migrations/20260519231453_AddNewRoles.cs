using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace restobar_core.Migrations
{
    /// <inheritdoc />
    public partial class AddNewRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE users SET \"Role\" = 'administrador' WHERE \"Role\" = 'admin'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE users SET \"Role\" = 'admin' WHERE \"Role\" = 'administrador'");
        }
    }
}
