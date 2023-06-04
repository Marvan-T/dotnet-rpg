using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        public Task<ServiceResponse<Character>> GetSingleCharacter(int id);

        public Task<ServiceResponse<List<Character>>> CreateCharacter(Character newCharacter);

        public Task<ServiceResponse<List<Character>>> GetAllCharacters();
    }
}