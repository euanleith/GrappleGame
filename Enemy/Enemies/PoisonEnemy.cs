using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEnemy : Enemy
{
    public override bool OnHit(int damage) {
        bool isAlive = base.OnHit(damage);
        if (isAlive) {
            combatController.state = CombatController.State.windup;
            float aggroWindupDuration = ((StaticAttack)combatController.currentAttack).aggroWindupDuration;
            if (aggroWindupDuration < combatController.countdown) {
                combatController.StartWindup(aggroWindupDuration);
            }
        }
        return isAlive;
    }
}
