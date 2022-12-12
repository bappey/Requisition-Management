namespace Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OtherRequistionDetailsNews", "CategoryID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OtherRequistionDetailsNews", "CategoryID");
        }
    }
}
