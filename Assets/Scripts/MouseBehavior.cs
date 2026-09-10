using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    [Header("Mouse Mode Settings")]
    public float mouseControlRadiusPixels = 500f;
    public float modeSwitchBufferPixels = 24f;
    public float rotationSpeed = 20f;

    private Camera cam;
    private PlayerMovement playerMovement;
    private bool useMouseFacing;

    void Start()
    {
        cam = Camera.main;
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (PauseMenu.Instance != null && PauseMenu.Instance.isPaused)
            return;

        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            return;

        Plane lookPlane = new Plane(Vector3.up, transform.position);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // ^^ basically makes a oplane parallel to the ground
        // (cause trying to use the actual world collides with objects)
        // Also you would have to manually make every floor plane on the floor layer otherwise.

        if (lookPlane.Raycast(ray, out float enter))
        {
            Vector3 targetPoint = ray.GetPoint(enter);
            Vector3 lookDir = targetPoint - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude < 0.0001f)
                return;

            Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
            float mouseDistancePixels = Vector2.Distance(
                new Vector2(screenPos.x, screenPos.y),
                new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            float enterMouseMode = Mathf.Max(0f, mouseControlRadiusPixels - modeSwitchBufferPixels);
            float exitMouseMode = mouseControlRadiusPixels + modeSwitchBufferPixels;

            // stops rapid flipping between modes on boundaries
            if (!useMouseFacing && mouseDistancePixels <= enterMouseMode)
                useMouseFacing = true;
            else if (useMouseFacing && mouseDistancePixels >= exitMouseMode)
                useMouseFacing = false;

            Vector3 desiredDirection;

            if (useMouseFacing)
                desiredDirection = lookDir.normalized;

            else if (playerMovement != null && playerMovement.CurrentMoveDirection.sqrMagnitude > 0.01f)
                desiredDirection = playerMovement.CurrentMoveDirection.normalized;

            else
                return;

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

}