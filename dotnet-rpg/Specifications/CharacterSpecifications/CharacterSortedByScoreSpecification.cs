using System.Linq.Expressions;
using dotnet_rpg.Specifications.Interfaces;

namespace dotnet_rpg.Specifications.CharacterSpecifications;

public class CharacterSortedByScoreSpecification : ISpecification<Character>
{
    public Expression<Func<Character, bool>> Criteria => character => character.Fights > 0;
    // public List<Expression<Func<Character, object>>> Includes { get; } = new()
    // {
    //     character => character.Weapon,
    //     character => character.Skills
    // };
    public List<Expression<Func<Character, object>>> Includes { get; } = new();
    public Func<IQueryable<Character>, IOrderedQueryable<Character>> OrderBy => 
        charactersQuery => charactersQuery.OrderByDescending(c => c.Victories).ThenByDescending(c => c.Defeats);
}