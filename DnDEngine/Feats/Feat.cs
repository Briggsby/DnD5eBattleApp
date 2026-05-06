using System;
using System.Collections.Generic;
using BugsbyEngine;

namespace DnD5eBattleApp;

// TODO: I should make an Ability Class for abilities that can be activated,
// And spells should be a subclass of those. A feat may provide multiple 'abilities'
// Abilities should also have a 'TargetingSpec' class on them

/// <summary>
/// Feats represent any abilities or passive effects a creature has
/// </summary>
public class Feat : IValueModification
{
    public Creature Owner { get; set; }
    public FeatSpec FeatSpec {get; set;}
    public String Name {get; set; }

    public Dictionary<string, int> valueChanges = new Dictionary<string, int>();
    public Dictionary<string, int> valueOverrides = new Dictionary<string, int>();

    public Feat()
    {
        throw new NotImplementedException("Deprecated Feat usage");
    }

    public Feat(Creature owner, FeatSpec spec)
    {
        FeatSpec = spec;
        Owner = owner;
        owner.Feats.Add(this);

        foreach (ValueModificationSpec valueModification in spec.ValueModifications)
        {
            valueChanges[valueModification.ValueType.ToString()] = valueModification.ValueChange;
            owner.Values.GetValue<int>(valueModification.ValueType.ToString()).AddModifier(this);
        }

    }

    public static bool TryAddFeat(Creature owner, string name, out Feat feat)
    {
        feat = null;
        if (DnDManager.TryGetResource(name, out FeatSpec spec))
        {
            feat = new Feat(owner, spec);
            return true;
        }
        return false;
    }

    public void RemoveFeat()
    {
        foreach (string valueType in valueChanges.Keys)
        {
            Owner.Values.GetValue<int>(valueType).RemoveModifier(this);
        }
        foreach (string valueType in valueOverrides.Keys)
        {
            Owner.Values.GetValue<int>(valueType).RemoveModifier(this);
        }
        Owner.Feats.Remove(this);
    }

    public T GetModification<T>(Value<T> value, T currentValue)
    {
        if (valueChanges.ContainsKey(value.ValueType))
        {
            currentValue = (dynamic)currentValue + valueChanges[value.ValueType];
        }
        if (valueOverrides.ContainsKey(value.ValueType))
        {
            currentValue = (dynamic)valueOverrides[value.ValueType];
        }
        return currentValue;
    }
}