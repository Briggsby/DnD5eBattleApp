namespace DnD5eBattleApp;

public enum TargetType
{
    SingleTargetRanged
}

public class Targeting
{
    public bool HasAttackRoll {get; set;} = true;
    public bool HasSavingThrow {get; set;} = false;
    public int TargetCount {get; set; } = 1;    
    public int Range {get; set;} = 0;

}
