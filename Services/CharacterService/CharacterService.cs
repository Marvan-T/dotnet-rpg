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


        // This is how you make a method asynchronous async Task<ReturnType>, have to add await in the method call (see controllera)
        public async Task<List<Character>> CreateCharacter(Character newCharacter)
        {
            characters.Add(newCharacter);
            return characters;
        }

        public async Task<Character> GetSingleCharacter(int id)
        {
            //! -  null forgiving character
           var character =  characters.FirstOrDefault(x => x.Id == id);
        
            if (character is not null)
                return character;
            
            throw new KeyNotFoundException($"Character with id:{id} not found");
        }

        public async Task<List<Character>> GetAllCharacters()
        {
            return characters;
        }

    }
}