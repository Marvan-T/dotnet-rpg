# How to access the `UserId`

There are 2 ways of doing this. 

1. Using the `User`` object which is of type `ClaimsPrincipal` from the controllers
```csharp
    public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> Get()
    {
        /* Claims could be access by the ClaimsPrincipal (authenticated user), this made available through ControllerBase.
        It is populated based on the claims that we define during authentication.. NameIdentifier claim represents a unique user identifier within the system  */
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _characterService.GetAllCharacters(userId)); 
    }
```

2. You can inject a dependency called `IHttpContextAccessor` in the services. This prevents the need to pass the userId's from Controllers to Services

```csharp
public class CharacterService : ICharacterService
{
    private readonly IHttpContextAccessor _iHttpContextAccessor;


    public CharacterService(IHttpContextAccessor iHttpContextAccessor)
    {
        _iHttpContextAccessor = iHttpContextAccessor;
    }

    private int GetUserId() => int.Parse(_iHttpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
```

Remember to register this as a dependency in `Program.cs`
```csharp
builder.Services.AddHttpContextAccessor();
```