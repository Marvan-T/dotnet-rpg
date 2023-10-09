using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.Helper;
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
            var characters = await FetchCharactersSortedByScore();
            serviceResponse.Data = _mapper.Map<List<GetHighScoreDto>>(characters);
        }
        catch (Exception e)
        {
            ServiceResponseHelper.HandleServiceException(serviceResponse, e);
        }
        return serviceResponse;
    }
    
    private async Task<IReadOnlyList<Character>> FetchCharactersSortedByScore()
    {
        var spec = new CharacterSortedByScoreSpecification();
        return await _characterRepository.ListAsync(spec);
    }
}