using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_poe_part2.backend;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {//start of class

        public enum eventType
        {
            Run,
            Walk,
            Cycle
        }

        public enum eventStatus
        {
            Upcoming,
            Open,
            Closed,
            Completed,
            Cancelled
        }


        /// <summary>Gets all events, soonest first (Organizers and Participants).</summary>
        /// <response code="200">Returns the events.</response>
        /// <response code="401">Not logged in.</response>
        [HttpGet]
        public IActionResult GetEvents()
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            EventDatabase store = new EventDatabase();
            List<Events> events = store.GetAllEvents();

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            return Ok(events);
        }//end of method


        /// <summary>Gets one event by its id (Organizers and Participants).</summary>
        /// <response code="200">Returns the event.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("{id}")]
        public IActionResult GetEvent(int id)
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            EventDatabase store = new EventDatabase();
            Events ev = store.GetEventById(id);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            return Ok(ev);
        }//end of method


        /// <summary>Creates an event (Organizer only). Expects name, description, date (not in the past), location, distance in km, type (Run, Walk or Cycle), max participants and status.</summary>
        /// <response code="200">Event created.</response>
        /// <response code="400">Invalid details.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Only Organizers can create events.</response>
        [HttpPost]
        public IActionResult CreateEvent(string EventName, DateTime EventDate, string Location, decimal DistanceKm, eventType EventType, int MaxParticipants, eventStatus Status, string Description = "")
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only organizers can create events.");
            }

            string problem = CheckEvent(EventName, Location, DistanceKm, MaxParticipants);

            if (problem != "")
            {
                return BadRequest(problem);
            }

            if (EventDate.Date < DateTime.Today)
            {
                return BadRequest("Event date cannot be in the past.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            Events newEvent = new Events()
            {
                EventName = EventName.Trim(),
                Description = Description,
                EventDate = EventDate,
                Location = Location.Trim(),
                DistanceKm = DistanceKm,
                EventType = EventType.ToString(),
                MaxParticipants = MaxParticipants,
                Status = Status.ToString(),
            };

            EventDatabase store = new EventDatabase();
            string result = store.CreateEvent(newEvent, userId);

            if (result.Contains("successfully") == false)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }//end of method


        /// <summary>Updates an event (only the Organizer who created it). Expects the same details as creating an event.</summary>
        /// <response code="200">Event updated.</response>
        /// <response code="400">Invalid details.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">You are not the organizer of this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpPut("{id}")]
        public IActionResult UpdateEvent(int id, string EventName, DateTime EventDate, string Location, decimal DistanceKm, eventType EventType, int MaxParticipants, eventStatus Status, string Description = "")
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only organizers can update events.");
            }

            string problem = CheckEvent(EventName, Location, DistanceKm, MaxParticipants);

            if (problem != "")
            {
                return BadRequest(problem);
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            EventDatabase store = new EventDatabase();
            Events ev = store.GetEventById(id);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            if (ev.OrganiserId != userId)
            {
                return StatusCode(403, "You can only update your own events.");
            }

            Events changed = new Events()
            {
                EventName = EventName.Trim(),
                Description = Description,
                EventDate = EventDate,
                Location = Location.Trim(),
                DistanceKm = DistanceKm,
                EventType = EventType.ToString(),
                MaxParticipants = MaxParticipants,
                Status = Status.ToString(),
            };

            string result = store.UpdateEvent(id, changed);

            if (result.Contains("successfully") == false)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }//end of method


        /// <summary>Deletes an event with its categories, enrolments and results (only the Organizer who created it).</summary>
        /// <response code="200">Event deleted.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">You are not the organizer of this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only organizers can delete events.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            EventDatabase store = new EventDatabase();
            Events ev = store.GetEventById(id);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            if (ev.OrganiserId != userId)
            {
                return StatusCode(403, "You can only delete your own events.");
            }

            string result = store.DeleteEvent(id);

            if (result.Contains("successfully") == false)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }//end of method


        //checks the event details, returns "" when everything is fine
        private string CheckEvent(string EventName, string Location, decimal DistanceKm, int MaxParticipants)
        {
            if (EventName.Trim() == "" || Location.Trim() == "")
            {
                return "Please give the event name and location.";
            }

            if (DistanceKm <= 0)
            {
                return "Distance must be more than 0.";
            }

            if (MaxParticipants < 1)
            {
                return "Max participants must be at least 1.";
            }

            return "";
        }

    }//end of class
}//end of namespace