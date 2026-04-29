using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public abstract class Pack : Item
    {
        public Dictionary<string, int> items;

        public void Unpack(Inventory inventory)
        {

        }
    }
}