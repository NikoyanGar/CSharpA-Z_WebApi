using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CachingAbstractionsinASP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private const string CustomerCacheKey = "CustomerCacheKey";

        public CustomerController(AppDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            if (!_memoryCache.TryGetValue(CustomerCacheKey, out List<Customer> customers))
            {
                customers = await _context.Customers.ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _memoryCache.Set(CustomerCacheKey, customers, cacheEntryOptions);
            }
            return customers;
        }

        [HttpGet("{id}")]
        public async Task<Customer> Get(int id)
        {
            var cacheKey = $"{CustomerCacheKey}_{id}";
            if (!_memoryCache.TryGetValue(cacheKey, out Customer customer))
            {
                customer = await _context.Customers.FindAsync(id);
                if (customer != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                    _memoryCache.Set(cacheKey, customer, cacheEntryOptions);
                }
            }
            return customer;
        }
    }
}
