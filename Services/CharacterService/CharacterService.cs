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

        private readonly IMapper _mapper;
        private readonly DataContext _context;
    

        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // This is how you make a method asynchronous async Task<ReturnType>, have to add await in the method call (see controller)
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
           var character =  await _context.Characters.FirstOrDefaultAsync(x => x.Id == id);
        
           serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);

           return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
            var dbCharacters = await _context.Characters.ToListAsync(); // Accessing Characters table (remember `Characters` needs to be defined in the DataContext.cs)
            serviceResponse.Data = dbCharacters.Select(x => _mapper.Map<GetCharacterResponseDto>(x)).ToList<GetCharacterResponseDto>();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

            try {
                var character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);

                if (character is null)
                    throw new Exception($"Character with id:{updatedCharacter.Id} not found");

                _mapper.Map(updatedCharacter, character);

                // character.Name = updatedCharacter.Name;
                // character.HitPoints = updatedCharacter.HitPoints;
                // character.Strength = updatedCharacter.Strength;
                // character.Defense = updatedCharacter.Defense;
                // character.Class = updatedCharacter.Class;

                serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
            } catch (Exception e) {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();

            try {
                var character = characters.FirstOrDefault(c => c.Id == id); // First can be used but it returns an exception when the character is not found
                if (character is null) throw new Exception($"Character with id: {id} not found");

                characters.Remove(character);
                serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterResponseDto>(c)).ToList();

            } catch (Exception e) {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }
    }
}