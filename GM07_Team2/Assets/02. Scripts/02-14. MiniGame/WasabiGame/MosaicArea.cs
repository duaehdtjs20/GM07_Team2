using UnityEngine;
using UnityEngine.UI;

public class MosaicArea : MonoBehaviour
{
    [SerializeField]
    private RectTransform _captureArea;
    [SerializeField]
    private RawImage _rawImage;
    [SerializeField]
    private Canvas _canvas;

    private readonly Vector3[] _corners = new Vector3[4];

    private void LateUpdate()
    {
        RefreshUVRect();
    }

    public void RefreshUVRect()
    {
        if (_captureArea == null || _rawImage == null) return;

        _captureArea.GetWorldCorners(_corners);

        Camera camera = null;
        if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            camera = _canvas.worldCamera;
        }

        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(camera, _corners[0]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(camera, _corners[2]);

        float x = bottomLeft.x / Screen.width;
        float y = bottomLeft.y / Screen.height;
        float width = (topRight.x - bottomLeft.x) / Screen.width;
        float height = (topRight.y - bottomLeft.y) / Screen.height;

        _rawImage.uvRect = new Rect(x, y, width, height);

    }
}
