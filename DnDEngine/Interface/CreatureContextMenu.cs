using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    enum CombatActions { Move, Attack, CastSpell, Dash, Disengage, Dodge, Help, Hide, Search, UseObject, UseFeat, SpellChildMenu }
    public enum CreatureMenuOtherOptions { StatMenu, ChangeName, ChangeStats, AddItem, ChangeWeapon, AddFeat, ChooseLanguage, ChooseSkill, ChooseItems, FeatChoices, EquipItem, ChooseWeapon, UseFeatChildMenu };

    public class CreatureContextMenu : ContextMenu
    {
        
        Creature creature;
        BoardTile tile;

        public CreatureContextMenu(BoardTile tile, Creature creature, ContextMenuTemplate template) : base(template, new Vector2(0, 0), tile)
        {
            this.tile = tile;
            this.creature = creature;
            destroyOnOtherClick = true;
        }

        public override void OnSetDestroy()
        {
            base.OnSetDestroy();
            tile.board.encounter.contextMenu = null;
        }

        public override void ButtonPress(List<string> tags, ContextMenuItem item)
        {
            base.ButtonPress(tags, item);

            if (tags.Contains(CombatActions.Move.ToString()))
            {
                new MoveOrder(creature);
            }

            else if (tags.Contains(CombatActions.Attack.ToString()))
            {
                new AttackOrder(creature, creature.GetAttackWeapons()[int.Parse(tags[1])]);
            }

            else if (tags.Contains(CombatActions.CastSpell.ToString()))
            {
                creature.SpellBook.SpellsByLevel[int.Parse(tags[1])][int.Parse(tags[1])].Use(creature);
            }
            
            else if (tags.Contains(CombatActions.SpellChildMenu.ToString()))
            {
                List<string> newTags = new List<string>();
                for (int i = 2; i<tags.Count; i++)
                {
                    newTags.Add(tags[i]);
                }
                creature.oldSpellbook.spells[int.Parse(tags[1])].ChildMenuPress(newTags);
            }

            else if (tags.Contains(CreatureMenuOtherOptions.ChooseLanguage.ToString()))
            {
                creature.baseStats.languages.Remove(DnDManager.Languages.Choose.ToString());
                creature.baseStats.languages.Add(tags[1]);
            }
            else if (tags.Contains(CreatureMenuOtherOptions.ChooseSkill.ToString()))
            {
                creature.baseStats.skillProficiencies.Add((Skill)Enum.Parse(typeof(Skill), tags[1]));
                creature.skillChoices.RemoveAt(0);
            }

            else if (tags.Contains(CombatActions.UseFeat.ToString()))
            {
                creature.oldFeats[int.Parse(tags[1])].UseFeat();
            }

            else if (tags.Contains(CreatureMenuOtherOptions.UseFeatChildMenu.ToString()))
            {
                List<string> featList = new List<string>();
                for (int i = 2; i<tags.Count; i++)
                {
                    featList.Add(tags[i]);
                }
                creature.oldFeats[int.Parse(tags[1])].UseFeatChildMenu(featList);

            }

            else if (tags.Contains(CreatureMenuOtherOptions.FeatChoices.ToString()))
            {
                creature.oldFeats[int.Parse(tags[1])].FeatChoice(tags[2]);
            }

            else if (tags.Contains(CreatureMenuOtherOptions.EquipItem.ToString()))
            {
                creature.inventory.items[int.Parse(tags[1])].Equip(creature);
            }

            else if (tags.Contains(CreatureMenuOtherOptions.ChooseItems.ToString()))
            {
                List<string> newTags = new List<string>();
                creature.itemChoices.RemoveAt(int.Parse(tags[1]));
                for (int i = 2; i<tags.Count; i++)
                {
                    newTags.Add(tags[i]);
                }
                foreach (string s in newTags)
                {
                    if (Enum.GetNames(typeof(WeaponChoices)).Contains(s)) 
                    {
                        creature.weaponChoices.Add(s);
                    }
                    else
                    {
                        creature.inventory.AddItem(DnDManager.items[s].CreateItem());
                    }
                }
            }

            else if (tags.Contains(CreatureMenuOtherOptions.ChooseWeapon.ToString()))
            {
                List<string> newTags = new List<string>();
                creature.weaponChoices.RemoveAt(int.Parse(tags[1]));
                for (int i = 2; i < tags.Count; i++)
                {
                    newTags.Add(tags[i]);
                }
                foreach (string s in newTags)
                {
                    creature.inventory.AddItem(DnDManager.items[s].CreateItem());
                }
            }

            else if (tags.Contains(CreatureMenuOtherOptions.AddItem.ToString()))
            {
                for (int i = 1; i<tags.Count; i++)
                {
                    creature.inventory.AddItem(DnDManager.items[tags[i]].CreateItem());
                }
            }

            if (tags.Contains(CreatureMenuOtherOptions.StatMenu.ToString()))
            {
                if (creature.encounter.statDisplay != null)
                {
                    creature.encounter.statDisplay.DestroyAndChildren();
                }
                creature.encounter.statDisplay = new StatDisplay(creature);
            }

            if (!tags.Contains(DefaultTags.ParentMenu.ToString()))
            {
                tile.board.encounter.contextMenu = null;
                DestroyAndChildren();
            }


        }
    }
}