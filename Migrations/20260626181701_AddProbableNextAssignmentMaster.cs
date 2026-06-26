using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProbableNextAssignmentMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProbableNextAssignmentId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProbableNextAssignmentMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProbableNextAssignmentMasters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProbableNextAssignmentMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("c0000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Alcan Integration - Utilized, Pre sales - investment" },
                    { new Guid("c0000000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, false, null, null, "Already billable in ING" },
                    { new Guid("c0000000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, false, null, null, "Amdocs/Omnia Data" },
                    { new Guid("c0000000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, false, null, null, "AutomatePro" },
                    { new Guid("c0000000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true, false, null, null, "Axiad" },
                    { new Guid("c0000000-0000-0000-0000-000000000006"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, true, false, null, null, "Bench" },
                    { new Guid("c0000000-0000-0000-0000-000000000007"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true, false, null, null, "Biolabs" },
                    { new Guid("c0000000-0000-0000-0000-000000000008"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, true, false, null, null, "BioLabs & Flightcheck" },
                    { new Guid("c0000000-0000-0000-0000-000000000009"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, true, false, null, null, "BIS" },
                    { new Guid("c0000000-0000-0000-0000-000000000010"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, true, false, null, null, "BIS: Client Portal and Data Lake Managed Platform Services" },
                    { new Guid("c0000000-0000-0000-0000-000000000011"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, true, false, null, null, "BresaTech" },
                    { new Guid("c0000000-0000-0000-0000-000000000012"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, true, false, null, null, "CBM Datalake Ph2" },
                    { new Guid("c0000000-0000-0000-0000-000000000013"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, true, false, null, null, "Cloudops MSP, RFC2.0" },
                    { new Guid("c0000000-0000-0000-0000-000000000014"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, true, false, null, null, "Cognida Tyler" },
                    { new Guid("c0000000-0000-0000-0000-000000000015"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, true, false, null, null, "CommScope M365" },
                    { new Guid("c0000000-0000-0000-0000-000000000016"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 16, true, false, null, null, "Core Team" },
                    { new Guid("c0000000-0000-0000-0000-000000000017"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, true, false, null, null, "Data Lens" },
                    { new Guid("c0000000-0000-0000-0000-000000000018"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, true, false, null, null, "Datalens" },
                    { new Guid("c0000000-0000-0000-0000-000000000019"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, true, false, null, null, "Deloitte" },
                    { new Guid("c0000000-0000-0000-0000-000000000020"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, true, false, null, null, "Deloitte - 1$" },
                    { new Guid("c0000000-0000-0000-0000-000000000021"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 21, true, false, null, null, "Deloitte - Cars" },
                    { new Guid("c0000000-0000-0000-0000-000000000022"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, true, false, null, null, "Deloitte - Cortex" },
                    { new Guid("c0000000-0000-0000-0000-000000000023"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23, true, false, null, null, "Deloitte - Dart" },
                    { new Guid("c0000000-0000-0000-0000-000000000024"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24, true, false, null, null, "Deloitte - Levvia" },
                    { new Guid("c0000000-0000-0000-0000-000000000025"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25, true, false, null, null, "Deloitte - Project Work for Pillar 2" },
                    { new Guid("c0000000-0000-0000-0000-000000000026"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26, true, false, null, null, "Deloitte - TieOut Global" },
                    { new Guid("c0000000-0000-0000-0000-000000000027"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 27, true, false, null, null, "Deloitte Levvia" },
                    { new Guid("c0000000-0000-0000-0000-000000000028"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 28, true, false, null, null, "Deloitte- Levvia" },
                    { new Guid("c0000000-0000-0000-0000-000000000029"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 29, true, false, null, null, "Deloitte- Levvia Automation" },
                    { new Guid("c0000000-0000-0000-0000-000000000030"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, true, false, null, null, "Deloitte Myinsight" },
                    { new Guid("c0000000-0000-0000-0000-000000000031"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 31, true, false, null, null, "Deloitte- RADC / Levvia" },
                    { new Guid("c0000000-0000-0000-0000-000000000032"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 32, true, false, null, null, "Deloitte-Learning POD" },
                    { new Guid("c0000000-0000-0000-0000-000000000033"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33, true, false, null, null, "Deloitte-RPA" },
                    { new Guid("c0000000-0000-0000-0000-000000000034"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 34, true, false, null, null, "Dodge and Cox" },
                    { new Guid("c0000000-0000-0000-0000-000000000035"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 35, true, false, null, null, "Dodge&Cox" },
                    { new Guid("c0000000-0000-0000-0000-000000000036"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 36, true, false, null, null, "Doxa" },
                    { new Guid("c0000000-0000-0000-0000-000000000037"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 37, true, false, null, null, "DWP" },
                    { new Guid("c0000000-0000-0000-0000-000000000038"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 38, true, false, null, null, "Echo360" },
                    { new Guid("c0000000-0000-0000-0000-000000000039"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 39, true, false, null, null, "ElevateBio" },
                    { new Guid("c0000000-0000-0000-0000-000000000040"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 40, true, false, null, null, "Exit" },
                    { new Guid("c0000000-0000-0000-0000-000000000041"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 41, true, false, null, null, "Exit/AutomatePro" },
                    { new Guid("c0000000-0000-0000-0000-000000000042"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 42, true, false, null, null, "Finbraine" },
                    { new Guid("c0000000-0000-0000-0000-000000000043"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 43, true, false, null, null, "GCS Advisory" },
                    { new Guid("c0000000-0000-0000-0000-000000000044"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 44, true, false, null, null, "GDAS Services" },
                    { new Guid("c0000000-0000-0000-0000-000000000045"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 45, true, false, null, null, "Global TieOut" },
                    { new Guid("c0000000-0000-0000-0000-000000000046"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 46, true, false, null, null, "Global TieOut/Exit" },
                    { new Guid("c0000000-0000-0000-0000-000000000047"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 47, true, false, null, null, "Global TieOut/Learning Coe" },
                    { new Guid("c0000000-0000-0000-0000-000000000048"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 48, true, false, null, null, "GTS" },
                    { new Guid("c0000000-0000-0000-0000-000000000049"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 49, true, false, null, null, "GTS - Impulse" },
                    { new Guid("c0000000-0000-0000-0000-000000000050"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 50, true, false, null, null, "GTS Impulse" },
                    { new Guid("c0000000-0000-0000-0000-000000000051"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 51, true, false, null, null, "GTS-Impulse" },
                    { new Guid("c0000000-0000-0000-0000-000000000052"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 52, true, false, null, null, "ING" },
                    { new Guid("c0000000-0000-0000-0000-000000000053"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 53, true, false, null, null, "Internal IT" },
                    { new Guid("c0000000-0000-0000-0000-000000000054"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 54, true, false, null, null, "KM" },
                    { new Guid("c0000000-0000-0000-0000-000000000055"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 55, true, false, null, null, "Learning Coe" },
                    { new Guid("c0000000-0000-0000-0000-000000000056"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 56, true, false, null, null, "Levvia" },
                    { new Guid("c0000000-0000-0000-0000-000000000057"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 57, true, false, null, null, "Levvia Automation" },
                    { new Guid("c0000000-0000-0000-0000-000000000058"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 58, true, false, null, null, "Levvia Bootcamp" },
                    { new Guid("c0000000-0000-0000-0000-000000000059"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 59, true, false, null, null, "Levvia Production Support" },
                    { new Guid("c0000000-0000-0000-0000-000000000060"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 60, true, false, null, null, "Levvia Rhino" },
                    { new Guid("c0000000-0000-0000-0000-000000000061"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 61, true, false, null, null, "Levvia Transformation" },
                    { new Guid("c0000000-0000-0000-0000-000000000062"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 62, true, false, null, null, "Ludi" },
                    { new Guid("c0000000-0000-0000-0000-000000000063"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 63, true, false, null, null, "Ludi Offshore" },
                    { new Guid("c0000000-0000-0000-0000-000000000064"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 64, true, false, null, null, "Management" },
                    { new Guid("c0000000-0000-0000-0000-000000000065"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 65, true, false, null, null, "Milliman" },
                    { new Guid("c0000000-0000-0000-0000-000000000066"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 66, true, false, null, null, "Miyahuna" },
                    { new Guid("c0000000-0000-0000-0000-000000000067"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 67, true, false, null, null, "ML return" },
                    { new Guid("c0000000-0000-0000-0000-000000000068"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 68, true, false, null, null, "ML start" },
                    { new Guid("c0000000-0000-0000-0000-000000000069"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 69, true, false, null, null, "NA" },
                    { new Guid("c0000000-0000-0000-0000-000000000070"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 70, true, false, null, null, "Nitish Shivankar - rep" },
                    { new Guid("c0000000-0000-0000-0000-000000000071"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 71, true, false, null, null, "Omnia data" },
                    { new Guid("c0000000-0000-0000-0000-000000000072"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 72, true, false, null, null, "Omnia Data (Cortex) - Manual - Forecasted" },
                    { new Guid("c0000000-0000-0000-0000-000000000073"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 73, true, false, null, null, "Omnia Data Cortex Automation" },
                    { new Guid("c0000000-0000-0000-0000-000000000074"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 74, true, false, null, null, "Omnia Serengeti Additional POD" },
                    { new Guid("c0000000-0000-0000-0000-000000000075"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 75, true, false, null, null, "Omnia Serengiti" },
                    { new Guid("c0000000-0000-0000-0000-000000000076"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 76, true, false, null, null, "PIP" },
                    { new Guid("c0000000-0000-0000-0000-000000000077"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 77, true, false, null, null, "PIP/Exit" },
                    { new Guid("c0000000-0000-0000-0000-000000000078"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 78, true, false, null, null, "RFC 2 Maintainance" },
                    { new Guid("c0000000-0000-0000-0000-000000000079"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 79, true, false, null, null, "RFC 2.0 phase 2" },
                    { new Guid("c0000000-0000-0000-0000-000000000080"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 80, true, false, null, null, "RKON" },
                    { new Guid("c0000000-0000-0000-0000-000000000081"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 81, true, false, null, null, "RKON -Strada" },
                    { new Guid("c0000000-0000-0000-0000-000000000082"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 82, true, false, null, null, "RSM" },
                    { new Guid("c0000000-0000-0000-0000-000000000083"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 83, true, false, null, null, "RTUK - Sportsmedia" },
                    { new Guid("c0000000-0000-0000-0000-000000000084"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 84, true, false, null, null, "RTUK RTHub" },
                    { new Guid("c0000000-0000-0000-0000-000000000085"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 85, true, false, null, null, "Saras Analytics/SeaSalt" },
                    { new Guid("c0000000-0000-0000-0000-000000000086"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 86, true, false, null, null, "SeaSalt - Buffer" },
                    { new Guid("c0000000-0000-0000-0000-000000000087"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 87, true, false, null, null, "SeaSalt - DWH Dev" },
                    { new Guid("c0000000-0000-0000-0000-000000000088"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 88, true, false, null, null, "Serengeti" },
                    { new Guid("c0000000-0000-0000-0000-000000000089"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 89, true, false, null, null, "SFC" },
                    { new Guid("c0000000-0000-0000-0000-000000000090"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 90, true, false, null, null, "SKPT - Wireless Upgrade" },
                    { new Guid("c0000000-0000-0000-0000-000000000091"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 91, true, false, null, null, "SKPT Databrick phase 2" },
                    { new Guid("c0000000-0000-0000-0000-000000000092"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 92, true, false, null, null, "SNOW ServiceNow Integrations" },
                    { new Guid("c0000000-0000-0000-0000-000000000093"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 93, true, false, null, null, "SNOW ServiceNow Integrations/Exit" },
                    { new Guid("c0000000-0000-0000-0000-000000000094"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 94, true, false, null, null, "SoftServ - Atlassian" },
                    { new Guid("c0000000-0000-0000-0000-000000000095"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 95, true, false, null, null, "SoftServ - BNY" },
                    { new Guid("c0000000-0000-0000-0000-000000000096"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 96, true, false, null, null, "SoftServ - Cisco" },
                    { new Guid("c0000000-0000-0000-0000-000000000097"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 97, true, false, null, null, "SoftServ- Atlassian" },
                    { new Guid("c0000000-0000-0000-0000-000000000098"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 98, true, false, null, null, "SoftServe - Expedia" },
                    { new Guid("c0000000-0000-0000-0000-000000000099"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 99, true, false, null, null, "SoftServe - NasDaq" },
                    { new Guid("c0000000-0000-0000-0000-000000000100"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 100, true, false, null, null, "USP" },
                    { new Guid("c0000000-0000-0000-0000-000000000101"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 101, true, false, null, null, "Votal" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_ProbableNextAssignmentId",
                table: "ResourceAllocations",
                column: "ProbableNextAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProbableNextAssignmentMasters_Name",
                table: "ProbableNextAssignmentMasters",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_ProbableNextAssignmentMasters_ProbableNextAssignmentId",
                table: "ResourceAllocations",
                column: "ProbableNextAssignmentId",
                principalTable: "ProbableNextAssignmentMasters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_ProbableNextAssignmentMasters_ProbableNextAssignmentId",
                table: "ResourceAllocations");

            migrationBuilder.DropTable(
                name: "ProbableNextAssignmentMasters");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_ProbableNextAssignmentId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "ProbableNextAssignmentId",
                table: "ResourceAllocations");
        }
    }
}
