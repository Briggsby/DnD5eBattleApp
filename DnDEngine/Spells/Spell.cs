using System.Diagnostics;
using System.Collections.Generic;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public abstract class SpellCreator
    {
        public abstract Spell CreateSpell();
    }

    public class Spell
    {
        public string name;
        public int spellLevel;
        public Creature caster;
        public SpellBook spellbook;

        public string spellClass;
        public Stat abilityModifier;

        public bool hideInSpellbook = false;

        public bool useAction = true;
        public bool useBonusAction = false;

        public bool perRest = false;
        public bool perRestSpellSlot = false;
        public bool perShortRest = false;
        public bool perLongRest = false;
        public bool usedThisRest = false;

        public bool scroll = false;

        public bool childMenu = false;

        public OldFeat linkedFeat;
        public bool noSpellSlot = false;

        public Spell() { }

        public Spell(SpellSpec spellSpec, Creature caster = null)
        {
            name = spellSpec.Name;
            spellLevel = spellSpec.Level;
            useAction = spellSpec.IsAction;
            useBonusAction = spellSpec.IsBonusAction;
            maxRange = spellSpec.Range;
            if (spellSpec.IsSpellAttack)
            {
                spellAttack = spellSpec.IsSpellAttack;
                targetType = spellSpec.TargetType;
                simpleSpellTargetRollDamage = true;
                damageDice = new List<int>();
                damageDiceNumber = new List<int>();
                damageTypes = new List<string>();
                foreach (DamageSpec damageSpec in spellSpec.Damages)
                {
                    damageDice.Add(damageSpec.DamageDiceValue);
                    damageDiceNumber.Add(damageSpec.DamageDiceNumber);
                    damageTypes.Add(damageSpec.Type.ToString());
                }           
            }
            if (caster != null)
            {
                this.caster = caster;
            }
        }

        public virtual ContextMenuTemplate GetChildMenu()
        {
            return null;
        }

        public virtual ContextMenuTemplate ChildMenuBase()
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(caster.encounter.contextMenuTextures.blankTileTextures);
            template.font = caster.encounter.contextMenuTextures.baseFont;
            template.texts = new List<string>();
            template.tags = new List<List<string>>();

            return template;
        }

        public virtual List<string> GetTagsForchildMenu(List<string> tags)
        {
            List<string> returnTags = new List<string>() { CombatActions.SpellChildMenu.ToString(), spellbook.spells.IndexOf(this).ToString() };
            returnTags.AddRange(tags);

            return returnTags;
        }

        public virtual void ChildMenuPress(List<string> tags)
        {

        }

        public virtual bool CheckCastable(Creature creature)
        {
            if (perRest && usedThisRest)
            {
                return false;
            }
            if (spellLevel > 0 && (!perRest || perRestSpellSlot) && !(creature.spellSlots.spellSlotsCurrent[spellLevel] > 0))
            {
                return false;
            }
            else
            {
                if (useAction && creature.actionTaken || useBonusAction && creature.bonusActionTaken)
                {
                    return false;
                }
                return true;
            }
        }

        public virtual void CastSpell(Creature creature)
        {
            if (simpleSpellTargetRollDamage || simpleSpellTargetRollEffect)
            {
                Debug.WriteLine(string.Format("Targeting {0}", name));
                if (targetType == TargetType.SingleTarget)
                {
                    SingleNormalTarget();
                }
                else if (targetType == TargetType.Cone)
                {
                    SimpleConeTarget();
                }
                else if (targetType == TargetType.Line)
                {
                    SimpleLineTarget();
                }
                else if (targetType == TargetType.Sphere)
                {
                    SimpleSphereTarget();
                }
                else if (targetType == TargetType.SphereOnSelf)
                {
                    SimpleSphereOnSelfTarget();
                }
            }
        }

        public virtual void UseSpellSlot(bool useActionsToo = true)
        {
            Debug.WriteLine(string.Format("{0} is casting {1}", caster.name, name));
            if ((!perRest || perRestSpellSlot) && !noSpellSlot)
            {
                if (spellLevel > 0)
                {
                    caster.spellSlots.spellSlotsCurrent[spellLevel]--;
                }
            }
            if (perRest)
            {
                usedThisRest = true;
            }

            if (useActionsToo)
            {
                if (useAction)
                {
                    caster.actionTaken = true;
                }
                if (useBonusAction)
                {
                    caster.bonusActionTaken = true;
                }
            }
        }

        public virtual int GetDC()
        {
            return 8 + caster.StatMod(abilityModifier) + caster.proficiencyBonus;
        }

        public bool simpleSpellTargetRollDamage = false;
        public bool simpleSpellTargetRollEffect = false;
        public enum TargetType {SingleTarget, Sphere, Cone, Line, SphereOnSelf}
        public TargetType targetType = TargetType.SingleTarget;
        public bool spellAttack = false;
        public bool savingThrow = false;
        public bool noneOnSave = false;
        public bool addBonus = false;
        public int bonusAdded = 0;

        public List<string> conditions;

        public Stat saveStat;

        public List<int> damageDice;
        public List<int> damageDiceNumber;
        public List<string> damageTypes;
        public int minRange;
        public int maxRange;
        public int width; //assume means radius for circles/spheres   

        public Creature target;
        public SavingThrow saveRoll;
        public List<FeatCreator> effectsOnFail = new List<FeatCreator>();

        public Dictionary<Creature, Roll> savingThrows;
        public List<Creature> targets;

        public virtual void GetDamageDice(Attack attack)
        {
            attack.damageDiceFaces = new List<int>(damageDice);
            attack.damageDiceNumber = new List<int>(damageDiceNumber);
            attack.damageTypes = new List<string>(damageTypes);
        }

        public virtual void GetDamageDice(DamageRoll roll)
        {
            roll.diceFaces = new List<int>(damageDice);
            roll.numberOfDice = new List<int>(damageDiceNumber);
        }

        public virtual void SingleNormalTarget()
        {
            new SpellSingleNormalTargetOrder(caster, this);
        }

        public virtual void SingleTargetTargeted(Creature target)
        {
            UseSpellSlot();
            if (simpleSpellTargetRollDamage)
            {
                if (spellAttack)
                {
                    SingleTargetAttackTargeted(target);
                }
                else if (savingThrow)
                {
                    SingleTargetSavingThrowTargeted(target);
                }
            }
        }

        public virtual void SimpleConeTarget(BoardTile origin = null)
        {
            new DirectionalAreaOrder(origin??caster.boardTile, maxRange, width, TargetTypeToStyle(targetType), null, this);
        }

        public virtual void SimpleLineTarget(BoardTile origin = null)
        {
            new DirectionalAreaOrder(origin ?? caster.boardTile, maxRange, width, TargetTypeToStyle(targetType), null, this);
        }

        public virtual void SimpleSphereTarget(BoardTile origin = null)
        {
            new DirectionalAreaOrder(origin ?? caster.boardTile, maxRange, width, TargetTypeToStyle(targetType), null, this);
        }

        public virtual void SimpleSphereOnSelfTarget(BoardTile origin = null, bool includeOrigin = false)
        {
            List<BoardTile> tiles = new List<BoardTile>();
            foreach(BoardTile tile in caster.encounter.board.GetTilesInRange(caster.boardTile, maxRange, includeOrigin))
            {
                if (tile.creature != null)
                {
                    tiles.Add(tile);
                }
            }
            AreaTargeted(tiles);
        }

        public DirectionalAreaOrder.Style TargetTypeToStyle(TargetType targetType)
        {
            switch(targetType)
            {
                case (TargetType.Sphere):
                    return DirectionalAreaOrder.Style.Circle;
                case (TargetType.Line):
                    return DirectionalAreaOrder.Style.Line;
                case (TargetType.Cone):
                    return DirectionalAreaOrder.Style.Cone;
            }
            return DirectionalAreaOrder.Style.Circle;
        }

        public virtual void AreaTargeted(List<BoardTile> tiles)
        {
            Debug.WriteLine("hello");
            UseSpellSlot();
            savingThrows = new Dictionary<Creature, Roll>();
            finishedSavingThrows = new List<Roll>();
            targets = new List<Creature>();
            if (simpleSpellTargetRollDamage)
            {
                if (savingThrow)
                {
                    foreach(BoardTile tile in tiles)
                    {
                        if (tile.creature != null)
                        {
                            targets.Add(tile.creature);
                            savingThrows.Add(tile.creature, new SavingThrow(saveStat, tile.creature, this, conditions));
                        }
                    }
                }
            }
        }

        public virtual Attack SingleTargetAttackTargeted(Creature target)
        {
            this.target = target;
            return new Attack(caster, target, this);
        }

        public virtual Roll SingleTargetSavingThrowTargeted(Creature target)
        {
            this.target = target;
            return new SavingThrow(saveStat, target, this);
        }

        public List<Roll> finishedSavingThrows;
        public virtual void SavingThrowFinished(Roll roll, RollEventArgs e)
        {
            if (roll.Success)
            {
                Debug.WriteLine(string.Format("{0} succeeded on their saving throw against {1}'s {2}", roll.roller, this.caster, name));
            }
            else
            {
                Debug.WriteLine(string.Format("{0} failed on their saving throw against {1}'s {2}", roll.roller, this.caster, name));
            }
            if (simpleSpellTargetRollDamage)
            {
                if (targetType == TargetType.SingleTarget)
                {
                    if (roll.Success)
                    {
                        SingleTargetFail(roll.roller);
                    }
                    else
                    {
                        SingleTargetSuccess(roll.roller);
                    }
                }
                else
                {
                    finishedSavingThrows.Add(roll);
                    if (finishedSavingThrows.Count == savingThrows.Keys.Count)
                    {
                        MakeDamageRoll();
                    }
                }
            }
            if (simpleSpellTargetRollEffect)
            {
                    if (roll.Success)
                    {
                        SingleTargetFail(roll.roller);
                    }
                    else
                    {
                        SingleTargetSuccess(roll.roller);
                    }

            }
        }

        public virtual void SingleTargetSuccess(Creature target)
        {
            if (simpleSpellTargetRollDamage)
            {
                if (spellAttack)
                {
                }
                else if (savingThrow)
                {
                    MakeDamageRoll();
                }
            }
            else if (simpleSpellTargetRollEffect)
            {
                SpellEffect(target);
            }
        }

        public virtual void MakeDamageRoll()
        {
            DamageRoll damageRoll = new DamageRoll(this);
            damageRoll.DoRoll();
        }

        public virtual void SpellEffect(Creature target)
        {
            foreach (FeatCreator featCreator in effectsOnFail)
            {
                target.AddFeat(featCreator.CreateFeat());
            }
        }

        public virtual void SingleTargetFail(Creature target)
        {
            if (spellAttack)
            {
                return;
            }
            else if (savingThrow)
            {
                if (noneOnSave)
                {
                    return;
                }
                else
                {
                    if(simpleSpellTargetRollDamage)
                    {
                        MakeDamageRoll();
                    }
                }
            }
        }

        bool waitingForFirstDamageRoll = false;

        public virtual int GetDamageBonus()
        {
            if (addBonus)
            {
                return bonusAdded + caster.StatMod(abilityModifier);
            }
            else
            {
                return bonusAdded;
            }
        }

        public virtual void FinishDamageRoll(Roll roll, RollEventArgs e)
        {
            //Debug.WriteLine(string.Format("{0} did {1} {2} damage to {3} with a {4}", caster.name, roll.score, damageTypes[0], rollDefender.name, name));
            DamageRoll damageRoll = roll as DamageRoll;

            if (targets == null)
            {
                foreach (Creature creature in targets)
                {
                    if (!savingThrows[creature].Success || !noneOnSave)
                    {
                        creature.TakeDamage(damageRoll.scoresPerDie, damageRoll.damageTypes, savingThrows[creature].Success);
                    }
                }
            }
            else
            {
                if (!saveRoll.Success || !noneOnSave)
                {
                    target.TakeDamage(damageRoll.scoresPerDie, damageRoll.damageTypes, saveRoll.Success);
                }
            }
        }

    }
}