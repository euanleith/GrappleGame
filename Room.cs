using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform player;

    [HideInInspector] public Enemy[] enemies;
    [HideInInspector] public Platform[] platforms;
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
        InitPlatforms();
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
        enemies = GetFromFolder<Enemy>("Enemies", "Controller");
        foreach (Enemy enemy in enemies) {
            enemy.Init();
        }
    }

    protected void InitPlatforms() {
        platforms = GetFromFolder<Platform>("MovingPlatforms", ""); // todo folder name
        foreach (Platform platform in platforms) {
            platform.Init();
        }
    }

    T[] GetFromFolder<T>(string folderPath, string itemPath) {
        Transform folder = gameObject.transform.Find(folderPath);
        if (folder) {
            T[] res = new T[folder.childCount];
            for (int i = 0; i < folder.childCount; i++) {
                res[i] = folder.GetChild(i).transform.Find(itemPath).GetComponent<T>();
            }
            return res;
        } else{} return new T[0];
    }

    public virtual void Enable() {
        EnableEnemies();
        EnablePlatforms();
    }

    protected void EnableEnemies() {
        foreach (Enemy enemy in enemies) {
            enemy.Reset(); // todo rename functions Enable()?
        }
    }

    protected void EnablePlatforms() {
        foreach (Platform platform in platforms) {
            platform.Reset(); // todo rename functions Enable()?
        }
    }

    public virtual void Disable() {
        DisableEnemies();
        DisablePlatforms();
    }

    protected void DisableEnemies() {
        foreach (Enemy enemy in enemies) {
            enemy.Disable();
        }
    }

    protected void DisablePlatforms() {
        foreach (Platform platform in platforms) {
            platform.Disable();
        }
    }

    // todo maybe always do player stuff here
    public void Reset() {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.position = spawn; // todo maybe tp camera instead of making it lerp? // todo this should call a function in camera
        rb.velocity = new Vector2(0, 0);
        player.GetComponent<PlayerControls>().grapple.grappleRope.enabled = false;
        Enable();
    }
}