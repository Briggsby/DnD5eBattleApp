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
}