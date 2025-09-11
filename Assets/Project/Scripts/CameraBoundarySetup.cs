using UnityEngine;

public class CameraBoundarySetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] bool autoSetupOnStart = true;
    [SerializeField] float boundaryMargin = 2f; // Extra space beyond level bounds
    
    [Header("Manual Boundaries")]
    [SerializeField] float leftBound = -25f;
    [SerializeField] float rightBound = 25f;
    [SerializeField] float topBound = 10f;
    [SerializeField] float bottomBound = -5f;
    
    [Header("References")]
    [SerializeField] CameraController cameraController;

    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCameraBoundaries();
        }
    }

    public void SetupCameraBoundaries()
    {
        // Find camera controller if not assigned
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
        }
        
        if (cameraController == null)
        {
            Debug.LogWarning("CameraBoundarySetup: No CameraController found!");
            return;
        }

        // Try to auto-calculate boundaries first
        if (TryAutoCalculateBoundaries())
        {
            Debug.Log("CameraBoundarySetup: Auto-calculated boundaries from level objects");
        }
        else
        {
            // Use manual boundaries
            cameraController.SetBoundaries(leftBound, rightBound, topBound, bottomBound);
            Debug.Log("CameraBoundarySetup: Using manual boundaries");
        }
    }

    bool TryAutoCalculateBoundaries()
    {
        // Method 1: Look for objects with "Boundary" tag
        var boundaryObjects = GameObject.FindGameObjectsWithTag("Boundary");
        if (boundaryObjects.Length >= 2) // Need at least 2 boundaries
        {
            CalculateFromBoundaryObjects(boundaryObjects);
            return true;
        }

        // Method 2: Look for objects with "Ground" tag (common in 2D games)
        var groundObjects = GameObject.FindGameObjectsWithTag("Ground");
        if (groundObjects.Length > 0)
        {
            CalculateFromGroundObjects(groundObjects);
            return true;
        }

        // Method 3: Look for objects with "Platform" tag
        var platformObjects = GameObject.FindGameObjectsWithTag("Platform");
        if (platformObjects.Length > 0)
        {
            CalculateFromPlatformObjects(platformObjects);
            return true;
        }

        return false;
    }

    void CalculateFromBoundaryObjects(GameObject[] boundaries)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var boundary in boundaries)
        {
            Vector3 pos = boundary.transform.position;
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxY = Mathf.Max(maxY, pos.y);
        }

        // Apply margin
        cameraController.SetBoundaries(
            minX - boundaryMargin,
            maxX + boundaryMargin,
            maxY + boundaryMargin,
            minY - boundaryMargin
        );
    }

    void CalculateFromGroundObjects(GameObject[] grounds)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var ground in grounds)
        {
            Vector3 pos = ground.transform.position;
            Vector3 scale = ground.transform.localScale;
            
            // Account for object scale
            float halfWidth = scale.x * 0.5f;
            float halfHeight = scale.y * 0.5f;
            
            minX = Mathf.Min(minX, pos.x - halfWidth);
            maxX = Mathf.Max(maxX, pos.x + halfWidth);
            minY = Mathf.Min(minY, pos.y - halfHeight);
            maxY = Mathf.Max(maxY, pos.y + halfHeight);
        }

        cameraController.SetBoundaries(
            minX - boundaryMargin,
            maxX + boundaryMargin,
            maxY + boundaryMargin,
            minY - boundaryMargin
        );
    }

    void CalculateFromPlatformObjects(GameObject[] platforms)
    {
        CalculateFromGroundObjects(platforms); // Same logic as ground objects
    }

    // Public method to manually set boundaries
    public void SetManualBoundaries(float left, float right, float top, float bottom)
    {
        leftBound = left;
        rightBound = right;
        topBound = top;
        bottomBound = bottom;
        
        if (cameraController != null)
        {
            cameraController.SetBoundaries(left, right, top, bottom);
        }
    }

    // Method to create boundary objects at scene edges
    [ContextMenu("Create Boundary Objects")]
    public void CreateBoundaryObjects()
    {
        // Create left boundary
        GameObject leftBoundary = new GameObject("LeftBoundary");
        leftBoundary.transform.position = new Vector3(leftBound, 0, 0);
        leftBoundary.tag = "Boundary";
        var leftScript = leftBoundary.AddComponent<LevelBoundary>();
        // Note: You'd need to set the boundary type in the inspector

        // Create right boundary
        GameObject rightBoundary = new GameObject("RightBoundary");
        rightBoundary.transform.position = new Vector3(rightBound, 0, 0);
        rightBoundary.tag = "Boundary";
        var rightScript = rightBoundary.AddComponent<LevelBoundary>();

        // Create top boundary
        GameObject topBoundary = new GameObject("TopBoundary");
        topBoundary.transform.position = new Vector3(0, topBound, 0);
        topBoundary.tag = "Boundary";
        var topScript = topBoundary.AddComponent<LevelBoundary>();

        // Create bottom boundary
        GameObject bottomBoundary = new GameObject("BottomBoundary");
        bottomBoundary.transform.position = new Vector3(0, bottomBound, 0);
        bottomBoundary.tag = "Boundary";
        var bottomScript = bottomBoundary.AddComponent<LevelBoundary>();

        Debug.Log("Created boundary objects. Set their boundary types in the inspector.");
    }
}
