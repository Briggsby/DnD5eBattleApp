using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DnD5eBattleApp;

public class Order {
    public Creature Creature {get; set;}  // The creature making the order
    public Ability Ability {get; set;}  // The ability being used
    public BoardTile Selection {get; set;} = null;

    public Color ValidTileColor {get => GetValidTileColor();}
    public HashSet<BoardTile> ValidTiles {get; set;} = new HashSet<BoardTile>();

    public static Order NewOrder(Creature creature, Ability ability) {
        return new Order(creature, ability);
    }

    public Order(Creature creature, Ability ability) {
        this.Creature = creature;
        this.Ability = ability;

        this.SetValidTiles();
        DnDManager.ongoingOrder = this;
    }

    public virtual void SetValidTiles() {
        foreach (BoardTile tile in Creature.Encounter.board.GetTilesInRange(Creature.BoardTile, Ability.Targeting.Range)) {
            if (IsValidTileIfInRange(tile)) {
                ValidTiles.Add(tile);
            }
        }
    }

    public virtual bool IsValidTileIfInRange(BoardTile boardTile) {
        // Assumes tile is in range
        // TODO: Split these out into subclasses for targeting types
        if (Ability.Targeting.TargetType == TargetType.SingleTargetRanged || Ability.Targeting.TargetType == TargetType.SingleTargetMelee) {
            return boardTile.creature is not null;
        } else if (Ability.Targeting.TargetType == TargetType.EmptyTile) {
            return boardTile.creature is null;
        }
        return true;
    }

    public void SelectionMade(BoardTile boardTile) {
        Selection = boardTile;
        Ability.SelectionMade(this);
        FinishOrder();
    }

    public virtual void Update() {

    }

    public virtual void FinishOrder() {
        DnDManager.ongoingOrder = null;
    }

    public virtual void CancelOrder() {
        DnDManager.ongoingOrder = null;
    }

    public virtual Color GetValidTileColor() {
        if (Ability is MoveAbility) {
            return Color.Aqua;
        }
        return Color.LightPink;
    }
}