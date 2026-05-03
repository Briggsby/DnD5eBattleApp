using System.Collections.Generic;
using BugsbyEngine;

public interface IValue
{
    
}
/// <summary>
/// Represents a value on a Unit in a game. This value has a base value, and a current value,
/// and triggers Effects whenever it is modified.
/// </summary>
public class Value<T> : IValue
{
    public object Source { get; set; }
    public string ValueType { get; set; }
    public T BaseValue { get; set; }

    private T modifiedValue;
    public T ModifiedValue
    {
        get { return GetValue(); }
    }

    public HashSet<IValueModification> Modifiers { get; set; } = new HashSet<IValueModification>();
    
    public Value(object Source, string ValueType, T baseValue)
    {
        this.Source = Source;
        this.ValueType = ValueType;
        BaseValue = baseValue;
        modifiedValue = baseValue;
    }

    public T SetBaseValue(T newBaseValue)
    {
        BaseValue = newBaseValue;
        ModifyValue(newBaseValue, false);
        return BaseValue;
    }

    public T GetValue()
    {
        T newValue = BaseValue;
        foreach (var modifier in Modifiers)
        {
            newValue = modifier.GetModification(this, newValue);
        }
        return newValue;
    }

    public void ModifyValue(T newValue, bool triggerEffect = true)
    {
        if (!triggerEffect)
        {
            modifiedValue = newValue;
            return;
        }
        
        var modifyEffect = new ModifyValue<T>(this, ModifiedValue, newValue);
        EffectManager.TriggerEffect(modifyEffect);
    }

    public void AddModifier(IValueModification modifier)
    {
        Modifiers.Add(modifier);
    }
    public void RemoveModifier(IValueModification modifier)
    {
        Modifiers.Remove(modifier);
    }

}

public class ValueManager
{
    public Dictionary<string, IValue> Values { get; set; } = new Dictionary<string, IValue>();

    public virtual void AddValue<T>(string valueType, T baseValue)
    {
        if (Values.ContainsKey(valueType))
        {
            throw new System.Exception($"Value of type {valueType} already exists on this object.");
        }
        Values[valueType] = new Value<T>(this, valueType, baseValue);
    }

    public virtual Value<T> GetValue<T>(string valueType)
    {
        if (!Values.ContainsKey(valueType))
        {
            throw new System.Exception($"Value of type {valueType} does not exist on this object.");
        }
        if (Values[valueType] is not Value<T>)
        {
            throw new System.Exception($"Value of type {valueType} is not of type {typeof(T)}.");
        }
        return Values[valueType] as Value<T>;
    }

    public virtual void ModifyValue<T>(string valueType, T newValue)
    {
        var value = GetValue<T>(valueType);
        value.ModifyValue(newValue);
    }
}

public interface IValueModification
{
    public T GetModification<T>(Value<T> value, T currentValue);
}

public class ModifyValue<T> : Effect
{
    public Value<T> Value { get; private set; }
    public T PreviousValue { get; private set; }
    public T NewValue { get; private set; }

    public ModifyValue(Value<T> value, T previous_value, T new_value)
     {
        EffectType = value.ValueType;
        Value = value;
        PreviousValue = previous_value;
        NewValue = new_value;
    }

    public override void EnactEffect()
    {
        Value.ModifyValue(NewValue, false);
        base.EnactEffect();
    }
}