/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace EducationAccelerator.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    ParentAcademicSessionId = table.Column<string>(type: "TEXT", nullable: true),
                    SchoolYear = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicSessions_AcademicSessions_ParentAcademicSessionId",
                        column: x => x.ParentAcademicSessionId,
                        principalTable: "AcademicSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineItemCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItemCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OauthNonces",
                columns: table => new
                {
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OauthNonces", x => x.Value);
                });

            migrationBuilder.CreateTable(
                name: "OauthTokens",
                columns: table => new
                {
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OauthTokens", x => x.Value);
                });

            migrationBuilder.CreateTable(
                name: "Orgs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<string>(type: "TEXT", nullable: true),
                    ParentOrgId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orgs_Orgs_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Orgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    EnabledUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    FamilyName = table.Column<string>(type: "TEXT", nullable: false),
                    GivenName = table.Column<string>(type: "TEXT", nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    SMS = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    _grades = table.Column<string>(type: "TEXT", nullable: true),
                    _userIds = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CourseCode = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    OrgId = table.Column<string>(type: "TEXT", nullable: false),
                    SchoolYearAcademicSessionId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    _grades = table.Column<string>(type: "TEXT", nullable: true),
                    _subjectCodes = table.Column<string>(type: "TEXT", nullable: true),
                    _resources = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Orgs_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Orgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_AcademicSessions_SchoolYearAcademicSessionId",
                        column: x => x.SchoolYearAcademicSessionId,
                        principalTable: "AcademicSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAgents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AgentUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SubjectUserId = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAgents_Users_AgentUserId",
                        column: x => x.AgentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAgents_Users_SubjectUserId",
                        column: x => x.SubjectUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserOrgs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    OrgId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrgs_Orgs_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Orgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrgs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IMSClasses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IMSClassCode = table.Column<string>(type: "TEXT", nullable: true),
                    IMSClassType = table.Column<int>(type: "INTEGER", nullable: false),
                    CourseId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    SchoolOrgId = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    _grades = table.Column<string>(type: "TEXT", nullable: true),
                    _periods = table.Column<string>(type: "TEXT", nullable: true),
                    _subjectCodes = table.Column<string>(type: "TEXT", nullable: true),
                    _resources = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMSClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IMSClasses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IMSClasses_Orgs_SchoolOrgId",
                        column: x => x.SchoolOrgId,
                        principalTable: "Orgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IMSClassId = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Primary = table.Column<bool>(type: "INTEGER", nullable: true),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    SchoolOrgId = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_IMSClasses_IMSClassId",
                        column: x => x.IMSClassId,
                        principalTable: "IMSClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Orgs_SchoolOrgId",
                        column: x => x.SchoolOrgId,
                        principalTable: "Orgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IMSClassAcademicSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AcademicSessionId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IMSClassId = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMSClassAcademicSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IMSClassAcademicSessions_AcademicSessions_AcademicSessionId",
                        column: x => x.AcademicSessionId,
                        principalTable: "AcademicSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IMSClassAcademicSessions_IMSClasses_IMSClassId",
                        column: x => x.IMSClassId,
                        principalTable: "IMSClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AcademicSessionId = table.Column<string>(type: "TEXT", nullable: false),
                    AssignDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IMSClassId = table.Column<string>(type: "TEXT", nullable: false),
                    LineItemCategoryId = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    ResultValueMax = table.Column<float>(type: "REAL", nullable: false),
                    ResultValueMin = table.Column<float>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineItems_AcademicSessions_AcademicSessionId",
                        column: x => x.AcademicSessionId,
                        principalTable: "AcademicSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineItems_IMSClasses_IMSClassId",
                        column: x => x.IMSClassId,
                        principalTable: "IMSClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineItems_LineItemCategories_LineItemCategoryId",
                        column: x => x.LineItemCategoryId,
                        principalTable: "LineItemCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LineItemId = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    ScoreDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScoreStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentUserId = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Results_LineItems_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "LineItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_Users_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Importance = table.Column<int>(type: "TEXT", nullable: true),
                    VendorResourceId = table.Column<string>(type: "TEXT", nullable: false),
                    VendorId = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationId = table.Column<string>(type: "TEXT", nullable: true),
                    _roles = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table => 
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Demographics",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Sex = table.Column<Vocabulary.Gender>(type: "TEXT", nullable: true),
                    AmericanIndianOrAlaskaNative = table.Column<bool>(type: "TEXT", nullable: true),
                    Asian = table.Column<bool>(type: "TEXT", nullable: true),
                    BlackOrAfricanAmerican = table.Column<bool>(type: "TEXT", nullable: true),
                    NativeHawaiianOrOtherPacificIslander = table.Column<bool>(type: "TEXT", nullable: true),
                    White = table.Column<bool>(type: "TEXT", nullable: true),
                    DemographicRaceTwoOrMoreRaces = table.Column<bool>(type: "TEXT", nullable: true),
                    HispanicOrLatinoEthnicity = table.Column<bool>(type: "TEXT", nullable: true),
                    CountryOfBirthCode = table.Column<string>(type: "TEXT", nullable: true),
                    StateOfBirthAbbreviation = table.Column<string>(type: "TEXT", nullable: true),
                    CityOfBirth = table.Column<string>(type: "TEXT", nullable: true),
                    PublicSchoolResidenceStatus = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demographic", x => x.Id);
                });
            migrationBuilder.CreateIndex(
                name: "IX_AcademicSessions_ParentAcademicSessionId",
                table: "AcademicSessions",
                column: "ParentAcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_OrgId",
                table: "Courses",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SchoolYearAcademicSessionId",
                table: "Courses",
                column: "SchoolYearAcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_IMSClassId",
                table: "Enrollments",
                column: "IMSClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_SchoolOrgId",
                table: "Enrollments",
                column: "SchoolOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IMSClassAcademicSessions_AcademicSessionId",
                table: "IMSClassAcademicSessions",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_IMSClassAcademicSessions_IMSClassId",
                table: "IMSClassAcademicSessions",
                column: "IMSClassId");

            migrationBuilder.CreateIndex(
                name: "IX_IMSClasses_CourseId",
                table: "IMSClasses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_IMSClasses_SchoolOrgId",
                table: "IMSClasses",
                column: "SchoolOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_AcademicSessionId",
                table: "LineItems",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_IMSClassId",
                table: "LineItems",
                column: "IMSClassId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_LineItemCategoryId",
                table: "LineItems",
                column: "LineItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orgs_ParentId",
                table: "Orgs",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_LineItemId",
                table: "Results",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_StudentUserId",
                table: "Results",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgents_AgentUserId",
                table: "UserAgents",
                column: "AgentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgents_SubjectUserId",
                table: "UserAgents",
                column: "SubjectUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrgs_OrgId",
                table: "UserOrgs",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrgs_UserId",
                table: "UserOrgs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "IMSClassAcademicSessions");

            migrationBuilder.DropTable(
                name: "OauthNonces");

            migrationBuilder.DropTable(
                name: "OauthTokens");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "UserAgents");

            migrationBuilder.DropTable(
                name: "UserOrgs");

            migrationBuilder.DropTable(
                name: "LineItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "IMSClasses");

            migrationBuilder.DropTable(
                name: "LineItemCategories");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Orgs");

            migrationBuilder.DropTable(
                name: "AcademicSessions");

            migrationBuilder.DropTable(
                name: "Demographics");

            migrationBuilder.DropTable(
                name: "Resources");
        }
    }
}
