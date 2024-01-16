using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    public Rigidbody2D rb;
    public GrapplingGun grapple;
    public UIDocument ui;
    public Camera camera;

    public int maxHealth;
    public int currentHealth;
    public float maxIFrames;
    public float currentIFrames;
    public Transform spawnPoint;
    public Transform roomSpawnPoint;
    public Transform spawnMinPos;
    public Transform spawnMaxPos;
    public Transform enemies; // todo might need to split by room/level

    void Start()
    {
        currentHealth = maxHealth;   
        currentIFrames = maxIFrames;
        CameraControls cameraControls = camera.GetComponent<CameraControls>();
        spawnMinPos = cameraControls.minPos;
        spawnMaxPos = cameraControls.maxPos;
        roomSpawnPoint = spawnPoint;
        rb.position = spawnPoint.position;
        // todo set spawn point & bounds from camera
    }

    void Update()
    {
        if (currentIFrames >= 0) {
            currentIFrames -= Time.deltaTime;
        }
    }

    // todo get knocked back &/ stunned?
    // todo here only processes collisions with player (not attack hitboxes), since those are onTrigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        // todo for each contact?
        switch (collision.gameObject.layer) {
            case 6: // todo "Death" or whatever; make these inspector variables
            case 7:
            case 15:
                GetHit(1, Vector2.zero); 
                ResetRoom(roomSpawnPoint); // respawn at start of room for each platforming death
                break;
            case 8:
                GetHit(collision.gameObject.GetComponent<Enemy>().combatController.GetDamage(), new Vector2(0, 1));
                GetComponent<PlayerControls>().Stun(collisionNormal: collision.GetContact(0).normal);
                break;
            case 11:
                GetHit(collision.gameObject.GetComponent<Enemy>().combatController.GetDamage(), new Vector2(0, 1));
                break;
        }
    }

    public void GetHit(int damage, Vector2 contactNormal) 
    {
        if (currentIFrames <= 0) // if not already invincible
        {
            if (damage > currentHealth) 
            {
                Respawn();
                
            }
            else 
            {
                GetComponent<PlayerControls>().Bounce(contactNormal); // todo how do other games do this?
                grapple.StopGrappling();
                for (int i = 0; i < damage; i++) {
                    currentHealth--;
                    ui.rootVisualElement
                        .ElementAt(0)
                        .ElementAt(currentHealth)
                        .visible = false;
                }
                currentIFrames = maxIFrames;
            }
            
        }
    }

    void ResetCamera()
    {
        CameraControls camControls = Camera.main.gameObject.GetComponent<CameraControls>();
        camControls.minPos = spawnMinPos;
        camControls.maxPos = spawnMaxPos;
        roomSpawnPoint = spawnPoint;
    }

    void ResetRoom(Transform spawn) {
        rb.position = spawn.position;
        rb.velocity = new Vector2(0, 0);
        grapple.grappleRope.enabled = false;
        Transform enemies = spawn.parent.parent.parent.Find("Enemies"); // todo there's probably a better way to get enemies in current room. maybe after adding 'only load enemies in current room'? should just have a currentRoomEnemies variable? But how would I get that? Should I have a Room object? That might also make setting up new player spawns in editor easier
        foreach (Transform enemy in enemies) {
            if (enemy.gameObject.activeSelf) {
                Transform controller = enemy.Find("Controller");
                controller.GetComponent<Enemy>().Reset();
            }
        }
    }

    void Respawn() {
        ResetRoom(spawnPoint);
        ResetCamera();
        currentHealth = maxHealth;
        foreach (VisualElement heart in ui.rootVisualElement.ElementAt(0).Children()) {
            heart.visible = true;
        }
    }
}
