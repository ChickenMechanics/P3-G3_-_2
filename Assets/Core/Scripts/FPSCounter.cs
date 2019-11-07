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
        int width = Screen.width, height = Screen.height;
        var rect = new Rect(0, 0, width, height * 2 / 100);

        var fps = 1.0f / m_DeltaTime;
        var text = $"{fps:0.} fps";

        var style = new GUIStyle
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = height * 2 / 50,
            normal = {textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)}
        };




        GUI.Label(rect, text, style);
    }
}