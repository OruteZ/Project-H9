using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserAccount 
{
    public static ScriptLanguage Language;

    static UserAccount()
    {
        Debug.Log("임시 UserAccount 생성자. 이 생성자는 유저 데이터를 지 멋대로 쳐 바꿔둡니다. 유의.");
        if (Language == ScriptLanguage.NULL) Language = UIManager.instance.scriptLanguage;
        else UIManager.instance.scriptLanguage = Language;
    }

    //public static void Save()
}
