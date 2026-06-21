using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public class Draggable2D : MonoBehaviour
{
    [Header("Bounds (World Space)")]
    [SerializeField] private bool clampToBounds = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-8f, -4.5f);
    [SerializeField] private Vector2 maxBounds = new Vector2(8f, 4.5f);

    [Header("Drag Settings")]
    [SerializeField] private float dragSmoothing = 0f; // 0 = instant, >0 = lerp speed

    [Header("Drop Target Detection")]
    [SerializeField] private string targetTag = "DropZone";
    [SerializeField] private LayerMask dropZoneLayers;

    [Header("Hover Animation")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float hoverAnimDuration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    [Header("Debug")]
    [SerializeField] private bool showBoundsGizmo = true;
    [SerializeField] private Color gizmoColor = Color.cyan;

    private Camera mainCamera;
    private Vector3 dragOffset;
    private Vector3 originalPosition;
    private Vector3 baseScale;
    private bool isDragging;
    private Collider2D myCollider;
    private Tween scaleTween;

    public bool IsDragging => isDragging;

    [Header("Optional Events")]
    public UnityEvent OnClicked;
    public UnityEvent OnSucessfullDrop;

    private void Awake()
    {
        mainCamera = Camera.main;
        myCollider = GetComponent<Collider2D>();
        baseScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        if (isDragging) return;

        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale * hoverScaleMultiplier, hoverAnimDuration).SetEase(hoverEase);
    }

    private void OnMouseExit()
    {
        if (isDragging) return;

        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale, hoverAnimDuration).SetEase(hoverEase);
    }

    private void OnMouseDown()
    {
        originalPosition = transform.position;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;
        isDragging = true;

        OnClicked?.Invoke();

    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 targetPosition = GetMouseWorldPosition() + dragOffset;

        if (clampToBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        if (dragSmoothing > 0f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSmoothing * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        Collider2D droppedOn = GetOverlappingDropTarget();

        if (droppedOn != null)
        {
            OnSuccessfulDrop(droppedOn); // <-- this disables the object now
        }
        else
        {
            SnapBack();
        }

        // this still runs on a now-inactive object — DOTween will just no-op, harmless
        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale, hoverAnimDuration).SetEase(hoverEase);
    }

    private Collider2D GetOverlappingDropTarget()
    {
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(
            myCollider.bounds.center,
            myCollider.bounds.size,
            0f,
            dropZoneLayers
        );

        foreach (var col in overlaps)
        {
            if (col.gameObject == gameObject) continue; // skip self

            if (col.CompareTag(targetTag))
            {
                return col;
            }
        }

        return null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    private void SnapBack()
    {
        transform.position = originalPosition;
    }

    // Override this to add behavior on a successful drop (e.g. snap to slot, trigger event)
    protected virtual void OnSuccessfulDrop(Collider2D dropTarget)
    {
        Debug.Log($"{gameObject.name} dropped successfully on {dropTarget.gameObject.name}");

        OnSucessfullDrop?.Invoke();
        //LevelObserver.Instance.NotifyOnBoxTethered();
        //gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
    }

    private void OnDrawGizmos()
    {
        if (!showBoundsGizmo || !clampToBounds) return;

        Gizmos.color = gizmoColor;

        Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0f);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0f);
        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0f);
        Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0f);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}