﻿using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client.Cache;

namespace Cache.Operations.Cache.Entity;

public class CustomerOrder
{
    [QuerySqlField(IsIndexed = true)]
    public long Id { get; set; }

    [QuerySqlField(IsIndexed = true)]
    public long CustomerId { get; set; }

    [QuerySqlField]
    public DateTime OrderDate { get; set; }

    [QuerySqlField]
    public string Remarks { get; set; }

    public static CacheClientConfiguration GetOrderCacheConfiguration()
    {
        return new CacheClientConfiguration
        {
            Name = "CustomerOrder",
            QueryEntities = new[]
            {
                new QueryEntity
                {
                    KeyType = typeof(int),
                    ValueType = typeof(CustomerOrder),
                    Fields = new[]
                    {
                        new QueryField("Id", typeof(long)),
                        new QueryField("CustomerId", typeof(long)),
                        new QueryField("OrderDate", typeof(DateTime)),
                        new QueryField("Remarks", typeof(string))
                    },
                    Indexes = new[]
                    {
                        new QueryIndex("Id"),
                        new QueryIndex("CustomerId")
                    }
                }
            },
            EnableStatistics = true,
            CacheMode = CacheMode.Replicated
        };
    }
}
