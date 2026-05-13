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
    public struct BoardTextureSet
    {
        public Texture2D tileHoverTexture;
        public Texture2D tileTexture;
    }

    public struct ContextMenuTextureSet
    {
        public List<Texture2D> baseTextures;
        public SpriteFont baseFont;
        public List<Texture2D> blankTileTextures;
    }

    public enum BoardShape { Square, Hex }

    public class Board : GameObject
    {
        public Encounter encounter;
        public BoardShape boardShape;
        BoardTile[,] boardTiles;
        BoardTextureSet textureSet;

        public bool activeBoard;
        public bool mouseHoverActive;

        public BoardTile hoveredBoardTile;
        public GameObject hoverOverlayTile;

        public int tileGameSize;

        public Board(Encounter encounter, BoardShape boardShape, Vector2 position, Vector2 dimensions, int tileSize, int tileGameSize, BoardTextureSet textureSet) : base(new Vector2(dimensions.X * tileSize / 2, dimensions.Y * tileSize / 2) + position)
        {
            this.encounter = encounter;
            this.textureSet = textureSet;
            this.tileGameSize = tileGameSize;
            if (boardShape == BoardShape.Square)
            {
                LaySquareBoard(dimensions, tileSize);
            }
        }

        public void LaySquareBoard(Vector2 dimensions, int tileSize)
        {
            boardShape = BoardShape.Square;
            MakeHitbox(new Vector2(0, 0), HitboxShape.Rectangle, new Vector3(dimensions.X * tileSize, dimensions.Y * tileSize, 0));
            boardTiles = new BoardTile[(int)dimensions.X, (int)dimensions.Y];
            int xPos = (int)((-dimensions.X / 2) * tileSize + tileSize / 2);
            for (int x = 0; x < dimensions.X; x++)
            {
                int yPos = (int)((-dimensions.Y / 2) * tileSize + tileSize / 2);
                for (int y = 0; y < dimensions.Y; y++)
                {
                    MakeBoardTileSquare(x, y, xPos, yPos, tileSize);
                    yPos += tileSize;
                }
                xPos += tileSize;
            }
        }

        public BoardTile MakeBoardTileSquare(int xIndex, int yIndex, int xPosition, int yPosition, int squareSize)
        {
            boardTiles[xIndex, yIndex] = new BoardTile(BoardShape.Square, new Vector2(xPosition, yPosition), textureSet.tileTexture, squareSize, this);
            boardTiles[xIndex, yIndex].coords = new Vector2(xIndex, yIndex);
            return boardTiles[xIndex, yIndex];
        }

        public void SetHoveredTile(BoardTile tile)
        {
            hoveredBoardTile = tile;
            if (hoverOverlayTile != null)
            {
                hoverOverlayTile.DestroyAndChildren();
            }
            hoverOverlayTile = new GameObject(new Vector2(0, 0), textureSet.tileHoverTexture, tile);
            hoverOverlayTile.transform.layerDepth = -0.001f;
            hoverOverlayTile.transform.Size = hoveredBoardTile.transform.Size;
        }

        public void DestroyBoard()
        {
            this.DestroyAndChildren();
        }

        public void ClearHoveredTile()
        {
            if (hoveredBoardTile == null)
            {
                return;
            }
            hoverOverlayTile.DestroyAndChildren();
            hoverOverlayTile = null;
            hoveredBoardTile = null;
        }

        public override void Update()
        {
            base.Update();

            CheckMouseHoverActive();

            if (mouseHoverActive)
            {
                if (!hitbox.CheckMouseOver())
                {
                    ClearHoveredTile();
                }
            }
        }

        public bool CheckMouseHoverActive()
        {
            if (EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.MouseActive))
            {
                if (encounter.contextMenu == null)
                {
                    return mouseHoverActive = true;
                }
            }

            return mouseHoverActive;
        }

        #region Delegates
        public class BoardEventArgs : EventArgs
        {

        }

        public delegate void BoardDelegate(BoardTile boardTile, BoardEventArgs eventArgs);

        public event BoardDelegate OnTileSelectionEvent;

        public void TileSelected(BoardTile tile)
        {
            OnTileSelectionEvent?.Invoke(tile, null);
        }

        #endregion

        #region Tile Stuff

        public BoardTile Tile(Vector2 coords)
        {
            return boardTiles[(int)coords.X, (int)coords.Y];
        }

        public BoardTile TileIfExists(Vector2 coords)
        {
            if (TileExists(coords))
            {
                return Tile(coords);
            }
            else
            {
                return null;
            }
        }

        public bool TileExists(Vector2 coords)
        {
            if (coords.X >= 0 && coords.X < boardTiles.GetLength(0) && coords.Y >= 0 && coords.Y < boardTiles.GetLength(1))
            {
                return Tile(coords) != null;
            }
            else
            {
                return false;
            }
        }

        public int GetDistance(BoardTile origin, BoardTile destination)
        {
            return ((int)Math.Max(Math.Abs(destination.coords.X - origin.coords.X), Math.Abs(destination.coords.Y - origin.coords.Y))) * tileGameSize;
        }

        public int GetDistance(Creature origin, Creature destination)
        {
            return GetDistance(origin.BoardTile, destination.BoardTile);
        }

        public List<BoardTile> GetTilesInRange(BoardTile originSquare, int range, bool includeOrigin = false)
        {
            List<BoardTile> tileList = new List<BoardTile>();

            //int offset; //The offset for diagonals in distance calculations for square
            if (boardShape == BoardShape.Square)
            {
                int rangeInTiles = (int)(range / tileGameSize);
                for (int x = -rangeInTiles; x <= rangeInTiles; x++)
                {
                    for (int y = -rangeInTiles; y <= rangeInTiles; y++)
                    {
                        Vector2 coords = new Vector2(originSquare.coords.X + x, originSquare.coords.Y + y);
                        if (TileExists(coords))
                        {
                            if (includeOrigin || Tile(coords) != originSquare)
                            {
                                tileList.Add(Tile(coords));
                            }
                        }
                    }
                }
            }

            return tileList;
        }

        public List<BoardTile> GetLine(BoardTile origin, BoardTile destination, int length, int width, bool includeOrigin = false, int error = 4)
        {
            Vector2 originPosition = origin.transform.GlobalPosition;
            Vector2 destinationPosition = destination.transform.GlobalPosition;
            Vector2 differencePixels = destinationPosition - originPosition;
            Vector2 directionPixels = Vector2.Normalize(differencePixels);

            Vector2 perpendicularDirectionPixels = new Vector2(directionPixels.Y, -directionPixels.X);

            int tilePixelSize = (int)origin.transform.Size.X;
            float sizePerPixel = tileGameSize / origin.transform.Size.X;

            List<BoardTile> tiles = new List<BoardTile>();
            tiles.Add(origin);
            BoardTile lastTile = origin;
            Vector2 currentPosition = originPosition;

            float differencePixelsLength = (int)Math.Abs(differencePixels.Length());

            for (int i = 0; i <= differencePixelsLength; i += error)
            {

                if (i * sizePerPixel > length)
                {
                    break;
                }
                currentPosition = originPosition + (i * directionPixels);

                Vector2 currentWidthPosition = currentPosition;
                BoardTile tile;


                for (int j = -(int)((width / 2) / sizePerPixel); j <= (int)((width / 2) / sizePerPixel); j += error)
                {
                    currentWidthPosition = currentPosition + (j * perpendicularDirectionPixels);
                    tile = FindTilePixel(currentWidthPosition);

                    if (tile != null && tile != lastTile && !tiles.Contains(tile))
                    {
                        tiles.Add(tile);
                        lastTile = tile;
                    }

                }
            }

            if (!includeOrigin)
            {
                tiles.Remove(origin);
            }

            return tiles;

        }

        public List<BoardTile> GetCone(BoardTile origin, BoardTile destination, int length, int width, bool includeOrigin = false, int error = 4)
        {
            Vector2 originPosition = origin.transform.GlobalPosition;
            Vector2 destinationPosition = destination.transform.GlobalPosition;
            Vector2 differencePixels = destinationPosition - originPosition;
            Vector2 directionPixels = Vector2.Normalize(differencePixels);

            Vector2 perpendicularDirectionPixels = new Vector2(directionPixels.Y, -directionPixels.X);

            int tilePixelSize = (int)origin.transform.Size.X;
            float sizePerPixel = tileGameSize / origin.transform.Size.X;

            List<BoardTile> tiles = new List<BoardTile>();
            tiles.Add(origin);
            BoardTile lastTile = origin;
            Vector2 currentPosition = originPosition;

            int widthInPixels = (int)(width / sizePerPixel);
            float rateOfWidthGrowth = (widthInPixels) / (length / sizePerPixel);

            for (int i = (int)(Math.Abs(differencePixels.Length())); i > 0; i -= error)
            {

                if (i * sizePerPixel > length)
                {
                    i = (int)(length / sizePerPixel);
                }
                currentPosition = originPosition + (i * directionPixels);

                Vector2 currentWidthPosition = currentPosition;
                BoardTile tile;

                int widthAti = (int)(i * rateOfWidthGrowth);
                for (int j = -widthAti / 2; j <= widthAti / 2; j += error)
                {
                    currentWidthPosition = currentPosition + (j * perpendicularDirectionPixels);
                    tile = FindTilePixel(currentWidthPosition);

                    if (tile != null && tile != lastTile && !tiles.Contains(tile))
                    {
                        tiles.Add(tile);
                        lastTile = tile;
                    }

                }
            }

            if (!includeOrigin)
            {
                tiles.Remove(origin);
            }

            return tiles;

        }

        public BoardTile FindTilePixel(Vector2 pixelCoords)
        {
            BoardTile reference = boardTiles[0, 0];
            Vector2 tilePixelSize = reference.transform.Size;
            Vector2 positionReference = reference.transform.GlobalPosition - reference.transform.Size / 2;
            Vector2 relativePosition = pixelCoords - positionReference;

            Vector2 coords = new Vector2((int)(relativePosition.X / tilePixelSize.X), (int)(relativePosition.Y / tilePixelSize.Y));

            return TileIfExists(coords);
        }

        public List<BoardTile> GetCircle(BoardTile origin, BoardTile destination, int length, int width)
        {
            Vector2 originCoords = origin.coords;
            Vector2 destinationCoords = destination.coords;
            Vector2 difference = destinationCoords - originCoords;
            if (Math.Abs(difference.Length()) > (length / tileGameSize))
            {
                difference = Vector2.Normalize(difference) * length / tileGameSize;
                destinationCoords = new Vector2((int)(originCoords.X + difference.X), (int)(originCoords.Y + difference.Y));
            }

            return GetTilesInRange(Tile(destinationCoords), width, true);
        }

        #endregion
    }
}
