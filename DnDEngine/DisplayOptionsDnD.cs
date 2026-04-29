using System.Collections.Generic;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class DisplayOptionsDnD : DisplayOptions
    {
        public Roll roll;
        public List<Roll> rolls;
        public Creature creature;
        public List<Creature> creatures;

        public DisplayOptionsDnD(string description, DisplayOptionsTextureSet textureSet, Roll roll = null, List<Roll> rolls = null, bool yesAndNo = true, bool haveCancelButton = true, bool haveScrollOptions = false, List<string> availableOptions = null, bool haveAdditionalScrollOptions = false, List<List<string>> additionalAvailableOptions = null) : base(description, textureSet, yesAndNo, haveCancelButton, haveScrollOptions, availableOptions, haveAdditionalScrollOptions, additionalAvailableOptions)
        {
            this.roll = roll;
            this.rolls = rolls??new List<Roll>();
        }
    }
}