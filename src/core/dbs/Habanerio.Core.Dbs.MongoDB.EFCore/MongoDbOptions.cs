namespace Habanerio.Core.DBs.MongoDB.EFCore;
public class MongoDbOptions : DbOptions
{
    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    /// <value>
    /// The name of the database.
    /// </value>
    public virtual string DatabaseName { get; set; } = "";
}
