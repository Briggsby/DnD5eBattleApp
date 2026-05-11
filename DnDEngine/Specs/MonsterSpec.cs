using System;
using System.Collections.Generic;

namespace DnD5eBattleApp;

public record StatsSpec
{
    public required int Strength {get; init;}
    public required int Dexterity {get; init;}
    public required int Constitution {get; init;}
    public required int Intelligence {get; init;}
    public required int Wisdom {get; init;}
    public required int Charisma {get; init;}
}

public record MonsterSpec
{
    public required string Name {get; init;}
    public required Size Size {get; init;}
    public required string Type {get; init;}
    public string SubType {get; init;}
    public required Alignment Alignment {get; init;}
    public required int ArmorClass {get; init;}
    public string Armor { get; init; } = "Natural Armor";
    public required int HitDiceNumber {get; init;}
    public required int HitDiceValue {get; init;}
    public required int Speed {get; init;}
    public required StatsSpec Stats {get; init;}

    public required int ProficiencyBonus {get; init;}
    public List<Stat> SavingThrowProficiencies {get; init;} = new List<Stat>();
    public List<Skill> SkillProficiencies {get; init;} = new List<Skill>();
    public List<Skill> Expertise {get; init;} = new List<Skill>();
    public int Darkvision {get; init;} = 0;
    public required List<string> Languages {get; init;}

    public required float CR {get; init;}

    public required List<string> Features {get; init;}
    public List<string> Equipment {get; init;} = new List<string>();

    public CreatureValue SpellcastingAbility {get; init;} = CreatureValue.Intelligence;
    public int Truesight {get; init;} = 0;
    public List<DamageType> DamageResistances {get; init;} = new List<DamageType>();
    public List<DamageType> DamageImmunities {get; init;} = new List<DamageType>();
    public List<DamageType> DamageVulnerabilities {get; init;} = new List<DamageType>();
    
    [SchemaRef("ConditionName.schema.json")]
    public List<string> ConditionImmunities {get; init;} = new List<string>();
    
    [SchemaRef("SpellName.schema.json")]
    public List<string> Spells {get; init;} = new List<string>();

    public Dictionary<string, int> LegendaryActions {get; init;} = new Dictionary<string, int>();
    public List<string> Actions {get; init;} = new List<string>();

    public Monster ToMonster()
    {
        var monster = new Monster();
        monster.Speed.SetBaseValue(Speed);
        monster.GetValue<int>(CreatureValue.Strength).SetBaseValue(Stats.Strength);
        monster.GetValue<int>(CreatureValue.Dexterity).SetBaseValue(Stats.Dexterity);
        monster.GetValue<int>(CreatureValue.Constitution).SetBaseValue(Stats.Constitution);
        monster.GetValue<int>(CreatureValue.Intelligence).SetBaseValue(Stats.Intelligence);
        monster.GetValue<int>(CreatureValue.Wisdom).SetBaseValue(Stats.Wisdom);
        monster.GetValue<int>(CreatureValue.Charisma).SetBaseValue(Stats.Charisma);

        monster.baseStats = new BaseStats(this, monster, true);
        monster.name = Name;
        monster.SpellBook = new SpellBook(monster, SpellcastingAbility, Spells);
        monster.oldSpellbook = new OldSpellBook(Spells, monster);
        // TODO: Add Feats
        // TODO: Handle Textures
        monster.Texture = DnDManager.monsterTextures[SRDLibrary.Monsters.Goblin.ToString()];
        // TODO: Handle Armor? Is it even needed

        var weapons = new List<Item>();
        foreach (string weaponName in Equipment)
        {
            if (DnDManager.TryGetResource(weaponName, out WeaponSpec weaponSpec))
            {
                weapons.Add(weaponSpec.ToWeapon());
            }
        }
        monster.inventory = new Inventory(monster, weapons);
        monster.RecalibrateStats();
        monster.ResetHP();

        if (Name == "Goblin")
        {
            monster.name = monster.GoblinNameGenerator();
        }
        return monster;
    }

    
    public Monster SpawnMonster(BoardTile tile)
    {
        Monster creature = ToMonster();
        creature.SetInitialTile(tile);
        creature.baseStats.RollHP();
        creature.ResetHP();
        creature.RecalibrateStats();
        creature.StartTurn();
        return creature;
    }
    
}