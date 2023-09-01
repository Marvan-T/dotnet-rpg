# Note about exceptions

From Microsoft:

```text
Minimize exceptions
Exceptions should be rare. Throwing and catching exceptions is slow relative to other code flow patterns. Because of this, exceptions shouldn't be used to control normal program flow.

Recommendations:

1. Do not use throwing or catching exceptions as a means of normal program flow, especially in hot code paths.
2. Do include logic in the app to detect and handle conditions that would cause an exception.
3. Do throw or catch exceptions for unusual or unexpected conditions.

App diagnostic tools, such as Application Insights, can help to identify common exceptions in an app that may affect performance.
```

Based on the above it is good idea to consider refactoring some of hot code paths that we have. Especially the path that involve adding Skills to Characters

```csharp
  public async Task<ServiceResponse<GetCharacterResponseDto>> AddSkillToCharacter(
        AddCharacterSkillDto addCharacterSkillDto)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

        try
        {
            var character = await FindCharacter(addCharacterSkillDto.CharacterId);
            var skill = await FindSkill(addCharacterSkillDto.SkillId);
            AddCharacterSkill(character, addCharacterSkillDto.SkillId, skill);
            await _characterRepository.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }

    private async Task<Character> FindCharacter(int characterId)
    {
        var character = await _characterRepository.GetByIdAsync(characterId);
        if (character == null) throw new CharacterNotFoundException(characterId);
        return character;
    }

    private async Task<Skill> FindSkill(int skillId)
    {
        var skill = await _skillRepository.GetByIdAsync(skillId);
        if (skill == null) throw new SkillNotFoundException(skillId);
        return skill;
    }

    private void AddCharacterSkill(Character character, int skillId, Skill skill)
    {
        if (character.Skills.Any(s => s.Id == skillId))
            throw new InvalidOperationException(
                $"Skill with ID {skillId} is already added to Character with ID {character.Id}");
        character.Skills.Add(skill);
    }
```

There are 2 proposed approaches that you can use

1. Using the result pattern
2. Using the DDD (domain driven design)


## Using the result pattern 

The "Result" pattern could be used by using a generic class named Result<T> or ServiceResult<T> where T can be the object of desire (Character, Skill) if found or null if not:

```csharp
public class ServiceResult<T> where T : class
{
    public T Data { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
}
```

Now, instead of throwing exceptions or returning null, you can return a ServiceResult from your helper methods like the following:

```csharp
private async Task<ServiceResult<Character>> FindCharacter(int characterId)
{
    var character = await _characterRepository.GetByIdAsync(characterId);

    return character != null 
        ? new ServiceResult<Character> { Data = character, IsSuccess = true }
        : new ServiceResult<Character> { ErrorMessage = $"Character with ID {characterId} not found", IsSuccess = false };
}

private async Task<ServiceResult<Skill>> FindSkill(int skillId)
{
    var skill = await _skillRepository.GetByIdAsync(skillId);

    return skill != null 
        ? new ServiceResult<Skill> { Data = skill, IsSuccess = true } 
        : new ServiceResult<Skill> { ErrorMessage = $"Skill with ID {skillId} not found", IsSuccess = false };
}
```

This will allow your AddSkillToCharacter method to remain clean and avoid redundancy:

```csharp

public async Task<ServiceResponse<GetCharacterResponseDto>> AddSkillToCharacter(
    AddCharacterSkillDto addCharacterSkillDto)
{
    var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();
    var characterResult = await FindCharacter(addCharacterSkillDto.CharacterId);
    var skillResult = await FindSkill(addCharacterSkillDto.SkillId);

    if (!characterResult.IsSuccess)
    {
        serviceResponse.Success = false;
        serviceResponse.Message = characterResult.ErrorMessage;
        return serviceResponse;
    }

    if (!skillResult.IsSuccess)
    {
        serviceResponse.Success = false;
        serviceResponse.Message = skillResult.ErrorMessage;
        return serviceResponse;
    }

    if (characterResult.Data.Skills.Any(s => s.Id == skillResult.Data.Id))
    {
        serviceResponse.Success = false;
        serviceResponse.Message = $"Skill with ID {skillResult.Data.Id} is already added to Character with ID {characterResult.Data.Id}";
        return serviceResponse;
    }

    characterResult.Data.Skills.Add(skillResult.Data);
    await _characterRepository.SaveChangesAsync();
    serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(characterResult.Data);

    return serviceResponse;
}
```

Here, instead of raising exceptions, we return a ServiceResult which contains the data (Character or Skill), whether the operation was successful (IsSuccess), and in case of failure, it contains an error message (ErrorMessage).

- This helps to encapsulate the runtime state of executing a task, including whether the operation was successful, a result if it was, and what error information on failure.

## However, note this well!

- Preoptimization is not always desirable, and it is often better to first ensure code quality in terms of readability and maintainability before considering optimization.

- Code correctness and readability often precede performance considerations unless you're working in areas where performance is critical. If the current design actually leads to a performance problem in future, then a change may need to be considered.


## Other patterns to consider:

- **Notification Pattern:** Instead of returning immediatelly when an error is found, collect all issues and return them together at once. This can be very helpful in scenarios where you want to validate multiple conditions and return all validation errors together.
- **Domain-Driven Design (DDD) pattern:** Rather than directly adding a skill to a character, you could craft methods that encapsulate the rules of the domain. For example, Character could have a method like "AssignSkill(Skill skill)" which checks if the skill is already assigned and only adds it if it's not. This way, the domain rules live in the domain objects and not in the service layer.
- **Optional Pattern:** Rather than returning null, methods would return an "Optional" object, which would signify that the method may or may not return a value. This can make null checks unnecessary and helps avoid NullReferenceException.
- **Retry Pattern:** In case of transient failures like temporary network issues or database unavailability, rather than failing the complete operation, the system can retry the operation considering it would eventually pass after a certain number of attempts.
- **Circuit Breaker Pattern:** If a system is failing continuously, this pattern suggests to stop calling the failing system for a while (open the circuit breaker), let it recover and then begin calling it again (close the circuit breaker). This pattern can be often seen in the microservice architecture based systems.

These are some patterns which are widely used in industry, but whether to use them or not should be based on the particular use-case and the problem you are trying to solve. No pattern is a silver bullet and we should always weigh the pros and cons before using them.