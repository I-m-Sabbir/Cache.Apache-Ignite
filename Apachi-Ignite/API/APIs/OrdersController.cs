using API.HelperFunctions;
using Cache.Operations;
using Cache.Operations.Cache.Dto;
using Cache.Operations.Cache.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace API.APIs
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ICacheServicce _cacheService;
        private readonly string _cacheName;
        private readonly string _orderDetailsCacheName;

        public OrdersController(ICacheServicce cacheService)
        {
            _cacheService = cacheService;
            _cacheName = _cacheService.GetOrCreateCache<int, CustomerOrder>(CustomerOrder.GetOrderCacheConfiguration()).Name;
            _orderDetailsCacheName = _cacheService.GetOrCreateCache<int, OrderDetails>(OrderDetails.GetOrderDetailsConfiguration()).Name;
        }

        [HttpGet]
        public IActionResult Get(int start, int length)
        {
            var str = new List<OrderDto>();
            try
            {
                var personCacheName = _cacheService.GetOrCreateCache<int, Person>(Person.GetPersonCacheConfiguration()).Name;
                var productCacheName = _cacheService.GetOrCreateCache<int, Product>(Product.GetProductConfiguration()).Name;

                var query = $@"
Select p.Name, p.Age, p.Address, STRING_AGG(pr.Name, ',') AS ProductName, o.OrderDate
from CustomerOrder as o
inner join ""{_orderDetailsCacheName}"".OrderDetails as od on od.OrderId = o.Id 
inner join ""{personCacheName}"".Person as p on p.Id = o.CustomerId 
inner join ""{productCacheName}"".Product as pr on pr.Id = od.ProductId 
Group by p.Name, p.Age, p.Address, o.OrderDate
Order by o.OrderDate asc
OFFSET {start} ROWS FETCH NEXT {length} ROWS ONLY";
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var result = _cacheService.ExecuteQuery<int, CustomerOrder>(_cacheName, query);
                if (result is not null)
                    str = CommonHelper.FieldsQueryCursorToList<OrderDto>(result);
                stopWatch.Stop();
                return Ok(new { message = $"{str.Count} Records Load Time: {stopWatch.ElapsedMilliseconds} milliseconds, {TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds).TotalSeconds} seconds and {TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds).TotalMinutes} minutes", data = str }); ;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateOrder(long customerId, string remarks, List<long> productIds)
        {
            try
            {
                var customerOrderKey = _cacheService.GetCacheSize<int, CustomerOrder>(_cacheName);
                customerOrderKey = customerOrderKey + 1;
                var order = new CustomerOrder { Id = customerOrderKey, CustomerId = customerId, Remarks = remarks, OrderDate = DateTime.UtcNow.AddHours(6) };
                _cacheService.Put<int, CustomerOrder>(_cacheName, (int)customerOrderKey, order);

                var orderDetailskey = _cacheService.GetCacheSize<int, OrderDetails>(_cacheName);
                orderDetailskey = orderDetailskey + 1;
                var orderDetailsKeyValuePair = new Dictionary<int, OrderDetails>();

                foreach (var productId in productIds)
                {
                    var orderDetails = new OrderDetails { Id = orderDetailskey, OrderId = customerOrderKey, ProductId = productId };
                    orderDetailsKeyValuePair.Add((int)orderDetailskey, orderDetails);
                    orderDetailskey += 1;
                }

                _cacheService.PutAll(_orderDetailsCacheName, orderDetailsKeyValuePair);

                return Ok("Successfully Order Added.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateThousandOrders()
        {
            string message = string.Empty;
            try
            {
                Random rnd = new Random();
                var customerOrderKey = _cacheService.GetCacheSize<int, CustomerOrder>(_cacheName);
                customerOrderKey = customerOrderKey + 1;
                var orders = new Dictionary<int, CustomerOrder>();
                
                var orderDetailskey = _cacheService.GetCacheSize<int, OrderDetails>(_cacheName);
                orderDetailskey = orderDetailskey + 1;
                var orderDetailsKeyValuePair = new Dictionary<int, OrderDetails>();

                for (int i = 0; i < 1000; i++)
                {
                    var order = new CustomerOrder { Id = customerOrderKey, CustomerId = i + 1, Remarks = "Demo", OrderDate = DateTime.UtcNow.AddHours(6) };

                    for (int j = 0; j < 5; j++)
                    {
                        var orderDetails = new OrderDetails { Id = orderDetailskey, OrderId = customerOrderKey, ProductId = rnd.Next(1, 100) };
                        orderDetailsKeyValuePair.Add((int)orderDetailskey, orderDetails);
                        orderDetailskey += 1;
                    }

                    orders.Add((int)customerOrderKey, order);
                    customerOrderKey = customerOrderKey + 1;
                }

                _cacheService.PutAll(_cacheName, orders);
                _cacheService.PutAll(_orderDetailsCacheName, orderDetailsKeyValuePair);

                message = "Successfully Created 1000 orders.";
                return Ok(message);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return BadRequest(message);
            }
        }
    }
}
