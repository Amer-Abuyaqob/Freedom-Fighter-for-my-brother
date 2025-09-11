using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    [Header("Boundary Settings")]
    [SerializeField] bool isLeftBoundary = false;
    [SerializeField] bool isRightBoundary = false;
    [SerializeField] bool isTopBoundary = false;
    [SerializeField] bool isBottomBoundary = false;
    
    [Header("Visual Settings")]
    [SerializeField] bool showGizmo = true;
    [SerializeField] Color gizmoColor = Color.red;
    [SerializeField] float gizmoSize = 1f;

    void Awake()
    {
        // Set tag for automatic detection
        gameObject.tag = "Boundary";
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        Gizmos.color = gizmoColor;
        
        // Draw different shapes based on boundary type
        if (isLeftBoundary || isRightBoundary)
        {
            // Draw vertical line for left/right boundaries
            Vector3 top = transform.position + Vector3.up * gizmoSize;
            Vector3 bottom = transform.position + Vector3.down * gizmoSize;
            Gizmos.DrawLine(top, bottom);
            
            // Draw arrow pointing inward
            Vector3 arrowBase = transform.position;
            Vector3 arrowTip = arrowBase + (isLeftBoundary ? Vector3.right : Vector3.left) * gizmoSize * 0.5f;
            Gizmos.DrawLine(arrowBase, arrowTip);
        }
        
        if (isTopBoundary || isBottomBoundary)
        {
            // Draw horizontal line for top/bottom boundaries
            Vector3 left = transform.position + Vector3.left * gizmoSize;
            Vector3 right = transform.position + Vector3.right * gizmoSize;
            Gizmos.DrawLine(left, right);
            
            // Draw arrow pointing inward
            Vector3 arrowBase = transform.position;
            Vector3 arrowTip = arrowBase + (isTopBoundary ? Vector3.down : Vector3.up) * gizmoSize * 0.5f;
            Gizmos.DrawLine(arrowBase, arrowTip);
        }
    }

    // Public methods for camera controller to use
    public bool IsLeftBoundary() => isLeftBoundary;
    public bool IsRightBoundary() => isRightBoundary;
    public bool IsTopBoundary() => isTopBoundary;
    public bool IsBottomBoundary() => isBottomBoundary;
    
    public float GetBoundaryValue()
    {
        if (isLeftBoundary || isRightBoundary)
            return transform.position.x;
        else if (isTopBoundary || isBottomBoundary)
            return transform.position.y;
        return 0f;
    }
}
