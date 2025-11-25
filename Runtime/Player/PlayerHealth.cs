using UnityEngine;
using UnityEngine.UIElements;

using static Utilities.Layer;

public class PlayerHealth : MonoBehaviour {

    public UIDocument ui;
    new public Camera camera;

    [SerializeField] private Decay health = new(3);
    [SerializeField] private Decay iFrames = new Cooldown(1);

    // todo these should be in Player
    public Room spawnRoom;
    public Room room;

    private void Start() {
        health.Activate();
        CameraControls cameraControls = camera.GetComponent<CameraControls>();
        room = cameraControls.room;
        spawnRoom = room;
    }

    private void Update() {
        iFrames.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (IsDeathLayer(collision.gameObject.layer)) {
            bool alive = GetHit(1, Vector2.zero);
            if (alive) Retry();
        } else {
            if (LayerEquals(collision.gameObject.layer, ENEMY)) {
                GetComponent<Player>().Stun(collisionNormal: collision.GetContact(0).normal);
                GetHit(collision.gameObject.GetComponent<Enemy>().combatController.GetContactDamage(), new Vector2(0, 1));
            } else if (LayerEquals(collision.gameObject.layer, ENEMY_ATTACK)) {
            GetComponent<Player>().Stun(collisionNormal: collision.GetContact(0).normal);
                GetHit(collision.gameObject.GetComponent<Enemy>().combatController.GetAttackDamage(), new Vector2(0, 1));
            }
        }
    }

    public int GetCurrent() {
        return (int) health.GetCurrent();
    }

    public bool GetHit(int damage, Vector2 contactNormal) {
        if (!iFrames.IsActive()) {
            if (damage >= health.GetCurrent()) {
                Respawn();
                return false;
            }
            else {
                GetComponent<PlayerMovement>().OnHit(contactNormal);
                for (int i = 0; i < damage; i++) {
                    health.Update();
                    ui.rootVisualElement
                        .ElementAt(0)
                        .ElementAt((int) health.GetCurrent())
                        .visible = false;
                }
                iFrames.Activate();
                return true;
            }
        }
        return false;
    }

    public void Respawn() {
        room = spawnRoom;
        room.CustomReset();
        camera.GetComponent<CameraControls>().CustomReset(room);
        health.Activate();
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

    public void DeactivateIFrames() {
        iFrames.Deactivate();
    }
}