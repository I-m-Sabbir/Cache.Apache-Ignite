using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client.Cache;

namespace Cache.Operations.Cache.Entity;

public class Person
{
    [QuerySqlField(IsIndexed = true)]
    public long Id { get; set; }

    [QuerySqlField]
    public string Name { get; set; } = null!;

    [QuerySqlField]
    public int Age { get; set; }

    [QuerySqlField]
    public string Address { get; set; } = null!;

    public static CacheClientConfiguration GetPersonCacheConfiguration()
    {
        return new CacheClientConfiguration
        {
            Name = "Person",
            QueryEntities = new[]
            {
                new QueryEntity
                {
                    KeyType = typeof(int),
                    ValueType = typeof(Person),
                    Fields = new[]
                    {
                        new QueryField("Id", typeof(long)),
                        new QueryField("Name", typeof(string)),
                        new QueryField("Age", typeof(int)),
                        new QueryField("Address", typeof(string))
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
