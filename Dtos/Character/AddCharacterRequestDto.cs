using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Dtos.Character
{
    public class AddCharacterRequestDto
    {

        public string Name { get; set; } = "Jerry"; //specifying defaults (you can use ? to make it nullable)

        public int HitPoints { get; set; } = 100;

        public int Strength { get; set; } = 10;

        public int Defense { get; set; }  = 10;

        public RpgClass Class { get; set; }  = RpgClass.Knight;
    }
}