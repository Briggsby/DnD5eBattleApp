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
    public string Name {get; set;}
    public Targeting Targeting {get; set;}
    public ActionType ActionType {get; set;}
    public Damage Damage {get; set;}
    public string AppliesCondition {get; set;}

    public Ability(AbilitySpec abilitySpec)
    {
        Name = abilitySpec.Name;
        Targeting = abilitySpec.Targeting;
        ActionType = abilitySpec.ActionType;
        Damage = abilitySpec.Damage;
        AppliesCondition = abilitySpec.AppliesCondition;
    }

    public void Use(Creature user)
    {
        if (Targeting.TargetType == TargetType.SingleTargetRanged)
        {
            new SingleNormalTargetOrder(user, this, Targeting.Range);
        }
    }

    public virtual void SpendResources(Creature user)
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

    public void SelectionMade(Control order)
    {
        SpendResources(order.creature);
        if (Targeting.HasAttackRoll)
        {
            // new Attack(order.creature, order.orderControl.selection.creature, this);
        }
    }

}