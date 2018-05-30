namespace ErasmusPlus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniIds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FieldOfStudies", "UniversityId", c => c.Int());
            AddColumn("dbo.StudySubjects", "UniversityId", c => c.Int());
            CreateIndex("dbo.FieldOfStudies", "UniversityId");
            CreateIndex("dbo.StudySubjects", "UniversityId");
            AddForeignKey("dbo.StudySubjects", "UniversityId", "dbo.Universities", "Id");
            AddForeignKey("dbo.FieldOfStudies", "UniversityId", "dbo.Universities", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FieldOfStudies", "UniversityId", "dbo.Universities");
            DropForeignKey("dbo.StudySubjects", "UniversityId", "dbo.Universities");
            DropIndex("dbo.StudySubjects", new[] { "UniversityId" });
            DropIndex("dbo.FieldOfStudies", new[] { "UniversityId" });
            DropColumn("dbo.StudySubjects", "UniversityId");
            DropColumn("dbo.FieldOfStudies", "UniversityId");
        }
    }
}
