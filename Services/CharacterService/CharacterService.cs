using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _iHttpContextAccessor;


    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor iHttpContextAccessor)
    {
        _mapper = mapper;
        _context = context;
        _iHttpContextAccessor = iHttpContextAccessor;
    }

    private int GetUserId() => int.Parse(_iHttpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // This is how you make a method asynchronous async Task<ReturnType>, have to add await in the method call (see controller)
    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> CreateCharacter(AddCharacterRequestDto newCharacter)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
        var character = _mapper.Map<Character>(newCharacter);
        character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

        //approach 1    
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        //approach 2 - , it's important to note that this AddAsync method doesn't make any database operations, it just provides an async way to add the entity to the DbContext
        /*
        There is not much benefit in using AddAsync over Add because the AddAsync method does not involve any I/O bound work, 
        it just prepares the data to be saved and this is a fast operation. 
        Async methods are mainly beneficial when there are I/O operations involved, like when calling SaveChangesAsync, 
        which does interact with the database.
        */
        // await _context.Characters.AddAsync(character);
        // await _context.SaveChangesAsync();

        serviceResponse.Data = await _context.Characters
            .Where(c => c.UserId == GetUserId())
            .Select(x => _mapper.Map<GetCharacterResponseDto>(x))
            .ToListAsync();
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
        var dbCharacters = await _context.Characters.Where(c => c.UserId == GetUserId()).ToListAsync(); // Accessing Characters table (remember `Characters` needs to be defined in the DataContext.cs)
        serviceResponse.Data = dbCharacters.Select(x => _mapper.Map<GetCharacterResponseDto>(x)).ToList<GetCharacterResponseDto>();
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

        try {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

            if (character is null)
                throw new Exception($"Character with id:{updatedCharacter.Id} not found");

            _mapper.Map(updatedCharacter, character);

            // character.Name = updatedCharacter.Name;
            // character.HitPoints = updatedCharacter.HitPoints;
            // character.Strength = updatedCharacter.Strength;
            // character.Defense = updatedCharacter.Defense;
            // character.Class = updatedCharacter.Class;

            await _context.SaveChangesAsync();
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
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id) ?? throw new Exception($"Character with id: {id} not found"); // First can be used but it returns an exception when the character is not found

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterResponseDto>(c)).ToListAsync();

        } catch (Exception e) {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }
}