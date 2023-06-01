using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    //All controllers derive from ControllerBase - controller without the view support (for views use Controller)
    [ApiController] //like @RestController (gives you http specific features like attribute routing and 400 when something is wrong with the model )
    [Route("api/[controller]")] //api/Character (suffix is automatically removed)
    public class CharacterController : ControllerBase
    {
        private static Character knight = new Character();

        //IACTIONRESULT - allows you to send the HTTP result + response for the actual request
        //APIController supports naming conventions (Get suffix means in a method name means it will be considered as a GET method)
        //ACTIONRESULT - extension of IACTIONRESULT - allows you to say which 'types' will be returned (helps with Swagger)
        [HttpGet]
        public ActionResult<Character> Get()
        {
            return Ok(knight); //http 200 + the mock character (OK from ControllerBase)
        }
    }
}