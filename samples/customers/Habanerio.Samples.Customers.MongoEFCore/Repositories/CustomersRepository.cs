using Habanerio.Core;
using Habanerio.Core.DBs.EFCore.Expressions;
using Habanerio.Core.DBs.EFCore.Interfaces;
using Habanerio.Core.DBs.MongoDB.EFCore;
using Habanerio.Samples.Customers.MongoEFCore.Entities;
using MongoDB.Bson;

namespace Habanerio.Samples.Customers.MongoEFCore.Repositories;

public interface ICustomersRepository : IDbRepository<CustomerDbEntity, ObjectId>
{
    ValueTask<CustomerDbEntity?> Find(string id, CancellationToken cancellationToken = default);

    Task<PagedResults<CustomerDbEntity>> SearchAsync(
            string firstName = "", string email = "", string lastName = "",
            int pageNo = 1, int pageSize = 25,
            CancellationToken cancellationToken = default);
}

public class CustomersRepository : MongoDbRepository<CustomerDbEntity>, ICustomersRepository
{
    //public CustomersRepository(IOptions<MongoDbOptions> options) : base(new SampleMongoDbContext(options)) { }

    public CustomersRepository(SampleMongoDbContext context) : base(context) { }

    public ValueTask<CustomerDbEntity?> Find(string id, CancellationToken cancellationToken = default)
    {
        return FindAsync(ObjectId.Parse(id), cancellationToken);
    }

    public Task<PagedResults<CustomerDbEntity>> SearchAsync(string firstName = "", string email = "", string lastName = "",
        int pageNo = 1, int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var orderBys = new OrderByExpression<CustomerDbEntity>()
            .AddOrderBy(c => c.LastName)
            .AddOrderByDescending(c => c.FirstName);

        var results =
            Filter(
                filter: c =>
                    string.IsNullOrWhiteSpace(firstName) || c.FirstName.Equals(lastName, StringComparison.CurrentCultureIgnoreCase) ||
                    string.IsNullOrWhiteSpace(lastName) || c.LastName.Equals(lastName, StringComparison.CurrentCultureIgnoreCase) ||
                    string.IsNullOrWhiteSpace(email) || c.Email.Equals(lastName, StringComparison.CurrentCultureIgnoreCase),
                orderBys: orderBys,
                pageNo: pageNo,
                pageSize: pageSize,
                cancellationToken);

        return results;
    }
}