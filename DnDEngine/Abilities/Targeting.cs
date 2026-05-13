namespace DnD5eBattleApp;

public enum TargetType
{
    SingleTargetRanged,
    SingleTargetMelee,
    Square,
    EmptyTile,
    Self
}

public class Targeting
{
    public TargetType TargetType {get; set;}
    public bool HasAttackRoll {get; set;} = true;
    public bool HasSavingThrow {get; set;} = false;
    public int TargetCount {get; set; } = 1;    
    public int Range {get; set;} = 0;
    public int Size {get; set;} = 5;

}
