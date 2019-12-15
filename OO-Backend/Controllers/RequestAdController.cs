using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JWTAuthDemo.Models;

namespace JWTAuthDemo.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api")]
    public class RequestAdController : ControllerBase
    {
        private readonly ILogger<RequestAdController> _logger;
        private readonly DatabaseContext _database;

        public RequestAdController(ILogger<RequestAdController> logger, DatabaseContext context)
        {
            _logger = logger;
            _database = context;
        }

        [HttpGet]
        [Route("requestAds")]
        [AllowAnonymous]
        public List<RequestServicesAdModel> GetAllRequestAds() => _database.GetRequestServicesAds();

        [HttpGet]
        [Route("requestAd/{id}")]
        [AllowAnonymous]
        public IActionResult GetRequestAd(long id)
        {
            if (RequestServicesAdExists(id))
            {
                var requestAd = _database.GetRequestServicesAds().Find(o => o.Id == id);

                return Ok(requestAd);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("requestAd")]
        public IActionResult AddRequestServicesAd([FromBody] RequestServicesAdModel requestAd)
        {
            var ownerId = Convert.ToInt64(User.Identity.Name);

            requestAd.UserId = ownerId;

            _logger.LogInformation("Add requestAd for requestAdId: {RequestAdId}", requestAd.Id);
            _database.AddRequestServicesAd(requestAd);
            return Ok(requestAd);
        }

        // PUT: api/RequestAd/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        [Route("requestAd/{id}")]
        public async Task<IActionResult> PutRequestAd(long id, RequestServicesAdModel requestAd)
        {
            if (id != requestAd.Id)
            {
                return BadRequest();
            }

            if (IsOwner(requestAd.UserId))
            {
                _database.Entry(requestAd).State = EntityState.Modified;
                try
                {
                    await _database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestServicesAdExists(id))
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

        // DELETE: api/RequestAd/5
        [HttpDelete]
        [Route("requestAd/{id}")]
        public async Task<ActionResult<RequestServicesAdModel>> DeleteRequestAd(long id)
        {
            var requestAd = await _database.RequestServicesAds.FindAsync(id);
            if (requestAd == null)
            {
                return NotFound();
            }

            if (IsOwner(requestAd.UserId))
            {
                _database.RemoveRequestServicesAd(requestAd);
            }
            else
            {
                return Unauthorized();
            }


            return requestAd;
        }
        
        private bool IsOwner(long id)
        {
            return User.Identity.Name == id.ToString();
        }

        private bool RequestServicesAdExists(long id)
        {
            return _database.GetRequestServicesAds().Any(e => e.Id == id);
        }
    }
}
