using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_poe_part2.backend;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {//start of class

        /// <summary>Captures a result (Organizer only). Expects the enrolment id, the finish time as hh:mm:ss (for example 00:48:32), the position and optional notes.</summary>
        /// <response code="200">Result saved.</response>
        /// <response code="400">Invalid details, or the participant was rejected or withdrew.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">You are not the organizer of this event.</response>
        /// <response code="404">Enrolment not found.</response>
        /// <response code="409">This enrolment already has a result.</response>
        [HttpPost]
        public IActionResult CreateResult(int EnrolmentId, string FinishTime, int Position, string Notes = "")
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only organizers can capture results.");
            }

            //check the finish time text
            TimeSpan finishTime;

            if (TimeSpan.TryParse(FinishTime, out finishTime) == false || finishTime <= TimeSpan.Zero)
            {
                return BadRequest("FinishTime must look like 00:48:32.");
            }

            if (Position < 1)
            {
                return BadRequest("Position must be 1 or higher.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            EnrolmentDatabase enrolmentStore = new EnrolmentDatabase();
            Enrolments enrolment = enrolmentStore.GetEnrolmentById(EnrolmentId);

            if (enrolmentStore.errorMessage != "")
            {
                return StatusCode(500, enrolmentStore.errorMessage);
            }

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            if (enrolment.OrganiserId != userId)
            {
                return StatusCode(403, "You can only capture results for your own events.");
            }

            if (enrolment.Status == "Rejected" || enrolment.Status == "Withdrawn")
            {
                return BadRequest("This participant was rejected or withdrew from the event.");
            }

            //an enrolment can only have one result
            ResultDatabase store = new ResultDatabase();
            List<Results> existing = store.GetResultsByEvent(enrolment.EventId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            foreach (Results item in existing)
            {
                if (item.EnrolmentId == EnrolmentId)
                {
                    return Conflict("This enrolment already has a result.");
                }
            }

            string result = store.CreateResult(EnrolmentId, finishTime.ToString(), Position, Notes);

            if (result.Contains("successfully") == false)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }//end of method


        /// <summary>Gets the results of the logged in Participant.</summary>
        /// <response code="200">Returns the results.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Only Participants can use this.</response>
        [HttpGet("Mine")]
        public IActionResult GetMyResults()
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

            ResultDatabase store = new ResultDatabase();
            List<Results> results = store.GetResultsByUser(userId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            return Ok(results);
        }//end of method


        /// <summary>Gets all results of an event, fastest first (only the Organizer who created it).</summary>
        /// <response code="200">Returns the results.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">You are not the organizer of this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("Event/{eventId}")]
        public IActionResult GetEventResults(int eventId)
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only organizers can view all results.");
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
                return StatusCode(403, "You can only view results for your own events.");
            }

            ResultDatabase store = new ResultDatabase();
            List<Results> results = store.GetResultsByEvent(eventId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            return Ok(results);
        }//end of method

    }//end of class
}//end of namespace