namespace PressFitApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        SearchTags = c.String(nullable: false),
                        CreatedDate = c.String(),
                        ModifiedDate = c.String(),
                        UpdatedBy = c.String(),
                        FileName = c.String(nullable: false),
                        ImageUrl = c.String(),
                        PdfUrl = c.String(),
                        HighPriority = c.Boolean(nullable: false),
                        PriorityNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Token",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.String(),
                        TokenId = c.String(),
                        ChannelId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VersionModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IOSVersion = c.String(nullable: false),
                        AndriodVersion = c.String(nullable: false),
                        ForceFulAndriodUpdate = c.Boolean(nullable: false),
                        ForceFulIOSUpdate = c.Boolean(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VersionModel");
            DropTable("dbo.Token");
            DropTable("dbo.Product");
        }
    }
}
