namespace ErasmusPlus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPermissions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        UniversityId = c.Int(nullable: false),
                        FacultyId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculties", t => t.FacultyId)
                .ForeignKey("dbo.Universities", t => t.UniversityId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.UniversityId)
                .Index(t => t.FacultyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPermissions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPermissions", "UniversityId", "dbo.Universities");
            DropForeignKey("dbo.UserPermissions", "FacultyId", "dbo.Faculties");
            DropIndex("dbo.UserPermissions", new[] { "FacultyId" });
            DropIndex("dbo.UserPermissions", new[] { "UniversityId" });
            DropIndex("dbo.UserPermissions", new[] { "UserId" });
            DropTable("dbo.UserPermissions");
        }
    }
}
