namespace Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BranchReqSeqAssigns", "BranchID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BranchReqSeqAssigns", "BranchID", c => c.Int(nullable: false));
        }
    }
}
