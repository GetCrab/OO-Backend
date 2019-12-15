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
    public class RatingController : ControllerBase
    {
        private readonly ILogger<RatingController> _logger;
        private readonly DatabaseContext _database;

        public RatingController(ILogger<RatingController> logger, DatabaseContext context)
        {
            _logger = logger;
            _database = context;
        }

        [HttpGet]
        [Route("rating/{id}")]
        [AllowAnonymous]
        public IActionResult GetRating(long id)
        {
            if (RatingExists(id))
            {
                var rating = _database.GetRatings().Find(rating => rating.Id == id);

                return Ok(rating);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("rating")]
        public IActionResult AddRating([FromBody] RatingModel rating)
        {
            if(!UserExists(rating.ReceivedUserId))
            {
                return BadRequest("Received User is not valid.");
            }

            var ownerId = Convert.ToInt64(User.Identity.Name);

            if (rating.ReceivedUserId == ownerId)
            {
                return BadRequest("User can't rate himself.");
            }

            rating.SendUserId = ownerId;

            _logger.LogInformation("Add Rating for RatingId: {RatingId}", rating.Id);
            _database.AddRating(rating);
            return Ok(rating);
        }

        // PUT: api/rating/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        [Route("rating/{id}")]
        public async Task<IActionResult> PutRating(long id, RatingModel rating)
        {
            if (id != rating.Id)
            {
                return BadRequest();
            }

            if (IsOwner(rating.SendUserId))
            {
                _database.Entry(rating).State = EntityState.Modified;
                try
                {
                    await _database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatingExists(id))
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

        // DELETE: api/rating/5
        [HttpDelete]
        [Route("rating/{id}")]
        public async Task<ActionResult<RatingModel>> DeleteRating(long id)
        {
            var rating = await _database.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            if (IsOwner(rating.SendUserId))
            {
                _database.RemoveRating(rating);
            }
            else
            {
                return Unauthorized();
            }

            return rating;
        }

        private bool IsOwner(long id)
        {
            return User.Identity.Name == id.ToString();
        }

        private bool UserExists(long id)
        {
            return _database.GetUsers().Any(e => e.Id == id);
        }

        private bool RatingExists(long id)
        {
            return _database.GetRatings().Any(e => e.Id == id);
        }
    }
}
