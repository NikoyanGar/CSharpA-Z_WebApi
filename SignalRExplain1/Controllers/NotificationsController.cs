using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRExplain1.Hubs;
using SignalRExplain1.SronglyTypeHubs;
using SignalRExplain1.SronglyTypeHubs.ClientInterfaces;

namespace SignalRExplain1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] IHubContext<NotificationsHub> hubContext, [FromBody] string content)
        {
            await hubContext.Clients.All.SendAsync("ReceiveNotification", content);
            return Ok();
        }

        [HttpPost("strongHub")]
        public async Task<IActionResult> PostOnStrong([FromServices] IHubContext<StrongNotificationsHub, INotificationsClient> hubContext, [FromBody] string content)
        {
            await hubContext.Clients.All.ReceiveNotification(content);
            return Ok();
        }

        [HttpPost("sendToUser")]
        public async Task<IActionResult> SendToUser([FromServices] IHubContext<NotificationsHub> hubContext, [FromBody] UserNotification notification)
        {
            await hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification.Content);
            return Ok();
        }
        //implement for authorized users
        [HttpPost("strongHub/sendToUser")]
        public async Task<IActionResult> SendToUserOnStrong(
            [FromServices] IHubContext<StrongNotificationsHub, INotificationsClient> hubContext,
            [FromBody] UserNotification notification)
        {
            await hubContext.Clients.User(notification.UserId).ReceiveNotification(notification.Content);
            return Ok();
        }
    }

    public class UserNotification
    {
        public string UserId { get; set; }
        public string Content { get; set; }
    }
}
