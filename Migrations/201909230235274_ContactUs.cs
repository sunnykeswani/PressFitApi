namespace PressFitApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactUs : DbMigration
    {
        public override void Up()
        {
           // AddColumn("dbo.SubjectModel", "EmailId", c => c.String(nullable: false));
            AlterColumn("dbo.SubjectModel", "Subject", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SubjectModel", "Subject", c => c.String());
            DropColumn("dbo.SubjectModel", "EmailId");
        }
    }
}
