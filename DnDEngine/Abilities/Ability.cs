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
            new Attack(order.Creature, order.Selection.creature, this);
        }
    }

    public virtual int GetAttackBonus(Creature user)
    {
        return 0;
    }

    public virtual int GetDamageBonus(Creature user)
    {
        return 0;
    }

    public void FinishAttackRoll(AttackRoll roll, RollEventArgs e)
    {
        Console.WriteLine(string.Format("{0} {1} {2} with an attack with an attack roll of {3} ({4} + {5}) against an AC of {6} {7}", 
            roll.attack.Attacker.Name, 
            roll.Success ? "hit" : "missed",
            roll.attack.Defender.Name, 
            roll.score, 
            roll.score - roll.bonus,
            roll.bonus, 
            roll.attack.Defender.AC,
            roll.WithAdvantagePrint()
        ));
        if (roll.Success)
        {
            if (AppliesCondition is not null && DnDManager.TryGetResource(AppliesCondition, out ConditionSpec spec)) {
                spec.ToCondition(roll.attack.Defender);
            }
            DamageRoll damageRoll = GetDamageRoll(roll.attack);
            damageRoll.DoRoll();
        }
    }

    public virtual DamageRoll GetDamageRoll(Attack attack)
    {
        Damage damage = BaseDamage;
        damage.FlatValue += GetDamageBonus(attack.Attacker);
        return new DamageRoll(
            attack.Attacker,
            this,
            attack.Defender,
            damage,
            attack.Attacker.Encounter,
            new RollDelegate(FinishDamageRoll),
            attack.AttackRoll.Natural == 20
        );
    }

    public void FinishDamageRoll(Roll roll, RollEventArgs e)
    {
        DamageRoll damageRoll = roll as DamageRoll;
        Console.WriteLine(string.Format("{0} did {1} ({2} + {3}) {4} damage to {5} with {6}", 
            roll.roller.Name, 
            damageRoll.score,
            damageRoll.score-damageRoll.bonus, 
            damageRoll.bonus, 
            damageRoll.damageTypes[0],
            damageRoll.Target.Name,
            this.Name
        ));
        damageRoll.Target.TakeDamage(damageRoll.scoresPerDie, damageRoll.damageTypes, this);
    }
}