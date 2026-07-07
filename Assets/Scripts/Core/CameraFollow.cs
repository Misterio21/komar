using UnityEngine;

/// <summary>
/// Jednoduchá kamera sledující hráče (funguje pro člověka i komára).
/// Umísti na Main Camera. Cíl se nastaví automaticky z PlayerSpawneru.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -6f);
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private bool lookAtTarget = true;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        if (lookAtTarget)
        {
            transform.LookAt(target.position + Vector3.up * 1f);
        }
    }
}
