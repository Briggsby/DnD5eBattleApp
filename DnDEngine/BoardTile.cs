using BugsbyEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DnD5eBattleApp
{
    public class BoardTile : GameObject
    {
        public Board board;
        public Vector2 coords;
        BoardShape boardTileShape;
        public Creature creature;

        public BoardTile(BoardShape boardShape, Vector2 position, Texture2D texture, int size, Board board) : base(position, texture, board)
        {
            this.board = board;
            boardTileShape = boardShape;
            transform.SetSize(new Vector2(size, size), texture);
            transform.LayerDepth = EngManager.layerDepths[LayerDepths.Ground];
            creature = null;

            if (boardShape == BoardShape.Square)
            {
                BoardTileSquare(position, texture, size);

            }
        }

        public override void Update()
        {
            base.Update();
            if (DnDManager.ongoingOrder != null && DnDManager.ongoingOrder.ValidTiles.Contains(this))
            {
                transform.color = DnDManager.ongoingOrder.ValidTileColor;
            } else {
                transform.color = Color.White;
            }
        }

        public void BoardTileSquare(Vector2 position, Texture2D texture, int size)
        {
            MakeHitbox(new Vector2(0, 0), HitboxShape.Rectangle, new Vector3(size, size, 0));
        }

        public override void WhileMouseOver()
        {
            base.WhileMouseOver();
            if (board.mouseHoverActive && !EngManager.controls.mouseOverContextMenu)
            {
                board.SetHoveredTile(this);
            }
        }

        public override void OnRightClick()
        {
            base.OnRightClick();
            board.SetHoveredTile(this);
            if (DnDManager.ongoingOrder != null)
            {
                DnDManager.ongoingOrder.CancelOrder();
            }
            board.encounter.MakeContextMenu(this);
        }

        public override void OnClick()
        {
            base.OnClick();
            if (!EngManager.controls.mouseOverContextMenu)
            {
                board.SetHoveredTile(this);
            }
            Select();
        }

        public void Select()
        {
            if (DnDManager.ongoingOrder != null && DnDManager.ongoingOrder.ValidTiles.Contains(this))
            {
                DnDManager.ongoingOrder.SelectionMade(this);
                return;
            }

            if (creature != null)
            {
                if (board.encounter.contextMenu == null)
                {
                    board.encounter.MakeCommandMenu(this, creature);
                }
            }

            board.TileSelected(this);
        }

        public void SetHoverColor(Color color)
        {
            transform.color = color;
        }

        public void RemoveHoverColor()
        {
            transform.color = Color.White;
        }
    }
}