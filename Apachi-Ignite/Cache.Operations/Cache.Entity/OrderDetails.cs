using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client.Cache;

namespace Cache.Operations.Cache.Entity;

public class OrderDetails
{
    [QuerySqlField(IsIndexed = true)]
    public long Id { get; set; }

    [QuerySqlField(IsIndexed = true)]
    public long OrderId { get; set; }

    [QuerySqlField(IsIndexed = true)]
    public long ProductId { get; set; }

    public static CacheClientConfiguration GetOrderDetailsConfiguration()
    {
        return new CacheClientConfiguration
        {
            Name = "OrderDetails",
            QueryEntities = new[]
            {
                new QueryEntity
                {
                    KeyType = typeof(int),
                    ValueType = typeof(OrderDetails),
                    Fields = new[]
                    {
                        new QueryField("Id", typeof(long)),
                        new QueryField("OrderId", typeof(long)),
                        new QueryField("ProductId", typeof(long))
                    },
                    Indexes = new[]
                    {
                        new QueryIndex("Id"),
                        new QueryIndex("OrderId"),
                        new QueryIndex("ProductId")
                    }
                }
            },
            EnableStatistics = true,
            CacheMode = CacheMode.Replicated
        };
    }

}
