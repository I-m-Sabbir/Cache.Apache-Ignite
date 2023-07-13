using API.HelperFunctions;
using Cache.Operations;
using Cache.Operations.Cache.Dto;
using Cache.Operations.Cache.Entity;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Get()
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
Group by p.Name, p.Age, p.Address, o.OrderDate";

                var result = _cacheService.ExecuteQuery<int, CustomerOrder>(_cacheName, query);
                if (result is not null)
                    str = CommonHelper.FieldsQueryCursorToList<OrderDto>(result);

                return Ok(str);
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
                var order = new CustomerOrder { Id = customerOrderKey, CustomerId = customerId, Remarks = remarks, OrderDate = DateTime.UtcNow };
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
    }
}
