using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_poe_part2.backend;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {//start of class

        public enum enrolmentStatus
        {
            Approved,
            Rejected
        }


        /// <summary>Enrols the logged in Participant in an event. Expects the event id and the id of one of that event's categories.</summary>
        /// <response code="200">Enrolled. The enrolment starts as Pending.</response>
        /// <response code="400">The event is not open, or the category is not part of the event.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Only Participants can enrol.</response>
        /// <response code="404">Event not found.</response>
        /// <response code="409">Already enrolled, or the event is full.</response>
        [HttpPost]
        public IActionResult Enrol(int EventId, int CategoryId)
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Participant")
            {
                return StatusCode(403, "Only participants can enrol in events.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            //find the event
            EventDatabase eventStore = new EventDatabase();
            Events ev = eventStore.GetEventById(EventId);

            if (eventStore.errorMessage != "")
            {
                return StatusCode(500, eventStore.errorMessage);
            }

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            if (ev.Status != "Open" && ev.Status != "Upcoming")
            {
                return BadRequest("This event is not open for enrolment.");
            }

            //the category must belong to the event
            CategoryDatabase categoryStore = new CategoryDatabase();
            Category category = categoryStore.GetCategoryById(CategoryId);

            if (categoryStore.errorMessage != "")
            {
                return StatusCode(500, categoryStore.errorMessage);
            }

            if (category == null || category.EventId != EventId)
            {
                return BadRequest("That category does not belong to this event.");
            }

            //check if the user already enrolled and count the places that are taken
            EnrolmentDatabase store = new EnrolmentDatabase();
            List<Enrolments> existing = store.GetEnrolmentsByEvent(EventId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            int placesTaken = 0;

            foreach (Enrolments item in existing)
            {
                if (item.UserId == userId)
                {
                    return Conflict("You are already enrolled in this event.");
                }

                if (item.Status != "Rejected" && item.Status != "Withdrawn")
                {
                    placesTaken = placesTaken + 1;
                }
            }

            if (placesTaken >= ev.MaxParticipants)
            {
                return Conflict("This event is full.");
            }

            string result = store.CreateEnrolment(userId, EventId, CategoryId);

            if (result.Contains("successfully") == false)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }//end of method


        /// <summary>Gets the enrolments of the logged in Participant.</summary>
        /// <response code="200">Returns the enrolments.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Only Participants can use this.</response>
        [HttpGet("Mine")]
        public IActionResult GetMyEnrolments()
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Participant")
            {
                return StatusCode(403, "Only participants can use this.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            EnrolmentDatabase store = new EnrolmentDatabase();
            List<Enrolments> enrolments = store.GetEnrolmentsByUser(userId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            return Ok(enrolments);
        }//end of method


        /// <summary>Gets all enrolments for an event (only the Organizer who created it).</summary>
        /// <response code="200">Returns the enrolments.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">You are not the organizer of this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("Event/{eventId}")]
        public IActionResult GetEventEnrolments(int eventId)
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only organizers can view enrolments.");
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
                return StatusCode(403, "You can only view enrolments for your own events.");
            }

            EnrolmentDatabase store = new EnrolmentDatabase();
            List<Enrolments> enrolments = store.GetEnrolmentsByEvent(eventId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            return Ok(enrolments);
        }//end of method


        /// <summary>Approves or rejects an enrolment (only the Organizer who created the event).</summary>
        /// <response code="200">Status updated.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">You are not the organizer of this event.</response>
        /// <response code="404">Enrolment not found.</response>
        [HttpPut("{id}")]
        public IActionResult UpdateEnrolmentStatus(int id, enrolmentStatus Status)
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only organizers can approve or reject enrolments.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            EnrolmentDatabase store = new EnrolmentDatabase();
            Enrolments enrolment = store.GetEnrolmentById(id);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            if (enrolment.OrganiserId != userId)
            {
                return StatusCode(403, "You can only manage enrolments for your own events.");
            }

            string result = store.UpdateEnrolmentStatus(id, Status.ToString());

            if (result.Contains("successfully") == false)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }//end of method

    }//end of class
}//end of namespace