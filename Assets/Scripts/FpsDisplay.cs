using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
    [SerializeField]
    private bool _isShowFps = false;

    private float _deltaTime;

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        if (_isShowFps)
        {
            int width = Screen.width;
            int height = Screen.height;
            GUIStyle gUIStyle = new GUIStyle();
            Rect position = new Rect(0f, 0f, width, height * 2 / 100);
            gUIStyle.alignment = TextAnchor.UpperLeft;
            gUIStyle.fontSize = height * 2 / 100;
            gUIStyle.normal.textColor = Color.green;
            float num = _deltaTime * 1000f;
            float num2 = 1f / _deltaTime;
            string text = $"   {num:0.0} ms ({num2:0.} fps)";
            GUI.Label(position, text, gUIStyle);
        }
    }
}
