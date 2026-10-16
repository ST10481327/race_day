using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_poe_part2.backend;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {//start of class

        /// <summary>Gets the profile of the logged in user (Organizer or Participant).</summary>
        /// <response code="200">Returns the profile.</response>
        /// <response code="401">Not logged in.</response>
        [HttpGet]
        public IActionResult GetProfile()
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            AuthDatabase store = new AuthDatabase();
            AuthModel user = store.GetUserById(userId);

            if (store.errorMessage != "")
            {
                return StatusCode(500, store.errorMessage);
            }

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }//end of method


        /// <summary>Updates the first name, last name and age of the logged in user.</summary>
        /// <response code="200">Profile updated.</response>
        /// <response code="400">Invalid details.</response>
        /// <response code="401">Not logged in.</response>
        [HttpPut]
        public IActionResult UpdateProfile(string FirstName, string LastName, int age)
        {//start of method
            string role = HttpContext.Session.GetString("Role");

            if (role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (FirstName.Trim() == "" || LastName.Trim() == "")
            {
                return BadRequest("Please fill in your first name and last name.");
            }

            if (age < 1 || age > 120)
            {
                return BadRequest("Age must be between 1 and 120.");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            AuthDatabase store = new AuthDatabase();
            string result = store.UpdateProfile(userId, FirstName.Trim(), LastName.Trim(), age);

            if (result.Contains("successfully") == false)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }//end of method

    }//end of class
}//end of namespace