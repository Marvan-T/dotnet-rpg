using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        public Character GetSingleCharacter(int id);

        public List<Character> CreateCharacter(Character newCharacter);

        public List<Character> GetAllCharacters();
    }
}