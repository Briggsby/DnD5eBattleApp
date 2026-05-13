using System;

namespace DnD5eBattleApp;

class WeaponAttackAbility : Ability {

    public override Damage BaseDamage {
        get {
            return new Damage{
                DamageType= Weapon.damageTypes[0],
                NumberOfDice= Weapon.damageDiceNumber[0],
                MaxValueOfDice= Weapon.damageDice[0],
                FlatValue= Weapon.damageBonus
            };
        }
    }
    public Weapon Weapon {get; set;}

    public WeaponAttackAbility(Weapon weapon) : base(weapon.name, new Targeting{
        TargetType= TargetType.SingleTargetRanged,
        Range= weapon.maxRange
    }, ActionType.Action, null, null) {
        Weapon = weapon;
    }

    public override int GetAttackBonus(Creature user)
    {
        return Weapon.attackBonus + user.StatMod(Weapon.AbilityStat) + (user.IsProficientInWeapon(Weapon) ? user.proficiencyBonus : 0);
    }

    public override int GetDamageBonus(Creature user)
    {
        return Math.Max(0, Weapon.damageBonus + user.StatMod(Weapon.AbilityStat));
    }
}