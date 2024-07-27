using Habanerio.Core.DBs.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Habanerio.Core.DBs.MongoDB.EFCore;

public class MongoDbContext : DbContextBase
{
    //private IMongoDatabase? _mongoDb;

    public MongoDbContext(DbContextOptions options) : base(options) { }

    public MongoDbContext(IOptions<MongoDbSettings> options) : base(GetDbContextOptions(options)) { }

    //public IMongoDatabase MongoDb
    //{
    //    get
    //    {
    //        if (_mongoDb is null)
    //        {
    //            var dbConnection = Database.GetDbConnection().Database;
    //            _mongoDb = new MongoClient(Database.GetConnectionString())
    //                .GetDatabase(dbConnection);
    //        }

    //        return _mongoDb;
    //    }
    //}

    private static DbContextOptions GetDbContextOptions(IOptions<MongoDbSettings> options)
    {
        var mongoOptions = options.Value;

        var connectionString = mongoOptions.ConnectionString;
        var databaseName = mongoOptions.DatabaseName;
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();

        dbContextOptionsBuilder.UseMongoDB(connectionString, databaseName);

        if (mongoOptions.EnableSensitiveDataLogging)
            dbContextOptionsBuilder.EnableSensitiveDataLogging();

        if (mongoOptions.EnableDetailedErrors)
            dbContextOptionsBuilder.EnableDetailedErrors();

        return dbContextOptionsBuilder.Options;
    }

    //TODO: Get the underlying MongoDb database
    //public IMongoDatabase? MongoDb
    //{
    //    get
    //    {
    //        if (_mongoDb == null)
    //        {
    //            var database = Database.GetDbConnection().Database;
    //            _mongoDb = new MongoClient(Database.GetConnectionString())
    //                .GetDatabase(database);
    //        }

    //        return _mongoDb;
    //    }
    //}


    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseMongoDB(_client, _dbName)
    //            .EnableSensitiveDataLogging()
    //            .EnableDetailedErrors();

    //    }
    //}
}
