using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BugsbyEngine;


namespace DnD5eBattleApp
{ 
    public enum Alignment { NeutralEvil, Neutral, LawfulEvil }

    public enum CreatureValue { Speed }

    public class Creature : GameObject
    {
        public ValueManager Values { get; set; } = new ValueManager();

        public void SetDefaultValues()
        {
            Values.AddValue<int>(CreatureValue.Speed.ToString(), 30);
        }

        public static Texture2D baseCommonTexture;
        public string name;
        public string controller;

        public Encounter encounter;
        public BoardTile boardTile;
        public int team;
        public int initiative;

        public BaseStats baseStats = new BaseStats();

        public int hitDice;
        public int hitPoints;
        public int temporaryHitPoints = 0;
        public List<string> Proficiencies
        {
            get
            {
                List<string> proficiencies = new List<string>();
                foreach (Skill skill in  skillProficiencies)
                {
                    proficiencies.Add(skill.ToString());
                }
                foreach (Stat stat in savingThrows)
                {
                    proficiencies.Add(stat.ToString());
                }
                proficiencies.AddRange(toolProficiencies);
                foreach (WeaponCategory prof in weaponCategoryProficiencies)
                {
                    proficiencies.Add(prof.ToString());
                }
                foreach (WeaponType prof in weaponTypeProficiencies)
                {
                    proficiencies.Add(prof.ToString());
                }
                foreach (ArmorCategories prof in armorCategoryProficiencies)
                {
                    proficiencies.Add(prof.ToString());
                }

                return proficiencies;
            }
        }

        public List<OldFeat> feats = new List<OldFeat>();

        public SpellBook spellbook;
        public SpellSlots spellSlots = new SpellSlots(0, SpellCasterType.None);
        public Inventory inventory;
        public Armor armor = new Armor();
        public Weapon weaponMainHand = null;
        public Weapon weaponOffHand = null;

        public Creature() : base(new Vector2(0,0))
        {
            Initializing();
            baseStats = new BaseStats(this);
            ResetHP();
            ResetHitDice();
            RecalibrateStats();
        }
        public Creature(BoardTile tile, Texture2D texture = null) : base(new Vector2(0,0), texture??baseCommonTexture, tile)
        {
            Initializing();
            baseStats = new BaseStats(this);
            SetInitialTile(tile);
            RecalibrateStats();
        }

        #region Initializing

        public void Initializing()
        {
            inventory = new Inventory(this, new List<Item>());
            spellbook = new SpellBook(this);
            SetDefaultValues();
        }

        public void SetInitialTile(BoardTile tile)
        {
            SetToTile(tile);
            encounter = tile.board.encounter;
            encounter.creatures.Add(this);
            SortTexture();

        }

        public Texture2D Texture
        {
            get { return transform.Texture; }
            set { transform.SetTexture(value); }
        }

        #endregion

        #region Stats
        public string                 creatureType                = "Humanoid";
        public string                 creatureSubType             = "Human";
        public Alignment              alignment                   = Alignment.Neutral;
        public Size                   size                        = Size.Medium;
        public int                    level                       = 1;
                                                                  
        public int                    naturalAC                   = 10;
        public int                    acBonus                     = 0;
        public int                    hitDie                      = 8;
        public int                    hitDiceNumber               = 1;
        public int                    hitPointMax                 = 8;
        public int                    darkvision                  = 0;
        public List<string>           languages                   = new List<string>() { "Common" };
                                                                  
        public int                    attacks                     = 1;
        public int                    attackRange                 = 5;
        public List<Weapon>           naturalWeapons              = new List<Weapon>() { new Weapon() };


        public int                    proficiencyBonus            = 2;
        public List<Stat>            savingThrows                = new List<Stat>();
        public Dictionary<Stat, int> stats                       = new Dictionary<Stat, int>() {
            {Stat.Strength, 10 },
            {Stat.Dexterity, 10 },
            {Stat.Constitution, 10 },
            {Stat.Intelligence, 10 },
            {Stat.Wisdom, 10 },
            {Stat.Charisma, 10 }
        };
        public List<Skill>           skillProficiencies          = new List<Skill>();
        public List<string>           toolProficiencies           = new List<string>();
        public List<WeaponType>      weaponTypeProficiencies     = new List<WeaponType>();
        public List<WeaponCategory> weaponCategoryProficiencies = new List<WeaponCategory>() { WeaponCategory.NaturalWeapon, WeaponCategory.SimpleWeapon };
        public List<ArmorCategories>  armorCategoryProficiencies  = new List<ArmorCategories>();
        public List<Skill>           expertises                  = new List<Skill>();

        public List<string>           vulnerabilities             = new List<string>();
        public List<string>           resistances                 = new List<string>();
        public List<string>           immunities                  = new List<string>();

        public bool                   silenced                    = false;

        public int                    flySpeed                    = 0;
        public int                    burrowSpeed                 = 0;
        public int                    swimSpeed                   = 0;



        public void RecalibrateStats()
        {
            creatureType                    = baseStats.creatureType                                            ;
            creatureSubType                 = baseStats.creatureSubType                                         ;
            alignment                       = baseStats.alignment                                               ;
            size                            = baseStats.size                                                    ;
            level                           = baseStats.level                                                   ;

            naturalAC                       = baseStats.naturalAC                                               ;
            acBonus                         = baseStats.acBonus                                                 ;
            hitDie                          = baseStats.hitDie                                                  ;
            hitDiceNumber                   = baseStats.hitDiceNumber                                           ;
            hitPointMax                     = baseStats.hitPointMax                                             ;
            vulnerabilities                 = new List<string>(baseStats.vulnerabilities)                       ;
            resistances                     = new List<string>(baseStats.resistances)                           ;
            immunities                      = new List<string>(baseStats.immunities  )                          ;


            darkvision                      = baseStats.darkvision                                              ;

            attacks                         = baseStats.attacks                                                 ;
            attackRange                     = baseStats.attackRange                                             ;
            naturalWeapons                  = new List<Weapon>(baseStats.naturalWeapons)                        ;

            proficiencyBonus                = baseStats.proficiencyBonus                                        ;
            savingThrows                    = new List<Stat>(baseStats.savingThrows)                           ;
            stats                           = new Dictionary<Stat, int>(baseStats.stats)                       ;
            skillProficiencies              = new List<Skill>(baseStats.skillProficiencies)                    ;
            toolProficiencies               = new List<string>(baseStats.toolProficiencies)                     ;
            weaponTypeProficiencies         = new List<WeaponType>(baseStats.weaponTypeProficiencies)          ;
            weaponCategoryProficiencies     = new List<WeaponCategory>(baseStats.weaponCategoryProficiencies) ;
            armorCategoryProficiencies      = new List<ArmorCategories>(baseStats.armorCategoryProficiencies)   ;
            expertises                      = new List<Skill>(baseStats.expertises)                            ;
            languages                       = new List<string>(baseStats.languages)                             ;

            silenced                        = false                                                             ;
                
            flySpeed                        = baseStats.flySpeed                                                ;
            burrowSpeed                     = baseStats.burrowSpeed                                             ;
            swimSpeed                       = baseStats.swimSpeed                                               ;

            spellSlots.BlankSpellSlotsMax();

            changeStatsDelegate?.Invoke(this);
            overrideStatsDelegate?.Invoke(this);
        }

        public delegate void RecalibrateStatsDelegateType(Creature creature);
        public RecalibrateStatsDelegateType changeStatsDelegate;
        public RecalibrateStatsDelegateType overrideStatsDelegate;

        #endregion

        #region Movement

        public Value<int> Speed {get => Values.GetValue<int>(CreatureValue.Speed.ToString()); }
        bool canMove = true;

        public bool CanMove { get { if (!canMove) { return false; } else { return amountMoved < baseStats.speed; } } }
        public int AttacksLeft { get { return baseStats.attacks - attacksTaken; } }
        public int MoveSpeedLeft
        {
            get { return Math.Max(Speed.GetValue() - amountMoved, 0); }
        }

        public void MoveTo(BoardTile tile)
        {
            amountMoved += encounter.board.GetDistance(boardTile, tile);
            SetToTile(tile);
        }

        #endregion

        #region Skills and Stats

        public int StatMod(Stat stat)
        {
            return (int)((stats[stat] / 2) - 5);
        }

        public int SkillMod(Skill skill)
        {
            if (Proficiencies.Contains(skill.ToString()))
            {
                return StatMod(baseStats.skillStats[skill]) + baseStats.proficiencyBonus;
            }
            else
            {
                return StatMod(baseStats.skillStats[skill]);
            }
        }
        #endregion

        #region Choices
        public List<List<Skill>> skillChoices = new List<List<Skill>>();
        public List<List<List<string>>> itemChoices = new List<List<List<string>>>();
        public List<string> weaponChoices = new List<string>();
        #endregion

        #region Turn Reset Variables
        public bool turnGone;
        public bool bonusActionTaken;
        public bool actionTaken;
        public int extraActions;
        public bool reactionTaken;
        public bool attacked;
        public int attacksTaken;
        public int amountMoved;
        public bool spellCast;

        public List<Attack> attacksThisTurn = new List<Attack>();

        public int damageTakenSinceTurn = 0;

        public void EndTurn()
        {
            endTurnEvent?.Invoke(this, null);
            damageTakenSinceTurn = 0;
        }

        public void StartTurn()
        {
            amountMoved = 0;
            turnGone = false;
            actionTaken = false;
            attacked = false;
            attacksThisTurn = new List<Attack>();
            bonusActionTaken = false;
            reactionTaken = false;
            spellCast = false;
            extraActions = 0;
            attacksTaken = 0;

            startTurnEvent?.Invoke(this, null);
        }

        #endregion

        #region Board Functions 

        public void SetToTile(BoardTile tile)
        {
            if (boardTile is not null)
            {
                boardTile.creature = null;
            }
            transform.localPosition = new Vector2(0, 0);
            boardTile = tile;
            tile.creature = this;
            SetParent(tile, true, true);
        }

        public void SortTexture()
        {
            transform.layerDepth = -0.01f;
            transform.SetSize(boardTile.transform.Size);
        }

        public bool HasLineOfSight(Creature sightCreature)
        {
            return true;
        }

        #endregion

        #region Health Functions

        public void ResetHP()
        {
            RecalibrateStats();
            hitPoints = hitPointMax;
            temporaryHitPoints = 0;
        }

        public void ResetHitDice()
        {
            hitDice = baseStats.hitDiceNumber;
        }

        public void Heal(int amount, int numberOfDie = 1, bool die = false)
        {
            if (die)
            {
                int newAmount = 0;
                for ( int i = 0; i < numberOfDie; i++)
                {
                    newAmount += EngManager.random.Next(1, amount + 1);
                }
                amount = newAmount;
            }
            hitPoints += amount;
            if (hitPoints > baseStats.hitPointMax)
            {
                hitPoints = baseStats.hitPointMax;
            }
            HealthCheck();
        }

        public void TakeDamage(Attack attack)
        {
            for (int d = 0; d < attack.damageRoll.scoresPerDie.Count; d++)
            {
                int damageTaken = attack.damageRoll.scoresPerDie[d];
                string damageType = attack.damageTypes[d];

                TakeDamage(damageTaken, damageType, attack.attackerWeapon);
            }
        }

        public void TakeDamage(List<int> damages, List<string> types, Object source, bool halfDamage = false)
        {
            for (int i = 0; i < damages.Count; i++)
            {
                int damageTaken = damages[i];
                string damageType = types[i];

                if (halfDamage)
                {
                    damageTaken = (int)(damageTaken / 2);
                }

                TakeDamage(damageTaken, damageType, source);
            }
        }

        public void TakeDamage(int damage, string type, Object source)
        {
            if (baseStats.resistances.Contains(type))
            {
                damage /= 2;
            }
            if (baseStats.immunities.Contains(type))
            {
                damage = 0;
            }
            if (baseStats.vulnerabilities.Contains(type))
            {
                damage *= 2;
            }

            Debug.WriteLine(string.Format("{0} took {1} {2} damage", name, damage, type));
            damageTakenSinceTurn += damage;
            hitPoints -= damage;
            TakeDamageEvent(damage, type, source);
            HealthCheck();
        }

        public void HealthCheck()
        {
            RecalibrateStats();
            if (hitPoints <= 0)
            {
                Debug.WriteLine(string.Format("{0} fell unconscious!", name));
            }
        }

        public void SetHP(int hp)
        {
            hitPoints = hp;
            HealthCheck();
        }

        #endregion

        #region Feats and Conditions

        public bool CheckFeat(Type type)
        {
            foreach (OldFeat feat in feats)
            {
                if (feat.GetType().IsAssignableFrom(type))
                {
                    return true;
                }
            }
            return false;
        }

        public OldFeat GetFeat(Type type)
        {
            foreach (OldFeat feat in feats)
            {
                if (feat.GetType().IsAssignableFrom(type))
                {
                    return feat;
                }
            }
            return null;
        }

        public OldFeat RemoveFeat(Type type, bool allConditions = false)
        {
            List<OldFeat> listFeats = new List<OldFeat>(feats);
            foreach (OldFeat feat in listFeats)
            {
                if (feat.GetType().IsAssignableFrom(type))
                {
                    feat.RemoveFeat();
                    feats.Remove(feat);
                    if (!allConditions)
                    {
                        RecalibrateStats();
                        return feat;
                    }
                }
            }
            RecalibrateStats();
            return null;
        }

        public bool CheckFeat(string form)
        {
            Type type;
            if (DnDManager.feats.ContainsKey(form))
            {
                OldFeat exampleFeat = DnDManager.feats[form].CreateFeat();
                type = exampleFeat.GetType();
            }
            if (DnDManager.conditions.ContainsKey(form))
            {
                OldCondition exampleCondition = DnDManager.conditions[form].CreateCondition();
                type = exampleCondition.GetType();
            }
            else
            {
                type = null;
            }

            foreach (OldFeat feat in feats)
            {
                if (feat.GetType().IsAssignableFrom(type))
                {
                    return true;
                }
            }
            return false;
        }

        public OldFeat RemoveFeat(OldFeat feat)
        {
            feat.RemoveFeat();
            feats.Remove(feat);
            RecalibrateStats();
            return feat;
        }

        public OldFeat AddFeat(OldFeat feat)
        {
            feat.creature = this;
            feats.Add(feat);
            feat.AddFeat();
            RecalibrateStats();
            return feat;
        }

        public bool HasFeatChoices()
        {
            foreach (OldFeat feat in feats)
            {
                if (feat.HasChoices())
                {
                    return true;
                }
            }
            return false;
        }

        public OldCondition AddCondition(string condition)
        {
            return AddFeat(DnDManager.conditions[condition].CreateCondition()) as OldCondition;
        }

        public bool CheckCondition(string condition)
        {
            return CheckFeat(DnDManager.conditions[condition].CreateCondition().GetType());
        }

        public OldCondition RemoveCondition(string condition, bool removeAll = false)
        {
            return RemoveFeat(DnDManager.conditions[condition].CreateCondition().GetType(), removeAll) as OldCondition;
        }

        public void GainExhaustion()
        {

        }

        public OldCondition MakeFrightened(Creature creature)
        {
            return null;
        }

        #endregion

        #region Battling

        public List<Weapon> GetAttackWeapons()
        {
            List<Weapon> weaponList = new List<Weapon>();
            if (weaponMainHand != null) { weaponList.Add(weaponMainHand); }
            if (weaponOffHand != null) { weaponList.Add(weaponOffHand); }
            if (naturalWeapons.Count > 0) { weaponList.AddRange(naturalWeapons); }

            return weaponList;
        }

        public List<Weapon> GetMeleeWeapons()
        {
            List<Weapon> weaponList = new List<Weapon>();
            if (weaponMainHand != null && !weaponMainHand.weaponProperties.Contains(WeaponProperty.Range)) { weaponList.Add(weaponMainHand); }
            if (weaponOffHand != null && !weaponMainHand.weaponProperties.Contains(WeaponProperty.Range)) { weaponList.Add(weaponOffHand); }
            if (naturalWeapons.Count > 0)
            {
                foreach (Weapon weapon in naturalWeapons)
                {
                    if (!weapon.weaponProperties.Contains(WeaponProperty.Range))
                    {
                        weaponList.AddRange(naturalWeapons);
                    }
                }
            }

            return weaponList;
        }

        public bool IsProficientInWeapon(Weapon weapon)
        {
            return (Proficiencies.Contains(weapon.weaponCategory.ToString()) || Proficiencies.Contains(weapon.weaponType.ToString()) || weapon.weaponCategory == WeaponCategory.NaturalWeapon);
        }

        public int AC
        {
            get { return Math.Max(baseStats.naturalAC + acBonus, armor.GetAC(this)) + WeaponACBonus() + acBonus; }
        }

        public int WeaponACBonus()
        {
            int bonus = 0;
            if (weaponMainHand != null)
            {
                bonus += weaponMainHand.acBonus;
            }
            if (weaponOffHand != null)
            {
                bonus += weaponOffHand.acBonus;
            }
            return bonus;
        }

        public Attack Attack(Creature defender, Weapon weapon, int numberOfAttacks = 1, bool standardAction = true, bool bonusAction = false, bool offHand = false)
        {
            attacked = true;
            if (standardAction)
            {
                actionTaken = true;
                attacksTaken += numberOfAttacks;
            }
            if (bonusAction)
            {
                bonusActionTaken = bonusAction;
            
            }
            return new Attack(this, defender, weapon, offHand, numberOfAttacks);
        }

        #endregion

        #region Roll Check
        public event RollDelegate preRollCheck;
        public event RollDelegate postRollCheck;
        public event RollDelegate finishRollCheck;

        public Dictionary<Roll, List<Object>> preRollCheckDoneObjects = new Dictionary<Roll, List<Object>>();
        public Dictionary<Roll, List<Object>> postRollCheckDoneObjects = new Dictionary<Roll, List<Object>>();
        public Dictionary<Roll, int> preRollCheckDoneNumber = new Dictionary<Roll, int>();
        public Dictionary<Roll, int> postRollCheckDoneNumber = new Dictionary<Roll, int>();

        public void PreRollCheck(Roll roll, RollEventArgs e)
        {
            if (preRollCheck == null)
            {
                roll.PreRollCheckDone(this);
            }
            else
            {
                preRollCheckDoneObjects.Add(roll, new List<Object>());
                preRollCheckDoneNumber.Add(roll, preRollCheck.GetInvocationList().GetLength(0));
                preRollCheck.Invoke(roll, e);
            }
        }
        public void PreRollCheckDone(Object obj, Roll roll)
        {
            preRollCheckDoneObjects[roll].Add(obj);
            if (preRollCheckDoneObjects[roll].Count >= preRollCheckDoneNumber[roll])
            {
                preRollCheckDoneObjects.Remove(roll);
                preRollCheckDoneNumber.Remove(roll);
                roll.PreRollCheckDone(this);
            }
        }

        public void PostRollCheck(Roll roll, RollEventArgs e)
        {
            if (postRollCheck == null)
            {
                roll.PostRollCheckDone(this);
            }
            else
            {
                postRollCheckDoneObjects.Add(roll, new List<Object>());
                postRollCheckDoneNumber.Add(roll, postRollCheck.GetInvocationList().GetLength(0));
                postRollCheck?.Invoke(roll, e);
            }
        }
        public void postRollCheckDone(Object obj, Roll roll)
        {
            postRollCheckDoneObjects[roll].Add(obj);
            if (postRollCheckDoneObjects[roll].Count >= postRollCheckDoneNumber[roll])
            {
                postRollCheckDoneObjects.Remove(roll);
                postRollCheckDoneNumber.Remove(roll);
                roll.PostRollCheckDone(this);
            }
        }

        public void FinishRollCheck(Roll roll, RollEventArgs e)
        {
            finishRollCheck?.Invoke(roll, e);
        }


        #endregion

        #region Delegates

        public class CreatureDelegateEventArgs : EventArgs
        {
            public Object source;

            public int amount;
            public string type;

            public CreatureDelegateEventArgs(Object source = null, int amount = 0, string type = null)
            {
                this.source = source;
                this.amount = amount;
                this.type = type;
            }

            public Creature GetCreatureSource()
            {
                if (source is Weapon)
                {
                    Weapon weaponSource = source as Weapon;
                    return weaponSource.inventory.creature;
                }
                if (source is Spell)
                {
                    Spell spellSource = source as Spell;
                    return spellSource.caster;
                }
                else
                {
                    return null;
                }
            }
        }

        public delegate void CreatureDelegate(Creature creature, CreatureDelegateEventArgs e);

        public event CreatureDelegate takeDamageEvent;
        public event CreatureDelegate shortRestEvent;
        public event CreatureDelegate longRestEvent;
        public event CreatureDelegate endTurnEvent;
        public event CreatureDelegate startTurnEvent;

        public void ShortRest()
        {
            shortRestEvent?.Invoke(this, null);
        }

        public void LongRest()
        {
            shortRestEvent?.Invoke(this, null);
            longRestEvent?.Invoke(this, null);
        }

        public void TakeDamageEvent(int amount, string type, Object source)
        {
            takeDamageEvent?.Invoke(this, new CreatureDelegateEventArgs(source, amount, type));
        }

        #endregion

        #region Species & Classes
        public Species species = null;
        public SubSpecies subSpecies = null;
        public Dictionary<PlayerClass, int> classes = new Dictionary<PlayerClass, int>();
        public List<SubClass> subclasses = new List<SubClass>();
        public PlayerClass primaryClass = null;

        public bool HasSubClass(Type type)
        {
            foreach (SubClass sc in subclasses)
            {
                if (sc.GetType().IsAssignableFrom(type))
                {
                    return true;
                }
                    
            }
            return false;
        }

        public int ClassLevel(string playerClass)
        {
            PlayerClass playerClassExample = DnDManager.classes[playerClass];
            if (classes.ContainsKey(playerClassExample))
            {
                return classes[playerClassExample];
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}