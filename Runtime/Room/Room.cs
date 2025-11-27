using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    public Transform player;
    [HideInInspector] public RoomElement[] elements;
    [HideInInspector] public Vector2 spawn; // todo remove this after migration, replace with defaultSpawn
    private Spawn defaultSpawn;
    [HideInInspector] public Vector2 minPos;
    [HideInInspector] public Vector2 maxPos;

    private bool hasLoggedNoDefaultSpawn = false;

    // todo camera clamping to bounds isn't working for (some) non-rectangular shapes
    //  honestly i dont know how it was working before
    //  probably need multiple bounds, which are switched between 
    //      set new current bound when player enters it for the first time. Then make sure bounds are overlapping, otherwise camera will stop before even reaching the new bound
    //      nope, that won't work where player is in bound 1, enters overlapping bound 2, then exits bound 2 while still in bound 1
    //      so maybe should set new current bound when player is only in one bound

    private void OnValidate() {
        if (defaultSpawn == null && !hasLoggedNoDefaultSpawn && GetComponentInChildren<Spawn>() != null) { // todo added this last condition temporarily while migrating to stop all the error logs
            Debug.LogError("Room " + name + " does not have a default spawn, please flag one child Spawn as 'isDefaultSpawn'", this);
            hasLoggedNoDefaultSpawn = true;
        } else {
            hasLoggedNoDefaultSpawn = false;
        }
    }

    public void Awake()
    {
        InitBounds();
        InitElements();
        Disable();
    }

    private void Reset() {
        InitNewBounds();
    }

    protected void Update() {
    }

    protected void InitBounds() {
        Transform boundsFolder = gameObject.transform.Find("Bounds");
        // todo is boundsFolder is null, use new system
        //if (boundsFolder != null) {
            minPos = boundsFolder.Find("MinPos").position;
            maxPos = boundsFolder.Find("MaxPos").position;
            Transform tSpawn = boundsFolder.Find("Spawn");
            if (tSpawn && tSpawn.gameObject.activeSelf) {
                spawn = tSpawn.position;
            }
        //} else {
        //}
    }

    private void InitNewBounds() {
        gameObject.AddComponent<RoomBound>();
    }

    protected void InitElements() {
        RoomElement[] enemies = GetFromFolder<Enemy>("Enemies");
        RoomElement[] platforms = GetFromFolder<RoomElement>("MovingPlatforms"); // todo folder name - ActionObjects? InteractionObjects?
        elements = enemies.Concat(platforms).ToArray();
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
    public void CustomReset() {
        // todo move to Reset() function in PlayerMovement
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        Player playerController = player.GetComponent<Player>();
        playerMovement.grapple.Disable();
        playerController.FinishStun();
        playerMovement.velocityOfGround = Vector2.zero;
        // todo isGrounded = true
        player.GetComponent<PlayerHealth>().DeactivateIFrames();
        Enable();
    }

    public void UpdateDefaultSpawn(Spawn spawn) {
        if (spawn.IsDefaultSpawn()) {
            if (defaultSpawn != null) defaultSpawn.RemoveAsDefaultSpawn();
            defaultSpawn = spawn;
            this.spawn = spawn.transform.position;
        } else if (defaultSpawn == spawn) {
            defaultSpawn = null;
            this.spawn = new(0, 0);
        }
        OnValidate();
    }

    public void Pause() {
        Time.timeScale = 0;
    }

    public bool isPaused() {
        return Time.timeScale == 0;
    }

    public void Unpause() {
        Time.timeScale = 1;
    }

    public RoomBound GetBounds() {
        return GetComponent<RoomBound>();
    }
}