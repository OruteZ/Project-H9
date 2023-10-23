using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCalculation
{
    public static bool CalculateLerpValue(ref float currentValue, float targetValue)
    {
        float threshold = 0.01f;
        if (Mathf.Abs(currentValue - targetValue) < threshold)
        {
            currentValue = targetValue;
            return false;
        }
        else
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime);
            return true;
        }
    }
    public static bool CalculateLerpValue(ref float currentValue, float targetValue, float speed)
    {
        float threshold = 0.01f;
        if (Mathf.Abs(currentValue - targetValue) < threshold)
        {
            currentValue = targetValue;
            return false;
        }
        else
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * speed);
            return true;
        }
    }
    public static bool CalculateLerpValue(ref float currentValue, float targetValue, float speed, float threshold)
    {
        if (Mathf.Abs(currentValue - targetValue) < threshold)
        {
            currentValue = targetValue;
            return false;
        }
        else
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * speed);
            return true;
        }
    }
    public static bool CalculateLerpValue(ref Vector3 currentValue, Vector3 targetValue, float speed, float threshold)
    {
        
        if (Vector3.Distance(currentValue, targetValue) < threshold)
        {
            currentValue = targetValue;
            return false;
        }
        else
        {
            currentValue = Vector3.Lerp(currentValue, targetValue, Time.deltaTime * speed);
            return true;
        }
    }
}