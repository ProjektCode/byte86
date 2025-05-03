using UnityEngine;

public class ResolutionManager : MonoBehaviour {
    void Start() {
        int targetWidth = 900;
        int targetHeight = 1200;
        float targetAspect = (float)targetWidth / targetHeight;

        int screenWidth = Display.main.systemWidth;
        int screenHeight = Display.main.systemHeight;

        int margin = 100; // leave room for taskbar, window borders, etc.

        // Check if screen is too small
        if (screenWidth < targetWidth + margin || screenHeight < targetHeight + margin) {
            int availableHeight = screenHeight - margin;
            int availableWidth = screenWidth - margin;

            // Fit height first, then clamp width
            int newHeight = availableHeight;
            int newWidth = Mathf.RoundToInt(newHeight * targetAspect);

            if (newWidth > availableWidth) {
                newWidth = availableWidth;
                newHeight = Mathf.RoundToInt(newWidth / targetAspect);
            }

            Screen.SetResolution(newWidth, newHeight, false);
        } else {
            Screen.SetResolution(targetWidth, targetHeight, false);
        }
    }
}
