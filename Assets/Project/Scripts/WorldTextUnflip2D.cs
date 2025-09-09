using UnityEngine;

public class WorldTextUnflip2D : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] bool keepUpright = true; // reset rotation in 2D

    Transform parentTransform;
    Vector3 desiredWorldScale;

    void Awake()
    {
        parentTransform = transform.parent;
        desiredWorldScale = transform.lossyScale; // keep text size constant in world space
    }

    void LateUpdate()
    {
        if (parentTransform == null) return;

        Vector3 parentLossy = parentTransform.lossyScale;
        float absX = Mathf.Abs(parentLossy.x);
        float absY = Mathf.Abs(parentLossy.y);
        float absZ = Mathf.Abs(parentLossy.z);

        // Compute local scale that results in a constant, non-mirrored world scale
        Vector3 local = new Vector3(
            absX > 1e-6f ? desiredWorldScale.x / absX : desiredWorldScale.x,
            absY > 1e-6f ? desiredWorldScale.y / absY : desiredWorldScale.y,
            absZ > 1e-6f ? desiredWorldScale.z / absZ : desiredWorldScale.z
        );

        // Cancel parent's X sign so text never mirrors
        float parentSignX = Mathf.Sign(parentLossy.x);
        if (parentSignX == 0f) parentSignX = 1f;
        local.x *= parentSignX;

        transform.localScale = local;

        if (keepUpright)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}


