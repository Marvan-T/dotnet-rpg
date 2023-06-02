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

        public List<Character> CreateCharacter(Character newCharacter)
        {
            characters.Add(newCharacter);
            return characters;
        }

        public Character GetSingleCharacter(int id)
        {
           return characters.FirstOrDefault(x => x.Id == id);
        }

        public List<Character> GetAllCharacters()
        {
            return characters;
        }

    }
}