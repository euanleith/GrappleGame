using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [HideInInspector] public Enemy[] enemies;
    [HideInInspector] public Vector2 spawn; // todo only spawn rooms need this, maybe could make SpawnRoom subclass
    [HideInInspector] public Vector2 minPos;
    [HideInInspector] public Vector2 maxPos;
    // todo could store all spawns for this room here and make BoundsEnter access them

    // todo camera clamping to bounds isn't working for (some) non-rectangular shapes
    //  honestly i dont know how it was working before
    //  probably need multiple bounds, which are switched between 
    //      set new current bound when player enters it for the first time. Then make sure bounds are overlapping, otherwise camera will stop before even reaching the new bound
    //      nope, that won't work where player is in bound 1, enters overlapping bound 2, then exits bound 2 while still in bound 1
    //      so maybe should set new current bound when player is only in one bound

    public void Awake()
    {
        InitBounds();
        InitEnemies();
        // todo InitMovingPlatforms()
        Disable();
    }

    protected void Update() {}

    protected void InitBounds() {
        Transform boundsFolder = gameObject.transform.Find("Bounds");
        minPos = boundsFolder.Find("MinPos").position;
        maxPos = boundsFolder.Find("MaxPos").position;
        Transform tSpawn = boundsFolder.Find("Spawn");
        if (tSpawn) {
            spawn = tSpawn.position;
        }
    }

    protected void InitEnemies() {
        Transform enemyFolder = gameObject.transform.Find("Enemies");
        if (enemyFolder) {
            enemies = new Enemy[enemyFolder.childCount];
            for (int i = 0; i < enemyFolder.childCount; i++) {
                enemies[i] = enemyFolder.GetChild(i).transform.Find("Controller").GetComponent<Enemy>();
                enemies[i].Init(); // todo also disable, or am i already doing that?
            }
        } else enemies = new Enemy[0];
        //enemies = GetComponentInChildren<Enemy>(); // todo this is a slower option, but doesn't require enemy folder to be called 'Enemies'
    }

    public virtual void Enable() {
        EnableEnemies();
        // todo EnableMovingPlatforms();
    }

    protected void EnableEnemies() {
        foreach (Enemy enemy in enemies) {
            enemy.Reset();
        }
    }

    public virtual void Disable() {
        DisableEnemies();
        // todo DisableMovingPlatforms();
    }

    protected void DisableEnemies() {
        foreach (Enemy enemy in enemies) {
            enemy.Disable();
        }
    }
}