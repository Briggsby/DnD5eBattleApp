using System;

namespace DnD5eBattleApp;

// TODO: Move specs to a separate folder, have Specs be responsible for creating the Objects, not vice-versa
// and implement properties on the specs like the following:

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

    public virtual Ability ToAbility(Creature owner)
    {
        return new Ability(Name, owner, Targeting, ActionType, Damage, AppliesCondition);
    }
}