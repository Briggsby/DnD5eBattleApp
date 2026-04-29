using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public class SubClass
    {
        public string parentClass;
        public string name;
        public Dictionary<int, List<string>> feats;

        public virtual void AssignToCharacter(Creature creature, int level)
        {
            //Debug.WriteLine(string.Format("Assigning {0} subclass", name));
            for (int l = 0; l <= level; l++)
            {
                if (feats.ContainsKey(l))
                {
                    foreach (string fc in feats[l])
                    {
                        creature.AddFeat(DnDManager.feats[fc].CreateFeat());
                    }
                }
            }
        }
    }
}