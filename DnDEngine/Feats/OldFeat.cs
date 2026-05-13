using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public enum FeatTriggerType { Attack, Damage, GetHit }

    public struct FeatTriggerDetails
    {

    }

    public abstract class FeatCreator
    {
        public Type TypeFeat()
        {
            return CreateFeat().GetType();
        }

        public abstract OldFeat CreateFeat();
    }

    public class OldFeat : IValueModification
    {
        public static DisplayOptionsTextureSet optionsDisplayTextures;

        public Creature creature;

        public string name = "";
        public string description;

        public DisplayOptionsDnD DisplayOptionsDnD(DisplayOptions dO)
        {
            return dO as DisplayOptionsDnD;
        }

        #region Display Options
        public DisplayOptionsDnD displayOptions;
        public string optionsDisplayDescription;

        public virtual DisplayOptionsDnD MakeOptionsDisplay(Roll roll, List<Roll> rolls = null, Creature creature = null)
        {
            displayOptions = new DisplayOptionsDnD(optionsDisplayDescription, optionsDisplayTextures, roll, rolls, true, false);
            displayOptions.creature = creature;
            displayOptions.yesOptionEvent += new DisplayOptions.DisplayOptionsDelegate(OptionsDisplayYes);
            displayOptions.noOptionEvent += new DisplayOptions.DisplayOptionsDelegate(OptionsDisplayNo);
            return displayOptions;
        }

        public virtual void OptionsDisplayYes(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
        {
        }

        public virtual void OptionsDisplayNo(DisplayOptions dO, DisplayOptions.DisplayOptionsEventArgs e)
        {

        }

        #endregion

        #region Subscriptions

        public bool takeDamage = false;

        List<Creature> preRollSubscribed = new List<Creature>();
        List<Creature> postRollSubscribed = new List<Creature>();
        List<Creature> finishRollSubscribed = new List<Creature>();
        List<Creature> statChangeSubscribed = new List<Creature>();
        List<Creature> statOverrideSubscribed = new List<Creature>();
        List<Creature> takeDamageSubscribed = new List<Creature>();
        List<Creature> shortRestSubscribed = new List<Creature>();
        List<Creature> longRestSubscribed = new List<Creature>();
        List<Creature> startTurnSubscribed = new List<Creature>();
        List<Creature> endTurnSubscribed = new List<Creature>();

        public void SubscribePreRoll    (Creature creature, bool unsubscribe = false)
        {
            if (preRollSubscribed.Contains(creature))
            {
                creature.preRollCheck -= FeatPreRollCheck;
                preRollSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.preRollCheck += FeatPreRollCheck;
                preRollSubscribed.Add(creature);
            }
        }
        public void SubscribePostRoll   (Creature creature, bool unsubscribe = false)
        {
            if (postRollSubscribed.Contains(creature))
            {
                creature.postRollCheck -= FeatPostRollCheck;
                postRollSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.postRollCheck += FeatPostRollCheck;
                postRollSubscribed.Add(creature);
            }
        }
        public void SubscribeFinishRoll (Creature creature, bool unsubscribe = false)
        {
            if (finishRollSubscribed.Contains(creature))
            {
                creature.finishRollCheck -= FeatFinishRollCheck;
                finishRollSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.finishRollCheck += FeatFinishRollCheck;
                finishRollSubscribed.Add(creature);
            }
        }
        public void SubscribeStatChange (Creature creature, bool unsubscribe = false)
        {
            if (statChangeSubscribed.Contains(creature))
            {
                creature.changeStatsDelegate -= StatChange;
                statChangeSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.changeStatsDelegate += StatChange;
                statChangeSubscribed.Add(creature);
            }
        }
        public void SubscribeStatOverride(Creature creature, bool unsubscribe = false)
        {
            if (statOverrideSubscribed.Contains(creature))
            {
                creature.overrideStatsDelegate -= StatOverride;
                statOverrideSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.overrideStatsDelegate += StatOverride;
                statOverrideSubscribed.Add(creature);
            }
        }
        public void SubscribeTakeDamage (Creature creature, bool unsubscribe = false)
        {
            if (takeDamageSubscribed.Contains(creature))
            {
                creature.takeDamageEvent -= FeatActivated;
                takeDamageSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.takeDamageEvent += FeatActivated;
                takeDamageSubscribed.Add(creature);
            }
        }
        public void SubscribeShortRest  (Creature creature, bool unsubscribe = false)
        {
            if (shortRestSubscribed.Contains(creature))
            {
                creature.shortRestEvent -= ShortRest;
                shortRestSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.shortRestEvent += ShortRest;
                shortRestSubscribed.Add(creature);
            }
        }
        public void SubscribeLongRest   (Creature creature, bool unsubscribe = false)
        {
            if (longRestSubscribed.Contains(creature))
            {
                creature.longRestEvent -= LongRest;
                longRestSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.longRestEvent += LongRest;
                longRestSubscribed.Add(creature);
            }
        }
        public void SubscribeStartTurn(Creature creature, bool unsubscribe = false)
        {
            if (startTurnSubscribed.Contains(creature))
            {
                creature.startTurnEvent -= FeatActivated;
                startTurnSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.startTurnEvent += FeatActivated;
                startTurnSubscribed.Add(creature);
            }
        }
        public void SubscribeEndTurn(Creature creature, bool unsubscribe = false)
        {
            if (endTurnSubscribed.Contains(creature))
            {
                creature.endTurnEvent -= FeatActivated;
                endTurnSubscribed.Remove(creature);
            }
            else if (!unsubscribe)
            {
                creature.endTurnEvent += FeatActivated;
                endTurnSubscribed.Add(creature);
            }
        }


        #endregion

        #region Adding and removing

        public virtual void AddFeat()
        {
            if (preRoll)
            {
                SubscribePreRoll(creature);
            }
            if (postRoll)
            {
                SubscribePostRoll(creature);
            }
            if (finishRoll)
            {
                SubscribeFinishRoll(creature);
            }
            if (statChange)
            {
                SubscribeStatChange(creature);
            }
            if (statOverride)
            {
                SubscribeStatOverride(creature);
            }
            if (perShortRest)
            {
                SubscribeShortRest(creature);
            }
            if (perLongRest)
            {
                SubscribeLongRest(creature);
            }
            if (startTurn)
            {
                SubscribeStartTurn(creature);
            }
            if (endTurn)
            {
                SubscribeEndTurn(creature);
            }
            if (takeDamage)
            {
                SubscribeTakeDamage(creature);
            }

            // Add all valueChange and valueoverride modifications
            foreach (string valueType in valueChanges.Keys)
            {
                creature.Values.GetValue<int>(valueType).AddModifier(this);
            }
            foreach (string valueType in valueOverrides.Keys)
            {
                creature.Values.GetValue<int>(valueType).AddModifier(this);
            }

            ResetUses();
        }

        public virtual void RemoveFeat()
        {
            SubscribePreRoll(creature, true);
            SubscribePostRoll   (creature, true);
            SubscribeFinishRoll(creature, true);
            SubscribeStatChange(creature, true);
            SubscribeStatOverride(creature, true);
            SubscribeTakeDamage(creature, true);
            SubscribeShortRest (creature, true);
            SubscribeLongRest(creature, true);
            SubscribeStartTurn(creature, true);
            SubscribeEndTurn(creature, true);
        }

        public virtual void RemoveFromCreature()
        {
            creature.oldFeats.Remove(this);
            RemoveFeat();
        }

        #endregion

        #region Old Stat Editing
        public bool statChange = false;
        public bool statOverride = false;

        public virtual void StatChange(Creature cr)
        {

        }

        public virtual void StatOverride(Creature cr)
        {

        }

        #endregion

        # region Value Editing

        public Dictionary<string, int> valueChanges = new Dictionary<string, int>();
        public Dictionary<string, int> valueOverrides = new Dictionary<string, int>();

        public T GetModification<T>(Value<T> value, T currentValue)
        {
            if (valueChanges.ContainsKey(value.ValueType))
            {
                currentValue = (dynamic)currentValue + valueChanges[value.ValueType];
            }
            if (valueOverrides.ContainsKey(value.ValueType))
            {
                currentValue = (dynamic)valueOverrides[value.ValueType];
            }
            return currentValue;
        }

        # endregion

        #region Using

        public bool action = false;
        public bool bonusAction = false;
        public bool reaction;
        public bool freeAction = false;

        public bool perShortRest = false;
        public bool perLongRest = false;
        public bool usedThisRest = false;

        public int MaxUses { get { return GetMaxUses(); } }
        public int uses = 0;

        public virtual void UseFeat()
        {
            if (action)
            {
                creature.actionTaken = true;
            }
            if (bonusAction)
            {
                creature.bonusActionTaken = true;
            }
            if (MaxUses != 0)
            {
                uses++;
                if (uses >= MaxUses)
                {
                    usedThisRest = true;
                }
            }
        }

        public virtual void ResetUses()
        {
            uses = 0;
            usedThisRest = false;
        }

        public virtual bool IsUsable()
        {
            if (usedThisRest)
            {
                return false;
            }
            if (action && !creature.actionTaken)
            {
                return true;
            }
            if (bonusAction && !creature.bonusActionTaken)
            {
                return true;
            }
            if (freeAction)
            {
                return true;
            }
            return false;
        }

        public virtual int GetMaxUses()
        {
            return 0;
        }

        #endregion

        #region Choices

        public List<string> featChoices;
        public bool choiceMade = true;

        public virtual bool HasChoices()
        {
            if (!choiceMade && featChoices.Count>0)
            {
                return true;
            }
            return false;
        }

        public virtual void FeatChoice(string choice)
        {

        }

        public virtual ContextMenuTemplate FeatChoiceChildMenu(string choice)
        {
            return null;
        }

        public List<string> GetChoiceChildMenuTags(List<string> tags)
        {
            List<string> returnTags = new List<string>() {CreatureMenuOtherOptions.FeatChoices.ToString(), creature.oldFeats.IndexOf(this).ToString()};
            returnTags.AddRange(tags);
            return returnTags;
        }

        #endregion

        #region Roll Checks

        public bool preRoll = false;
        public bool postRoll = false;
        public bool finishRoll = false;

        public virtual void FeatPreRollCheck(Roll roll, RollEventArgs e)
        {
            FeatPreRollFinished(roll);
        }

        public virtual void FeatPostRollCheck(Roll roll, RollEventArgs e)
        {
            FeatPostRollFinished(roll);
        }

        public virtual void FeatPreRollFinished(Roll roll)
        {
            creature.PreRollCheckDone(this, roll);
        }

        public virtual void FeatPostRollFinished(Roll roll)
        {
            creature.postRollCheckDone(this, roll);
        }

        public virtual void FeatFinishRollCheck(Roll roll, RollEventArgs e)
        {

        }

        public virtual void FeatActivated(Creature creature, Creature.CreatureDelegateEventArgs e)
        {

        }

        #endregion

        #region Turn Timers

        public bool startTurn = false;
        public bool endTurn = false;

        public virtual void TurnReset()
        {

        }

        public virtual void StartTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
        {

        }

        public virtual void EndTurn(Creature cr, Creature.CreatureDelegateEventArgs e)
        {

        }

        public virtual void ShortRest(Creature cr, Creature.CreatureDelegateEventArgs e)
        {
            if (perShortRest)
            {
                ResetUses();
            }
        }

        public virtual void LongRest(Creature cr, Creature.CreatureDelegateEventArgs e)
        {
            if (perLongRest)
            {
                ResetUses();
            }
        }

        #endregion

        #region Child Menus

        public virtual ContextMenuTemplate ChildMenu()
        {
            return null;
        }

        public ContextMenuTemplate BaseChildMenuTemplate()
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(creature.encounter.contextMenuTextures.blankTileTextures);
            template.font = creature.encounter.contextMenuTextures.baseFont;
            template.texts = new List<string>();
            template.tags = new List<List<string>>();
            template.childMenus = new List<ContextMenuTemplate>();
            template.inactives = new List<bool>();

            return template;
        }

        public List<string> MakeTags(List<string> tags)
        {
            int index = 0;
            for (int i = 0; i <creature.oldFeats.Count; i++)
            {
                if (creature.oldFeats[i] == this)
                {
                    index = i;
                }
            }
            List<string> returnTags = new List<string>() { CreatureMenuOtherOptions.UseFeatChildMenu.ToString(), index.ToString() };
            returnTags.AddRange(tags);

            return returnTags;
        }

        public virtual void UseFeatChildMenu(List<string> tags)
        {
            if (!targeting)
            {
                if (action)
                {
                    creature.actionTaken = true;
                }
                if (bonusAction)
                {
                    creature.bonusActionTaken = true;
                }
                if (MaxUses != 0)
                {
                    uses++;
                    if (uses >= MaxUses)
                    {
                        usedThisRest = true;
                    }
                }
            }
        }

        #endregion

        #region Selecting

        public bool targeting = false;
        public int range;
        public Color? targetingColor = null;
        public enum TargetType { SingleTarget, Sphere, Cone, Line }
        public TargetType targetType = TargetType.SingleTarget;
        public Order currentControl;

        public virtual void SelectionMadeOrder(Order order)
        {
            currentControl = order;
            if (action)
            {
                creature.actionTaken = true;
            }
            if (bonusAction)
            {
                creature.bonusActionTaken = true;
            }
            if (MaxUses != 0)
            {
                uses++;
                if (uses >= MaxUses)
                {
                    usedThisRest = true;
                }
            }
            UseFeat();
        }

        #endregion
    }
}