using System;

namespace DnD5eBattleApp;

/// <summary>
/// Conditions represent a subset of Feats which are temporary, and are removed
/// after specific criteria are met.
/// </summary>
public class Condition : Feat
{
    public Condition() : base()
    {
        throw new NotImplementedException("Deprecated Feat usage");
    }
    public Condition(Creature owner, FeatSpec spec) : base(owner, spec)
    {
    }
}