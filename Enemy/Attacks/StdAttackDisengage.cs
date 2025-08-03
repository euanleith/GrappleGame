using System;
using UnityEngine;

// todo name - its like stdattack, except it keeps attacking until its a certain distance away from player
// todo could also do distance from startPos
public class StdAttackDisengage : Attack
{
    public Vector2 minDisengageRange = new Vector2(3, 3); // todo maybe default to aggro range?

    public override bool IsFinished() {
        Transform player = gameObject.GetComponentInParent<Enemy>().player;
        Transform enemy = gameObject.GetComponentInParent<Transform>();
        return Math.Abs(enemy.position.x - player.transform.position.x) > minDisengageRange.x &&
               Math.Abs(enemy.position.y - player.transform.position.y) > minDisengageRange.y;
    }
}
