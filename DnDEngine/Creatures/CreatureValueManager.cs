using System;
using System.Collections.Generic;
using System.Linq;


namespace DnD5eBattleApp;


[AttributeUsage(AttributeTargets.Field)]
public class DefaultValueAttribute<T> : Attribute
{
    public T DefaultValue { get; }

    public DefaultValueAttribute(T defaultValue)
    {
        DefaultValue = defaultValue;
    }
}

public enum CreatureValue { 
    [DefaultValue<int>(10)]
    Strength,

    [DefaultValue<int>(10)]
    Dexterity,

    [DefaultValue<int>(10)]
    Constitution,

    [DefaultValue<int>(10)]
    Wisdom,

    [DefaultValue<int>(10)]
    Intelligence,

    [DefaultValue<int>(10)]
    Charisma,

    [DefaultValue<int>(8)]
    HitPoints,

    [DefaultValue<int>(0)]
    TemporaryHitPoints,

    [DefaultValue<int>(30)]
    Speed,

    SavingThrowProficiencies
    
}

public static class CreatureValueExtensions{

    // Overrides for types not allowed in Attribute parameters
    static Dictionary<CreatureValue, Tuple<Type, object>> Overrides = new Dictionary<CreatureValue, Tuple<Type, object>>() {
        {CreatureValue.SavingThrowProficiencies, new Tuple<Type, object>(typeof(HashSet<CreatureValue>), new HashSet<CreatureValue> { })}
    };

    
    public static T GetDefaultValue<T>(this CreatureValue value)
    {
        if (Overrides.ContainsKey(value))
        {
            return (T)Overrides[value].Item2;
        }
        var memInfo = typeof(CreatureValue).GetMember(value.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DefaultValueAttribute<T>), false);
        return ((DefaultValueAttribute<T>)attributes[0]).DefaultValue;
    }

    public static Type GetValueType(this CreatureValue value)
    {
        if (Overrides.ContainsKey(value))
        {
            return Overrides[value].Item1;
        }
        var memInfo = typeof(CreatureValue).GetMember(value.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DefaultValueAttribute<>), false);
        return attributes[0].GetType().GetGenericArguments()[0];
    }
}

public class CreatureValueManager : ValueManager
{
    public CreatureValueManager() {
        foreach (CreatureValue value in Enum.GetValues(typeof(CreatureValue))) {
            Type type = value.GetValueType();
            dynamic defaultValue = typeof(CreatureValueExtensions).GetMethod(nameof(CreatureValueExtensions.GetDefaultValue)).MakeGenericMethod(type).Invoke(null, new object[] {value});            
            AddValue(value.ToString(), defaultValue);
        }
    }

    public Value<T> GetValue<T>(CreatureValue value)
    {
        return (Value<T>)Values[value.ToString()];
    }
}