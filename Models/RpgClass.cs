using System.Text.Json.Serialization;
namespace dotnet_rpg.Models
{
    // specifying that it should be serialized as a string instead of an integer when converting to JSON
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass
    {
        Knight = 1,
        Mage = 2,
        Cleric = 3
    }
}