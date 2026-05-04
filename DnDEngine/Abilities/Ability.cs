using System;
using System.Diagnostics;

namespace DnD5eBattleApp;

public enum ActionType
{
    Action,
    BonusAction,
    Reaction,
    Free
}


public class Ability
{
    public Creature Owner {get; set;}
    public string Name {get; set;}
    public Targeting Targeting {get; set;}
    public ActionType ActionType {get; set;}
    public Damage Damage {get; set;}
    public string AppliesCondition {get; set;}

    public Ability(AbilitySpec abilitySpec, Creature owner)
    {
        Owner = owner;
        Name = abilitySpec.Name;
        Targeting = abilitySpec.Targeting;
        ActionType = abilitySpec.ActionType;
        Damage = abilitySpec.Damage;
        AppliesCondition = abilitySpec.AppliesCondition;
    }

    public void Use()
    {
        if (Targeting.TargetType == TargetType.SingleTargetRanged)
        {
            new SingleNormalTargetOrder(Owner, this, Targeting.Range);
        }
    }

    public virtual void SpendResources()
    {
        if (ActionType == ActionType.Action)
        {
            Owner.actionTaken = true;
        } else if (ActionType == ActionType.BonusAction)
        {
            Owner.bonusActionTaken = true;
        } else if (ActionType == ActionType.Reaction)
        {
            Owner.reactionTaken = true;
        }
    }

    public void SelectionMade(Control order)
    {
        SpendResources();
        if (Targeting.HasAttackRoll)
        {
            // TODO: The whole attack -> attack roll -> damage roll -> damage flow can be simplified and cleaned up
            // and made to ONLY happen from an Ability (All 'attack' feats can be subclasses of abilities)
            new Attack(order.creature, order.orderControl.selection.creature, this);
        }
    }

    public virtual int GetAttackBonus()
    {
        // TODO: Implement bonuses for non-spell Abilities
        return 0;
    }

    public void FinishAttackRoll(AttackRoll roll, RollEventArgs e)
    {
        Console.WriteLine(string.Format("{0} {1} {2} with an attack with an attack roll of {3} ({4} + {5}) against an AC of {6} {7}", 
            roll.attack.attacker.name, 
            roll.Success ? "hit" : "missed",
            roll.attack.defender.name, 
            roll.score, 
            roll.score - roll.bonus,
            roll.bonus, 
            roll.attack.defender.AC,
            roll.WithAdvantagePrint()
        ));
        if (roll.Success)
        {
            if (AppliesCondition is not null && DnDManager.TryGetResource(AppliesCondition, out ConditionSpec spec)) {
                new Condition(roll.attack.defender, spec);
            }
            DamageRoll damageRoll = new DamageRoll(
                Owner,
                this,
                roll.attack.defender,
                Damage,
                Owner.encounter,
                new RollDelegate(FinishDamageRoll)
            );
            damageRoll.DoRoll();
        }
    }

    public void FinishDamageRoll(Roll roll, RollEventArgs e)
    {
        DamageRoll damageRoll = roll as DamageRoll;
        Console.WriteLine(string.Format("{0} did {1} ({2} + {3}) {4} damage to {5} with {6}", 
            Owner.name, 
            damageRoll.score,
            damageRoll.score-damageRoll.bonus, 
            damageRoll.bonus, 
            damageRoll.damageTypes[0],
            damageRoll.Target.name,
            this.Name
        ));
        damageRoll.Target.TakeDamage(damageRoll.scoresPerDie, damageRoll.damageTypes, this);
    }
}