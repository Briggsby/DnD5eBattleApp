//using BugsbyEngine;
//using Microsoft.Xna.Framework;
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Diagnostics;

//namespace DnD5eBattleApp
//{
//    #region Rolls
//    public enum RollType { Initiative, Attack, Damage, Check, SavingThrow }
//    public enum RollTag { Initiative, Attack, Damage, Check, SavingThrow, Spell, AttackDamage, SpellDamage }
//    public enum Advantage { None, Advantage, Disadvantage }
//    public enum RollState { Starting, PreRoll, Rolling, Rerolling, PostRoll, Ended }
//    public enum ConditionTags { Charmed, Sleep, Frightened };
//    public enum MiscRollTags { Magic, Poison }

//    public class OldRoll
//    {
//        public RollType rollType;
//        public List<string> rollTags;
//        public string desc;

//        public RollState rollState;

//        public Creature creatureActor;
//        public Creature creatureDefender;

//        public List<int> diceFaces;
//        public List<int> numberOfDie;
//        public List<List<int>> diceResults;
//        public List<int> scoresPerDie; //standard bonus always applied to the first damage type
//        public int bonus;
//        public int DC;
//        public bool disadvantage;
//        public bool advantage;
//        public int otherDie;
//        public List<bool> reroll;
//        public List<int> rerollMax;
//        public List<int> numberOfRerolls;
//        public bool rerolled;
//        public bool critical;

//        public List<List<int>> setDiceResults;

//        public bool nat1;
//        public bool nat20;
//        public bool success;
//        public int score;

//        public Spell spell;
//        public Attack attack;
//        public Creature Creature;
//        public Stats attackStat;
//        public Stats savingThrow;
//        public Stats stat;
//        public Skills skill;
//        public bool halfRoll = false;

//        public Roll(Attack attack, Advantage advantage)
//        {
//            SetAdvantage(advantage);
//            this.attack = attack;
//            AttackRoll();
//        }

//        public Roll(Attack attack, bool damage, bool critical = false)
//        {
//            this.attack = attack;
//            this.critical = critical;
//            if (!damage)
//            {
//                AttackRoll();
//            }
//            else
//            {
//                AttackDamageRoll();
//            }
//        }

//        public Roll(Creature encCreature, RollType rollType)
//        {
//            switch (rollType)
//            {
//                case (RollType.Initiative):
//                    InitiativeRoll();
//                    break;
//            }
//        }

//        public Roll(Spell spell, Creature target, RollType rollType, bool half = false, List<string> rollTags = null, List<List<int>> setRoll = null)
//        {
//            creatureActor = spell.caster.Creature;
//            creatureDefender = target;
//            this.rollTags = new List<string>(rollTags ?? new List<string>());
//            this.rollTags.Add(RollTag.Spell.ToString());

//            this.spell = spell;
//            halfRoll = half;

//            if (setRoll != null)
//            {
//                setDiceResults = new List<List<int>>(setRoll);
//            }

//            if (rollType == RollType.SavingThrow)
//            {
//                DoSavingThrow(spell.saveStat, spell.GetDC());
//            }
//            else if (rollType == RollType.Attack)
//            {

//            }
//            else if (rollType == RollType.Damage)
//            {
//                DoSpellDamageRoll();
//            }
//        }

//        public Roll(RollType type, Stats stat, Creature creature, int DC)
//        {
//            creatureDefender = creature;
//            if (type == RollType.SavingThrow)
//            {
//                DoSavingThrow(stat, DC);
//            }
//        }

//        public void SetDefaults(RollType rollType, List<string> rollTags, Creature actor, Creature defender, List<int> die, List<int> numberOfDie, int bonus = 0, int DC = 10, bool advantage = false, bool disadvantage = false, List<bool> reroll = null, List<int> rerollMax = null, bool critical = false, bool challenge = false, Attack attack = null)
//        {
//            rollState = RollState.Starting;
//            DnDManager.ongoingRolls.Add(this);

//            this.rollType = rollType;
//            this.rollTags = new List<string>(rollTags);
//            this.creatureActor = actor;
//            this.creatureDefender = defender;
//            this.diceFaces = new List<int>(die);
//            this.numberOfDie = new List<int>(numberOfDie);
//            this.bonus = bonus;
//            this.DC = DC;
//            this.disadvantage = disadvantage;
//            this.advantage = advantage;
//            this.critical = critical;
//            this.rerolled = false;

//            nat1 = false;
//            nat20 = false;
//            success = false;
//            score = 0;

//            if (reroll == null)
//            {
//                this.reroll = new List<bool>(new bool[diceFaces.Count]);
//                this.rerollMax = new List<int>(new int[diceFaces.Count]);
//            }
//            else
//            {
//                this.reroll = new List<bool>(reroll);
//                this.rerollMax = new List<int>(rerollMax);
//            }
//        }

//        public void DoSavingThrow(Stats stat, int DC)
//        {
//            savingThrow = stat;
//            this.DC = DC;
//            bonus = 0;

//            bonus += creatureDefender.creature.StatMod(stat);
//            if (creatureDefender.creature.savingThrows.Contains(stat))
//            {
//                bonus += creatureDefender.creature.proficiencyBonus;
//            }
//            //Debug.WriteLine(string.Format("{0} is making a {1} saving throw against a DC of {2}", creatureDefender.creature.name, stat.ToString(), DC));


//            SetDefaults(RollType.SavingThrow, rollTags, creatureActor, creatureDefender, new List<int> { 20 }, new List<int> { 1 }, bonus: bonus, DC: DC);
//            CheckPreRoll();
//        }

//        public void DoSpellDamageRoll()
//        {
//            SetDefaults(RollType.Damage, new List<string>() { RollTag.SpellDamage.ToString() }, creatureActor, creatureDefender, new List<int>(), new List<int>());

//            bonus = spell.GetDamageBonus(this);
//            spell.GetDamageDice(this);

//            CheckPreRoll();

//        }

//        public void AttackRoll()
//        {
//            //Get attack bonus
//            bonus = 0;

//            if (!attack.isSpell)
//            {
//                Stats statUsed;
//                if (attack.attackerWeapon.weaponProperties.Contains(WeaponProperty.Finesse))
//                {
//                    bonus += MathHelper.Max(attack.attacker.creature.StatMod(Stats.Strength), attack.attacker.creature.StatMod(Stats.Dexterity));
//                    if (attack.attacker.creature.stats[Stats.Strength] >= attack.attacker.creature.stats[Stats.Dexterity])
//                    {
//                        statUsed = Stats.Strength;
//                    }
//                    else
//                    {
//                        statUsed = Stats.Dexterity;
//                    }
//                }
//                else
//                {
//                    bonus += attack.attacker.creature.StatMod(attack.attackerWeapon.abilityStat);
//                    statUsed = attack.attackerWeapon.abilityStat;
//                }

//                if (attack.attacker.creature.IsProficientInWeapon(attack.attackerWeapon))
//                {
//                    bonus += attack.attacker.creature.proficiencyBonus;
//                }
//                //Debug.WriteLine(string.Format("Bonus is {0}, from a {3} mod of {1} and a proficiency bonus of {2}", bonus, attack.attacker.creature.StatMod(statUsed), attack.attacker.creature.proficiencyBonus, statUsed));
//            }
//            else
//            {
//                spell = attack.spell;
//                bonus = 0;
//                if (!spell.scroll)
//                {
//                    bonus += attack.attacker.creature.StatMod(spell.abilityModifier);
//                    bonus += attack.attacker.creature.proficiencyBonus;
//                }
//                //Debug.WriteLine(string.Format("Bonus is {0}, from a mod of {1} and a proficiency bonus of {2}", bonus, attack.attacker.creature.StatMod(spell.abilityModifier), attack.attacker.creature.proficiencyBonus));
//            }

//            //Get AC
//            DC = attack.defender.creature.AC;

//            SetDefaults(RollType.Attack, new List<string>() { RollTag.Attack.ToString() }, attack.attacker, attack.defender, new List<int> { 20 }, new List<int> { 1 }, bonus: bonus, DC: DC);

//            CheckPreRoll();
//        }

//        public void AttackDamageRoll()
//        {
//            //Get Damage bonus
//            bonus = 0;

//            if (attack.isSpell)
//            {
//                attack.spell.GetDamageBonus(this);
//                attackStat = attack.spell.abilityModifier;
//            }
//            else
//            {
//                if (attack.attackerWeapon.weaponProperties.Contains(WeaponProperty.Finesse))
//                {
//                    if (attack.attacker.creature.StatMod(Stats.Strength) >= attack.attacker.creature.StatMod(Stats.Dexterity))
//                    {
//                        bonus += attack.attacker.creature.StatMod(Stats.Strength);
//                        attackStat = Stats.Strength;
//                    }
//                    else
//                    {
//                        bonus += attack.attacker.creature.StatMod(Stats.Dexterity);
//                        attackStat = Stats.Dexterity;
//                    }
//                }
//                else
//                {
//                    bonus += attack.attacker.creature.StatMod(attack.attackerWeapon.abilityStat);
//                    attackStat = attack.attackerWeapon.abilityStat;
//                }

//            }

//            SetDefaults(RollType.Damage, new List<string>() { RollTag.AttackDamage.ToString() }, attack.attacker, attack.defender, new List<int>(), new List<int>(), bonus: bonus, DC: 0);
//            diceFaces = attack.damageDice;
//            numberOfDie = attack.damageDiceNumber;
//            reroll = new List<bool>(new bool[diceFaces.Count]);
//            rerollMax = new List<int>(new int[diceFaces.Count]);

//            CheckPreRoll();

//        }

//        public void InitiativeRoll()
//        {

//        }

//        public void SetAdvantage(Advantage advantage)
//        {
//            switch (advantage)
//            {
//                case (Advantage.Advantage):
//                    this.advantage = true;
//                    this.disadvantage = false;
//                    break;
//                case (Advantage.Disadvantage):
//                    this.advantage = false;
//                    this.disadvantage = true;
//                    break;
//                case (Advantage.None):
//                    this.advantage = false;
//                    this.disadvantage = false;
//                    break;
//            }
//        }

//        public void CheckPreRoll()
//        {
//            //Debug.WriteLine("Checking PreRoll Feats");
//            rollState = RollState.PreRoll;
//            foreach (Feat feat in creatureActor.creature.feats)
//            {
//                feat.finishedPreRollCheck = false;
//            }
//            foreach (Feat feat in creatureDefender.creature.feats)
//            {
//                feat.finishedPreRollCheck = false;
//            }

//        }

//        public void FinishedCheckPreRoll()
//        {
//            for (int i = 0; i < diceFaces.Count; i++)
//            {
//                //Debug.WriteLine(string.Format("Rolling {0} d{1}s and adding {2}", numberOfDie[i], diceFaces[i], bonus));
//            }
//            rollState = RollState.Rolling;
//            score = DoRoll();
//            CheckPostRoll();
//        }

//        public void SetDiceResults(List<List<int>> setResults)
//        {
//            setDiceResults = setResults;
//        }

//        public int DoSetRoll()
//        {
//            diceResults = new List<List<int>>(setDiceResults);
//            GetScoresPerDie();

//            int sumOfDice = 0;
//            foreach (int i in scoresPerDie)
//            {
//                sumOfDice += i;
//            }

//            if (halfRoll)
//            {
//                return (int)(sumOfDice / 2);
//            }
//            return sumOfDice;

//        }

//        public int DoRoll()
//        {

//            if (setDiceResults != null)
//            {
//                return DoSetRoll();
//            }
//            diceResults = new List<List<int>>(diceFaces.Count);


//            if (critical)
//            {
//                for (int n = 0; n < numberOfDie.Count; n++)
//                {
//                    numberOfDie[n] *= 2;
//                }
//            }

//            int sumOfDice = 0;
//            for (int d = 0; d < diceFaces.Count; d++)
//            {
//                diceResults.Add(new List<int>(numberOfDie[d]));
//                for (int n = 0; n < numberOfDie[d]; n++)
//                {
//                    int roll = RollD(diceFaces[d]);
//                    if ((disadvantage || advantage) && !(disadvantage && advantage))
//                    {
//                        int roll2 = RollD(diceFaces[d]);
//                        if (advantage)
//                        {
//                            otherDie = Math.Min(roll, roll2);
//                            diceResults[d].Add(Math.Max(roll, roll2));
//                            sumOfDice += Math.Max(roll, roll2);
//                        }
//                        if (disadvantage)
//                        {
//                            otherDie = Math.Max(roll, roll2);
//                            diceResults[d].Add(Math.Min(roll, roll2));
//                            sumOfDice += MathHelper.Min(roll, roll2);
//                        }
//                    }
//                    else
//                    {

//                        if (reroll[d])
//                        {
//                            if (roll <= rerollMax[d])
//                            {
//                                int rerollVal = RollD(diceFaces[d]);
//                                diceResults[d].Add(rerollVal);
//                                sumOfDice += rerollVal;
//                            }
//                        }
//                        else
//                        {
//                            diceResults[d].Add(roll);
//                            sumOfDice += roll;
//                        }
//                    }
//                }
//            }
//            GetScoresPerDie();

//            if (halfRoll)
//            {
//                return (sumOfDice / 2);
//            }
//            return sumOfDice + bonus;
//        }

//        public void GetScoresPerDie()
//        {
//            scoresPerDie = new List<int>();
//            for (int d = 0; d < diceFaces.Count; d++)
//            {
//                scoresPerDie.Add(0);
//                foreach (int n in diceResults[d])
//                {
//                    scoresPerDie[d] += n;
//                }
//            }
//            scoresPerDie[0] += bonus;
//        }

//        public void CheckPostRoll()
//        {
//            //Debug.WriteLine("Checking Post Roll Feats");
//            rollState = RollState.PostRoll;
//            success = score >= DC;
//            foreach (Feat feat in creatureActor.creature.feats)
//            {
//                feat.finishedPostRollCheck = false;
//            }
//            foreach (Feat feat in creatureDefender.creature.feats)
//            {
//                feat.finishedPostRollCheck = false;
//            }

//        }

//        public void FinishedCheckPostRoll()
//        {
//            nat20 = CheckNat20();
//            nat1 = CheckNat1();
//            success = score >= DC;
//            // Debug.WriteLine("Finished Roll");
//            rollState = RollState.Ended;
//            FinishRoll();
//        }

//        public void FinishRoll()
//        {
//            //Debug.WriteLine("Finishing Roll");
//            if (rollType == RollType.Attack)
//            {
//                if (nat1)
//                {
//                    success = false;
//                }
//                attack.FinishAttackRoll(this);
//            }
//            if (rollType == RollType.Damage)
//            {

//                if (rollTags.Contains(RollTag.AttackDamage.ToString()))
//                {
//                    Debug.WriteLine(string.Format("{0} rolled a total of {1} ({2} + {3}) damage against {4}", creatureActor.creature.name, score, score - bonus, bonus, creatureDefender.creature.name));
//                    attack.FinishDamageRoll(this);
//                }
//                else if (rollTags.Contains(RollTag.SpellDamage.ToString()))
//                {
//                    if (halfRoll)
//                    {
//                        Debug.WriteLine(string.Format("{0} rolled a total of {1} ({2} + {3}) damage against {4}, who succeeded their saving throw", creatureActor.creature.name, score, score - bonus, bonus, creatureDefender.creature.name));
//                    }
//                    else
//                    {
//                        Debug.WriteLine(string.Format("{0} rolled a total of {1} ({2} + {3}) damage against {4}, who failed their saving throw", creatureActor.creature.name, score, score - bonus, bonus, creatureDefender.creature.name));
//                    }
//                    spell.FinishDamageRoll(this);
//                }
//            }
//            if (rollType == RollType.Initiative)
//            {
//                Creature.SetInitiative(score);
//            }
//            if (rollType == RollType.SavingThrow)
//            {
//                if (rollTags.Contains(RollTag.Spell.ToString()))
//                {
//                    if (success)
//                    {
//                        Debug.WriteLine(string.Format("{0} suceeded, rolling a {1} ({2} + {3}) on the {4} saving throw of DC {5} against {6}'s {7}", creatureDefender.creature.name, score, score - bonus, bonus, savingThrow, DC, creatureActor.creature.name, spell.name));
//                    }
//                    else
//                    {
//                        Debug.WriteLine(string.Format("{0} failed, rolling a {1} ({2} + {3}) on the {4} saving throw of DC {5} against {6}'s {7}", creatureDefender.creature.name, score, score - bonus, bonus, savingThrow, DC, creatureActor.creature.name, spell.name));
//                    }
//                    spell.SavingThrowFinished(this);
//                }
//            }

//            rollFinishedEvent?.Invoke(this, null);
//        }

//        public bool IsRoller(Creature creature)
//        {
//            Creature encCreature = creature.Creature;
//            if (rollType == RollType.Attack || rollType == RollType.Damage)
//            {
//                if (creatureActor == encCreature)
//                {
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            else if (rollType == RollType.SavingThrow)
//            {
//                if (creatureDefender == encCreature)
//                {
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            else
//            {
//                return true;
//            }

//        }

//        bool CheckNat20()
//        {
//            if (((score - bonus) == 20) && diceFaces[0] == 20)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        bool CheckNat1()
//        {
//            if (((score - bonus) == 1) && diceFaces[0] == 20)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public void Update()
//        {
//            #region PreRoll
//            if (rollState == RollState.PreRoll)
//            {
//                //Debug.WriteLine("On Preroll feats");
//                #region Actor
//                if (creatureActor != null)
//                {
//                    foreach (Feat feat in creatureActor.creature.feats)
//                    {
//                        if (!feat.finishedPreRollCheck)
//                        {
//                            //Debug.WriteLine(string.Format("Doing preroll check for {0}", feat.name));

//                            if (!feat.doingPreRollCheck)
//                            {
//                                feat.FeatPreRollCheck(this);
//                            }
//                            return;
//                        }
//                    }
//                }

//                #endregion

//                #region Defender
//                if (creatureDefender != null)
//                {
//                    foreach (Feat feat in creatureDefender.creature.feats)
//                    {
//                        //Debug.WriteLine(string.Format("Doing preroll check for {0}", feat.name));
//                        if (!feat.finishedPreRollCheck)
//                        {
//                            if (!feat.doingPreRollCheck)
//                            {
//                                feat.FeatPreRollCheck(this);
//                            }
//                            return;
//                        }
//                    }
//                }

//                #endregion

//                FinishedCheckPreRoll();
//            }

//            #endregion

//            #region PostRoll
//            if (rollState == RollState.PostRoll)
//            {
//                //Debug.WriteLine("On Postroll feats");
//                #region Actor
//                if (creatureActor != null)
//                {
//                    foreach (Feat feat in creatureActor.creature.feats)
//                    {
//                        //Debug.WriteLine(string.Format("Doing postroll check for {0}", feat.name));
//                        if (!feat.finishedPostRollCheck)
//                        {
//                            if (!feat.doingPostRollCheck)
//                            {
//                                feat.FeatPostRollCheck(this);
//                            }
//                            return;
//                        }
//                    }
//                }

//                #endregion

//                #region Defender
//                if (creatureDefender != null)
//                {
//                    foreach (Feat feat in creatureDefender.creature.feats)
//                    {
//                        if (!feat.finishedPostRollCheck)
//                        {
//                            if (!feat.doingPostRollCheck)
//                            {
//                                //Debug.WriteLine(string.Format("Doing postroll check for {0}", feat.name));
//                                feat.FeatPostRollCheck(this);
//                            }
//                            return;
//                        }
//                    }
//                }

//                #endregion

//                FinishedCheckPostRoll();
//            }

//            #endregion
//        }

//        public void EndRoll()
//        {
//            rollState = RollState.Ended;

//        }

//        public void AutoSuccess()
//        {
//            success = true;
//            rollState = RollState.Ended;
//            FinishRoll();
//        }

//        public int RollD(int d)
//        {
//            int roll = EngManager.random.Next(1, d + 1);
//            //Debug.WriteLine(string.Format("Rolled a {0}", roll));
//            return roll;

//        }

//        public void ReRoll(bool takeBest = false)
//        {
//            rerolled = true;
//            DoRoll();
//            CheckPostRoll();
//        }

//        public class RollEventArgs : EventArgs
//        {

//        }
//        public delegate void RollDelegate(Roll roll, RollEventArgs e);

//        public event RollDelegate rollFinishedEvent;

//    }
//}
