using UnityEngine;

public class LightFlash : MonoBehaviour
{
    public float onTime = 0.5f;
    public float offTime = 2f;
    public float startOffset = 0f;

    void Start()
    {
        Invoke("FlashOn", startOffset);
    }

    void FlashOn() {
        gameObject.SetActive(true);
        Invoke("FlashOff", onTime);
    }

    void FlashOff() {
        gameObject.SetActive(false);
        Invoke("FlashOn", offTime);
    }
}
