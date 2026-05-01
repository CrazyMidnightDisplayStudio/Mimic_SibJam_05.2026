using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxMouseCamera : MonoBehaviour
{
    [SerializeField] private Transform firstLayer;
    [SerializeField] private Transform secondLayer;
    [SerializeField] private Transform thirdLayer;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float cameraOffsetX = 0.2f;
    [SerializeField] private float cameraOffsetY = 0.2f;
    [SerializeField] private float cameraSmoothSpeed = 5f;
    [SerializeField] private float firstLayerMultiplier = 0.2f;
    [SerializeField] private float secondLayerMultiplier = 0.5f;
    [SerializeField] private float thirdLayerMultiplier = 0.8f;

    private Vector3 _cameraStartPosition;
    private Vector3 _firstLayerStartPosition;
    private Vector3 _secondLayerStartPosition;
    private Vector3 _thirdLayerStartPosition;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        _cameraStartPosition = transform.position;

        if (firstLayer != null)
        {
            _firstLayerStartPosition = firstLayer.position;
        }

        if (secondLayer != null)
        {
            _secondLayerStartPosition = secondLayer.position;
        }

        if (thirdLayer != null)
        {
            _thirdLayerStartPosition = thirdLayer.position;
        }
    }

    private void Update()
    {
        if (targetCamera == null || Mouse.current == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float normalizedX = mousePosition.x / Screen.width - 0.5f;
        float normalizedY = mousePosition.y / Screen.height - 0.5f;

        Vector3 cameraOffset = new Vector3(normalizedX * cameraOffsetX, normalizedY * cameraOffsetY, 0f);
        Vector3 targetPosition = _cameraStartPosition + cameraOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            cameraSmoothSpeed * Time.deltaTime);

        UpdateLayer(firstLayer, _firstLayerStartPosition, cameraOffset, firstLayerMultiplier);
        UpdateLayer(secondLayer, _secondLayerStartPosition, cameraOffset, secondLayerMultiplier);
        UpdateLayer(thirdLayer, _thirdLayerStartPosition, cameraOffset, thirdLayerMultiplier);
    }

    private void UpdateLayer(Transform layer, Vector3 startPosition, Vector3 cameraOffset, float multiplier)
    {
        if (layer == null)
        {
            return;
        }

        Vector3 targetPosition = startPosition + cameraOffset * multiplier;

        layer.position = Vector3.Lerp(
            layer.position,
            targetPosition,
            cameraSmoothSpeed * Time.deltaTime);
    }
}
