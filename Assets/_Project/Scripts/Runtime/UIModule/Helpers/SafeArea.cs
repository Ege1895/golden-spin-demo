using UnityEngine;

namespace GoldenSpin.UIModule
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private RectTransform rectTransform;
        
        private Rect lastSafeArea;
        
        private ScreenOrientation lastOrientation;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            lastOrientation = Screen.orientation;
            Apply();
        }

        private void Update()
        {
            if (Screen.safeArea != lastSafeArea || Screen.orientation != lastOrientation)
            {
                lastOrientation = Screen.orientation;
                Apply();
            }
        }

        private void Apply()
        {
            lastSafeArea = Screen.safeArea;

            Vector2 anchorMin = lastSafeArea.position;
            Vector2 anchorMax = lastSafeArea.position + lastSafeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
