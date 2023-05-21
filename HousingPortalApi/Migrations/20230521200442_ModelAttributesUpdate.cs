using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HousingPortalApi.Migrations
{
    /// <inheritdoc />
    public partial class ModelAttributesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Students_studentId",
                table: "Listings");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Students",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Students",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "major",
                table: "Students",
                newName: "Major");

            migrationBuilder.RenameColumn(
                name: "graduationYear",
                table: "Students",
                newName: "GraduationYear");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Students",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "studentId",
                table: "Students",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "zip",
                table: "Listings",
                newName: "Zip");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Listings",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "studentId",
                table: "Listings",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "Listings",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Listings",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "image",
                table: "Listings",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Listings",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Listings",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Listings",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "listingId",
                table: "Listings",
                newName: "ListingId");

            migrationBuilder.RenameIndex(
                name: "IX_Listings_studentId",
                table: "Listings",
                newName: "IX_Listings_StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Students_StudentId",
                table: "Listings",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Students_StudentId",
                table: "Listings");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Students",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Students",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Major",
                table: "Students",
                newName: "major");

            migrationBuilder.RenameColumn(
                name: "GraduationYear",
                table: "Students",
                newName: "graduationYear");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Students",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Students",
                newName: "studentId");

            migrationBuilder.RenameColumn(
                name: "Zip",
                table: "Listings",
                newName: "zip");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Listings",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Listings",
                newName: "studentId");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Listings",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Listings",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Listings",
                newName: "image");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Listings",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Listings",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Listings",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "ListingId",
                table: "Listings",
                newName: "listingId");

            migrationBuilder.RenameIndex(
                name: "IX_Listings_StudentId",
                table: "Listings",
                newName: "IX_Listings_studentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Students_studentId",
                table: "Listings",
                column: "studentId",
                principalTable: "Students",
                principalColumn: "studentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
