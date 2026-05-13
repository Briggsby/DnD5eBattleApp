using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BugsbyEngine;

namespace DnD5eBattleApp;

public enum Alignment { NeutralEvil, Neutral, LawfulEvil }


public class Creature : GameObject
{
    public static Texture2D BaseCommonTexture {get; set;}
    public CreatureValueManager Values { get; set; } = new CreatureValueManager();
    public string Name {get; set;}

    public Encounter Encounter {get; set;}
    public BoardTile BoardTile {get; set;}
    // public int team;

    public BaseStats baseStats = new BaseStats();

    public int MaxHP {get => GetValue<int>(CreatureValue.HitPoints).BaseValue; set => GetValue<int>(CreatureValue.HitPoints).SetBaseValue(value); }
    public int HP {
        get => GetValue<int>(CreatureValue.HitPoints).CurrentValue;
        set => GetValue<int>(CreatureValue.HitPoints).ModifyValue(value); 
    }

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

    public List<OldFeat> oldFeats = new List<OldFeat>();
    public List<Feat> Feats {get; set;} = new List<Feat>();

    public SpellBook SpellBook {get; set;}
    public SpellSlots spellSlots = new SpellSlots(0, SpellCasterType.None);
    public Inventory inventory;
    public Armor armor = new Armor();
    public Weapon weaponMainHand = null;
    public Weapon weaponOffHand = null;

    public Creature() : base(new Vector2(0,0), BaseCommonTexture, null)
    {
        Initializing();
        baseStats = new BaseStats(this);
        ResetHP();
        RecalibrateStats();
    }

    public Creature(BoardTile tile, Texture2D texture = null) : base(new Vector2(0,0), texture??BaseCommonTexture, tile)
    {
        Initializing();
        baseStats = new BaseStats(this);
        SetInitialTile(tile);
        RecalibrateStats();
    }

    #region Initializing

    public void Initializing()
    {
        inventory = new Inventory(this, new List<string>());
    }

    public void SetInitialTile(BoardTile tile)
    {
        SetToTile(tile);
        Encounter = tile.board.encounter;
        Encounter.creatures.Add(this);
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

    public bool CanMove { get { if (!canMove) { return false; } else { return AmountMoved < baseStats.speed; } } }
    public int AttacksLeft { get { return baseStats.attacks - attacksTaken; } }
    public int MoveSpeedLeft
    {
        get { return Math.Max(Speed.GetValue() - AmountMoved, 0); }
    }

    public void MoveTo(BoardTile tile)
    {
        AmountMoved += Encounter.board.GetDistance(BoardTile, tile);
        SetToTile(tile);
    }

    #endregion

    #region Skills and Stats

    public int StatMod(Stat stat)
    {
        // TODO: Change all of these to use the below version
        return (int)((stats[stat] / 2) - 5);
    }

    public int StatMod(CreatureValue stat)
    {
        return (GetValue<int>(stat).CurrentValue / 2) - 5;
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
    public int AmountMoved { 
        get; 
        set;
    }
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
        AmountMoved = 0;
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
        if (BoardTile is not null)
        {
            BoardTile.creature = null;
        }
        transform.localPosition = new Vector2(0, 0);
        BoardTile = tile;
        tile.creature = this;
        SetParent(tile, true, true);
    }

    public void SortTexture()
    {
        transform.layerDepth = -0.01f;
        transform.SetSize(BoardTile.transform.Size);
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
        HP = MaxHP;
        GetValue<int>(CreatureValue.TemporaryHitPoints).ModifyValue(0);
    }
    public void Heal(int amount)
    {
        int increase = Math.Min(amount, MaxHP - HP);
        HP += increase;
        HealthCheck();
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

        damage = Math.Min(damage, HP);

        Debug.WriteLine(string.Format("{0} took {1} {2} damage", Name, damage, type));
        damageTakenSinceTurn += damage;
        HP -= damage;
        TakeDamageEvent(damage, type, source);
        HealthCheck();
    }

    public void HealthCheck()
    {
        RecalibrateStats();
        if (HP <= 0)
        {
            Debug.WriteLine(string.Format("{0} fell unconscious!", Name));
        }
    }

    public void SetHP(int hp)
    {
        HP = hp;
        HealthCheck();
    }

    #endregion

    #region Feats and Conditions

    public bool CheckFeat(Type type)
    {
        foreach (OldFeat feat in oldFeats)
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
        foreach (OldFeat feat in oldFeats)
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
        List<OldFeat> listFeats = new List<OldFeat>(oldFeats);
        foreach (OldFeat feat in listFeats)
        {
            if (feat.GetType().IsAssignableFrom(type))
            {
                feat.RemoveFeat();
                oldFeats.Remove(feat);
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
        if (DnDManager.oldFeats.ContainsKey(form))
        {
            OldFeat exampleFeat = DnDManager.oldFeats[form].CreateFeat();
            type = exampleFeat.GetType();
        }
        if (DnDManager.oldConditions.ContainsKey(form))
        {
            OldCondition exampleCondition = DnDManager.oldConditions[form].CreateCondition();
            type = exampleCondition.GetType();
        }
        else
        {
            type = null;
        }

        foreach (OldFeat feat in oldFeats)
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
        oldFeats.Remove(feat);
        RecalibrateStats();
        return feat;
    }

    public OldFeat AddFeat(OldFeat feat)
    {
        feat.creature = this;
        oldFeats.Add(feat);
        feat.AddFeat();
        RecalibrateStats();
        return feat;
    }

    public bool HasFeatChoices()
    {
        foreach (OldFeat feat in oldFeats)
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
        return AddFeat(DnDManager.oldConditions[condition].CreateCondition()) as OldCondition;
    }

    public bool CheckCondition(string condition)
    {
        return CheckFeat(DnDManager.oldConditions[condition].CreateCondition().GetType());
    }

    public OldCondition RemoveCondition(string condition, bool removeAll = false)
    {
        return RemoveFeat(DnDManager.oldConditions[condition].CreateCondition().GetType(), removeAll) as OldCondition;
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
    public List<Weapon> GetAttackWeapons(bool onlyEquipped = false)
    {
        List<Weapon> weaponList = new List<Weapon>();
        if (!onlyEquipped)
        {
            weaponList.AddRange(inventory.weapons);
        }
        else {
            if (weaponMainHand != null) { weaponList.Add(weaponMainHand); }
            if (weaponOffHand != null) { weaponList.Add(weaponOffHand); }
        }
        if (naturalWeapons.Count > 0) { weaponList.AddRange(naturalWeapons); }

        return weaponList;
    }

    public List<Weapon> GetMeleeWeapons(bool onlyEquipped = false)
    {
        List<Weapon> weaponList = GetAttackWeapons(onlyEquipped);
        foreach (Weapon weapon in weaponList)
        {
            if (weapon.weaponProperties.Contains(WeaponProperty.Range))
            {
                weaponList.Remove(weapon);
            }
        }
        return weaponList;
    }

    public virtual bool IsProficientInWeapon(Weapon weapon)
    {
        return (Proficiencies.Contains(weapon.weaponCategory.ToString()) || Proficiencies.Contains(weapon.weaponType.ToString()) || weapon.weaponCategory == WeaponCategory.NaturalWeapon);
    }

    public int AC
    {
        get { return Math.Max(baseStats.naturalAC, armor.GetAC(this)) + WeaponACBonus() + acBonus; }
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

    public Value<T> GetValue<T>(CreatureValue value)
    {
        return Values.GetValue<T>(value.ToString());
    }
}