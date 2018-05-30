namespace ErasmusPlus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgreementReasonAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Agreements", "DeclineReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Agreements", "DeclineReason");
        }
    }
}
