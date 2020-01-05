namespace PressFitApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactUs1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContactUs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        MobileNo = c.String(),
                        State = c.String(),
                        City = c.String(),
                        Message = c.String(),
                        Subject = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ContactUs");
        }
    }
}
