// using System;
// using System.Collections.Generic;
// using System.Numerics;
// using BugsbyEngine;

// namespace DnD5eBattleApp;

// public enum CreatureValueNew
// {
//     Speed,
//     HitPoints,
//     TemporaryHitPoints,
//     ArmorClass,
//     Strength,
//     Dexterity,
//     Constitution,
//     Intelligence,
//     Wisdom,
//     Charisma,
//     SavingThrowProficiencies,
//     DamageVulnerabilities,
//     DamageResistances,
//     DamageImmunities
// }
// public class CreatureNew : GameObject
// {
//     public BoardTile BoardTile {get; set;}
//     public string Name {get; set;} = "Creature";
//     public int HitDiceValue {get; set;} = 8;
//     public int HitDiceNumber {get; set;} = 1;

//     public ValueManager<CreatureValueNew> Values = new ValueManager<CreatureValueNew>();
//     public Inventory Inventory {get; set;}
//     public SpellBook SpellBook {get; set;}
//     public FeatManager FeatManager {get; set;}

//     public int AmountMovedThisTurn {get; set;} = 0;
//     public bool ActionTakenThisTurn {get; set;} = false;
//     public bool BonusActionTakenThisTurn {get; set;} = false;

//     public void SetDefaultValues()
//     {
//         Values.AddValue<int>(CreatureValueNew.Speed, 30);
//         Values.AddValue<int>(CreatureValueNew.HitPoints, 10);
//         Values.AddValue<int>(CreatureValueNew.TemporaryHitPoints, 0);
//         Values.AddValue<int>(CreatureValueNew.ArmorClass, 10);
//         Values.AddValue<int>(CreatureValueNew.Strength, 10);
//         Values.AddValue<int>(CreatureValueNew.Dexterity, 10);
//         Values.AddValue<int>(CreatureValueNew.Constitution, 10);
//         Values.AddValue<int>(CreatureValueNew.Intelligence, 10);
//         Values.AddValue<int>(CreatureValueNew.Wisdom, 10);
//         Values.AddValue<int>(CreatureValueNew.Charisma, 10);
//         Values.AddValue<HashSet<Stat>>(CreatureValueNew.SavingThrowProficiencies, new HashSet<Stat>());
//         Values.AddValue<HashSet<DamageType>>(CreatureValueNew.DamageVulnerabilities, new HashSet<DamageType>());
//         Values.AddValue<HashSet<DamageType>>(CreatureValueNew.DamageResistances, new HashSet<DamageType>());
//         Values.AddValue<HashSet<DamageType>>(CreatureValueNew.DamageImmunities, new HashSet<DamageType>());
//     }

//     public CreatureNew() : base(new Vector2(0, 0))
//     {
//         Inventory = new Inventory(this, new List<Item>());
//         SpellBook = new SpellBook(this);
//         FeatManager = new FeatManager(this);
//         SetDefaultValues();
//     }

//     public void StartTurn()
//     {
//         AmountMovedThisTurn = 0;
//         ActionTakenThisTurn = false;
//         BonusActionTakenThisTurn = false;
//     }

//     public void EndTurn()
//     {
        
//     }

//     # region Movement & Board Tile Logic

//     protected Encounter Encounter {get => BoardTile.board.encounter; }

//     public void AddToBoard(BoardTile tile)
//     {
//         SetToTile(tile);
//         Encounter.creatures.Add(this);
//         SortTexture();
//     }

//     public void SortTexture()
//     {
//         transform.layerDepth = -0.01f;
//         transform.SetSize(BoardTile.transform.Size);
//     }

//     public void SetToTile(BoardTile tile)
//     {
//         if (BoardTile is not null)
//         {
//             BoardTile.creature = null;
//         }
//         transform.localPosition = new Vector2(0, 0);
//         BoardTile = tile;
//         tile.creature = this;
//         SetParent(tile, true, true);
//     }

//     public void MoveTo(BoardTile tile)
//     {
//         AmountMoved += tile.board.GetDistance(BoardTile, tile);
//         SetToTile(tile);
//     }

//     # endregion


// }