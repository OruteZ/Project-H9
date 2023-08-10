using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using System.Text.RegularExpressions;

public class FileRead : MonoBehaviour
{
	static readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; //regular expression
	static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static readonly char[] TRIM_CHARS = { ' ' };

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
	
	public static int[] ConvertStringToArray(string input)
	{
		// 문자열에서 "["와 "]"를 제거하고, 쉼표로 분리하여 숫자 부분만 가져옵니다.
		string[] numbersAsString = input.Trim('\"','\"').Split(',');

		// 결과로 반환할 정수 배열을 초기화합니다.
		int[] result = new int[numbersAsString.Length];

		// 숫자를 정수로 변환하여 결과 배열에 저장합니다.
		for (int i = 0; i < numbersAsString.Length; i++)
		{
			result[i] = int.Parse(numbersAsString[i]);
		}

		return result;
	}
}
