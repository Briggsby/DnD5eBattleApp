using System;
using System.Collections.Generic;


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

    [DefaultValue<int>(30)]
    Speed
}

public static class CreatureValueExtensions{
    public static T GetDefaultValue<T>(this CreatureValue value)
    {
        var type = typeof(CreatureValue).GetType();
        var memInfo = type.GetMember(value.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DefaultValueAttribute<T>), false);
        return (T)attributes[0];
    }

    public static Type GetValueType(this CreatureValue value)
    {
        var type = typeof(CreatureValue).GetType();
        var memInfo = type.GetMember(value.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(Attribute), true);
        return (Type)attributes[0];
    }
}
