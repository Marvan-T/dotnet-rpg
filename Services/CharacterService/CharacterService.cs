using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{

    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character> {
            new Character(),
            new Character { Id = 1,  Name = "Sam" }
        };
        public IMapper _mapper { get; }

        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        // This is how you make a method asynchronous async Task<ReturnType>, have to add await in the method call (see controllera)
        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> CreateCharacter(AddCharacterRequestDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.Id = characters.Max(x => x.Id) + 1;
            characters.Add(character);
            serviceResponse.Data = characters.Select(x => _mapper.Map<GetCharacterResponseDto>(x)).ToList<GetCharacterResponseDto>();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterResponseDto>> GetSingleCharacter(int id)
        {
           var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

            //! -  null forgiving character
           var character =  characters.FirstOrDefault(x => x.Id == id);
        
           serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);

           return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
            serviceResponse.Data = characters.Select(x => _mapper.Map<GetCharacterResponseDto>(x)).ToList<GetCharacterResponseDto>();
            return serviceResponse;
        }

    }
}