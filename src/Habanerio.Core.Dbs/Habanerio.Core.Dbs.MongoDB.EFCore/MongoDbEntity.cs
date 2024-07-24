using System.ComponentModel.DataAnnotations;
using Habanerio.Core.DBs.MongoDB.EFCore.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Core.DBs.MongoDB.EFCore;

public class MongoDbEntity : IMongoDbEntity // DbEntity<ObjectId>, IMongoDbEntity
{
    [BsonId]
    [Key]
    public ObjectId Id { get; set; }

    public MongoDbEntity()
    {
        Id = ObjectId.GenerateNewId();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((MongoDbEntity)obj);
    }

    protected bool Equals(MongoDbEntity other)
    {
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}