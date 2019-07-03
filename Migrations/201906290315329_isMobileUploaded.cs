namespace PressFitApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isMobileUploaded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "IsMobileUploaded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "IsMobileUploaded");
        }
    }
}
