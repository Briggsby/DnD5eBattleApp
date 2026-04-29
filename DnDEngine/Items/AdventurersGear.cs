namespace DnD5eBattleApp
{ 
    public abstract class AdventurersGear : Item
    {
        public AdventurersGear() : base()
        {
            itemType = ItemType.AdventuringGear.ToString();
        }
    }
}