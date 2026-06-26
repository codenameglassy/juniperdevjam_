using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrowOnGameFinished : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float torqueForce = 5f;
    [SerializeField] private ForceMode2D forceMode = ForceMode2D.Impulse;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (LevelObserver.Instance != null)
            LevelObserver.Instance.OnGameFinished += HandleGameFinished;
    }

    private void OnDestroy()
    {
        if (LevelObserver.Instance != null)
            LevelObserver.Instance.OnGameFinished -= HandleGameFinished;
    }

    private void HandleGameFinished()
    {
        ThrowInRandomDirection();
    }

    private void ThrowInRandomDirection()
    {
        Debug.Log("Object throw");
        Vector2 dir = Random.insideUnitCircle.normalized;
        float force = Random.Range(minForce, maxForce);
        float torque = Random.Range(-torqueForce, torqueForce);

        rb.AddForce(dir * force, forceMode);
        rb.AddTorque(torque, forceMode);
    }
}