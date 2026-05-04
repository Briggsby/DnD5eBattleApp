using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class DirectionalAreaOrder : Control
    {
        Board board;
        OldSpell spell;

        public enum Style { Circle, Cone, Line }

        BoardTile selected;
        List<BoardTile> selection;

        public BoardTile origin;
        public bool includeOrigin;
        public int length;
        public int width; //default to radius
        public Color color;
        public Style style;


        public DirectionalAreaOrder(Creature creature, BoardTile origin, int length, int width, Style style, Color? color = null, OldSpell spell = null, bool includeOrigin = false) : base(creature)
        {
            Debug.WriteLine("Directional Order Started");
            board = origin.board;
            this.origin = origin;
            this.length = length;
            this.width = width;
            this.style = style;
            this.color = color ?? Color.Goldenrod;
            this.spell = spell;
            this.includeOrigin = includeOrigin;
            board.OnTileSelectionEvent += new Board.BoardDelegate(Selected);
            selection = new List<BoardTile>();

        }

        public void Selected(BoardTile tile, Board.BoardEventArgs e)
        {
            GetSelection(tile);
            SelectionMade();
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            spell.AreaTargeted(selection);
            ClearSelection();
        }

        public override void Update()
        {
            base.Update();
            if (board.hoveredBoardTile != null)
            {
                GetSelection(board.hoveredBoardTile);
            }
            if (EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.RightMouseJustDown))
            {
                RemoveControl();
                ClearSelection();
            }
        }

        public void GetSelection(BoardTile tile)
        {
            if (selected != null && selected == tile)
            {
                return;
            }
            switch (style)
            {
                case (Style.Circle):
                    SetSelection(tile, board.GetCircle(origin, tile, length, width));
                    break;
                case (Style.Cone):
                    SetSelection(tile, board.GetCone(origin, tile, length, width, includeOrigin));
                    break;
                case (Style.Line):
                    SetSelection(tile, board.GetLine(origin, tile, length, width, includeOrigin));
                    break;
            }
        }

        public void SetSelection(BoardTile tile, List<BoardTile> boardtiles)
        {
            foreach (BoardTile oldHovered in selection)
            {
                oldHovered.RemoveHoverColor();
            }
            selected = tile;
            selection = new List<BoardTile>(boardtiles);
            foreach (BoardTile hovered in selection)
            {
                hovered.SetHoverColor(color);
            }
        }

        public void ClearSelection()
        {
            foreach (BoardTile hovered in selection)
            {
                hovered.RemoveHoverColor();
            }
            board.OnTileSelectionEvent -= Selected;
        }

    }
}