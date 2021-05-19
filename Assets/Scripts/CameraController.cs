using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Room camera
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    // Follow Player
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    private float lookAhead;

    // Update is called once per frame
    private void Update() {
        /*
            //Room camera
            // smoothDamp(current pos, destination, velocity, speed of movement)

            transform.position = Vector3.SmoothDamp(transform.position,
                new Vector3(currentPosX, transform.position.y, transform.position.z),
                ref velocity,
                speed);
        */

        // Follow Player
        transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
        // aheadDistance * eaither 1 or -1. depending on which direction player is looking
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    }

    public void MoveToNewRoom(Transform _newRoom) {
        // when in new room the camera will move to the center of the room
        currentPosX = _newRoom.position.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
