#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RaycastTargetCleaner : EditorWindow
{
    private const string LOG_WARNING_FORMAT = "[RaycastTarget] {0} — consider disabling";
    private const string LOG_RESULT_FORMAT  = "Checked {0} images. {1} potential issues found.";

    [MenuItem("Tools/GoldenSpin/Check RaycastTargets")]
    public static void CheckRaycastTargets()
    {
        Image[] images = FindObjectsOfType<Image>(false);
        int warningCount = 0;

        foreach (Image img in images)
        {
            if (img.raycastTarget && img.GetComponent<Button>() == null)
            {
                Debug.LogWarning(string.Format(LOG_WARNING_FORMAT, img.gameObject.name), img.gameObject);
                warningCount++;
            }
        }

        Debug.Log(string.Format(LOG_RESULT_FORMAT, images.Length, warningCount));
    }
}
#endif
