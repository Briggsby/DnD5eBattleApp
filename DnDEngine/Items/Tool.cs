namespace DnD5eBattleApp
{ 
    public abstract class Tool : Item
    {
        public Tool() : base()
        {
            itemType = ItemType.Tools.ToString();
        }
    }
}