using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public enum SpellCasterType { Full, Half, Third, Warlock, None };

    public class SpellSlots
    {
        public Dictionary<int, int> spellSlotsMax;
        public Dictionary<int, int> spellSlotsCurrent;

        public SpellSlots(int spellcasterLevel, SpellCasterType type)
        {
            spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };

            if (type == SpellCasterType.Full)
            {
                switch (spellcasterLevel)
                {
                    case (1):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 2 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (2):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 3 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (3):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 2 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (4):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (5):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 2 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (6):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (7):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 1 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (8):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 2 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (9):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 1 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (10):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 2 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (11):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 2 }, { 6, 1 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (12):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 2 }, { 6, 1 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (13):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 2 }, { 6, 1 }, { 7, 1 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (14):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 2 }, { 6, 1 }, { 7, 1 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (15):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 2 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 0 } };
                        break;
                    case (16):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 2 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 0 } };
                        break;
                    case (17):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 2 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 } };
                        break;
                    case (18):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 } };
                        break;
                    case (19):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 2 }, { 7, 1 }, { 8, 1 }, { 9, 1 } };
                        break;
                    case (20):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 4 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 2 }, { 7, 2 }, { 8, 1 }, { 9, 1 } };
                        break;
                }
            }
            else if (type == SpellCasterType.Half)
            {

            }
            else if (type == SpellCasterType.Third)
            {

            }
            else if (type == SpellCasterType.Warlock)
            {
                switch (spellcasterLevel)
                {
                    case (1):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 1 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (2):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 2 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (3):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 2 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (4):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 2 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (5):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 2 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (6):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 2 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (7):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 2 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (8):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 2 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (9):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 2 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (10):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 2 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (11):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 3 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;                                                        
                    case (12):                                                        
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 3 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;                                                        
                    case (13):                                                        
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 3 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;                                                        
                    case (14):                                                        
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 3 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;                                                        
                    case (15):                                                        
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 3 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;                                                        
                    case (16):                                                        
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 3 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;                                                        
                    case (17):                                                        
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 4 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;                                                        
                    case (18):                                                        
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 4 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;                                                        
                    case (19):                                                        
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 4 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                    case (20):
                        spellSlotsMax = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 4 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
                        break;
                }
            }                                                                                           

            spellSlotsCurrent = new Dictionary<int, int>(spellSlotsMax);
        }                                                                                               

        public SpellSlots(Dictionary<int, int> spellSlotsMax, Dictionary<int,int> spellSlotsCurrent)
        {
            spellSlotsMax = new Dictionary<int, int>(spellSlotsMax);
            spellSlotsCurrent = new Dictionary<int, int>(spellSlotsCurrent);
        }

        public void ResetSpellSlots()
        {
            spellSlotsCurrent = new Dictionary<int, int>(spellSlotsMax);
        }

        public SpellSlots CopySpellSlots()
        {
            return new SpellSlots(spellSlotsMax, spellSlotsCurrent);
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

        public void SpellcastingFeat(int level, SpellCasterType type)
        {
            switch(type)
            {
                case (SpellCasterType.Full):
                    switch (level)
                    {
                        case (1):
                            spellSlotsMax[1] += 2;
                            break;
                        case (2):
                            spellSlotsMax[1] += 3;
                            break;
                        case (3):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 2;
                            break;
                        case (4):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            break;
                        case (5):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 2;
                            break;
                        case (6):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            break;
                        case (7):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 1;
                            break;
                        case (8):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 2;
                            break;
                        case (9):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 1;
                            break;
                        case (10):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 2;
                            break;
                        case (11):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 2;
                            spellSlotsMax[6] += 1;
                            break;
                        case (12):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 2;
                            spellSlotsMax[6] += 1;
                            break;
                        case (13):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 2;
                            spellSlotsMax[6] += 1;
                            spellSlotsMax[7] += 1;
                            break;
                        case (14):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 2;
                            spellSlotsMax[6] += 1;
                            spellSlotsMax[7] += 1;
                            break;
                        case (15):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 2;
                            spellSlotsMax[6] += 1;
                            spellSlotsMax[7] += 1;
                            spellSlotsMax[8] += 1;
                            break;
                        case (16):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 2;
                            spellSlotsMax[6] += 1;
                            spellSlotsMax[7] += 1;
                            spellSlotsMax[8] += 1;
                            break;
                        case (17):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 2;
                            spellSlotsMax[6] += 1;
                            spellSlotsMax[7] += 1;
                            spellSlotsMax[8] += 1;
                            spellSlotsMax[9] += 1;
                            break;
                        case (18):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 3;
                            spellSlotsMax[6] += 1;
                            spellSlotsMax[7] += 1;
                            spellSlotsMax[8] += 1;
                            spellSlotsMax[9] += 1;
                            break;
                        case (19):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 3;
                            spellSlotsMax[6] += 2;
                            spellSlotsMax[7] += 1;
                            spellSlotsMax[8] += 1;
                            spellSlotsMax[9] += 1;
                            break;
                        case (20):
                            spellSlotsMax[1] += 4;
                            spellSlotsMax[2] += 3;
                            spellSlotsMax[3] += 3;
                            spellSlotsMax[4] += 3;
                            spellSlotsMax[5] += 3;
                            spellSlotsMax[6] += 2;
                            spellSlotsMax[7] += 2;
                            spellSlotsMax[8] += 1;
                            spellSlotsMax[9] += 1;
                            break;
                    }
                    break;
            }
        }
    }
}