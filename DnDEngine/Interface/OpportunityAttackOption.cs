using System.Collections;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class OpportunityAttackOption : DisplayOptions
    {
        public static DisplayOptionsTextureSet opportunityAttackTextureSet;

        Creature mover;
        Creature attacker;

        public OpportunityAttackOption(Creature mover, Creature attacker) : 
            base(question: string.Format("{0} : Use reaction to opportunity attack {1}?", attacker.Name, mover.Name), textureSet: opportunityAttackTextureSet)
        {
            this.mover = mover;
            this.attacker = attacker;
            questionTextObj.transform.color = Color.Black;
        }

        public override void Yes(Button button, Button.ButtonEventArgs e)
        {
            DestroyAndChildren();
            EngManager.StartCoroutine(DoAttack());
        }

        public override void No(Button button, Button.ButtonEventArgs e)
        {
            base.No(button, e);
        }

        IEnumerator DoAttack()
        {
            Attack attack = new Attack(attacker, mover, attacker.weaponMainHand);
            while (!attack.finished)
            {
                yield return null;
            }
            finished = true;
        }
                            
    }
}