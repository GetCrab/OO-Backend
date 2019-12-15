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
    public class DogController : ControllerBase
    {
        private readonly ILogger<DogController> _logger;
        private readonly DatabaseContext _database;

        public DogController(ILogger<DogController> logger, DatabaseContext context)
        {
            _logger = logger;
            _database = context;
        }

        [HttpGet]
        [Route("dogs")]
        [AllowAnonymous]
        public List<DogModel> GetAllDogs() => _database.GetDogs();
        
        [HttpGet]
        [Route("dog/{id}")]
        [AllowAnonymous]
        public IActionResult GetDog(long id)
        {
            if (DogExists(id))
            {
                var dog = GetDogFromDatabase(id);

                return Ok(dog);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("dog")]
        public IActionResult AddDog([FromBody] DogModel dog)
        {
            var ownerId = Convert.ToInt64(User.Identity.Name);

            if (!UserExists(ownerId))
            {
                return BadRequest("Owner doesn't exist");
            }

            dog.OwnerId = ownerId;

            _logger.LogInformation("Add Dog for DogId: {DogId}", dog.Id);
            _database.AddDog(dog);
            return Ok(dog);
        }

        // PUT: api/Dogs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        [Route("dog/{id}")]
        public async Task<IActionResult> PutDog(long id, DogModel dog)
        {
            if (id != dog.Id)
            {
                return BadRequest();
            }

            if (IsOwner(dog.OwnerId))
            {
                _database.Entry(dog).State = EntityState.Modified;
                try
                {
                    await _database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DogExists(id))
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

        // DELETE: api/Dogs/5
        [HttpDelete]
        [Route("dog/{id}")]
        public async Task<ActionResult<DogModel>> DeleteDog(long id)
        {
            var dog = await _database.Dogs.FindAsync(id);
            if (dog == null)
            {
                return NotFound();
            }

            if (IsOwner(dog.OwnerId))
            {
                _database.RemoveDog(dog);
            }
            else
            {
                return Unauthorized();
            }


            return dog;
        }

        private bool IsOwner(long id)
        {
            return User.Identity.Name == id.ToString();
        }

        private DogModel GetDogFromDatabase(long id)
        {
            return _database.GetDogs().Find(dog => dog.Id == id);
        }

        private bool UserExists(long id)
        {
            return _database.GetUsers().Any(e => e.Id == id);
        }

        private bool DogExists(long id)
        {
            return _database.GetDogs().Any(e => e.Id == id);
        }
    }
}
