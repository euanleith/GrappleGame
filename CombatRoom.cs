using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : Room
{
    bool completed = false;
    [SerializeField] public Enemy[][] waves;

    void Update() {
        base.Update();

        completed = true;
        foreach (Enemy enemy in enemies) {
            if (enemy.gameObject.activeSelf) {
                completed = false;
                break;
            }
        }
        if (completed) {
            Transform combatDoor = gameObject.transform.Find("Triggers/CombatDoor/Door");
            if (combatDoor) {
                combatDoor.GetComponent<Animator>().enabled = true;
            }
        }
    }

    public override void Enable() {
        if (!completed) { // enemies don't respawn in combat rooms
            EnableEnemies();
        }
        // todo EnableMovingPlatforms();
    }
}
