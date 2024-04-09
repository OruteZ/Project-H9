using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using ExcelDataReader;
using System.Text;

public class CsvConvertor : EditorWindow
{
    private const string START_DIRECTORY_KEY = "EDITOR_CSV_START_KEY";
    private const string DESTINATION_DIRECTORY_KEY = "EDITOR_CSV_DESTINATION_KEY";
    private const string EXEUTED_LOG = "EDITOR_CSV_LOG_KEY";
    private string _startDirectory;
    private string _destinationDirectory;
    private string _executedLog;

    private DirectoryInfo _excelDirectoryInfo;
    private Vector2 _totalScroll;
    private Vector2 _excelFileScroll;
    private Vector2 _recentExecuteScroll;

    [MenuItem("Tools/CsvConverter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CsvConvertor));
    }

    public void OnEnable()
    {
        if (EditorPrefs.HasKey(START_DIRECTORY_KEY))
            _startDirectory = EditorPrefs.GetString(START_DIRECTORY_KEY);
        if (EditorPrefs.HasKey(DESTINATION_DIRECTORY_KEY))
            _destinationDirectory = EditorPrefs.GetString(DESTINATION_DIRECTORY_KEY);
        if (EditorPrefs.HasKey(EXEUTED_LOG))
            _executedLog = EditorPrefs.GetString(EXEUTED_LOG);
        if (!string.IsNullOrEmpty(_startDirectory) && Directory.Exists(_startDirectory))
        {
            OpenDirectory(_startDirectory);
        }
    }

    public void OnGUI()
    {
        Color backupColor = GUI.color;

        _totalScroll = GUILayout.BeginScrollView(_totalScroll);
        GUILayout.Label("CSV Convertor");

        // Input Field
        GUILayout.BeginHorizontal();
        GUILayout.Label("Input ", GUILayout.Width(60));
        _startDirectory = EditorGUILayout.TextField(_startDirectory);
        if (GUILayout.Button("Browse", GUILayout.Width(100)))
        {
            string tmpPath = EditorUtility.OpenFolderPanel("Choose Excel Files Forlder", "", "");
            if (!string.IsNullOrEmpty(tmpPath) && tmpPath != _startDirectory)
            {
                _startDirectory = tmpPath;
                if (OpenDirectory(_startDirectory))
                {
                    Debug.Log($"{_startDirectory}를 레지스터에 저장했습니다.");
                    EditorPrefs.SetString(START_DIRECTORY_KEY, _startDirectory);
                }
            }
        }
        GUILayout.EndHorizontal();
        if (!Directory.Exists(_startDirectory))
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.red;
            GUILayout.Label($"시작 디렉토리를 찾을 수 없습니다.");
            GUI.color = backupColor;
            GUILayout.EndHorizontal();
        }

        // Output Field
        GUILayout.BeginHorizontal();
        GUILayout.Label("Output ", GUILayout.Width(60));
        _destinationDirectory = EditorGUILayout.TextField(_destinationDirectory);
        if (GUILayout.Button("Browse", GUILayout.Width(100)))
        {
            string tmpPath = EditorUtility.OpenFolderPanel("Choose Excel Files Forlder", "", "");
            if (!string.IsNullOrEmpty(tmpPath) && tmpPath != _destinationDirectory)
            {
                _destinationDirectory = tmpPath;
                Debug.Log($"{_destinationDirectory}를 레지스터에 저장했습니다.");
                EditorPrefs.SetString(DESTINATION_DIRECTORY_KEY, _destinationDirectory);
            }
        }
        GUILayout.EndHorizontal();
        if (!Directory.Exists(_destinationDirectory))
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.red;
            GUILayout.Label($"도착 디렉토리를 찾을 수 없습니다.");
            GUI.color = backupColor;
            GUILayout.EndHorizontal();
        }

        // Execute
        if (!IsValidDirectoryInfo(_excelDirectoryInfo))
            OpenDirectory(_startDirectory);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Convert", GUILayout.Width(150)))
        {
            var files = _excelDirectoryInfo.GetFiles();
            Debug.Log($"Converting {files.Length}개 후보");
            var count = 0;
            const string DOUBLE_QUOTES = "\"";

            foreach (var file in files)
            {
                if (file.Extension == ".xlsx")
                {
                    var streamer = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

                    using (var reader = ExcelReaderFactory.CreateReader(streamer))
                    {
                        #region 주석: 모든 시트 혹은 시트 이름 참조가 필요한 경우
                        /*
                        var tables = reader.AsDataSet().Tables;
                        for (var sheetIndex = 0; sheetIndex < tables.Count; sheetIndex++)
                        {
                            var sheet = tables[sheetIndex];

                            //시트 이름 필터링 가능
                            Debug.Log($"Sheet[{sheetIndex}] Name: {sheet.TableName}");

                            for (var rowIndex = 0; rowIndex < sheet.Rows.Count; rowIndex++)
                            {
                                // 행 가져오기
                                var slot = sheet.Rows[rowIndex];
                                for (var columnIndex = 0; columnIndex < slot.ItemArray.Length; columnIndex++)
                                {
                                    var item = slot.ItemArray[columnIndex];
                                    // 열 가져오기
                                    Debug.Log($"slot[{rowIndex}][{columnIndex}] : {item}");
                                }
                            }
                        }
                        */
                        #endregion

                        StringBuilder sb = new StringBuilder();
                        var newName = $"{_destinationDirectory}/{file.Name.Replace(".xlsx", ".csv")}";

                        var table = reader.AsDataSet().Tables[0];
                        for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                        {
                            // 행 가져오기
                            var slot = table.Rows[rowIndex];
                            string[] line = new string[slot.ItemArray.Length];
                            for (var columnIndex = 0; columnIndex < slot.ItemArray.Length; columnIndex++)
                            {
                                var item = slot.ItemArray[columnIndex];
                                var stringedItem = item.ToString();
                                if (stringedItem.Contains(","))
                                    stringedItem = DOUBLE_QUOTES + stringedItem + DOUBLE_QUOTES;

                                line[columnIndex] = stringedItem;
                            }
                            sb.AppendLine(string.Join(',', line));
                        }

                        bool isAppend = false; // true일시 파일에 데이터 추가, false일 시 파일에 덮어쓰기 
                        StreamWriter outStream = new StreamWriter(newName, isAppend, Encoding.UTF8);
                        outStream.Write(sb);
                        outStream.Close();

                        reader.Dispose();
                        reader.Close();
                        count++;
                    }
                }
            }

            if (0 < count)
            {
                string time = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss"));
                _executedLog += $"{time}@:{_startDirectory}@:{_destinationDirectory}@:" ;
                EditorPrefs.SetString(EXEUTED_LOG, _executedLog);
                Debug.Log($"{count}/{files.Length} 개 변환 완료");
            }
        }
        GUILayout.EndHorizontal();

        // Files
        if (IsValidDirectoryInfo(_excelDirectoryInfo))
        {
            _excelFileScroll = GUILayout.BeginScrollView(_excelFileScroll);
            GUILayout.Label($"{_excelDirectoryInfo.Name}");
            string fileName = "";
            var files = _excelDirectoryInfo.GetFiles();
            foreach (var file in files)
            {
                fileName += $"{file.Name}\n";
            }
            EditorGUILayout.TextArea(fileName);
            GUILayout.EndScrollView(); // _excelFileScroll
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{files.Length} 개의 파일이 존재합니다.");
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.TextArea($"시작 디렉토리의 정보를 불러오는데 실패했습니다.");
        }


        // Recents logs
        if (!string.IsNullOrEmpty(_executedLog))
        {
            GUILayout.BeginHorizontal();
            _recentExecuteScroll = GUILayout.BeginScrollView(_recentExecuteScroll);
            GUILayout.Label($"최신 기록");
            string[] logs = _executedLog.Split("@:");
            int length = logs.Length / 3;
            for (int i = 0; i < length; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Load", GUILayout.Width(80)))
                {
                    _startDirectory = logs[i * 3 + 1];
                    _destinationDirectory = logs[i * 3 + 2];
                }
                GUILayout.Label($"{logs[i * 3 + 0]} ", GUILayout.Width(140));
                GUILayout.Label($"{logs[i * 3 + 1]} => {logs[i * 3 + 2]}");
                OpenDirectory(logs[i * 3 + 1]);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("초기화", GUILayout.Width(200)))
            {
                _executedLog = "";
                EditorPrefs.SetString(EXEUTED_LOG, _executedLog);
            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView(); // _recentExecuteScroll
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView(); // _totalScroll
    }

    private bool IsValidDirectoryInfo(in DirectoryInfo directoryInfo)
    {
        if (directoryInfo == null)
        {
            return false;
        }
        if (!directoryInfo.Exists)
        {
            return false;
        }

        return true;
    }

    private bool OpenDirectory(in string path)
    {
        _excelDirectoryInfo = new DirectoryInfo(path);
        if (!IsValidDirectoryInfo(_excelDirectoryInfo))
            return false;

        return true;
    }

}
