using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public float movementWeight;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(Camera.main.velocity * Time.deltaTime / (GetComponent<SpriteRenderer>().sortingOrder * movementWeight), Camera.main.transform); 
    }
}
