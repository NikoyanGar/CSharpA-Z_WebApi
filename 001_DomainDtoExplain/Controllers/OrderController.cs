using _001_DomainDtoExplain.Models.Doamin.AggregatesAndAggregateRoots;
using Microsoft.AspNetCore.Mvc;

namespace _001_DomainDtoExplain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            _orderService.PlaceOrder(order);
            return Ok();
        }
    }
}
