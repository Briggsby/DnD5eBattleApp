using System;

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
            // TODO: Add Feats
            // TODO: Handle Textures
            Texture = DnDManager.monsterTextures[SRDLibrary.Monsters.Goblin.ToString()];
            // TODO: Handle Armor? Is it even needed
            // TODO: Handle weapons and equipment
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