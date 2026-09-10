using UnityEngine;

/// We can edit this camera system in the future if we want to add stuff like pulling the camera towards the player when
/// something is in the way, but for now this should work fine.
///
/// To use (assuming already attatched to the maincamera):
/// Create empty gameObjects as children of the room as follows:
///     RoomVolume (an empty gameobject with a box collider set to trigger, and a rigidbody with gravity off and kinematic on, sized to fit the room)
///     RoomCameraAnchor (an empty gameobject positioned where you want the camera to be in that room, with the desired rotation)
/// (2) Next, add a RoomCameraTrigger component to the RoomVolume and set the RoomCameraAnchor as its anchor.
public class RoomCamera : MonoBehaviour
{
    public Transform player;
    public Transform startingAnchor;

    [Header("Pull Settings")]
    public float pullStrengthBetweenZeroAndOne = 0.35f;
    public float followSpeed = 3f;
    public float maxPullAngle = 25f;

    [Header("Rail Settings")]
    public float railFollowSpeed = 4f;

    public float transitionSpeed = 6f;


    private Transform _currentAnchor;
    private Transform _targetAnchor;
    private float _currentRailWidth = 0f;

    private Quaternion _baseRotation;
    private Quaternion _currentRotation;

    private Vector3 _currentRailPosition;

    private bool _transitioning = false;
    private float _transitionProgress = 0f;
    private Vector3 _transitionStartPos;
    private Quaternion _transitionStartRot;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (_currentAnchor == null)
        {
            Debug.LogWarning("RoomCamera: No initial anchor set. Camera will not function until SetUpTransition is called.");
        }

        if (startingAnchor != null)
        TransitionToRoomInstant(startingAnchor);
    }
    private void LateUpdate()
    {
        if (_currentAnchor == null) return;

        if (_transitioning)
            HandleTransition();
        else
            HandleFollow();
    }
    public void SetUpTransition(Transform newAnchor, float railWidth = 0f)
    {
        if (newAnchor == _currentAnchor) return;
        _targetAnchor = newAnchor;
        _currentRailWidth = railWidth;
        _transitionStartPos = transform.position;
        _transitionStartRot = transform.rotation;
        _transitionProgress = 0f;
        _transitioning = true;
    }

    public void TransitionToRoomInstant(Transform anchor) //use this for respawn, initial camera setup, etc
    {

        _currentAnchor = anchor;
        _targetAnchor = anchor;
        _transitioning = false;

        transform.position = anchor.position;
        _currentRailPosition = anchor.position;
        _baseRotation = anchor.rotation;
        _currentRotation = _baseRotation;

        transform.rotation = _baseRotation;
    }

    private void HandleTransition()
    {
        _transitionProgress += Time.deltaTime * transitionSpeed;
        float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp(_transitionProgress, 0f, 1f));

        transform.position = Vector3.Lerp(_transitionStartPos, _targetAnchor.position, t);
        transform.rotation = Quaternion.Slerp(_transitionStartRot, _targetAnchor.rotation, t);

        if (_transitionProgress >= 1f)
        {
            _currentAnchor = _targetAnchor;
            transform.position = _currentAnchor.position;
            _baseRotation = _currentAnchor.rotation;

            _currentRotation = transform.rotation;

            _transitioning = false;
        }
    }



    private void HandleFollow()
    {

        Vector3 targetRailPos = ComputeRailPosition();
        _currentRailPosition = Vector3.Lerp(_currentRailPosition, targetRailPos, railFollowSpeed * Time.deltaTime);
        transform.position = _currentRailPosition;


        Quaternion desiredRotation = ComputeDesiredRotation();

        // slerp to mimic little nightmares' camera 'lag'(?) behind players
        _currentRotation = Quaternion.Slerp(_currentRotation, desiredRotation, followSpeed * Time.deltaTime);
        transform.rotation = _currentRotation;
    }

    private Vector3 ComputeRailPosition()
    {
        if (player == null || _currentRailWidth <= 0f)
            return _currentAnchor.position;

        // Project the player's position onto the anchor's local X axis (left/right)
        Vector3 toPlayer = player.position - _currentAnchor.position;
        float lateralOffset = Vector3.Dot(toPlayer, _currentAnchor.right);
        lateralOffset = Mathf.Clamp(lateralOffset, -_currentRailWidth, _currentRailWidth);

        return _currentAnchor.position + _currentAnchor.right * lateralOffset;
    }

    private Quaternion ComputeDesiredRotation()
    {
        if (player == null) return _baseRotation;

        Vector3 toPlayer = (player.position - transform.position).normalized;
        if (toPlayer == Vector3.zero)
            return _baseRotation;

        Quaternion lookAtPlayer = Quaternion.LookRotation(toPlayer);

        //looks towards the player but not directly at it to mimic camera from Little Nightmares (visibile in discord)
        //think of this as a magnet on the end of a spring where the player is another magnet
        Quaternion pulled = Quaternion.Slerp(_baseRotation, lookAtPlayer, pullStrengthBetweenZeroAndOne);

        float angle = Quaternion.Angle(_baseRotation, pulled);
        if (angle > maxPullAngle)
        {
            pulled = Quaternion.Slerp(_baseRotation, pulled, maxPullAngle / angle);
        }

        return pulled;
    }
}