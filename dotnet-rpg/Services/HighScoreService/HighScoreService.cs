using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Specifications.CharacterSpecifications;

namespace dotnet_rpg.Services.HighScoreService;

public class HighScoreService : IHighScoreService
{
    private readonly IRepository<Character> _characterRepository;
    private readonly IMapper _mapper;

    public HighScoreService(IRepository<Character> characterRepository, IMapper mapper)
    {
        _characterRepository = characterRepository;
        _mapper = mapper;
    }
    
    public async Task<ServiceResponse<List<GetHighScoreDto>>> GetCharactersByScore()
    {
        var serviceResponse = new ServiceResponse<List<GetHighScoreDto>>();
        try
        {
            var spec = new CharacterSortedByScoreSpecification();
            var characters = await _characterRepository.ListAsync(spec);
            serviceResponse.Data = _mapper.Map<List<GetHighScoreDto>>(characters);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        return serviceResponse;
    }
}