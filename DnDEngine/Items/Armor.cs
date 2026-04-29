using System;


namespace DnD5eBattleApp
{
    public enum ArmorCategories { None, LightArmor, MediumArmor, HeavyArmor, Shields };
    public enum ArmorTypes { NoArmor, UnarmoredDefense, Padded, Leather, StuddedLeather, HideArmor, ChainShirt, ScaleMail, Breastplate, HalfPlate, RingMail, ChainMail, SplintArmor, PlateArmor, Shield };

    public abstract class ArmorCreator : ItemCreator
    {
        public virtual Armor CreateArmor()
        {
            return CreateItem() as Armor;
        }
    }

    public class Armor : Item
    {
        public ArmorCategories armorCategory;
        public ArmorTypes baseArmorType;

        public int baseAC;
        public int maxDexBonus;

        public bool stealthDisadvantage;
        public int strengthRequirement;

        public Armor()
        {
            itemType = ItemType.Armor.ToString();

            name = "None";
            armorCategory = ArmorCategories.None;
            baseArmorType = ArmorTypes.NoArmor;
            baseAC = 10;
            maxDexBonus = 10;
            stealthDisadvantage = false;
            strengthRequirement = 0;
        }

        public Armor(ArmorTemplate armor)
        {
            itemType = ItemType.Armor.ToString();

            name = armor.name;
            armorCategory = armor.category;
            baseArmorType = armor.baseArmorType;
            baseAC = armor.baseAC;
            maxDexBonus = armor.maxDexBonus;
            stealthDisadvantage = armor.stealthDisadvantage;
            strengthRequirement = armor.strengthRequirement;
            cost = armor.cost;
            weight = armor.weight;
        }

        public override bool Equippable(Creature creature)
        {
            if (creature.armorCategoryProficiencies.Contains(armorCategory))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Equip(Creature creature, bool offHand = false)
        {
            if (creature.armor != null)
            {
                creature.armor.UnEquip(creature);
            }
            creature.armor = this;
            base.Equip(creature, offHand);
        }

        public virtual int GetAC(Creature creature)
        {
            return baseAC + Math.Min(creature.StatMod(Stats.Dexterity), maxDexBonus);
        }
    }

    public struct ArmorTemplate
    {
        public string name;
        public ArmorCategories category;
        public ArmorTypes baseArmorType;
        public int baseAC;
        public int maxDexBonus;
        public bool stealthDisadvantage;
        public int strengthRequirement;
        public int cost;
        public int weight;

        public ArmorTemplate(string name = "No Armor", ArmorCategories category = ArmorCategories.None, ArmorTypes baseArmorType = ArmorTypes.NoArmor, int baseAC = 10, int maxDexBonus = 10, bool stealthDisadvantage = false, int strengthReq = 0, int cost = 0, int weight = 0)
        {
            this.name = name;
            this.category = category;
            this.baseArmorType = baseArmorType;
            this.baseAC = baseAC;
            this.maxDexBonus = maxDexBonus;
            this.stealthDisadvantage = stealthDisadvantage;
            this.strengthRequirement = strengthReq;
            this.cost = cost;
            this.weight = weight;
        }
    }
}