using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Dtos.Character;

public class UpdateCharacterRequestDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "Jerry"; 

    public int HitPoints { get; set; } = 100;

    public int Strength { get; set; } = 10;

    public int Defense { get; set; }  = 10;

    public RpgClass Class { get; set; }  = RpgClass.Knight;
}