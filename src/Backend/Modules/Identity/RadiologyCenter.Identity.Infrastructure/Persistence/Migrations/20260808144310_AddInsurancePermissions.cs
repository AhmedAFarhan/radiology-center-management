using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInsurancePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Identity",
                table: "AspNetPermissions",
                columns: new[] { "Id", "Code", "Description", "Group", "Name" },
                values: new object[,]
                {
                    { new Guid("0d0decde-1413-1cd1-5b0e-a10491ee5cab"), "insurance.policies.delete", "Remove insurance policies", "Insurance", "Delete Insurance Policies" },
                    { new Guid("132122fc-1997-a6d7-3a26-73321a222012"), "insurance.companies.read", "View insurance companies", "Insurance", "Read Insurance Companies" },
                    { new Guid("1b0affa6-08cf-6c02-f07b-4e4e581b2acb"), "insurance.policies.read", "View patient insurance policies", "Insurance", "Read Insurance Policies" },
                    { new Guid("1cd6d7c7-54ef-0ef8-a571-2ce54c289db8"), "insurance.claims.read", "View claims and their lifecycle", "Insurance", "Read Claims" },
                    { new Guid("2fd73645-e204-fd4c-34fb-d406faf0b826"), "insurance.claims.create", "Create claims for covered examinations", "Insurance", "Create Claims" },
                    { new Guid("467e6cd7-0231-2549-cdb2-81fe748d1181"), "insurance.policies.update", "Modify policies, coverage and status", "Insurance", "Update Insurance Policies" },
                    { new Guid("5441cf09-ce40-6b0f-5e79-bd3b26e88e33"), "insurance.preauthorizations.read", "View pre-authorizations and documents", "Insurance", "Read Pre-authorizations" },
                    { new Guid("5a6d94df-7ddf-4379-8bb2-6484890a47bc"), "insurance.preauthorizations.update", "Approve, deny or attach documents", "Insurance", "Update Pre-authorizations" },
                    { new Guid("5dcfa2a7-f105-26de-b69d-1f77f4bd146c"), "insurance.companies.delete", "Remove insurance companies", "Insurance", "Delete Insurance Companies" },
                    { new Guid("7246f9e5-5290-a93d-94fd-de8ea1155658"), "insurance.companies.update", "Modify insurance company details", "Insurance", "Update Insurance Companies" },
                    { new Guid("74d81d37-dad6-4cc5-d480-e208dbcd9419"), "insurance.claims.settle", "Record insurance settlements", "Insurance", "Settle Claims" },
                    { new Guid("87502c83-12b4-29a2-cbed-1ee184a9ff3c"), "insurance.preauthorizations.create", "Request pre-authorization for examinations", "Insurance", "Create Pre-authorizations" },
                    { new Guid("8f39be78-ecee-6ac8-8720-a47d11f2f6c9"), "insurance.preauthorizations.attach-document", "Upload or remove official approval documents", "Insurance", "Attach Pre-authorization Documents" },
                    { new Guid("d6f95542-491f-0bf6-8ad1-61e153ada390"), "insurance.companies.create", "Add new insurance companies", "Insurance", "Create Insurance Companies" },
                    { new Guid("d8a7d18a-9676-f315-26d9-4e4d30c408d6"), "insurance.claims.update", "Submit, resubmit and adjudicate claims", "Insurance", "Update Claims" },
                    { new Guid("ff6e49ff-a8bd-f63b-10b9-f3c6b96d55d2"), "insurance.policies.create", "Add new patient insurance policies", "Insurance", "Create Insurance Policies" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("0d0decde-1413-1cd1-5b0e-a10491ee5cab"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("132122fc-1997-a6d7-3a26-73321a222012"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("1b0affa6-08cf-6c02-f07b-4e4e581b2acb"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("1cd6d7c7-54ef-0ef8-a571-2ce54c289db8"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("2fd73645-e204-fd4c-34fb-d406faf0b826"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("467e6cd7-0231-2549-cdb2-81fe748d1181"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("5441cf09-ce40-6b0f-5e79-bd3b26e88e33"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("5a6d94df-7ddf-4379-8bb2-6484890a47bc"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("5dcfa2a7-f105-26de-b69d-1f77f4bd146c"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("7246f9e5-5290-a93d-94fd-de8ea1155658"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("74d81d37-dad6-4cc5-d480-e208dbcd9419"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("87502c83-12b4-29a2-cbed-1ee184a9ff3c"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("8f39be78-ecee-6ac8-8720-a47d11f2f6c9"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("d6f95542-491f-0bf6-8ad1-61e153ada390"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("d8a7d18a-9676-f315-26d9-4e4d30c408d6"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("ff6e49ff-a8bd-f63b-10b9-f3c6b96d55d2"));
        }
    }
}
