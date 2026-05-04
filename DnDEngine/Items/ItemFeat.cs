namespace DnD5eBattleApp
{
    public abstract class ItemFeat : Feat
    {
        protected ItemFeat(Creature owner, FeatSpec spec) : base(owner, spec)
        {
        }
    }
}