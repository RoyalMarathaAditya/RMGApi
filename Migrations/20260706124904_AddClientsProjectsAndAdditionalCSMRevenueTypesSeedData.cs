using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddClientsProjectsAndAdditionalCSMRevenueTypesSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "ContractEndDate", "ContractStartDate", "CreatedBy", "CreatedOn", "IsDeleted", "Location", "ModifiedBy", "ModifiedOn", "Name", "StatusId" },
                values: new object[,]
                {
                    { 1, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Aivontis", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 2, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Austills", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 3, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Axiad", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 4, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "BCT - Nama", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 5, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Beyondsoft", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 6, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Biolabs", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 7, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "BIS", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 8, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Deloitte", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 9, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Doxa", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 10, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Experis", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 11, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Globe Tele Services", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 12, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Growth IT", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 13, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "ING Financial Services LLC", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 14, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Miyahuna", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 15, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "NewVision", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 16, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Open Road App LLC", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 17, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Oracle", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 18, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "RKON", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 19, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "RSM", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 20, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Seasalt", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 21, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "SoftServe", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 22, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Sports Media Agency", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 23, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "USP", new Guid("10000000-0000-0000-0000-000000000001") },
                    { 24, null, null, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, null, "Votal", new Guid("10000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CSM", "CSMRevenueTypeId", "ClientId", "CreatedBy", "CreatedOn", "DeliveryHead", "Description", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "ProjectCode", "ProjectManager", "ProjectName" },
                values: new object[,]
                {
                    { 1, "Balan Ramaswamy", new Guid("e0000000-0000-0000-0000-000000000001"), 2, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000102", "Jayashree Kudale", "ACE System Upgrade" },
                    { 2, "Balan Ramaswamy", new Guid("e0000000-0000-0000-0000-000000000002"), 4, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Balan Ramaswamy", null, true, false, null, null, "NV000103", "Hari Parlapalli", "NAMA MDM Support" },
                    { 3, "Balan Ramaswamy", new Guid("e0000000-0000-0000-0000-000000000001"), 6, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Prakash Ghaitadke", null, true, false, null, null, "NV000110", "Kiranjit Bhowmick", "BioLabs" },
                    { 4, "Balan Ramaswamy", new Guid("e0000000-0000-0000-0000-000000000002"), 7, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000112", "Lakshmi Soumya Konambhotla", "Phase III NewDL Application" },
                    { 5, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 9, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000165", "Vikrant Kale", "Doxa - Promont - App Support" },
                    { 6, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 10, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000167", "Shailesh Erande", "Procom/Experis" },
                    { 7, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000002"), 12, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Prakash Ghaitadke", null, true, false, null, null, "NV000168", "Kiranjit Bhowmick", "FlightCheck Managed Service" },
                    { 8, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 18, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lakshmi Soumya Konambhotla", null, true, false, null, null, "NV000207", "Lakshmi Soumya Konambhotla", "BAS+ODS" },
                    { 9, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 18, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000208", "Manish Sharma", "Ludi offshore" },
                    { 10, "Balan Ramaswamy", new Guid("e0000000-0000-0000-0000-000000000001"), 23, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Balan Ramaswamy", null, true, false, null, null, "NV000214", "Balan Ramaswamy", "USP - CC&B - ConEd" },
                    { 11, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 12, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Prakash Ghaitadke", null, true, false, null, null, "NV000219", "Nilesh Vachpekar", "FlightCheck AMS" },
                    { 12, "Kapil Godani", new Guid("e0000000-0000-0000-0000-000000000001"), 18, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sandeep Dhir", null, true, false, null, null, "NV000238", "Sandeep Dhir", "Senior Advisory Role" },
                    { 13, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 3, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000257", "Uday D'Souza", "Axiad StaffAug" },
                    { 14, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000261", "-", "GCS SR&T Contractor SOW for NV" },
                    { 15, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000264", "-", "Omnia Data - NV" },
                    { 16, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000265", "-", "DataLens - NV" },
                    { 17, "Kapil Godani", new Guid("e0000000-0000-0000-0000-000000000001"), 11, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000276", "Anup Sikka", "Finbraine - staff-aug" },
                    { 18, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 19, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "-", null, true, false, null, null, "NV000278", "-", "RSM StaffAug" },
                    { 19, "Kapil Godani", new Guid("e0000000-0000-0000-0000-000000000001"), 24, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000280", "Aditi Joshi", "Votal Product development and enhancement" },
                    { 20, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000281", "-", "Levvia Transformation - NV" },
                    { 21, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000282", "-", "Levvia Production Support - NV" },
                    { 22, "Kapil Godani", new Guid("e0000000-0000-0000-0000-000000000001"), 11, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000283", "Prakash Ghaitadke", "GTS-StaffAug" },
                    { 23, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000284", null, "INK Transformation - NV" },
                    { 24, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000285", null, "Tax myInsights POD Team - NV" },
                    { 25, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000286", null, "New Vision Consulting Services - RPA RUN SOW" },
                    { 26, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000288", null, "Project work for Pillar 2" },
                    { 27, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000289", null, "Omnia Data Tie-Out - NV" },
                    { 28, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000290", null, "DWP (Distributed Work Portal) Transformation Project - NV" },
                    { 29, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000291", null, "DT Solutions - POD Team to support Global Learning" },
                    { 30, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000292", null, "DT Tax Solutions common SOW" },
                    { 31, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000293", null, "GDAS CoRe services - QA automation" },
                    { 32, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000294", null, "QAP Testing Automation Maintenance - NV" },
                    { 33, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000295", null, "GCS T&T Contractor SoW for NV" },
                    { 34, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000296", null, "Digital Platforms NV" },
                    { 35, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000297", null, "Deloitte.com STK INV" },
                    { 36, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000298", null, "Dayshape O&M" },
                    { 37, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000299", null, "DTTL TAX INTELA- NV" },
                    { 38, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000300", null, "Omnia Products - NV" },
                    { 39, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000302", null, "Omnia Data - NV - Second SoW" },
                    { 40, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000002"), 22, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000303", "Anupriya Jaiswal / Soumya Lakshmi", "RFC 2.0 Phase -2" },
                    { 41, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000305", null, "Serengeti Playwright Automation" },
                    { 42, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000306", null, "SOW for DTFM Reporting" },
                    { 43, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 18, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000307", "Prakash Ghaitadke", "Wrench - Azure DevOps" },
                    { 44, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 18, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000310", "Prakash Ghaitadke", "Neovance - API Integration between Amazon Connect and Engagement Platform" },
                    { 45, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000002"), 1, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000311", "Prakash Ghaitadke", "CommScope ITSM Automations" },
                    { 46, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000002"), 18, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000313", "Prakash Ghaitadke", "Neovance Cloud Operations and Engineering" },
                    { 47, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000314", null, "OnPremAnalytics (AERS) Data PG SOW" },
                    { 48, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000002"), 1, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Prakash Ghaitadke", null, true, false, null, null, "NV000315", "Kiranjit Bhowmick", "ElevateBio" },
                    { 49, "Ravindra Bhuyarkar", new Guid("e0000000-0000-0000-0000-000000000001"), 22, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000317", "Lakshmi Soumya Konambhotla", "RTHub & RTUK Website Maintenance" },
                    { 50, "Harshad Mandlecha", new Guid("e0000000-0000-0000-0000-000000000001"), 21, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Devarajan P", null, true, false, null, null, "NV000320", "Vedavati Shetty", "CISCO staff-Aug" },
                    { 51, "Kapil Godani", new Guid("e0000000-0000-0000-0000-000000000001"), 8, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shailesh Erande", null, true, false, null, null, "NV000321", null, "Automate Pro engagement ( ServiceNow COE)" },
                    { 52, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000002"), 20, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Devarajan P", null, true, false, null, null, "NV000322", "Uday D'Souza", "Managed Data Operations" },
                    { 53, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 22, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000324", "Lakshmi Soumya Konambhotla", "RFC2.0 Maintenance" },
                    { 54, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 22, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000325", "Lakshmi Soumya Konambhotla", "SFC Refinement Phase" },
                    { 55, "Saksham Sarode", new Guid("e0000000-0000-0000-0000-000000000002"), 5, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000326", "Lakshmi Soumya Konambhotla", "Client Portal and Data Lake Managed Platform Services" },
                    { 56, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000002"), 13, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saksham Sarode", null, true, false, null, null, "NV000329", "Sugato Goswami", "Testing Framework Phase-1" },
                    { 57, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 21, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000331", "Vedavati Shetty", "SoftServe - Atlassian" },
                    { 58, "Lawlesh Tiwari", new Guid("e0000000-0000-0000-0000-000000000001"), 22, "System", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vedavati Shetty", null, true, false, null, null, "NV000334", "Lakshmi Soumya Konambhotla", "SFC Pre-development functional updates" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 24);
        }
    }
}
