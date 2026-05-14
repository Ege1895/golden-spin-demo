using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace GoldenSpin
{
    public sealed class ScreenShotManager : MonoBehaviour
    {
        private const string SCREENSHOT_FOLDER_NAME = "Screenshots";
        private const KeyCode SCREENSHOT_KEY = KeyCode.S;

        private void Update()
        {
            if (Input.GetKeyDown(SCREENSHOT_KEY))
            {
                TakeScreenshot();
            }
        }

        private void TakeScreenshot()
        {
            StartCoroutine(CaptureScreenshot());
        }

        private IEnumerator CaptureScreenshot()
        {
            yield return new WaitForEndOfFrame();

            string screenshotsDirectoryPath = Path.Combine(Application.dataPath, "..", SCREENSHOT_FOLDER_NAME);

            if (!Directory.Exists(screenshotsDirectoryPath))
            {
                Directory.CreateDirectory(screenshotsDirectoryPath);
            }

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            string aspectRatioText = GetAspectRatioText(screenWidth, screenHeight);

            string fileName =
                $"screenshot_{screenWidth}x{screenHeight}_{aspectRatioText}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

            string fullPath = Path.Combine(screenshotsDirectoryPath, fileName);

            Texture2D texture = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            Destroy(texture);

            File.WriteAllBytes(fullPath, bytes);

            Debug.Log($"Screenshot saved: {fullPath}");
        }
        
        private static readonly (float ratio, string shortSide, string longSide)[] KNOWN_RATIOS =
        {
            (4f / 3f,    "3",  "4"),
            (3f / 2f,    "2",  "3"),
            (16f / 10f,  "10", "16"),
            (16f / 9f,   "9",  "16"),
            (2f / 1f,    "9",  "18"),
            (19f / 9f,   "9",  "19"),
            (19.5f / 9f, "9",  "19"),
            (20f / 9f,   "9",  "20"),
            (21f / 9f,   "9",  "21"),
        };

        private const float ASPECT_RATIO_TOLERANCE = 0.03f;

        private static string GetAspectRatioText(int width, int height)
        {
            int shortPx = Mathf.Min(width, height);
            int longPx = Mathf.Max(width, height);
            float ratio = (float)longPx / shortPx;

            foreach ((float known, string shortSide, string longSide) in KNOWN_RATIOS)
            {
                if (Mathf.Abs(ratio - known) < ASPECT_RATIO_TOLERANCE)
                {
                    return $"{shortSide}-{longSide}";
                }
            }

            return $"{shortPx}-{longPx}";
        }
    }
}
