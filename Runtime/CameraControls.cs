using UnityEngine;

using static Utilities.Vector;

public class CameraControls : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 10f;
    public Room room;
    public new Camera camera;
    private float height, width;
    public float offset;
    private bool isChangingRoom;
    public float settledThreshold = 0.01f;

    void Start()
    {
        camera.aspect = 16f / 9f;
        height = 2f * camera.orthographicSize;
        width = height * (16f / 9f);
        room.Enable();
        Move(false); // todo maybe this is running before player is moved
    }

    void Update()
    {
    }

    void LateUpdate() {
        Vector3 newPos = GetNewPosition();
        if (ShouldMove(newPos)) {
            float speed = GetSpeed(transform.position, newPos);
            Move(newPos);
            if (isChangingRoom) {
                if (speed > settledThreshold) {
                    if (!room.isPaused()) {
                        room.Pause();
                    }
                } else {
                    room.Unpause();
                    isChangingRoom = false;
                }
            }
        }
    }

    private Vector3 GetNewPosition(bool lerp = true) {
        Vector3 targetPos = player.position;
        Vector3 camBounds = new Vector3(
            Mathf.Clamp(targetPos.x, room.minPos.x + (width / 2), room.maxPos.x - (width / 2)),
            Mathf.Clamp(targetPos.y, room.minPos.y + (height / 2), room.maxPos.y - (height / 2)),
            0
        );

        return lerp ? Vector3.Lerp(transform.position, camBounds, smoothSpeed * Time.unscaledDeltaTime) : camBounds;
    }

    private bool ShouldMove(Vector3 newPos) {
        return transform.position != newPos;
    }

    private void Move(Vector3 pos) {
        transform.position = pos;
    }

    public void Move(bool lerp = true) {
        Move(GetNewPosition(lerp));
    }

    public void ChangeRoom(Room room, Vector2 spawn) {
        this.room = room;
        this.room.spawn = spawn;
        isChangingRoom = true;
    }

    public void CustomReset(Room room) {
        this.room = room;
        Move(false);
    }
}