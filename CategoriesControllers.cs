using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_poe_part2.backend;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {//start of class

        /// <summary>Gets the categories of an event, for example Under 20, Senior or 10km (Organizers and Participants).</summary>
        /// <response code="200">Returns the categories.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("{eventId}")]
        public IActionResult GetCategories(int eventId)
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            EventDatabase eventStore = new EventDatabase();
            Events ev = eventStore.GetEventById(eventId);

            if (eventStore.errorMessage != "")
            {
                return StatusCode(500, eventStore.errorMessage);
            }

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            CategoryDatabase store = new CategoryDatabase();
            List<Category> categories = store.GetCategoriesByEvent(eventId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            return Ok(categories);
        }//end of method


        /// <summary>Adds a category to an event (only the Organizer who created the event).</summary>
        /// <response code="200">Category created.</response>
        /// <response code="400">Invalid details.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">You are not the organizer of this event.</response>
        /// <response code="404">Event not found.</response>
        /// <response code="409">The event already has a category with that name.</response>
        [HttpPost]
        public IActionResult CreateCategory(int eventId, string CategoryName, string Description = "")
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only organizers can add categories.");
            }

            if (CategoryName.Trim() == "")
            {
                return BadRequest("Please give the category a name.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            EventDatabase eventStore = new EventDatabase();
            Events ev = eventStore.GetEventById(eventId);

            if (eventStore.errorMessage != "")
            {
                return StatusCode(500, eventStore.errorMessage);
            }

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            if (ev.OrganiserId != userId)
            {
                return StatusCode(403, "You can only add categories to your own events.");
            }

            //check that the event does not have this category yet
            CategoryDatabase store = new CategoryDatabase();
            List<Category> existing = store.GetCategoriesByEvent(eventId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            foreach (Category item in existing)
            {
                if (item.CategoryName.ToLower() == CategoryName.Trim().ToLower())
                {
                    return Conflict("This event already has a category with that name.");
                }
            }

            string result = store.CreateCategory(eventId, CategoryName.Trim(), Description);

            if (result.Contains("successfully") == false)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }//end of method

    }//end of class
}//end of namespace