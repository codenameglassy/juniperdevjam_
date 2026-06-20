using UnityEngine;

public class HandFollowMouse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [Header("Movement")]
    [SerializeField] private float followSpeed = 20f; // higher = snappier
    [SerializeField] private bool smoothFollow = true;

    [Header("Boundary (world space box)")]
    [SerializeField] private Vector2 boundsMin = new Vector2(-3f, -2f);
    [SerializeField] private Vector2 boundsMax = new Vector2(3f, 2f);

    private Vector3 targetPosition;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        targetPosition = ClampToBox(mouseWorldPos);

        if (smoothFollow)
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        else
            transform.position = targetPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    private Vector3 ClampToBox(Vector3 point)
    {
        Vector3 clamped = point;
        clamped.x = Mathf.Clamp(point.x, boundsMin.x, boundsMax.x);
        clamped.y = Mathf.Clamp(point.y, boundsMin.y, boundsMax.y);
        clamped.z = transform.position.z;
        return clamped;
    }

    // Visualize the bounds in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((boundsMin.x + boundsMax.x) / 2f, (boundsMin.y + boundsMax.y) / 2f, 0f);
        Vector3 size = new Vector3(boundsMax.x - boundsMin.x, boundsMax.y - boundsMin.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}