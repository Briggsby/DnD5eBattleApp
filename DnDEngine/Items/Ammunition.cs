namespace DnD5eBattleApp
{
    public abstract class Ammunition : Item
    {

    }

    public class Arrow : Ammunition
    {
        public Arrow() : base()
        {
            name = "Arrow";
        }
    }

    public class Bolt : Ammunition
    {
        public Bolt() : base()
        {
            name = "Bolt";
        }
    }
}