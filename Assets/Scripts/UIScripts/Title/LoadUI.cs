using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadUI : MonoBehaviour
{
    [SerializeReference]
    private RectTransform _panel;
    [SerializeReference]
    private GameObject _loadFilePrefab;

    [SerializeField]
    private TitleUI _titleUI;

    private List<UserData> _lists = new List<UserData>();

    public void Awake()
    {
        ShowFiles();
    }

    public void CreateFile()
    {
        UserDataFileSystem.New(out var userData);
        ShowFiles();
    }

    public void ShowFiles()
    {
        for (int i = _panel.childCount - 1 ; 0 <= i ; i--)
        {
            Destroy(_panel.GetChild(i).gameObject);
        }

        DirectoryInfo di = new DirectoryInfo(UserDataFileSystem.DefaultPath);
        foreach (FileInfo file in di.GetFiles())
        {
            if (IsCorrectSaveFile(file))
            {
                if (UserDataFileSystem.Load(out var userData, file.FullName))
                {
                    _lists.Add(userData);
                    var ins = GameObject.Instantiate(_loadFilePrefab);
                    ins.transform.parent = _panel;
                    ins.GetComponentInChildren<TMPro.TMP_Text>().text = file.Name;

                    ins.GetComponentInChildren<Button>().onClick.AddListener(
                        () => {_titleUI.OnClickLoadBtn(userData);  });
                }
            }
        }
    }

    public void DeleteAll()
    {
        DirectoryInfo di = new DirectoryInfo(UserDataFileSystem.DefaultPath);
        var files = di.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            if (IsCorrectSaveFile(file))
            {
                file.Delete();
            }
        }
        ShowFiles();
    }

    private bool IsCorrectSaveFile(FileInfo file)
    {
        if (file.Extension != ".json")
            return false;
        return true;
    }
    
}
