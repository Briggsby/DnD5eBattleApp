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
    public enum MiscRollTags { Poison, Disease };

    public class Roll
    {
        public Encounter encounter;
        public List<string> rollTags;

        public int Natural { get { return score - bonus; } }
        public int score;
        public int DC;

        public int die;
        public int bonus;
        public virtual int Bonus {
            get { return bonus; }
            set
            {

                bonus += value; score += value;
            }
        }

        public bool advantage;
        public bool disadvantage;
        public int otherScore;

        public Creature roller;

        public Object source;

        public Roll(Encounter encounter, List<string> rollTags = null)
        {
            this.encounter = encounter;
            DnDManager.ongoingRolls.Add(this);
            this.rollTags = rollTags ?? new List<string>();
        }

        public bool autoSuccess = false;
        public bool autoFail = false;
        public bool Success
        {
            get { if (autoSuccess) { return true; } else if (autoFail) { return false; } else { return (score >= DC); } }
        }

        public int RollD(int d)
        {
            return EngManager.random.Next(1, d + 1);
        }

        public virtual void DoRoll()
        {
            PreRoll();
        }

        public virtual int MakeRoll()
        {
            if (advantage || disadvantage && !(advantage && disadvantage))
            {
                int roll1 = RollD(die);
                int roll2 = RollD(die);
                if (advantage)
                {
                    otherScore = Math.Min(roll1, roll2) + bonus;
                    return score = Math.Max(roll1, roll2) + bonus;
                }
                else if (disadvantage)
                {
                    otherScore = Math.Min(roll2, roll1) + bonus;
                    return score = Math.Min(RollD(die), RollD(die)) + bonus;
                }
            }
            return score = RollD(die) + bonus;        
        }

        public bool preRollDone;
        List<Creature> preRollCreatures;
        List<Object> preRollObjects;
        public int preRollObjectNumber;

        public virtual void PreRoll()
        {
            preRollDone = false;
            preRollCreatures = new List<Creature>(encounter.creatures);
            preRollObjects = new List<object>();

            if (preRoll == null)
            {
                preRollObjectNumber = 0;
            }
            else
            {
                preRollObjectNumber = preRoll.GetInvocationList().GetLength(0);
                preRoll?.Invoke(this, null);
            }

            List<Creature> preRollCreaturesOriginalList = new List<Creature>(encounter.creatures);
            foreach (Creature cr in preRollCreaturesOriginalList)
            {
                cr.PreRollCheck(this, null);
            }


        }
        public event RollDelegate preRoll;
        public virtual void PreRollCheckDone(Creature creature)
        {
            preRollCreatures.Remove(creature);
            if (preRollCreatures.Count < 1 && preRollObjects.Count >= preRollObjectNumber)
            {
                MakeRoll();
                PostRoll();
            }
        }

        public bool reroll = false;
        public bool rerolled = false;

        public bool postRollDone;
        List<Creature> postRollCreatures;
        List<Object> postRollObjects;
        int postRollObjectNumber;
        public virtual void PostRoll()
        {
            postRollDone = false;
            postRollCreatures = new List<Creature>(encounter.creatures);
            postRollObjects = new List<object>();

            if (postRoll == null)
            {
                postRollObjectNumber = 0;
            }
            else
            {
                postRollObjectNumber = postRoll.GetInvocationList().GetLength(0);
                postRoll?.Invoke(this, null);
            }

            List<Creature> postRollCreaturesOriginalList = new List<Creature>(encounter.creatures);
            foreach (Creature encCr in postRollCreaturesOriginalList)
            {
                encCr.PostRollCheck(this, null);
            }
        }
        public event RollDelegate postRoll;
        public virtual void PostRollCheckDone(Creature creature = null)
        {
            if (creature != null)
            {
                postRollCreatures.Remove(creature);
            }
            if (postRollCreatures.Count < 1 && postRollObjects.Count >= postRollObjectNumber)
            {
                if (reroll)
                {
                    reroll = false;
                    MakeRoll();
                    rerolled = true;
                    PostRoll();
                }
                FinishRoll();
            }
        }


        public bool finishedRoll = false;
        List<Creature> finishRollCreatures;
        public virtual void FinishRoll()
        {
            DnDManager.ongoingRolls.Remove(this);
            finishedRoll = true;
            finishRollCreatures = new List<Creature>(encounter.creatures);

            List<Creature> finishRollCreaturesOriginalList = new List<Creature>(encounter.creatures);
            foreach (Creature encCr in finishRollCreaturesOriginalList)
            {
                encCr.FinishRollCheck(this, null);
            }
            finishRoll?.Invoke(this, null);
            endRoll?.Invoke(this, null);
        }
        public event RollDelegate finishRoll;

        public bool cancelledRoll = false;
        public virtual void CancelRoll()
        {
            cancelledRoll = true;
            finishedRoll = true;
            DnDManager.ongoingRolls.Remove(this);
            endRoll?.Invoke(this, null);
        }
        public event RollDelegate endRoll;


        public void GetSource(Attack attack)
        {
            if (attack.isSpell)
            {
                source = attack.spell;
            }
            else
            {
                source = attack.attackerWeapon;
            }
        }

        public Creature GetSourceCreature()
        {
            if (source is Spell)
            {
                return (source as Spell).caster;
            }
            else if (source is OldFeat)
            {
                return (source as OldFeat).creature;
            }
            else if (source is Item)
            {
                return (source as Item).inventory.creature;
            }
            else
            {
                return null;
            }
        }

        public string WithAdvantagePrint()
        {
            if (advantage && disadvantage)
            {
                return "with advantages cancelled out";
            }
            else if (advantage)
            {
                return string.Format("with advantage ({0})", otherScore);
            }
            else if (disadvantage)
            {
                return string.Format("with disadvantage ({0})", otherScore);
            }
            else
            {
                return "";
            }
        }
    }

    public class RollEventArgs : EventArgs
    {
    }

    public delegate void RollDelegate(Roll roll, RollEventArgs e);
}