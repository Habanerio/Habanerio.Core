using Habanerio.Core.DBs.MongoDB.EFCore;

namespace Habanerio.Samples.Customers.MongoEFCore;

public class SampleMongoDbOptions : MongoDbOptions
{
    public override string DatabaseName { get; set; } = "samples";

}