using System;
using System.Collections.Generic;

namespace DnD5eBattleApp;

/// <summary>
/// Conditions represent a subset of Feats which are temporary, and are removed
/// after specific criteria are met.
/// </summary>
public class Condition : Feat
{
    public Condition(string name, Creature owner, Dictionary<string, int> valueChanges = null, Dictionary<string, int> valueOverrides = null) : base(name, owner, valueChanges, valueOverrides)
    {
    }
}