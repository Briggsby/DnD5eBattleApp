using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace DnD5eBattleApp;
public class Attack
{
    public Creature Attacker {get; set;}
    public Creature Defender {get; set;}
    public Ability Ability {get; set;}
    public int Range {get => Attacker.Encounter.board.GetDistance(Attacker, Defender);}

    public AttackRoll AttackRoll {get; set;}
    public bool Finished {get; set;}

    public Attack(Creature attacker, Creature defender, Ability ability)
    {
        this.Attacker = attacker;
        this.Defender = defender;
        this.Ability = ability;
        Finished = false;
        attacker.attacksThisTurn.Add(this);
        AttackRoll = new AttackRoll(this);
        AttackRoll.DoRoll();
    }

    public void FinishAttackRoll(Roll roll, RollEventArgs e)
    {
        Ability.FinishAttackRoll(roll as AttackRoll, e);
        Finished = true;
    }
}