using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JWTAuthDemo.Models;
using JWTAuthDemo.Responses;

namespace webapi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DatabaseContext _database;

        public UserController(ILogger<UserController> logger, DatabaseContext context)
        {
            _logger = logger;
            _database = context;
        }

        [HttpGet]
        [Route("users")]
        [AllowAnonymous]
        public IActionResult GetUsers()
        {
            var users = _database.GetUsers();
            var response = new List<UserResponse>();
            foreach(var user in users)
            {
                response.Add(CreateUserResponse(user.Id));
            }

            return Ok(response);            
        }

        [HttpGet]
        [Route("user/{id}")]
        [AllowAnonymous]
        public IActionResult GetUser(long id)
        {
            if (UserExists(id))
            {
                return Ok(CreateUserResponse(id));
            } else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("user")]
        [AllowAnonymous]
        public IActionResult AddUser([FromBody] UserModel user)
        {
            if(UsernameExists(user.UserName))
            {
                return BadRequest("Username already exists.");
            }

            _logger.LogInformation("Add User for UserId: {UserId}", user.Id);
            _database.AddUser(user);
            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        [Route("user/{id}")]
        public async Task<IActionResult> PutUser(long id, UserModel user)
        {
            if (id != user.Id)
            {
                return BadRequest("Wrong user id.");
            }

            if (IsOwner(user.Id))
            {
                _database.Entry(user).State = EntityState.Modified;
                try
                {
                    await _database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
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

        // DELETE: api/Users/5
        [HttpDelete]
        [Route("user/{id}")]
        public async Task<ActionResult<UserModel>> DeleteUser(long id)
        {
            var user = await _database.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (IsOwner(user.Id))
            {
                _database.RemoveUser(user);
            }
            else
            {
                return Unauthorized();
            }

            return user;
        }

        private UserResponse CreateUserResponse(long id)
        {
            var user = _database.GetUsers().Find(user => user.Id == id);
            var response = new UserResponse(user.Id, user.UserName, user.EmailAddress,
                new List<DogModel>(), new List<RatingModel>(), new List<NotificationModel>());

            var dogs = _database.GetDogs().FindAll(dog => dog.OwnerId == id);
            response.Dogs.AddRange(dogs);

            var ratings = _database.GetRatings().FindAll(rating => rating.ReceivedUserId == id);
            response.Ratings.AddRange(ratings);

            var notifications = _database.GetNotifications().FindAll(notifications => notifications.ReceivedUserId == id);
            response.Notifications.AddRange(notifications);

            return response;
        }

        private bool IsOwner(long id)
        {
            return User.Identity.Name == id.ToString();
        }

        private bool UsernameExists(string username)
        {
            return _database.GetUsers().Any(o => o.UserName == username);
        }

        private bool UserExists(long id)
        {
            return _database.GetUsers().Any(e => e.Id == id);
        }
    }
}
