using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor.Rendering;

/// <summary>
/// CSV확장자로 된 각종 테이블 파일을 읽어서 string이중리스트로 반환하는 클래스
/// </summary>
public static class FileRead
{
	static readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; //regular expression
	static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static readonly char[] TRIM_CHARS = { ' ' };

    /// <summary>
    /// CSV 파일을 읽어서 문자열 이중리스트로 반환합니다.
    /// </summary>
    /// <param name="file"> 파일 이름 </param>
    /// <returns> 문자열 이중리스트로 변환된 파일의 내용 </returns>
	public static List<List<string>> Read(string file)
	{
		TextAsset data = Resources.Load("files/" + file) as TextAsset;
        if (data == null)
        {
            Debug.Log("파일이 존재하지 않습니다.");
            return null;
        }

        List<List<string>> rowList = new List<List<string>>();
		string[] lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
        {
            Debug.Log("파일 내용이 존재하지 않습니다.");
            return null;
        }

        //string[] header = Regex.Split(lines[0], SPLIT_RE);    //for test
		//첫 줄은 value 설명 로우로 취급하여 생략.
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            List<string> columnList = new List<string>();
            for (int j = 0; j < values.Length; j++)
            {
                string value = values[j];
                //공백 제거
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS);
                string finalvalue = value;
                columnList.Add(finalvalue);
            }
            rowList.Add(columnList);
        }
        return rowList;
	}

    public static T[] ConvertStringToArray<T>(string input)
    {
	    if (input == "") return Array.Empty<T>();
	    
	    T type = default;
	    if ((type is int or float) is false)
	    {
		    Debug.LogError("Cant String to array with type" + typeof(T));
		    return null;
	    }
	    
	    // 문자열에서 "를 제거하고, 쉼표로 분리하여 숫자 부분만 가져옵니다.
	    string[] numbersAsString = input.Trim('\"', '\"').Split(',');

	    // 결과로 반환할 배열을 초기화합니다.
	    T[] result = new T[numbersAsString.Length];
	    if(numbersAsString.Length == 0) return result;

	    // 변환하여 결과 배열에 저장합니다.
	    for (int i = 0; i < numbersAsString.Length; i++)
	    {
		    if (result is int[] ints)
		    {
			    ints[i] = int.Parse(numbersAsString[i]);
		    }

		    if (result is float[] floats)
		    {
			    floats[i] = float.Parse(numbersAsString[i]);
		    }
	    }

	    return result;
    }
}
