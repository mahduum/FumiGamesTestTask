using UnityEngine;
using UnityEngine.UIElements;

public class UI_InputMapper : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private void Awake()
    {
        _uiDocument.panelSettings.SetScreenToPanelSpaceFunction(screenPos =>
        {
            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, 500f, LayerMask.GetMask("UI")))
            {
                Debug.DrawRay(cameraRay.origin, cameraRay.direction * 500, Color.magenta);
                return new Vector2(float.NaN, float.NaN);
            }

            Debug.DrawLine(cameraRay.origin, hit.point, Color.green);
            
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.y = 1 - pixelUV.y;
            pixelUV.y *= _uiDocument.panelSettings.targetTexture.height;
            pixelUV.x *= _uiDocument.panelSettings.targetTexture.width;

            var cursor = _uiDocument.rootVisualElement.Q<VisualElement>("Cursor");
            var button = _uiDocument.rootVisualElement.Q<Button>("OK");

            if (cursor != null)
            {
                cursor.style.left = pixelUV.x;
                cursor.style.top = pixelUV.y;
                
            }
            else
            {
                Debug.LogError("Cursor not found!");
            }

            if (button != null)
            {
                button.CaptureMouse();//this is necessary otherwise won't register clicks
                if (button.localBound.Contains(pixelUV))
                {
                    Debug.Log("Mouse hovering over button!");
                }
            }
            
            return pixelUV;
        });

        var button = _uiDocument.rootVisualElement.Q<Button>("OK");
        if (button != null)
        {
            button.clicked += () => Debug.Log("OK button clicked on the texture.");
        }
        else
        {
            Debug.LogError("Button not found!");
        }
            
    }
}
