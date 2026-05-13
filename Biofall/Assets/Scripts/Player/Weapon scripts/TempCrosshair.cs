using UnityEngine;

public class TempCrosshair : MonoBehaviour
{
    [SerializeField] CameraOrbit cameraOrbit;
    [SerializeField] float size = 10f;
    [SerializeField] float gap = 5f;
    [SerializeField] Color color = Color.white;

    Texture2D dot;

    void Start()
    {
        dot = new Texture2D(1, 1);
        dot.SetPixel(0, 0, color);
        dot.Apply();
    }

    void OnGUI()
    {
        if (cameraOrbit == null || !cameraOrbit.isAiming) return;

        float cx = Screen.width / 2f;
        float cy = Screen.height / 2f;

        GUI.DrawTexture(new Rect(cx - size / 2, cy - gap - size, size, size), dot);
        GUI.DrawTexture(new Rect(cx - size / 2, cy + gap, size, size), dot);
        GUI.DrawTexture(new Rect(cx - gap - size, cy - size / 2, size, size), dot);
        GUI.DrawTexture(new Rect(cx + gap, cy - size / 2, size, size), dot);
    }
}
