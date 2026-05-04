using System.Collections.Generic;
using DnD5eBattleApp;

public record ValueModificationSpec
{
    public required string ValueType {get; init;}
    public required int ValueChange {get; init;}
}

public record FeatSpec
{
    public required string Name {get; init; }
    public List<ValueModificationSpec> ValueModifications {get; init;} 
}