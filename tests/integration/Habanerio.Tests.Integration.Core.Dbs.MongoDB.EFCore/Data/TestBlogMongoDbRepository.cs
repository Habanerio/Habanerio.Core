using System.Linq.Expressions;
using Habanerio.Core;
using Habanerio.Core.DBs.EFCore.Expressions;
using Habanerio.Core.DBs.EFCore.Interfaces;
using Habanerio.Core.DBs.MongoDB.EFCore;
using MongoDB.Bson;

namespace Habanerio.Tests.Integration.Core.Dbs.MongoDB.EFCore.Data;

public interface ITestBlogMongoDbRepository : IDbRepository<BlogPostEntity, ObjectId>
{
    //IMongoCollection<BlogPostEntity> GetCollection();

    Task<PagedResults<BlogPostEntity>> SearchAsync(
        string title = "", string author = "", string content = "",
        int pageNo = 1, int pageSize = 25,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// This is only to wrap and expose the underlying MongoDbRepository
/// </summary>
public class TestBlogMongoDbRepository : MongoDbRepository<BlogPostEntity>, ITestBlogMongoDbRepository
{
    public TestBlogMongoDbRepository(MongoDbContext context) : base(context)
    {
        //DbSet = Context.Set<BlogPostEntity>();
    }

    //public IMongoCollection<BlogPostEntity> GetCollection()
    //{
    //    return Collection;
    //}

    /// <summary>
    /// Calls the underlying Filter()
    /// </summary>
    /// <param name="title"></param>
    /// <param name="author"></param>
    /// <param name="content"></param>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PagedResults<BlogPostEntity>> SearchAsync(
        string title = "", string author = "", string content = "",
        int pageNo = 1, int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var orderBys = new OrderByExpression<BlogPostEntity>()
            .AddOrderBy(c => c.DateCreated)
            .AddOrderByDescending(c => c.Title);

        Expression<Func<BlogPostEntity, bool>> filter = c =>
            (string.IsNullOrWhiteSpace(title) || c.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(author) || c.Author.Contains(author, StringComparison.CurrentCultureIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(content) || c.Content.Contains(content, StringComparison.CurrentCultureIgnoreCase));

        var results =
            Filter(
                filter: filter,
                orderBys: orderBys,
                pageNo: pageNo,
                pageSize: pageSize,
                cancellationToken);

        return results;
    }
}