using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public enum CharacterCreationMenuComponents { Name, Species, SubSpecies, Class, Subclass, Stats, Level, }

    public class CharacterCreationMenu : GameObject
    {
        public static Vector2 menuPosition;
        public static Texture2D texture;
        public static ButtonTextures tabButtonTexture;
        public static List<Vector2> tabLocations;
        public static Vector2 tabSize;


        public List<string> tabs = new List<string>() { "Name, Stats, and Level", "Species", "Class", "Background", "Skills", "Spells" };
        public List<Button> tabButtons;

        public CharacterCreationMenu() : base(menuPosition, texture)
        {
            tabButtons = new List<Button>();
            for (int i = 0; i < tabs.Count; i++)
            {
                Button button = new Button(tabLocations[i], tabs[i], null, tabButtonTexture, tabSize, this);
                button.OnButtonActivationEvent += new Button.ButtonDelegate(TabButtonPress);
                tabButtons.Add(button);
            }
        }

        public void TabButtonPress(Button button, Button.ButtonEventArgs e)
        {
            foreach (Button b in tabButtons)
            {
                if (b != button)
                {
                    b.SetDeActivated();
                }
            }
        }
    }
}