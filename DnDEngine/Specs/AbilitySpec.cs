using System;

namespace DnD5eBattleApp;


[AttributeUsage(AttributeTargets.Property)]
public class SchemaRefAttribute : Attribute
{
    public SchemaRefAttribute(string schemaRef) => SchemaRef = schemaRef;
    public string SchemaRef { get; }
}


public record AbilitySpec
{
    public required string Name {get; init;}
    public required Targeting Targeting {get; init;}
    public required ActionType ActionType {get; init;}
    public Damage Damage {get; init;}

    [SchemaRef("ConditionName.schema.json")]
    public string AppliesCondition {get; init;}

    public virtual Ability ToAbility()
    {
        return new Ability(Name, Targeting, ActionType, Damage, AppliesCondition);
    }
}