namespace ErasmusPlus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Full : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agreements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        State = c.Int(nullable: false),
                        StoragePath = c.String(),
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Scholarship = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinancingSource = c.String(),
                        ErasmusUserId = c.String(nullable: false, maxLength: 128),
                        SourceUniversityId = c.Int(nullable: false),
                        TargetUniversityId = c.Int(nullable: false),
                        SourceFacultyId = c.Int(nullable: false),
                        TargetFacultyId = c.Int(nullable: false),
                        SourceFieldOfStudyId = c.Int(nullable: false),
                        TargetFieldOfStudyId = c.Int(nullable: false),
                        StudyField = c.Int(nullable: false),
                        Language = c.String(),
                        LanguageLevel = c.Int(nullable: false),
                        Semester = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ErasmusUserId, cascadeDelete: true)
                .ForeignKey("dbo.Faculties", t => t.SourceFacultyId)
                .ForeignKey("dbo.FieldOfStudies", t => t.SourceFieldOfStudyId)
                .ForeignKey("dbo.Universities", t => t.SourceUniversityId)
                .ForeignKey("dbo.Faculties", t => t.TargetFacultyId)
                .ForeignKey("dbo.FieldOfStudies", t => t.TargetFieldOfStudyId)
                .ForeignKey("dbo.Universities", t => t.TargetUniversityId)
                .Index(t => t.ErasmusUserId)
                .Index(t => t.SourceUniversityId)
                .Index(t => t.TargetUniversityId)
                .Index(t => t.SourceFacultyId)
                .Index(t => t.TargetFacultyId)
                .Index(t => t.SourceFieldOfStudyId)
                .Index(t => t.TargetFieldOfStudyId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        StudentId = c.String(),
                        PersonalIdCode = c.String(),
                        Birthday = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        UniversityId = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Universities", t => t.UniversityId)
                .Index(t => t.UniversityId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Universities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ErasmusCode = c.String(),
                        Country = c.String(),
                        City = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Faculties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UniversityId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Universities", t => t.UniversityId)
                .Index(t => t.UniversityId);
            
            CreateTable(
                "dbo.FieldOfStudies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Degree = c.Int(nullable: false),
                        FacultyId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculties", t => t.FacultyId)
                .Index(t => t.FacultyId);
            
            CreateTable(
                "dbo.StudySubjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Language = c.String(),
                        Evaluation = c.String(),
                        Credits = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Semester = c.Int(nullable: false),
                        FieldOfStudyId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FieldOfStudies", t => t.FieldOfStudyId)
                .Index(t => t.FieldOfStudyId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UniversityAgreements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceUniversityId = c.Int(nullable: false),
                        TargetUniversityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Universities", t => t.SourceUniversityId)
                .ForeignKey("dbo.Universities", t => t.TargetUniversityId)
                .Index(t => t.SourceUniversityId)
                .Index(t => t.TargetUniversityId);
            
            CreateTable(
                "dbo.SourceAgreementSubjects",
                c => new
                    {
                        AgreementId = c.Int(nullable: false),
                        SubjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AgreementId, t.SubjectId })
                .ForeignKey("dbo.Agreements", t => t.AgreementId, cascadeDelete: true)
                .ForeignKey("dbo.StudySubjects", t => t.SubjectId, cascadeDelete: true)
                .Index(t => t.AgreementId)
                .Index(t => t.SubjectId);
            
            CreateTable(
                "dbo.TargetAgreementSubjects",
                c => new
                    {
                        AgreementId = c.Int(nullable: false),
                        SubjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AgreementId, t.SubjectId })
                .ForeignKey("dbo.Agreements", t => t.AgreementId, cascadeDelete: true)
                .ForeignKey("dbo.StudySubjects", t => t.SubjectId, cascadeDelete: true)
                .Index(t => t.AgreementId)
                .Index(t => t.SubjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UniversityAgreements", "TargetUniversityId", "dbo.Universities");
            DropForeignKey("dbo.UniversityAgreements", "SourceUniversityId", "dbo.Universities");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Agreements", "TargetUniversityId", "dbo.Universities");
            DropForeignKey("dbo.TargetAgreementSubjects", "SubjectId", "dbo.StudySubjects");
            DropForeignKey("dbo.TargetAgreementSubjects", "AgreementId", "dbo.Agreements");
            DropForeignKey("dbo.Agreements", "TargetFieldOfStudyId", "dbo.FieldOfStudies");
            DropForeignKey("dbo.Agreements", "TargetFacultyId", "dbo.Faculties");
            DropForeignKey("dbo.Agreements", "SourceUniversityId", "dbo.Universities");
            DropForeignKey("dbo.SourceAgreementSubjects", "SubjectId", "dbo.StudySubjects");
            DropForeignKey("dbo.SourceAgreementSubjects", "AgreementId", "dbo.Agreements");
            DropForeignKey("dbo.Agreements", "SourceFieldOfStudyId", "dbo.FieldOfStudies");
            DropForeignKey("dbo.Agreements", "SourceFacultyId", "dbo.Faculties");
            DropForeignKey("dbo.Agreements", "ErasmusUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "UniversityId", "dbo.Universities");
            DropForeignKey("dbo.Faculties", "UniversityId", "dbo.Universities");
            DropForeignKey("dbo.StudySubjects", "FieldOfStudyId", "dbo.FieldOfStudies");
            DropForeignKey("dbo.FieldOfStudies", "FacultyId", "dbo.Faculties");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.TargetAgreementSubjects", new[] { "SubjectId" });
            DropIndex("dbo.TargetAgreementSubjects", new[] { "AgreementId" });
            DropIndex("dbo.SourceAgreementSubjects", new[] { "SubjectId" });
            DropIndex("dbo.SourceAgreementSubjects", new[] { "AgreementId" });
            DropIndex("dbo.UniversityAgreements", new[] { "TargetUniversityId" });
            DropIndex("dbo.UniversityAgreements", new[] { "SourceUniversityId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.StudySubjects", new[] { "FieldOfStudyId" });
            DropIndex("dbo.FieldOfStudies", new[] { "FacultyId" });
            DropIndex("dbo.Faculties", new[] { "UniversityId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "UniversityId" });
            DropIndex("dbo.Agreements", new[] { "TargetFieldOfStudyId" });
            DropIndex("dbo.Agreements", new[] { "SourceFieldOfStudyId" });
            DropIndex("dbo.Agreements", new[] { "TargetFacultyId" });
            DropIndex("dbo.Agreements", new[] { "SourceFacultyId" });
            DropIndex("dbo.Agreements", new[] { "TargetUniversityId" });
            DropIndex("dbo.Agreements", new[] { "SourceUniversityId" });
            DropIndex("dbo.Agreements", new[] { "ErasmusUserId" });
            DropTable("dbo.TargetAgreementSubjects");
            DropTable("dbo.SourceAgreementSubjects");
            DropTable("dbo.UniversityAgreements");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.StudySubjects");
            DropTable("dbo.FieldOfStudies");
            DropTable("dbo.Faculties");
            DropTable("dbo.Universities");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Agreements");
        }
    }
}
