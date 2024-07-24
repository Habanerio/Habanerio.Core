using System.ComponentModel.DataAnnotations;
using Habanerio.Core.DBs.MongoDB.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Habanerio.Tests.Integration.Core.Dbs.MongoDB.EFCore.Data;

public class BlogPostEntity : MongoDbEntity
{
    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Content { get; set; }

    [Required]
    public required string Author { get; set; }

    public List<string> Tags { get; set; }

    public ICollection<BlogPostCommentEntity> Comments { get; set; } = [];

    [Required]
    public required DateTimeOffset DateCreated { get; set; }

    public DateTimeOffset? DateUpdated { get; set; }

    public DateTimeOffset? DatePublished { get; set; }

    public bool IsDeleted { get; set; }
}

[Owned]
public class BlogPostCommentEntity : MongoDbEntity
{
    //public ObjectId BlogPostId { get; set; }
    [Required]
    public required string Content { get; set; }
    [Required]
    public required string Author { get; set; }
    [Required]
    public required DateTimeOffset DatePosted { get; set; }
}