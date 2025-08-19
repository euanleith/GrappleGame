using UnityEngine;
using UnityEngine.UIElements;

using static Utilities.Layer;

public class Health : MonoBehaviour
{
    public Rigidbody2D rb;
    public GrapplingGun grapple;
    public UIDocument ui;
    new public Camera camera;

    public int maxHealth;
    public int currentHealth;
    public float maxIFrames;
    public float currentIFrames;
    public Room spawnRoom;
    public Room room;
    public Transform enemies; // todo might need to split by room/level // todo can now get this from room

    public void Start()
    {
        currentHealth = maxHealth;   
        currentIFrames = 0f;
        CameraControls cameraControls = camera.GetComponent<CameraControls>();
        room = cameraControls.room;
        spawnRoom = room;
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
        if (IsDeathLayer(collision.gameObject.layer)) {
            bool alive = GetHit(1, Vector2.zero);
            if (alive) Retry(); // respawn at start of room for each platforming death
        } 
        else if (LayerEquals(collision.gameObject.layer, ENEMY)) {
            GetComponent<PlayerControls>().Stun(collisionNormal: collision.GetContact(0).normal);
            GetHit(collision.gameObject.GetComponent<Enemy>().combatController.GetDamage(), new Vector2(0, 1));
        } 
        else if (LayerEquals(collision.gameObject.layer, ENEMY_ATTACK)) {
            // todo why doesnt this stun?
            GetHit(collision.gameObject.GetComponent<Enemy>().combatController.GetDamage(), new Vector2(0, 1));
        }
    }

    public bool GetHit(int damage, Vector2 contactNormal) 
    {
        if (currentIFrames <= 0) // if not already invincible
        {
            if (damage >= currentHealth) 
            {
                Respawn();
                return false;
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
                return true;
            }
        }
        return false;
    }

    void ResetCamera()
    {
        CameraControls cameraControls = camera.GetComponent<CameraControls>();
        cameraControls.room = room;
        cameraControls.Move(false);
    }

    public void Respawn() {
        room = spawnRoom;
        room.CustomReset();
        ResetCamera();
        currentHealth = maxHealth;
        foreach (VisualElement heart in ui.rootVisualElement.ElementAt(0).Children()) {
            heart.visible = true;
        }
    }

    public void Retry() {
        room.CustomReset();
    }

    public void SetSpawnRoom(Room room) {
        spawnRoom = room;
    }
}