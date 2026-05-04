using System;
using System.Collections.Generic;

namespace DnD5eBattleApp;

public record DamageSpec
{
    public required DamageType Type {get; init;}
    public required int DamageDiceValue {get; init;}
    public required int DamageDiceNumber {get; init;}
}

public record ConditionApplySpec
{
    public required string Condition {get; init;}
    public Dictionary<object, object> ConditionParameters {get; init;} = new Dictionary<object, object>();
}

public record SpellSpec
{
    public required string Name {get; init;}
    public required int Level {get; init;}
    public required string School {get; init;}
    public bool IsAction {get; init;} = true;
    public bool IsBonusAction {get; init;} = false;
    public required int Range {get; init;}  // 5 for touch, 0 for self

    public required bool VerbalComponent {get; init;}
    public required bool SomaticComponent {get; init;}
    public required bool MaterialComponent {get; init;}

    public string MaterialComponentDescription {get; init;} = "";

    public int Duration {get; init;}  // Duration in Rounds

    public bool IsSpellAttack {get; init;} = false;
    public Spell.TargetType TargetType {get; init;}
    public int TargetCount {get; init;} = 1;

    public List<DamageSpec> Damages {get; init;} = new List<DamageSpec>();
    public List<ConditionApplySpec> AppliesConditions {get; init; } = new List<ConditionApplySpec>();
    public bool ScaleCantripDamage {get; init;} = true;

    public string Description {get; init;} = "";
}