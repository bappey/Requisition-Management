namespace Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test2 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RequestionFileModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RequestionFileModels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Serial = c.String(),
                        RequestionNumber = c.String(),
                        ImageId = c.Int(),
                    })
                .PrimaryKey(t => t.id);
            
        }
    }
}
