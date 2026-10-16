using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_poe_part2.backend;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {//start of class

        //HttpPost request to send the user info to the database
        AuthModel enduser1;

        public enum role
        {
            Organizer,
            Participant
        }

        [HttpPost("Register")]

        //method to send user info to the database

        public string RegisterInfo(string FirstName, string LastName, int age, string Email, string passwordHashed, role UserRole)
        {//start of method
            enduser1 = new AuthModel()
            {
                FirstName = FirstName,
                LastName = LastName,
                age = age,
                Email = Email,
                passwordHashed = passwordHashed,
                //createdAt = createdAt,

            };

            //hash the users passoerd
            string hashedpassword = BCrypt.Net.BCrypt.HashPassword(passwordHashed);

            //call the user class into this method
            AuthDatabase store = new AuthDatabase();
            string result = store.RegisterUserInfo(enduser1.FirstName, enduser1.LastName, enduser1.age, enduser1.Email, hashedpassword, UserRole.ToString());



            return result + "  " + UserRole.ToString();
        }//end of method



        //httppost reqyest 
        [HttpGet("Login")]
        public string LoginUser(string Email, string passwordHashed)
        {//start of login method

            AuthDatabase use = new AuthDatabase();
            string reply = use.LogInInfo(Email, passwordHashed);

            //verify the hashed password
            string hashed_password = BCrypt.Net.BCrypt.HashPassword(passwordHashed);

            bool correctpassword = BCrypt.Net.BCrypt.Verify(passwordHashed, hashed_password);

            if (correctpassword.Equals(hashed_password))
            {//start of if
                return reply;

            }//end of if
            else
            {//start of els
                return reply;
            }//end of else


        }//end of login method
    }//end of class
}//end of namespace

