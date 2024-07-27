using Habanerio.Core.DBs.MongoDB.EFCore;

namespace Habanerio.Samples.Customers.MongoEFCore;

public sealed class SampleMongoDbSettings : MongoDbSettings
{
    public override string DatabaseName { get; set; } = "samples";

    public override bool EnableSensitiveDataLogging { get; set; } = true;

    public override bool EnableDetailedErrors { get; set; } = true;

}