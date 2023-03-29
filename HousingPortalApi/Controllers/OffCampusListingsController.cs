using HousingPortalApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HousingPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffCampusListingsController : Controller
    {
        private readonly OffCampusHousingContext _context;

        public OffCampusListingsController(OffCampusHousingContext context)
        {
            _context = context;
        }

        // GET: api/OffCampusListings
        [HttpGet]
        public IEnumerable<OffCampusListing> GetOffCampusListings()
        {
            return _context.OffCampusListings.ToList();
        }
    }
}
