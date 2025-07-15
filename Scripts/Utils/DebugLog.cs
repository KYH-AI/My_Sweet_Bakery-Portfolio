using UnityEngine;

public class DebugLog 
{
    public static void CustomLog(string log, Color logColor)
    {
        var hexColor = ColorUtility.ToHtmlStringRGB(logColor);
        Debug.Log($"<color=#{hexColor}>{log}</color>");
    }
}
