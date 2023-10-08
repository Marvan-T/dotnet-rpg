using dotnet_rpg.Services.HighScoreService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[Authorize]
[ApiController]
[Route("/score")]
public class ScoreController : ControllerBase
{
    private readonly IHighScoreService _highScoreService;

    public ScoreController(IHighScoreService highScoreService)
    {
        _highScoreService = highScoreService;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> GetCharacterScores()
    {
        var response = await _highScoreService.GetCharactersByScore();
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}