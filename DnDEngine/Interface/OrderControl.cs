using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace DnD5eBattleApp
{ 
    public enum TileOrderCriteria { WithoutCreature, WithCreature }

    public class OrderControl
    {
        Encounter encounter;

        public string description;
        public List<BoardTile> validTiles;
        public Color color;
        public BoardTile selection;
        public bool cancelled;

        public bool list;

        public List<string> descriptionList;
        public List<List<BoardTile>> validTileList;
        public List<Color> colorList;
        public List<BoardTile> selectionList;

        public void SetDefaults()
        {
            selection = null;
            selectionList = new List<BoardTile>();
            cancelled = false;
        }

        public OrderControl(Encounter encounter, List<BoardTile> validTiles, Color color, string description = "", List<TileOrderCriteria> criteria = null)
        {
            SetDefaults();
            encounter.orderControl = this;
            this.encounter = encounter;
            list = false;
            this.validTiles = new List<BoardTile>(validTiles);
            this.color = color;
            this.description = description;
            this.selection = null;

            SetCriteria(criteria);

            SetHovers();
        }

        public OrderControl(Board board, BoardTile originSquare, int range, Color color, List<TileOrderCriteria> criteria = null, string description = "")
        {
            SetDefaults();
            board.encounter.orderControl = this;
            encounter = board.encounter;
            this.color = color;
            this.description = description;

            validTiles = new List<BoardTile>(board.GetTilesInRange(originSquare, range));
            SetCriteria(criteria);
            SetHovers();
        }

        public void SetCriteria(List<TileOrderCriteria> criteria)
        {
            if (criteria == null)
            {
                return;
            }

            List<BoardTile> oldValidTiles = new List<BoardTile>(validTiles);

            foreach(BoardTile bT in oldValidTiles)
            {
                if (criteria.Contains(TileOrderCriteria.WithoutCreature))
                {
                    if (bT.creature != null)
                    {
                        validTiles.Remove(bT);
                    }
                }
                if (criteria.Contains(TileOrderCriteria.WithCreature))
                {
                    if (bT.creature == null)
                    {
                        validTiles.Remove(bT);
                    }
                }
            }
        }

        public void SetHovers(bool listForm = false)
        {
            foreach(BoardTile bT in validTiles)
            {
                bT.SetOrderControl(this);
            }
        }

        public void Select(BoardTile tile)
        {
            selection = tile;
        }

        public void Cancel()
        {
            encounter.orderControl = null;
            cancelled = false;
            foreach(BoardTile bt in validTiles)
            {
                bt.RemoveOrderControl();
            }
        }
    }
}