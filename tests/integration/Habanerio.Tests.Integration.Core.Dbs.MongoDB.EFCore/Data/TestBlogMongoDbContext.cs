using Habanerio.Core.DBs.MongoDB.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Habanerio.Tests.Integration.Core.Dbs.MongoDB.EFCore.Data;

public class TestBlogMongoDbContext : MongoDbContext
{
    public TestBlogMongoDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<BlogPostEntity>().HasIndex(e => e.Id);
        modelBuilder.Entity<BlogPostEntity>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Title).IsRequired();
            e.Property(p => p.Author).IsRequired();
            e.Property(p => p.Content).IsRequired();
            e.Property(p => p.DateCreated).IsRequired();
            e.OwnsMany(p => p.Comments);
        });


        //modelBuilder.Entity<BlogPostCommentEntity>().HasIndex(e => e.Id);
        //modelBuilder.Entity<BlogPostCommentEntity>(e =>
        //{
        //    e.Property(p => p.Id).IsRequired();
        //    e.Property(p => p.Content).IsRequired();
        //    e.Property(p => p.Author).IsRequired();
        //    e.Property(p => p.DatePosted).IsRequired();
        //});


        //modelBuilder.Entity<BlogPostEntity>().HasData(
        //    new BlogPostEntity()
        //    {
        //        Title = "Blog Post 1",
        //        Author = "John Doe",
        //        Content = "This is some content",
        //        DateCreated = DateTime.Now.AddDays(-12),
        //        Comments = new Collection<BlogPostCommentEntity>
        //        {
        //            new BlogPostCommentEntity()
        //            {
        //                Author = "sdfg sdfg sdfg s",
        //                Content = "sdfgsdf gsdf gsd",
        //                DatePosted = DateTimeOffset.Now.AddDays(-11)
        //            },
        //            new BlogPostCommentEntity()
        //            {
        //                Author = "sdfg sdfg sdfg s",
        //                Content = "sdfgsdf gsdf gsd",
        //                DatePosted = DateTimeOffset.Now.AddDays(-10)
        //            },
        //            new BlogPostCommentEntity()
        //            {
        //                Author = "sdfg sdfg sdfg s",
        //                Content = "sdfgsdf gsdf gsd",
        //                DatePosted = DateTimeOffset.Now.AddDays(-10)
        //            },
        //            new BlogPostCommentEntity()
        //            {
        //                Author = "sdfg sdfg sdfg s",
        //                Content = "sdfgsdf gsdf gsd",
        //                DatePosted = DateTimeOffset.Now.AddDays(-10)
        //            },
        //        }
        //    },
        //    new BlogPostEntity()
        //    {
        //        Title = "Blog Post 2",
        //        Author = "John Doe",
        //        Content = "This is some content",
        //        DateCreated = DateTime.Now.AddDays(-2)
        //    },
        //    new BlogPostEntity()
        //    {
        //        Title = "Blog Post 3",
        //        Author = "John Doe",
        //        Content = "This is some content",
        //        DateCreated = DateTime.Now.AddDays(-4)
        //    },
        //    new BlogPostEntity()
        //    {
        //        Title = "Blog Post 4",
        //        Author = "John Doe",
        //        Content = "This is some content",
        //        DateCreated = DateTime.Now.AddDays(-6)
        //    },
        //    new BlogPostEntity()
        //    {
        //        Title = "Blog Post 5",
        //        Author = "John Doe",
        //        Content = "This is some content",
        //        DateCreated = DateTime.Now.AddDays(-9)
        //    });

    }
}