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

        public Monster(MonsterSpec spec)
        {
            baseStats = new BaseStats(spec, this, true);
            name = spec.Name;
            spellbook = new SpellBook(spec.Spells, this);
            // TODO: Add Feats
            // TODO: Handle Textures
            Texture = DnDManager.monsterTextures[SRDLibrary.Monsters.Goblin.ToString()];
            // TODO: Handle Armor? Is it even needed

            var weapons = new List<Item>();
            foreach (string weaponName in spec.Equipment)
            {
                if (DnDManager.weapons.ContainsKey(weaponName))
                {
                    weapons.Add(new Weapon(DnDManager.weapons[weaponName]));
                }
                else
                {
                    Console.WriteLine($"Error: Weapon {weaponName} not found in DnDManager.weapons");
                }
            }
            inventory = new Inventory(this, weapons);
            RecalibrateStats();
            ResetHP();

            if (spec.Name == "Goblin")
            {
                name = GoblinNameGenerator();
            }
        }

        public string GoblinNameGenerator()
        {
            string[] names = new string[] { "Gobbo", "GibGob", "Gablu", "GibbyGob", "GabbyGoo", "GibGab", "GobbyGob", "GobbyGoo", "Gombini", "GabGab" };
            return names[new Random().Next(0, names.Length)];
        }

        public static Monster SpawnMonster(MonsterSpec spec, BoardTile tile)
        {
            Monster creature = new Monster(spec);
            creature.SetInitialTile(tile);
            creature.baseStats.RollHP();
            creature.ResetHP();
            creature.RecalibrateStats();
            creature.StartTurn();
            return creature;
        }
    }
}