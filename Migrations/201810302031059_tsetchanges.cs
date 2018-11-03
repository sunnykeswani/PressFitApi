namespace PressFitApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tsetchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "PriorityNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "PriorityNumber");
        }
    }
}
