namespace DnD5eBattleApp;

public record ConditionSpec : FeatSpec
{
    // TODO: Handle duration, currently they last forever

    public Condition ToCondition(Creature owner)
    {
        return new Condition(Name, owner, ValueChanges);
    }
}