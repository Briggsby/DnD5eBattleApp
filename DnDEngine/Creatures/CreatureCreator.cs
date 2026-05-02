namespace DnD5eBattleApp
{ 
    public abstract class CreatureCreator
    {
        public abstract Creature CreateCreature();

        public virtual Creature CreateCreature(BoardTile tile, bool rollHP = true)
        {
            Creature creature = CreateCreature();
            creature.SetInitialTile(tile);
            creature.baseStats.RollHP();
            creature.ResetHP();
            creature.RecalibrateStats();
            creature.StartTurn();
            return creature;
        }
    }

    public abstract class MonsterCreator : CreatureCreator
    {
        public virtual Monster CreateMonster()
        {
            return CreateCreature() as Monster;
        }
        public virtual Monster CreateMonster(BoardTile tile)
        {
            return CreateCreature(tile) as Monster;
        }
    }
}