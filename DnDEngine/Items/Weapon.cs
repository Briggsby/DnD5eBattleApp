using System;
using System.Collections.Generic;


namespace DnD5eBattleApp
{

    public enum WeaponCategory { None, SimpleWeapon, MartialWeapon, NaturalWeapon, Shields }
    public enum WeaponType { None, Shield, Scimitar, Shortbow, Club, Dagger, Shortsword, Battleaxe, Handaxe, LightHammer, Warhammer, Greataxe, Javelin, Rapier, Longsword, Mace, LightCrossbow };
    public enum WeaponProperty { Ammunition, Finesse, Heavy, Light, Loading, Range, Reach, Special, Thrown, TwoHanded, Versatile, Improvised, Silvered, SpecialLance, SpecialNet }

    public enum WeaponChoices { SimpleWeapon, MartialWeapon, MartialMeleeWeapon }

    public class Weapon : Item
    {
        public Ability Ability {
            get {
                return new WeaponAttackAbility(this, inventory.creature);
            }
        }
        public WeaponCategory weaponCategory;
        public WeaponType weaponType;
        public List<int> damageDice;
        public List<int> versatileDamageDice = new List<int>();
        public List<int> damageDiceNumber;
        public List<string> damageTypes;

        public Stat abilityStat;
        public Stat AbilityStat {
            get { 
                if (abilityStat == Stat.Strength && weaponProperties.Contains(WeaponProperty.Finesse)) {
                    return inventory.creature.StatMod(Stat.Dexterity) > inventory.creature.StatMod(Stat.Strength) ? Stat.Dexterity : Stat.Strength;
                }
                return abilityStat;
            }
            set { abilityStat = value; }
        }
        public int minRange;
        public int maxRange;
        public List<WeaponProperty> weaponProperties;
        public int acBonus;

        public Type ammunition = typeof(Arrow);
        public bool loadingAttackedThisTurn = false;

        public int attackBonus = 0;
        public int damageBonus = 0;

        public Weapon()
        {
            itemType = ItemType.Weapon.ToString();

            name = "Unarmed Strike";
            weaponType = WeaponType.None;
            weaponCategory = WeaponCategory.None;
            damageDice = new List<int>() { 1 };
            damageDiceNumber = new List<int>() { 1 };
            damageTypes = new List<string>() { DamageType.Bludgeoning.ToString() };
            AbilityStat = Stat.Strength;
            minRange = 5;
            maxRange = 5;
            weaponProperties = new List<WeaponProperty>() { WeaponProperty.Light };
            acBonus = 0;
        }

        public Weapon(
            Inventory inventory,
            string name, 
            WeaponCategory category, 
            List<int> damageDice, 
            List<int> damageDiceNumber, 
            List<string> damageTypes, 
            Stat abilityStat, 
            int minRange = 5, 
            int maxRange = 5, 
            List<WeaponProperty> weaponProperties = null, 
            int acBonus = 0,
            Type ammunition = null,
            int attackBonus = 0,
            int damageBonus = 0
        )
        {
            itemType = ItemType.Weapon.ToString();
            this.inventory = inventory;
            this.name = name;
            this.weaponCategory = category;
            this.damageDice = damageDice;
            this.damageDiceNumber = damageDiceNumber;
            this.damageTypes = damageTypes;
            this.AbilityStat = abilityStat;
            this.minRange = minRange;
            this.maxRange = maxRange;
            this.weaponProperties = weaponProperties ?? new List<WeaponProperty>();
            this.acBonus = acBonus;
            this.ammunition = ammunition;
            this.attackBonus = attackBonus;
            this.damageBonus = damageBonus;
        }

        public void Use() {
            Ability.Use();
        }

        // TODO: Stuff below can be removed if equipping is removed

        public override bool Equippable(Creature creature)
        {
            if (creature.weaponCategoryProficiencies.Contains(weaponCategory) || creature.weaponTypeProficiencies.Contains(weaponType))
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

            if (weaponProperties.Contains(WeaponProperty.Light) && creature.weaponMainHand != null && creature.weaponMainHand.weaponProperties.Contains(WeaponProperty.Light))
            {
                offHand = true;
            }
            if (this is Shield)
            {
                offHand = true;
            }

            if (offHand)
            {
                if (creature.weaponOffHand != null)
                {
                    creature.weaponOffHand.UnEquip(creature);
                }
                creature.weaponOffHand = this;
            }
            else
            {
                if (creature.weaponMainHand != null)
                {
                    creature.weaponMainHand.UnEquip(creature);
                }
                creature.weaponMainHand = this;
                if (weaponProperties.Contains(WeaponProperty.TwoHanded))
                {
                    creature.weaponOffHand = null;
                }
            }

            base.Equip(creature);
        }

        public void GetDamageDice(Attack attack)
        {
            if (weaponProperties.Contains(WeaponProperty.Versatile) && equipper != null && equipper.weaponMainHand == this && equipper.weaponOffHand == null)
            {
                attack.damageDiceFaces = new List<int>(versatileDamageDice);
            }
            else
            {
                attack.damageDiceFaces = new List<int>(damageDice);
            }
            attack.damageDiceNumber = new List<int>(damageDiceNumber);
            attack.damageTypes = new List<string>(damageTypes);
        }

        public virtual void OnHit(Attack attack)
        {

        }
    }

    public abstract class WeaponCreator : ItemCreator
    {
        public virtual Weapon CreateWeapon()
        {
            return CreateItem() as Weapon;
        }
    }
}
