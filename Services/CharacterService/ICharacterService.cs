using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        public Task<Character> GetSingleCharacter(int id);

        public Task<List<Character>> CreateCharacter(Character newCharacter);

        public Task<List<Character>> GetAllCharacters();
    }
}