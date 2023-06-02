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
        
        private readonly ICharacterService _characterService;
        
        // ctor - snippet to create the constructor 
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        //IACTIONRESULT - allows you to send the HTTP result + response for the actual request
        //APIController supports naming conventions (Get suffix means in a method name means it will be considered as a GET method)
        //ACTIONRESULT - extension of IACTIONRESULT - allows you to say which 'types' will be returned (helps with Swagger)
        [HttpGet("GetAll")]
        // [Route("GetAll")] - Same thing as above
        public ActionResult<List<Character>> Get()
        {
            return Ok(_characterService.GetAllCharacters()); //http 200 + the mock character (OK from ControllerBase)
        }


        [HttpGet("{id}")]
        public ActionResult<Character> GetSingleCharacter(int id)
        {
            return Ok(_characterService.GetSingleCharacter(id));
        }

        [HttpPost]
        public ActionResult<List<Character>> CreateCharacter(Character newCharacter)
        {
            return Ok(_characterService.CreateCharacter(newCharacter));
            // return CreatedAtAction(nameof(GetSingleCharacter), new {id = newCharacter.Id}, newCharacter); //Returns the newly created character + URI (location header) 
        }
    }
}