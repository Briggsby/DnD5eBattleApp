using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public enum ItemType { AdventuringGear, Tools, Kits, Weapon, Armor, WondrousItem };

    public enum Currency { Copper, Silver, Electrum, Gold };
    public enum Tools { SmithsTools, BrewersSupplies, MasonsTools };
    public enum Packs { ExplorersPack }

    public abstract class ItemCreator
    {
        public abstract Item CreateItem();
    }

    public abstract class Item
    {
        public Inventory inventory;

        public string itemType;

        public string name;
        public int cost;
        public int weight;
        public bool magical = false;

        public Creature equipper = null;
        public Creature attuner = null;

        public List<ItemFeat> equipFeats = new List<ItemFeat>();
        public List<ItemFeat> attuneFeats = new List<ItemFeat>();

        public virtual bool Equippable(Creature creature)
        {
            return false;
        }

        public virtual void Hold(Creature creature)
        {

        }

        public virtual void Equip(Creature creature, bool offHand = false)
        {
            equipper = creature;
            foreach (ItemFeat feat in equipFeats)
            {
                creature.AddFeat(feat);
            }
            creature.RecalibrateStats();
        }

        public virtual void UnEquip(Creature creature)
        {
            equipper = null;
            foreach (ItemFeat feat in equipFeats)
            {
                creature.RemoveFeat(feat);
            }
            creature.RecalibrateStats();
        }

        public virtual void Attune(Creature creature)
        {
            attuner = creature;
            foreach (ItemFeat feat in attuneFeats)
            {
                creature.AddFeat(feat);
            }
            creature.RecalibrateStats();
        }

        public virtual void UnAttune(Creature creature)
        {
            attuner = null;
            foreach (ItemFeat feat in attuneFeats)
            {
                creature.AddFeat(feat);
            }
            creature.RecalibrateStats();
        }

        public virtual bool HasUsableButton()
        {
            return false;
        }

        public virtual bool IsUsable()
        {
            return false;
        }

        public virtual void UseItem(Creature creature)
        {

        }
    }
}