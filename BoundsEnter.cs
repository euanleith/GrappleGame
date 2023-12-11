using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsEnter : MonoBehaviour 
{
    // todo to save computer cost, maybe store next rooms in bounds, then enable them when moving into them (they should be disabled by default)
    //  then can also have new bounds be found in the room's data, instead of storing under LeftBound/etc.
    public CameraControls camControls;

    // Start is called before the first frame update
    void Start()
    {
        camControls = Camera.main.GetComponent<CameraControls>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("hi");
        if (collision.gameObject.name == "Player") {
            camControls.minPos = transform.Find("MinPos");
            camControls.maxPos = transform.Find("MaxPos");
            collision.transform.position = transform.Find("NewSpawn").position; // todo just x/y when entering vert/horiz bounds respectively
            collision.gameObject.GetComponent<Health>().roomSpawnPoint = transform.Find("NewSpawn");
        }
    }
}
