using System.Collections.Generic;
using DnD5eBattleApp;

public record ValueModificationSpec
{
    public required CreatureValue ValueType {get; init;}
    public required int ValueChange {get; init;}
}

public record FeatSpec
{
    public required string Name {get; init; }
    public Dictionary<string, int> ValueChanges {get; init;} 

    public Feat ToFeat(Creature owner)
    {
        return new Feat(Name, owner, ValueChanges);
    }
}