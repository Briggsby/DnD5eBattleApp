namespace DnD5eBattleApp;

class WeaponAttackAbility : Ability {

    public override Damage Damage {
        get {
            return new Damage{
                DamageType= Weapon.damageTypes[0],
                NumberOfDice= Weapon.damageDiceNumber[0],
                MaxValueOfDice= Weapon.damageDice[0],
                FlatValue= Owner.StatMod(Weapon.AbilityStat) + Weapon.damageBonus
            };
        }
    }
    public Weapon Weapon {get; set;}

    public WeaponAttackAbility(Weapon weapon, Creature owner) : base(weapon.name, owner, new Targeting{
        TargetType= TargetType.SingleTargetRanged,
        Range= weapon.maxRange
    }, ActionType.Action, null, null) {
        Weapon = weapon;
    }

    public override int GetAttackBonus()
    {
        return Weapon.attackBonus + Owner.StatMod(Weapon.AbilityStat) + (Owner.IsProficientInWeapon(Weapon) ? Owner.proficiencyBonus : 0);
    }


}