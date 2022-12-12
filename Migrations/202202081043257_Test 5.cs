namespace Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestionFileModels", "DateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.RequestionFileModels", "UploadBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RequestionFileModels", "UploadBy");
            DropColumn("dbo.RequestionFileModels", "DateTime");
        }
    }
}
