using UnityEngine;

public class PointsCanvasBehaviour : MonoBehaviour
{
    public Camera trackingCamera;

    public RectTransform redPoint;
    public RectTransform greenPoint;
    public RectTransform yellowPoint;
    public RectTransform bluePoint;
    public RectTransform redIntensePoint;
    public RectTransform greenIntensePoint;
    public RectTransform yellowIntensePoint;
    public RectTransform blueIntensePoint;

    public Collider redScenePoint;
    public Collider greenScenePoint;
    public Collider yellowScenePoint;
    public Collider blueScenePoint;

    public CalibrationPlaneBehaviour calibrationPlane;

    public bool IsMatching
    {
        get;
        private set;
    }

	private void Update()
    {
        // Find the points in the scene that are currently overlaid by the ones on the screen
        bool red = PointMatches(redPoint, redScenePoint);
        bool green = PointMatches(greenPoint, greenScenePoint);
        bool yellow = PointMatches(yellowPoint, yellowScenePoint);
        bool blue = PointMatches(bluePoint, blueScenePoint);

        // Highlight the matching points by intensifying the respective color
        redIntensePoint.gameObject.SetActive(red);
        greenIntensePoint.gameObject.SetActive(green);
        yellowIntensePoint.gameObject.SetActive(yellow);
        blueIntensePoint.gameObject.SetActive(blue);

        IsMatching = red && green && yellow && blue;
    }

    private bool PointMatches(RectTransform screenPoint, Collider scenePoint)
    {
        float x = screenPoint.position.x;
        float y = screenPoint.position.y;
        float width = screenPoint.rect.width;
        float height = screenPoint.rect.height;
        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;

        var rays = new Ray[]
        {
            trackingCamera.ScreenPointToRay(new Vector3(x, y)),
            trackingCamera.ScreenPointToRay(new Vector3(x + width, y)),
            trackingCamera.ScreenPointToRay(new Vector3(x, y - height)),
            trackingCamera.ScreenPointToRay(new Vector3(x + width, y - height)),

            trackingCamera.ScreenPointToRay(new Vector3(x + halfWidth, y - halfHeight)),

            trackingCamera.ScreenPointToRay(new Vector3(x + halfWidth, y)),
            trackingCamera.ScreenPointToRay(new Vector3(x, y - halfHeight)),        
            trackingCamera.ScreenPointToRay(new Vector3(x + width, y - halfHeight)),
            trackingCamera.ScreenPointToRay(new Vector3(x + halfWidth, y - height))
        };

        foreach (var ray in rays)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider == scenePoint)
            {
                return true;
            }
        }

        return false;
    }

    public void Fit(float calibrationPlaneWidth, float calibrationPlaneHeight)
    {
        var rectTransform = GetComponent<RectTransform>();

        float pointSize = ((rectTransform.rect.width + rectTransform.rect.height) / 2.0f) * 0.06f;
        float intensePointSize = pointSize * 1.2f;

        redPoint.sizeDelta = new Vector2(pointSize, pointSize);
        greenPoint.sizeDelta = new Vector2(pointSize, pointSize);
        yellowPoint.sizeDelta = new Vector2(pointSize, pointSize);
        bluePoint.sizeDelta = new Vector2(pointSize, pointSize);

        redIntensePoint.sizeDelta = new Vector2(intensePointSize, intensePointSize);
        greenIntensePoint.sizeDelta = new Vector2(intensePointSize, intensePointSize);
        yellowIntensePoint.sizeDelta = new Vector2(intensePointSize, intensePointSize);
        blueIntensePoint.sizeDelta = new Vector2(intensePointSize, intensePointSize);

        // To fit the whole buttons into the canvas, a smaller area has to be used for the calculations.
        Vector2 size = new Vector2(
            rectTransform.rect.size.x
            - (Mathf.Max(redIntensePoint.rect.width, blueIntensePoint.rect.width) / 2.0f)
            - (Mathf.Max(greenIntensePoint.rect.width, yellowIntensePoint.rect.width) / 2.0f),
            rectTransform.rect.size.y
            - (Mathf.Max(redIntensePoint.rect.height, greenIntensePoint.rect.height) / 2.0f)
            - (Mathf.Max(blueIntensePoint.rect.height, yellowIntensePoint.rect.height) / 2.0f));

        // Calculate the aspect ratio of the calibration plane
        Vector2 aspectRatio;
        if (calibrationPlaneHeight < calibrationPlaneWidth)
        {
            aspectRatio = new Vector2(
                1.0f,
                calibrationPlaneHeight / calibrationPlaneWidth);
        }
        else
        {
            aspectRatio = new Vector2(
                calibrationPlaneWidth / calibrationPlaneHeight,
                1.0f);
        }

        // Calculate the dimensions of the area the points will be in.
        Vector2 pointsAreaSize;
        if (aspectRatio.y * size.x <= size.y)
        {
            pointsAreaSize = new Vector2(
                size.x * aspectRatio.x,
                size.x * aspectRatio.y);
        }
        else
        {
            pointsAreaSize = new Vector2(
                size.y * aspectRatio.x,
                size.y * aspectRatio.y);
        }

        // Calculate the space that is left horizontally and vertically on each side outside the points area.
        Vector2 margins = new Vector2(
            (rectTransform.rect.size.x - pointsAreaSize.x) / 2.0f,
            (rectTransform.rect.size.y - pointsAreaSize.y) / 2.0f);

        var redPointPosition = new Vector2(margins.x, -margins.y);
        var greenPointPosition = new Vector2(margins.x + pointsAreaSize.x, -margins.y);
        var yellowPointPosition = new Vector2(margins.x + pointsAreaSize.x, -margins.y - pointsAreaSize.y);
        var bluePointPosition = new Vector2(margins.x, -margins.y - pointsAreaSize.y);
        
        redPoint.anchoredPosition = CenterPosition(redPointPosition, redPoint.rect.size);
        greenPoint.anchoredPosition = CenterPosition(greenPointPosition, greenPoint.rect.size);
        yellowPoint.anchoredPosition = CenterPosition(yellowPointPosition, yellowPoint.rect.size);
        bluePoint.anchoredPosition = CenterPosition(bluePointPosition, bluePoint.rect.size);

        redIntensePoint.anchoredPosition = CenterPosition(redPointPosition, redIntensePoint.rect.size);
        greenIntensePoint.anchoredPosition = CenterPosition(greenPointPosition, greenIntensePoint.rect.size);
        yellowIntensePoint.anchoredPosition = CenterPosition(yellowPointPosition, yellowIntensePoint.rect.size);
        blueIntensePoint.anchoredPosition = CenterPosition(bluePointPosition, blueIntensePoint.rect.size);
    }

    private Vector2 CenterPosition(Vector2 position, Vector2 size)
    {
        return new Vector2(position.x - (size.x / 2.0f), position.y + (size.y / 2.0f));
    }
}
