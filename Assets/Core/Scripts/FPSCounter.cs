#if DEBUG
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float m_DeltaTime;

    private void Update()
    {
        m_DeltaTime += (Time.unscaledDeltaTime - m_DeltaTime) * 0.1f;
    }

    void OnGUI()
    {
        var textSize = Screen.height * 2 / 50;
        var rect = new Rect(0, 0, Screen.width, textSize);

        var fps = 1.0f / m_DeltaTime;
        var text = $"{fps:0.} fps";

        var style = new GUIStyle
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = textSize,
            normal = {textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)}
        };

        GUI.Label(rect, text, style);
    }
}
#endif