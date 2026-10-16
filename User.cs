using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay_poe_part2.Controllers
{//start of the namespace
    [Route("api/[controller]")]
    [ApiController]
    public class User : ControllerBase
    {//start of the class

        //creaate a method that returnsa string called Register()
        //must be [httpPost]
        //pass one parameter called name
        //we will setup the session

        [HttpPost]
        public string Register(string name)
        {//start of the method
            //registering a session so that
            //the data stored when the program is running
            //last accprding to the time set is session 
            HttpContext.Session.SetString("Username", name);
            return "Saved in the memory" + name;
        }//end of the method

        //craete a Load() method
        //this one will use to test if the session ended or not

        [HttpGet]
        public string Load()
        {//start of the  Load method

            //calling session from post method Register()
            //called username(name of the session)

            string name = HttpContext.Session.GetString("username");

            if (name == null)
            {//start for the iff statement
                return "Sessionis expired";
            }//end of the if statement
            else
            {//start of else
                return "";
            }//end of else
                return name + "session in not times out";
        }//end of the Load method

    }//end of the namespace
}//end of the class
