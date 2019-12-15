using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JWTAuthDemo.Models;

namespace webapi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly DatabaseContext _database;

        public NotificationController(ILogger<NotificationController> logger, DatabaseContext context)
        {
            _logger = logger;
            _database = context;
        }

        [HttpGet]
        [Route("notification/{id}")]
        [AllowAnonymous]
        public IActionResult GetNotification(long id)
        {
            if (NotificationExists(id))
            {
                var notification = GetNotificationFromDatabase(id);

                return Ok(notification);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("notification")]
        public IActionResult AddNotification([FromBody] NotificationModel notification)
        {
            if (!UserExists(notification.ReceivedUserId))
            {
                return BadRequest("Received User is not valid.");
            }

            var ownerId = Convert.ToInt64(User.Identity.Name);

            if (notification.ReceivedUserId == ownerId)
            {
                return BadRequest("User can't notify himself.");
            }

            notification.SendUserId = ownerId;

            _logger.LogInformation("Add notification for notificationId: {notification}", notification.Id);
            _database.AddNotification(notification);
            return Ok(notification);
        }

        // PUT: api/notification/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        [Route("notification/{id}")]
        public async Task<IActionResult> PutNotification(long id, NotificationModel notification)
        {
            if (id != notification.Id)
            {
                return BadRequest();
            }

            if (!NotificationExists(id))
            {
                return NotFound();
            }

            if (IsOwner(notification.SendUserId))
            {
                _database.Entry(notification).State = EntityState.Modified;
                try
                {
                    await _database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                return Unauthorized();
            }


            return NoContent();
        }

        // DELETE: api/notification/5
        [HttpDelete]
        [Route("notification/{id}")]
        public async Task<ActionResult<NotificationModel>> DeleteNotification(long id)
        {
            if (NotificationExists(id))
            {
                return NotFound();
            }
            var notification = await _database.Notifications.FindAsync(id);

            if (IsOwner(notification.SendUserId))
            {
                _database.RemoveNotification(notification);
            }
            else
            {
                return Unauthorized();
            }

            return notification;
        }

        private bool UserExists(long id)
        {
            return _database.GetUsers().Any(e => e.Id == id);
        }

        private bool IsOwner(long id)
        {
            return User.Identity.Name == id.ToString();
        }

        private NotificationModel GetNotificationFromDatabase(long id)
        {
            return _database.GetNotifications().Find(notification => notification.Id == id);
        }
        private bool NotificationExists(long id)
        {
            return _database.GetNotifications().Any(e => e.Id == id);
        }
    }
}
