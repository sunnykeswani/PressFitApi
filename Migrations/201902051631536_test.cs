namespace PressFitApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubjectModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Subject");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Subject",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubjectLine = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.SubjectModel");
        }
    }
}
