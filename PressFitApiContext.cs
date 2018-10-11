using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

public class PressFitApiContext : DbContext
{
    // You can add custom code to this file. Changes will not be overwritten.
    // 
    // If you want Entity Framework to drop and regenerate your database
    // automatically whenever you change your model schema, please use data migrations.
    // For more information refer to the documentation:
    // http://msdn.microsoft.com/en-us/data/jj591621.aspx

    public PressFitApiContext() : base("name=PressFitApiContext")
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        Database.SetInitializer<PressFitApiContext>(null);
        base.OnModelCreating(modelBuilder);
        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        //modelBuilder.Entity<Blog>()
        //    .Ignore(b => b.LoadedFromDatabase);
    }

    public System.Data.Entity.DbSet<PressFitApi.Models.Product> Product { get; set; }

    public System.Data.Entity.DbSet<PressFitApi.Models.Token> Token { get; set; }

    //public System.Data.Entity.DbSet<PressFitApi.Models.Token> TokenModels { get; set; }
}
