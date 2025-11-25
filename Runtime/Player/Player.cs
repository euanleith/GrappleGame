using UnityEngine;

public class Player : MonoBehaviour {

    private GrapplingGun grapple;
    private PlayerMovement movementController; // todo can this be instantiated here and no longer extend MonoBehaviour?

    [SerializeField] private Decay stun = new Cooldown(1);

    private void Start() {
        movementController = GetComponent<PlayerMovement>();
        grapple = GetComponentInChildren<GrapplingGun>();
        stun.SetOnFinished(FinishStun);
    }

    void Update() {
        stun.Update();
    }

    public bool IsStunned() { 
        return stun.IsActive();
    }

    public void FinishStun() {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Stun(float? duration = null, Vector2 collisionNormal = new Vector2()) {
        stun.Activate(duration);
        GetComponent<SpriteRenderer>().color = Color.cyan;
        movementController.Stun(collisionNormal);
        grapple.StopGrappling(); // todo currently this is causing no grapple animation to play
    }
}