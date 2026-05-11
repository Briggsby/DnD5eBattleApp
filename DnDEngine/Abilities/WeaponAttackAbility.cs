namespace DnD5eBattleApp;

class WeaponAttackAbility : Ability {

    public override Damage Damage {
        get {
            return new Damage{
                DamageType= Weapon.damageTypes[0],
                NumberOfDice= Weapon.damageDiceNumber[0],
                MaxValueOfDice= Weapon.damageDice[0]
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
}