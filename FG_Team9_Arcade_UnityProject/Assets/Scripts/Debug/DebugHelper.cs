#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class DebugHelper
{
    
    // Gizmos
    public static void DrawNormalGizmo(Vector3 normal, Vector3 point)
    {
        DrawLine(point, point + normal, Color.green);
        DrawTextGizmo(point + normal, "normal");
    }

    public static void DrawNormalGizmo(Vector3 normal, Vector3 point, Color color)
    {
        DrawLine( point, point + normal, color);
        DrawTextGizmo(point + normal, "normal");
    }
    
    public static void DrawTextGizmo(Vector3 position, string text)
    {
        GUIContent guiText = new GUIContent(text);
        Handles.Label(position, guiText);
    }
    
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);
    }

    public static void DrawLine(Vector3 start, Vector3 end)
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);
    }
}
#endif
