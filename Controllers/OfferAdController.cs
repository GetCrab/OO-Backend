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
    public class OfferAdController : ControllerBase
    {
        private readonly ILogger<OfferAdController> _logger;
        private readonly DatabaseContext _database;

        public OfferAdController(ILogger<OfferAdController> logger, DatabaseContext context)
        {
            _logger = logger;
            _database = context;
        }

        [HttpGet]
        [Route("offerAds")]
        [AllowAnonymous]
        public List<OfferServicesAdModel> GetAllOfferAds() => _database.GetOfferServicesAds();
        
        [HttpGet]
        [Route("offerAd/{id}")]
        [AllowAnonymous]
        public IActionResult GetOfferAd(long id)
        {
            if (OfferServicesAdExists(id))
            {
                var offerAd = GetOfferAdFromDatabase(id);

                return Ok(offerAd);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("offerAd")]
        public IActionResult AddOfferServicesAd([FromBody] OfferServicesAdModel offerAd)
        {
            var ownerId = Convert.ToInt64(User.Identity.Name);

            offerAd.UserId = ownerId;

            _logger.LogInformation("Add OfferAd for OfferAdId: {OfferAdId}", offerAd.Id);
            _database.AddOfferServicesAd(offerAd);
            return Ok(offerAd);
        }

        // PUT: api/offerAd/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        [Route("offerAd/{id}")]
        public async Task<IActionResult> PutOfferAd(long id, OfferServicesAdModel offerAd)
        {
            if (id != offerAd.Id)
            {
                return BadRequest();
            }

            if (IsOwner(offerAd.UserId))
            {
                _database.Entry(offerAd).State = EntityState.Modified;
                try
                {
                    await _database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferServicesAdExists(id))
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

        // DELETE: api/offerAd/5
        [HttpDelete]
        [Route("offerAd/{id}")]
        public async Task<ActionResult<OfferServicesAdModel>> DeleteOfferAd(long id)
        {
            var offerAd = await _database.OfferServicesAds.FindAsync(id);
            if (offerAd == null)
            {
                return NotFound();
            }

            if (IsOwner(offerAd.UserId))
            {
                _database.RemoveOfferServicesAd(offerAd);
            }
            else
            {
                return Unauthorized();
            }


            return offerAd;
        }

        private bool IsOwner(long id)
        {
            return User.Identity.Name == id.ToString();
        }

        private OfferServicesAdModel GetOfferAdFromDatabase(long id)
        {
            return _database.GetOfferServicesAds().Find(o => o.Id == id);
        }

        private bool OfferServicesAdExists(long id)
        {
            return _database.GetOfferServicesAds().Any(e => e.Id == id);
        }
    }
}
