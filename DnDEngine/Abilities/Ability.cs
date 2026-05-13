using System;
using System.Diagnostics;

namespace DnD5eBattleApp;

public enum ActionType
{
    Action,
    BonusAction,
    Reaction,
    Free,
    Movement
}


public class Ability
{
    public string Name {get; set;}
    public Targeting Targeting {get; set;}
    public ActionType ActionType {get; set;}
    public virtual Damage BaseDamage {get; set;}
    public string AppliesCondition {get; set;}

    public Ability(string name, Targeting targeting, ActionType actionType, Damage damage, string appliesCondition = null)
    {
        Name = name;
        Targeting = targeting;
        ActionType = actionType;
        BaseDamage = damage;
        AppliesCondition = appliesCondition;
    }

    public void Use(Creature user)
    {
        if (Targeting.TargetType != TargetType.Self)
        {
            Order.NewOrder(user, this);
        }
    }

    public virtual void SpendResources(Creature user, Order order = null)
    {
        if (ActionType == ActionType.Action)
        {
            user.actionTaken = true;
        } else if (ActionType == ActionType.BonusAction)
        {
            user.bonusActionTaken = true;
        } else if (ActionType == ActionType.Reaction)
        {
            user.reactionTaken = true;
        }
    }

    public virtual void SelectionMade(Order order)
    {
        SpendResources(order.Creature, order);
        if (Targeting.HasAttackRoll)
        {
            // TODO: The whole attack -> attack roll -> damage roll -> damage flow can be simplified and cleaned up
            // and made to ONLY happen from an Ability (All 'attack' feats can be subclasses of abilities)
            new Attack(order.Creature, order.Selection.creature, this);
        }
    }

    public virtual int GetAttackBonus(Creature user)
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
                spec.ToCondition(roll.attack.defender);
            }
            DamageRoll damageRoll = GetDamageRoll(roll.attack);
            damageRoll.DoRoll();
        }
    }

    public virtual DamageRoll GetDamageRoll(Attack attack)
    {
        return new DamageRoll(
            attack.attacker,
            this,
            attack.defender,
            BaseDamage,
            attack.attacker.encounter,
            new RollDelegate(FinishDamageRoll)
        );
    }

    public void FinishDamageRoll(Roll roll, RollEventArgs e)
    {
        DamageRoll damageRoll = roll as DamageRoll;
        Console.WriteLine(string.Format("{0} did {1} ({2} + {3}) {4} damage to {5} with {6}", 
            roll.roller.name, 
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