using System;
using System.Collections.Generic;
using System.Linq;
using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class Inventory
    {
        public Creature creature;
        public Dictionary<Type, int> itemAmounts;
        public List<Item> items;

        public List<Weapon> weapons;
        public List<Armor> armors;
        public List<WondrousItem> wondrousItems;
        public List<AdventurersGear> adventurersGears;
        public List<Tool> tools;

        public Inventory(Creature creature, List<string> items)
        {
            this.creature = creature;
            this.items = new List<Item>();
            this.weapons = new List<Weapon>();
            foreach (string itemName in items)
            {
                // TODO: Handle other item types
                if (DnDManager.TryGetResource<WeaponSpec>(itemName, out WeaponSpec weaponSpec))
                {
                    Weapon weapon = weaponSpec.ToWeapon(this);
                    this.items.Add(weapon);
                    this.weapons.Add(weapon);
                }
            }
            SortInventory();
        }

        public void SortInventory()
        {
            itemAmounts = new Dictionary<Type, int>();

            weapons = new List<Weapon>();
            armors = new List<Armor>();
            wondrousItems = new List<WondrousItem>();
            adventurersGears = new List<AdventurersGear>();
            tools = new List<Tool>();

            foreach (Item item in items)
            {
                if (itemAmounts.ContainsKey(item.GetType()))
                {
                    itemAmounts[item.GetType()]++;
                }
                else
                {
                    itemAmounts.Add(item.GetType(), 1);
                }

                if (item is Weapon)
                {
                    weapons.Add(item as Weapon);
                }
                else if (item is Armor)
                {
                    armors.Add(item as Armor);
                }
                else if (item is WondrousItem)
                {
                    wondrousItems.Add(item as WondrousItem);
                }
                else if (item is AdventurersGear)
                {
                    adventurersGears.Add(item as AdventurersGear);
                }
                else if (item is Tool)
                {
                    tools.Add(item as Tool);
                }
            }
        }

        public void SortItem(Item item)
        {
            if (itemAmounts.ContainsKey(item.GetType()))
            {
                itemAmounts[item.GetType()]++;
            }
            else
            {
                itemAmounts.Add(item.GetType(), 1);
            }


            if (item is Weapon)
            {
                weapons.Add(item as Weapon);
            }
            else if (item is Armor)
            {
                armors.Add(item as Armor);
            }
            else if (item is WondrousItem)
            {
                wondrousItems.Add(item as WondrousItem);
            }
            else if (item is AdventurersGear)
            {
                adventurersGears.Add(item as AdventurersGear);
            }
            else if (item is Tool)
            {
                tools.Add(item as Tool);
            }
        }

        public void AddItem(Item item)
        {
            if (item is Pack)
            {
                (item as Pack).Unpack(this);
            }

            items.Add(item);
            SortItem(item);
        }

        public void MakeEquipMenu(ref ContextMenuTemplate template)
        {
            template.texts = new List<string>()
            {
                "Weapons",
                "Armors",
                "Other",
            };
            template.tags = new List<List<string>>()
            {
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString() },

            };
            template.childMenus = new List<ContextMenuTemplate>()
            {
                MakeWeaponEquipMenu(template),
                MakeArmorEquipMenu(template),
                MakeOtherItemEquipMenu(template)
            };

        }

        public ContextMenuTemplate MakeWeaponEquipMenu(ContextMenuTemplate baseTemplate)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = baseTemplate.textures;
            template.font = baseTemplate.font;
            template.texts = new List<string>();
            template.tags = new List<List<string>>();

            foreach (Item item in weapons)
            {
                if (item.Equippable(creature))
                {
                    template.texts.Add(item.name);
                    template.tags.Add(new List<string>() { CreatureMenuOtherOptions.EquipItem.ToString(), items.IndexOf(item).ToString() });
                }
            }

            return template;
        }

        public ContextMenuTemplate MakeArmorEquipMenu(ContextMenuTemplate baseTemplate)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = baseTemplate.textures;
            template.font = baseTemplate.font;
            template.texts = new List<string>();
            template.tags = new List<List<string>>();

            foreach (Item item in armors)
            {
                if (item.Equippable(creature))
                {
                    template.texts.Add(item.name);
                    template.tags.Add(new List<string>() { CreatureMenuOtherOptions.EquipItem.ToString(), items.IndexOf(item).ToString() });
                }
            }

            return template;
        }

        public ContextMenuTemplate MakeOtherItemEquipMenu(ContextMenuTemplate baseTemplate)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = baseTemplate.textures;
            template.font = baseTemplate.font;
            template.texts = new List<string>();
            template.tags = new List<List<string>>();

            foreach (Item item in wondrousItems)
            {
                if (item.Equippable(creature))
                {
                    template.texts.Add(item.name);
                    template.tags.Add(new List<string>() { CreatureMenuOtherOptions.EquipItem.ToString(), items.IndexOf(item).ToString() });
                }
            }

            return template;
        }
    }
}