using System.Collections.Generic;

namespace DnD5eBattleApp;

public record WeaponSpec
{
    public required string Name {get; init;}
    public required string Damage {get; init;}
    public required string DamageType {get; init;}
    public required WeaponCategory Category {get; init;}
    public required List<WeaponProperty> Properties {get; init;}
    public int Range {get; init;}
    public int MaxRange {get; init;}

    public Weapon MakeWeapon()
    {
        // TODO
        return new Weapon();
    }
}