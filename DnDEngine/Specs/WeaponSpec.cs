using System.Collections.Generic;

namespace DnD5eBattleApp;

public record WeaponSpec
{
    public required string Name {get; init;}
    public required Damage Damage {get; init;}
    public required WeaponCategory Category {get; init;}
    public required List<WeaponProperty> Properties {get; init;} = new List<WeaponProperty>();
    public int Range {get; init;}
    public int MaxRange {get; init;}

    public Weapon ToWeapon()
    {
        return new Weapon(
            Name, 
            Category, 
            new List<int>() { Damage.MaxValueOfDice }, 
            new List<int>() { Damage.NumberOfDice }, 
            new List<string>() { Damage.DamageType }, 
            Properties.Contains(WeaponProperty.Range) ? Stat.Dexterity : Stat.Strength,
            Range, 
            MaxRange, 
            Properties 
        );
    }
}