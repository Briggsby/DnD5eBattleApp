using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class EncounterControlsContextMenu : ContextMenu
    {
        BoardTile tile;

        public EncounterControlsContextMenu(BoardTile tile, ContextMenuTemplate template) : base(template, new Vector2(0, 0), tile)
        {
            this.tile = tile;
            destroyOnOtherClick = true;

        }

        public EncounterControlsContextMenu(BoardTile tile, ContextMenuTemplate template1, ContextMenuTemplate template2) : base(template1, template2, new Vector2(0,0), tile)
        {
            this.tile = tile;
            destroyOnOtherClick = true;

        }

        public EncounterControlsContextMenu(Vector2 position, ContextMenuTemplate template) : base(template, position)
        {
            destroyOnOtherClick = true;
        }

        public override void OnSetDestroy()
        {
            base.OnSetDestroy();
            if (tile != null)
            {
                tile.board.encounter.contextMenu = null;
            }
            DnDManager.contextMenu = null;
        }

        public override void ButtonPress(List<string> tags, ContextMenuItem item)
        {
            base.ButtonPress(tags, item);

            if (tags.Contains("SpawnMonster"))
            {
                tile.board.encounter.SpawnMonster(DnDManager.monsters[tags[1]], tile);
            }

            if (tags.Contains(Encounter.EncounterControls.EndTurn.ToString()))
            {
                tile.board.encounter.EndTurn();
            }

            if (tags.Contains("NewEncounter"))
            {
                DnDManager.NewEncounter();
            }

            if (tags.Contains(Encounter.EncounterControls.SpawnCharacter.ToString()))
            {

            }

            if (tags.Contains(Encounter.EncounterControls.DestroyEncounter.ToString()))
            {
                tile.board.encounter.DestroyEncounter();
            }

            if (tags.Contains(Encounter.EncounterControls.SpawnQuickCharacter.ToString()))
            {
                tile.board.encounter.SpawnQuickCharacter(tags.GetRange(1,5), tile);
            }

            if (tags.Contains(Encounter.EncounterControls.ShortRest.ToString()))
            {
                tile.board.encounter.ShortRest();
            }

            if (tags.Contains(Encounter.EncounterControls.LongRest.ToString()))
            {
                tile.board.encounter.LongRest();
            }

            if (!tags.Contains(ContextMenu.DefaultTags.ParentMenu.ToString()) && !tags.Contains(ContextMenu.DefaultTags.NotButton.ToString()))
            {
                if (tile != null)
                {
                    tile.board.encounter.contextMenu = null;
                }
                DnDManager.contextMenu = null;
                DestroyAndChildren();
            }

        }

    }
    }