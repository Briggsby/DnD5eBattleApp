using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace DnD5eBattleApp
{
    public abstract class Control
    {
        public OrderControl orderControl;

        public Feat linkedFeat;

        public Control()
        {
            DnDManager.ongoingControls.Add(this);
        }

        protected IEnumerator SetOrderControl(Board board, BoardTile originSquare, int range, Color color, List<TileOrderCriteria> criteria = null, string description = "")
        {
            orderControl = new OrderControl(board, originSquare, range, color, criteria, description);
            while (!orderControl.cancelled)
            {
                if (orderControl.cancelled)
                {
                    break;
                }

                if (orderControl.selection == null)
                {
                    yield return null;
                }
                else
                {
                    orderControl.Cancel();
                    SelectionMade();
                    break;
                }
            }
        }

        public virtual void SelectionMade()
        {
            if (linkedFeat != null)
            {
                linkedFeat.SelectionMadeOrder(this);
            }
            RemoveControl();
        }

        public virtual void RemoveControl()
        {
            DnDManager.ongoingControls.Remove(this);
        }

        public virtual void Update()
        {

        }

    }
}