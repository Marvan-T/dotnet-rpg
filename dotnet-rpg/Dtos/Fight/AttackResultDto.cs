namespace dotnet_rpg.Dtos.Fight;

public class AttackResultDto
{
    public string Attacker { get; set; }
    public string Opponent { get; set; }
    public int AttackerHp { get; set; }
    public int OpponentHp { get; set; }
    public int DamageDealt { get; set; }
}