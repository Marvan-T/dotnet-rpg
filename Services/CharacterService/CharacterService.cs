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
        public async Task<ServiceResponse<List<Character>>> CreateCharacter(Character newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<Character>>();
            characters.Add(newCharacter);
            serviceResponse.Data = characters;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Character>> GetSingleCharacter(int id)
        {
           var serviceResponse = new ServiceResponse<Character>();

            //! -  null forgiving character
           var character =  characters.FirstOrDefault(x => x.Id == id);
        
            serviceResponse.Data = character;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<Character>>();
            serviceResponse.Data = characters;
            return serviceResponse;
        }

    }
}