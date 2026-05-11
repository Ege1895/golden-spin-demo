#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RaycastTargetCleaner : EditorWindow
{
    [MenuItem("Tools/GoldenSpin/Check RaycastTargets")]
    public static void CheckRaycastTargets()
    {
        Image[] images = FindObjectsOfType<Image>();
        int warningCount = 0;

        foreach (Image img in images)
        {
            if (img.raycastTarget && img.GetComponent<Button>() == null)
            {
                Debug.LogWarning($"[RaycastTarget] {img.gameObject.name} — consider disabling", img.gameObject);
                warningCount++;
            }
        }

        Debug.Log($"Checked {images.Length} images. {warningCount} potential issues found.");
    }
}
#endif
