using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubPracticeAndUpdateImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubPracticeId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubPracticeMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PracticeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPracticeMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubPracticeMasters_Practices_PracticeId",
                        column: x => x.PracticeId,
                        principalTable: "Practices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SubPracticeId",
                table: "Employees",
                column: "SubPracticeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPracticeMasters_Name_PracticeId",
                table: "SubPracticeMasters",
                columns: new[] { "Name", "PracticeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubPracticeMasters_PracticeId",
                table: "SubPracticeMasters",
                column: "PracticeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_SubPracticeMasters_SubPracticeId",
                table: "Employees",
                column: "SubPracticeId",
                principalTable: "SubPracticeMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Seed new practices not in existing seed data
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Practices WHERE Name = 'Data and AI' AND IsDeleted = 0)
                    INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn) VALUES ('50000000-0000-0000-0000-000000000001', 'Data and AI', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM Practices WHERE Name = 'HR' AND IsDeleted = 0)
                    INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn) VALUES ('50000000-0000-0000-0000-000000000002', 'HR', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM Practices WHERE Name = 'Internal IT' AND IsDeleted = 0)
                    INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn) VALUES ('50000000-0000-0000-0000-000000000003', 'Internal IT', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM Practices WHERE Name = 'LAMP' AND IsDeleted = 0)
                    INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn) VALUES ('50000000-0000-0000-0000-000000000004', 'LAMP', 1, 0, '2026-01-01 00:00:00');
            ");

            // Seed SubPracticeMasters
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Administration' AND PracticeId = '40000000-0000-0000-0000-000000000001' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('337ae42c-b8ed-4850-9821-912807115dc3', 'Administration', '40000000-0000-0000-0000-000000000001', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Android' AND PracticeId = '40000000-0000-0000-0000-000000000013' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('bb061004-e7e3-4144-84f8-42d4acce7bca', 'Android', '40000000-0000-0000-0000-000000000013', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Application Development' AND PracticeId = '40000000-0000-0000-0000-000000000003' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('88f706b9-50a1-46be-b201-7d103d360cbe', 'Application Development', '40000000-0000-0000-0000-000000000003', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Application Development' AND PracticeId = '50000000-0000-0000-0000-000000000001' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('ced47138-ddb9-43da-aec6-5a0e70e5ead9', 'Application Development', '50000000-0000-0000-0000-000000000001', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Application Development' AND PracticeId = '40000000-0000-0000-0000-000000000005' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('988d1d3b-43b0-43b7-adbc-42ee0a27b76c', 'Application Development', '40000000-0000-0000-0000-000000000005', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Application Development' AND PracticeId = '40000000-0000-0000-0000-000000000009' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('3a61e088-cc07-4eb1-8cef-384457bde283', 'Application Development', '40000000-0000-0000-0000-000000000009', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Application Development' AND PracticeId = '50000000-0000-0000-0000-000000000004' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('14de8e1e-1d92-4b2f-b86a-a8ef0db7397b', 'Application Development', '50000000-0000-0000-0000-000000000004', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Application Development' AND PracticeId = '40000000-0000-0000-0000-000000000012' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('52910d12-a887-4798-ab1d-7dbeafae2f9e', 'Application Development', '40000000-0000-0000-0000-000000000012', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Application Development' AND PracticeId = '40000000-0000-0000-0000-000000000013' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('5601b2b4-67e4-470d-9e89-6f302adfd661', 'Application Development', '40000000-0000-0000-0000-000000000013', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Application Development' AND PracticeId = '40000000-0000-0000-0000-000000000015' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('d5342790-beae-4f69-bf0b-d803be3c1dbd', 'Application Development', '40000000-0000-0000-0000-000000000015', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'BI' AND PracticeId = '50000000-0000-0000-0000-000000000001' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('3c440cb6-7124-4774-b522-3dfd05683cec', 'BI', '50000000-0000-0000-0000-000000000001', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Business Analyst' AND PracticeId = '40000000-0000-0000-0000-000000000006' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('f65e1ba1-78ad-4ef4-bf5f-20d0f6c80680', 'Business Analyst', '40000000-0000-0000-0000-000000000006', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Business Assurance / Functional' AND PracticeId = '40000000-0000-0000-0000-000000000005' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('943dfcea-f5fb-492f-9eea-a5863265c2c6', 'Business Assurance / Functional', '40000000-0000-0000-0000-000000000005', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Business Assurance / Functional' AND PracticeId = '50000000-0000-0000-0000-000000000004' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('63995ef7-1525-4c55-8fb7-83c025b8171e', 'Business Assurance / Functional', '50000000-0000-0000-0000-000000000004', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Business Assurance / Functional' AND PracticeId = '40000000-0000-0000-0000-000000000012' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('580494d7-db4e-4ccf-ac21-3d7d47a0e0d7', 'Business Assurance / Functional', '40000000-0000-0000-0000-000000000012', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Business Assurance / Functional' AND PracticeId = '40000000-0000-0000-0000-000000000015' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('7dc17b5d-c141-4257-b430-ea837eb79f70', 'Business Assurance / Functional', '40000000-0000-0000-0000-000000000015', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Cloud and Devops' AND PracticeId = '40000000-0000-0000-0000-000000000012' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('06781d6a-6536-46d0-91d2-1fd2d992fd28', 'Cloud and Devops', '40000000-0000-0000-0000-000000000012', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Data' AND PracticeId = '40000000-0000-0000-0000-000000000003' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('c3f96d1e-14f3-4088-abf3-e766f41abfba', 'Data', '40000000-0000-0000-0000-000000000003', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Data' AND PracticeId = '50000000-0000-0000-0000-000000000001' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('92be13d3-136c-4960-abb8-5f26c478a7e2', 'Data', '50000000-0000-0000-0000-000000000001', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Data Engineering' AND PracticeId = '40000000-0000-0000-0000-000000000003' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('34ed58b0-54bb-4d3f-8ab5-06d893aebb52', 'Data Engineering', '40000000-0000-0000-0000-000000000003', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Data Engineering' AND PracticeId = '50000000-0000-0000-0000-000000000001' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('f7a76000-1ca2-4dee-9b48-f2269257c048', 'Data Engineering', '50000000-0000-0000-0000-000000000001', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Data Scientist' AND PracticeId = '40000000-0000-0000-0000-000000000003' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('c90e1812-53cf-4336-864c-506c2a66f2c9', 'Data Scientist', '40000000-0000-0000-0000-000000000003', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Data Scientist' AND PracticeId = '50000000-0000-0000-0000-000000000001' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('05aa7a2f-bdcb-47b6-bddd-690585ee0a0c', 'Data Scientist', '50000000-0000-0000-0000-000000000001', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Delivery Excellence' AND PracticeId = '40000000-0000-0000-0000-000000000004' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('ef30931f-0b58-4939-8e4a-f9236be82871', 'Delivery Excellence', '40000000-0000-0000-0000-000000000004', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Development' AND PracticeId = '40000000-0000-0000-0000-000000000017' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('7f863fd6-99b7-41a5-92f7-d18d6084b1b1', 'Development', '40000000-0000-0000-0000-000000000017', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Digital Assurance' AND PracticeId = '40000000-0000-0000-0000-000000000005' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('6d7e3850-8929-4e2b-a359-9d83bd6cf6c1', 'Digital Assurance', '40000000-0000-0000-0000-000000000005', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Digital Product Studio' AND PracticeId = '40000000-0000-0000-0000-000000000006' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('52e6a40b-54a0-4196-9c96-fbb77b9782ed', 'Digital Product Studio', '40000000-0000-0000-0000-000000000006', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Finance' AND PracticeId = '40000000-0000-0000-0000-000000000007' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('c2bab55a-6382-48ef-8c91-56f4434722d1', 'Finance', '40000000-0000-0000-0000-000000000007', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Functional' AND PracticeId = '40000000-0000-0000-0000-000000000017' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('37cd0752-9447-4aed-80e4-4973f857fcf9', 'Functional', '40000000-0000-0000-0000-000000000017', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'IoS' AND PracticeId = '40000000-0000-0000-0000-000000000013' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('a23736dc-9edf-433d-97de-14ff7e237358', 'IoS', '40000000-0000-0000-0000-000000000013', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'ITIS' AND PracticeId = '50000000-0000-0000-0000-000000000003' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('f2d719da-9b85-4c2d-99a0-4667a418b855', 'ITIS', '50000000-0000-0000-0000-000000000003', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'ITIS' AND PracticeId = '40000000-0000-0000-0000-000000000010' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('2febcb75-e314-4f63-bcb8-bfbdc019f2e6', 'ITIS', '40000000-0000-0000-0000-000000000010', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'ITIS' AND PracticeId = '40000000-0000-0000-0000-000000000016' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('54d2cc56-0ac2-4528-aefa-46b045f6e664', 'ITIS', '40000000-0000-0000-0000-000000000016', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Management' AND PracticeId = '40000000-0000-0000-0000-000000000002' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('7c1ccd1c-0556-466f-b3db-fdd213dc6fb3', 'Management', '40000000-0000-0000-0000-000000000002', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Management' AND PracticeId = '40000000-0000-0000-0000-000000000004' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('6e569052-596a-47df-8379-c3ecb41ac2e6', 'Management', '40000000-0000-0000-0000-000000000004', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Management' AND PracticeId = '40000000-0000-0000-0000-000000000006' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('8e4c7893-9fca-49ac-8c94-d3489fc013ec', 'Management', '40000000-0000-0000-0000-000000000006', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Management' AND PracticeId = '40000000-0000-0000-0000-000000000011' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('3c0cd1d5-1e54-49ef-8229-7cdb633886d1', 'Management', '40000000-0000-0000-0000-000000000011', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Marketing' AND PracticeId = '40000000-0000-0000-0000-000000000016' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('37398092-2f5b-4af9-b4d2-e18d9b452e66', 'Marketing', '40000000-0000-0000-0000-000000000016', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Others' AND PracticeId = '40000000-0000-0000-0000-000000000004' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('01f85dd2-6a0a-4b7b-8cbf-35433b19fbd0', 'Others', '40000000-0000-0000-0000-000000000004', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'PMO' AND PracticeId = '40000000-0000-0000-0000-000000000002' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('6f0ac894-8780-4e3c-80ca-24d76cb90c32', 'PMO', '40000000-0000-0000-0000-000000000002', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Presales' AND PracticeId = '40000000-0000-0000-0000-000000000016' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('8211b01b-1040-4c1c-9fca-887dad4b2a56', 'Presales', '40000000-0000-0000-0000-000000000016', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Product Design' AND PracticeId = '40000000-0000-0000-0000-000000000006' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('e082cdc7-b712-4476-be7f-e9789cd7f250', 'Product Design', '40000000-0000-0000-0000-000000000006', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Product Management' AND PracticeId = '40000000-0000-0000-0000-000000000006' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('6771d015-5962-4a9c-b9ed-46cd78918641', 'Product Management', '40000000-0000-0000-0000-000000000006', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Quality Assurance' AND PracticeId = '40000000-0000-0000-0000-000000000005' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('725b8002-c1f6-4ce2-a001-52f80607933f', 'Quality Assurance', '40000000-0000-0000-0000-000000000005', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'RPA' AND PracticeId = '40000000-0000-0000-0000-000000000015' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('6d29085d-a85d-4c16-b09e-dacd3d654188', 'RPA', '40000000-0000-0000-0000-000000000015', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Sales and Marketing' AND PracticeId = '40000000-0000-0000-0000-000000000016' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('bbfceaed-a5c9-4608-bb1c-7796bb4ddc2a', 'Sales and Marketing', '40000000-0000-0000-0000-000000000016', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Scrum Master' AND PracticeId = '50000000-0000-0000-0000-000000000001' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('82c765a7-4835-459e-9f26-4c96044d87c2', 'Scrum Master', '50000000-0000-0000-0000-000000000001', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Scrum Master' AND PracticeId = '40000000-0000-0000-0000-000000000004' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('c6ca23c2-c347-4c3f-a83e-87bb612cf907', 'Scrum Master', '40000000-0000-0000-0000-000000000004', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Talent Acquisition' AND PracticeId = '50000000-0000-0000-0000-000000000002' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('574243bb-2d51-4fd5-8b16-64048db6c05a', 'Talent Acquisition', '50000000-0000-0000-0000-000000000002', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Talent Management & Operations' AND PracticeId = '50000000-0000-0000-0000-000000000002' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('82484b97-7b34-47e6-a68b-ed48420a2a12', 'Talent Management & Operations', '50000000-0000-0000-0000-000000000002', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Technical Lead' AND PracticeId = '40000000-0000-0000-0000-000000000012' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('93f8806b-24a1-4856-9c39-5ed42aa78977', 'Technical Lead', '40000000-0000-0000-0000-000000000012', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Technology Assurance / Automation' AND PracticeId = '40000000-0000-0000-0000-000000000005' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('e7cf0f2a-e97e-4140-a3c2-c45de4b0e5a0', 'Technology Assurance / Automation', '40000000-0000-0000-0000-000000000005', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Travel and front tesk' AND PracticeId = '50000000-0000-0000-0000-000000000002' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('704f57a3-50c0-4c04-bb6d-bdc235dbe4bc', 'Travel and front tesk', '50000000-0000-0000-0000-000000000002', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Utility Testing' AND PracticeId = '40000000-0000-0000-0000-000000000001' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('5631f807-ccda-479f-a2ff-5b06629b38ee', 'Utility Testing', '40000000-0000-0000-0000-000000000001', 1, 0, '2026-01-01 00:00:00');
                IF NOT EXISTS (SELECT 1 FROM SubPracticeMasters WHERE Name = 'Utility Testing' AND PracticeId = '40000000-0000-0000-0000-000000000005' AND IsDeleted = 0)
                    INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn) VALUES ('640e7070-7252-446f-b264-2fcabcb4d171', 'Utility Testing', '40000000-0000-0000-0000-000000000005', 1, 0, '2026-01-01 00:00:00');
            ");

            // Drop old EmployeeImportStaging and recreate with new columns matching updated template
            migrationBuilder.Sql("DROP TABLE IF EXISTS EmployeeImportStaging");

            migrationBuilder.Sql(@"
                CREATE TABLE EmployeeImportStaging (
                    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                    BatchId UNIQUEIDENTIFIER NOT NULL,
                    EmployeeCode NVARCHAR(50) NULL,
                    FullName NVARCHAR(200) NULL,
                    Email NVARCHAR(200) NULL,
                    EmployeeType NVARCHAR(100) NULL,
                    Designation NVARCHAR(100) NULL,
                    Practice NVARCHAR(100) NULL,
                    SubPractice NVARCHAR(100) NULL,
                    Location NVARCHAR(100) NULL,
                    ReportingManager NVARCHAR(200) NULL,
                    PracticeHead NVARCHAR(200) NULL,
                    ActiveStatus NVARCHAR(50) NULL,
                    DOJ DATETIME NULL,
                    LWD DATETIME NULL,
                    ImportedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    ImportedBy NVARCHAR(200) NULL
                );
            ");

            // Drop old stored procedure and recreate with updated logic (handles SubPractice, L1 Manager, Practice Head)
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ProcessEmployeeImport");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ProcessEmployeeImport
                    @BatchId UNIQUEIDENTIFIER,
                    @ImportedBy NVARCHAR(200) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @TotalRows INT = 0;
                    DECLARE @ImportedRows INT = 0;
                    DECLARE @FailedRows INT = 0;
                    DECLARE @DeletedRows INT = 0;
                    DECLARE @DefaultWorkModelId UNIQUEIDENTIFIER;
                    DECLARE @DefaultDeptTypeId UNIQUEIDENTIFIER;

                    CREATE TABLE #Errors (
                        RowNum INT IDENTITY(1,1),
                        EmployeeCode NVARCHAR(50),
                        ErrorMessage NVARCHAR(MAX)
                    );

                    BEGIN TRY
                        SELECT @TotalRows = COUNT(1) FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        IF @TotalRows = 0
                        BEGIN
                            SELECT 0 AS TotalRows, 0 AS ImportedRows, 0 AS FailedRows, 0 AS DeletedRows;
                            RETURN;
                        END

                        SELECT TOP 1 @DefaultWorkModelId = Id FROM WorkModelMasters WHERE IsDeleted = 0 AND IsActive = 1;
                        SELECT TOP 1 @DefaultDeptTypeId = Id FROM DepartmentTypeMasters WHERE IsDeleted = 0 AND IsActive = 1;

                        -- Required field validation
                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Employee Code is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (EmployeeCode IS NULL OR EmployeeCode = '');

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Email is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (Email IS NULL OR Email = '');

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Full Name is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (FullName IS NULL OR FullName = '');

                        -- Auto-create missing designations (Role)
                        INSERT INTO DesignationMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Designation, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Designation IS NOT NULL AND s.Designation != ''
                          AND NOT EXISTS (SELECT 1 FROM DesignationMasters d WHERE d.Name = s.Designation AND d.IsDeleted = 0);

                        -- Auto-create missing practices (OU 4 - Practice)
                        INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Practice, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Practice IS NOT NULL AND s.Practice != ''
                          AND NOT EXISTS (SELECT 1 FROM Practices p WHERE p.Name = s.Practice AND p.IsDeleted = 0);

                        -- Auto-create missing sub-practices (OU 5 - Sub-practice, linked to Practice)
                        INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.SubPractice, p.Id, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        INNER JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        WHERE s.BatchId = @BatchId AND s.SubPractice IS NOT NULL AND s.SubPractice != ''
                          AND NOT EXISTS (SELECT 1 FROM SubPracticeMasters sp WHERE sp.Name = s.SubPractice AND sp.PracticeId = p.Id AND sp.IsDeleted = 0);

                        -- Auto-create missing locations
                        INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Location, s.Location, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Location IS NOT NULL AND s.Location != ''
                          AND NOT EXISTS (SELECT 1 FROM Locations l WHERE l.Name = s.Location AND l.IsDeleted = 0);

                        -- Auto-create missing employment types (FTE/ Consultant)
                        INSERT INTO EmploymentTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.EmployeeType, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeType IS NOT NULL AND s.EmployeeType != ''
                          AND NOT EXISTS (SELECT 1 FROM EmploymentTypeMasters e WHERE e.Name = s.EmployeeType AND e.IsDeleted = 0);

                        -- Duplicate validation within batch
                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT s.EmployeeCode, 'Duplicate Employee Code ''' + s.EmployeeCode + ''' within upload'
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeCode IS NOT NULL AND s.EmployeeCode != ''
                          AND EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s2
                            WHERE s2.BatchId = @BatchId AND s2.EmployeeCode = s.EmployeeCode AND s2.Id != s.Id
                          );

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT s.EmployeeCode, 'Duplicate Email ''' + s.Email + ''' within upload'
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Email IS NOT NULL AND s.Email != ''
                          AND EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s2
                            WHERE s2.BatchId = @BatchId AND s2.Email = s.Email AND s2.Id != s.Id
                          );

                        IF EXISTS (SELECT 1 FROM #Errors)
                        BEGIN
                            SET @FailedRows = (SELECT COUNT(DISTINCT EmployeeCode) FROM #Errors WHERE EmployeeCode IS NOT NULL AND EmployeeCode != '');
                            SELECT @TotalRows AS TotalRows, 0 AS ImportedRows, @FailedRows AS FailedRows, 0 AS DeletedRows;
                            DROP TABLE #Errors;
                            RETURN;
                        END

                        BEGIN TRANSACTION;

                        SELECT @DeletedRows = COUNT(1) FROM Employees WHERE IsDeleted = 0;

                        -- Clear self-referencing FKs before deleting
                        UPDATE Employees SET ReportingManagerId = NULL, PracticeHeadId = NULL;

                        DELETE FROM EmployeeSkills;
                        DELETE FROM ResourceAllocations;
                        DELETE FROM Employees;

                        -- Phase 1: Insert employees with all FKs except self-referencing (ReportingManagerId, PracticeHeadId)
                        INSERT INTO Employees (
                            EmployeeCode, FullName, Email,
                            DOJ, LWD,
                            DesignationId,
                            EmploymentTypeId, LocationId, PracticeId, SubPracticeId,
                            WorkModelId, DepartmentTypeId,
                            StatusId,
                            PriorExperience,
                            IsDeleted, CreatedOn, CreatedBy
                        )
                        SELECT
                            s.EmployeeCode,
                            s.FullName,
                            s.Email,
                            ISNULL(s.DOJ, GETUTCDATE()),
                            s.LWD,
                            dm.Id,
                            etm.Id,
                            l.Id,
                            p.Id,
                            spm.Id,
                            ISNULL(@DefaultWorkModelId, (SELECT TOP 1 Id FROM WorkModelMasters WHERE IsDeleted = 0 AND IsActive = 1)),
                            ISNULL(@DefaultDeptTypeId, (SELECT TOP 1 Id FROM DepartmentTypeMasters WHERE IsDeleted = 0 AND IsActive = 1)),
                            ISNULL(st.Id, (SELECT TOP 1 Id FROM StatusMasters WHERE Name = 'Active' AND IsDeleted = 0)),
                            0,
                            0,
                            GETUTCDATE(),
                            @ImportedBy
                        FROM EmployeeImportStaging s
                        LEFT JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        LEFT JOIN SubPracticeMasters spm ON spm.Name = s.SubPractice AND spm.PracticeId = p.Id AND spm.IsDeleted = 0
                        LEFT JOIN DesignationMasters dm ON dm.Name = s.Designation AND dm.IsDeleted = 0
                        LEFT JOIN Locations l ON l.Name = s.Location AND l.IsDeleted = 0
                        LEFT JOIN EmploymentTypeMasters etm ON etm.Name = s.EmployeeType AND etm.IsDeleted = 0
                        LEFT JOIN StatusMasters st ON st.Name = ISNULL(s.ActiveStatus, 'Active') AND st.IsDeleted = 0
                        WHERE s.BatchId = @BatchId;

                        SET @ImportedRows = @@ROWCOUNT;

                        -- Phase 2: Update self-referencing FKs (L1 Manager, Practice Head) by matching full names
                        UPDATE e
                        SET e.ReportingManagerId = rm.Id
                        FROM Employees e
                        INNER JOIN EmployeeImportStaging s ON s.EmployeeCode = e.EmployeeCode AND s.BatchId = @BatchId
                        INNER JOIN Employees rm ON rm.FullName = s.ReportingManager AND rm.IsDeleted = 0
                        WHERE s.ReportingManager IS NOT NULL AND s.ReportingManager != '';

                        UPDATE e
                        SET e.PracticeHeadId = ph.Id
                        FROM Employees e
                        INNER JOIN EmployeeImportStaging s ON s.EmployeeCode = e.EmployeeCode AND s.BatchId = @BatchId
                        INNER JOIN Employees ph ON ph.FullName = s.PracticeHead AND ph.IsDeleted = 0
                        WHERE s.PracticeHead IS NOT NULL AND s.PracticeHead != '';

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        COMMIT TRANSACTION;

                        SELECT @TotalRows AS TotalRows, @ImportedRows AS ImportedRows, 0 AS FailedRows, @DeletedRows AS DeletedRows;

                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        SELECT @TotalRows AS TotalRows, 0 AS ImportedRows, @TotalRows AS FailedRows, 0 AS DeletedRows;
                    END CATCH;

                    DROP TABLE #Errors;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore old stored procedure first
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ProcessEmployeeImport");

            // Restore old staging table
            migrationBuilder.Sql("DROP TABLE IF EXISTS EmployeeImportStaging");

            // Recreate old staging table
            migrationBuilder.Sql(@"
                CREATE TABLE EmployeeImportStaging (
                    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                    BatchId UNIQUEIDENTIFIER NOT NULL,
                    EmployeeCode NVARCHAR(50) NULL,
                    FirstName NVARCHAR(100) NULL,
                    LastName NVARCHAR(100) NULL,
                    Email NVARCHAR(200) NULL,
                    EmployeeType NVARCHAR(100) NULL,
                    Designation NVARCHAR(100) NULL,
                    Practice NVARCHAR(100) NULL,
                    ReportingManager NVARCHAR(200) NULL,
                    Location NVARCHAR(100) NULL,
                    WorkModel NVARCHAR(100) NULL,
                    Experience DECIMAL(10,2) NULL,
                    Skills NVARCHAR(MAX) NULL,
                    DOJ DATETIME NULL,
                    PhoneNumber NVARCHAR(20) NULL,
                    Client NVARCHAR(200) NULL,
                    Onboarding NVARCHAR(100) NULL,
                    PracticeHead NVARCHAR(200) NULL,
                    SubPractice NVARCHAR(100) NULL,
                    ImportedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    ImportedBy NVARCHAR(200) NULL
                );
            ");

            // Recreate old stored procedure
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ProcessEmployeeImport
                    @BatchId UNIQUEIDENTIFIER,
                    @ImportedBy NVARCHAR(200) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @TotalRows INT = 0;
                    DECLARE @ImportedRows INT = 0;
                    DECLARE @FailedRows INT = 0;
                    DECLARE @DeletedRows INT = 0;
                    DECLARE @DefaultDeptTypeId UNIQUEIDENTIFIER;
                    DECLARE @DefaultStatusId UNIQUEIDENTIFIER;

                    CREATE TABLE #Errors (
                        RowNum INT IDENTITY(1,1),
                        EmployeeCode NVARCHAR(50),
                        ErrorMessage NVARCHAR(MAX)
                    );

                    BEGIN TRY
                        SELECT @TotalRows = COUNT(1) FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        IF @TotalRows = 0
                        BEGIN
                            SELECT 0 AS TotalRows, 0 AS ImportedRows, 0 AS FailedRows, 0 AS DeletedRows;
                            RETURN;
                        END

                        SELECT TOP 1 @DefaultDeptTypeId = Id FROM DepartmentTypeMasters WHERE IsDeleted = 0 AND IsActive = 1;
                        SELECT TOP 1 @DefaultStatusId = Id FROM StatusMasters WHERE IsDeleted = 0 AND IsActive = 1;

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Employee Code is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (EmployeeCode IS NULL OR EmployeeCode = '');

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Email is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (Email IS NULL OR Email = '');

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'First Name is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (FirstName IS NULL OR FirstName = '');

                        INSERT INTO DesignationMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Designation, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Designation IS NOT NULL AND s.Designation != ''
                          AND NOT EXISTS (SELECT 1 FROM DesignationMasters d WHERE d.Name = s.Designation AND d.IsDeleted = 0);

                        INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Practice, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Practice IS NOT NULL AND s.Practice != ''
                          AND NOT EXISTS (SELECT 1 FROM Practices p WHERE p.Name = s.Practice AND p.IsDeleted = 0);

                        INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Location, s.Location, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Location IS NOT NULL AND s.Location != ''
                          AND NOT EXISTS (SELECT 1 FROM Locations l WHERE l.Name = s.Location AND l.IsDeleted = 0);

                        INSERT INTO WorkModelMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.WorkModel, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.WorkModel IS NOT NULL AND s.WorkModel != ''
                          AND NOT EXISTS (SELECT 1 FROM WorkModelMasters w WHERE w.Name = s.WorkModel AND w.IsDeleted = 0);

                        INSERT INTO EmploymentTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.EmployeeType, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeType IS NOT NULL AND s.EmployeeType != ''
                          AND NOT EXISTS (SELECT 1 FROM EmploymentTypeMasters e WHERE e.Name = s.EmployeeType AND e.IsDeleted = 0);

                        INSERT INTO Clients (Name, StatusId, IsDeleted, CreatedOn)
                        SELECT DISTINCT s.Client, @DefaultStatusId, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Client IS NOT NULL AND s.Client != ''
                          AND NOT EXISTS (SELECT 1 FROM Clients c WHERE c.Name = s.Client AND c.IsDeleted = 0);

                        INSERT INTO OnboardingTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Onboarding, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Onboarding IS NOT NULL AND s.Onboarding != ''
                          AND NOT EXISTS (SELECT 1 FROM OnboardingTypeMasters o WHERE o.Name = s.Onboarding AND o.IsDeleted = 0);

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT s.EmployeeCode, 'Duplicate Employee Code ''' + s.EmployeeCode + ''' within upload'
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeCode IS NOT NULL AND s.EmployeeCode != ''
                          AND EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s2
                            WHERE s2.BatchId = @BatchId AND s2.EmployeeCode = s.EmployeeCode AND s2.Id != s.Id
                          );

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT s.EmployeeCode, 'Duplicate Email ''' + s.Email + ''' within upload'
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Email IS NOT NULL AND s.Email != ''
                          AND EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s2
                            WHERE s2.BatchId = @BatchId AND s2.Email = s.Email AND s2.Id != s.Id
                          );

                        IF EXISTS (SELECT 1 FROM #Errors)
                        BEGIN
                            SET @FailedRows = (SELECT COUNT(DISTINCT EmployeeCode) FROM #Errors WHERE EmployeeCode IS NOT NULL AND EmployeeCode != '');
                            SELECT @TotalRows AS TotalRows, 0 AS ImportedRows, @FailedRows AS FailedRows, 0 AS DeletedRows;
                            DROP TABLE #Errors;
                            RETURN;
                        END

                        BEGIN TRANSACTION;

                        SELECT @DeletedRows = COUNT(1) FROM Employees WHERE IsDeleted = 0;

                        -- Clear self-referencing FKs before deleting
                        UPDATE Employees SET ReportingManagerId = NULL, PracticeHeadId = NULL;

                        DELETE FROM EmployeeSkills;
                        DELETE FROM ResourceAllocations;
                        DELETE FROM Employees;

                        INSERT INTO Employees (
                            EmployeeCode, FirstName, LastName, FullName, Email,
                            DOJ, ExperienceYears, PriorExperience, RelevantExperience,
                            DesignationId, ClientId, OnboardingTypeId,
                            EmploymentTypeId, LocationId, WorkModelId, PracticeId,
                            DepartmentTypeId, StatusId,
                            MobileNumber, IsDeleted, CreatedOn, CreatedBy
                        )
                        SELECT
                            s.EmployeeCode,
                            s.FirstName,
                            s.LastName,
                            LTRIM(RTRIM(ISNULL(s.FirstName, '') + ' ' + ISNULL(s.LastName, ''))),
                            s.Email,
                            ISNULL(s.DOJ, GETUTCDATE()),
                            s.Experience,
                            ISNULL(s.Experience, 0),
                            s.Experience,
                            dm.Id,
                            c.Id,
                            ot.Id,
                            etm.Id,
                            l.Id,
                            wm.Id,
                            p.Id,
                            ISNULL(@DefaultDeptTypeId, (SELECT TOP 1 Id FROM DepartmentTypeMasters WHERE IsDeleted = 0 AND IsActive = 1)),
                            ISNULL(@DefaultStatusId, (SELECT TOP 1 Id FROM StatusMasters WHERE IsDeleted = 0 AND IsActive = 1)),
                            s.PhoneNumber,
                            0,
                            GETUTCDATE(),
                            @ImportedBy
                        FROM EmployeeImportStaging s
                        LEFT JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        LEFT JOIN DesignationMasters dm ON dm.Name = s.Designation AND dm.IsDeleted = 0
                        LEFT JOIN Locations l ON l.Name = s.Location AND l.IsDeleted = 0
                        LEFT JOIN WorkModelMasters wm ON wm.Name = s.WorkModel AND wm.IsDeleted = 0
                        LEFT JOIN EmploymentTypeMasters etm ON etm.Name = s.EmployeeType AND etm.IsDeleted = 0
                        LEFT JOIN Clients c ON c.Name = s.Client AND c.IsDeleted = 0
                        LEFT JOIN OnboardingTypeMasters ot ON ot.Name = s.Onboarding AND ot.IsDeleted = 0
                        WHERE s.BatchId = @BatchId;

                        SET @ImportedRows = @@ROWCOUNT;

                        INSERT INTO Skills (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), LTRIM(RTRIM(value)), 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        CROSS APPLY STRING_SPLIT(s.Skills, ',')
                        WHERE s.BatchId = @BatchId
                          AND s.Skills IS NOT NULL AND s.Skills != ''
                          AND LTRIM(RTRIM(value)) != ''
                          AND NOT EXISTS (
                            SELECT 1 FROM Skills sk
                            WHERE LTRIM(RTRIM(sk.Name)) = LTRIM(RTRIM(value)) AND sk.IsDeleted = 0
                          );

                        INSERT INTO EmployeeSkills (EmployeeId, SkillId)
                        SELECT DISTINCT e.Id, sk.Id
                        FROM EmployeeImportStaging s
                        INNER JOIN Employees e ON e.EmployeeCode = s.EmployeeCode
                        CROSS APPLY STRING_SPLIT(s.Skills, ',')
                        INNER JOIN Skills sk ON LTRIM(RTRIM(sk.Name)) = LTRIM(RTRIM(value)) AND sk.IsDeleted = 0
                        WHERE s.BatchId = @BatchId
                          AND s.Skills IS NOT NULL AND s.Skills != ''
                          AND LTRIM(RTRIM(value)) != ''
                          AND NOT EXISTS (
                            SELECT 1 FROM EmployeeSkills es
                            WHERE es.EmployeeId = e.Id AND es.SkillId = sk.Id
                          );

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        COMMIT TRANSACTION;

                        SELECT @TotalRows AS TotalRows, @ImportedRows AS ImportedRows, 0 AS FailedRows, @DeletedRows AS DeletedRows;

                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        SELECT @TotalRows AS TotalRows, 0 AS ImportedRows, @TotalRows AS FailedRows, 0 AS DeletedRows;
                    END CATCH;

                    DROP TABLE #Errors;
                END
            ");

            // Remove model changes
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_SubPracticeMasters_SubPracticeId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "SubPracticeMasters");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SubPracticeId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SubPracticeId",
                table: "Employees");
        }
    }
}
