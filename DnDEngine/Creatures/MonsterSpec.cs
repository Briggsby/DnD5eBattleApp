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
    public required string SubType {get; init;}
    public required Alignment Alignment {get; init;}
    public required int ArmorClass {get; init;}
    public string Armor { get; init; } = "Natural Armor";
    public required int HitDiceNumber {get; init;}
    public required int HitDiceValue {get; init;}
    public required int Speed {get; init;}
    public required StatsSpec Stats {get; init;}

    public List<Skills> SkillProficiencies {get; init;} = new List<Skills>();
    public int Darkvision {get; init;} = 0;
    public required List<string> Languages {get; init;}

    public required float CR {get; init;}

    public required List<string> Features {get; init;}
    public required List<string> Equipment {get; init;}
    
}