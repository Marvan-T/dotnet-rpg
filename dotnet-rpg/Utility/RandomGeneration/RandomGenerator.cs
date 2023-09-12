using dotnet_rpg.Utility.RandomGeneration;

public class RandomGenerator : IRandomGenerator
{
    private readonly Random _random = new();

    public int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }
}