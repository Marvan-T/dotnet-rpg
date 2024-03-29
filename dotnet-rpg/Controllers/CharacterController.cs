using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

//All controllers derive from ControllerBase - controller without the view support (for views use Controller)
[Authorize] // Secures the complete controller (there is an alternative as well) -- See GetSingleCharacter
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
    // Task - represents an asynchronous operation that can return a value
    public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> Get()
    {
        // /* Claims could be access by the ClaimsPrincipal (authenticated user), this made available through ControllerBase.
        // It is populated based on the claims that we define during authentication.. NameIdentifier claim represents a unique user identifier within the system  */
        // var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _characterService.GetAllCharacters()); //http 200 + the mock characters (OK from ControllerBase)
    }

    // [AllowAnonymous] - to make this API open, however, this would also mean that the User won't be populated
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterResponseDto>>> GetCharacterById(int id)
    {
        return Ok(await _characterService.GetCharacterById(id));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> CreateCharacter(
        AddCharacterRequestDto newCharacter)
    {
        return Ok(await _characterService.CreateCharacter(newCharacter));
        // return CreatedAtAction(nameof(GetSingleCharacter), new {id = newCharacter.Id}, newCharacter); //Returns the newly created character + URI (location header) 
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetCharacterResponseDto>>> UpdateCharacter(
        UpdateCharacterRequestDto updatedCharacter)
    {
        var response = await _characterService.UpdateCharacter(updatedCharacter);
        if (response.Data is null) return NotFound(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> DeleteCharacter(int id)
    {
        var response = await _characterService.DeleteCharacter(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}