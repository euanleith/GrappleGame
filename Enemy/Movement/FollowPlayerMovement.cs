using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerMovement : Movement
{
    Transform player;
    public Vector2 maxDistanceFromPlayer;
    Vector2 minDistanceFromSurface = new Vector2(0.5f, 0.5f);
    int layerMask;
    float plsStopWiggling = 0.01f;
    public GameObject platforms; // todo make this transform

    public void Start() {
        player = GetComponentInParent<Enemy>().player;
        layerMask = (1 << 8) & (1 << 17); // Enemy & EnemyInvulnerable todo add variable to inspector
    }

    public override Vector2 Move(ref Vector2 direction, Transform transform, Vector2 currentDecelVelocity, Vector2 collisionNormal) 
    {
        
        Vector2 newVel = Vector2.zero;
        if (IsMinDistanceFromSurfaceX(transform.position, transform)) newVel.x = 0;
        else if (transform.position.x < player.position.x - maxDistanceFromPlayer.x) newVel.x = speed.x;
        else if (transform.position.x > player.position.x + maxDistanceFromPlayer.x) newVel.x = -speed.x;
        if (IsMinDistanceFromSurfaceY(transform.position, transform)) newVel.y = 0;
        else if (transform.position.y < player.position.y - maxDistanceFromPlayer.y - plsStopWiggling) newVel.y = speed.y;
        else if (transform.position.y > player.position.y + maxDistanceFromPlayer.y + plsStopWiggling) newVel.y = -speed.y;
        direction = newVel;
        return new Vector2(transform.position.x + newVel.x * Time.deltaTime, transform.position.y + newVel.y * Time.deltaTime);
    }
    
    // todo raycast doesnt work on corners, might be better (and faster) to iterate through the platforms in the room and check distance
    //  could raycast diagonally from corners of enemy?
    // todo just awful code, everywhere :(
    private bool IsMinDistanceFromSurfaceX(Vector2 position, Transform transform) {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(position.x + transform.localScale.x/2, position.y), Vector2.right, minDistanceFromSurface.x + 0.01f, ~layerMask);
        if (hit && CloseEnough(hit.distance, minDistanceFromSurface.x,0.02f)) return true;
        //if (hit && CloseEnough(hit.point.x - (position.x + rb.transform.localScale.x/2), minDistanceFromSurface.x,0.1f)) return true;
        hit = Physics2D.Raycast(new Vector2(position.x - transform.localScale.x/2, position.y), Vector2.left, minDistanceFromSurface.x + 0.01f, ~layerMask);
        //if (hit) Debug.Log(hit.distance + " " +  minDistanceFromSurface.x);
        if (hit && CloseEnough(hit.distance, minDistanceFromSurface.x,0.02f)) return true;
        //if (hit && CloseEnough((position.x - rb.transform.localScale.x/2) - hit.point.x, minDistanceFromSurface.x,0.1f)) return true;
        return false;
    }

    bool IsMinDistanceFromSurfaceX2(Vector2 position, Transform transform) {
        foreach (Transform platform in platforms.transform) {
            if ((CloseEnough(platform.position.x + platform.localScale.x/2, position.x - transform.localScale.x/2 - minDistanceFromSurface.x, 0.05f) ||
                CloseEnough(platform.position.x - platform.localScale.x/2, position.x + transform.localScale.x/2 + minDistanceFromSurface.x, 0.05f)) &&
                platform.position.y + platform.localScale.y/2 > position.y - transform.localScale.y/2 - minDistanceFromSurface.y &&
                platform.position.y - platform.localScale.y/2 < position.y + transform.localScale.y/2 + minDistanceFromSurface.y) return true;
        }
        return false;
    }
    
    bool IsMinDistanceFromSurfaceY2(Vector2 position, Transform transform) {
        foreach (Transform platform in platforms.transform) {
            if (platform.position.x + platform.localScale.x/2 > position.x - transform.localScale.x/2 - minDistanceFromSurface.x &&
                platform.position.x - platform.localScale.x/2 < position.x + transform.localScale.x/2 + minDistanceFromSurface.x &&
                (CloseEnough(platform.position.y + platform.localScale.y/2, position.y - transform.localScale.y/2 - minDistanceFromSurface.y, 0.05f) || // todo i think its using distance from below, so need to use direction
                CloseEnough(platform.position.y - platform.localScale.y/2, position.y + transform.localScale.y/2 + minDistanceFromSurface.y, 0.05f))) return true;
        }
        return false;
    }

    private bool IsMinDistanceFromSurfaceY(Vector2 position, Transform transform) {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.down, minDistanceFromSurface.y + 0.01f, ~layerMask);
        if (hit && CloseEnough(hit.distance, minDistanceFromSurface.y,0.02f)) return true;
        //if (hit && CloseEnough((position.y - rb.transform.localScale.y/2) - hit.point.y, minDistanceFromSurface.y,0.01f)) return true;
        hit = Physics2D.Raycast(new Vector2(position.x, position.y + transform.localScale.y/2), Vector2.up, minDistanceFromSurface.y + 0.01f, ~layerMask);
        if (hit && CloseEnough(hit.distance, minDistanceFromSurface.y,0.02f)) return true;
        //if (hit && CloseEnough(hit.point.y - (position.y + rb.transform.localScale.y/2), minDistanceFromSurface.y,0.01f)) return true;
        return false;
    }

    // todo should take into account direction instead (e.g. if moving left and x < platform x + minDistanceFromSurface.x)
    //  do i even need to do this? or just do > or < instead of equals? 
    //      because then it wouldnt work when coming from below/above/behind instead of head on, so use direction instead
    // idk what im saying anymore i need a break
    // todo JUST GIVE UP SILLY, ENEMIES DONT EVEN MOVE LIKE THAT
    //  or do they... :(
    // todo rename ApproxEquals
    bool CloseEnough(float f1, float f2, float maxDif) {
        return Mathf.Abs(f2 - f1) <= maxDif;
    }

    public override Vector2 OnCollision(Collision2D collision) {
        Vector2 normal = collision.GetContact(0).normal;
        if (normal.y == 0) normal.y = 1; 
        if (normal.x == 0) normal.x = 1;
        return normal * speed;
    }

    public override Vector2 OnHit(Collider2D collider, Transform transform) {
        Vector2 relativePosition = Vector2.zero;
        relativePosition.x = collider.transform.position.x > transform.position.x ? -1 : 1;
        relativePosition.y = collider.transform.position.y > transform.position.y ? -1 : 1;
        return relativePosition * speed;
    }
}
