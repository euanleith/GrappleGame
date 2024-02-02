using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    public Transform player;

    [HideInInspector] public RoomElement[] elements;
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

    // todo should only be able to grapple platforms in the current room

    public void Awake()
    {
        InitBounds();
        InitElements();
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

    protected void InitElements() {
        RoomElement[] enemies = GetFromFolder<Enemy>("Enemies");
        RoomElement[] platforms = GetFromFolder<RoomElement>("MovingPlatforms"); // todo folder name - ActionObjects? InteractionObjects?
        elements = enemies.Concat(platforms).ToArray();;
        foreach (RoomElement elem in elements) {
            elem.Init();
        }
    }

    T[] GetFromFolder<T>(string folderPath) {
        Transform folder = gameObject.transform.Find(folderPath);
        if (folder) {
            T[] res = new T[folder.childCount];
            for (int i = 0; i < folder.childCount; i++) {
                res[i] = folder.GetChild(i).transform.GetComponentInChildren<T>();
            }
            return res;
        } else return new T[0];
    }

    public virtual void Enable() {
        player.position = spawn;
        foreach (RoomElement elem in elements) {
            elem.Reset();
        }
    }

    public virtual void Disable() {
        foreach (RoomElement elem in elements) {
            elem.Disable();
        }
    }

    // todo maybe always do player stuff here
    public void Reset() {
        // todo move to Reset() function in PlayerControls
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        PlayerControls playerControls = player.GetComponent<PlayerControls>();
        playerControls.grapple.grappleRope.enabled = false;
        playerControls.isGrounded = true;
        playerControls.FinishStun();
        player.GetComponent<Health>().currentIFrames = 0;
        Enable();
    }
}