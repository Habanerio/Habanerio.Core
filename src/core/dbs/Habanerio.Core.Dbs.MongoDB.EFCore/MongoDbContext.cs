using Habanerio.Core.DBs.EFCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Habanerio.Core.DBs.MongoDB.EFCore;

public class MongoDbContext : DbContextBase
{
    private readonly MongoDbClient _client;
    private readonly string _dbName;

    private IMongoDatabase? _mongoDb;

    //public MongoDbContext(IOptions<MongoDbOptions> options)
    //{
    //    _client = new MongoDbClient(options);
    //    _dbName = options.Value.DatabaseName;
    //}

    public MongoDbContext(DbContextOptions options) : base(options)
    {

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