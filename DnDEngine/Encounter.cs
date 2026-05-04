using BugsbyEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DnD5eBattleApp
{
    public class Encounter
    {
        public Library library;

        public List<IEnumerator> coroutineList;
        public List<Roll> rollList;

        public ContextMenu contextMenu;

        public ContextMenuTextureSet contextMenuTextures;

        public Board board;

        public List<Creature> creatures;
        public List<Creature> initiativeOrder;
        int turn;

        public StatDisplay statDisplay;

        public OrderControl orderControl;

        #region Initializing

        public Encounter(DnDManager manager, Vector2 boardDimensions, int boardTileSize, BoardTextureSet boardTextureSet)
        {
            DnDManager.encounters.Add(this);
            library = DnDManager.libraries[0];
            contextMenuTextures = manager.menuTextureSet;
            turn = 0;
            creatures = new List<Creature>();
            initiativeOrder = new List<Creature>();

            MakeSquareBoard(new Vector2(0, 0), boardDimensions, boardTileSize, boardTextureSet);
        }

        public Encounter(Library library, Vector2 boardDimensions, int boardTileSize, BoardTextureSet boardTextureSet, ContextMenuTextureSet menuTextureSet, List<Creature> creatures = null, List<int> teams = null, List<Vector2> positions = null)
        {
            this.library = library;
            DnDManager.encounters.Add(this);
            turn = 0;
            creatures = new List<Creature>();
            initiativeOrder = new List<Creature>();
            creatures = new List<Creature>();

            contextMenuTextures = menuTextureSet;

            if (creatures != null)
            {
                for (int i = 0; i < creatures.Count; i++)
                {
                    this.creatures.Add(creatures[i]);
                    creatures[i].SetToTile(board.Tile(positions[i]));
                }
            }

            MakeSquareBoard(new Vector2(0, 0), boardDimensions, boardTileSize, boardTextureSet);

        }

        public Board MakeSquareBoard(Vector2 position, Vector2 boardDimensions, int boardTileSize, BoardTextureSet textureSet, int boardTileGameSize = 5)
        {
            return board = new Board(this, BoardShape.Square, position, boardDimensions, boardTileSize, boardTileGameSize, textureSet);
        }

        #endregion

        #region Blank Tile context Menu

        public void MakeContextMenu(BoardTile tile)
        {
            board.mouseHoverActive = false;
            if (contextMenu != null)
            {
                contextMenu.DestroyAndChildren();
            }
            contextMenu = new EncounterControlsContextMenu(tile, GetContextMenuBlankTile(tile), GetContextMenuEncounterControls());
        }

        public ContextMenuTemplate GetContextMenuBlankTile(BoardTile tile)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;

            template.texts = new List<string>() {
                "Spawn Monster",
                "Spawn Quick Character",
                "Spawn Character",
                "Destroy Encounter"
            };
            template.tags = new List<List<string>>() {
                new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() { EncounterControls.SpawnCharacter.ToString() },
                new List<string>() { EncounterControls.DestroyEncounter.ToString() }
            };

            template.childMenus = new List<ContextMenuTemplate>() { GetSpawnMonsterMenu(tile), GetSpawnQuickCharacterSpeciesMenu() };

            return template;
        }

        public ContextMenuTemplate GetSpawnMonsterMenu(BoardTile tile)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;


            // Get one for each Monster ingested
            template.texts = new List<string>();
            template.tags = new List<List<string>>();
            for (int i = 0; i < DnDManager.monsters.Keys.Count; i++)
            {
                template.texts.Add(DnDManager.monsters.Keys.ElementAt<string>(i));
                template.tags.Add(new List<string>() { "SpawnMonster", DnDManager.monsters.Keys.ElementAt<string>(i) });
            }

            return template;
        }

        public ContextMenuTemplate BaseControlsTemplate()
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;
            template.texts = new List<string>();
            template.tags = new List<List<string>>();

            return template;
        }

        public ContextMenuTemplate GetSpawnQuickCharacterSpeciesMenu()
        {
            ContextMenuTemplate template = new ContextMenuTemplate();

            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;

            template.texts = new List<string>(DnDManager.species.Keys);
            template.tags = new List<List<string>>();
            template.childMenus = new List<ContextMenuTemplate>();
            for (int i = 0; i < DnDManager.species.Keys.Count; i++)
            {
                template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                template.childMenus.Add(GetSpawnQuickCharacterSubSpeciesMenu(DnDManager.species.Keys.ElementAt<string>(i)));
            }

            return template;
        }

        public ContextMenuTemplate GetSpawnQuickCharacterSubSpeciesMenu(string race)
        {
            ContextMenuTemplate template = BaseControlsTemplate();

            if (DnDManager.species[race].subSpecies == null || DnDManager.species[race].subSpecies.Count < 1)
            {
                template.texts = new List<string>() { "None" };
                template.tags = new List<List<string>>() { new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() } };
                template.childMenus = new List<ContextMenuTemplate>() { GetSpawnQuickCharacterClassMenu(race, null) };
                return template;
            }

            template.texts = new List<string>(DnDManager.species[race].subSpecies);
            template.tags = new List<List<string>>();
            template.childMenus = new List<ContextMenuTemplate>();
            for (int i = 0; i < DnDManager.species[race].subSpecies.Count; i++)
            {
                template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                template.childMenus.Add(GetSpawnQuickCharacterClassMenu(race, DnDManager.species[race].subSpecies[i]));
            }

            return template;
        }

        public ContextMenuTemplate GetSpawnQuickCharacterClassMenu(string race, string subRace)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;

            template.texts = new List<string>(DnDManager.classes.Keys);
            template.tags = new List<List<string>>();
            template.childMenus = new List<ContextMenuTemplate>();
            for (int i = 0; i < DnDManager.classes.Keys.Count; i++)
            {
                template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                template.childMenus.Add(GetSpawnQuickCharacterSubClassMenu(race, subRace, DnDManager.classes.Keys.ElementAt<string>(i)));
            }

            return template;
        }

        public ContextMenuTemplate GetSpawnQuickCharacterSubClassMenu(string race, string subRace, string playerClass)
        {
            ContextMenuTemplate template = BaseControlsTemplate();
            template.texts = new List<string>(DnDManager.classes[playerClass].subClasses);
            template.tags = new List<List<string>>();
            template.childMenus = new List<ContextMenuTemplate>();
            for (int i = 0; i < DnDManager.classes[playerClass].subClasses.Count; i++)
            {
                template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                template.childMenus.Add(GetSpawnQuickCharacterLevelMenu(race, subRace, playerClass, DnDManager.classes[playerClass].subClasses[i]));
            }

            return template;
        }

        public ContextMenuTemplate GetSpawnQuickCharacterLevelMenu(string race, string subrace, string playerClass, string subClass)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;
            template.numberOfColumns = 2;

            template.texts = new List<string>();
            template.tags = new List<List<string>>();
            for (int i = 0; i < 20; i++)
            {
                template.texts.Add((i + 1).ToString());
                template.tags.Add(new List<string>() { EncounterControls.SpawnQuickCharacter.ToString(), race, subrace, playerClass, subClass, (i + 1).ToString() });
            }

            return template;
        }

        //Need to add subraces and subclasses

        public Creature SpawnMonster(CreatureCreator monsterCreator, BoardTile tile)
        {
            return monsterCreator.CreateCreature(tile);
        }

        public Creature SpawnMonster(MonsterSpec monsterSpec, BoardTile tile)
        {
            return Monster.SpawnMonster(monsterSpec, tile);
        }

        public PlayerCharacter SpawnQuickCharacter(List<string> choices, BoardTile tile)
        {
            return new PlayerCharacter(choices, tile);
        }

        #endregion

        #region Creature Controls

        public CreatureContextMenu MakeCommandMenu(BoardTile tile, Creature creature)
        {
            ContextMenuTemplate menu = GetCommandMenu(creature, tile);
            CreatureContextMenu commandMenu = new CreatureContextMenu(tile, creature ?? default(Creature), menu);
            contextMenu = commandMenu;
            return commandMenu;
        }

        public ContextMenuTemplate GetCommandMenu(Creature creature, BoardTile tile = null)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;
            template.numberOfColumns = 2;


            template.texts = new List<string>()
            {
                "Info",
                "Move",
                "Attack",
                "Cast a Spell",
                "Dash",
                "Disengage",
                "Dodge",
                "Help",
                "Hide",
                "Search",
                "Use an Object",
                "Use a Feat",
                "Equip Items",
                "Make Choices"
            };

            template.tags = new List<List<string>>()
            {
                new List<string>() {"StatMenu" },
                new List<string>() { CombatActions.Move.ToString() },
                new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() { CombatActions.Dash.ToString() },
                new List<string>() { CombatActions.Disengage.ToString() },
                new List<string>() { CombatActions.Dodge.ToString() },
                new List<string>() { CombatActions.Help.ToString() },
                new List<string>() { CombatActions.Hide.ToString() },
                new List<string>() { CombatActions.Search.ToString() },
                new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() },
                new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() }
            };

            template.childMenus = new List<ContextMenuTemplate>()
            {
                null,
                null,
                GetAttackMenu(creature),
                creature.spellbook.SpellControls(),
                null,
                null,
                null,
                null,
                null,
                null,
                GetUsableObjectMenu(creature),
                GetFeatMenu(creature),
                GetEquipMenu(creature),
                GetChoicesMenu(creature)
            };

            template.inactives = new List<bool>()
            {
                false,
                !creature.CanMove,
                (creature.AttacksLeft==0 || (creature.actionTaken && !creature.attacked)),
                creature.silenced,
                creature.actionTaken,
                creature.actionTaken,
                creature.actionTaken,
                creature.actionTaken,
                creature.actionTaken,
                creature.actionTaken,
                creature.actionTaken,
                false
            };

            return template;
        }

        ContextMenuTemplate GetAttackMenu(Creature creature)
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;
            template.tags = new List<List<string>>();
            template.texts = new List<string>();

            List<Weapon> weapons = creature.GetAttackWeapons();

            if (weapons.Count > 0)
            {
                for (int w = 0; w < weapons.Count; w++)
                {
                    if (weapons[w].weaponType != WeaponType.Shield)
                    {
                        template.texts.Add(weapons[w].name);
                        template.tags.Add(new List<string>() { CombatActions.Attack.ToString(), w.ToString() });
                    }
                }
            }

            return template;
        }

        ContextMenuTemplate GetUsableObjectMenu(Creature creature)
        {
            ContextMenuTemplate template = BaseControlsTemplate();
            template.texts = new List<string>();
            return template;
        }

        ContextMenuTemplate GetFeatMenu(Creature creature)
        {
            ContextMenuTemplate template = BaseControlsTemplate();
            template.texts = new List<string>();
            template.tags = new List<List<string>>();
            template.inactives = new List<bool>();
            template.childMenus = new List<ContextMenuTemplate>();

            for (int i = 0; i < creature.feats.Count; i++)
            {
                if (creature.feats[i].IsUsable())
                {
                    template.texts.Add(creature.feats[i].name);
                    template.inactives.Add(false);
                    template.childMenus.Add(creature.feats[i].ChildMenu());
                    template.tags.Add(new List<string>() { CombatActions.UseFeat.ToString(), i.ToString() });
                }

            }
            return template;
        }

        ContextMenuTemplate GetEquipMenu(Creature creature)
        {
            ContextMenuTemplate template = BaseControlsTemplate();
            template.texts = new List<string>();
            template.tags = new List<List<string>>();

            creature.inventory.MakeEquipMenu(ref template);

            return template;
        }

        #region Choices


        ContextMenuTemplate GetChoicesMenu(Creature creature)
        {
            ContextMenuTemplate template = BaseControlsTemplate();
            template.texts = new List<string>()
            {
                "Change Name",
                "Change Stats",
                "Add Item",
                "Change Weapon",
                "Add Feat",
                "Choose Language",
                "Choose Skill",
                "Choose Items",
                "Choose Feat"
            };
            template.tags = new List<List<string>>()
            {
                new List<string>() {"ChangeName" },
                new List<string>() {"ChangeStats" },
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString()},
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString()},
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString()},
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString()},
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString()},
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString()},
                new List<string>() {ContextMenu.DefaultTags.ParentMenu.ToString()},
            };
            template.childMenus = new List<ContextMenuTemplate>()
            {
                null,
                null,
                null,
                null,
                null,
                GetCreatureLanguageChoices(creature),
                GetCreatureSkillChoiceMenu(creature),
                GetCreatureItemChoicesMenu(creature),
                GetCreatureFeatChoiceMenu(creature)
            };
            template.inactives = new List<bool>()
            {
                false,
                false,
                false,
                false,
                false,
                !creature.languages.Contains(DnDManager.Languages.Choose.ToString()),
                creature.skillChoices.Count < 1,
                (creature.itemChoices.Count < 1 && creature.weaponChoices.Count <1),
                !creature.HasFeatChoices()
            };

            return template;
        }

        public ContextMenuTemplate GetCreatureLanguageChoices(Creature creature)
        {
            if (!creature.languages.Contains(DnDManager.Languages.Choose.ToString()))
            {
                return null;
            }
            else
            {
                ContextMenuTemplate template = BaseControlsTemplate();
                template.texts = new List<string>();
                template.tags = new List<List<string>>();
                foreach (string s in DnDManager.languages)
                {
                    if (!creature.languages.Contains(s))
                    {
                        template.texts.Add(s);
                        template.tags.Add(new List<string>() { CreatureMenuOtherOptions.ChooseLanguage.ToString(), s });
                    }
                }
                return template;
            }
        }

        public ContextMenuTemplate GetCreatureSkillChoiceMenu(Creature creature)
        {
            ContextMenuTemplate template = BaseControlsTemplate();
            template.texts = new List<string>();
            template.tags = new List<List<string>>();
            if (creature.skillChoices.Count > 0)
            {
                foreach (Skill skill in creature.skillChoices[0])
                {
                    if (!creature.skillProficiencies.Contains(skill))
                    {
                        template.texts.Add(skill.ToString());
                        template.tags.Add(new List<string>() { CreatureMenuOtherOptions.ChooseSkill.ToString(), skill.ToString() });
                    }
                }
            }


            return template;

        }

        public ContextMenuTemplate GetCreatureFeatChoiceMenu(Creature creature)
        {
            ContextMenuTemplate template = BaseControlsTemplate();
            template.texts = new List<string>();
            template.tags = new List<List<string>>();
            template.childMenus = new List<ContextMenuTemplate>();
            foreach (Feat feat in creature.feats)
            {
                if (feat.HasChoices())
                {
                    template.texts.Add(feat.Name);
                    template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                    ContextMenuTemplate childMenu = BaseControlsTemplate();
                    childMenu.childMenus = new List<ContextMenuTemplate>();
                    foreach (string s in feat.FeatChoices)
                    {
                        childMenu.texts.Add(s);
                        if (feat.FeatChoiceChildMenu(s) == null)
                        {
                            childMenu.tags.Add(new List<string>() { CreatureMenuOtherOptions.FeatChoices.ToString(), creature.feats.IndexOf(feat).ToString(), s });
                            childMenu.childMenus.Add(null);
                        }
                        else
                        {
                            childMenu.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                            childMenu.childMenus.Add(feat.FeatChoiceChildMenu(s));
                        }
                    }
                    template.childMenus.Add(childMenu);
                }
            }

            return template;
        }

        public ContextMenuTemplate GetCreatureItemChoicesMenu(Creature creature)
        {
            ContextMenuTemplate template = BaseControlsTemplate();
            template.texts = new List<string>();
            template.tags = new List<List<string>>();
            template.childMenus = new List<ContextMenuTemplate>();
            if (creature.itemChoices.Count > 0)
            {
                int index = 0;

                foreach (List<List<string>> set in creature.itemChoices)
                {
                    template.texts.Add("Equipment Option");
                    template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });

                    ContextMenuTemplate setTemplate = BaseControlsTemplate();
                    setTemplate.texts = new List<string>();
                    setTemplate.tags = new List<List<string>>();
                    foreach (List<string> items in set)
                    {
                        List<string> tags = new List<string>() { CreatureMenuOtherOptions.ChooseItems.ToString(), index.ToString()};
                        string itemString = "";
                        for (int i = 0; i<items.Count; i++)
                        {
                            if (i>0)
                            {
                                itemString += " & ";
                            }
                            itemString += items[i];
                            tags.Add(items[i]);
                        }
                        setTemplate.texts.Add(itemString);
                        setTemplate.tags.Add(tags);
                    }


                    template.childMenus.Add(setTemplate);

                    index++;

                }
            }
            if (creature.weaponChoices.Count > 0)
            {
                for (int i = 0; i<creature.weaponChoices.Count; i++)
                {
                    GetWeaponChoices(ref template, creature.weaponChoices[i], i);
                }
            }

            return template;
        }

        public void GetWeaponChoices(ref ContextMenuTemplate template, string weaponCategory, int choiceIndex)
        {
            if (weaponCategory == WeaponChoices.SimpleWeapon.ToString())
            {
                template.texts.Add("Simple Weapon");
                template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                ContextMenuTemplate options = BaseControlsTemplate();
                options.texts = new List<string>();
                options.tags = new List<List<string>>();
                foreach (string s in DnDManager.simpleMeleeWeapons)
                {
                    options.texts.Add(s);
                    options.tags.Add(new List<string>() { CreatureMenuOtherOptions.ChooseWeapon.ToString(), choiceIndex.ToString(), s });
                }
                foreach (string s in DnDManager.simpleRangedWeapons)
                {
                    options.texts.Add(s);
                    options.tags.Add(new List<string>() { CreatureMenuOtherOptions.ChooseWeapon.ToString(), choiceIndex.ToString(), s });
                }

                template.childMenus.Add(options);

            }
            else if (weaponCategory == WeaponChoices.MartialWeapon.ToString())
            {
                template.texts.Add("Martial Weapon");
                template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                ContextMenuTemplate options = BaseControlsTemplate();
                options.texts = new List<string>();
                options.tags = new List<List<string>>();
                foreach (string s in DnDManager.martialMeleeWeapons)
                {
                    options.texts.Add(s);
                    options.tags.Add(new List<string>() { CreatureMenuOtherOptions.ChooseWeapon.ToString(), choiceIndex.ToString(), s });
                }
                foreach (string s in DnDManager.martialRangedWeapons)
                {
                    options.texts.Add(s);
                    options.tags.Add(new List<string>() { CreatureMenuOtherOptions.ChooseWeapon.ToString(), choiceIndex.ToString(), s });
                }

                template.childMenus.Add(options);
            }
            else if (weaponCategory == WeaponChoices.MartialMeleeWeapon.ToString())
            {
                template.texts.Add("Martial Melee Weapon");
                template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                ContextMenuTemplate options = BaseControlsTemplate();
                options.texts = new List<string>();
                options.tags = new List<List<string>>();
                foreach (string s in DnDManager.martialMeleeWeapons)
                {
                    options.texts.Add(s);
                    options.tags.Add( new List<string>() { CreatureMenuOtherOptions.ChooseWeapon.ToString(), choiceIndex.ToString(), s });
                }
                template.childMenus.Add(options);
            }
            
        }

        #endregion

        #endregion

        #region Encounter Controls

        public enum EncounterControls { EndTurn, DestroyEncounter, SpawnCharacter, SpawnQuickCharacter, ShortRest, LongRest }

        public void EndTurn()
        {
            Debug.WriteLine("Ending Turn");
            turn++;
            foreach (Creature encCreature in creatures)
            {
                encCreature.EndTurn();
                encCreature.StartTurn();
            }
        }

        public ContextMenuTemplate GetContextMenuEncounterControls()
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(contextMenuTextures.blankTileTextures);
            template.font = contextMenuTextures.baseFont;

            template.texts = new List<string>() {
                string.Format("Current Turn : {0}", turn),
                "End Turn"
            };
            template.tags = new List<List<string>>() {
                new List<string>() { ContextMenu.DefaultTags.NotButton.ToString() },
                new List<string>() { EncounterControls.EndTurn.ToString() }
            };


            return template;
        }

        public void DestroyEncounter()
        {
            DnDManager.encounters.Remove(this);
            board.DestroyAndChildren();
        }

        public void ShortRest()
        {
            List<Creature> creatureList = new List<Creature>(creatures);
            foreach(Creature creature in creatureList)
            {
                creature.ShortRest();
            }
        }

        public void LongRest()
        {
            List<Creature> creatureList = new List<Creature>(creatures);
            foreach (Creature creature in creatureList)
            {
                creature.LongRest();
            }
        }

        #endregion
    }
}