using System.Collections.Generic;

namespace DnD5eBattleApp;

public enum SpellCasterType { Full, Half, Third, Warlock, None };

public class SpellSlots
{
    public Dictionary<int, int> spellSlotsMax;
    public Dictionary<int, int> spellSlotsCurrent;

    public static Dictionary<int, int[]> fullCasterSpellSlotsMax = new Dictionary<int, int[]>() {
        { 1, new int[]  { 2, 0, 0, 0, 0, 0, 0, 0, 0 } },
        { 2, new int[]  { 3, 0, 0, 0, 0, 0, 0, 0, 0 } },
        { 3, new int[]  { 4, 2, 0, 0, 0, 0, 0, 0, 0 } },
        { 4, new int[]  { 4, 3, 0, 0, 0, 0, 0, 0, 0 } },
        { 5, new int[]  { 4, 3, 2, 0, 0, 0, 0, 0, 0 } },
        { 6, new int[]  { 4, 3, 3, 0, 0, 0, 0, 0, 0 } },
        { 7, new int[]  { 4, 3, 3, 1, 0, 0, 0, 0, 0 } },
        { 8, new int[]  { 4, 3, 3, 2, 0, 0, 0, 0, 0 } },
        { 9, new int[]  { 4, 3, 3, 3, 1, 0, 0, 0, 0 } },
        { 10, new int[] { 4, 3, 3, 3, 2, 0, 0, 0, 0 } },
        { 11, new int[] { 4, 3, 3, 3, 2, 1, 0, 0, 0 } },
        { 12, new int[] { 4, 3, 3, 3, 2, 1, 0, 0, 0 } },
        { 13, new int[] { 4, 3, 3, 3, 2, 1, 1, 0, 0 } },
        { 14, new int[] { 4, 3, 3, 3, 2, 1, 1, 0, 0 } },
        { 15, new int[] { 4, 3, 3, 3, 2, 1, 1, 1, 0 } },
        { 16, new int[] { 4, 3, 3, 3, 2, 1, 1, 1, 0 } },
        { 17, new int[] { 4, 3, 3, 3, 2, 1, 1, 1, 1 } },
        { 18, new int[] { 4, 3, 3, 3, 3, 1, 1, 1, 1 } },
        { 19, new int[] { 4, 3, 3, 3, 3, 2, 2, 1, 1 } },
        { 20, new int[] { 4, 3, 3, 3, 3, 2, 2, 1, 1 } },
    };

    public SpellSlots(int spellcasterLevel, SpellCasterType spellcasterType)
    {
        int effectiveLevel = spellcasterLevel;
        if (spellcasterType == SpellCasterType.Full) {
            effectiveLevel = spellcasterLevel;
        } else if (spellcasterType == SpellCasterType.Half) {
            effectiveLevel = spellcasterLevel / 2;
        } else if (spellcasterType == SpellCasterType.Third) {
            effectiveLevel = spellcasterLevel / 3;
        }
        Dictionary<int, int> spellSlotsMax = new Dictionary<int, int>();
        for (int level = 1; level <= 9; level++) {
                spellSlotsMax[level] = fullCasterSpellSlotsMax[effectiveLevel][level - 1];
        }
        spellSlotsCurrent = new Dictionary<int, int>(spellSlotsMax);
    }    

    public void ResetSpellSlots()
    {
        spellSlotsCurrent = new Dictionary<int, int>(spellSlotsMax);
    }

    public void BlankSpellSlotsMax()
    {
        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
    }

    public bool HasSpellSlotOfLevel(int level)
    {
        if (spellSlotsMax[level] > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
