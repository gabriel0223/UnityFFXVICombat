using UnityEngine;

public class WorldCanvasRelativeToCamera : MonoBehaviour
{
    [SerializeField] private Camera _referenceCamera;
    [SerializeField] private float _referenceDistance = 10f;

    private void Update()
    {
        AdjustCanvasScale();
    }

    void AdjustCanvasScale()
    {
        float distanceFromCamera = Vector3.Distance(transform.position, _referenceCamera.transform.position);
        float scalingFactor = distanceFromCamera / _referenceDistance;

        transform.localScale = Vector3.one * scalingFactor;
    }
}
