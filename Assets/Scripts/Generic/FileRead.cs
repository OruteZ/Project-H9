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
	public static List<List<string>> Read(string file, out Dictionary<string, int> columnInfo)
	{
        columnInfo = null;
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

        // 첫 줄인 Column Info
        columnInfo = new Dictionary<string, int>();
        string[] columns = Regex.Split(lines[0], SPLIT_RE);
        for (int i = 0; i < columns.Length; i++)
        {
            if (!columnInfo.TryAdd(columns[i], i))
                Debug.LogError($"ReadError: {file} has two columns at the same name");
        }

		// 두번째 줄의 데이터만 담아서, 반환
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
        Resources.UnloadAsset(data);
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

    // 사용자 언어에 따라 [인덱스, 언어1, 언어2, 언어3 ... ]을 [인덱스, 사용자 언어] 로 반환하는 코드
    public static bool ParseLocalization(in string PATH, out Dictionary<int, string> localizationData)
    {
        localizationData = null;
        var file = FileRead.Read(PATH, out var columnInfo);
        if (file is null)
        {
            Debug.LogError($"There is no LocalizationTable: {PATH}");
            return false;
        }

        int languageIndex = 0;
        switch (UserAccount.Language)
        {
            case ScriptLanguage.Korean:
                languageIndex = columnInfo["kor"];
                break;
            case ScriptLanguage.English:
                languageIndex = columnInfo["eng"];
                break;
            case ScriptLanguage.NULL:
                Debug.LogError("Localization is doing Before Set UserAccount Language.");
                break;
        };

        localizationData = new Dictionary<int, string>();
        for (int i = 0; i < file.Count; i++)
        {
            var line = file[i];

            try
            {
                int index = int.Parse(line[0]);
                string item = line[languageIndex];
                Debug.Log($"{index} {item}");
                localizationData.Add(index, item);
            }
            catch
            {
                string lineSum = "";
                for (int j = 0; j < line.Count; j++)
                    lineSum += $"[{j}] {line[j]}\n";
                Debug.LogError($"Localization Parser error: ({i} line) {lineSum}");
                break;
            }
        }

        return true;
    }
}
