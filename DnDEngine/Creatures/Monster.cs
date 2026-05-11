using System;
using System.Collections.Generic;

namespace DnD5eBattleApp
{ 
    public class Monster : Creature
    {
        public float cr;
        public int xp;

        public Monster()
        {
        }

        public string GoblinNameGenerator()
        {
            string[] names = new string[] { "Gobbo", "GibGob", "Gablu", "GibbyGob", "GabbyGoo", "GibGab", "GobbyGob", "GobbyGoo", "Gombini", "GabGab" };
            return names[new Random().Next(0, names.Length)];
        }

        public override bool IsProficientInWeapon(Weapon weapon)
        {
            return true;
        }
    }
}