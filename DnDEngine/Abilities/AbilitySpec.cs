namespace DnD5eBattleApp;

public record AbilitySpec
{
    public required string Name {get; init;}
    public required Targeting Targeting {get; init;}
    public required ActionType ActionType {get; init;}
    public Damage Damage {get; init;}
    public string AppliesCondition {get; init;}
}