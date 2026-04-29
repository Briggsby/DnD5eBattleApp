using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BugsbyEngine;


namespace DnD5eBattleApp
{ 
    public class StatDisplay : GameObject
    {
        public enum StatsDisplay { Picture, MaxHP, HPOutOfMax, Name, AmountMoved, StatStr, StatDex, StatCon, StatInt, StatWis, StatCha }
        public static Dictionary<StatsDisplay, Vector2> statLocations;

        public List<TextObject> statsDisplayed;

        public static Texture2D texture;

        static public Vector2 location;

        static public Vector2 size;
        static public SpriteFont font;
        static public Vector2 textSize;
        static public Vector2 pictureSize;
        static public Color textColor;

        public Creature creature;

        public StatDisplay(Creature creature) : base(location, texture)
        {
            this.creature = creature;
        }

        public static void SetInfo(Vector2? location = null, Texture2D texture = null, Dictionary<StatsDisplay, Vector2> statLocations = null, SpriteFont font = null, Vector2? textSize = null, Vector2? pictureSize = null, Color? color = null)
        {
            if (location != null)
            {
                StatDisplay.location = location??default(Vector2);
            }

            if (texture != null)
            {
                StatDisplay.texture = texture;
            }

            if (statLocations != null)
            {
                StatDisplay.statLocations = statLocations;
            }

            if (font != null)
            {
                StatDisplay.font = font;
            }
            
            if (textSize != null)
            {
                StatDisplay.textSize = textSize ?? default(Vector2);
            }

            if (pictureSize != null)
            {
                StatDisplay.pictureSize = pictureSize ?? default(Vector2);
            }

            if (color != null)
            {
                StatDisplay.textColor = color ?? default(Color);
            }
        }

        public override void Update()
        {
            UpdateStats();
            base.Update();
        }

        public void UpdateStats()
        {
            statsDisplayed = new List<TextObject>();

            foreach (StatsDisplay sD in statLocations.Keys)
            {
                if (sD != StatsDisplay.Picture)
                {
                    statsDisplayed.Add(new TextObject(this, DisplayStat(sD), font, statLocations[sD]));
                    statsDisplayed[statsDisplayed.Count - 1].transform.scale = textSize;
                    statsDisplayed[statsDisplayed.Count - 1].transform.color = textColor;

                }
            }

            textObjects = new List<TextObject>(statsDisplayed);
        }

        public string DisplayStat(StatsDisplay sD)
        {
            switch (sD)
            {
                case (StatsDisplay.MaxHP):
                    return creature.hitPointMax.ToString();
                case (StatsDisplay.HPOutOfMax):
                    return string.Format("{0} / {1}", creature.hitPoints, creature.hitPointMax);
                case (StatsDisplay.Name):
                    return creature.name;
                case (StatsDisplay.AmountMoved):
                    return creature.amountMoved.ToString();
                case (StatsDisplay.StatStr):
                    return string.Format("STR: {0} (+{1})", creature.stats[Stats.Strength], creature.StatMod(Stats.Strength));
                case (StatsDisplay.StatDex):
                    return string.Format("DEX: {0} (+{1})", creature.stats[Stats.Dexterity], creature.StatMod(Stats.Dexterity));
                case (StatsDisplay.StatCon):
                    return string.Format("CON: {0} (+{1})", creature.stats[Stats.Constitution], creature.StatMod(Stats.Constitution));
                case (StatsDisplay.StatInt):
                    return string.Format("INT: {0} (+{1})", creature.stats[Stats.Intelligence], creature.StatMod(Stats.Intelligence));
                case (StatsDisplay.StatWis):
                    return string.Format("WIS: {0} (+{1})", creature.stats[Stats.Wisdom], creature.StatMod(Stats.Wisdom));
                case (StatsDisplay.StatCha):
                    return string.Format("CHA: {0} (+{1})", creature.stats[Stats.Charisma], creature.StatMod(Stats.Charisma));

            }

            return "N/A";
        }
    }
}