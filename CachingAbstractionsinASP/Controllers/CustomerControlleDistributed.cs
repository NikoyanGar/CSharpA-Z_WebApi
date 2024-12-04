using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CachingAbstractionsinASP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerControlleDistributed : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _distributedCache;
        private const string CustomerCacheKey = "CustomerCacheKey";

        public CustomerControlleDistributed(AppDbContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            var customers = await _distributedCache.GetOrSetAsync(CustomerCacheKey, async () =>
            {
                return await _context.Customers.ToListAsync();
            });

            return customers;
        }

        [HttpGet("{id}")]
        public async Task<Customer> Get(int id)
        {
            var cacheKey = $"{CustomerCacheKey}_{id}";
            var customer = await _distributedCache.GetOrSetAsync(cacheKey, async () =>
            {
                return await _context.Customers.FindAsync(id);
            });

            return customer;
        }
    }
}