using Habanerio.Core.DBs.MongoDB.EFCore;
using Habanerio.Samples.Customers.MongoEFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habanerio.Samples.Customers.MongoEFCore;

public class SampleMongoDbContext : MongoDbContext
{
    /// <summary>
    /// Uses the specified options and the default database name.
    /// </summary>
    /// <param name="options"></param>
    public SampleMongoDbContext(DbContextOptions options) : base(options) { }

    //public SampleMongoDbContext(IOptions<MongoDbOptions> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerDbEntity>().HasIndex(e => e.Id);
        modelBuilder.Entity<CustomerDbEntity>(e =>
        {
            //e.Property(e => e.Id).HasDefaultValue(ObjectId.GenerateNewId());
            e.Property(e => e.FirstName).IsRequired();
            e.Property(e => e.FirstName).IsRequired();
            e.Property(e => e.LastName).IsRequired();
        });
        //.ToCollection("customers");

        //modelBuilder.Entity<CustomerDbEntity>().HasData(
        //        new CustomerDbEntity()
        //        {
        //            FirstName = "John",
        //            LastName = "Doe",
        //            Email = "jdoe@samples.com"
        //        });
        //modelBuilder.Entity<CustomerDbEntity>().HasData(
        //        new CustomerDbEntity()
        //        {
        //            FirstName = "Jane",
        //            LastName = "Doe",
        //            Email = "janed@sample.com"
        //        });
        //modelBuilder.Entity<CustomerDbEntity>().HasData(
        //        new CustomerDbEntity()
        //        {
        //            FirstName = "John",
        //            LastName = "Smith",
        //            Email = "jsmith@sample.com"
        //        });


    }
}