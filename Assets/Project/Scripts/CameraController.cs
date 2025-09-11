using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target; // Player to follow
    
    [Header("Camera Settings")]
    [SerializeField] float followSpeed = 2f;
    [SerializeField] Vector3 offset = new Vector3(0, 2, -10);
    [SerializeField] bool smoothFollow = true;
    
    [Header("Level Boundaries")]
    [SerializeField] float leftBoundary = -20f;
    [SerializeField] float rightBoundary = 20f;
    [SerializeField] float topBoundary = 10f;
    [SerializeField] float bottomBoundary = -5f;
    
    [Header("Camera Limits")]
    [SerializeField] bool useLevelBoundaries = true;
    [SerializeField] float cameraHalfWidth = 8f; // Half of camera's orthographic size * aspect ratio
    [SerializeField] float cameraHalfHeight = 5f; // Half of camera's orthographic size
    
    [Header("Debug")]
    [SerializeField] bool showBoundaries = true;
    [SerializeField] Color boundaryColor = Color.red;

    Camera cam;
    Vector3 velocity = Vector3.zero;

    void Awake()
    {
        cam = GetComponent<Camera>();
        
        // Auto-find player if not assigned
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
        
        // Calculate camera dimensions based on orthographic size
        if (cam != null && cam.orthographic)
        {
            cameraHalfHeight = cam.orthographicSize;
            cameraHalfWidth = cameraHalfHeight * cam.aspect;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Apply level boundaries if enabled
        if (useLevelBoundaries)
        {
            desiredPosition = ApplyBoundaries(desiredPosition);
        }
        
        // Move camera to desired position
        if (smoothFollow)
        {
            // Smooth follow with damping
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / followSpeed);
        }
        else
        {
            // Instant follow
            transform.position = desiredPosition;
        }
    }

    Vector3 ApplyBoundaries(Vector3 desiredPosition)
    {
        Vector3 boundedPosition = desiredPosition;
        
        // Calculate camera bounds
        float cameraLeft = boundedPosition.x - cameraHalfWidth;
        float cameraRight = boundedPosition.x + cameraHalfWidth;
        float cameraTop = boundedPosition.y + cameraHalfHeight;
        float cameraBottom = boundedPosition.y - cameraHalfHeight;
        
        // Apply horizontal boundaries
        if (cameraLeft < leftBoundary)
        {
            boundedPosition.x = leftBoundary + cameraHalfWidth;
        }
        else if (cameraRight > rightBoundary)
        {
            boundedPosition.x = rightBoundary - cameraHalfWidth;
        }
        
        // Apply vertical boundaries
        if (cameraTop > topBoundary)
        {
            boundedPosition.y = topBoundary - cameraHalfHeight;
        }
        else if (cameraBottom < bottomBoundary)
        {
            boundedPosition.y = bottomBoundary + cameraHalfHeight;
        }
        
        return boundedPosition;
    }

    // Public methods to update boundaries at runtime
    public void SetBoundaries(float left, float right, float top, float bottom)
    {
        leftBoundary = left;
        rightBoundary = right;
        topBoundary = top;
        bottomBoundary = bottom;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetFollowSpeed(float speed)
    {
        followSpeed = Mathf.Max(0.1f, speed);
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    public void EnableBoundaries(bool enable)
    {
        useLevelBoundaries = enable;
    }

    // Method to automatically calculate boundaries from level objects
    public void AutoCalculateBoundaries()
    {
        // Find all objects with "Boundary" tag or similar
        var boundaryObjects = GameObject.FindGameObjectsWithTag("Boundary");
        
        if (boundaryObjects.Length > 0)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            
            foreach (var boundary in boundaryObjects)
            {
                Vector3 pos = boundary.transform.position;
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);
            }
            
            SetBoundaries(minX, maxX, maxY, minY);
        }
    }

    void OnDrawGizmos()
    {
        if (!showBoundaries) return;
        
        Gizmos.color = boundaryColor;
        
        // Draw level boundaries
        Vector3 topLeft = new Vector3(leftBoundary, topBoundary, 0);
        Vector3 topRight = new Vector3(rightBoundary, topBoundary, 0);
        Vector3 bottomLeft = new Vector3(leftBoundary, bottomBoundary, 0);
        Vector3 bottomRight = new Vector3(rightBoundary, bottomBoundary, 0);
        
        // Draw boundary rectangle
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
        
        // Draw camera bounds if target is assigned
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 camPos = target.position + offset;
            Vector3 camCenter = camPos;
            
            // Draw camera view bounds
            Vector3 camTopLeft = new Vector3(camCenter.x - cameraHalfWidth, camCenter.y + cameraHalfHeight, camCenter.z);
            Vector3 camTopRight = new Vector3(camCenter.x + cameraHalfWidth, camCenter.y + cameraHalfHeight, camCenter.z);
            Vector3 camBottomLeft = new Vector3(camCenter.x - cameraHalfWidth, camCenter.y - cameraHalfHeight, camCenter.z);
            Vector3 camBottomRight = new Vector3(camCenter.x + cameraHalfWidth, camCenter.y - cameraHalfHeight, camCenter.z);
            
            Gizmos.DrawLine(camTopLeft, camTopRight);
            Gizmos.DrawLine(camTopRight, camBottomRight);
            Gizmos.DrawLine(camBottomRight, camBottomLeft);
            Gizmos.DrawLine(camBottomLeft, camTopLeft);
        }
    }

    void OnValidate()
    {
        // Ensure boundaries make sense
        if (leftBoundary > rightBoundary)
        {
            float temp = leftBoundary;
            leftBoundary = rightBoundary;
            rightBoundary = temp;
        }
        
        if (bottomBoundary > topBoundary)
        {
            float temp = bottomBoundary;
            bottomBoundary = topBoundary;
            topBoundary = temp;
        }
        
        // Ensure positive values
        followSpeed = Mathf.Max(0.1f, followSpeed);
        cameraHalfWidth = Mathf.Max(0.1f, cameraHalfWidth);
        cameraHalfHeight = Mathf.Max(0.1f, cameraHalfHeight);
    }
}
