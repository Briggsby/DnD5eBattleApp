namespace DnD5eBattleApp
{ 
    public abstract class ConditionCreator : FeatCreator
    {
        public virtual Condition CreateCondition(Creature creature = null)
        {
            Condition condition = CreateFeat() as Condition;
            condition.creature = creature;
            return condition;
        }
    }

    public abstract class Condition : Feat
    {
        public string source;
        public int duration = 0;
        public int timeActive = 0;

        public override void TurnReset()
        {
            base.TurnReset();
            timeActive++;
            if (duration > 0 && timeActive > duration)
            {
                creature.RemoveFeat(this);
            }
        }

        public virtual void RemoveCondition(Creature creature = null)
        {
            RemoveFeat();
        }

        public virtual void AddCondition()
        {
            AddFeat();
        }


        public override void RemoveFromCreature()
        {
            creature.feats.Remove(this);
            RemoveFeat();
        }
    }
}