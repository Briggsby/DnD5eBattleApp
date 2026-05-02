using BugsbyEngine;

/// <summary>
/// Represents a value on a Unit in a game. This value has a base value, and a current value,
/// and triggers events whenever it is modified.
/// </summary>
public class Value<T>
{
    public string ValueType { get; set; }
    public T BaseValue { get; set; }

    private T modifiedValue;
    public T ModifiedValue
    {
        get { return modifiedValue; }
    }
    
    public Value(string ValueType, T baseValue)
    {
        this.ValueType = ValueType;
        BaseValue = baseValue;
        modifiedValue = baseValue;
    }

    public void ModifyValue(T newValue, bool triggerEvent = true)
    {
        if (!triggerEvent)
        {
            modifiedValue = newValue;
            return;
        }
        
        var modifyEvent = new ModifyValue<T>(this, ModifiedValue, newValue);
        EventManager.TriggerEvent(modifyEvent);
    }
}

public class ModifyValue<T> : IEvent
{
    public Value<T> Value { get; private set; }
    public T PreviousValue { get; private set; }
    public T NewValue { get; private set; }

    public ModifyValue(Value<T> value, T previous_value, T new_value)
     {
        EventType = value.ValueType;
        Value = value;
        PreviousValue = previous_value;
        NewValue = new_value;
    }

    public override void EnactEvent()
    {
        Value.ModifyValue(NewValue, false);
        base.EnactEvent();
    }
}