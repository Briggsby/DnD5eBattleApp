namespace DnD5eBattleApp
{ 
    public abstract class OldConditionCreator : OldFeatCreator
    {
        public virtual OldCondition CreateCondition(Creature creature = null)
        {
            OldCondition condition = CreateFeat() as OldCondition;
            condition.creature = creature;
            return condition;
        }
    }

    public class OldCondition : OldFeat
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
                this.RemoveFeat();
            }
        }

        public virtual void RemoveCondition(Creature creature = null)
        {
            RemoveFeat();
        }

        public virtual void AddCondition(Creature creature = null)
        {
            if (creature is not null) {this.creature = creature;}
            AddFeat();
        }


        public override void RemoveFromCreature()
        {
            creature.feats.Remove(this);
            RemoveFeat();
        }
    }
}