using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LerpCalculation
{
    public static bool CalculateLerpValue(ref float currentValue, float targetValue, float speed = 1, float threshold = 0.01f)
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
    public static bool CalculateLerpValue(ref Vector3 currentValue, Vector3 targetValue, float speed = 1, float threshold = 0.01f)
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
    public static bool CalculateLerpValue(ref Quaternion currentValue, Quaternion targetValue, float speed = 1, float threshold = 0.01f)
    {

        if (Quaternion.Angle(currentValue, targetValue) < threshold)
        {
            currentValue = targetValue;
            return false;
        }
        else
        {
            currentValue = Quaternion.Lerp(currentValue, targetValue, Time.deltaTime * speed);
            return true;
        }
    }
    public static bool CalculateSLerpValue(ref Vector3 currentValue, Vector3 targetValue, float speed = 1, float threshold = 0.01f)
    {

        if (Vector3.Distance(currentValue, targetValue) < threshold)
        {
            currentValue = targetValue;
            return false;
        }
        else
        {
            currentValue = Vector3.Slerp(currentValue, targetValue, Time.deltaTime * speed);
            return true;
        }
    }
}