using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCalculation
{
    public static bool CalculationLerpValue(ref float currentValue, float targetValue)
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
    public static bool CalculationLerpValue(ref float currentValue, float targetValue, float speed)
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
    public static bool CalculationLerpValue(ref float currentValue, float targetValue, float speed, float threshold)
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
}
