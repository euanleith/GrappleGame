using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnHit : MonoBehaviour
{
    // todo reset on death

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log("hi");
        if (collider.gameObject.layer == 2) { // todo player attack - make it its own layer
            anim.enabled = true;
            GetComponent<SpriteRenderer>().color = Color.black;
            //anim.Play(0);
        }
    }
}
