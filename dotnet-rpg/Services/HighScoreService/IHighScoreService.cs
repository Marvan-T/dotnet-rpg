using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.HighScoreService;

public interface IHighScoreService
{
    Task<ServiceResponse<List<GetHighScoreDto>>> GetCharactersByScore();
}