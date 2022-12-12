namespace Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequestionFileModels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        RequestionNumber = c.Int(nullable: false),
                        Extention = c.String(),
                        FileSize = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RequestionFileModels");
        }
    }
}
