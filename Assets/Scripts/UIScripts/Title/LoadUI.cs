using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LoadUI : MonoBehaviour
{
    [SerializeReference]
    private RectTransform _slotParent;
    [SerializeReference]
    private GameObject _slotPrefab;

    [SerializeField]
    private TitleUI _titleUI;
    [SerializeField]
    private TMP_Text _exitButtonText;
    [SerializeField]
    private TMP_Text _LoadTitleText;

    private string _slotStr;
    private string _hourStr;
    private string _dayStr;
    private List<UserData> _lists = new List<UserData>();

    public void Awake()
    {
        ShowFiles();
    }

    private void Start()
    {
        //_exitButtonText.text = UIManager.instance.UILocalization[102];
        //_slotStr = UIManager.instance.UILocalization[104];
        //_hourStr = UIManager.instance.UILocalization[105];
        //_dayStr = UIManager.instance.UILocalization[106];
    }

    public void CreateFile()
    {
        UserDataFileSystem.New(out var userData);
        UserDataFileSystem.Save(userData);
        ShowFiles();
    }

    public void ShowFiles()
    {
        for (int i = _slotParent.childCount - 1 ; 0 <= i ; i--)
        {
            Destroy(_slotParent.GetChild(i).gameObject);
        }

        DirectoryInfo di = new DirectoryInfo(UserDataFileSystem.DefaultPath);
        foreach (FileInfo file in di.GetFiles())
        {
            if (IsCorrectSaveFile(file))
            {
                if (UserDataFileSystem.Load(out var userData, file.FullName))
                {
                    _lists.Add(userData);
                    var ins = GameObject.Instantiate(_slotPrefab);
                    ins.transform.parent = _slotParent;
                    var name = file.Name.Split(".")[0];
                    var slotText = $"{_slotStr} {name}\n{userData.Description}";
                    ins.transform.Find("SlotText").GetComponent<TMP_Text>().text = slotText;
                    var dateText = (name.Equals("autosaved")) ? $"autosaved\n{userData.SaveTime}" : $"{userData.SaveTime}";
                    ins.transform.Find("DateText").GetComponent<TMP_Text>().text = dateText;

                    ins.GetComponentInChildren<Button>().onClick.AddListener(
                        () => {_titleUI.OnClickLoadSlot(userData);  });
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
