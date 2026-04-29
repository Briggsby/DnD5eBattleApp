namespace DnD5eBattleApp
{ 
    public abstract class WondrousItem : Item
    {
        public WondrousItem() : base()
        {
            itemType = ItemType.WondrousItem.ToString();
        }
    }
}