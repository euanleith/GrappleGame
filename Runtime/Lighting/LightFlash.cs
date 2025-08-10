using UnityEngine;

public class LightFlash : MonoBehaviour
{
    public float startOffset = -1;
    public float onDuration = 0.4f;
    public float offDuration = 5f;

    void Start()
    {
        gameObject.SetActive(false);
        if (startOffset == -1) {
            startOffset = Random.Range(0f, 2f);
        }
        Invoke("FlashOn", startOffset);
    }

    void FlashOn() {
        gameObject.SetActive(true);
        Invoke("FlashOff", onDuration);
    }

    void FlashOff() {
        gameObject.SetActive(false);
        Invoke("FlashOn", offDuration);
    }
}
