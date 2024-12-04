using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace RedisExplain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ICacheService _cache;

        public RedisController(ICacheService cacheService, AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
            _cache = cacheService;
        }
        [HttpGet("redis")]
        public async Task<IActionResult> GetAllCustomersUsingRedisCache()
        {
            var cacheKey = "customerList";
            List<Customer> resp;
            resp = await _cache.GetCacheValueAsync<List<Customer>>(cacheKey);
            if (resp == default)
            {
                _dbContext.Database.EnsureCreated();
                resp = await _dbContext.Customers.ToListAsync();
                await _cache.SetCacheValueAsync(cacheKey, resp);
            }

            return Ok(resp);
        }


    }
}
