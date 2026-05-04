using System;
using System.Collections.Generic;

namespace DnD5eBattleApp;

public record SpellSpec : AbilitySpec
{
    public required int Level {get; init;}
    public required string School {get; init;}
    public required SpellComponent Components {get; init;}
}