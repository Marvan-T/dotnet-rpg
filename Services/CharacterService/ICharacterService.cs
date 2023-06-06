using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        public Task<ServiceResponse<GetCharacterResponseDto>> GetSingleCharacter(int id);

        public Task<ServiceResponse<List<GetCharacterResponseDto>>> CreateCharacter(AddCharacterRequestDto newCharacter);

        public Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacters();
    }
}