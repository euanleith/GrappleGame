using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLayer : MonoBehaviour
{
    public int health = 1;

    public void OnHit(int damage) {
        health -= damage;
        if (health <= 0) {   
            Destroy(gameObject);
        }
        
    }
}
