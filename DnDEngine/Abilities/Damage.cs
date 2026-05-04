namespace DnD5eBattleApp;

public class Damage
{
    public string DamageType {get; set;}
    public int NumberOfDice { get; set; }
    public int MaxValueOfDice {get; set;}
    public int FlatValue {get; set;} = 0;
}