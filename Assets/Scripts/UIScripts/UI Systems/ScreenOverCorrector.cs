using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenOverCorrector
{
    public static Vector3 GetCorrectedUIPosition(Canvas canvas, Vector3 pos, Vector3 size, Vector3 pivot) 
    {
        Vector3 result = pos;

        Vector2 canvasSize = canvas.gameObject.GetComponent<RectTransform>().sizeDelta;
        Vector2 windowSize = size;
        Vector2 windowPivot = pivot;
        float[] screenLimit = { canvasSize.x, 0, 
                                0, canvasSize.y };  //EWSN
        float[] uiSizeLimit = { windowSize.x * (1 - windowPivot.x),  windowSize.x * -windowPivot.x, 
                                windowSize.y * - windowPivot.y,      windowSize.y * (1 - windowPivot.y) };

        if (result.x + uiSizeLimit[0] > screenLimit[0]) result.x = screenLimit[0] - uiSizeLimit[0]; //E
        if (result.x + uiSizeLimit[1] < screenLimit[1]) result.x = screenLimit[1] - uiSizeLimit[1]; //W
        if (result.y + uiSizeLimit[2] < screenLimit[2]) result.y = screenLimit[2] - uiSizeLimit[2]; //S
        if (result.y + uiSizeLimit[3] > screenLimit[3]) result.y = screenLimit[3] - uiSizeLimit[3]; //N

        return result;
    }
    public static Vector3 GetCorrectedUIPosition(Canvas canvas, GameObject ui) 
    {
        return GetCorrectedUIPosition(canvas, ui.GetComponent<RectTransform>().position, ui.GetComponent<RectTransform>().sizeDelta, ui.GetComponent<RectTransform>().pivot);
    }

    public static Vector3 GetCorrectedUIPositionWithoutConsideringUISize(Canvas canvas, Vector3 pos, Vector3 size, Vector3 pivot)
    {
        Vector3 result = pos;

        Vector2 canvasSize = canvas.gameObject.GetComponent<RectTransform>().sizeDelta - new Vector2(5, 5);
        Vector2 windowSize = size;
        Vector2 windowPivot = pivot;
        float[] screenLimit = { canvasSize.x, 0,
                                0, canvasSize.y };  //EWSN
        float[] uiSizeLimit = { windowSize.x * (1 - windowPivot.x),  windowSize.x * -windowPivot.x,
                                windowSize.y * - windowPivot.y,      windowSize.y * (1 - windowPivot.y) };

        if (result.x + uiSizeLimit[0] > screenLimit[0]) result.x = screenLimit[0]; //E
        if (result.x + uiSizeLimit[1] < screenLimit[1]) result.x = screenLimit[1]; //W
        if (result.y + uiSizeLimit[2] < screenLimit[2]) result.y = screenLimit[2]; //S
        if (result.y + uiSizeLimit[3] > screenLimit[3]) result.y = screenLimit[3]; //N
                
        return result;
    }
}
