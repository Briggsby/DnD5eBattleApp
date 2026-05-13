using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace DnD5eBattleApp
{
    public class Attack
    {
        public Creature attacker;
        public Weapon attackerWeapon;
        public int numberAttacks;
        public Creature defender;

        public Ability Ability {get; set;}

        public bool isSpell;
        public OldSpell spell;

        public bool offHand = false;

        public AttackRoll attackRoll;
        public DamageRoll damageRoll;
        public Stat statUsed;

        public List<int> damageDiceFaces;
        public List<int> damageDiceNumber;
        public List<string> damageTypes;

        public bool finished;

        public int range;

        public bool IsMelee
        {
            get { if (range <= attacker.attackRange) { return true; } else { return false; } }
        }

        public Attack(Creature attacker, Creature defender, Weapon attackerWeapon, bool offHand = false, int numberOfAttacks = 1)
        {
            this.attacker = attacker;
            this.defender = defender;
            this.attackerWeapon = attackerWeapon ?? new Weapon();
            this.numberAttacks = numberOfAttacks;
            this.offHand = offHand;

            GetRange();

            finished = false;

            attacker.attacksThisTurn.Add(this);
            DoAttackRoll();

        }

        public Attack(Creature attacker, Creature defender, OldSpell spell)
        {
            this.attacker = attacker;
            this.defender = defender;
            this.spell = spell;
            isSpell = true;
            this.numberAttacks = 1;

            GetRange();

            finished = false;
            attacker.attacksThisTurn.Add(this);
            DoAttackRoll();
        }

        public Attack(Creature attacker, Creature defender, Ability ability)
        {
            this.attacker = attacker;
            this.defender = defender;
            this.Ability = ability;

            GetRange();
            finished = false;
            attacker.attacksThisTurn.Add(this);
            DoAttackRoll();
        }

        public void GetRange()
        {
            range = attacker.encounter.board.GetDistance(attacker.boardTile, defender.boardTile);
        }

        public int GetAttackBonus()
        {
            int bonus = 0;
            if (Ability is not null)
            {
                return Ability.GetAttackBonus(attacker);
            }

            if (!isSpell)
            {
                if (attackerWeapon.weaponProperties.Contains(WeaponProperty.Finesse))
                {
                    if (attacker.stats[Stat.Strength] >= attacker.stats[Stat.Dexterity])
                    {
                        statUsed = Stat.Strength;
                        bonus += attacker.StatMod(Stat.Strength);
                    }
                    else
                    {
                        statUsed = Stat.Dexterity;
                        bonus += attacker.StatMod(Stat.Dexterity);
                    }
                }
                else
                {
                    bonus += attacker.StatMod(attackerWeapon.AbilityStat);
                    statUsed = attackerWeapon.AbilityStat;
                }

                if (attacker.IsProficientInWeapon(attackerWeapon))
                {
                    bonus += attacker.proficiencyBonus;
                }

                bonus += attackerWeapon.attackBonus;
                //Debug.WriteLine(string.Format("Bonus is {0}, from a {3} mod of {1} and a proficiency bonus of {2}", bonus, attack.attacker.StatMod(statUsed), attack.attacker.proficiencyBonus, statUsed));
            }
            else
            {
                if (!spell.scroll)
                {
                    bonus += attacker.StatMod(spell.abilityModifier);
                    bonus += attacker.proficiencyBonus;
                }
                //Debug.WriteLine(string.Format("Bonus is {0}, from a mod of {1} and a proficiency bonus of {2}", bonus, attack.attacker.StatMod(spell.abilityModifier), attack.attacker.proficiencyBonus));
            }

            return bonus;
        }

        public int GetDamageBonus()
        {
            if (isSpell)
            {
                return spell.GetDamageBonus();
            }
            else
            {
                if (!offHand)
                {
                    return attacker.StatMod(statUsed) + attackerWeapon.damageBonus;
                }
                else
                {
                    return attackerWeapon.damageBonus;
                }
            }
        }

        public bool GetAdvantage()
        {
            return false;
        }

        public bool GetDisadvantage()
        {
            return false;
        }

        public void DoAttackRoll()
        {
            attackRoll = new AttackRoll(this);
            attackRoll.DoRoll();
        }

        public void FinishAttackRoll(Roll roll, RollEventArgs e)
        {
            RemoveThrownOrAmmunition();
            if (Ability is not null)
            {
                Ability.FinishAttackRoll(roll as AttackRoll, e);
                finished = true;
                return;
            }
            if (attackRoll.Success)
            {
                Debug.WriteLine(string.Format("{0} hit {1} with an attack with an attack roll of {2} ({3} + {4}) against an AC of {5} {6}", attacker.name, defender.name, roll.score, roll.score - roll.bonus, roll.bonus, defender.AC, roll.WithAdvantagePrint()));
                DoDamageRoll();
            }
            else
            {
                Debug.WriteLine(string.Format("{0} missed {1} with an attack with an attack roll of {2} ({3} + {4}) against an AC of {5} {6}", attacker.name, defender.name, roll.score, roll.score - roll.bonus, roll.bonus, defender.AC, roll.WithAdvantagePrint()));
            }
            finished = true;
        }

        public void RemoveThrownOrAmmunition()
        {
            if (Ability is not null)
            {
                // Todo: Should be handled by 'spend' on ability, remove this method that's implemented
                return;
            }
            if (isSpell)
            {
                return;
            }
            if (attackerWeapon.weaponProperties.Contains(WeaponProperty.Thrown))
            {
                if (!attackerWeapon.weaponProperties.Contains(WeaponProperty.Reach) && attacker.boardTile.board.GetDistance(attacker.boardTile, defender.boardTile) > 5)
                {
                    if (attacker.weaponMainHand == attackerWeapon)
                    {
                        attacker.weaponMainHand = null;
                    }
                    else if (attacker.weaponOffHand == attackerWeapon)
                    {
                        attacker.weaponOffHand = null;
                    }
                }
                else if (attackerWeapon.weaponProperties.Contains(WeaponProperty.Reach) && attacker.boardTile.board.GetDistance(attacker.boardTile, defender.boardTile) > 10)
                {
                    if (attacker.weaponMainHand == attackerWeapon)
                    {
                        attacker.weaponMainHand = null;
                    }
                    else if (attacker.weaponOffHand == attackerWeapon)
                    {
                        attacker.weaponOffHand = null;
                    }
                }
            }
        }

        public void DoDamageRoll()
        {
            damageRoll = new DamageRoll(this);
            damageRoll.DoRoll();
        }

        public void GetDamageDice(DamageRoll roll)
        {
            if (Ability is not null)
            {
                damageDiceFaces = new List<int>() {Ability.BaseDamage.MaxValueOfDice};
                damageDiceNumber = new List<int>() {Ability.BaseDamage.NumberOfDice};
                damageTypes = new List<string>() {Ability.BaseDamage.DamageType};
            } else if (!isSpell)
            {
                attackerWeapon.GetDamageDice(this);
            }
            else
            {
                spell.GetDamageDice(this);
            }

            roll.diceFaces = new List<int>(damageDiceFaces);
            roll.numberOfDice = new List<int>(damageDiceNumber);
            roll.damageTypes = new List<string>(damageTypes);

        }

        public void FinishDamageRoll(Roll roll, RollEventArgs e)
        {
            if (Ability is not null)
            {
                Ability.FinishDamageRoll(roll as DamageRoll, e);
                return;
            }
            if (!isSpell)
            {
                Debug.WriteLine(string.Format("{0} did {1} ({2} + {3}) {4} damage to {5} with a {6}", attacker.name, roll.score, roll.score-roll.bonus, roll.bonus, damageTypes[0], defender.name, attackerWeapon.name));
            }
            else
            {
                Debug.WriteLine(string.Format("{0} did {1} ({2} + {3}) {4} damage to {5} with {6}", attacker.name, roll.score, roll.score-roll.bonus, roll.bonus, damageTypes[0], defender.name, spell.name));
            }

            defender.TakeDamage(this);
        }

        public object GetSource()
        {
            if (Ability is not null)
            {
                return Ability;
            }
            if (isSpell)
            {
                return spell;
            }
            else
            {
                return attackerWeapon;
            }
        }
    }
}