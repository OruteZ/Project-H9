using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataLoader
{
    private static bool _isReady = false;
    public static bool IsReady => _isReady;

    private static UserData _userData;
    public static UserData Data => _userData;

    public static void New()
    {
        UserDataFileSystem.New(out _userData);
        _isReady = true;
    }
    public static void Stack(UserData userData)
    {
        _userData = userData;
        _isReady = true;
    }

    public static void Clear()
    {
        _userData = null;
        _isReady = false;
    }
}
