using API.HelperFunctions;
using Cache.Operations;
using Cache.Operations.Cache.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.APIs
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ICacheServicce _cacheService;
        private readonly string _cacheName;

        public ProductsController(ICacheServicce cacheService)
        {
            _cacheService = cacheService;
            _cacheName = _cacheService.GetOrCreateCache<int, Product>(Product.GetProductConfiguration()).Name;
        }

        [HttpPost]
        public IActionResult AddProduct(string name, string description)
        {
            string message = string.Empty;
            try
            {
                var key = _cacheService.GetCacheSize<int, Product>(_cacheName);
                key = key + 1;
                var product = new Product { Id = key, Name = name, Description = description };
                _cacheService.Put(_cacheName, (int)key, product);

                message = "Successfully Product Added.";
                return Ok(message);
            }
            catch (Exception ex)
            {
                message = ex.Message;

                return BadRequest(message);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            var str = new List<Product>();
            try
            {
                var result = _cacheService.ExecuteQuery<int, Product>(_cacheName, $"Select * from Product");

                if (result is not null)
                    str = CommonHelper.FieldsQueryCursorToList<Product>(result);

                return Ok(str);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var str = new List<Product>();
            try
            {
                var result = _cacheService.ExecuteQuery<int, Product>(_cacheName, $"Select * from Product where id= {id}");

                if (result is not null)
                    str = CommonHelper.FieldsQueryCursorToList<Product>(result);

                return Ok(str);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddProductsWithSql(string name, string description)
        {
            try
            {
                var key = _cacheService.GetCacheSize<int, Product>(_cacheName);
                key = key + 1;
                var query = $"Insert into Product(_key, Id, Name, Description) Values({key}, {key}, '{name}', '{description}')";
                var result = _cacheService.ExecuteQuery<int, Product>(_cacheName, query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddTenProducts()
        {
            string message = string.Empty;
            try
            {
                var key = _cacheService.GetCacheSize<int, Product>(_cacheName);
                key = key + 1;
                var products = new Dictionary<int, Product>();
                for (int i = 0; i < 10; i++)
                {
                    var product = new Product { Id = key, Name = $"Demo-Product-{key}", Description = $"Demo-Description-{key}" };
                    products.Add((int)key, product);
                    key = key + 1;
                }

                _cacheService.PutAll(_cacheName, products);
                message = "Successfully 10 Products Added.";
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
