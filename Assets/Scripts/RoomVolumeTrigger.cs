using UnityEngine;

// to use:
// (1) Attach this script to the roomVolume
// (2) Set the cameraAnchor to the RoomCameraAnchor child of the room
public class RoomVolumeTrigger : MonoBehaviour
{
    public Transform cameraAnchor;
    public RoomCamera roomCamera;
    public float railWidth = 0f;

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Trigger hit by: " + other.name + "    Tag: " + other.tag);
        if (!other.CompareTag("Player"))
            return;
        // Debug.Log("roomCamera: " + roomCamera + "    anchor: " + cameraAnchor);
        roomCamera.SetUpTransition(cameraAnchor, railWidth);
    }
}