using UnityEngine;

public class Player : MonoBehaviour {

    private GrapplingGun grapple;
    private PlayerMovement movementController; // todo can this be instantiated here and no longer extend MonoBehaviour?

    private bool stunned = false;
    public const float stunDuration = 1f;
    private float stunCnt = 0f;

    private void Start() {
        movementController = GetComponent<PlayerMovement>();
        grapple = GetComponentInChildren<GrapplingGun>();
    }

    void Update() {
        if (stunCnt > 0) {
            stunCnt -= Time.deltaTime;
            return;
        } else if (stunned) FinishStun();
    }

    public bool IsStunned() { 
        return stunned;
    }

    public void FinishStun() {
        stunned = false;
        stunCnt = 0;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Stun(float duration = stunDuration, Vector2 collisionNormal = new Vector2()) {
        stunned = true;
        stunCnt = duration;
        GetComponent<SpriteRenderer>().color = Color.cyan;
        movementController.Stun(collisionNormal);
        grapple.StopGrappling(); // todo currently this is causing no grapple animation to play
    }
}