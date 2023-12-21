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
        if (currentHealth <= 0) {
            // respawning when at 0 health (from combat / platforming)
            Respawn(spawnPoint.position);
            ResetCamera();
            currentHealth = maxHealth;
            foreach (VisualElement heart in ui.rootVisualElement.ElementAt(0).Children()) {
                heart.visible = true;
            }
            // todo add this back after adding room logic to enemies
            //for (int i = 0; i < enemies.childCount; i++) 
            //{
            //    enemies.GetChild(i).gameObject.SetActive(true);
            //}
        }
        if (currentIFrames != maxIFrames) {
            currentIFrames -= Time.deltaTime;
            if (currentIFrames <= 0) {
                currentIFrames = maxIFrames;
            }
        }
    }

    // todo get knocked back &/ stunned?
    // todo here only processes collisions with player (not attack hitboxes), since those are onTrigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.layer) {
            case 6: // todo "Death" or whatever
            case 7:
                GetHit(1, Vector2.zero); 
                Respawn(roomSpawnPoint.position); // respawn at start of room for each platforming death
                break;
            case 11:
                GetHit(collision.gameObject.GetComponent<Enemy>().combatController.GetDamage(), new Vector2(0, 1));
                break;
            case 8:
                GetHit(collision.gameObject.GetComponent<Enemy>().combatController.GetDamage(), new Vector2(0, 1));
                break;
        }
    }

    public void GetHit(int damage, Vector2 contactNormal) 
    {
        if (currentIFrames == maxIFrames) // if not already invincible
        {
            if (damage > currentHealth) 
            {
                currentHealth = 0;
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

    void Respawn(Vector2 position) {
        rb.position = position;
        rb.velocity = new Vector2(0, 0);
        grapple.grappleRope.enabled = false;
    }
}
