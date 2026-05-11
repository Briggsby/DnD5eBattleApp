using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    // TODO: This should be a large JSON, not code
    public class SRDLibrary : Library
    {
        public enum Languages { Common, Elvish, Draconic, Dwarvish, Gnomish, Goblin, Halfling, Infernal, Orc, DeepSpeech, Choose }
        public enum Weapons { UnarmedStrike, Club, Scimitar, Shortbow, Longbow, Dagger, Shortsword, Greataxe, Handaxe, Javelin, Rapier, Longsword, Mace, Warhammer, LightCrossbow }
        public enum Armors { Unarmored, Padded, Leather, StuddedLeather, HideArmor, ChainShirt, ScaleMail, Breastplate, HalfPlate, RingMail, ChainMail, SplintArmor, PlateArmor }
        public enum Specieses { Dwarf, Elf, Halfling, Human, Dragonborn, Gnome, HalfElf, HalfOrc, Tiefling }
        public enum SubSpecieses { HighElf, HillDwarf, LightfootHalfling, RockGnome }
        public enum Classes { Barbarian, Bard, Cleric, Druid, Fighter, Monk, Paladin, Ranger, Rogue, Sorceror, Warlock, Wizard}
        public enum SubClasses { Champion, Berserker, LoreBard, LifeDomain }
        public enum Feats { FighterFightingStyle, SecondWind, DarkDevotion, NimbleEscape }
        public enum Monsters { Goblin, Commoner, Cultist, CultFanatic }
        public enum CreatureType { Humanoid }
        public enum CreatureSubType { Human, Goblinoid }
        public enum Spells { Light, SacredFlame, Thaumaturgy, Command, InflictWounds, ShieldOfFaith, HoldPerson, SpiritualWeapon }
        public enum Conditions { Rage, Blinded, Dashing, Deafened, Disengaged, Dodging, Incapacitated, Surprised, Exhausted, Charmed, Frightened, Sleep }
        public enum Packs { ExplorersPack, DiplomatsPack, EntertainersPack, PriestsPack }
        public enum Tools { MusicalInstrument, ThievesTools, SmithsTools, BrewersSupplies, MasonsTools, HerbalismKit}
        public enum ArcaneFoci { HolySymbol, DruidicFocus }

        public SRDLibrary() : base()
        {
        }

        public override void InitializeLanguages()
        {
            languages = new List<string>();
            foreach (string s in Enum.GetNames(typeof(Languages)))
            {
                languages.Add(s);
            }
        }

        #region Conditions

        public override void InitializeConditions()
        {
            conditions = new Dictionary<string, ConditionCreator>()
            {
                {Conditions.Rage.ToString(), new RageCreator() },
                {Conditions.Blinded.ToString(), new BlindedCreator() },
                {Conditions.Deafened.ToString(), new DeafenedCreator() },
                {Conditions.Incapacitated.ToString(), new IncapacitatedCreator() },
                {Conditions.Surprised.ToString(), new SurprisedCreator() },
                {Conditions.Exhausted.ToString(), new ExhaustedCreator() },
                {Conditions.Charmed.ToString(), new CharmedCreator() },
                {Conditions.Frightened.ToString(), new FrightenedCreator() }
            };
        }

        public class RageCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new RageCondition();
            }
        }

        public class RageCondition : OldCondition
        {
            public List<DamageType> resistances = new List<DamageType>()
            {DamageType.Bludgeoning, DamageType.Slashing, DamageType.Piercing };

            public override void EndTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                base.EndTurn(cr, e);
                if (!creature.CheckFeat(BarbarianFeats.PersistentRage.ToString()))
                {
                    if (creature.damageTakenSinceTurn == 0 || !creature.attacked)
                    {
                        creature.RemoveFeat(typeof(RageCondition), true);
                    }
                }
            }

            public RageCondition() : base()
            {
                name = "Raging";
                preRoll = true;
                postRoll = true;
                bonusAction = true;
                endTurn = true;
                statChange = true;
            }

            public override void UseFeat()
            {
                base.UseFeat();
                creature.RemoveFeat(this);
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                AbilityCheck rollAb = roll as AbilityCheck;
                SavingThrow rollSav = roll as SavingThrow;
                if ((roll is AbilityCheck && rollAb.stat == Stat.Strength) || (roll is SavingThrow && rollSav.stat == Stat.Strength))
                {
                    roll.advantage = true;
                }
                base.FeatPreRollCheck(roll, e);
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                DamageRoll rollDam = roll as DamageRoll;
                if (roll.roller == creature && roll is DamageRoll && rollDam.attack != null)
                {
                    if (rollDam.attack.IsMelee && !rollDam.attack.isSpell)
                    {
                        roll.bonus += GetRageBonus();
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }

            public int GetRageBonus()
            {
                if (creature.level < 9)
                {
                    return 2;
                }
                else if (creature.level < 16)
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }

            public override void StatChange(Creature cr)
            {
                creature.silenced = true;
                foreach (DamageType resistance in resistances)
                {
                    if (creature.resistances.Contains(resistance.ToString()))
                    {
                        creature.resistances.Add(resistance.ToString());
                    }
                }
                base.StatChange(cr);
            }
        }

        public class BlindedCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Blinded();
            }
        }

        public class Blinded : OldCondition
        {
            public Blinded() : base()
            {
                name = "Blinded";
            }
        }

        public class DeafenedCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Deafened();
            }
        }

        public class Deafened : OldCondition
        {
            public Deafened() : base()
            {
                name = "Deafened";
            }
        }

        public class IncapacitatedCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Incapacitated();
            }
        }

        public class Incapacitated : OldCondition
        {
            public Incapacitated() : base()
            {
                name = "Incapacitated";
            }
        }

        public class SurprisedCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Surprised();
            }
        }

        public class Surprised : OldCondition
        {
            public Surprised() : base()
            {
                name = "Surprised";
            }
        }

        public class ExhaustedCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Exhausted();
            }
        }

        public class Exhausted : OldCondition
        {
            public Exhausted() : base()
            {
                name = "Exhausted";
            }
        }

        public class CharmedCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Charmed();
            }
        }

        public class Charmed : OldCondition
        {
            public Charmed() : base()
            {
                name = "Charmed";
            }
        }

        public class FrightenedCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Frightened();
            }

            public OldFeat CreateFeat(Creature creature)
            {
                Frightened feat = CreateFeat() as Frightened;
                feat.fearSource = creature;
                return CreateFeat();
            }
        }

        public class Frightened : OldCondition
        {
            public Creature fearSource;

            public Frightened() : base()
            {
                name = "Frightened";
            }

            public Frightened(Creature creature) : base()
            {
                name = "Frightened";
                fearSource = creature;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (creature.HasLineOfSight(fearSource))
                {
                    if (roll.roller == creature && (roll is AttackRoll || roll is AbilityCheck))
                    {
                        roll.disadvantage = true;
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        public class TurnedCreator : FrightenedCreator
        {

        }

        public class Turned : Frightened
        {
            public Turned() : base()
            {
                name = "Turned";
            }
            public Turned(Creature creature) : base(creature)
            {
                name = "Turned";
            }
        }

        #endregion

        #region Items

        public override void InitializeItems()
        {
            items = new Dictionary<string, ItemCreator>()
            {
                #region Armors
                {Armors.Padded.ToString(),         new PaddedArmorCreator()  },
                {Armors.Leather.ToString(),        new LeatherArmorCreator() },
                {Armors.StuddedLeather.ToString(), new StuddedLeatherArmorCreator() },
                {Armors.HideArmor.ToString(),      new HideArmorCreator() },
                {Armors.ChainShirt.ToString(),     new ChainShirtCreator() },
                {Armors.ScaleMail.ToString(),      new ScaleMailCreator() },
                {Armors.Breastplate.ToString(),    new BreastplateCreator() },
                {Armors.HalfPlate.ToString(),      new HalfPlateCreator() },
                {Armors.RingMail.ToString(),       new RingMailCreator() },
                {Armors.ChainMail.ToString(),      new ChainMailCreator() },
                {Armors.SplintArmor.ToString(),    new SplintArmorCreator() },
                {Armors.PlateArmor.ToString(),     new PlateArmorCreator() },
                #endregion
                #region Weapons
                {Weapons.Club.ToString(),          new ClubCreator() } ,
                {Weapons.Scimitar.ToString(),      new ScimitarCreator() },
                {Weapons.Shortbow.ToString(),      new ShortbowCreator() },
                {Weapons.Dagger.ToString(),        new DaggerCreator() },
                {Weapons.Greataxe.ToString(),      new GreataxeCreator() },
                {Weapons.Handaxe.ToString(),       new HandaxeCreator() },
                {Weapons.Javelin.ToString(),       new JavelinCreator() },
                {Weapons.Rapier.ToString(),       new RapierCreator() },
                {Weapons.Longsword.ToString(),       new LongswordCreator() },
                {Weapons.Shortsword.ToString(),    new ShortswordCreator() },
                #endregion
                #region Packs
                {Packs.ExplorersPack.ToString(),   new ExplorersPackCreator() },
                {Packs.DiplomatsPack.ToString(),   new DiplomatsPackCreator() },
                {Packs.EntertainersPack.ToString(),   new EntertainersPackCreator() },
                {Packs.PriestsPack.ToString(),     new PriestsPackCreator() },
                #endregion
                #region Tools
                {Tools.MusicalInstrument.ToString(),   new MusicalInstrumentCreator() },
                {Tools.ThievesTools.ToString(),   new ThievesToolsCreator() },
                {Tools.SmithsTools.ToString(),   new SmithsToolsCreator() },
                {Tools.BrewersSupplies.ToString(),   new BrewersSuppliesCreator() },
                {Tools.MasonsTools.ToString(),   new MasonsToolsCreator() }

                #endregion

            };
        }

        #region Weapons

        public override void InitializeWeaponLists()
        {
            martialMeleeWeapons = new List<string>()
            {Weapons.Greataxe.ToString(), Weapons.Scimitar.ToString(), Weapons.Shortsword.ToString()};
            martialRangedWeapons = new List<string>()
            {Weapons.Longbow.ToString() };
            simpleMeleeWeapons = new List<string>()
            {Weapons.Club.ToString(), Weapons.Dagger.ToString(), Weapons.Handaxe.ToString(),
             Weapons.Javelin.ToString()};
            simpleRangedWeapons = new List<string>()
            {Weapons.Shortbow.ToString() };

        }

        public class UnarmedStrikeCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new UnarmedStrike();
            }
            public override Weapon CreateWeapon()
            {
                return CreateItem() as Weapon;
            }
        }

        public class UnarmedStrike : Weapon
        {
            public UnarmedStrike() : base()
            {

            }
        }

        public class ClubCreator : WeaponCreator
        {
            public ClubCreator() : base() { }

            public override Item CreateItem()
            {
                return new Club();
            }

            public override Weapon CreateWeapon()
            {
                return new Club();
            }

        }

        public class Club : Weapon
        {
            public Club() : base()
            {
                name = "Club";
                weaponCategory = WeaponCategory.SimpleWeapon;
                weaponType = WeaponType.Club;
                damageDice = new List<int>() { 4 };
                damageDiceNumber = new List<int>() { 1 };
                damageTypes = new List<string>() { DamageType.Bludgeoning.ToString() };
                abilityStat = Stat.Strength;
                minRange = 5;
                maxRange = 5;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Light };
                acBonus = 0;
            }
        }

        public class ScimitarCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new Scimitar();
            }
            public override Weapon CreateWeapon()
            {
                return new Scimitar();
            }
        }

        public class Scimitar : Weapon
        {
            public Scimitar() : base()
            {
                name = "Scimitar";
                weaponCategory = WeaponCategory.SimpleWeapon;
                weaponType = WeaponType.Scimitar;
                damageDice = new List<int>() { 6 };
                damageDiceNumber = new List<int>() { 1 };
                damageTypes = new List<string>() { DamageType.Slashing.ToString() };
                abilityStat = Stat.Strength;
                minRange = 5;
                maxRange = 5;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Finesse, WeaponProperty.Light };
                acBonus = 0;
            }
        }

        public class ShortbowCreator : WeaponCreator
        {
            public ShortbowCreator() : base() { }
            public override Item CreateItem()
            {
                return new Shortbow();
            }
        }

        public class Shortbow : Weapon
        {
            public Shortbow() : base()
            {
                name = "Shortbow";
                weaponCategory = WeaponCategory.SimpleWeapon;
                weaponType = WeaponType.Shortbow;
                damageDice = new List<int>() { 6 };
                damageDiceNumber = new List<int>() { 1 };
                damageTypes = new List<string>() { DamageType.Piercing.ToString() };
                abilityStat = Stat.Dexterity;
                minRange = 80;
                maxRange = 320;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Range };
                acBonus = 0;
            }
        }

        public class DaggerCreator : WeaponCreator
        {
            public DaggerCreator() : base()
            { }
            public override Item CreateItem()
            {
                return new Dagger();
            }
            public override Weapon CreateWeapon()
            {
                return new Dagger();
            }
        }

        public class Dagger : Weapon
        {
            public Dagger() : base()
            {
                name = "Dagger";
                weaponCategory = WeaponCategory.SimpleWeapon;
                weaponType = WeaponType.Dagger;
                damageDice = new List<int>() { 4 };
                damageDiceNumber = new List<int>() { 1 };
                damageTypes = new List<string>() { DamageType.Piercing.ToString() };
                abilityStat = Stat.Strength;
                minRange = 20;
                maxRange = 60;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Finesse, WeaponProperty.Light, WeaponProperty.Thrown };
                acBonus = 0;
            }
        }

        public class ShortswordCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new Shortsword();
            }
        }

        public class Shortsword : Weapon
        {
            public Shortsword() : base()
            {
                name = "Shortsword";
                weaponCategory = WeaponCategory.MartialWeapon;
                weaponType = WeaponType.Shortsword;
                damageDice = new List<int>() { 6 };
                damageDiceNumber = new List<int>() { 1 };
                damageTypes = new List<string>() { DamageType.Slashing.ToString() };
                abilityStat = Stat.Strength;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Finesse, WeaponProperty.Light};
                acBonus = 0;
            }
        }

        public class GreataxeCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new Greataxe();
            }

            public override Weapon CreateWeapon()
            {
                return new Greataxe();
            }
        }

        public class Greataxe : Weapon
        {
            public Greataxe() : base()
            {
                name = "Greataxe";
                weaponCategory = WeaponCategory.MartialWeapon;
                weaponType = WeaponType.Greataxe;
                damageDice = new List<int>() { 12 };
                damageDiceNumber = new List<int>() { 1 };
                damageTypes = new List<string>() { DamageType.Slashing.ToString() };
                abilityStat = Stat.Strength;
                minRange = 5;
                maxRange = 5;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.TwoHanded };
                acBonus = 0;
            }
        }

        public class HandaxeCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new Handaxe();
            }

            public override Weapon CreateWeapon()
            {
                return new Handaxe();
            }
        }

        public class Handaxe : Weapon
        {
            public Handaxe() : base()
            {
                name = "Handaxe";
                weaponCategory = WeaponCategory.SimpleWeapon;
                weaponType = WeaponType.Handaxe;
                damageDice = new List<int>() { 6 };
                damageDiceNumber = new List<int>() { 1 };
                damageTypes = new List<string>() { DamageType.Slashing.ToString() };
                abilityStat = Stat.Strength;
                minRange = 20;
                maxRange = 60;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Light, WeaponProperty.Thrown };
                acBonus = 0;
            }
        }

        public class JavelinCreator : WeaponCreator
        {

            public override Item CreateItem()
            {
                return new Javelin();
            }

            public override Weapon CreateWeapon()
            {
                return new Javelin();
            }
        }

        public class Javelin : Weapon
        {
            public Javelin() : base()
            {
                name = "Javelin";
                weaponCategory = WeaponCategory.SimpleWeapon;
                weaponType = WeaponType.Javelin;
                damageDice = new List<int>() { 6 };
                damageDiceNumber = new List<int>() { 1 };
                damageTypes = new List<string>() { DamageType.Piercing.ToString() };
                abilityStat = Stat.Strength;
                minRange = 30;
                maxRange = 120;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Thrown };
                acBonus = 0;
            }
        }

        public class RapierCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new Rapier();
            }
        }

        public class Rapier : Weapon
        {
            public Rapier() : base()
            {
                name = "Rapier";
                damageDice = new List<int>() { 8 };
                weaponCategory = WeaponCategory.MartialWeapon;
                weaponType = WeaponType.Rapier;
                damageTypes = new List<string>() { DamageType.Piercing.ToString() };
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Finesse };
            }
        }

        public class LongswordCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new Longsword();
            }
        }

        public class Longsword : Weapon
        {
            public Longsword() : base()
            {
                name = "Longsword";
                damageDice = new List<int>() { 8 };
                versatileDamageDice = new List<int>() { 10 };
                weaponCategory = WeaponCategory.MartialWeapon;
                weaponType = WeaponType.Longsword;
                damageTypes = new List<string>() { DamageType.Slashing.ToString() };
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Versatile };
            }
        }

        public class MaceCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new Mace();
            }
        }

        public class Mace : Weapon
        {
            public Mace() : base()
            {
                name = "Mace";
                damageDice = new List<int>() { 6 };
                weaponType = WeaponType.Mace;
            }
        }

        public class WarhammerCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new Warhammer();
            }
        }

        public class Warhammer : Weapon
        {
            public Warhammer() : base()
            {
                name = "Warhammer";
                damageDice = new List<int>() { 8 };
                versatileDamageDice = new List<int>() { 10 };
                weaponType = WeaponType.Warhammer;
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Versatile };
            }
        }

        public class LightCrossbowCreator : WeaponCreator
        {
            public override Item CreateItem()
            {
                return new LightCrossbow();
            }
        }

        public class LightCrossbow : Weapon
        {
            public LightCrossbow() : base()
            {
                name = "Light Crossbow";
                damageDice = new List<int>() { 8 };
                weaponType = WeaponType.LightCrossbow;
                minRange = 80;
                maxRange = 320;
                ammunition = typeof(Bolt);
                damageTypes = new List<string>() { DamageType.Piercing.ToString() };
                weaponProperties = new List<WeaponProperty>() { WeaponProperty.Ammunition, WeaponProperty.Range, WeaponProperty.Loading, WeaponProperty.TwoHanded };
            }
        }
        #endregion

        #region Armor

        public override void InitializeArmors()
        {
            armors = new Dictionary<string, ArmorCreator>
            {
                {Armors.Unarmored.ToString(),      new UnarmoredCreator() },
                {Armors.Padded.ToString(),         new PaddedArmorCreator()  },
                {Armors.Leather.ToString(),        new LeatherArmorCreator() },
                {Armors.StuddedLeather.ToString(), new StuddedLeatherArmorCreator() },
                {Armors.HideArmor.ToString(),      new HideArmorCreator() },
                {Armors.ChainShirt.ToString(),     new ChainShirtCreator() },
                {Armors.ScaleMail.ToString(),      new ScaleMailCreator() },
                {Armors.Breastplate.ToString(),    new BreastplateCreator() },
                {Armors.HalfPlate.ToString(),      new HalfPlateCreator() },
                {Armors.RingMail.ToString(),       new RingMailCreator() },
                {Armors.ChainMail.ToString(),      new ChainMailCreator() },
                {Armors.SplintArmor.ToString(),    new SplintArmorCreator() },
                {Armors.PlateArmor.ToString(),     new PlateArmorCreator() }
            };
        }

        public class BarbarianUnarmoredDefenseArmorCreator : ArmorCreator
        {
            public override Item CreateItem()
            {
                return new BarbarianUnarmoredDefenseArmor();
            }
            public override Armor CreateArmor()
            {
                return new BarbarianUnarmoredDefenseArmor();
            }
        }

        public class BarbarianUnarmoredDefenseArmor : Armor
        {
            public BarbarianUnarmoredDefenseArmor() : base()
            {
                name = "Unarmored Defense";
                armorCategory = ArmorCategories.None;
                baseArmorType = ArmorTypes.UnarmoredDefense;
                baseAC = 10;
                cost = 0;
                weight = 0;
            }

            public override int GetAC(Creature creature)
            {
                return baseAC + creature.StatMod(Stat.Dexterity) + creature.StatMod(Stat.Constitution);
            }
        }

        public class UnarmoredCreator : ArmorCreator
        {
            public override Item CreateItem()
            {
                return new Armor();
            }
            public override Armor CreateArmor()
            {
                return new Armor();
            }
        }

        public class PaddedArmor : Armor
        {
            public PaddedArmor() : base()
            {
                name = "Padded Armor";
                armorCategory = ArmorCategories.LightArmor;
                baseArmorType = ArmorTypes.Padded;
                baseAC = 11;
                stealthDisadvantage = true;
                cost = 500;
                weight = 8;
            }
        }

        public class PaddedArmorCreator : ArmorCreator
        {
            public PaddedArmorCreator() : base() { }
            public override Item CreateItem()
            {
                return new PaddedArmor();
            }
            public override Armor CreateArmor()
            {
                return new PaddedArmor();
            }
        }

        public class LeatherArmorCreator : ArmorCreator
        {
            public LeatherArmorCreator() : base() { }
            public override Item CreateItem()
            {
                return new LeatherArmor();
            }
            public override Armor CreateArmor()
            {
                return new LeatherArmor();
            }
        }

        public class LeatherArmor : Armor
        {
            public LeatherArmor() : base()
            {
                name = "Leather Armor";
                armorCategory = ArmorCategories.LightArmor;
                baseArmorType = ArmorTypes.Leather;
                baseAC = 11;
                cost = 1000;
                weight = 10;
            }
        }

        public class StuddedLeatherArmorCreator : ArmorCreator
        {
            public StuddedLeatherArmorCreator() : base() { }
            public override Item CreateItem()
            {
                return new StuddedLeatherArmor();
            }
            public override Armor CreateArmor()
            {
                return new StuddedLeatherArmor();
            }
        }

        public class StuddedLeatherArmor : Armor
        {
            public StuddedLeatherArmor() : base()
            {
                name = "Studded Leather Armor";
                armorCategory = ArmorCategories.LightArmor;
                baseArmorType = ArmorTypes.StuddedLeather;
                baseAC = 12;
                cost = 4500;
                weight = 13;
            }
        }

        public class HideArmorCreator : ArmorCreator
        {
            public HideArmorCreator() : base() { }
            public override Item CreateItem()
            {
                return new HideArmor();
            }
            public override Armor CreateArmor()
            {
                return new HideArmor();
            }
        }

        public class HideArmor : Armor
        {
            public HideArmor() : base()
            {
                name = "Hide Armor";
                armorCategory = ArmorCategories.MediumArmor;
                baseArmorType = ArmorTypes.HideArmor;
                baseAC = 12;
                maxDexBonus = 2;
                cost = 1000;
                weight = 12;
            }
        }

        public class ChainShirtCreator : ArmorCreator
        {
            public ChainShirtCreator() : base() { }
            public override Item CreateItem()
            {
                return new ChainShirt();
            }
            public override Armor CreateArmor()
            {
                return new ChainShirt();
            }
        }

        public class ChainShirt : Armor
        {
            public ChainShirt() : base()
            {
                name = "Chain Shirt";
                armorCategory = ArmorCategories.MediumArmor;
                baseArmorType = ArmorTypes.ChainShirt;
                baseAC = 13;
                maxDexBonus = 2;
                cost = 5000;
                weight = 20;
            }
        }

        public class ScaleMailCreator : ArmorCreator
        {
            public ScaleMailCreator() : base() { }

            public override Item CreateItem()
            {
                return new ScaleMail();
            }
            public override Armor CreateArmor()
            {
                return new ScaleMail();
            }
        }

        public class ScaleMail : Armor
        {
            public ScaleMail() : base()
            {
                name = "Scale Mail";
                armorCategory = ArmorCategories.MediumArmor;
                baseArmorType = ArmorTypes.ScaleMail;
                baseAC = 14;
                maxDexBonus = 2;
                stealthDisadvantage = true;
                cost = 5000;
                weight = 45;
            }
        }

        public class BreastplateCreator : ArmorCreator
        {
            public BreastplateCreator() : base() { }
            public override Item CreateItem()
            {
                return new Breastplate();
            }
            public override Armor CreateArmor()
            {
                return new Breastplate();
            }
        }

        public class Breastplate : Armor
        {
            public Breastplate() : base()
            {
                name = "Breastplate";
                armorCategory = ArmorCategories.MediumArmor;
                baseArmorType = ArmorTypes.Breastplate;
                baseAC = 14;
                maxDexBonus = 2;
                cost = 40000;
                weight = 20;
            }
        }

        public class HalfPlateCreator : ArmorCreator
        {
            public HalfPlateCreator() : base() { }
            public override Item CreateItem()
            {
                return new HalfPlate();
            }
            public override Armor CreateArmor()
            {
                return new HalfPlate();
            }
        }

        public class HalfPlate : Armor
        {
            public HalfPlate() : base()
            {
                name = "Half Plate";
                armorCategory = ArmorCategories.MediumArmor;
                baseArmorType = ArmorTypes.HalfPlate;
                baseAC = 15;
                maxDexBonus = 2;
                stealthDisadvantage = true;
                cost = 75000;
                weight = 40;
            }
        }

        public class RingMailCreator : ArmorCreator
        {
            public RingMailCreator() : base() { }
            public override Item CreateItem()
            {
                return new RingMail();
            }
            public override Armor CreateArmor()
            {
                return new RingMail();
            }
        }

        public class RingMail : Armor
        {
            public RingMail() : base()
            {
                name = "Ring Mail";
                armorCategory = ArmorCategories.HeavyArmor;
                baseArmorType = ArmorTypes.RingMail;
                baseAC = 14;
                maxDexBonus = 0;
                stealthDisadvantage = true;
                cost = 3000;
                weight = 40;
            }
        }

        public class ChainMailCreator : ArmorCreator
        {
            public ChainMailCreator() : base() { }
            public override Item CreateItem()
            {
                return new ChainMail();
            }
            public override Armor CreateArmor()
            {
                return new ChainMail();
            }
        }

        public class ChainMail : Armor
        {
            public ChainMail() : base()
            {
                name = "Chain Mail";
                armorCategory = ArmorCategories.HeavyArmor;
                baseArmorType = ArmorTypes.ChainMail;
                baseAC = 16;
                maxDexBonus = 0;
                stealthDisadvantage = true;
                strengthRequirement = 13;
                cost = 20000;
                weight = 55;
            }
        }

        public class SplintArmorCreator : ArmorCreator
        {
            public SplintArmorCreator() : base() { }
            public override Item CreateItem()
            {
                return new SplintArmor();
            }
            public override Armor CreateArmor()
            {
                return new SplintArmor();
            }
        }

        public class SplintArmor : Armor
        {
            public SplintArmor() : base()
            {
                name = "Splint Armor";
                armorCategory = ArmorCategories.HeavyArmor;
                baseArmorType = ArmorTypes.ScaleMail;
                baseAC = 17;
                maxDexBonus = 0;
                stealthDisadvantage = true;
                strengthRequirement = 15;
                cost = 20000;
                weight = 60;
            }
        }

        public class PlateArmorCreator : ArmorCreator
        {
            public PlateArmorCreator() : base() { }
            public override Item CreateItem()
            {
                return new PlateArmor();
            }
            public override Armor CreateArmor()
            {
                return new PlateArmor();
            }
        }

        public class PlateArmor : Armor
        {
            public PlateArmor() : base()
            {
                name = "Plate Armor";
                armorCategory = ArmorCategories.HeavyArmor;
                baseArmorType = ArmorTypes.PlateArmor;
                baseAC = 18;
                maxDexBonus = 0;
                stealthDisadvantage = true;
                strengthRequirement = 15;
                cost = 150000;
                weight = 65;
            }
        }

        #endregion

        #region Packs

        public class ExplorersPackCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new ExplorersPack();
            }
        }

        public class ExplorersPack : Pack
        {

        }

        public class DiplomatsPackCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new DiplomatsPack();
            }
        }

        public class DiplomatsPack : Pack
        {

        }

        public class EntertainersPackCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new EntertainersPack();
            }
        }

        public class EntertainersPack : Pack
        {

        }

        public class PriestsPackCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new PriestsPack();
            }
        }

        public class PriestsPack : Pack
        {

        }

        #endregion

        #region Tools

        public class SmithsToolsCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new SmithsTools();
            }
        }

        public class SmithsTools : Tool
        {

        }

        public class ThievesToolsCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new ThievesTools();
            }
        }

        public class ThievesTools : Tool
        {

        }

        public class BrewersSuppliesCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new BrewersSupplies();
            }
        }

        public class BrewersSupplies : Tool
        {

        }

        public class MasonsToolsCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new MasonsTools();
            }
        }

        public class MasonsTools : Tool
        {

        }

        public class MusicalInstrumentCreator : ItemCreator
        {
            public override Item CreateItem()
            {
                return new MusicalInstrument();
            }
        }

        public class MusicalInstrument : Tool
        {

        }

        #endregion

        #region Other

        public class HolySymbol : SpellcastingFocus
        {

        }

        public class DruidicFocus : SpellcastingFocus
        {

        }

        #endregion

        #endregion

        #region Species

        public override void InitializeSpecies()
        {
            species = new Dictionary<string, Species>()
            {
                {Specieses.Elf.ToString(), new Elf() },
                {Specieses.Dwarf.ToString(), new Dwarf() },
                {Specieses.Halfling.ToString(), new Halfling() },
                {Specieses.Human.ToString(), new Human() },
                {Specieses.Dragonborn.ToString(), new Dragonborn() },
                {Specieses.Gnome.ToString(), new Gnome() },
                {Specieses.HalfElf.ToString(), new HalfElf() },
                {Specieses.HalfOrc.ToString(), new HalfOrc() },
                {Specieses.Tiefling.ToString(), new Tiefling() }
            };

            subSpecies = new Dictionary<string, SubSpecies>()
            {
                {SubSpecieses.HighElf.ToString(), new HighElf() },
                {SubSpecieses.HillDwarf.ToString(), new HillDwarf() },
                {SubSpecieses.LightfootHalfling.ToString(), new LightfootHalfling() },
                {SubSpecieses.RockGnome.ToString(), new RockGnome() }
            };
        }

        #region Elves

        public class ElfCreator : SpeciesCreator
        {

            public ElfCreator() : base() { }

            public override Species CreateSpecies()
            {
                return new Elf();
            }
        }

        public class Elf : Species
        {
            public Elf() : base()
            {
                name = "Elf";
                statIncreases = new Dictionary<Stat, int> { { Stat.Dexterity, 2 } };
                size = Size.Medium;
                speed = 30;
                darkvision = 60;
                languages = new List<string>() { DnDManager.Languages.Common.ToString(), DnDManager.Languages.Elvish.ToString() };

                feats = new List<string>() { };
                subSpecies = new List<string>() { SubSpecieses.HighElf.ToString() };
            }
        }

        public class HighElf : SubSpecies
        {
            public HighElf() : base()
            {
                name = "High Elf";
                parentSpecies = Specieses.Elf.ToString();
                statIncreases = new Dictionary<Stat, int> { { Stat.Intelligence, 1 } };
                feats = new List<string>() { };
                languages = new List<string>() { DnDManager.Languages.Choose.ToString() };
            }
        }

        #endregion

        #region Dwarves

        public class Dwarf : Species
        {
            public Dwarf() : base()
            {
                name = "Dwarf";
                statIncreases = new Dictionary<Stat, int>() { { Stat.Constitution, 2 } };
                size = Size.Medium;
                speed = 25;
                darkvision = 60;
                languages = new List<string>() { DnDManager.Languages.Common.ToString(), DnDManager.Languages.Dwarvish.ToString() };
                feats = new List<string>() { RacialFeats.DwarvenCombatTraining.ToString(), RacialFeats.DwarvenResilience.ToString(), RacialFeats.DwarvenToolProficiency.ToString(), RacialFeats.Stonecunning.ToString() };
                subSpecies = new List<string>() { SubSpecieses.HillDwarf.ToString() };
            }
        }

        public class HillDwarf : SubSpecies
        {
            public HillDwarf() : base()
            {
                name = "Hill Dwarf";
                statIncreases = new Dictionary<Stat, int>() { { Stat.Wisdom, 1 } };
                feats = new List<string>() { RacialFeats.DwarvenToughness.ToString() };
            }
        }

        #endregion

        #region Halfling

        public class Halfling : Species
        {
            public Halfling() : base()
            {
                name = "Halfling";
                statIncreases = new Dictionary<Stat, int>() { { Stat.Dexterity, 2 } };
                size = Size.Small;
                speed = 25;
                feats = new List<string>() { RacialFeats.HalflingLucky.ToString(), RacialFeats.HalflingBrave.ToString(), RacialFeats.HalflingNimbleness.ToString() };
                languages = new List<string>() { DnDManager.Languages.Common.ToString(), DnDManager.Languages.Halfling.ToString() };
            }
        }

        public class LightfootHalfling : SubSpecies
        {
            public LightfootHalfling() : base()
            {
                name = "Lightfoot Halfling";
                statIncreases = new Dictionary<Stat, int>() { { Stat.Charisma, 1 } };
                feats = new List<string>() { RacialFeats.NaturallyStealthy.ToString() };
            }
        }

        #endregion

        public class Human : Species
        {
            public Human() : base()
            {
                name = "Human";
                statIncreases = new Dictionary<Stat, int>() { { Stat.Strength, 1 }, { Stat.Dexterity, 1 }, { Stat.Constitution, 1 }, { Stat.Intelligence, 1 }, { Stat.Wisdom, 1 }, { Stat.Charisma, 1 } };
                size = Size.Medium;
                speed = 30;
                languages = new List<string>() { DnDManager.Languages.Common.ToString(), DnDManager.Languages.Choose.ToString() };
            }
        }

        public class Dragonborn : Species
        {
            public Dragonborn() : base()
            {
                name = "Dragonborn";
                statIncreases = new Dictionary<Stat, int>() { { Stat.Strength, 2 }, { Stat.Charisma, 1 } };
                size = Size.Medium;
                speed = 30;
                feats = new List<string>() { RacialFeats.DraconicAncestry.ToString() };
                languages = new List<string>() { DnDManager.Languages.Common.ToString(), DnDManager.Languages.Draconic.ToString() };
            }
        }

        #region Gnomes

        public class Gnome : Species
        {
            public Gnome() : base()
            {
                statIncreases = new Dictionary<Stat, int>() { { Stat.Intelligence, 2 } };
                size = Size.Small;
                speed = 25;
                darkvision = 60;
                feats = new List<string>() { RacialFeats.GnomeCunning.ToString() };
                languages = new List<string>() { Languages.Common.ToString(), Languages.Gnomish.ToString() };
                subSpecies = new List<string>() { SubSpecieses.RockGnome.ToString() };
            }
        }

        public class RockGnome : SubSpecies
        {
            public RockGnome() : base()
            {
                statIncreases = new Dictionary<Stat, int>() { { Stat.Constitution, 1 } };
            }
        }
        #endregion

        public class HalfElf : Species
        {
            public HalfElf() : base()
            {
                statIncreases = new Dictionary<Stat, int>() { { Stat.Charisma, 2 } };
                darkvision = 60;
                feats = new List<string>() { RacialFeats.FeyAncestry.ToString(), RacialFeats.HalfElfStatVersatility.ToString(), RacialFeats.HalfElfSkillVersatility.ToString() };
                languages = new List<string>() { Languages.Common.ToString(), Languages.Elvish.ToString(), Languages.Choose.ToString() };
            }
        }

        public class HalfOrc : Species
        {
            public HalfOrc() : base()
            {
                statIncreases = new Dictionary<Stat, int>() { { Stat.Strength, 2 }, { Stat.Constitution, 1 } };
                darkvision = 60;
                proficiencies = new List<Skill>() { Skill.Intimidation };
                feats = new List<string>() { RacialFeats.RelentlessEndurance.ToString(), RacialFeats.SavageAttacks.ToString() };
                languages = new List<string>() { Languages.Orc.ToString(), Languages.Common.ToString() };
            }
        }

        public class Tiefling : Species
        {
            public Tiefling() : base()
            {
                statIncreases = new Dictionary<Stat, int>() { { Stat.Intelligence, 1 }, { Stat.Charisma, 2 } };
                darkvision = 60;
                languages = new List<string>() { Languages.Common.ToString(), Languages.Infernal.ToString() };
                feats = new List<string>() { RacialFeats.InfernalLegacy.ToString(), RacialFeats.HellishResistance.ToString() };
            }
        }
        #endregion

        #region Classes

        public override void InitializeClasses()
        {
            classes = new Dictionary<string, PlayerClass>()
            {
                { Classes.Fighter.ToString(),   new Fighter() },
                { Classes.Barbarian.ToString(), new Barbarian() },
                {Classes.Bard.ToString(), new Bard() }
            };

            subClasses = new Dictionary<string, SubClass>()
            {
                {SubClasses.Champion.ToString(), new ChampionFighter() },
                {SubClasses.Berserker.ToString(), new BerserkerPath() },
                {SubClasses.LoreBard.ToString(), new LoreBard() }
            };
        }

        #region Fighter

        public class FighterCreator : PlayerClassCreator
        {
            public FighterCreator() : base() { }

            public override PlayerClass CreatePlayerClass()
            {
                return new Fighter();
            }
        }

        public class Fighter : PlayerClass
        {
            public Fighter() : base()
            {
                name = "Fighter";
                hitDice = 10;
                armorCategoryProficiencies = new List<ArmorCategories>() { ArmorCategories.LightArmor, ArmorCategories.MediumArmor, ArmorCategories.HeavyArmor, ArmorCategories.Shields };
                weaponCategoryProficiencies = new List<WeaponCategory>() { WeaponCategory.SimpleWeapon, WeaponCategory.MartialWeapon, WeaponCategory.Shields };
                weaponProficiencies = new List<WeaponType>();
                savingThrows = new List<Stat>() { Stat.Strength, Stat.Constitution };
                skillProficiencyOptions = new List<Skill>() { Skill.Acrobatics, Skill.AnimalHandling, Skill.Athletics, Skill.History, Skill.Insight, Skill.Intimidation, Skill.Perception, Skill.Survival };
                skillProficiencyNumberOfChoices = 2;
                equipmentOptions = new List<List<List<string>>>();
                subClassLevel = 3;
                subClasses = new List<string>() { SubClasses.Champion.ToString() };
                feats = new Dictionary<int, List<string>>();

                statRelevanceInOrder = new List<Stat>() { Stat.Strength, Stat.Constitution, Stat.Dexterity, Stat.Wisdom, Stat.Charisma, Stat.Intelligence };

                items = new List<string>() { Armors.PlateArmor.ToString(), Armors.HalfPlate.ToString() };
                equipmentOptions = new List<List<List<string>>>()
                {
                    new List<List<string>>()
                    {
                        new List<string>() {Armors.ChainMail.ToString() },
                        new List<string>() {Armors.Leather.ToString(), Weapons.Longbow.ToString() }
                    },
                    new List<List<string>>()
                    {
                        new List<string>() {Weapons.Shortbow.ToString()},
                        new List<string>() {Weapons.Scimitar.ToString() }
                    }
                };
            }
        }

        public class ChampionFighter : SubClass
        {
            public ChampionFighter() : base()
            {
                name = "Champion";
                parentClass = Classes.Fighter.ToString();
                feats = new Dictionary<int, List<string>>();
            }
        }

        #endregion

        #region Barbarian

        public class Barbarian : PlayerClass
        {
            public Barbarian() : base()
            {
                name = "Barbarian";
                hitDice = 12;
                armorCategoryProficiencies = new List<ArmorCategories>() { ArmorCategories.LightArmor, ArmorCategories.MediumArmor, ArmorCategories.Shields };
                weaponCategoryProficiencies = new List<WeaponCategory>() { WeaponCategory.SimpleWeapon, WeaponCategory.MartialWeapon, WeaponCategory.Shields };
                weaponProficiencies = new List<WeaponType>();
                savingThrows = new List<Stat>() { Stat.Strength, Stat.Constitution };
                skillProficiencyOptions = new List<Skill>() { Skill.AnimalHandling, Skill.Athletics, Skill.Nature, Skill.Intimidation, Skill.Perception, Skill.Survival };
                skillProficiencyNumberOfChoices = 2;
                subClassLevel = 3;
                subClasses = new List<string>() { SubClasses.Berserker.ToString() };
                feats = new Dictionary<int, List<string>>()
                {
                    { 1, new List<string>()  {BarbarianFeats.UnarmoredDefenseBarbarian.ToString(), BarbarianFeats.RageFeat.ToString() } },
                    { 2, new List<string>()  {BarbarianFeats.RecklessAttack.ToString(), BarbarianFeats.DangerSense.ToString() } },
                    { 4, new List<string>()  {ClassFeats.AbilityScoreImprovement.ToString() }},
                    { 5, new List<string>()  {ClassFeats.ExtraAttack.ToString(), BarbarianFeats.FastMovementBarbarian.ToString() }},
                    { 7, new List<string>()  {BarbarianFeats.FeralInstinct.ToString() } },
                    { 8, new List<string>()  {ClassFeats.AbilityScoreImprovement.ToString() }},
                    { 9, new List<string>()  {BarbarianFeats.BrutalCritical.ToString() }},
                    { 11, new List<string>() {BarbarianFeats.RelentlessRage.ToString()} },
                    { 12, new List<string>() {ClassFeats.AbilityScoreImprovement.ToString()} },
                    { 15, new List<string>() {BarbarianFeats.PersistentRage.ToString()} },
                    { 16, new List<string>() {ClassFeats.AbilityScoreImprovement.ToString() } },
                    { 18, new List<string>() {BarbarianFeats.IndomitableMight.ToString()} },
                    { 19, new List<string>() {ClassFeats.AbilityScoreImprovement.ToString() } },
                    { 20, new List<string>() {BarbarianFeats.PrimalChampion.ToString()} },




                };

                statRelevanceInOrder = new List<Stat>() { Stat.Strength, Stat.Constitution, Stat.Dexterity, Stat.Wisdom, Stat.Charisma, Stat.Intelligence };

                items = new List<string>() { Packs.ExplorersPack.ToString(), Weapons.Javelin.ToString(), Weapons.Javelin.ToString(), Weapons.Javelin.ToString(), Weapons.Javelin.ToString() };
                equipmentOptions = new List<List<List<string>>>()
                {
                    new List<List<string>>()
                    {
                        new List<string>() {Weapons.Greataxe.ToString() },
                        new List<string>() {WeaponChoices.MartialMeleeWeapon.ToString()}
                    },
                    new List<List<string>>()
                    {
                        new List<string>() {Weapons.Handaxe.ToString(), Weapons.Handaxe.ToString()},
                        new List<string>() {WeaponChoices.SimpleWeapon.ToString() }
                    }
                };

            }
        }

        public class BerserkerPath : SubClass
        {
            public BerserkerPath() : base()
            {
                name = "Berserker";
                parentClass = Classes.Barbarian.ToString();
                feats = new Dictionary<int, List<string>>() {
                    {3, new List<string>() {BarbarianFeats.Frenzy.ToString() } },
                    {6, new List<string>() {BarbarianFeats.MindlessRage.ToString() } },
                    {10, new List<string>() {BarbarianFeats.IntimidatingPresence.ToString() } },
                    {14, new List<string>() {BarbarianFeats.Retaliation.ToString() } }
                };
            }
        }

        public class Bard : PlayerClass
        {
            public Bard() : base()
            {
                name = "Bard";
                hitDice = 8;
                armorCategoryProficiencies = new List<ArmorCategories>() { ArmorCategories.LightArmor };
                weaponCategoryProficiencies = new List<WeaponCategory>();
                weaponProficiencies = new List<WeaponType>();
                savingThrows = new List<Stat>() { Stat.Dexterity, Stat.Charisma };
                skillProficiencyNumberOfChoices = 3;
                skillProficiencyOptions = new List<Skill>() { Skill.Athletics, Skill.AnimalHandling, Skill.Arcana, Skill.Athletics, Skill.Deception, Skill.History, Skill.Insight, Skill.Intimidation, Skill.Investigation, Skill.Medicine, Skill.Nature, Skill.Perception, Skill.Performance, Skill.Persuasion, Skill.Religion, Skill.SleightOfHand, Skill.Stealth, Skill.Survival };
                items = new List<string>() { Armors.Leather.ToString(), Weapons.Dagger.ToString(), Tools.MusicalInstrument.ToString() };
                feats = new Dictionary<int, List<string>>()
                {
                    {1, new List<string>() {BardFeats.BardCantripFeat.ToString(), BardFeats.BardSpellcasting.ToString(), ClassFeats.RitualCasting.ToString(), BardFeats.BardicInspirationGranter.ToString()} },
                    {2, new List<string>() {BardFeats.JackOfAllTrades.ToString(), BardFeats.SongOfRest.ToString() } },
                    {3, new List<string>() {BardFeats.ExpertiseBard.ToString(), BardFeats.ExpertiseBard.ToString() } },
                    {4, new List<string>() {ClassFeats.AbilityScoreImprovement.ToString() } },
                    {5, new List<string>() {BardFeats.FontofInspiration.ToString() } },
                    {6, new List<string>() {BardFeats.Countercharm.ToString() } },
                    {8, new List<string>() {ClassFeats.AbilityScoreImprovement.ToString() } },
                    {10, new List<string>() {BardFeats.MagicalSecrets.ToString() } },
                    {12, new List<string>() {ClassFeats.AbilityScoreImprovement.ToString() } },
                    {16, new List<string>() {ClassFeats.AbilityScoreImprovement.ToString() } },
                    {19, new List<string>() {ClassFeats.AbilityScoreImprovement.ToString() } },
                    {20, new List<string>() {BardFeats.SuperiorInspiration.ToString() } }
                };
                subClassLevel = 3;
                subClasses = new List<string>() { SubClasses.LoreBard.ToString() };
                equipmentOptions = new List<List<List<string>>>()
                {
                    new List<List<string>>()
                    {
                        new List<string>() {Weapons.Rapier.ToString()},
                        new List<string>() {Weapons.Longsword.ToString() },
                        new List<string>() {WeaponChoices.SimpleWeapon.ToString() }
                    },
                    new List<List<string>>()
                    {
                        new List<string>() {Packs.DiplomatsPack.ToString() },
                        new List<String>() {Packs.EntertainersPack.ToString() }
                    }
                };
                statRelevanceInOrder = new List<Stat>() { Stat.Charisma, Stat.Dexterity, Stat.Intelligence, Stat.Wisdom, Stat.Constitution, Stat.Strength };
            }
        }

        public class LoreBard : SubClass
        {
            public LoreBard() : base()
            {
                name = "College of Lore";
                parentClass = Classes.Bard.ToString();
                feats = new Dictionary<int, List<string>>()
                {
                    {3, new List<string>() {BardFeats.BonusProficienciesLore.ToString(), BardFeats.CuttingWords.ToString() } },
                    {14, new List<string>() {BardFeats.PeerlessSkill.ToString()} }
                };
            }
        }

        #endregion

        #endregion

        #region Feats
        public enum RacialFeats { DwarvenResilience, DwarvenCombatTraining, DwarvenToolProficiency, Stonecunning, DwarvenToughness,
            FeyAncestry, Trance, HighElfCantrip, HalflingLucky, HalflingBrave, HalflingNimbleness, NaturallyStealthy,
            DraconicAncestry, GnomeCunning, HalfElfStatVersatility, HalfElfSkillVersatility, RelentlessEndurance, SavageAttacks, InfernalLegacy, HellishResistance
        }
        public enum ClassFeats { AbilityScoreImprovement, ExtraAttack, Spellcasting, Cantrips, RitualCasting};

        public override void InitializeFeats()
        {
            feats = new Dictionary<string, FeatCreator>()
            {
                #region Racial Feats
                {RacialFeats.DwarvenResilience.ToString(), new DwarvenResilienceCreator() },
                {RacialFeats.DwarvenCombatTraining.ToString(), new DwarvenCombatTrainingCreator() },
                {RacialFeats.DwarvenToolProficiency.ToString(), new DwarvenToolProficiencyCreator() },
                {RacialFeats.Stonecunning.ToString(), new StonecunningCreator() },
                {RacialFeats.DwarvenToughness.ToString(), new DwarvenToughnessCreator() },
                {RacialFeats.FeyAncestry.ToString(), new FeyAncestryCreator() },
                {RacialFeats.Trance.ToString(), new ElfTranceCreator() },
                {RacialFeats.HighElfCantrip.ToString(), new HighElfCantripCreator() },
                {RacialFeats.HalflingLucky.ToString(), new HalflingLuckyCreator() },
                {RacialFeats.HalflingBrave.ToString(), new HalflingBraveCreator() },
                {RacialFeats.HalflingNimbleness.ToString(), new HalflingNimblenessCreator() },
                {RacialFeats.NaturallyStealthy.ToString(), new NaturallyStealthyCreator() },
                {RacialFeats.DraconicAncestry.ToString(), new DraconicAncestryCreator() },
                {RacialFeats.GnomeCunning.ToString(), new GnomeCunningCreator() },
                {RacialFeats.HalfElfSkillVersatility.ToString(), new HalfElfSkillVersatilityCreator() },
                {RacialFeats.HalfElfStatVersatility.ToString(), new HalfElfStatVersatilityCreator() },
                {RacialFeats.RelentlessEndurance.ToString(), new RelentlessEnduranceCreator() },
                {RacialFeats.SavageAttacks.ToString(), new SavageAttacksCreator() },
                {RacialFeats.HellishResistance.ToString(), new HellishResistanceCreator() },
                {RacialFeats.InfernalLegacy.ToString(), new InfernalLegacyCreator() },
                #endregion
                #region Class Feats
                {ClassFeats.AbilityScoreImprovement.ToString(), new AbilityScoreImprovementCreator() },
                {ClassFeats.ExtraAttack.ToString(), new ExtraAttackCreator() },
                {ClassFeats.Cantrips.ToString(), new CantripsFeatCreator() },
                {ClassFeats.Spellcasting.ToString(), new SpellcastingCreator() },
                {ClassFeats.RitualCasting.ToString(), new RitualCastingCreator() },
                #region Barbarian
                {BarbarianFeats.UnarmoredDefenseBarbarian.ToString(), new UnarmoredDefenseBarbarianCreator() },
                {BarbarianFeats.RageFeat.ToString(),                  new RageFeatCreator() },
                {BarbarianFeats.RecklessAttack.ToString(),            new RecklessAttackCreator() },
                {BarbarianFeats.DangerSense.ToString(),               new DangerSenseCreator() },
                {BarbarianFeats.FastMovementBarbarian.ToString(),     new FastMovementBarbarianCreator() },
                {BarbarianFeats.FeralInstinct.ToString(),             new FeralInstinctCreator() },
                {BarbarianFeats.BrutalCritical.ToString(),            new BrutalCriticalCreator() },
                {BarbarianFeats.RelentlessRage.ToString(),            new RelentlessRageCreator() },
                {BarbarianFeats.PersistentRage.ToString(),            new PersistentRageCreator() },
                {BarbarianFeats.IndomitableMight.ToString(),          new IndomitableMightCreator() },
                {BarbarianFeats.PrimalChampion.ToString(),            new PrimalChampionCreator() },
                {BarbarianFeats.Frenzy.ToString(),                    new FrenzyCreator() },
                {BarbarianFeats.MindlessRage.ToString(),              new MindlessRageCreator() },
                {BarbarianFeats.IntimidatingPresence.ToString(),      new IntimidatingPresenceCreator() },
                {BarbarianFeats.Retaliation.ToString(),               new RetaliationCreator() },
                #endregion
                #region Bard
                {BardFeats.BardCantripFeat.ToString(),                new CantripsFeatBardCreator() },
                {BardFeats.BardSpellcasting.ToString(),               new SpellcastingBardCreator() },
                {BardFeats.BardicInspirationGranter.ToString(),                new BardicInspirationGranterCreator() },
                {BardFeats.JackOfAllTrades.ToString(),               new JackOfAllTradesCreator() },
                {BardFeats.SongOfRest.ToString(),                new SongOfRestCreator() },
                {BardFeats.ExpertiseBard.ToString(),               new ExpertiseBardCreator() },
                {BardFeats.FontofInspiration.ToString(),                new FontOfInspirationCreator() },
                {BardFeats.Countercharm.ToString(),               new CounterCharmCreator() },
                {BardFeats.MagicalSecrets.ToString(),                new MagicalSecretsCreator() },
                {BardFeats.SuperiorInspiration.ToString(),               new SuperiorInspirationCreator() },
                {BardFeats.BonusProficienciesLore.ToString(),               new BonusProficienciesLoreCreator() },
                {BardFeats.CuttingWords.ToString(),               new CuttingWordsCreator() },
                {BardFeats.PeerlessSkill.ToString(),               new PeerlessSkillCreator() },
                #endregion
                #endregion
                {Feats.FighterFightingStyle.ToString(), new FighterFightingStyleCreator() },
                {Feats.SecondWind.ToString(), new SecondWindCreator() },
                {Feats.DarkDevotion.ToString(), new DarkDevotionCreator() },
                {Feats.NimbleEscape.ToString(), new NimbleEscapeCreator() }

            };
        }

        #region Racial Feats

        #region Dwarves

        public class DwarvenResilienceCreator : FeatCreator
        {

            public override OldFeat CreateFeat()
            {
                return new DwarvenResilience();
            }
        }

        public class DwarvenResilience : OldFeat
        {
            public DwarvenResilience() : base()
            {
                name = "Dwarven Resilience";
                description = "You have advantage on saving throws against poison, and resistance against poison damage";
                preRoll = true;
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                if (!creature.resistances.Contains(DamageType.Poison.ToString()))
                {
                    creature.resistances.Add(DamageType.Poison.ToString());
                }
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is SavingThrow && roll.rollTags.Contains(MiscRollTags.Poison.ToString()))
                {
                    roll.advantage = true;
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        public class DwarvenCombatTrainingCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DwarvenCombatTraining();
            }
        }

        public class DwarvenCombatTraining : OldFeat
        {
            public DwarvenCombatTraining() : base()
            {
                name = "Dwarven Combat Training";
                description = "You have proficiency with the battleaxe, handaxe, light hammer, and warhammer.";
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                if (!creature.weaponTypeProficiencies.Contains(WeaponType.Battleaxe))
                {
                    creature.weaponTypeProficiencies.Add(WeaponType.Battleaxe);
                }
                if (!creature.weaponTypeProficiencies.Contains(WeaponType.Handaxe))
                {
                    creature.weaponTypeProficiencies.Add(WeaponType.Handaxe);
                }
                if (!creature.weaponTypeProficiencies.Contains(WeaponType.LightHammer))
                {
                    creature.weaponTypeProficiencies.Add(WeaponType.LightHammer);
                }
                if (!creature.weaponTypeProficiencies.Contains(WeaponType.Warhammer))
                {
                    creature.weaponTypeProficiencies.Add(WeaponType.Warhammer);
                }

            }
        }

        public class DwarvenToolProficiencyCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DwarvenToolProficiency();
            }
        }

        public class DwarvenToolProficiency : OldFeat
        {
            public DwarvenToolProficiency() : base()
            {
                name = "Tool Proficiency";
                description = "You gain proficiency with the artisan’s tools of your choice: smith’s tools, brewer’s supplies, or mason’s tools.";
                featChoices = new List<string>() { Tools.SmithsTools.ToString(), Tools.BrewersSupplies.ToString(), Tools.MasonsTools.ToString() };
                choiceMade = false;
            }

            public override void FeatChoice(string choice)
            {
                base.FeatChoice(choice);
                if (!creature.baseStats.toolProficiencies.Contains(choice))
                {
                    creature.baseStats.toolProficiencies.Add(choice);
                    choiceMade = true;
                }
            }

        }

        public class StonecunningCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Stonecunning();
            }
        }

        public class Stonecunning : OldFeat
        {
            public Stonecunning() : base()
            {
                name = "Stonecunning";
                description = "Whenever you make an Intelligence(History) check related to the origin of stonework, you are considered proficient in the History skill and add double your proficiency bonus to the check, instead of your normal proficiency bonus.";
            }
        }

        public class DwarvenToughnessCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DwarvenToughness();
            }
        }

        public class DwarvenToughness : OldFeat
        {
            public DwarvenToughness() : base()
            {
                name = "Dwarven Toughness";
                description = "Your hit point maximum increases by 1, and it increases by 1 every time you gain a level.";
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                creature.hitPointMax += creature.level;
            }

        }

        #endregion

        #region Elves 
        public class FeyAncestryCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FeyAncestry();
            }
        }

        public class FeyAncestry : OldFeat
        {
            public FeyAncestry() : base()
            {
                name = "Fey Ancestry";
                description = "You have advantage on saving throws against being charmed, and magic can’t put you to sleep.";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is SavingThrow)
                {
                    if (roll.rollTags.Contains(Conditions.Sleep.ToString()))
                    {
                        roll.autoSuccess = true;
                    }
                    if (roll.rollTags.Contains(Conditions.Charmed.ToString()))
                    {
                        roll.advantage = true;
                    }
                }
                base.FeatPreRollCheck(roll, e);

            }
        }

        public class ElfTranceCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new ElfTrance();
            }
        }

        public class ElfTrance : OldFeat
        {
            public ElfTrance() : base()
            {
                name = "Trance";
                description = "Elves don’t need to sleep. Instead, they meditate deeply, remaining semiconscious, for 4 hours a day. (The Common word for such meditation is “trance.”) While meditating, you can dream after a fashion; such dreams are actually mental exercises that have become reflexive through years of practice. After resting in this way, you gain the same benefit that a human does from 8 hours of sleep.";
            }
        }

        public class HighElfCantripCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new HighElfCantrip();
            }
        }

        public class HighElfCantrip : OldFeat
        {
            public HighElfCantrip() : base()
            {
                name = "Cantrip";
                description = "You know one cantrip of your choice from the wizard spell list. Intelligence is your spellcasting ability for it.";
            }


        }

        #endregion

        #region Halfling

        public class HalflingLuckyCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new HalflingLucky();
            }
        }

        public class HalflingLucky : OldFeat
        {
            public HalflingLucky() : base()
            {
                name = "Lucky";
                description = "When you roll a 1 on the d20 for an attack roll, ability check, or saving throw, you can reroll the die and must use the new roll.";
                postRoll = true;
            }


            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature)
                {
                    if (roll is AbilityCheck || roll is AttackRoll || roll is SavingThrow)
                    {
                        if (!roll.Success && (roll.Natural == 1 || roll.otherScore - roll.bonus == 1))
                        {
                            if (!roll.rerolled)
                            {
                                roll.advantage = false;
                                roll.disadvantage = false;
                                roll.reroll = true;
                            }
                        }
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class HalflingBraveCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new HalflingBrave();
            }
        }

        public class HalflingBrave : OldFeat
        {
            public HalflingBrave() : base()
            {
                name = "Brave";
                description = "You have advantage on saving throws against being frightened";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature)
                {
                    if (roll is SavingThrow && roll.rollTags.Contains(Conditions.Frightened.ToString()))
                    {
                        roll.advantage = true;
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        public class HalflingNimblenessCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new HalflingNimbleness();
            }
        }

        public class HalflingNimbleness : OldFeat
        {
            public HalflingNimbleness() : base()
            {
                name = "Halfling Nimbleness";
                description = "You can move through the space of any creature that is of a size larger than yours.";
            }
        }

        public class NaturallyStealthyCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new NaturallyStealthy();
            }
        }

        public class NaturallyStealthy : OldFeat
        {
            public NaturallyStealthy() : base()
            {
                name = "Naturally Stealthy";
                description = "You can attempt to hide even when you are obscured only by a creature that is at least one size larger than you.";
            }
        }

        #endregion

        #region Dragonborn

        public class DraconicAncestryCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DraconicAncestry();
            }
        }

        public class DraconicAncestry : OldFeat
        {
            public enum DragonTypes { Black, Blue, Brass, Bronze, Copper, Gold, Green, Red, Silver, White }

            public Dictionary<string, DamageType> damageType = new Dictionary<string, DamageType>()
            {
                {DragonTypes.Black.ToString(), DamageType.Acid },
                {DragonTypes.Blue.ToString(), DamageType.Lightning },
                {DragonTypes.Brass.ToString(), DamageType.Fire },
                {DragonTypes.Bronze.ToString(), DamageType.Lightning },
                {DragonTypes.Copper.ToString(), DamageType.Acid },
                {DragonTypes.Gold.ToString(), DamageType.Fire },
                {DragonTypes.Green.ToString(), DamageType.Poison },
                {DragonTypes.Red.ToString(), DamageType.Fire },
                {DragonTypes.Silver.ToString(), DamageType.Cold },
                {DragonTypes.White.ToString(), DamageType.Cold }
            };
            public Dictionary<string, Stat> saveType = new Dictionary<string, Stat>()
            {
                {DragonTypes.Black.ToString(), Stat.Dexterity },
                {DragonTypes.Blue.ToString(),  Stat.Dexterity },
                {DragonTypes.Brass.ToString(), Stat.Dexterity },
                {DragonTypes.Bronze.ToString(),Stat.Dexterity },
                {DragonTypes.Copper.ToString(),Stat.Dexterity },
                {DragonTypes.Gold.ToString(),  Stat.Dexterity },
                {DragonTypes.Green.ToString(), Stat.Constitution },
                {DragonTypes.Red.ToString(),   Stat.Dexterity },
                {DragonTypes.Silver.ToString(),Stat.Constitution },
                {DragonTypes.White.ToString(), Stat.Constitution }

            };

            public Dictionary<string, OldSpell.TargetType> targetTypes = new Dictionary<string, OldSpell.TargetType>()
            {
                {DragonTypes.Black.ToString(), OldSpell.TargetType.Line },
                {DragonTypes.Blue.ToString(),  OldSpell.TargetType.Line },
                {DragonTypes.Brass.ToString(), OldSpell.TargetType.Line },
                {DragonTypes.Bronze.ToString(),OldSpell.TargetType.Line },
                {DragonTypes.Copper.ToString(),OldSpell.TargetType.Line },
                {DragonTypes.Gold.ToString(),  OldSpell.TargetType.Cone },
                {DragonTypes.Green.ToString(), OldSpell.TargetType.Cone },
                {DragonTypes.Red.ToString(),   OldSpell.TargetType.Cone },
                {DragonTypes.Silver.ToString(),OldSpell.TargetType.Cone },
                {DragonTypes.White.ToString(), OldSpell.TargetType.Cone }
            };

            OldSpell dragonBreath = new DragonBreath();

            public DraconicAncestry() : base()
            {
                name = "Draconic Ancestry";
                description = "You have draconic ancestry. Choose one type of dragon from the Draconic Ancestry table. Your breath weapon and damage resistance are determined by the dragon type, as shown in the table.";
                choiceMade = false;
                perShortRest = true;
                featChoices = new List<string>(Enum.GetNames(typeof(DragonTypes)));
            }

            public override void FeatChoice(string choice)
            {
                base.FeatChoice(choice);
                choiceMade = true;
                if (creature.baseStats.resistances.Contains(damageType[choice].ToString())) {
                    creature.baseStats.resistances.Add(DamageType.Cold.ToString());
                }
                name = "Breath Weapon";
                description = "You can use your action to exhale destructive energy.Your draconic ancestry determines the size, shape, and damage type of the exhalation.When you use your breath weapon, each creature in the area of the exhalation must make a saving throw, the type of which is determined by your draconic ancestry.The DC for this saving throw equals 8 + your Constitution modifier + your proficiency bonus.A creature takes 2d6 damage on a failed save, and half as much damage on a successful one.The damage increases to 3d6 at 6th level, 4d6 at 11th level, and 5d6 at 16th level.After you use your breath weapon, you can’t use it again until you complete a short or long rest.";

                dragonBreath = GetDragonBreathSpell(choice);

                creature.oldSpellbook.AddSpell(dragonBreath, false);

            }

            public OldSpell GetDragonBreathSpell(string choice)
            {
                OldSpell spell = new DragonBreath();
                spell.damageTypes[0] = damageType[choice].ToString();
                spell.targetType = targetTypes[choice];
                if (spell.targetType == OldSpell.TargetType.Cone)
                {
                    spell.width = 15;
                    spell.maxRange = 15;
                }
                spell.saveStat = saveType[choice];

                return spell;

            }

            public override bool IsUsable()
            {
                if (choiceMade)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override void UseFeat()
            {
                base.UseFeat();
                if (choiceMade)
                {
                    dragonBreath.CastSpell(creature);
                }
            }
        }

        #endregion

        #region Gnome

        public class GnomeCunningCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new GnomeCunning();
            }
        }

        public class GnomeCunning : OldFeat
        {
            public GnomeCunning() : base()
            {
                name = "Gnome Cunning";
                description = "You have advantage on all Intelligence, Wisdom, and Charisma saving throws against magic";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is SavingThrow)
                {
                    SavingThrow rollSave = roll as SavingThrow;
                    if (rollSave.source is OldSpell && (rollSave.stat == Stat.Intelligence || rollSave.stat == Stat.Wisdom || rollSave.stat == Stat.Charisma))
                    {
                        roll.advantage = true;
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        #endregion

        #region Half-Elf 

        public class HalfElfStatVersatilityCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new HalfElfStatVersatility();
            }
        }

        public class HalfElfStatVersatility : OldFeat
        {
            int statChoicesLeft = 2;

            public HalfElfStatVersatility() : base()
            {
                name = "Half-Elf Stat Increases";
                description = "Two ability scores of your choice, other than Charisma, increase by 1";
                featChoices = new List<string>() { Stat.Strength.ToString(), Stat.Dexterity.ToString(), Stat.Constitution.ToString(), Stat.Intelligence.ToString(), Stat.Wisdom.ToString() };
                choiceMade = false;
            }

            public override void FeatChoice(string choice)
            {
                base.FeatChoice(choice);
                if (statChoicesLeft == 1)
                {
                    choiceMade = true;
                }
                else
                {
                    statChoicesLeft--;
                }
                creature.baseStats.ChangeStat((Stat)Enum.Parse(typeof(Stat), choice), 1);
                featChoices.Remove(choice);

            }
        }

        public class HalfElfSkillVersatilityCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new HalfElfSkillVersatility();
            }
        }

        public class HalfElfSkillVersatility : OldFeat
        {
            public int skillChoicesLeft = 2;
            public HalfElfSkillVersatility() : base()
            {
                name = "Skill Versatility";
                description = "You gain proficiency in two skills of your choice.";
                choiceMade = false;
            }

            public override bool HasChoices()
            {
                featChoices = new List<string>();
                foreach (string skill in Enum.GetNames(typeof(Skill)))
                {
                    if (skill != Skill.Choose.ToString() && !creature.skillProficiencies.Contains((Skill)Enum.Parse(typeof(Skill), skill)))
                    {
                        featChoices.Add(skill);
                    }
                }
                return base.HasChoices();
            }

            public override void FeatChoice(string choice)
            {
                base.FeatChoice(choice);
                if (skillChoicesLeft == 1)
                {
                    choiceMade = true;
                }
                else
                {
                    skillChoicesLeft--;
                }
                creature.baseStats.skillProficiencies.Add((Skill)Enum.Parse(typeof(Skill), choice));
            }
        }

        #endregion

        #region Half-Orcs

        public class RelentlessEnduranceCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new RelentlessEndurance();
            }
        }

        public class RelentlessEndurance : OldFeat
        {
            public RelentlessEndurance() : base()
            {
                name = "Relentless Endurance";
                description = "When you are reduced to 0 hit points but not killed outright, you can drop to 1 hit point instead. You can’t use this feature again until you finish a long rest.";
                perLongRest = true;
            }
            public override void AddFeat()
            {
                base.AddFeat();
                SubscribeTakeDamage(creature);
            }

            public override void FeatActivated(Creature encCreature, Creature.CreatureDelegateEventArgs e)
            {
                if (creature.hitPoints <= 0)
                {
                    Debug.WriteLine("Relentless Endurance Feat Used");
                    creature.SetHP(1);
                    usedThisRest = true;
                }
            }
        }

        public class SavageAttacksCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SavageAttacks();
            }
        }

        public class SavageAttacks : OldFeat
        {
            public SavageAttacks() : base()
            {
                name = "Savage Attacks";
                description = "When you score a critical hit with a melee weapon attack, you can roll one of the weapon’s damage dice one additional time and add it to the extra damage of the critical hit.";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll is DamageRoll && roll.roller == creature)
                {
                    DamageRoll rollDam = roll as DamageRoll;
                    if (rollDam.critical)
                    {
                        rollDam.numberOfDice[rollDam.diceFaces.IndexOf(rollDam.diceFaces.Max())]++;
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }
        }
        #endregion

        #region Tiefling 

        public class HellishResistanceCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new HellishResistance();
            }
        }

        public class HellishResistance : OldFeat
        {
            public HellishResistance() : base()
            {
                name = "Hellish Resistance";
                description = "You have resistance to fire damage";
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                if (!creature.resistances.Contains(DamageType.Fire.ToString()))
                {
                    creature.resistances.Add(DamageType.Fire.ToString());
                }
            }
        }

        public class InfernalLegacyCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new InfernalLegacy();
            }
        }

        public class InfernalLegacy : OldFeat
        {
            public InfernalLegacy() : base()
            {
                name = "Infernal Legacy";
                description = "You know the thaumaturgy cantrip. When you reach 3rd level, you can cast the hellish rebuke spell as a 2nd -­‐‑level spell once with this trait and regain the ability to do so when you finish a long rest. When you reach 5th level, you can cast the darkness spell once with this trait and regain the ability to do so when you finish a long rest. Charisma is your spellcasting ability for these spells.";
            }
        }

        #endregion

        #endregion

        #region Class Feats

        public enum BarbarianFeats { RageFeat, UnarmoredDefenseBarbarian, RecklessAttack, DangerSense, FastMovementBarbarian, FeralInstinct,
            BrutalCritical, RelentlessRage, PersistentRage, IndomitableMight, PrimalChampion, Frenzy, MindlessRage, IntimidatingPresence,
            Retaliation };
        public enum BardFeats { BardCantripFeat, BardSpellcasting, BardicInspirationGranter, JackOfAllTrades, SongOfRest, ExpertiseBard, FontofInspiration, Countercharm, MagicalSecrets, SuperiorInspiration, BonusProficienciesLore, CuttingWords, AdditionalMagicalSecrets, PeerlessSkill}

        #region General Feats

        public class ExtraAttackCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new AbilityScoreImprovement();
            }
        }

        public class ExtraAttack : OldFeat
        {

            public ExtraAttack() : base()
            {
                name = "Extra Attack";
                statChange = true;
            }
            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                if (creature.attacks < 2)
                {
                    creature.attacks = 2;
                }
            }
        }

        public class AbilityScoreImprovementCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new AbilityScoreImprovement();
            }
        }

        public class AbilityScoreImprovement : OldFeat
        {
            int choices = 2;
            public AbilityScoreImprovement() : base()
            {
                name = "Ability Score Improvement";
                choiceMade = false;
                featChoices = new List<string>() { Stat.Strength.ToString(), Stat.Dexterity.ToString(), Stat.Constitution.ToString(), Stat.Intelligence.ToString(), Stat.Wisdom.ToString(), Stat.Charisma.ToString() };
            }

            public override bool HasChoices()
            {
                List<string> oldFeatChoices = new List<string>(featChoices);
                foreach (string s in oldFeatChoices)
                {
                    if (!creature.CheckFeat(BarbarianFeats.PrimalChampion.ToString()))
                    {
                        if (creature.stats[(Stat)Enum.Parse(typeof(Stat), s)] >= 20)
                        {
                            featChoices.Remove(s);
                        }
                    }
                    else
                    {
                        if (creature.stats[(Stat)Enum.Parse(typeof(Stat), s)] >= 24)
                        {
                            featChoices.Remove(s);
                        }
                    }

                }
                return base.HasChoices();
            }

            public override void FeatChoice(string choice)
            {
                base.FeatChoice(choice);
                creature.baseStats.ChangeStat((Stat)Enum.Parse(typeof(Stat), choice), 1);
                choices--;
                if (choices < 1)
                {
                    choiceMade = true;
                }

            }
        }

        public class CantripsFeatCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new CantripsFeat();
            }
        }

        public class CantripsFeat : OldFeat
        {
            public List<OldSpell> cantrips = new List<OldSpell>();
            public List<OldSpell> cantripChoices = new List<OldSpell>();

            public int NumberOfChoices { get { return GetNumberOfChoices(); } }
            public PlayerClass playerClass;

            public CantripsFeat() : base()
            {
                name = "Cantrips";
                statChange = true;
            }

            public override bool HasChoices()
            {
                if (cantrips.Count >= NumberOfChoices)
                {
                    return false;
                }
                featChoices = new List<string>();
                foreach (OldSpell spell in cantripChoices)
                {
                    if (!creature.oldSpellbook.HasSpellOfName(spell.name))
                    {
                        featChoices.Add(spell.name);
                    }
                }
                return base.HasChoices();
            }

            public override void FeatChoice(string choice)
            {
                base.FeatChoice(choice);
                foreach (OldSpell spell in cantripChoices)
                {
                    if (spell.name == choice)
                    {
                        cantrips.Add(spell);
                        cantripChoices.Remove(spell);
                    }
                }
            }

            public override void StatChange(Creature creature)
            {
                base.StatChange(creature);
                creature.oldSpellbook.AddSpells(cantrips, false);
            }

            public virtual int GetNumberOfChoices()
            {
                return 0;
            }
        }

        public class SpellcastingCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SpellCasting();
            }
        }

        public class SpellCasting : OldFeat
        {
            public List<OldSpell> spells = new List<OldSpell>();
            public List<OldSpell> spellChoices = new List<OldSpell>();
            public PlayerClass playerClass;
            public int classLevel;
            public SpellCasterType spellcasterType;

            public int spellsToChoose;

            public Type spellcastingFocus;

            public SpellCasting() : base()
            {
                name = "Spellcasting";
                statChange = true;
            }

            public override bool HasChoices()
            {
                if (spellsToChoose > spellChoices.Count)
                {
                    GetChoiceMenu();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public Dictionary<int, List<OldSpell>> spellChoicesByLevel;

            public void GetChoiceMenu()
            {
                featChoices = new List<string>();
                for (int level = 1; level <= 9; level++)
                {
                    if (creature.spellSlots.HasSpellSlotOfLevel(level))
                    {
                        featChoices.Add(string.Format("Level {0}", level));
                    }
                }
            }

            public override ContextMenuTemplate FeatChoiceChildMenu(string choice)
            {
                ContextMenuTemplate template = BaseChildMenuTemplate();
                int level = (choice[choice.Length - 1]) - '0';
                foreach (OldSpell spell in spellChoicesByLevel[level])
                {
                    if (!creature.oldSpellbook.HasSpellOfName(spell.name))
                    {
                        template.texts.Add(spell.name);
                        template.tags.Add(GetChoiceChildMenuTags(new List<string>() { spell.name }));
                    }
                }
                return template;
            }

            public override void FeatChoice(string choice)
            {
                base.FeatChoice(choice);
                OldSpell spellChosen = null;
                foreach (OldSpell spell in spellChoices)
                {
                    if (spell.name == choice)
                    {
                        spells.Add(spell);
                        spellChosen = spell;
                        break;
                    }
                }
                spellChoices.Remove(spellChosen);
                creature.RecalibrateStats();
            }

            public override void StatChange(Creature creature)
            {
                base.StatChange(creature);
                creature.oldSpellbook.AddSpells(spells, false);
                creature.spellSlots.SpellcastingFeat(classLevel, spellcasterType);
            }
        }

        public class RitualCastingCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new RitualCasting();
            }
        }
       
        public class RitualCasting : OldFeat
        {
            PlayerClass spellClass;

            public RitualCasting() : base()
            {
                name = "Ritual Caster";
            }
        }
        #endregion

        #region Barbarian Feats

        public class RageFeatCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new RageFeat();
            }
        }

        public class RageFeat : OldFeat
        {
            public new int MaxUses
            {
                get
                {
                    if (creature == null)
                    {
                        return 0;
                    }
                    if (creature.level < 3)
                    {
                        return 2;
                    }
                    else if (creature.level < 6)
                    {
                        return 3;
                    }
                    else if (creature.level < 12)
                    {
                        return 4;
                    }
                    else if (creature.level < 17)
                    {
                        return 5;
                    }
                    else if (creature.level < 20)
                    {
                        return 6;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            public RageCondition rage = new RageCondition();

            public RageFeat() : base()
            {
                name = "Rage";
                bonusAction = true;
                perLongRest = true;
            }

            public override void ResetUses()
            {
                uses = MaxUses;
            }

            public override void UseFeat()
            {
                //Debug.WriteLine(uses);
                base.UseFeat();
                rage = creature.AddFeat(new RageCondition()) as RageCondition;
            }

            public override bool IsUsable()
            {
                if (creature.armor.armorCategory == ArmorCategories.HeavyArmor)
                {
                    return false;
                }
                if (creature.CheckFeat(typeof(RageCondition)))
                {
                    return false;
                }
                return base.IsUsable();
            }
        }

        public class UnarmoredDefenseBarbarianCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new UnarmoredDefenseBarbarian();
            }
        }

        public class UnarmoredDefenseBarbarian : OldFeat
        {
            public UnarmoredDefenseBarbarian() : base()
            {
                name = "Unarmored Defense";
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                if (creature.armor.baseArmorType == ArmorTypes.NoArmor)
                {
                    creature.armor = new BarbarianUnarmoredDefenseArmor();
                }
            }
        }

        public class RecklessAttackCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new RecklessAttack();
            }
        }

        public class RecklessAttack : OldFeat
        {
            bool usedThisTurn = false;

            public RecklessAttack() : base()
            {
                name = "Reckless Attack";
                optionsDisplayDescription = "Attack Recklessly? (Get Advantage, but incur advantage on all attacks against you until your next turn";
                preRoll = true;
                startTurn = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is AttackRoll)
                {
                    AttackRoll rollAtt = roll as AttackRoll;
                    if (usedThisTurn)
                    {
                        if (!rollAtt.attack.isSpell && rollAtt.attack.IsMelee && rollAtt.attack.statUsed == Stat.Strength)
                        {
                            roll.advantage = true;
                        }
                        base.FeatPreRollCheck(roll, e);
                    }
                    else if (creature.attacksTaken == 1)
                    {
                        MakeOptionsDisplay(rollAtt);
                    }
                    else
                    {
                        base.FeatPreRollCheck(roll, e);
                    }
                }
                else
                {
                    base.FeatPreRollCheck(roll, e);
                }
            }

            public override void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayYes(dO, e);
                AttackRoll currentRoll = (dO as DisplayOptionsDnD).roll as AttackRoll;
                usedThisTurn = true;
                if (!currentRoll.attack.isSpell && currentRoll.attack.IsMelee && currentRoll.attack.statUsed == Stat.Strength)
                {
                    currentRoll.advantage = true;
                }

                FeatPreRollFinished(currentRoll);
            }

            public override void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayNo(dO, e);
                FeatPreRollFinished((dO as DisplayOptionsDnD).roll);
            }

            public override void StartTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                base.StartTurn( cr, e);
                usedThisTurn = false;
            }
        }

        public class DangerSenseCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DangerSense();
            }
        }

        public class DangerSense : OldFeat
        {
            public DangerSense() : base()
            {
                name = "Danger Sense";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is SavingThrow && (roll as SavingThrow).stat == Stat.Dexterity)
                {
                    if (!(creature.CheckFeat(typeof(Blinded)) || creature.CheckFeat(typeof(Deafened)) || creature.CheckFeat(typeof(Incapacitated))))
                    {
                        roll.advantage = true;
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        public class FastMovementBarbarianCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FastMovementBarbarian();
            }
        }

        public class FastMovementBarbarian : OldFeat
        {
            bool heavyArmorReduced = false;

            public FastMovementBarbarian() : base()
            {
                name = "Fast Movement";
                statChange = true;
            }

            // public override void StatChange(Creature cr)
            // {
            //     base.StatChange(cr);
            //     if (creature.armor.armorCategory != ArmorCategories.HeavyArmor)
            //     {
            //         creature.speed += 10;
            //     }
            // }

        }

        public class FeralInstinctCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FeralInstinct();
            }
        }

        public class FeralInstinct : OldFeat
        {
            public FeralInstinct() : base()
            {
                name = "Feral Instinct";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is InitiativeRoll)
                {
                    roll.advantage = true;
                }
                base.FeatPreRollCheck(roll, e);
            }

            public override bool IsUsable()
            {
                if (creature.CheckCondition(Conditions.Surprised.ToString()))
                {
                    return true;
                }
                return base.IsUsable();
            }

            public override void UseFeat()
            {
                base.UseFeat();
                creature.RemoveCondition(Conditions.Surprised.ToString());
                creature.bonusActionTaken = true;
                creature.AddCondition(Conditions.Rage.ToString());
            }

        }

        public class BrutalCriticalCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new BrutalCritical();
            }
        }

        public class BrutalCritical : OldFeat
        {
            public BrutalCritical() : base()
            {
                name = "Brutal Critical";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is DamageRoll)
                {
                    DamageRoll rollDam = roll as DamageRoll;
                    if (rollDam.attack != null && rollDam.attack.IsMelee && rollDam.critical)
                    {
                        rollDam.numberOfDice[0]++;
                        if (creature.level >= 13)
                        {
                            rollDam.numberOfDice[0]++;
                        }
                        if (creature.level >= 17)
                        {
                            rollDam.numberOfDice[0]++;
                        }
                    }
                }
                base.FeatPreRollCheck(roll, e);

            }
        }

        public class RelentlessRageCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new RelentlessRage();
            }
        }

        public class RelentlessRage : OldFeat
        {
            int DC = 10;
            Roll damageRoll;
            Roll saveRoll;


            public RelentlessRage() : base()
            {
                name = "Relentless Rage";
            }

            public override void AddFeat()
            {
                base.AddFeat();
                SubscribeShortRest(creature);
                SubscribeTakeDamage(creature);
            }

            public override void FeatActivated(Creature encCreature, Creature.CreatureDelegateEventArgs e)
            {
                if (creature.hitPoints <= 0)
                {
                    Debug.WriteLine("Relentless Rage Feat Used");
                    saveRoll = new SavingThrow(Stat.Constitution, creature, this);
                    saveRoll.finishRoll += new RollDelegate(RollMade);
                }
            }

            public void RollMade(Roll saveRoll, RollEventArgs e)
            {
                if (saveRoll.Success)
                {
                    Debug.WriteLine(string.Format("{0} succeeded using {1}", creature.name, name));
                    creature.SetHP(1);
                }
                else
                {
                    damageRoll.FinishRoll();
                }
                DC += 5;
            }

            public override void ShortRest(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                base.ShortRest(cr, e);
                DC = 10;
            }
        }

        public class PersistentRageCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new PersistentRage();
            }
        }

        public class PersistentRage : OldFeat
        {
            public PersistentRage() : base()
            {
                name = "Persistent Rage";
            }
        }

        public class IndomitableMightCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new IndomitableMight();
            }
        }

        public class IndomitableMight : OldFeat
        {
            public IndomitableMight() : base()
            {
                name = "Indomitable Might";
                postRoll = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is AbilityCheck)
                {
                    if ((roll as AbilityCheck).stat == Stat.Strength)
                    {
                        if (roll.score < creature.stats[Stat.Strength])
                        {
                            roll.score = creature.stats[Stat.Strength];
                        }
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class PrimalChampionCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new PrimalChampion();
            }
        }

        public class PrimalChampion : OldFeat
        {
            public PrimalChampion() : base()
            {
                name = "Primal Champion";
            }

            public override void AddFeat()
            {
                base.AddFeat();
                creature.baseStats.stats[Stat.Strength] = Math.Min(creature.baseStats.stats[Stat.Strength] + 4, 24);
                creature.baseStats.stats[Stat.Constitution] = Math.Min(creature.baseStats.stats[Stat.Constitution] + 4, 24);
            }
        }

        public class FrenzyCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Frenzy();
            }
        }

        public class Frenzy : OldFeat
        {
            OldFeat rageFeat;
            bool frenzied = false;
            List<Weapon> weaponList;

            public Frenzy() : base()
            {
                name = "Frenzy";

            }

            public override void AddFeat()
            {
                base.AddFeat();
                foreach (OldFeat feat in creature.oldFeats)
                {
                    if (feat is RageFeat)
                    {
                        rageFeat = feat;
                        break;
                    }
                }
            }

            public override bool IsUsable()
            {
                if (frenzied && !creature.bonusActionTaken)
                {
                    return true;
                }
                if (rageFeat.IsUsable())
                {
                    return true;
                }
                return base.IsUsable();
            }

            public override void UseFeat()
            {
                base.UseFeat();
                if (!frenzied)
                {
                    frenzied = true;
                    name = "Frenzied Attack";
                }
            }

            public override ContextMenuTemplate ChildMenu()
            {
                if (!frenzied || creature.bonusActionTaken)
                {
                    return null;
                }
                ContextMenuTemplate template = BaseChildMenuTemplate();
                weaponList = creature.GetMeleeWeapons();
                for (int i = 0; i < weaponList.Count; i++)
                {
                    template.texts.Add(weaponList[i].name);
                    template.tags.Add(MakeTags(new List<string>() { i.ToString() }));
                }
                return template;
            }

            public override void UseFeatChildMenu(List<string> tags)
            {
                base.UseFeatChildMenu(tags);
                AttackOrder attackOrder = new AttackOrder(creature, weaponList[int.Parse(tags[0])], false, true);
                attackOrder.linkedFeat = this;
            }

            public override void SelectionMadeOrder(Control order)
            {
                base.SelectionMadeOrder(order);
                creature.bonusActionTaken = true;
            }

            public void RageEnded()
            {
                if (frenzied)
                {
                    frenzied = false;
                    creature.GainExhaustion();
                }
            }

        }

        public class MindlessRageCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new MindlessRage();
            }
        }

        public class MindlessRage : OldFeat
        {
            public MindlessRage() : base()
            {
                name = "Mindless Rage";
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                if (creature.CheckFeat(typeof(Charmed)))
                {
                    creature.RemoveFeat(typeof(Frightened), true);
                }
                if (creature.CheckFeat(typeof(Frightened)))
                {
                    creature.RemoveFeat(typeof(Frightened), true);
                }
            }
        }

        public class IntimidatingPresenceCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new IntimidatingPresence();
            }
        }

        public class IntimidatingPresence : OldFeat
        {
            IntimidatingPresenceSpell intimidatingPresence;
            public bool usedThisTurn = false;

            public IntimidatingPresence() : base()
            {
                name = "Intimidating Presence";
                action = true;
                intimidatingPresence = new IntimidatingPresenceSpell();
                intimidatingPresence.linkedFeat = this;
                startTurn = true;
                endTurn = true;
            }

            public override void AddFeat()
            {
                base.AddFeat();
                creature.oldSpellbook.AddSpell(intimidatingPresence);
            }

            public override void UseFeat()
            {
                base.UseFeat();
                intimidatingPresence.CastSpell(creature);
            }

            public override void StartTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                base.StartTurn(cr, e);
                usedThisTurn = false;
            }

            public override void EndTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                if (!usedThisTurn)
                {
                    intimidatingPresence.fearThisTurn = null;
                }
                foreach (OldCondition con in intimidatingPresence.fears)
                {
                    if (con != intimidatingPresence.fearThisTurn)
                    {
                        intimidatingPresence.affectedCreatures.Remove(con.creature);
                        con.RemoveFromCreature();
                    }
                }
                base.EndTurn(cr, e);

            }
        }

        public class RetaliationCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Retaliation();
            }
        }

        public class Retaliation : OldFeat
        {
            public Retaliation() : base()
            {
                name = "Retaliation";
                optionsDisplayDescription = "Use reaction to make retaliation attack?";
                reaction = true;
            }

            public override void AddFeat()
            {
                base.AddFeat();
                SubscribeTakeDamage(creature);
            }

            public override void FeatActivated(Creature encCreature, Creature.CreatureDelegateEventArgs e)
            {
                if (!encCreature.reactionTaken && e.amount > 0)
                {
                    DisplayOptionsDnD displayOptions = MakeOptionsDisplay(null);
                    displayOptions.creature = e.GetCreatureSource();
                }
            }

            public override void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayYes(dO, e);
                creature.reactionTaken = true;
                creature.Attack((dO as DisplayOptionsDnD).creature, creature.weaponMainHand, 1, false, false, false);

            }

            public override void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayNo(dO, e);
            }
        }
        #endregion

        #region Bard Feats

        public class CantripsFeatBardCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new CantripsFeatBard();
            }
        }

        public class CantripsFeatBard : CantripsFeat
        {
            public CantripsFeatBard() : base()
            {
                playerClass = DnDManager.classes[Classes.Bard.ToString()];
                GetBardCantripList();
            }

            public void GetBardCantripList()
            {
                foreach (string s in DnDManager.spellLists[Classes.Bard.ToString()][0])
                {
                    if (DnDManager.spells.ContainsKey(s))
                    {
                        OldSpell spell = new OldSpell(DnDManager.spells[s]);
                        spell.abilityModifier = Stat.Charisma;
                        cantripChoices.Add(spell);
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Spell {0} not found for bard cantrip feat", s));
                    }
                }
            }

            public override int GetNumberOfChoices()
            {
                if (creature.classes[playerClass] < 4)
                {
                    return 2;
                }
                else if (creature.classes[playerClass] < 10)
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }
        }

        public class SpellcastingBardCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SpellCastingBard();
            }
        }

        public class SpellCastingBard : SpellCasting
        {
            public SpellCastingBard() : base()
            {
                GetBardSpellList();
                spellcasterType = SpellCasterType.Full;
                playerClass = DnDManager.classes[Classes.Bard.ToString()];
                spellcastingFocus = typeof(MusicalInstrument);
                statOverride = true;
            }

            public void GetBardSpellList()
            {
                foreach (int level in DnDManager.spellLists[Classes.Bard.ToString()].Keys)
                {
                    foreach (string s in DnDManager.spellLists[Classes.Bard.ToString()][level])
                    {
                        if (DnDManager.spells.ContainsKey(s))
                        {
                            OldSpell spell = new OldSpell(DnDManager.spells[s]);
                            spell.abilityModifier = Stat.Charisma;
                            spellChoices.Add(spell);
                        }
                        else
                        {
                            Debug.WriteLine(string.Format("Spell {0} not found for bard spell list", s));
                        }
                    }
                }
                spellChoicesByLevel = OldSpellBook.SortSpellsByLevelStatic(spellChoices);
            }

            public override void StatOverride(Creature cr)
            {
                classLevel = creature.level;
                spellsToChoose = spellsToChooseByLevel[creature.level];
                base.StatOverride(cr);
            }

            public Dictionary<int, int> spellsToChooseByLevel = new Dictionary<int, int>()
            {
                {1, 4 }, {2, 5 }, {3, 6 }, {4, 7 }, {5, 8 }, {6, 9 }, {7, 10 }, {8, 11 }, {9, 12 }, {10, 12 },
                {11, 13}, {12, 13}, {13, 14 }, {14, 14}, {15, 15 }, {16, 15 }, {17, 16 }, {18, 16 }, {19, 16 }, {20, 16 }
            };
        }

        public class BardicInspirationGranterCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new BardicInspirationGranter();
            }
        }

        public class BardicInspirationGranter : OldFeat
        {
            public int die;

            public BardicInspirationGranter() : base()
            {
                name = "Bardic Inspiration";
                statOverride = true;
                bonusAction = true;
                perLongRest = true;
                finishRoll = true;
            }

            public override void AddFeat()
            {
                base.AddFeat();
                uses = 0;
            }

            public override void UseFeat()
            {
                base.UseFeat();
                BardicInspiration inspiration = new BardicInspiration();
                inspiration.die = die;
                currentControl.orderControl.selection.creature.AddFeat(inspiration);
            }

            public override void StatOverride(Creature cr)
            {
                if (creature.CheckFeat(typeof(FontOfInspiration))){
                    perShortRest = true;
                }
                if (creature.level < 5)
                {
                    die = 6;
                }
                else if (creature.level < 10)
                {
                    die = 8;
                }
                else if (creature.level < 15)
                {
                    die = 10;
                }
                else
                {
                    die = 12;
                }
                base.StatOverride(cr);
            }

            public override void FeatFinishRollCheck(Roll roll, RollEventArgs e)
            {
                if (creature.CheckFeat(typeof(SuperiorInspiration)))
                {
                    ResetUses();
                }
                base.FeatFinishRollCheck(roll, e);
            }

            public override int GetMaxUses()
            {
                return creature.StatMod(Stat.Charisma);
            }
        }

        public class BardicInspirationCreator : ConditionCreator
        {
            public override OldFeat CreateFeat()
            {
                return new BardicInspiration();
            }
        }

        public class BardicInspiration : OldCondition
        {
            public int die;

            public BardicInspiration() : base()
            {
                name = "Bardic Inspiration";
                postRoll = true;
                description = "Use bardic inspiration?";
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is AbilityCheck || roll is SavingThrow || roll is AttackRoll)
                MakeOptionsDisplay(roll);
            }

            public override void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                Roll roll = (dO as DisplayOptionsDnD).roll;

                int inspirationResult = EngManager.random.Next(1, die + 1);
                roll.Bonus += inspirationResult;
                creature.RemoveFeat(this);
                base.OptionsDisplayYes(dO, e);
                FeatPostRollFinished(roll);
            }

            public override void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayNo(dO, e);
                FeatPostRollFinished((dO as DisplayOptionsDnD).roll);
            }

        }

        public class JackOfAllTradesCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new JackOfAllTrades();
            }
        }

        public class JackOfAllTrades : OldFeat
        {
            public JackOfAllTrades()
            {
                name = "Jack of All Trades";
                postRoll = true;
                range = 120;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll is AbilityCheck && roll.roller == creature)
                {
                    if (!creature.Proficiencies.Contains((roll as AbilityCheck).proficiency))
                    {
                        roll.bonus += (int)(creature.proficiencyBonus / 2);
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }

        }

        public class SongOfRestCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SongOfRest();
            }
        }

        public class SongOfRest : OldFeat
        {
            int die = 6;

            public SongOfRest() : base()
            {
                name = "Song of Rest";
                statOverride = true;
                perShortRest = true;
            }

            public override void AddFeat()
            {
                base.AddFeat();
            }

            public override void StatOverride(Creature cr)
            {
                if (creature.level < 9)
                {
                    die = 6;
                }
                else if (creature.level < 13)
                {
                    die = 8;
                }
                else if (creature.level < 17)
                {
                    die = 10;
                }
                else
                {
                    die = 12;
                }
                base.StatOverride(cr);
            }

            public override void ShortRest(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                foreach ( BoardTile tile in creature.boardTile.board.GetTilesInRange(creature.boardTile, 120, true))
                {
                    if (tile.creature != null)
                    {
                        creature.Heal(die, 1, true);
                    }
                }
                base.ShortRest(cr, e);
            }
        }

        public class ExpertiseBardCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new ExpertiseBard();
            }
        }

        public class ExpertiseBard : OldFeat
        {
            public List<Skill> expertises;
            int choicesLeft = 2;
            bool above10ChoicesDone = false;

            public ExpertiseBard() : base()
            {
                name = "Expertise";
                statChange = true;
                expertises = new List<Skill>();
            }

            public override bool HasChoices()
            {
                if (!above10ChoicesDone)
                {
                    if (creature.level >= 10)
                    {
                        choicesLeft += 2;
                        above10ChoicesDone = true;
                    }
                }
                featChoices = GetSkillChoices();
                return base.HasChoices();
            }

            public List<string> GetSkillChoices()
            {
                if (choicesLeft < 1)
                {
                    return null;
                }
                List<string> choices = new List<string>();
                foreach (Skill skill in creature.skillProficiencies)
                {
                    if (expertises.Contains(skill))
                    {
                        choices.Add(skill.ToString());
                    }
                }
                return choices;
            }

            public override void FeatChoice(string choice)
            {
                expertises.Add((Skill)Enum.Parse(typeof(Skill), choice));
                choicesLeft--;
                base.FeatChoice(choice);
            }

            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                foreach (Skill skill in expertises)
                {
                    if (!creature.skillProficiencies.Contains(skill))
                    {
                        creature.skillProficiencies.Add(skill);
                    }
                }
            }
        }

        public class FontOfInspirationCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FontOfInspiration();
            }
        }

        public class FontOfInspiration : OldFeat
        {
            public FontOfInspiration() : base()
            {
                name = "Font of Inspiration";
            }
        }

        public class SuperiorInspirationCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SuperiorInspiration();
            }
        }

        public class SuperiorInspiration : OldFeat
        {
            public SuperiorInspiration() : base()
            {
                name = "Superior Inspiration";
            }
        }

        public class CounterCharmCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new CounterCharm();
            }
        }

        public class CounterCharm : OldFeat
        {
            public bool active = true;
            bool usedThisTurn = false;

            public CounterCharm() : base()
            {
                name = "Countercharm";
                action = true;
                preRoll = true;
                endTurn = true;
            }

            public override void UseFeat()
            {
                base.UseFeat();
                usedThisTurn = true;
                active = true;
            }

            public override void EndTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                if (active && !usedThisTurn)
                {
                    active = false;
                }
                base.EndTurn(cr, e);
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll is SavingThrow && creature.boardTile.board.GetDistance(creature.boardTile, roll.roller.boardTile) <= 30)
                {
                    if (roll.rollTags.Contains(Conditions.Frightened.ToString()) || roll.rollTags.Contains(Conditions.Charmed.ToString()))
                    {
                        if (roll.roller.team == creature.team)
                        {
                            roll.advantage = true;
                        }
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        public class MagicalSecretsCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new MagicalSecrets();
            }
        }

        public class MagicalSecrets : SpellCasting
        {
            public MagicalSecrets() : base()
            {
                GetCompleteSpellList();
                spellcasterType = SpellCasterType.Full;
                playerClass = DnDManager.classes[Classes.Bard.ToString()];
                statOverride = true;
            }

            public void GetCompleteSpellList()
            {
                spellChoices = new List<OldSpell>();
                foreach (string s in DnDManager.spells.Keys)
                {
                    if (DnDManager.spells.ContainsKey(s))
                    {
                        OldSpell spell = new OldSpell(DnDManager.spells[s]);
                        spell.abilityModifier = Stat.Charisma;
                        spellChoices.Add(spell);
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Spell {0} not found for complete spell list", s));
                    }
                }
                spellChoicesByLevel = OldSpellBook.SortSpellsByLevelStatic(spellChoices);
            }

            public override void StatOverride(Creature cr)
            {
                classLevel = creature.level;
                if (classLevel < 14)
                {
                    spellsToChoose = 2;
                }
                else if (classLevel < 18)
                {
                    spellsToChoose = 4;
                }
                else
                {
                    spellsToChoose = 6;
                }
                if (creature.HasSubClass(typeof(LoreBard)) && creature.level >= 6)
                {
                    spellsToChoose += 2;
                }                                               
                base.StatOverride(cr);
            }
        }

        public class BonusProficienciesLoreCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new BonusProficienciesLore();
            }
        }

        public class BonusProficienciesLore : HalfElfSkillVersatility
        {
            public BonusProficienciesLore() : base()
            {
                name = "Bonus Proficiencies";
                skillChoicesLeft = 3;
            }
        }

        public class CuttingWordsCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new CuttingWords();
            }
        }

        public class CuttingWords : OldFeat
        {

            BardicInspirationGranter bardicFeat;

            public CuttingWords() : base()
            {
                name = "Cutting Words";
                reaction = true;
                postRoll = true;
                statOverride = true;
                range = 60;
                description = "Use Cutting words?";
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if ((roll is AbilityCheck || roll is AttackRoll || roll is DamageRoll) && creature.boardTile.board.GetDistance(roll.roller.boardTile, creature.boardTile) < 60 && bardicFeat != null)
                {
                    description = string.Format("Use Cutting Words (d{0}) on {1}?", bardicFeat.die, roll.roller);
                    MakeOptionsDisplay(roll);
                }

                else
                {
                    base.FeatPostRollCheck(roll, e);
                }
            }

            public override void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                Roll currentRoll = (dO as DisplayOptionsDnD).roll;

                currentRoll.Bonus -= EngManager.random.Next(1, bardicFeat.die);
                bardicFeat.uses++;
                creature.reactionTaken = true;
                base.OptionsDisplayYes(dO, e);
                FeatPostRollFinished(currentRoll);
            }

            public override void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayNo(dO, e);
                FeatPostRollFinished((dO as DisplayOptionsDnD).roll);
            }

        }

        public class PeerlessSkillCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new PeerlessSkill();
            }
        }

        public class PeerlessSkill : OldFeat
        {

            BardicInspirationGranter bardicFeat;

            public PeerlessSkill() : base()
            {
                name = "Peerless Skill";
                postRoll = true;
                statOverride = true;
                description = "Use Peerless Skill?";
            }


            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && (roll is AbilityCheck) && bardicFeat != null)
                {
                    description = string.Format("Use Peerless Skill (d{0}) on {1}?", bardicFeat.die, roll.roller);
                    MakeOptionsDisplay(roll);
                }

                else
                {
                    base.FeatPostRollCheck(roll, e);
                }
            }

            public override void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                Roll currentRoll = (dO as DisplayOptionsDnD).roll;
                currentRoll.Bonus += EngManager.random.Next(1, bardicFeat.die);
                bardicFeat.uses++;
                base.OptionsDisplayYes(dO, e);
                FeatPostRollFinished(currentRoll);
            }

            public override void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayNo(dO, e);
                FeatPostRollFinished((dO as DisplayOptionsDnD).roll);
            }

        }
        #endregion

        #region Cleric Feats

        public class BonusProficiencyLifeDomainCreator : CantripsFeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new BonusProficiencyLifeDomain();
            }
        }

        public class BonusProficiencyLifeDomain : OldFeat
        {
            public BonusProficiencyLifeDomain() : base()
            {
                name = "Bonus Proficiency";
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                if (!creature.armorCategoryProficiencies.Contains(ArmorCategories.HeavyArmor))
                {
                    creature.armorCategoryProficiencies.Add(ArmorCategories.HeavyArmor);
                }
                base.StatChange(cr);
            }
        }

        public class CantripFeatClericCreator : CantripsFeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new CantripFeatCleric();
            }
        }

        public class CantripFeatCleric : CantripsFeat
        {
            public CantripFeatCleric() : base()
            {
                cantripChoices = GetClericCantripChoices();
                playerClass = DnDManager.classes[Classes.Cleric.ToString()];
            }

            public List<OldSpell> GetClericCantripChoices()
            {
                List<OldSpell> list = new List<OldSpell>();
                foreach (string s in DnDManager.spellLists[Classes.Cleric.ToString()][0])
                {
                    if (DnDManager.spells.ContainsKey(s))
                    {
                        OldSpell spell = new OldSpell(DnDManager.spells[s]);
                        spell.abilityModifier = Stat.Wisdom;
                        list.Add(spell);
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Spell {0} not found for cleric cantrip list", s));
                    }
                }

                return list;
            }

            public override int GetNumberOfChoices()
            {
                if (creature.classes[playerClass] < 4)
                {
                    return 3;
                }
                else if (creature.classes[playerClass] < 10)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }
        }

        public class SpellcastingClericCreator : SpellcastingCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SpellcastingCleric();
            }
        }

        public class SpellcastingCleric : SpellCasting
        {
            public override bool HasChoices()
            {
                return false;
            }

            public SpellcastingCleric() : base()
            {
                spellcastingFocus = typeof(HolySymbol);
                playerClass = DnDManager.classes[Classes.Cleric.ToString()];
                spells = GetClericSpellList();
            }

            public List<OldSpell> GetClericSpellList()
            {
                List<OldSpell> list = new List<OldSpell>();
                for (int i = 1; i<=9; i++ )
                {
                    foreach (string s in DnDManager.spellLists[Classes.Cleric.ToString()][i])
                    {
                        if (DnDManager.spells.ContainsKey(s))
                        {
                            OldSpell spell = new OldSpell(DnDManager.spells[s]);
                            spell.abilityModifier = Stat.Wisdom;
                            list.Add(spell);
                        }
                        else
                        {
                            Debug.WriteLine(string.Format("Spell {0} not found for cleric spell list", s));
                        }
                    }
                }

                return list;
            }
        }

        public class ChannelDivinityCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new ChannelDivinity();
            }
        }

        public class ChannelDivinity : OldFeat
        {
            public ChannelDivinity() : base()
            {
                name = "Channel Divinity";
                perShortRest = true;
                statOverride = true;
                channelDivinities = new List<OldSpell>() { new TurnUndead() };
                channelDivinities[0].linkedFeat = this;
            }

            public override void StatOverride(Creature cr)
            {             
                GetChannelDivinities();
                base.StatOverride(cr);
            }

            public override int GetMaxUses()
            {
                if(creature.classes[DnDManager.classes[Classes.Cleric.ToString()]] >= 6)
                {
                    return 2;
                }
                else if (creature.classes[DnDManager.classes[Classes.Cleric.ToString()]] >= 18)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }

            public void GetChannelDivinities()
            {
                if (channelDivinities.Count < 1)
                {
                    channelDivinities.Add(new TurnUndead());
                }
                if (creature.subclasses.Contains(DnDManager.subClasses[SubClasses.LifeDomain.ToString()]))
                {
                    if (channelDivinities.Count < 1)
                    {
                        channelDivinities.Add(new PreserveLife());
                    }
                }
            }

            List<OldSpell> channelDivinities;

            public override void UseFeatChildMenu(List<string> tags)
            {
                int index = int.Parse(tags[0]);
                channelDivinities[index].CastSpell(creature);
                base.UseFeatChildMenu(tags);

            }

            public override ContextMenuTemplate ChildMenu()
            {
                ContextMenuTemplate template = BaseChildMenuTemplate();
                for (int i = 0; i < channelDivinities.Count; i++)
                {
                    template.texts.Add(channelDivinities[i].name);
                    template.tags.Add(MakeTags(new List<string>() { i.ToString() }));
                }
                return template;
            }
        }

        public class DiscipleOfLifeCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DiscipleOfLife();
            }
        }

        public class DiscipleOfLife : OldFeat
        {
            public DiscipleOfLife() : base()
            {
                name = "Disciple of Life";
                postRoll = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is HealingRoll)
                {
                    if (roll.source is OldSpell && (roll.source as OldSpell).spellLevel >= 1)
                    {
                        (roll as HealingRoll).target.Heal(2 + (roll.source as OldSpell).spellLevel);
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class BlessedHealerCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new BlessedHealer();
            }
        }

        public class BlessedHealer : OldFeat
        {
            public BlessedHealer() : base()
            {
                name = "Blessed Healing";
                postRoll = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is HealingRoll)
                {
                    if (roll.source is OldSpell && (roll.source as OldSpell).spellLevel >= 1)
                    {
                        creature.Heal(2 + (roll.source as OldSpell).spellLevel);
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class DivineStrikeCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DivineStrike();
            }
        }

        public class DivineStrike : OldFeat
        {
            public int die = 8;
            public int NumberOfDie { get { if (creature.level < 14) { return 1; } else { return 2; } } }

            public DivineStrike() : base()
            {
                name = "Divine Strike";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is DamageRoll)
                {
                    DamageRoll rollDam = roll as DamageRoll;
                    if (rollDam.attack != null && !rollDam.attack.isSpell)
                    {
                        rollDam.diceFaces.Add(die);
                        rollDam.numberOfDice.Add(NumberOfDie);
                        rollDam.damageTypes.Add(DamageType.Radiant.ToString());
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        public class SupremeHealingCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SupremeHealing();
            }
        }

        public class SupremeHealing : OldFeat
        {
            public SupremeHealing() : base()
            {
                name = "Supreme Healing";
                postRoll = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is HealingRoll)
                {
                    HealingRoll rollHeal = roll as HealingRoll;
                    rollHeal.score = (rollHeal.die * rollHeal.numberOfDice) + rollHeal.bonus;
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class SpellcastingLifeDomainCreator : SpellcastingCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SpellcastingLifeDomain();
            }
        }

        public class SpellcastingLifeDomain : SpellCasting
        {
            public override bool HasChoices()
            {
                return false;
            }

            public SpellcastingLifeDomain() : base()
            {
                spellcastingFocus = typeof(HolySymbol);
                playerClass = DnDManager.classes[Classes.Cleric.ToString()];
                spells = GetLifeDomainSpellList();
            }

            public List<OldSpell> GetLifeDomainSpellList()
            {
                List<OldSpell> list = new List<OldSpell>();
                list.AddRange(new List<OldSpell>() { });

                foreach (OldSpell spell in list)
                {
                    spell.abilityModifier = Stat.Wisdom;
                }

                return list;
            }
        }

        #endregion

        #region Druid Feats

        public class CantripFeatDruidCreator : CantripsFeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new CantripFeatDruid();
            }
        }

        public class CantripFeatDruid : CantripsFeat
        {
            public CantripFeatDruid() : base()
            {
                cantripChoices = GetDruidCantripChoices();
                playerClass = DnDManager.classes[Classes.Druid.ToString()];
            }

            public List<OldSpell> GetDruidCantripChoices()
            {
                List<OldSpell> list = new List<OldSpell>();
                foreach (string s in DnDManager.spellLists[Classes.Druid.ToString()][0])
                {
                    if (DnDManager.spells.ContainsKey(s))
                    {
                        OldSpell spell = new OldSpell(DnDManager.spells[s]);
                        spell.abilityModifier = Stat.Wisdom;
                        list.Add(spell);
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Spell {0} not found for druid cantrip list", s));
                    }
                }

                return list;
            }

            public override int GetNumberOfChoices()
            {
                if (creature.classes[playerClass] < 4)
                {
                    if (creature.subclasses.Contains(DnDManager.subClasses[Classes.Druid.ToString()]))
                    {
                        return 3;
                    }
                    return 2;
                }
                else if (creature.classes[playerClass] < 10)
                {
                    if (creature.subclasses.Contains(DnDManager.subClasses[Classes.Druid.ToString()]))
                    {
                        return 4;
                    }
                    return 3;
                }
                else
                {
                    if (creature.subclasses.Contains(DnDManager.subClasses[Classes.Druid.ToString()]))
                    {
                        return 5;
                    }
                    return 4;
                }
            }
        }

        public class SpellcastingDruidCreator : SpellcastingCreator
        {
            public override OldFeat CreateFeat()
            {
                return new SpellcastingDruid();
            }
        }

        public class SpellcastingDruid : SpellCasting
        {
            public override bool HasChoices()
            {
                return false;
            }

            public SpellcastingDruid() : base()
            {
                spellcastingFocus = typeof(DruidicFocus);
                playerClass = DnDManager.classes[Classes.Druid.ToString()];
                spells = GetDruidSpellList();
            }

            public List<OldSpell> GetDruidSpellList()
            {
                List<OldSpell> list = new List<OldSpell>();
                for (int i = 1; i <= 9; i++)
                {
                    foreach (string s in DnDManager.spellLists[Classes.Druid.ToString()][i])
                    {
                        if (DnDManager.spells.ContainsKey(s))
                        {
                            OldSpell spell = new OldSpell(DnDManager.spells[s]);
                            spell.abilityModifier = Stat.Wisdom;
                            list.Add(spell);
                        }
                        else
                        {
                            Debug.WriteLine(string.Format("Spell {0} not found for druid spell list", s));
                        }
                    }
                }

                return list;
            }
        }

        public class WildShapedCreator : SpellcastingCreator
        {
            public override OldFeat CreateFeat()
            {
                return new WildShape();
            }
        }

        public class WildShape : OldFeat
        {
            Dictionary<int, float> crLimit = new Dictionary<int, float>() { { 2, 1 / 4 }, { 4, 1 / 2 }, { 8, 1 } };
            Dictionary<int, bool> flying = new Dictionary<int, bool>() { { 2, false }, { 4, false }, { 8, true } };
            Dictionary<int, bool> swimming = new Dictionary<int, bool>() { { 2, false }, { 4, true }, { 8, true } };

            List<float> challengeRatings = new List<float>();
            Dictionary<float, List<Monster>> monsters = new Dictionary<float, List<Monster>>();

            int BeastShapeDuration { get { return GetDuration(); } }

            WildShapeCondition currentCondition = null;

            public WildShape() : base()
            {
                name = "Wild Shape";
                perShortRest = true;
                action = true;
                GetBeasts();
            }

            public override bool IsUsable()
            {
                if (creature.CheckFeat(typeof(WildShapeCondition)))
                {
                    return false;
                }
                return base.IsUsable();
            }

            public override ContextMenuTemplate ChildMenu()
            {
                ContextMenuTemplate template = BaseChildMenuTemplate();
                template.childMenus = new List<ContextMenuTemplate>();

                float cr = 0;
                bool canFly = false;
                bool canSwim = false;

                foreach (int i in crLimit.Keys)
                {
                    if (i > creature.ClassLevel(Classes.Druid.ToString()))
                    {
                        break;
                    }
                    else
                    {
                        if (crLimit[i] > cr)
                        {
                            cr = crLimit[i];
                        }
                    }
                }
                foreach (int i in flying.Keys)
                {
                    if (canFly || i > creature.ClassLevel(Classes.Druid.ToString()))
                    {
                        break;
                    }
                    else
                    {
                        if (flying[i])
                        {
                            canFly = true;
                        }
                    }
                }
                foreach (int i in swimming.Keys)
                {
                    if (canSwim || i > creature.ClassLevel(Classes.Druid.ToString()))
                    {
                        break;
                    }
                    else
                    {
                        if (swimming[i])
                        {
                            canSwim = true;
                        }
                    }
                }

                for (int i = 0; i < challengeRatings.Count; i++)
                {
                    float challengeRate = challengeRatings[i];

                        if (challengeRate >= 1)
                        {
                            template.texts.Add(challengeRate.ToString());
                        }
                        else
                        {
                            template.texts.Add(string.Format("1/{0}", (int)1 / challengeRate));
                        }
                        template.tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                        template.childMenus.Add(BaseChildMenuTemplate());

                    foreach (Monster monster in monsters[challengeRate])
                    {
                        if (monster.creatureType != DnDManager.CreatureType.Beast.ToString())
                        {
                            template.childMenus[i].texts.Add(monster.name);
                            template.childMenus[i].tags.Add(MakeTags(new List<string>() { i.ToString(), monsters[challengeRate].IndexOf(monster).ToString() }));
                        }
                        else
                        {
                            if (!(monster.cr > cr || (monster.swimSpeed > 0 && !canSwim) || (monster.flySpeed > 0 && !canFly)))
                            {
                                template.childMenus[i].texts.Add(monster.name);
                                template.childMenus[i].tags.Add(MakeTags(new List<string>() { i.ToString(), monsters[challengeRate].IndexOf(monster).ToString() }));
                            }
                        }
                    }

                }
                

                return template;
            }

            public void GetBeasts()
            {
                foreach(string mc in DnDManager.monsters.Keys)
                {
                    Monster creature = DnDManager.monsters[mc].ToMonster();
                    if (creature.creatureType == DnDManager.CreatureType.Beast.ToString())
                    {
                        if (!monsters.Keys.Contains(creature.cr))
                        {
                            monsters.Add(creature.cr, new List<Monster>());
                        }
                        monsters[creature.cr].Add(creature);
                    }
                }
            }

            public override void UseFeatChildMenu(List<string> tags)
            {
                Transform(monsters[float.Parse(tags[0])][int.Parse(tags[1])]);
                base.UseFeatChildMenu(tags);
            }

            public void Transform(Monster monster)
            {
                currentCondition = creature.AddFeat(new WildShapeCondition()) as WildShapeCondition;
                currentCondition.baseMonster = monster;
                currentCondition.duration = GetDuration();
            }

            public int GetDuration()
            {
                return creature.classes[DnDManager.classes[Classes.Druid.ToString()]] / 2;
            }

            public override int GetMaxUses()
            {
                if (creature.ClassLevel(Classes.Druid.ToString()) >= 20)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }

        }

        public class WildShapeCondition : OldCondition
        {
            public Monster baseMonster = null;

            public WildShapeCondition() : base()
            {
                name = "Wild Shaped";
                bonusAction = true;
                takeDamage = true;
                statOverride = true;
            }

            public override void FeatActivated(Creature creature, Creature.CreatureDelegateEventArgs e)
            {
                if (creature.hitPoints <= 0)
                {
                    RemoveFromCreature();
                }
                base.FeatActivated(creature, e);
            }
        }

        public class NaturalRecoveryCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new NaturalRecovery();
            }
        }

        public class NaturalRecovery : OldFeat
        {
            
        }

        public class NaturalRecoveryOptions : DisplayOptions
        {
            public NaturalRecoveryOptions() : base("Recover which spell slots?",OldFeat.optionsDisplayTextures)
            {

            }
        }

        public class DruidCircleSpellsCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DruidCircleSpells();
            }
        }

        public class DruidCircleSpells : SpellCasting
        {
            public enum DruidCircles { None, Arctic, Coast, Desert, Forest, Grassland, Mountain, Swamp };
            public Dictionary<DruidCircles, List<OldSpell>> circleSpells = new Dictionary<DruidCircles, List<OldSpell>>()
            {

            };
            public DruidCircles choice = DruidCircles.None;

            public override bool HasChoices()
            {
                if (choice != DruidCircles.None)
                {
                    return false;
                }
                else
                {
                    featChoices = new List<string>(Enum.GetNames(typeof(DruidCircles)));
                    return true;
                }
            }

            public DruidCircleSpells() : base()
            {
                spellcastingFocus = typeof(DruidicFocus);
                playerClass = DnDManager.classes[Classes.Druid.ToString()];
            }

            public override void FeatChoice(string choice)
            {
                this.choice = (DruidCircles)Enum.Parse(typeof(DruidCircles), choice);
                spells = circleSpells[this.choice];
            }

        }

        public class NaturesWardCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new NaturesWard();
            }
        }

        public class NaturesWard : OldFeat
        {
            public NaturesWard() : base()
            {
                name = "Nature's Ward";
                preRoll = true;
                statChange = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.rollTags.Contains(DamageType.Poison.ToString()) || roll.rollTags.Contains(MiscRollTags.Disease.ToString()))
                {
                    roll.autoSuccess = true;
                }
                if ((roll.rollTags.Contains(Conditions.Charmed.ToString()) || roll.rollTags.Contains(Conditions.Frightened.ToString())) && 
                   (roll.GetSourceCreature() != null && (roll.GetSourceCreature().creatureType == DnDManager.CreatureType.Elemental.ToString() || roll.GetSourceCreature().creatureType == DnDManager.CreatureType.Fey.ToString())))
                {
                    roll.autoSuccess = true;
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        public class NaturesSanctuaryCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new NaturesSanctuary();
            }
        }

        public class NaturesSanctuary : OldFeat
        {
            List<Creature> immuneCreatures = new List<Creature>();

            public Dictionary<SavingThrow, AttackRoll> ongoingChecks = new Dictionary<SavingThrow, AttackRoll>();

            public NaturesSanctuary() : base()
            {
                name = "Nature's Sanctuary";
                preRoll = true;
                statChange = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll is AttackRoll)
                {
                    AttackRoll rollAtt = roll as AttackRoll;
                    if (rollAtt.attack.defender == creature && (rollAtt.attack.attacker.creatureType == DnDManager.CreatureType.Beast.ToString() || rollAtt.attack.attacker.creatureType == DnDManager.CreatureType.Plant.ToString()))
                    {
                        SavingThrow save = new SavingThrow(Stat.Wisdom, rollAtt.attack.attacker, this);
                        save.DC = 8 + creature.StatMod(Stat.Wisdom) + creature.proficiencyBonus;
                        save.finishRoll += SaveFinished;
                        ongoingChecks.Add(save, rollAtt);
                        save.DoRoll();
                    }
                }
            }

            public void SaveFinished(Roll roll, RollEventArgs e)
            {
                base.FeatPreRollCheck(ongoingChecks[roll as SavingThrow], e);
            }
        }

        #endregion

        #region Fighter Feats

        public class FighterFightingStyleCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FighterFightingStyle();
            }
        }

        public class FighterFightingStyle : OldFeat
        {
            enum FightingStyleType { None, Archery, Defense, Dueling, GreatWeaponFighting, Protection, TwoWeaponFighting };

            Dictionary<FightingStyleType, FeatCreator> fightingStyles = new Dictionary<FightingStyleType, FeatCreator>()
            {
                {FightingStyleType.Archery, new FightingStyleArcheryCreator() },
                {FightingStyleType.Defense, new FightingStyleDefenseCreator() },
                {FightingStyleType.Dueling, new FightingStyleDuelingCreator() },
                {FightingStyleType.GreatWeaponFighting, new FightingStyleGreatWeaponFightingCreator() },
                {FightingStyleType.Protection, new FightingStyleProtectionCreator() },
                {FightingStyleType.TwoWeaponFighting, new FightingStyleTwoWeaponFightingCreator() }
            };

            public FighterFightingStyle() : base()
            {
                name = "Fighting Style";
                featChoices = new List<string>(Enum.GetNames(typeof(FightingStyleType)));
                choiceMade = false;
            }

            public override bool HasChoices()
            {
                List<string> featChoicesOld = new List<string>(featChoices);
                foreach (string style in featChoicesOld)
                {
                    if (creature.CheckFeat(fightingStyles[(FightingStyleType)(Enum.Parse(typeof(FightingStyleType), style))].TypeFeat()))
                    {
                        featChoices.Remove(style);
                    }
                }
                return base.HasChoices();
            }

            public override void FeatChoice(string choice)
            {
                base.FeatChoice(choice);
                creature.AddFeat(fightingStyles[(FightingStyleType)(Enum.Parse(typeof(FightingStyleType), choice))].CreateFeat());
                RemoveFromCreature();
            }
        }

        public class FightingStyleArcheryCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FightingStyleArchery();
            }
        }

        public class FightingStyleArchery : OldFeat
        {
            public FightingStyleArchery() : base()
            {
                name = "Archery Fighting Style";
                postRoll = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is DamageRoll)
                {
                    roll.Bonus += 2;
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class FightingStyleDefenseCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FightingStyleDefense();
            }
        }

        public class FightingStyleDefense : OldFeat
        {
            public FightingStyleDefense() : base()
            {
                name = "Defense Fighting Style";
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                if (creature.armor.armorCategory != ArmorCategories.None)
                {
                    creature.acBonus++;
                }
                base.StatChange(cr);
            }
        }

        public class FightingStyleDuelingCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FightingStyleDueling();
            }
        }

        public class FightingStyleDueling : OldFeat
        {
            public FightingStyleDueling() : base()
            {
                name = "Dueling Fighting Style";
                postRoll = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is DamageRoll)
                {
                    DamageRoll rollDam = roll as DamageRoll;
                    if (rollDam.attack != null && rollDam.attack.attackerWeapon == creature.weaponMainHand && (creature.weaponOffHand == null || creature.weaponOffHand is Shield))
                    {
                        if (!rollDam.attack.attackerWeapon.weaponProperties.Contains(WeaponProperty.Range))
                        {
                            roll.Bonus += 2;
                        }
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class FightingStyleGreatWeaponFightingCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FightingStyleGreatWeaponFighting();
            }
        }

        public class FightingStyleGreatWeaponFighting : OldFeat
        {
            public FightingStyleGreatWeaponFighting() : base()
            {
                name = "Great Weapon Fighting";
                postRoll = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is DamageRoll)
                {
                    DamageRoll rollDam = roll as DamageRoll;
                    if (rollDam.attack != null && rollDam.attack.attackerWeapon == creature.weaponMainHand && creature.weaponOffHand == null)
                    {
                        if (rollDam.attack.attackerWeapon.weaponProperties.Contains(WeaponProperty.TwoHanded) || rollDam.attack.attackerWeapon.weaponProperties.Contains(WeaponProperty.Versatile))
                        {
                            if (!roll.rerolled)
                            {
                                rollDam.Reroll(2);
                            }
                        }
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class FightingStyleProtectionCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FightingStyleProtection();
            }
        }

        public class FightingStyleProtection : OldFeat
        {
            public FightingStyleProtection() : base()
            {
                name = "Protection Fighting Style";
                preRoll = true;
                description = "Use reaction to impose disadvantage?";
                reaction = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll is AttackRoll && (creature.weaponMainHand is Shield || creature.weaponOffHand is Shield))
                {
                    if (creature.boardTile.board.GetDistance(creature.boardTile, (roll as AttackRoll).attack.defender.boardTile) <= 5)
                    {
                        if (!creature.reactionTaken)
                        {
                            MakeOptionsDisplay(roll);
                        }
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }

            public override void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                (dO as DisplayOptionsDnD).roll.disadvantage = true;
                creature.reactionTaken = true;
                base.OptionsDisplayYes(dO, e);
                FeatPreRollFinished((dO as DisplayOptionsDnD).roll);
            }

            public override void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayNo(dO, e);
                FeatPreRollFinished((dO as DisplayOptionsDnD).roll);
            }
        }

        public class FightingStyleTwoWeaponFightingCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new FightingStyleTwoWeaponFighting();
            }
        }

        public class FightingStyleTwoWeaponFighting : OldFeat
        {
            public FightingStyleTwoWeaponFighting() : base()
            {
                name = "Two-Weapon Fighting";
                postRoll = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is DamageRoll)
                {
                    DamageRoll damRoll = roll as DamageRoll;
                    if (damRoll.attack != null && damRoll.attack.offHand)
                    {
                        roll.Bonus += damRoll.attack.attacker.StatMod(damRoll.attack.statUsed);
                    }
                }
                base.FeatPostRollCheck(roll, e);
            }
        }

        public class SecondWindCreator : FeatCreator
        {
            public SecondWindCreator() : base() { }

            public override OldFeat CreateFeat()
            {
                return new SecondWind();
            }
        }

        public class SecondWind : OldFeat
        {
            public SecondWind()
            {
                name = "Second Wind";
                description = "Vou have a limited well of stamina that you can draw on to protect yourself from harm. " +
                              "On your turn, you can use a bonus action to regain hit points equal to ld10 + your fighter leveI. " +
                              "Once you use this feature, you must finish a short or long rest before you can use it again.";
                bonusAction = true;
                perShortRest = true;
            }

            public override void UseFeat()
            {
                creature.Heal(EngManager.random.Next(1, 11) + creature.level);
            }
        }

        public class ActionSurgeCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new ActionSurge();
            }
        }

        public class ActionSurge : OldFeat
        {
            bool usedThisTurn = false;

            public ActionSurge() : base()
            {
                name = "Acton Surge";
                perShortRest = true;
                endTurn = true;
            }

            public override bool IsUsable()
            {
                if (creature.actionTaken && uses < MaxUses && !usedThisTurn)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override void UseFeat()
            {
                base.UseFeat();
                usedThisTurn = true;
                creature.actionTaken = false;
            }

            public override void EndTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                base.EndTurn(cr, e);
                if (usedThisTurn)
                {
                    usedThisTurn = false;
                }
            }

            public override int GetMaxUses()
            {
                if (creature.ClassLevel(Classes.Fighter.ToString()) < 17)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }

        public class ExtraAttackFighterCreator : ExtraAttackCreator
        {
            public override OldFeat CreateFeat()
            {
                return new ExtraAttackFighter();
            }
        }

        public class ExtraAttackFighter : ExtraAttack
        {
            public override void StatChange(Creature cr)
            {
                if (creature.ClassLevel(Classes.Fighter.ToString()) > 5 && creature.attacks < 2)
                {
                    creature.attacks = 2;
                }
                if (creature.ClassLevel(Classes.Fighter.ToString()) > 11 && creature.attacks < 3)
                {
                    creature.attacks = 3;
                }
                if (creature.ClassLevel(Classes.Fighter.ToString()) > 20 && creature.attacks < 4)
                {
                    creature.attacks = 4;
                }
            }
        }

        public class IndomitableCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Indomitable();
            }
        }

        public class Indomitable : OldFeat
        {
            public Indomitable() : base()
            {
                name = "Indomitable";
                postRoll = true;
                optionsDisplayDescription = string.Format("Reroll saving throw? (Uses left: {0}", MaxUses - uses);
                perLongRest = true;
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is SavingThrow && !roll.Success && uses < MaxUses)
                {
                    MakeOptionsDisplay(roll);
                }
            }

            public override void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayYes(dO, e);
                uses++;
                DisplayOptionsDnD(dO).roll.reroll = true;
                FeatPostRollFinished(DisplayOptionsDnD(dO).roll);
            }

            public override void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                base.OptionsDisplayNo(dO, e);
                FeatPostRollFinished((dO as DisplayOptionsDnD).roll);
            }

            public override int GetMaxUses()
            {
                if (creature.ClassLevel(Classes.Fighter.ToString()) < 13)
                {
                    return 1;
                }
                else if (creature.ClassLevel(Classes.Fighter.ToString()) < 17)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
        }

        public class RemarkableAthleteCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new RemarkableAthlete();
            }
        }

        public class RemarkableAthlete : OldFeat
        {
            public RemarkableAthlete() : base()
            {
                name = "Remarkable Athlete";
                preRoll = true;
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                //Can jump an extra distance equal to your strenght modifier
                base.StatChange(cr);
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is AbilityCheck)
                {
                    AbilityCheck rollAb = roll as AbilityCheck;
                    if (!rollAb.proficiencyUsed)
                    { 
                        if (rollAb.stat == Stat.Strength || rollAb.stat == Stat.Dexterity|| rollAb.stat == Stat.Constitution)
                        {

                        }
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }
        }

        public class ImprovedCriticalCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new ImprovedCritical();
            }
        }

        public class ImprovedCritical : OldFeat
        {
            int CriticalRoll { get { return GetCriticalRoll(); } }

            public ImprovedCritical() : base()
            {
                name = "Improved Critical";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is DamageRoll)
                {
                    DamageRoll damRoll = roll as DamageRoll;
                    if (damRoll.attack != null && damRoll.attack.attackRoll.Natural >= CriticalRoll)
                    {
                        damRoll.critical = true;
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }

            public int GetCriticalRoll()
            {
                if (creature.ClassLevel(Classes.Fighter.ToString()) < 15)
                {
                    name = "Improved Critical";
                    return 19;
                }
                else
                {
                    name = "Superior Critical";
                    return 18;
                }
            }


        }

        public class SurvivorCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Survivor();
            }
        }

        public class Survivor : OldFeat
        {
            public Survivor() : base()
            {
                name = "Survivor";
                startTurn = true;
            }

            public override void StartTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                base.StartTurn(cr, e);
                if (creature.hitPoints > 0 && creature.hitPoints <= creature.hitPointMax/2)
                {
                    creature.Heal(5 + creature.StatMod(Stat.Constitution));
                }
            }
        }

        #endregion

        #region Monk Feats

        public class UnarmoredDefenseMonkCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new UnarmoredDefenseMonk();
            }
        }

        public class UnarmoredDefenseMonk : OldFeat
        {
            public UnarmoredDefenseMonk() : base()
            {
                name = "Unarmored Defense";
                statChange = true;
            }

            public override void StatChange(Creature cr)
            {
                base.StatChange(cr);
                if (creature.armor.baseArmorType == ArmorTypes.NoArmor)
                {
                    creature.armor = new MonkUnarmoredDefenseArmor();
                }
            }
        }

        public class MonkUnarmoredDefenseArmorCreator : ArmorCreator
        {
            public override Item CreateItem()
            {
                return new MonkUnarmoredDefenseArmor();
            }
        }

        public class MonkUnarmoredDefenseArmor : Armor
        {
            public MonkUnarmoredDefenseArmor() : base()
            {
                name = "Unarmored Defense";
                armorCategory = ArmorCategories.None;
                baseArmorType = ArmorTypes.UnarmoredDefense;
                baseAC = 10;
                cost = 0;
                weight = 0;
            }

            public override int GetAC(Creature creature)
            {
                return baseAC + creature.StatMod(Stat.Dexterity) + creature.StatMod(Stat.Wisdom);
            }
        }

        public class MartialArtsCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new MartialArts();
            }
        }

        public class MartialArts : OldFeat
        {
            public int MartialArtsDie { get { return GetMartialArtsDie(); } }

            public int GetMartialArtsDie()
            {
                if (creature.ClassLevel(Classes.Monk.ToString()) < 5)
                {
                    return 4;
                }
                else if (creature.ClassLevel(Classes.Monk.ToString()) < 11)
                {
                    return 6;
                }
                else if (creature.ClassLevel(Classes.Monk.ToString()) < 17)
                {
                    return 8;
                }
                else
                {
                    return 10;
                }
            }

            public bool IsMonkWeapon(Weapon weapon)
            {
                if (weapon is Shortsword)
                {
                    return true;
                }
                else if (weapon.weaponCategory == WeaponCategory.SimpleWeapon && !(weapon.weaponProperties.Contains(WeaponProperty.Range) || weapon.weaponProperties.Contains(WeaponProperty.TwoHanded) || weapon.weaponProperties.Contains(WeaponProperty.Heavy)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public MartialArts() : base()
            {
                name = "Martial Arts";
                preRoll = true;
            }

            public override void FeatPreRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll.roller == creature && roll is DamageRoll)
                {
                    DamageRoll damRoll = roll as DamageRoll;
                    if (damRoll.IsAttack && IsMonkWeapon(damRoll.attack.attackerWeapon))
                    {
                        if (damRoll.diceFaces[0] < MartialArtsDie)
                        {
                            damRoll.diceFaces[0] = MartialArtsDie;
                        }
                    }
                }
                else if (roll.roller == creature && roll is AttackRoll)
                {
                    AttackRoll attRoll = roll as AttackRoll;
                    if (attRoll.attack.statUsed == Stat.Strength && creature.StatMod(Stat.Dexterity) > creature.StatMod(Stat.Strength))
                    {
                        attRoll.attack.statUsed = Stat.Dexterity;
                        attRoll.Bonus += creature.StatMod(Stat.Dexterity) - creature.StatMod(Stat.Strength);
                    }
                }
                base.FeatPreRollCheck(roll, e);
            }

            public override bool IsUsable()
            {
                if (creature.attacked && !creature.bonusActionTaken)
                {
                    if (IsMonkWeapon(creature.attacksThisTurn[0].attackerWeapon))
                    {
                        return true;
                    }
                }
                return base.IsUsable();
            }

            public override void UseFeat()
            {
                new AttackOrder(creature, new UnarmedStrike(), false, true, false);
                base.UseFeat();
            }
        }

        public class KiCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Ki();
            }
        }
        
        public class Ki : OldFeat
        {
            public List<KiFeat> kiFeats = new List<KiFeat>() { new FlurryOfBlows(), new PatientDefense(), new StepOfTheWindDisengage(), new StepOfTheWindDash() };

            bool emptyBody = false;

            public Ki() : base()
            {
                name = "Ki";
                perShortRest = true;
                foreach(KiFeat kiF in kiFeats)
                {
                    kiF.kiFeat = this;
                }
            }

            public override int GetMaxUses()
            {
                return creature.ClassLevel(Classes.Monk.ToString());
            }

            public override ContextMenuTemplate ChildMenu()
            {
                ContextMenuTemplate template = BaseChildMenuTemplate();
                foreach (KiFeat kiFeat in kiFeats)
                {
                    if (kiFeat.IsKiUsable())
                    {
                        template.texts.Add(kiFeat.name);
                        template.tags.Add(MakeTags(new List<string>() { kiFeats.IndexOf(kiFeat).ToString() }));
                    }
                }

                return template;
            }

            public override void UseFeatChildMenu(List<string> tags)
            {
                base.UseFeatChildMenu(tags);
                kiFeats[int.Parse(tags[0])].UseFeat();
            }
        }



        public abstract class KiFeat : OldFeat
        {
            public Ki kiFeat;
            public int kiCost;

            public override void AddFeat()
            {
                base.AddFeat();
                kiFeat = creature.GetFeat(typeof(Ki)) as Ki;
                kiFeat.kiFeats.Add(this);
            }

            public override bool IsUsable()
            {
                return false;
            }

            public virtual bool IsKiUsable()
            {
                return base.IsUsable();
            }

            public override void UseFeat()
            {
                kiFeat.uses += kiCost;
                base.UseFeat();
            }
        }

        public class FlurryOfBlows : KiFeat
        {
            bool usedThisTurn = false;
            int attacks = 0;

            public FlurryOfBlows() : base()
            {
                name = "Flurry of Blows";
                action = true;
                kiCost = 1;
                endTurn = true;
            }

            public override void EndTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
            {
                base.EndTurn(cr, e);
                attacks = 0;
                usedThisTurn = false;
            }

            public override bool IsKiUsable()
            {
                if (usedThisTurn && attacks < 2)
                {
                    return true;
                }
                else if (!usedThisTurn && creature.attacked && !creature.bonusActionTaken)
                {
                    return true;
                }
                return base.IsKiUsable();
            }

            public override void UseFeat()
            {
                if (!usedThisTurn)
                {
                    base.UseFeat();
                    usedThisTurn = true;
                    attacks++;
                    new AttackOrder(creature, new Weapon(), false, false, false);
                }
                else
                {
                    attacks++;
                    new AttackOrder(creature, new Weapon(), false, false, false);
                }                   
            }
        }

        public class PatientDefense : KiFeat
        {
            public override bool IsKiUsable()
            {
                if (!creature.bonusActionTaken && !creature.CheckCondition(Conditions.Dodging.ToString()))
                {
                    return true;
                }
                return base.IsKiUsable();
            }

            public override void UseFeat()
            {
                creature.AddCondition(Conditions.Dodging.ToString());
                base.UseFeat();
            }
        }

        public class StepOfTheWindDash : KiFeat
        {
            public override bool IsKiUsable()
            {
                if (!creature.bonusActionTaken)
                {
                    return true;
                }
                return base.IsKiUsable();
            }

            public override void UseFeat()
            {
                creature.AddCondition(Conditions.Dashing.ToString());
                base.UseFeat();
            }
        }

        public class StepOfTheWindDisengage : KiFeat
        {
            public override bool IsKiUsable()
            {
                if (!creature.bonusActionTaken && creature.CheckCondition(Conditions.Disengaged.ToString()))
                {
                    return true;
                }
                return base.IsKiUsable();
            }

            public override void UseFeat()
            {
                creature.AddCondition(Conditions.Disengaged.ToString());
                base.UseFeat();
            }
        }

        public class UnarmoredMovement : OldFeat
        {
            public int MovementBonus { get { return GetMovementBonus(); } }

            public int GetMovementBonus()
            {
                if (creature.ClassLevel(Classes.Monk.ToString()) < 6)
                {
                    return 10;
                }
                else if (creature.ClassLevel(Classes.Monk.ToString()) < 10)
                {
                    return 15;
                }
                else if (creature.ClassLevel(Classes.Monk.ToString()) < 14)
                {
                    return 20;
                }
                else if (creature.ClassLevel(Classes.Monk.ToString()) < 18)
                {
                    return 25;
                }
                else
                {
                    return 30;
                }
            }

            public UnarmoredMovement() : base()
            {
                name = "Unarmored Movement";
                statChange = true;
            }

            // public override void StatChange(Creature cr)
            // {
            //     base.StatChange(cr);
            //     if (creature.armor.armorCategory == ArmorCategories.None && !(creature.weaponMainHand is Shield) && !(creature.weaponOffHand is Shield))
            //     {
            //         creature.speed += MovementBonus;
            //     }
            // }
        }

        public class DeflectMissilesCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DeflectMissiles();
            }
        }

        public class DeflectMissiles : KiFeat
        {
            public DeflectMissiles() : base()
            {
                name = "Deflect Missiles";
                postRoll = true;
                description = "Use reaction to try and catch missile?";
            }

            public override void FeatPostRollCheck(Roll roll, RollEventArgs e)
            {
                if (roll is DamageRoll)
                {
                    DamageRoll rollDam = roll as DamageRoll;
                    if (rollDam.IsAttack && rollDam.attack.defender == creature)
                    {
                        if (rollDam.attack.attackerWeapon.weaponProperties.Contains(WeaponProperty.Range) || (rollDam.attack.attackerWeapon.weaponProperties.Contains(WeaponProperty.Thrown) && !rollDam.attack.IsMelee))
                        {
                            MakeOptionsDisplay(roll);
                        }

                    }
                }
            }

            public override DisplayOptionsDnD MakeOptionsDisplay(Roll roll, List<Roll> rolls = null, Creature creature = null)
            {
                if (creature.reactionTaken)
                {
                    description = "Spend ki to make attack?";
                }
                else
                {
                    description = "Use reaction to try and catch missile?";
                }
                return base.MakeOptionsDisplay(roll, rolls, creature);
            }

            public override void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                if (creature.reactionTaken)
                {

                }
                else
                {
                    base.OptionsDisplayNo(dO, e);
                    FeatPostRollFinished(DisplayOptionsDnD(dO).roll);
                }
            }

            public override void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
            {
                if (creature.reactionTaken)
                {
                    kiFeat.uses++;
                    new AttackOrder(creature, new ProjectileMonkWeapon(), false, false, false);
                }
                else
                {
                    base.OptionsDisplayYes(dO, e);
                    creature.reactionTaken = true;
                    DisplayOptionsDnD(dO).roll.Bonus -= EngManager.random.Next(1, 11) + creature.StatMod(Stat.Dexterity) + creature.ClassLevel(Classes.Monk.ToString());
                    if ((DisplayOptionsDnD(dO).roll as DamageRoll).TotalScore <= 0)
                    {
                        MakeOptionsDisplay(DisplayOptionsDnD(dO).roll);
                    }
                    FeatPostRollFinished(DisplayOptionsDnD(dO).roll);
                }

            }


        }

        public class ProjectileMonkWeapon : Weapon
        {
            public ProjectileMonkWeapon() : base()
            {
                name = "Projectile Monk Weapon";
                minRange = 20;
                maxRange = 60;
                weaponProperties.Add(WeaponProperty.Thrown);
            }
        }

        public class StunningStrikeCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new StunningStrike();
            }
        }

        public class StunningStrike : KiFeat
        {
            public StunningStrike() : base()
            {
                name = "Stunning strike";
            }
        }

        public class KiEmpoweredStrikesCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new KiEmpoweredStrikes();
            }
        }

        public class KiEmpoweredStrikes : OldFeat
        {
            public KiEmpoweredStrikes() : base()
            {
                name = "Ki-empowered Strikes";
            }
        }

        public class EvasionCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Evasion();
            }
        }

        public class Evasion : OldFeat
        {
            public Evasion() : base()
            {
                name = "Evasion";
            }
        }

        public class StillnessOfMindCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new StillnessOfMind();
            }
        }

        public class StillnessOfMind : OldFeat
        {
            public StillnessOfMind() : base()
            {
                name = "Stillness of Mind";
            }
        }

        public class PurityOfBodyCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new PurityOfBody();
            }
        }

        public class PurityOfBody : KiFeat
        {
            public PurityOfBody() : base()
            {
                name = "Purity of Body";
            }
        }

        public class TongueoftheSunandMoonCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new TongueoftheSunandMoon();
            }
        }

        public class TongueoftheSunandMoon : OldFeat
        {
            public TongueoftheSunandMoon() : base()
            {
                name = "Tongue of the Sun and Moon";
            }
        }

        public class DiamondSoulCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new DiamondSoul();
            }
        }

        public class DiamondSoul : KiFeat
        {
            public DiamondSoul() : base()
            {
                name = "Diamond Soul";
            }
        }

        public class EmptyBodyCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new EmptyBody();
            }
        }

        public class EmptyBody : KiFeat
        {
            public EmptyBody() : base()
            {
                name = "Empty Body";
            }
        }

        public class PerfectSelfCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new PerfectSelf();
            }
        }

        public class PerfectSelf : KiFeat
        {
            public PerfectSelf() : base()
            {
                name = "Perfect Self";
            }
        }

        public class OpenHandTechniqueCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new StunningStrike();
            }
        }

        public class OpenHandTechnique : KiFeat
        {
            public OpenHandTechnique() : base()
            {
                name = "Open Hand Technique";
            }
        }

        public class WholenessOfBodyCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new WholenessOfBody();
            }
        }

        public class WholenessOfBody : OldFeat
        {
            public WholenessOfBody() : base()
            {
                name = "Wholeness of Body";
            }
        }

        public class TranquilityCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Tranquility();
            }
        }

        public class Tranquility : OldFeat
        {
            public Tranquility() : base()
            {
                name = "Tranquility";
            }
        }

        public class QuiveringPalmCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new QuiveringPalm();
            }
        }

        public class QuiveringPalm : KiFeat
        {
            public QuiveringPalm() : base()
            {
                name = "Quivering Palm";
            }
        }

        #endregion

        #region Paladin

        public class DivineSense : OldFeat
        {

        }

        public class LayOnHands : OldFeat
        {

        }

        public class FightingStylePaladin : FighterFightingStyle
        {

        }

        public class SpellcastingPaladin : SpellCasting
        {

        }

        public class DivineSmite : OldFeat
        {

        }

        public class DivineHealth : OldFeat
        {

        }

        public class SacredOath : OldFeat
        {

        }

        public class ChannelDivinityPaladin : OldFeat
        {

        }

        public class ChannelDivinityFeat : OldFeat
        {

        }

        public class AuraOfProtection : OldFeat
        {

        }

        public class AuraOfCourage : OldFeat
        {

        }

        public class ImprovedDivineSmite : OldFeat
        {

        }

        public class CleansingTouch : OldFeat
        {

        }

        public class OathOfDevotionSpellcasting : OldFeat
        {

        }

        public class SacredWeapon : ChannelDivinityFeat
        {

        }

        public class TurnTheUnholy : ChannelDivinityFeat
        {

        }

        public class AuraOfDevotion : OldFeat
        {

        }

        public class PurityOfSpirit : OldFeat
        {

        }

        public class HolyNimbus : OldFeat
        {

        }

        #endregion

        #region Ranger

        public class FavoredEnemy : OldFeat
        {

        }

        public class NaturalExplorer : OldFeat
        {

        }

        public class FightingStyleRanger : FighterFightingStyle
        {

        }

        public class SpellcastingRanger : SpellCasting
        {

        }

        public class PrimevalAwareness : OldFeat
        {

        }

        public class LandsStride : OldFeat
        {

        }

        public class HideInPlainSight : OldFeat
        {

        }

        public class Vanish : OldFeat
        {

        }

        public class FeralSenses : OldFeat
        {

        }

        public class FoeSlayer : OldFeat
        {

        }

        public class HuntersPrey : OldFeat
        {

        }

        public class DefensiveTactics : OldFeat
        {

        }

        public class Multiattack : OldFeat
        {

        }

        public class SuperiorHuntersDefense : OldFeat
        {

        }

        #endregion

        #region Rogue

        public class RogueExpertise : ExpertiseBard
        {

        }

        public class SneakAttack : OldFeat
        {

        }

        public class CunningActionFeat : OldFeat
        {

        }

        public class UncannyDodge : OldFeat
        {

        }

        public class ReliableTalent : OldFeat
        {

        }

        public class Blindsense : OldFeat
        {

        }

        public class SlipperyMind : OldFeat
        {

        }

        public class Elusive : OldFeat
        {

        }

        public class StrokeOfLuck : OldFeat
        {

        }

        public class FastHands : OldFeat
        {

        }

        public class SecondStoryWork : OldFeat
        {

        }

        public class SupremeSneak : OldFeat
        {

        }

        public class UseMagicDevice : OldFeat
        {

        }

        public class ThiefsReflexes : OldFeat
        {

        }

        #endregion

        #region Sorceror

        public class SorcerorCantrips : CantripsFeat
        {

        }

        public class SpellcastingSorceror : SpellCasting
        {

        }

        public class SorcerousOrigin : OldFeat
        {

        }

        public class FontofMagic : OldFeat
        {

        }

        public class Metamagic : OldFeat
        {

        }
        public class SorcerousRestoration : OldFeat
        {

        }

        public class DragonAncestor : OldFeat
        {

        }

        public class DraconicResilience : OldFeat
        {

        }

        public class ElementalAffinity : OldFeat
        {

        }

        public class DragonWings : OldFeat
        {

        }

        public class DraconicPresence : OldFeat
        {

        }

        #endregion

        #region Warlock 

        public class WarlockCantrips : CantripsFeat
        {

        }

        public class WarlockSpellcasting : SpellCasting
        {

        }

        public class EldritchInvocationFeat : OldFeat
        {

        }

        public class EldritchInvocation : OldFeat
        {

        }

        public class PactBoon : OldFeat
        {

        }

        public class PactOfTheChain : OldFeat
        {

        }

        public class PactOfTheBlade : OldFeat
        {

        }

        public class PactOfTheTome : OldFeat
        {

        }

        public class MysticArcanum : OldFeat
        {

        }

        public class EldritchMaster : OldFeat
        {

        }

        public class DarkOnesBlessing : OldFeat
        {

        }

        public class DarkOnesOwnLuck : OldFeat
        {
        }

        public class FiendishResilience : OldFeat
        {

        }

        public class HurlThroughHell : OldFeat
        {

        }

        public class AgonizingBlast : EldritchInvocation
        {

        }

        public class ArmorOfShadows : EldritchInvocation
        {

        }

        public class AscendentStep : EldritchInvocation
        {

        }

        public class BeastSpeech : EldritchInvocation
        {

        }

        public class BeguilingInfluence : EldritchInvocation
        {

        }

        public class BewitchingWhispers : EldritchInvocation
        {

        }

        public class BookOfAncientSecrets : EldritchInvocation
        {

        }

        public class ChainsOfCarceri : EldritchInvocation
        {

        }

        public class DevilsSight : EldritchInvocation
        {

        }

        public class DreadfulWord : EldritchInvocation
        {

        }

        public class EldritchSight : EldritchInvocation
        {

        }

        public class EldritchSpear : EldritchInvocation
        {

        }

        public class EyesOfTheRuneKeeper : EldritchInvocation
        {

        }

        public class FiendishVigor : EldritchInvocation
        {

        }

        public class GazeOfTwoMinds : EldritchInvocation
        {

        }

        public class Lifedrinker : EldritchInvocation
        {

        }

        public class MaskOfManyFaces : EldritchInvocation
        {

        }

        public class MasterOfMyriadForms : EldritchInvocation
        {

        }

        public class MinionsOfChaos : EldritchInvocation
        {

        }

        public class MireTheMind : EldritchInvocation
        {

        }

        public class MistyVisions : EldritchInvocation
        {

        }

        public class OneWithShadows : EldritchInvocation
        {

        }

        public class OtherworldlyLeap : EldritchInvocation
        {

        }

        public class RepellingBlast : EldritchInvocation
        {

        }

        public class SculptorOfFlesh : EldritchInvocation
        {

        }

        public class SignOfIllOmen : EldritchInvocation
        {

        }

        public class ThiefOfFiveFates : EldritchInvocation
        {

        }

        public class ThirstingBlade : EldritchInvocation
        {

        }

        public class VisionsOfDistantRealms : EldritchInvocation
        {

        }

        public class VoiceOfTheChainMaster : EldritchInvocation
        {

        }

        public class WhispersOfTheGrave : EldritchInvocation
        {

        }

        public class WitchSight : EldritchInvocation
        {

        }

        #endregion

        #region Wizard

        public class WizardSpellcasting : SpellCasting
        {

        }

        public class WizardCantrips : CantripsFeat
        {

        }

        public class ArcaneRecovery : OldFeat
        {

        }

        public class SpellMastery : OldFeat
        {

        }

        public class SignatureSpells : OldFeat
        {

        }

        public class EvocationSavant : OldFeat
        {

        }

        public class SculptSpells : OldFeat
        {

        }

        public class PotentCantrip : OldFeat
        {

        }

        public class EmpoweredEvocation : OldFeat
        {

        }

        public class Overchannel : OldFeat
        {

        }

        #endregion

        public class ABIFeature : OldFeat
        {

        }

        public class Grappler : ABIFeature
        {

        }

        #endregion

        #region Monster Feats

        public class AmphibiousCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new Amphibious();
            }
        }

        public class Amphibious : OldFeat
        {

        }

        #region Cultists

        public class DarkDevotionCreator : FeatCreator
        {
            public DarkDevotionCreator() : base() { }

            public override OldFeat CreateFeat()
            {
                return new DarkDevotion();
            }
        }

        public class DarkDevotion : OldFeat
        {
            public DarkDevotion()
            {
                name = "Dark Devotion";
            }
        }
        #endregion

        #region Goblin

        public class NimbleEscapeCreator : FeatCreator
        {
            public override OldFeat CreateFeat()
            {
                return new NimbleEscape();
            }
        }

        public class NimbleEscape : OldFeat
        {
            public NimbleEscape()
            {
                name = "Nimble Escape";
            }


        }

        #endregion

        #endregion

        #endregion

        #region Monsters

        public override void InitializeMonsters()
        {
            monsters = new Dictionary<string, MonsterCreator>();
            monsters.Add(Monsters.Goblin.ToString(), new GoblinCreator());
            monsters.Add(Monsters.Commoner.ToString(), new CommonerCreator());
            monsters.Add(Monsters.Cultist.ToString(), new CultistCreator());
            monsters.Add(Monsters.CultFanatic.ToString(), new CultFanaticCreator());
        }

        #region Monster classes

        public class GoblinCreator : MonsterCreator
        {
            public override Creature CreateCreature()
            {
                return new Goblin();
            }
        }

        public class Goblin : Monster
        {
            public Goblin(bool rollHealth = true, bool randomName = true) : base()
            {
                baseStats = new GoblinBaseStats(this);
                name = "Goblin";
                oldFeats.Add(new NimbleEscape());
                Texture = DnDManager.monsterTextures[Monsters.Goblin.ToString()];
                armor = DnDManager.armors[Armors.Leather.ToString()].CreateArmor();
                weaponMainHand = new Scimitar();
                weaponOffHand = new Shield();
                RecalibrateStats();
                ResetHP();

                if (randomName)
                {
                    name = GoblinNameGenerator();
                }
            }

            public class GoblinBaseStats : BaseStats
            {
                public GoblinBaseStats(Creature creature = null, bool rollHP = true) : base(creature, false)
                {
                    creatureSubType = CreatureSubType.Goblinoid.ToString();
                    hitDie = 6;
                    hitDiceNumber = 2;
                    stats[Stat.Strength] = 8;
                    stats[Stat.Dexterity] = 14;
                    stats[Stat.Wisdom] = 8;
                    stats[Stat.Charisma] = 8;
                    skillProficiencies.Add(Skill.Stealth);
                    darkvision = 60;
                    languages.Add(DnDManager.Languages.Goblin.ToString());
                    naturalWeapons = new List<Weapon>() { new Shortbow() };
                    if (rollHP)
                    {
                        RollHP();
                    }
                }
            }

        }

        public class CultistCreator : MonsterCreator
        {
            public override Creature CreateCreature()
            {
                return new Cultist();
            }
        }

        public class Cultist : Monster
        {
            public Cultist(bool rollHealth = true) : base()
            {
                baseStats = new CultistBaseStats(this);
                name = "Cultist";
                armor = DnDManager.armors[Armors.Leather.ToString()].CreateArmor();
                oldFeats.Add(new DarkDevotion());
                weaponMainHand = new Scimitar();
                if (DnDManager.monsterTextures.ContainsKey(Monsters.Cultist.ToString()))
                {
                    Texture = DnDManager.monsterTextures[Monsters.Cultist.ToString()];
                }
                RecalibrateStats();
                ResetHP();

            }

            public class CultistBaseStats : BaseStats
            {
                public CultistBaseStats(Creature creature = null, bool rollHP = true) : base(creature, false)
                {
                    hitDiceNumber = 2;
                    creatureSubType = CreatureSubType.Human.ToString();
                    alignment = Alignment.LawfulEvil;
                    stats[Stat.Strength] = 11;
                    stats[Stat.Dexterity] = 12;
                    stats[Stat.Wisdom] = 11;
                    skillProficiencies.Add(Skill.Deception);
                    skillProficiencies.Add(Skill.Religion);
                    if (rollHP)
                    {
                        RollHP();
                    }
                }
            }
        }

        public class CultFanaticCreator : MonsterCreator
        {
            public override Creature CreateCreature()
            {
                return new CultFanatic();
            }
        }

        public class CultFanatic : Monster
        {
            public CultFanatic(bool rollHealth = true) : base()
            {
                name = "Cult Fanatic";
                baseStats = new CultistFanaticBaseStats();
                armor = DnDManager.armors[Armors.Leather.ToString()].CreateArmor();
                weaponMainHand = new Dagger();
                spellSlots = new SpellSlots(4, SpellCasterType.Full);
                oldSpellbook = new OldSpellBook(new List<OldSpell>()
                    {
                        new LightCantrip(), new SacredFlame(),
                        new Thaumaturgy(), new CommandSpell(),
                        new InflictWounds(), new ShieldOfFaith(),
                        new HoldPerson(), new SpiritualWeapon(),
                    }, this, Stat.Wisdom);
                if (DnDManager.monsterTextures.ContainsKey(Monsters.Cultist.ToString()))
                {
                    Texture = DnDManager.monsterTextures[Monsters.Cultist.ToString()];
                }

                AddFeat(new DarkDevotion());
                AddFeat(new DraconicAncestry());
                RecalibrateStats();
                ResetHP();

            }

            public class CultistFanaticBaseStats : BaseStats
            {
                public CultistFanaticBaseStats(Creature creature = null, bool rollHP = true) : base(creature, false)
                {
                    hitDiceNumber = 6;
                    creatureSubType = CreatureSubType.Human.ToString();
                    alignment = Alignment.LawfulEvil;
                    stats[Stat.Strength] = 11;
                    stats[Stat.Dexterity] = 14;
                    stats[Stat.Constitution] = 12;
                    stats[Stat.Wisdom] = 13;
                    stats[Stat.Charisma] = 14;
                    skillProficiencies.Add(Skill.Deception);
                    skillProficiencies.Add(Skill.Persuasion);
                    skillProficiencies.Add(Skill.Religion);
                    attacks = 2;
                    level = 4;
                    if (rollHP)
                    {
                        RollHP();
                    }

                }
            }

        }

        public class CommonerCreator : MonsterCreator
        {
            public override Creature CreateCreature()
            {
                return new Commoner();
            }
        }

        public class Commoner : Monster
        {
            public Commoner(bool rollHealth = true) : base()
            {
                name = "Commoner";
                weaponMainHand = new Club();
                if (DnDManager.monsterTextures.ContainsKey(Monsters.Commoner.ToString()))
                {
                    Texture = DnDManager.monsterTextures[Monsters.Commoner.ToString()];
                }
                RecalibrateStats();
                ResetHP();
            }
        }

        public class AbolethCreator : MonsterCreator
        {
            public override Creature CreateCreature()
            {
                return new Aboleth();
           }
        }

        public class Aboleth : Monster
        {
            public Aboleth() : base()
            {
                name = "Aboleth";
                cr = 10;
                naturalWeapons = new List<Weapon>() { new AbolethTentacle() };
                oldFeats = new List<OldFeat>() { new Amphibious(), new MucousCloud(), new ProbingTelepathy(), new AbolethEnslave() };
            }

            public class AbolethBaseStats : BaseStats
            {
                public AbolethBaseStats() : base()
                {
                    naturalAC = 17;
                    hitDiceNumber = 18;
                    hitDie = 10;
                    size = Size.Large;
                    speed = 10;
                    swimSpeed = 40;
                    attacks = 3;
                    stats[Stat.Strength] = 21;
                    stats[Stat.Dexterity] = 9;
                    stats[Stat.Constitution] = 15;
                    stats[Stat.Intelligence] = 18;
                    stats[Stat.Wisdom] = 15;
                    stats[Stat.Charisma] = 18;
                    proficiencyBonus = 4;
                    savingThrows = new List<Stat>() { Stat.Constitution, Stat.Intelligence, Stat.Wisdom };
                    skillProficiencies = new List<Skill>() { Skill.History, Skill.Perception };
                    darkvision = 120;
                    languages = new List<string>() { Languages.DeepSpeech.ToString() };
                    
                }
            }

            public class AbolethTentacle : Weapon
            {
                public AbolethTentacle() : base()
                {
                    minRange = 10;
                    maxRange = 10;
                    damageDice = new List<int>() { 6 };
                    damageDiceNumber = new List<int>() { 2 };
                }

                public override void OnHit(Attack attack)
                {

                }
            }

            public class MucousCloudCreator : FeatCreator
            {
                public override OldFeat CreateFeat()
                {
                    return new MucousCloud();
                }
            }

            public class MucousCloud : OldFeat
            {

            }

            public class ProbingTelepathyCreator : FeatCreator
            {
                public override OldFeat CreateFeat()
                {
                    return new ProbingTelepathy();
                }
            }

            public class ProbingTelepathy : OldFeat
            {

            }

            public class AbolethEnslave : OldFeat
            {

            }
        }

        #endregion

        #endregion

        #region Spells

        public enum Cantrips { DancingLights, Light, MageHand, Mending, Message, MinorIllusion, Prestidigitation, TrueStrike, ViciousMockery }
        public enum FirstLevel {AnimalFriendship, Bane };

        public override void InitializeSpells()
        {
            spells = new Dictionary<string, SpellCreator>()
            {
                #region Cantrips
                { Spells.Light.ToString(), new LightCantripCreator()},
                { Spells.SacredFlame.ToString(), new SacredFlameCreator() },
                { Spells.Thaumaturgy.ToString(), new ThaumaturgyCreator() },
                { Cantrips.DancingLights.ToString(), new DancingLightsCreator() },
                { Cantrips.MageHand.ToString(), new MageHandCreator() },
                { Cantrips.Mending.ToString(), new MendingCreator() },
                { Cantrips.Message.ToString(), new MessageCreator() },
                { Cantrips.MinorIllusion.ToString(), new MinorIllusionCreator() },
                { Cantrips.Prestidigitation.ToString(), new PrestidigitationCreator() },
                { Cantrips.TrueStrike.ToString(), new TrueStrikeCreator() },
                { Cantrips.ViciousMockery.ToString(), new ViciousMockeryCreator() },

                #endregion
                #region 1stLevel
                { FirstLevel.AnimalFriendship.ToString(), new AnimalFriendshipCreator() },
                { FirstLevel.Bane.ToString(), new BaneCreator() },
                { Spells.Command.ToString(), new CommandSpellCreator() },
                { Spells.InflictWounds.ToString(), new InflictWoundsCreator() },
                { Spells.ShieldOfFaith.ToString(), new ShieldOfFaithCreator() },
                { Spells.HoldPerson.ToString(), new HoldPersonCreator() },
                { Spells.SpiritualWeapon.ToString(), new SpiritualWeaponCreator() }
                #endregion
            };

            spellLists = new Dictionary<string, Dictionary<int,List<string>>>()
            {
                {Classes.Bard.ToString(),
                    new Dictionary<int, List<string>>()
                { {0, new List<string>() {  Cantrips.DancingLights.ToString(), Cantrips.Light.ToString(), Cantrips.MageHand.ToString(),
                Cantrips.Mending.ToString(), Cantrips.Message.ToString(), Cantrips.MinorIllusion.ToString(),
                Cantrips.Prestidigitation.ToString(), Cantrips.TrueStrike.ToString(), Cantrips.ViciousMockery.ToString(),
                }},
                {1, new List<string>() {FirstLevel.AnimalFriendship.ToString(), FirstLevel.Bane.ToString() } } } }
            };

        }

        #region Cantrips

        public class LightCantripCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new LightCantrip();
            }
        }
        public class LightCantrip : OldSpell
        {
            public LightCantrip()
            {
                name = "Light";
                spellLevel = 0;
            }
        }
        public class SacredFlameCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new SacredFlame();
            }
        }
        public class SacredFlame : OldSpell
        {
            public SacredFlame()
            {
                name = "Sacred Flame";
                spellLevel = 0;
                simpleSpellTargetRollDamage = true;
                savingThrow = true;
                maxRange = 60;
                minRange = 60;
                saveStat = Stat.Dexterity;
                damageTypes = new List<string>() { DamageType.Radiant.ToString() };
                damageDice = new List<int>() { 8 };
                noneOnSave = true;
            }

            public override void GetDamageDice(Attack attack)
            {
                if (caster.level < 5)
                {
                    damageDiceNumber = new List<int>() { 1 };
                }
                else if (caster.level < 11)
                {
                    damageDiceNumber = new List<int>() { 2 };
                }
                else if (caster.level < 17)
                {
                    damageDiceNumber = new List<int>() { 3 };
                }
                else
                {
                    damageDiceNumber = new List<int>() { 4 };
                }

                base.GetDamageDice(attack);
            }

        }
        public class ThaumaturgyCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new Thaumaturgy();
            }
        }
        public class Thaumaturgy : OldSpell
        {
            public Thaumaturgy()
            {
                name = "Thaumaturgy";
                spellLevel = 0;
            }
        }

        public class DancingLightsCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new DancingLights();
            }
        }

        public class DancingLights : OldSpell
        {
            public DancingLights() : base()
            {
                name = "Dancing Lights";
                spellLevel = 0;
            }
        }

        public class MageHandCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new MageHand();
            }
        }

        public class MageHand : OldSpell
        {
            public MageHand() : base()
            {
                name = "Mage Hand";
                spellLevel = 0;
            }

        }

        public class MendingCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new Mending();
            }
        }

        public class Mending : OldSpell
        {
            public Mending() : base()
            {
                name = "Mending";
                spellLevel = 0;
            }

        }

        public class MessageCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new Message();
            }
        }

        public class Message : OldSpell
        {
            public Message() : base()
            {
                name = "Message";
                spellLevel = 0;
            }
        }

        public class MinorIllusionCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new MinorIllusion();
            }
        }

        public class MinorIllusion : OldSpell
        {
            public MinorIllusion() : base()
            {
                name = "Minor Illusion";
                spellLevel = 0;
            }
        }

        public class PrestidigitationCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new Prestidigitation();
            }
        }

        public class Prestidigitation : OldSpell
        {
            public Prestidigitation() : base()
            {
                name = "Prestidigitation";
                spellLevel = 0;
            }
        }

        public class TrueStrikeCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new TrueStrike();
            }
        }
        public class TrueStrike : OldSpell
        {
            public TrueStrike() : base()
            {
                name = "True Strike";
                spellLevel = 0;
            }
        }

        public class ViciousMockeryCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new ViciousMockery();
            }
        }
        public class ViciousMockery : OldSpell
        {
            public ViciousMockery() : base()
            {
                name = "Vicious Mockery";
                spellLevel = 0;
            }
        }


        #endregion

        #region 1st Level

        public class AnimalFriendshipCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new AnimalFriendship();
            }
        }

        public class AnimalFriendship : OldSpell
        {
            public AnimalFriendship()
            {
                name = "Animal Friendship";
                spellLevel = 1;
            }
        }

        public class BaneCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new Bane();
            }
        }

        public class Bane : OldSpell
        {
            public Bane()
            {
                name = "Bane";
                spellLevel = 1;
            }
        }

        public class CommandSpellCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new CommandSpell();
            }
        }
        public class CommandSpell : OldSpell
        {
            public CommandSpell()
            {
                name = "Command";
                spellLevel = 1;
            }
        }

        public class InflictWoundsCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new InflictWounds();
            }
        }
        public class InflictWounds : OldSpell
        {
            public InflictWounds()
            {
                name = "Inflict Wounds";
                spellLevel = 1;
                simpleSpellTargetRollDamage = true;
                spellAttack = true;
                maxRange = 5;
                minRange = 5;
                damageTypes = new List<string>() { DamageType.Necrotic.ToString() };
                damageDice = new List<int>() { 10 };
                damageDiceNumber = new List<int>() { 3 };
            }
        }

        public class ShieldOfFaithCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new ShieldOfFaith();
            }
        }
        public class ShieldOfFaith : OldSpell
        {
            public ShieldOfFaith()
            {
                name = "Shield Of Faith";
                spellLevel = 1;
            }
        }

        #endregion

        #region 2nd Level

        public class HoldPersonCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new HoldPerson();
            }
        }
        public class HoldPerson : OldSpell
        {
            public HoldPerson()
            {
                name = "Hold Person";
                spellLevel = 2;
            }
        }

        public class SpiritualWeaponCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new SpiritualWeapon();
            }
        }
        public class SpiritualWeapon : OldSpell
        {
            public SpiritualWeapon()
            {
                name = "Spiritual Weapon";
                spellLevel = 2;
            }
        }

        #endregion

        #region Channel Divinities

        public class TurnUndeadCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new TurnUndead();
            }
        }

        public class TurnUndead : OldSpell
        {
            public TurnUndead() : base()
            {
                name = "Turn Undead";
                maxRange = 30;
                hideInSpellbook = true;
                noSpellSlot = true;
                savingThrow = true;
                saveStat = Stat.Wisdom;
                abilityModifier = Stat.Wisdom;
                conditions = new List<string>() { Conditions.Frightened.ToString() };
                targetType = TargetType.SphereOnSelf;
                simpleSpellTargetRollEffect = true;
            }

            public override void SimpleSphereOnSelfTarget(BoardTile origin = null, bool includeOrigin = false)
            {
                List<BoardTile> tiles = new List<BoardTile>();
                foreach (BoardTile tile in caster.boardTile.board.GetTilesInRange(caster.boardTile, maxRange, includeOrigin))
                {
                    if (tile.creature != null && tile.creature.creatureType == DnDManager.CreatureType.Undead.ToString())
                    {
                        tiles.Add(tile);
                    }
                }
                AreaTargeted(tiles);
            }

            public override void SpellEffect(Creature target)
            {
                target.AddFeat(new Turned(caster));
                if (target is Monster)
                {
                    if ((target as Monster).cr <= GetDestroyLevel())
                    {
                        target.hitPoints = 0;
                        target.HealthCheck();
                    }
                }
            }

            public float GetDestroyLevel()
            {
                if (caster.level < 5)
                {
                    return 0;
                }
                else if (caster.level < 8)
                {
                    return 1 / 2;
                }
                else if (caster.level < 11)
                {
                    return 1;
                }
                else if (caster.level <14)
                {
                    return 2;
                }
                else if (caster.level < 17)
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }

        }

        public class PreserveLifeCreator : SpellCreator
        {
            public override OldSpell CreateSpell()
            {
                return new PreserveLife();
            }
        }

        public class PreserveLife : OldSpell
        {
            int Healing { get { return caster.level * 5; } }
            public PreserveLife() : base()
            {
                name = "Preserve Life";
                useAction = true;
                hideInSpellbook = true;
                noSpellSlot = true;
                maxRange = 30;
            }

        }
        #endregion

        #region Other

        public class DragonBreath : OldSpell
        {
            public DragonBreath() : base()
            {
                name = "Breath Weapon";

                spellLevel = 0;
                perShortRest = true;
                perRest = true;
                perRestSpellSlot = false;
                simpleSpellTargetRollDamage = true;
                savingThrow = true;
                targetType = TargetType.Line;
                saveStat = Stat.Dexterity;
                width = 5;
                maxRange = 30;
                abilityModifier = Stat.Constitution;
                damageTypes = new List<string>() { DamageType.Acid.ToString() };
                damageDice = new List<int>() { 6 };
                noneOnSave = false;
                hideInSpellbook = true;

            }

            public override void GetDamageDice(DamageRoll roll)
            {
                if (caster.level < 6)
                {
                    damageDiceNumber = new List<int>() { 2 };
                }
                else if (caster.level < 11)
                {
                    damageDiceNumber = new List<int>() { 3 };
                }
                else if (caster.level < 16)
                {
                    damageDiceNumber = new List<int>() { 4 };
                }
                else
                {
                    damageDiceNumber = new List<int>() { 5 };
                }
                base.GetDamageDice(roll);
            }
        }

        public class IntimidatingPresenceSpell : OldSpell
        {
            public List<Creature> resistedCreatures;
            public List<Creature> affectedCreatures;

            public List<OldCondition> fears;
            public OldCondition fearThisTurn;

            public IntimidatingPresenceSpell() : base()
            {
                name = "Intimidating Presence";

                spellLevel = 0;
                perShortRest = false;
                perRest = false;
                perRestSpellSlot = false;
                noSpellSlot = true;
                simpleSpellTargetRollEffect = true;
                savingThrow = true;
                targetType = TargetType.SingleTarget;
                saveStat = Stat.Wisdom;
                width = 5;
                maxRange = 30;
                abilityModifier = Stat.Charisma;
                noneOnSave = true;
                hideInSpellbook = true;
                linkedFeat = null;
                conditions = new List<string>() { Conditions.Frightened.ToString() };

                resistedCreatures = new List<Creature>();
                affectedCreatures = new List<Creature>();
                fears = new List<OldCondition>();
            }

            public override Roll SingleTargetSavingThrowTargeted(Creature target)
            {
                if (resistedCreatures.Contains(target))
                {
                    Debug.WriteLine(string.Format("{0} has already resisted {1}", target.name, name));
                    return null;
                }
                foreach (OldCondition con in target.oldFeats)
                {
                    if (fears.Contains(con))
                    {
                        Debug.WriteLine(string.Format("{0} fear was extended with {1}", target.name, name));
                        fearThisTurn = con;
                    }
                }
                return base.SingleTargetSavingThrowTargeted(target);
            }

            public override void SpellEffect(Creature target)
            {
                fearThisTurn = target.MakeFrightened(caster);
                fears.Add(fearThisTurn);
                (linkedFeat as IntimidatingPresence).usedThisTurn = true;
                affectedCreatures.Add(target);
            }

            public override void SingleTargetFail(Creature target)
            {
                base.SingleTargetFail(target);
                resistedCreatures.Add(target);
            }


        }

        #endregion
        #endregion
    }
}