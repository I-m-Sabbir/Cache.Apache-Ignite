using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client.Cache;

namespace Cache.Operations.Cache.Entity;

public class Product
{
    [QuerySqlField(IsIndexed = true)]
    public long Id { get; set; }

    [QuerySqlField]
    public string Name { get; set; }

    [QuerySqlField]
    public string Description { get; set; }

    public static CacheClientConfiguration GetProductConfiguration()
    {
        return new CacheClientConfiguration
        {
            Name = "Product",
            QueryEntities = new[]
            {
                new QueryEntity
                {
                    KeyType = typeof(int),
                    ValueType = typeof(Product),
                    Fields = new[]
                    {
                        new QueryField("Id", typeof(long)),
                        new QueryField("Name", typeof(string)),
                        new QueryField("Description", typeof(string))
                    },
                    Indexes = new[]
                    {
                        new QueryIndex("Id")
                    }
                }
            },
            EnableStatistics = true,
            CacheMode = CacheMode.Replicated
        };
    }
}
