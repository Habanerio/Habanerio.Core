using System.Collections.ObjectModel;
using Habanerio.Tests.Integration.Core.Dbs.MongoDB.EFCore.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Testcontainers.MongoDb;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Habanerio.Tests.Integration.Core.Dbs.MongoDB.EFCore;

/// <summary>
/// Test Fixture that can be reused globally for all tests in this assembly
/// </summary>
public class MongoDbContainerFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container = new MongoDbBuilder()
        .WithImage("mongo:6.0")
    .Build();

    public string ConnectionString => _container.GetConnectionString();
    public string ContainerId => $"{_container.Id}";

    public virtual async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public virtual Task DisposeAsync()
        => _container.DisposeAsync().AsTask();
}

/// <summary>
/// Test Fixture that is specific to the Test Blog Context
/// </summary>
public class MongoDbContainerBlogFixture : MongoDbContainerFixture
{
    private TestBlogMongoDbContext? _dbContext;
    public TestBlogMongoDbContext DbContext => _dbContext;


    private DbContextOptions<TestBlogMongoDbContext>? _dbOptions;
    public DbContextOptions<TestBlogMongoDbContext> DbOptions => _dbOptions;

    private ITestBlogMongoDbRepository _blogMongoDbRepository;

    public List<ObjectId> BlogIds { get; } = new();

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _dbOptions = new DbContextOptionsBuilder<TestBlogMongoDbContext>()
            .UseMongoDB(ConnectionString, "test-blog")
            .Options;

        _dbContext = new TestBlogMongoDbContext(_dbOptions);
        _blogMongoDbRepository = new TestBlogMongoDbMongoDbRepository(_dbContext);

        //var collection = _blogMongoDbRepository.GetCollection();

        PopulateData();

        Thread.Sleep(500);
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }

    private void PopulateData()
    {
        // If the db already exists, delete it and reseed it
        DbContext.Database.EnsureDeleted();

        var rdm = new Random();

        var authors = new List<string> { "John Doe", "Jane Doe", "John Smith", "Jane Smith", "John Johnson", "Jane Johnson" };


        for (var i = 1; i < 100; i++)
        {
            var comments = new Collection<BlogPostCommentEntity>();

            for (var j = 1; j < new Random().Next(1, 10); j++)
            {
                comments.Add(new BlogPostCommentEntity
                {
                    Author = authors[rdm.Next(0, authors.Count - 1)],
                    Content = $"Blog {i} - Comment {j}",
                    DatePosted = DateTime.Now.AddDays(-j),
                });
            }

            var blog = new BlogPostEntity()
            {
                Title = $"Blog {i}",
                Author = authors[rdm.Next(0, authors.Count - 1)],
                Content = $"Content for Blog {i}",
                DateCreated = DateTime.Now.AddDays(-i),
                DatePublished = DateTime.Now.AddDays(-i),
                DateUpdated = DateTime.Now.AddDays(-i),
                Tags = new List<string> { $"test{i % 2}", $"blog{i % 3}", $"blog{i % 5}" },
                Comments = comments
            };

            _blogMongoDbRepository.Add(blog);
            _blogMongoDbRepository.SaveChanges();

            BlogIds.Add(blog.Id);
        }
    }
}

public class MongoDbBlogFixture : MongoDbContainerFixture
{
    private TestBlogMongoDbContext? _dbContext;
    public TestBlogMongoDbContext DbContext => _dbContext;


    private DbContextOptions<TestBlogMongoDbContext>? _dbOptions;
    public DbContextOptions<TestBlogMongoDbContext> DbOptions => _dbOptions;

    public List<ObjectId> BlogIds { get; } = new();

    private ITestBlogMongoDbRepository _blogMongoDbRepository;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _dbOptions = new DbContextOptionsBuilder<TestBlogMongoDbContext>()
            .UseMongoDB("mongodb://mongo:g?7<Vd>9v4;ZKk=J@localhost:27018", "test")
            .EnableSensitiveDataLogging()
            .Options;

        _dbContext = new TestBlogMongoDbContext(_dbOptions);
        _blogMongoDbRepository = new TestBlogMongoDbMongoDbRepository(_dbContext);
        //_dbContext.BlogPosts =  _dbContext.Set<BlogPostEntity>();

        PopulateData();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }

    private void PopulateData()
    {
        // If the db already exists, delete it and reseed it
        DbContext.Database.EnsureDeleted();

        var rdm = new Random();

        var authors = new List<string> { "John Doe", "Jane Doe", "John Smith", "Jane Smith", "John Johnson", "Jane Johnson" };


        for (var i = 1; i < 100; i++)
        {
            var comments = new Collection<BlogPostCommentEntity>();

            for (var j = 1; j < new Random().Next(1, 10); j++)
            {
                comments.Add(new BlogPostCommentEntity
                {
                    Author = authors[rdm.Next(0, authors.Count - 1)],
                    Content = $"Blog {i} - Comment {j}",
                    DatePosted = DateTime.Now.AddDays(-j),
                });
            }

            var blog = new BlogPostEntity()
            {
                Title = $"Blog {i}",
                Author = authors[rdm.Next(0, authors.Count - 1)],
                Content = $"Content for Blog {i}",
                DateCreated = DateTime.Now.AddDays(-i),
                DatePublished = DateTime.Now.AddDays(-i),
                DateUpdated = DateTime.Now.AddDays(-i),
                Tags = new List<string> { $"test{i % 2}", $"blog{i % 3}", $"blog{i % 5}" },
                Comments = comments
            };

            _blogMongoDbRepository.Add(blog);
            _blogMongoDbRepository.SaveChanges();

            BlogIds.Add(blog.Id);
        }
    }
}


public class MongoDbRepositoryTests : IClassFixture<MongoDbContainerBlogFixture>, IDisposable
{

    private readonly MongoDbContainerBlogFixture _fixture;
    private readonly ITestOutputHelper _output;

    private readonly ITestBlogMongoDbRepository _mongoDbRepository;

    public MongoDbRepositoryTests(MongoDbContainerBlogFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;

        _mongoDbRepository = new TestBlogMongoDbMongoDbRepository(_fixture.DbContext);
    }

    [Fact]
    public void CanConstruct_MongoDbContext()
    {
        var context = new TestBlogMongoDbContext(_fixture.DbOptions);

        Assert.NotNull(context);
    }

    [Fact]
    public void CanConstruct_MongoDbRepository()
    {
        var repository = new TestBlogMongoDbMongoDbRepository(_fixture.DbContext);

        Assert.NotNull(repository);
    }

    [Fact]
    public void CanCall_Exists_ById()
    {
        var blogId = _fixture.BlogIds.First();

        var exists = _mongoDbRepository.Exists(blogId);

        Assert.True(exists);
    }

    [Fact]
    public void CanCall_Exists_ById_False()
    {
        var blogId = ObjectId.GenerateNewId();

        var exists = _mongoDbRepository.Exists(blogId);

        Assert.False(exists);
    }

    [Fact]
    public void CanNotCall_Exists_ById_Empty_Id_Throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _mongoDbRepository.Exists(ObjectId.Empty));
    }


    [Fact]
    public async Task CanCall_ExistsAsync_ById()
    {
        var blogId = _fixture.BlogIds.First();

        var exists = await _mongoDbRepository.ExistsAsync(blogId);

        Assert.True(exists);
    }

    [Fact]
    public async Task CanCall_ExistsAsync_ById_False()
    {
        var blogId = ObjectId.GenerateNewId();

        var exists = await _mongoDbRepository.ExistsAsync(blogId);

        Assert.False(exists);
    }

    [Fact]
    public async Task CanNotCall_ExistsAsync_ById_Empty_Id_Throws_ArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => await _mongoDbRepository.ExistsAsync(ObjectId.Empty));
    }

    [Fact]
    public async Task CanCall_FilterAsync()
    {
        var term = "Blog 1";
        var pageNo = 1;
        var pageSize = 25;
        var results = await _mongoDbRepository.SearchAsync(term, "", "", pageNo, pageSize);

        Assert.NotNull(results);
        Assert.NotEmpty(results.Items);
        Assert.Equal(1, results.TotalPages);
        Assert.Equal(1, results.TotalCount);

        Assert.True(results.Items.All(p => p.Title.Contains(term, StringComparison.CurrentCultureIgnoreCase)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99)]
    public async Task CanNotCall_FilterAsync_WithInvalid_PageNo_Throws_ArgumentOutOfRangeException(int pageNo)
    {
        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _mongoDbRepository.SearchAsync("Blog 1", "", "", pageNo, 25));

        Assert.True(ex.Message.Contains("Parameter 'pageNo'"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99)]
    public async Task CanNotCall_FilterAsync_WithInvalid_PageSizeNo_Throws_ArgumentOutOfRangeException(int pageSize)
    {
        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _mongoDbRepository.SearchAsync("Blog 1", "", "", 1, pageSize));

        Assert.True(ex.Message.Contains("Parameter 'pageSize'"));
    }

    [Fact]
    public void CanCall_Find_ById()
    {
        var blogId = _fixture.BlogIds.First();

        var post = _mongoDbRepository.Find(blogId);

        Assert.NotNull(post);
        Assert.Equal(blogId, post.Id);
        Assert.Equal("Blog 1", post.Title);

        Assert.NotNull(post.Comments);
        Assert.NotEmpty(post.Comments);
    }

    [Fact]
    public void CanNotCall_Find_ById_Empty_Id_Throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _mongoDbRepository.Find(ObjectId.Empty));
    }


    [Fact]
    public async Task CanCall_FindAsync_ById()
    {
        var blogId = _fixture.BlogIds.Skip(1).Take(1).FirstOrDefault();

        var post = await _mongoDbRepository.FindAsync(blogId);

        Assert.NotNull(post);
        Assert.Equal(blogId, post.Id);
        Assert.Equal("Blog 2", post.Title);

        Assert.NotNull(post.Comments);
        Assert.NotEmpty(post.Comments);
    }

    [Fact]
    public async Task CanNotCall_FindAsync_ById_Empty_Id_Throws_ArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => await _mongoDbRepository.FindAsync(ObjectId.Empty));
    }


    [Fact]
    public void CanCall_Find_ByIds()
    {
        var blogIds = _fixture.BlogIds.Take(5).ToList();

        var posts = _mongoDbRepository.Find(blogIds.AsEnumerable()).ToList();

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);

        for (var i = 1; i <= 5; i++)
        {
            var blogId = blogIds[i - 1];
            var post = posts.FirstOrDefault(x => x.Id == blogId);

            Assert.NotNull(post);
            Assert.Equal(blogId, post.Id);
            Assert.Equal($"Blog {i}", post.Title);

            Assert.NotNull(post.Comments);
            Assert.NotEmpty(post.Comments);
        }
    }

    [Fact]
    public async Task CanCall_FindAsync_ByIds()
    {
        var blogIds = _fixture.BlogIds.Take(5).ToList();

        var posts = (await _mongoDbRepository.FindAsync(blogIds)).ToList();

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);

        for (var i = 1; i <= 5; i++)
        {
            var blogId = blogIds[i - 1];
            var post = posts.FirstOrDefault(x => x.Id == blogId);

            Assert.NotNull(post);
            Assert.Equal(blogId, post.Id);
            Assert.Equal($"Blog {i}", post.Title);

            Assert.NotNull(post.Comments);

            try
            {
                Assert.NotEmpty(post.Comments);
            }
            catch (NotEmptyException e)
            {
                _output.WriteLine(e.ToString());
                throw;
            }
        }
    }

    [Fact]
    public async Task CanNotCall_FindAsync_ByIds_EmptyCollection_Throws_Exception()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => await _mongoDbRepository.FindAsync(new List<ObjectId>()));
    }

    [Fact]
    public async Task CanNotCall_FindAsync_ByIds_CollectionContains_EmptyId_Throws_Exception()
    {
        var blogIds = new List<ObjectId>()
        {
            ObjectId.GenerateNewId(), ObjectId.Empty, ObjectId.GenerateNewId(), ObjectId.GenerateNewId()
        };

        await Assert.ThrowsAsync<ArgumentException>(async () => await _mongoDbRepository.FindAsync(blogIds));
    }


    [Fact]
    public void CanCall_FirstOrDefault()
    {
        var post = _mongoDbRepository.FirstOrDefault(x => x.Title.Contains("Blog 1", StringComparison.CurrentCultureIgnoreCase));

        Assert.NotNull(post);
        Assert.Equal("Blog 1", post.Title);
    }

    [Fact]
    public void CanCall_FirstOrDefault_Returns_Null()
    {
        var post = _mongoDbRepository.FirstOrDefault(x => x.Title == "Blog -1");

        Assert.Null(post);
    }


    [Fact]
    public async Task CanCall_FirstOrDefaultAsync()
    {

        var post = await _mongoDbRepository.FirstOrDefaultAsync(x => x.Title == "Blog 1");

        Assert.NotNull(post);
        Assert.Equal("Blog 1", post.Title);
    }

    [Fact]
    public async Task CanCall_FirstOrDefaultAsync_DoesNotExist_Returns_Null()
    {
        var post = await _mongoDbRepository.FirstOrDefaultAsync(x => x.Title == "Blog -1");

        Assert.Null(post);
    }


    [Fact]
    public void CanCall_Add()
    {
        var blog = new BlogPostEntity()
        {
            Title = "New Blog 1000",
            Author = Guid.NewGuid().ToString(),
            Content = "Content for New Blog",
            DateCreated = DateTime.Now,
            DatePublished = DateTime.Now,
            DateUpdated = DateTime.Now,
            Tags = ["test", "blog", "new"],
            Comments =
            [
                new BlogPostCommentEntity
                {
                    Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                }
            ]
        };

        _mongoDbRepository.Add(blog);

        var exists = _mongoDbRepository.Exists(blog.Id);

        Assert.True(exists);
    }

    [Fact]
    public void CanNotCall_Add_Null_Throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _mongoDbRepository.Add(default));
    }

    [Fact]
    public void CanCall_AddRange()
    {
        var posts = new List<BlogPostEntity>()
        {
            new BlogPostEntity()
            {
                Title = "New Blog 1001",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            },
            new BlogPostEntity()
            {
                Title = "New Blog 1002",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            },
            new BlogPostEntity()
            {
                Title = "New Blog 1003",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            }
        };

        _mongoDbRepository.AddRange(posts);

        foreach (var expected in posts)
        {
            var actual = _mongoDbRepository.Find(expected.Id);

            Assert.NotNull(actual);
            Assert.Equal(expected.Title, actual.Title);
        }
    }

    [Fact]
    public void CanCall_AddRange_EmptyCollection_Throws_Exception()
    {
        Assert.Throws<ArgumentException>(() => _mongoDbRepository.AddRange(new List<BlogPostEntity>()));
    }

    [Fact]
    public async Task CanCall_AddRangeAsync()
    {
        var posts = new List<BlogPostEntity>()
        {
            new BlogPostEntity()
            {
                Title = "New Blog 2001",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            },
            new BlogPostEntity()
            {
                Title = "New Blog 2002",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            },
            new BlogPostEntity()
            {
                Title = "New Blog 2003",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            }
        };

        await _mongoDbRepository.AddRangeAsync(posts);

        foreach (var expected in posts)
        {
            var actual = await _mongoDbRepository.FindAsync(expected.Id);

            Assert.NotNull(actual);
            Assert.Equal(expected.Title, actual.Title);
        }
    }

    [Fact]
    public async Task CanCall_AddRangeAsync_EmptyCollection_Throws_Exception()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => await _mongoDbRepository.AddRangeAsync(new List<BlogPostEntity>()));
    }

    [Fact]
    public void CanCall_Remove()
    {
        var blog = new BlogPostEntity()
        {
            Title = "New Blog 3001",
            Author = Guid.NewGuid().ToString(),
            Content = "Content for New Blog",
            DateCreated = DateTime.Now,
            DatePublished = DateTime.Now,
            DateUpdated = DateTime.Now,
            Tags = ["test", "blog", "new"],
            Comments =
            [
                new BlogPostCommentEntity
                {
                    Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                }
            ]
        };

        _mongoDbRepository.Add(blog);
        _mongoDbRepository.SaveChanges();

        var exists = _mongoDbRepository.Exists(blog.Id);

        Assert.True(exists);

        _mongoDbRepository.Remove(blog);
        _mongoDbRepository.SaveChanges();

        exists = _mongoDbRepository.Exists(blog.Id);

        Assert.False(exists);
    }

    [Fact]
    public void CanCall_Remove_ById()
    {
        var blog = new BlogPostEntity()
        {
            Title = "New Blog 3001",
            Author = Guid.NewGuid().ToString(),
            Content = "Content for New Blog",
            DateCreated = DateTime.Now,
            DatePublished = DateTime.Now,
            DateUpdated = DateTime.Now,
            Tags = ["test", "blog", "new"],
            Comments =
            [
                new BlogPostCommentEntity
                {
                    Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                }
            ]
        };

        _mongoDbRepository.Add(blog);
        _mongoDbRepository.SaveChanges();

        var exists = _mongoDbRepository.Exists(blog.Id);

        Assert.True(exists);

        _mongoDbRepository.Remove(blog.Id);
        _mongoDbRepository.SaveChanges();

        exists = _mongoDbRepository.Exists(blog.Id);

        Assert.False(exists);
    }

    [Fact]
    public async Task CanCall_RemoveAsync()
    {
        await Assert.ThrowsAsync<NotImplementedException>(async () => await _mongoDbRepository.RemoveAsync(ObjectId.Empty));
    }

    [Fact]
    public void CanCall_RemoveRange()
    {
        var posts = new List<BlogPostEntity>()
        {
            new BlogPostEntity()
            {
                Title = "New Blog 4001",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            },
            new BlogPostEntity()
            {
                Title = "New Blog 4002",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            },
            new BlogPostEntity()
            {
                Title = "New Blog 4003",
                Author = Guid.NewGuid().ToString(),
                Content = "Content for New Blog",
                DateCreated = DateTime.Now,
                DatePublished = DateTime.Now,
                DateUpdated = DateTime.Now,
                Tags = ["test", "blog", "new"],
                Comments =
                [
                    new BlogPostCommentEntity
                    {
                        Author = Guid.NewGuid().ToString(), Content = "New Comment", DatePosted = DateTime.Now,
                    }
                ]
            }
        };

        _mongoDbRepository.AddRange(posts);
        _mongoDbRepository.SaveChanges();

        foreach (var expected in posts)
        {
            var actual = _mongoDbRepository.Find(expected.Id);

            Assert.NotNull(actual);
            Assert.Equal(expected.Title, actual.Title);
        }

        _mongoDbRepository.RemoveRange(posts.Select(x => x.Id));
        _mongoDbRepository.SaveChanges();

        foreach (var expected in posts)
        {
            var actual = _mongoDbRepository.Find(expected.Id);

            Assert.Null(actual);
        }
    }

    [Fact]
    public async Task CanCall_RemoveRangeAsync()
    {
        await Assert.ThrowsAsync<NotImplementedException>(async () => await _mongoDbRepository.RemoveRangeAsync(new List<ObjectId>()));
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}